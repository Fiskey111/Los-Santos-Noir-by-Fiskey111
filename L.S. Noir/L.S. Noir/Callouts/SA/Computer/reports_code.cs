using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Gwen.Control;
using LSNoir.Callouts.SA.Data;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using Rage;
using Rage.Forms;

namespace LSNoir.Callouts.SA.Computer
{
    public class ReportsCode : GwenForm
    {
        // Gwen
        private ComboBox report_select_combobox;
        private Button report_return_but;
        private Label report_name_value, report_occupation_value, report_update_value;
        private Label report_select_lbl, report_name_lbl, report_occupation_lbl, report_statement_lbl, report_update_lbl;
        private MultilineTextBox report_statement_box;

        // Data
        private ReportData _foData, _emsData, _corData, _meData, _vfData;

        public ReportsCode()
            : base(typeof(ReportsForm))
        {

        }

        public override void InitializeLayout()
        {
            base.InitializeLayout();
            Window.IsClosable = false;
            Position = new Point(Game.Resolution.Width / 2 - Window.Width / 2, Game.Resolution.Height / 2 - Window.Height / 2);
            "Initializing SAJRS Report Statement Viewer".AddLog();

            RetreiveData();

            HideStuff();

            StartMethods();
        }

        private void RetreiveData()
        {
            _foData = LtFlash.Common.Serialization.Serializer.GetSelectedListElementFromXml<ReportData>(Main.RDataPath,
                c => c.FirstOrDefault(r => r.Type == ReportData.Service.FO));
            _emsData = LtFlash.Common.Serialization.Serializer.GetSelectedListElementFromXml<ReportData>(Main.RDataPath,
                c => c.FirstOrDefault(r => r.Type == ReportData.Service.EMS));
            _corData = LtFlash.Common.Serialization.Serializer.GetSelectedListElementFromXml<ReportData>(Main.RDataPath,
                c => c.FirstOrDefault(r => r.Type == ReportData.Service.Coroner));
            _meData = LtFlash.Common.Serialization.Serializer.GetSelectedListElementFromXml<ReportData>(Main.RDataPath,
                c => c.FirstOrDefault(r => r.Type == ReportData.Service.ME));
            _vfData = LtFlash.Common.Serialization.Serializer.GetSelectedListElementFromXml<ReportData>(Main.RDataPath,
                c => c.FirstOrDefault(r => r.Type == ReportData.Service.VicFamily));
        }

        private void HideStuff()
        {
            report_name_lbl.Hide();
            report_occupation_lbl.Hide();
            report_name_value.Hide();
            report_occupation_value.Hide();
            report_statement_lbl.Hide();
            report_statement_box.Hide();
            report_update_lbl.Hide();
            report_update_value.Hide();
        }

        private void StartMethods()
        {
            if (_foData != null)
                report_select_combobox.AddItem("First Officer");
            if (_emsData != null)
                report_select_combobox.AddItem("EMS");
            if (_corData != null)
                report_select_combobox.AddItem("Coroner");
            if (_meData != null)
                report_select_combobox.AddItem("Medical Examiner");
            if (_vfData != null)
                report_select_combobox.AddItem("Victim's Family");

            report_select_combobox.ItemSelected += Wit_select_combobox_ItemSelected;
            report_return_but.Clicked += Witness_return_but_Clicked;
        }

        private void Wit_select_combobox_ItemSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            switch (report_select_combobox.SelectedItem.Text)
            {
                case "First Officer":
                    report_name_value.Text = _foData.Name;
                    report_update_value.Text = _foData.Time.ToShortTimeString();
                    report_occupation_value.Text = "Police Officer";
                    report_statement_box.Text = ConversationSplitter(_foData.Transcript);
                    break;
                case "EMS":
                    report_name_value.Text = _emsData.Name;
                    report_update_value.Text = _emsData.Time.ToShortTimeString();
                    report_occupation_value.Text = "Paramedic";
                    report_statement_box.Text = ConversationSplitter(_emsData.Transcript);
                    break;
                case "Coroner":
                    report_name_value.Text = _corData.Name;
                    report_update_value.Text = _corData.Time.ToShortTimeString();
                    report_occupation_value.Text = "Coroner";
                    report_statement_box.Text = ConversationSplitter(_corData.Transcript);
                    break;
                case "Medical Examiner":
                    report_name_value.Text = _meData.Name;
                    report_update_value.Text = _meData.Time.ToShortTimeString();
                    report_occupation_value.Text = "Medical Examiner";
                    report_statement_box.Text = ConversationSplitter(_meData.Transcript);
                    break;
                case "Victim's Family":
                    report_name_value.Text = _vfData.Name;
                    report_update_value.Text = _vfData.Time.ToShortTimeString();
                    report_occupation_value.Text = "Victim's Family";
                    report_statement_box.Text = ConversationSplitter(_vfData.Transcript);
                    break;
            }
            ShowStuff();
        }

        private void ShowStuff()
        {
            report_name_value.Show();
            report_occupation_lbl.Show();
            report_name_lbl.Show();
            report_occupation_value.Show();
            report_statement_lbl.Show();
            report_statement_box.Show();
            report_update_value.Show();
            report_update_lbl.Show();
        }

        private string ConversationSplitter(List<string> conversation) => string.Join("\n", conversation.ToArray());

        private void Witness_return_but_Clicked(Base sender, ClickedEventArgs arguments)
        {
            "Returning to main form".AddLog();
            Window.Close();
            Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.ReportFiber, ComputerController.Fibers.MainFiber);
        }
    }
}
