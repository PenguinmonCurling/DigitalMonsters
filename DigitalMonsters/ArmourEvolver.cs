using System;
using System.Windows.Forms;

namespace DigitalMonsters
{
    public partial class ArmourEvolver : Form
    {
        public event EventHandler<string> ArmourEvolved;
        public event EventHandler NormalEvolution;

        public ArmourEvolver()
        {
            InitializeComponent();
        }

        private void DigimentalPressed(object sender, EventArgs e)
        {
            var button = sender as Button;
            ArmourEvolved(this, button.Name);
            Close();
        }

        private void NormalCynka(object sender, EventArgs e)
        {
            NormalEvolution(this, e);
            Close();
        }
    }
}
