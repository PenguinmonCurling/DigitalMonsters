using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DigitalMonsters
{
    public partial class Form1 : Form
    {
        private DigimonList _digimonList;
        private int _imageCount;
        private Point LastLocation;

        public Form1()
        {
            _digimonList = new DigimonList();
            _digimonList.LoadDigimon();
            LastLocation = new Point(0, 40);
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= 200; i++)
            {
                var digimon = _digimonList.GetDigimon(_imageCount);

                if (digimon != null)
                {
                    var pictureBox = new PictureBox();
                    pictureBox.Name = digimon.Name;
                    pictureBox.Size = new Size(165, 159);
                    pictureBox.Location = new Point(LastLocation.X, LastLocation.Y);
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        //_digimonList.DigimonLoader.LoadImage(digimon);
                        if (!string.IsNullOrWhiteSpace(digimon.ImageUrl))
                        {
                            try
                            {
                                pictureBox.Load(digimon.ImageUrl);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    });
                    this.Controls.Add(pictureBox);

                    var textBox = new TextBox();
                    textBox.Name = string.Concat(digimon.Name, "Text");
                    textBox.Text = digimon.DisplayName;
                    textBox.Size = new Size(165, 159);
                    textBox.Location = new Point(LastLocation.X, LastLocation.Y + 160);
                    this.Controls.Add(textBox);
                    textBox.BringToFront();

                    GetNewLastLocation(pictureBox);

                    _imageCount++;
                }
                else
                {
                    break;
                }
            }

            foreach (var digimon in _digimonList.DigimonCollection)
            {
                var digiBox = this.Controls.Find(string.Concat(digimon.Name, "Text"), true).FirstOrDefault() as TextBox;
                if(digiBox != null)
                {
                    digiBox.Text = digimon.DisplayName + " (" + digimon.DubLevel + ")";
                }
            }
            _digimonList.DigimonLoader.SaveDigimon(_digimonList.DigimonCollection);
        }

        private void GetNewLastLocation(PictureBox pictureBox)
        {
            var y = pictureBox.Location.Y;
            var x = pictureBox.Location.X + 170;
            if (pictureBox.Location.X + 170 % 2000 > 1800)
            {
                y += 180;
                x = 0;
            }

            LastLocation = new Point(x, y);
        }
    }
}
