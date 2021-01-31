using System.Windows.Forms;

namespace DigitalMonsters
{
    public partial class MessageForm : Form
    {
        public MessageForm(string text)
        {
            InitializeComponent();
            MessageText.Text = text;
        }

        private void ok_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
