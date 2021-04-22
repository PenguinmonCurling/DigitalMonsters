using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DigitalMonsters
{
    public partial class EditForm : Form
    {
        private DigimonList _digimonList;
        private Digimon _currentDigimon;

        public EditForm(DigimonList digimonList, Digimon currentDigimon)
        {
            InitializeComponent();
            _currentDigimon = currentDigimon;
            _digimonList = digimonList;
            SetText();
        }

        private void SetText()
        {
            if (_currentDigimon != null)
            {
                NameText.Text = _currentDigimon.Name;
                DubNameText.Text = _currentDigimon.DubName;
                YearText.Text = _currentDigimon.DebutYear.ToString();
                EvolutionsText.Text = string.Join(",", _currentDigimon.Digivolutions);
                LevelText.Text = _currentDigimon.Level;
            }
        }

        private void UpdateCurrentMon()
        {
            if (_currentDigimon != null)
            {
                _currentDigimon.Name = NameText.Text;
                _currentDigimon.DubName = DubNameText.Text;
                if (int.TryParse(YearText.Text, out int debutYear))
                {
                    _currentDigimon.DebutYear = debutYear;
                }
                _currentDigimon.Digivolutions = EvolutionsText.Text.Split(',').ToList();
                _currentDigimon.Level = LevelText.Text;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //capture page up key
            if (keyData == Keys.PageUp)
            {
                UpdateCurrentMon();
                GoToNextMon();
                return true;
            }
            //capture page down key
            if (keyData == Keys.PageDown)
            {
                UpdateCurrentMon();
                GoToPreviousMon();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void GoToPreviousMon()
        {
            var nextDigimon = _digimonList.DigimonCollection.OrderByDescending(z => z.Number).FirstOrDefault(x => x.Number < _currentDigimon.Number);
            if (nextDigimon != null)
            {
                _currentDigimon = nextDigimon;
                SetText();
            }
        }

        private void GoToNextMon()
        {
            var nextDigimon = _digimonList.DigimonCollection.OrderBy(z => z.Number).FirstOrDefault(x => x.Number > _currentDigimon.Number);
            if (nextDigimon != null)
            {
                _currentDigimon = nextDigimon;
                SetText();
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            _digimonList.DigimonCollection.Remove(_currentDigimon);
            _digimonList.UnfilteredDigimonCollection.Remove(_currentDigimon);
            GoToNextMon();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            var loader = new DigimonFileLoader(null);
            loader.SaveDigimon(_digimonList.UnfilteredDigimonCollection, "DigimonList.xml");
            new MessageForm($"Digimon List saved").ShowDialog();
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            var newDigimon = new Digimon
            {
                Name = NameText.Text,
                DubName = DubNameText.Text,
                DebutYear = int.TryParse(YearText.Text, out int debutYear) ? debutYear : DateTime.Now.Year,
                Digivolutions = EvolutionsText.Text.Split(',').ToList(),
                Level = LevelText.Text
            };
            if (_digimonList.TryAddDigimon(newDigimon))
            {
                new MessageForm($"New Digimon {newDigimon.Name} added to list").ShowDialog();
            }
            else
            {
                new MessageForm($"No Digimon named {newDigimon.Name} found").ShowDialog();
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            _digimonList.UpdateDigimon(_currentDigimon);
            SetText();
        }
    }
}
