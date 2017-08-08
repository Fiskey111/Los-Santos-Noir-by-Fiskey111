using Gwen.Control;
using Rage;
using System;
using System.Drawing;
using System.Linq;

namespace LSNoir.Computer.GwenForms
{
    class SharedMethods
    {
        public static void AddSplittedTxtToMultilineTextBox(string txt, MultilineTextBox tb)
        {
            //clean textBox
            Enumerable.Range(0, 19).ToList().ForEach(n => tb.SetTextLine(n, ""));

            string[] t = txt.Split(new string[] { "{n}" }, StringSplitOptions.None);
            for (int i = 0; i < t.Length; i++)
            {
                tb.SetTextLine(i, t[i]);
            }
        }

        public static void SetFormPositionCenter(Rage.Forms.GwenForm form)
        {
            int x = (Game.Resolution.Width / 2) - (form.Size.Width / 2);
            int y = (Game.Resolution.Height / 2) - (form.Size.Height / 2);

            form.Position = new Point(x, y);
        }

        public static void DisplayProgressWnd(ComputerController host, string text)
        {
            GameFiber.StartNew(() =>
            {
                var progressWnd = new ProgressForm(text);
                host.AddWnd(progressWnd);
                progressWnd.Show();
            });
        }
    }
}
