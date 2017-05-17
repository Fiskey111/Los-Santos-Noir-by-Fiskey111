using Gwen.Control;
using LSNoir.Data;
using Rage;
using System;

namespace LSNoir.Computer
{
    class ReportsListForm : Rage.Forms.GwenForm
    {
        private ReportData[] data;
        private ListBox reportsList;
        private Button close;
        private TextBox reportTitle;
        private MultilineTextBox reportText;

        public ReportsListForm(ReportData[] reportsData) : base(typeof(ReportsList_Form))
        {
            data = reportsData;    
        }

        public override void InitializeLayout()
        {
            int x = (Game.Resolution.Width / 2) - (Size.Width / 2);
            int y = (Game.Resolution.Height / 2) - (Size.Height / 2);

            Position = new System.Drawing.Point(x, y);

            Array.ForEach(data, d => reportsList.AddRow(d.Title, d.ID, d));

            reportsList.RowSelected += ReportsList_RowSelected;
            close.Clicked += (s, e) => Window.Close();

            base.InitializeLayout();
        }

        private void ReportsList_RowSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            var d = (reportsList.SelectedRow.UserData as ReportData);
            if (d == null) return;
            reportTitle.Text = d.Title;
            reportTitle.Disable();
            reportTitle.KeyboardInputEnabled = false;

            string[] t = d.Text.Split(new string[]{ "{n}" }, StringSplitOptions.None);
            for (int i = 0; i < t.Length; i++)
            {
                reportText.SetTextLine(i, t[i]);
            }

            reportText.Disable();
            reportText.KeyboardInputEnabled = false;
            reportText.TextPadding = Gwen.Padding.One;
        }
    }
}
