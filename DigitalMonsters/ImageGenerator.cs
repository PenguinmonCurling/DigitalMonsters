using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMonsters
{
    public class ImageGenerator
    {
        public void SaveGeneratedImage(IEnumerable<Digimon> digimonList)
        {
            var bitmap = CombineBitmap(digimonList
                .Where(z => z.Number > 0 && !string.IsNullOrWhiteSpace(z.ImageUrl))
                .OrderBy(y => y.LevelNumber).ThenBy(z => z.Name)
                .Select(x => new DigimonImage { ImageUrl = x.ImageUrl, Name = x.Name }), 12);
            //bitmap.MakeTransparent(Color.White);
            bitmap.Save("DigiPoster.png", ImageFormat.Png);
        }

        private Bitmap CombineBitmap(IEnumerable<DigimonImage> files, int numberPerRow)
        {
            //read all images into memory
            IEnumerable<DigimonImage> images = null;
            Bitmap finalImage = null;

            try
            {
                images = GetImageList(files, numberPerRow);
                var rowHeight = 400;

                //create a bitmap to hold the combined image
                finalImage = new Bitmap(320 * numberPerRow, rowHeight * ((files.Count() / numberPerRow) + (files.Count() % numberPerRow == 0 ? 0 : 1)));
                var numberInRow = 1;
                var rowCount = 0;

                //get a graphics object from the image so we can draw on it
                using (var g = Graphics.FromImage(finalImage))
                {
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        g.FillRectangle(brush, 0, 0, finalImage.Width, finalImage.Height);
                    }
                    //go through each image and draw it on the final image
                    int offset = 0;
                    foreach (var image in images)
                    {
                        g.DrawImage(image.Image,
                          new Rectangle(offset, rowCount * rowHeight, image.Image.Width, image.Image.Height));
                        if (numberInRow < numberPerRow)
                        {
                            numberInRow++;
                            offset += image.Image.Width;
                        }
                        else
                        {
                            numberInRow = 1;
                            offset = 0;
                            rowCount++;
                        }
                    }

                    //go through each image and add text
                    offset = 0;
                    numberInRow = 1;
                    rowCount = 0;
                    foreach (var image in images)
                    {
                        var font = new Font("Arial", 32);
                        var index = 0;
                        foreach (var cutstring in image.Name.Split(' '))
                        {
                            g.DrawString(cutstring, font, Brushes.Black, offset + 160 - (((float)cutstring.Length / 2) * 30), 
                                (rowCount * rowHeight) + 300 + (index  * font.Size));
                            index++;
                        }
                        if (numberInRow < numberPerRow)
                        {
                            numberInRow++;
                            offset += image.Image.Width;
                        }
                        else
                        {
                            numberInRow = 1;
                            offset = 0;
                            rowCount++;
                        }
                    }
                }

                return finalImage;
            }
            catch (Exception ex)
            {
                if (finalImage != null)
                    finalImage.Dispose();

                throw ex;
            }
            finally
            {
                if (images != null)
                {
                    //clean up memory
                    foreach (var image in images)
                    {
                        image.Image.Dispose();
                    }
                }
            }
        }

        private IEnumerable<DigimonImage> GetImageList(IEnumerable<DigimonImage> files, int numberPerRow)
        {
            var images = new List<DigimonImage>();
            var defaultRowHeight = 320;

            foreach (var image in files)
            {
                try
                {
                    using (var webClient = new WebClient())
                    {
                        var data = webClient.DownloadData(image.ImageUrl);

                        using (var mem = new MemoryStream(data))
                        {
                            using (var yourImage = Image.FromStream(mem))
                            {
                                var bitmap = new Bitmap(yourImage);

                                if (bitmap.Width != 320 || bitmap.Height != defaultRowHeight)
                                {
                                    bitmap = ResizeImage(bitmap, 320, defaultRowHeight);
                                }
                                image.Image = bitmap;
                                images.Add(image);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var bitmap = new Bitmap(320, defaultRowHeight);
                    image.Image = bitmap;
                    images.Add(image);
                }
            }
            return images;
        }

        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private class DigimonImage
        {
            public string Name { get; set; }
            public string ImageUrl { get; set; }
            public Bitmap Image { get; set; }
        }
    }
}
