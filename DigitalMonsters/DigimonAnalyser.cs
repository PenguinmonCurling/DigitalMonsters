using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DigitalMonsters
{
    public partial class DigimonAnalyser : Form
    {
        private DigimonList _digimonList;
        private Digimon _currentDigimon;
        private List<ArmourChart> _armourChart;
        private DigimonFilter _filter;
        private bool _justChanged;

        public DigimonAnalyser()
        {
            _digimonList = new DigimonList();
            _digimonList.LoadDigimon();

            var armourLoader = new DigimonArmourLoader();
            _armourChart = armourLoader.LoadArmourData();

            InitializeComponent();
        }

        private void DigimonEntered(object sender, EventArgs e)
        {
            if (!_justChanged)
            {
                var digimonName = textBox1.Text;
                if (_digimonList.DigimonCollection.Any(x => x.NameCheck(digimonName)))
                {
                    _currentDigimon = _digimonList.DigimonCollection.FirstOrDefault(x => x.NameCheck(digimonName));
                    SetDigimon(_currentDigimon);
                }
                _justChanged = false;
            }
        }

        private void NumberEntered(object sender, EventArgs e)
        {
            var digimonNumber = textBox2.Text;
            if (int.TryParse(digimonNumber, out int digimonInteger) && digimonInteger > 0 
                && _digimonList.DigimonCollection.Any(x => x.Number == digimonInteger))
            {
                _currentDigimon = _digimonList.DigimonCollection.FirstOrDefault(x => x.Number == digimonInteger);
                SetDigimon(_currentDigimon);
            }
            _justChanged = false;
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (_digimonList.DigimonCollection.Any(x => x.Number > _currentDigimon?.Number))
            {
                _currentDigimon = _digimonList.DigimonCollection.OrderBy(z => z.Number).FirstOrDefault(x => x.Number > _currentDigimon.Number);
                SetDigimon(_currentDigimon);
            }
            _justChanged = false;
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            if (_digimonList.DigimonCollection.Any(x => x.Number < _currentDigimon?.Number))
            {
                _currentDigimon = _digimonList.DigimonCollection.OrderByDescending(z => z.Number).FirstOrDefault(x => x.Number < _currentDigimon.Number);
                SetDigimon(_currentDigimon);
            }
            _justChanged = false;
        }

        private void evolveButton_Click(object sender, EventArgs e)
        {
            if(!TryArmourEvolve(_currentDigimon))
            {
                Digivolve();
            }
            _justChanged = false;
        }

        private void Digivolve()
        {
            if (_currentDigimon.Digivolutions != null)
            {
                var random = new Random();
                foreach (var digimon in _currentDigimon.Digivolutions.Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(y => random.Next(100)))
                {
                    if (_digimonList.DigimonCollection.Any(x => x.NameCheck(digimon)))
                    {
                        ChangeCurrentDigimon(digimon);
                        break;
                    }
                }
            }
        }

        private void Dedigivolve()
        {
            var random = new Random();
            foreach (var digimon in PossibleDevolutions()
                    .OrderBy(x => random.Next(100)))
            {
                ChangeCurrentDigimon(digimon);
                break;
            }
        }

        private IEnumerable<String> PossibleDevolutions()
        {
            var devolutions = _digimonList.DigimonCollection.Where(x =>
                            x.Digivolutions.Contains(_currentDigimon.DisplayName, StringComparer.OrdinalIgnoreCase)
                            || x.Digivolutions.Contains(_currentDigimon.Name, StringComparer.OrdinalIgnoreCase)
                            || (!string.IsNullOrWhiteSpace(_currentDigimon.DubName) && x.Digivolutions.Contains(_currentDigimon.DubName, StringComparer.OrdinalIgnoreCase))
                            || x.Digivolutions.Contains(_currentDigimon.Name.Replace(" ", string.Empty), StringComparer.OrdinalIgnoreCase)
                            || (!string.IsNullOrWhiteSpace(_currentDigimon.DubName) && x.Digivolutions.Contains(_currentDigimon.DubName.Replace(" ", string.Empty), StringComparer.OrdinalIgnoreCase)))
                            .Select(x => x.Name).ToList();
            if (TryArmourDevolve(_currentDigimon, out string devolution))
            {
                devolutions.Add(devolution);
            }
            return devolutions;
        }

        private void ChangeCurrentDigimon(string digimon)
        {
            var nextDigimon = _digimonList.DigimonCollection.FirstOrDefault(x => x.NameCheck(digimon));
            if (nextDigimon != null)
            {
                _currentDigimon = nextDigimon;
                SetDigimon(_currentDigimon);
            }
        }

        private bool TryArmourEvolve(Digimon digimon)
        {
            if(_armourChart.Any(x => string.Equals(x.Digimon, digimon.Name, StringComparison.OrdinalIgnoreCase)))
            {
                var armourForm = new ArmourEvolver();
                armourForm.ArmourEvolved += ArmourCynka;
                armourForm.NormalEvolution += NormalCynka;
                armourForm.ShowDialog();
                return true;
            }
            return false;
        }

        private bool TryArmourDevolve(Digimon digimon, out string digimonToDevolve)
        {
            var devolutionFound = false;
            digimonToDevolve = string.Empty;
            var devolveTo = _armourChart.FirstOrDefault(x => x.DigimentalDigivolution.Values.Any(y => string.Equals(y, digimon.Name, StringComparison.OrdinalIgnoreCase)));
            if (devolveTo != null)
            {
                digimonToDevolve = devolveTo.Digimon;
                devolutionFound = true;
            }
            return devolutionFound;
        }

        private void NormalCynka(object sender, EventArgs e)
        {
            Digivolve();

            var form = sender as ArmourEvolver;
            if (form != null)
            {
                form.ArmourEvolved -= ArmourCynka;
            }
        }

        private void ArmourCynka(object sender, string digimental)
        {
            var armourChart = _armourChart.FirstOrDefault(x => string.Equals(x.Digimon, _currentDigimon.Name, StringComparison.OrdinalIgnoreCase));
            if (armourChart != null)
            {
                var digimon = armourChart.DigimentalDigivolution[digimental];
                ChangeCurrentDigimon(digimon);
            }

            var form = sender as ArmourEvolver;
            if (form != null)
            {
                form.ArmourEvolved -= ArmourCynka;
            }
        }

        private void SetDigimon(Digimon digimon)
        {
            _justChanged = true;
            if (digimon != null)
            {
                pictureBox2.Visible = true;
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                ThreadPool.QueueUserWorkItem(x =>
                {
                    try
                    {
                        if (digimon.ImageUrl != null && string.Equals(_currentDigimon.Name, digimon.Name))
                        {
                            pictureBox2.LoadAsync(digimon.ImageUrl);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                });
                textBox2.Text = digimon.Number.ToString();
                textBox1.Text = digimon.DisplayName;
                textBox3.Lines = new string[3] { digimon.DubLevel, digimon.Attribute, digimon.Type };
                if (digimon.Appearances != null)
                {
                    textBox4.Lines = digimon.Appearances.Where(y => y.AppearanceValue() < 1000).OrderBy(x => x.AppearanceValue()).Select(y => y.AppearanceNameConvert()).ToArray();
                    if (textBox4.Lines.Count() > 3)
                    {
                        textBox4.ScrollBars = ScrollBars.Vertical;
                    }
                    else
                    {
                        textBox4.ScrollBars = ScrollBars.None;
                    }
                }
            }
        }

        private void xAntibodyButton_Click(object sender, EventArgs e)
        {
            if (_digimonList.DigimonCollection.Any(x => x.XAntibodyNameCheck(_currentDigimon.Name)))
            {
                var digimon = _digimonList.DigimonCollection.FirstOrDefault(x => x.XAntibodyNameCheck(_currentDigimon.Name));
                ChangeCurrentDigimon(digimon.Name);
            }
            else if (_digimonList.TryAddDigimon(new Digimon { Name = string.Format("{0} X-Antibody", _currentDigimon.Name) }))
            {
                var digimon = _digimonList.DigimonCollection.FirstOrDefault(x => x.XAntibodyNameCheck(_currentDigimon.Name));
                ChangeCurrentDigimon(digimon.Name);
            }
            else if(_digimonList.DigimonCollection.Any(x => (string.Equals(x.Name, string.Format("{0} (2006 Anime Version)", _currentDigimon.Name), StringComparison.OrdinalIgnoreCase))))
            {
                var digimon = _digimonList.DigimonCollection.FirstOrDefault(x => (string.Equals(x.Name, string.Format("{0} (2006 Anime Version)", _currentDigimon.Name), StringComparison.OrdinalIgnoreCase)));
                ChangeCurrentDigimon(digimon.Name);
            }
            _justChanged = false;
        }

        private void deXAntibodyButton_Click(object sender, EventArgs e)
        {
            if (_currentDigimon.Name.IndexOf("2006 Anime Version") >= 0 && _digimonList.DigimonCollection.Any(x => (string.Equals(x.Name, _currentDigimon.Name.Replace(" (2006 Anime Version)", string.Empty), StringComparison.OrdinalIgnoreCase))))
            {
                var digimon = _digimonList.DigimonCollection.FirstOrDefault(x => (string.Equals(x.Name, _currentDigimon.Name.Replace(" (2006 Anime Version)", string.Empty), StringComparison.OrdinalIgnoreCase)));
                ChangeCurrentDigimon(digimon.Name);
            }
            else if (_digimonList.DigimonCollection.Any(x => x.DeXAntibodyNameCheck(_currentDigimon.Name)))
            {
                var digimon = _digimonList.DigimonCollection.FirstOrDefault(x => x.DeXAntibodyNameCheck(_currentDigimon.Name));
                ChangeCurrentDigimon(digimon.Name);
            }
            _justChanged = false;
        }

        private void filterButton_Click(object sender, EventArgs e)
        {
            var filterForm = new FilterForm(_filter);
            filterForm.FilterSet += Filter;
            filterForm.Reset += Reset;
            filterForm.ShowDialog();
        }

        private void Filter(object sender, DigimonFilter e)
        {
            _digimonList.FilterList(e);
            SetDigimon(_digimonList.DigimonCollection.FirstOrDefault(x => x.Number == 1));
            _filter = e;
            var form = sender as FilterForm;
            if (form != null)
            {
                form.FilterSet -= Filter;
                form.Reset -= Reset;
            }
        }

        private void Reset(object sender, DigimonFilter e)
        {
            _digimonList.ResetList(e);
            SetDigimon(_digimonList.DigimonCollection.FirstOrDefault(x => x.Number == 1));
            _filter = e;
            var form = sender as FilterForm;
            if (form != null)
            {
                form.FilterSet -= Filter;
                form.Reset -= Reset;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //capture left arrow key
            if (keyData == Keys.PageDown)
            {
                prevButton_Click(string.Empty, null);
                return true;
            }
            //capture right arrow key
            if (keyData == Keys.PageUp)
            {
                nextButton_Click(string.Empty, null);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void devolveButton_Click(object sender, EventArgs e)
        {
            Dedigivolve();
            _justChanged = false;
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            var editForm = new EditForm(_digimonList, _currentDigimon);
            editForm.ShowDialog();
        }

        private void pictureBox2_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (!string.Equals(pictureBox2.ImageLocation, _currentDigimon.ImageUrl))
            {
                pictureBox2.LoadAsync(_currentDigimon.ImageUrl);
            }
        }
    }
}
