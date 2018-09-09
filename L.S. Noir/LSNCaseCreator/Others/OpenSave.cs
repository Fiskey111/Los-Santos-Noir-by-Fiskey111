using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSNCaseCreator.Others
{
    class OpenSave
    {
        private bool _isSave;
        private string _saveLocation;

        internal OpenSave(bool isSave)
        {
            _isSave = isSave;
            SetData();
        }

        internal void Run()
        {
            Thread t = new Thread(SaveOpen);
            t.Start();
        }

        internal void Save()
        {
            
        }

        private string _boxText;

        private void SetData()
        {
            _boxText = _isSave ? "Save Case" : "Open Case";
        }

        private void SaveOpen()
        {
            Forms.OpenSave menu = new Forms.OpenSave();
            menu.Show();
            if (_isSave)
            {
                menu.SaveFile = new SaveFileDialog();
                menu.SaveFile.Title = _boxText;
                menu.ShowDialog();

                if (menu.SaveFile.FileName != "")
                {

                }
            }


            _saveLocation = menu.SelectedCaseFolder;
        }
    }
}
