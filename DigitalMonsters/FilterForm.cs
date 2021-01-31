using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace DigitalMonsters
{
    public partial class FilterForm : Form
    {
        public event EventHandler<DigimonFilter> FilterSet;

        public FilterForm(DigimonFilter filter)
        {
            InitializeComponent();
            AppearanceTypeBox.DataSource = Enum.GetValues(typeof(Appearance.AppearanceType));
            if (filter != null)
            {
                NameFilter.Text = filter.NameFilter;
                AppearanceFilter.Text = filter.AppearanceFilter;
                LevelFilter.Text = filter.LevelFilter;
                AppearanceTypeBox.SelectedItem = filter.AppearanceTypeFilter;
                if (filter.NumberFilter > 0)
                {
                    NumberFilter.Text = filter.NumberFilter.ToString();
                }
                if (filter.YearFilter > 0)
                {
                    YearFilter.Text = filter.YearFilter.ToString();
                }
                if (filter.YearEndFilter > 0)
                {
                    YearEndFilter.Text = filter.YearEndFilter.ToString();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var filter = GetFilter();
            FilterSet(this, filter);
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var filter = new DigimonFilter();
            filter.AppearanceTypeFilter = Appearance.AppearanceType.Any;
            FilterSet(this, filter);
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var filter = GetFilter();
            filter.AntiFilter = true;
            FilterSet(this, filter);
            Close();
        }

        private DigimonFilter GetFilter()
        {
            var filter = new DigimonFilter();
            filter.NameFilter = NameFilter.Text;
            filter.AppearanceFilter = AppearanceFilter.Text;
            filter.LevelFilter = LevelFilter.Text;
            filter.TypeFilter = TypeFilter.Text;
            if (Enum.TryParse(AppearanceTypeBox.SelectedValue.ToString(), out Appearance.AppearanceType appearanceTypeFilter))
            {
                filter.AppearanceTypeFilter = appearanceTypeFilter;
            }
            if (int.TryParse(NumberFilter.Text, out int numberFilter))
            {
                filter.NumberFilter = numberFilter;
            }
            if (int.TryParse(YearFilter.Text, out int yearFilter))
            {
                filter.YearFilter = yearFilter;
            }
            if (int.TryParse(YearEndFilter.Text, out int yearEndFilter))
            {
                filter.YearEndFilter = yearEndFilter;
            }

            return filter;
        }

        private void Export_Click(object sender, EventArgs e)
        {
            var filter = GetFilter();
            var loader = new DigimonFileLoader(null);
            var digimon = new DigimonList();
            digimon.LoadDigimon();
            digimon.FilterList(filter);
            loader.SaveDigimon(digimon.DigimonCollection, "DigimonListExport.xml");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var filter = GetFilter();
            var digimon = new DigimonList();
            digimon.LoadDigimon();
            digimon.FilterList(filter);
            new ImageGenerator().SaveGeneratedImage(digimon.DigimonCollection);
        }
    }
}
