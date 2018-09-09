using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSNCaseCreator.Others;

namespace LSNCaseCreator
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();

            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSave openMenu = new OpenSave(false);
            openMenu.Run();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSave openMenu = new OpenSave(true);
            openMenu.Run();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSave openMenu = new OpenSave(true);
            openMenu.Save();
        }
    }
}
