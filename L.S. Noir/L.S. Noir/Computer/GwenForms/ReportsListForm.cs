using Gwen.Control;
using LSNoir.Data;
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
            GwenForms.SharedMethods.SetFormPositionCenter(this);

            Window.DisableResizing();

            Array.ForEach(data, d => reportsList.AddRow(d.Title, d.ID, d));

            reportsList.RowSelected += ReportsList_RowSelected;
            close.Clicked += (s, e) => Window.Close();

            base.InitializeLayout();
        }

        private void ReportsList_RowSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            var selectedReport = (reportsList.SelectedRow.UserData as ReportData);
            if (selectedReport == null) return;

            reportTitle.Text = selectedReport.Title;
            reportTitle.Disable();
            reportTitle.KeyboardInputEnabled = false;

            GwenForms.SharedMethods.AddSplittedTxtToMultilineTextBox(selectedReport.Text, reportText);

            reportText.Disable();
            reportText.KeyboardInputEnabled = false;
            reportText.TextPadding = Gwen.Padding.One;
        }
    }
}
