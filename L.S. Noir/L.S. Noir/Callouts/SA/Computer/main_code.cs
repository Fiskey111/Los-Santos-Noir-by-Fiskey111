using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Gwen.Control;
using LSNoir.Callouts.SA.Data;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using LtFlash.Common.Serialization;
using Rage;
using Rage.Forms;

namespace LSNoir.Callouts.SA.Computer
{
    public class MainCode : GwenForm
    {
        // System
        internal static bool HideForm = false, isFormAlive;
        internal static string PlateNumber = "";

        // Gwen
        private Label main_casenumber_label, main_casestatus_label, main_completedcase_label, main_laststage_label, main_warrant_lbl,
            main_update_label, main_intro_label, FiskeyLabel, main_witness_label, main_suspect_label, navigate_to_label;
        private Label main_casenumber_value, main_casestatus_value, main_completedcases_value, main_laststage_value,
            main_warrant_value_lbl, main_witness_value;

        private TextBox _susBox;
        private Button nav_button, main_close_but, _infoBut;
        private ListBox main_updates_value;
        private ComboBox menu_combo;

        // Data
        private static CaseData _cData;
        private static List<PedData> _sData = new List<PedData>();

        public MainCode()
            : base(typeof(MainForm))
        {

        }

        public override void InitializeLayout()
        {
            "1".AddLog();
            base.InitializeLayout();
            StartForm();
        }

        private void StartForm()
        {
            Window.IsClosable = false;
            Position = new Point(Game.Resolution.Width / 2 - Window.Width / 2,
                Game.Resolution.Height / 2 - Window.Height / 2);
            Window.Height = 600;
            Window.Width = 800;
            Window.DeleteOnClose = true;

            _cData = Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);     
            _sData = Serializer.LoadFromXML<PedData>(Main.SDataPath);
            
            CreateForm();
            
            CreateMethods();
            
            FillData();
            
            isFormAlive = true;
        }

        private void CreateForm()
        {
            _infoBut.Hide();
            main_casenumber_label.SetPosition(106, 80);
            main_casestatus_label.SetPosition(380, 80);
            main_completedcase_label.SetPosition(106, 120);
            main_laststage_label.SetPosition(380, 120);
            main_update_label.SetPosition(106, 210);
            main_intro_label.SetPosition(5, 5);
            FiskeyLabel.SetPosition(515, 5);
            main_witness_label.SetPosition(106, 373);
            main_suspect_label.SetPosition(106, 413);


            main_casenumber_value.SetPosition(239, 80);
            main_casestatus_value.SetPosition(535, 80);
            main_completedcases_value.SetPosition(239, 120);
            main_laststage_value.SetPosition(535, 120);
            main_witness_value.SetPosition(239, 373);
            _susBox.SetSize(170, 30);

            menu_combo.SetPosition(273, 478);

            main_updates_value.SetPosition(242, 210);
        }

        private void CreateMethods()
        {
            nav_button.Clicked += Nav_button_Clicked;
            main_close_but.Clicked += Main_close_but_Clicked;
        }
        
        private void FillData()
        {
            main_casenumber_value.Text = _cData.Number.ToString();

            main_casestatus_value.TextColorOverride = Color.Red;
            
            main_casestatus_value.Text = "Unsolved";

            main_completedcases_value.Text = _cData.TotalSolvedCases.ToString();
            main_laststage_value.Text = _cData.LastCompletedStage.ToString();
            
            UpdateSajrs();
            
            if (GetWitData()) { }
            else main_witness_value.Text = "None";

            if (!string.IsNullOrWhiteSpace(_cData.CurrentSuspect))
                _susBox.Text = _cData.CurrentSuspect;

            _infoBut.Show();
            this._infoBut.Clicked += _infoBut_Clicked;
            _infoBut.SetToolTipText("Run the suspect information.");
            _susBox.SetToolTipText("Enter a suspect name to run their information.");
            
            if (_cData.WarrantSubmitted)
            {
                if (_cData.WarrantApproved)
                {
                    main_warrant_value_lbl.Text = "Approved";
                    main_warrant_value_lbl.TextColorOverride = Color.Green;
                }
                else
                {
                    main_warrant_value_lbl.Text = "Not Approved";
                    main_warrant_value_lbl.TextColorOverride = Color.Red;
                }
            }
            else
            {
                main_warrant_value_lbl.Text = "No warrant submitted";
            }

            menu_combo.AddItem("Victim Information");
            menu_combo.AddItem("Witness Statements");
            menu_combo.AddItem("SAJRS Reports");
            menu_combo.AddItem("Evidence Viewer");
            menu_combo.AddItem("View Security Camera Footage");
            if (_cData.WarrantAccess)
            {
                menu_combo.AddItem("Warrant Request Form");
            }
        }

        private bool GetWitData()
        {
            try
            {
                var witDataList = Serializer.LoadItemFromXML<List<PedData>>(Main.WDataPath);

                if (witDataList.Count < 1) return false;

                if (witDataList.Count == 1)
                {
                    var w1Data = Serializer.GetSelectedListElementFromXml<PedData>(Main.WDataPath,
                        c => Enumerable.FirstOrDefault<PedData>(c, p => p.Type == PedType.Witness1));

                    main_witness_value.Text = w1Data.Name;
                }
                else
                {
                    var w1Data = Serializer.GetSelectedListElementFromXml<PedData>(Main.WDataPath,
                        c => Enumerable.FirstOrDefault<PedData>(c, p => p.Type == PedType.Witness1));
                    var w2Data = Serializer.GetSelectedListElementFromXml<PedData>(Main.WDataPath,
                        c => Enumerable.FirstOrDefault<PedData>(c, p => p.Type == PedType.Witness2));

                    main_witness_value.Text = w1Data.Name + ", " + w2Data.Name;
                }
                return true;
            }
            catch (Exception ex)
            {
                $"Error loading witnesses: {ex.ToString()}".AddLog(true);
                return false;
            }
        }

        private void _infoBut_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var isMatch = false;
            PedData suspect = null;
            foreach (var s in _sData)
            {
                if (s.Name.ToLower() != _susBox.Text.ToLower()) continue;
                isMatch = true;
                suspect = s;
            }

            if (isMatch)
            {
                _cData.CurrentSuspect = _susBox.Text;
                Serializer.SaveItemToXML(_cData, Main.CDataPath);
                this._susBox.Text = suspect.Name;
                "Displaying MessageBox".AddLog();
                MessageBoxCode.Message = $"Name: {suspect.Name};  DOB: {suspect.Dob}\nGender: {suspect.Gender}\n\nAdd address to GPS?";
                OpenMessageBox();
            }
            else
            {
                "Displaying MessageBox".AddLog();
                MessageBoxCode.Message = "No information available on suspect";
                OpenMessageBox();
            }
        }

        private void UpdateSajrs()
        {
            foreach (string text in _cData.SajrsUpdates)
                main_updates_value.AddRow(text);
        }

        #region Methods
        private void Nav_button_Clicked(Base sender, ClickedEventArgs arguments)
        {
            switch (menu_combo.Text)
            {
                case "Witness Statements":
                    Window.Close();
                    Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.MainFiber, ComputerController.Fibers.WitnessFiber);
                    break;
                case "SAJRS Reports":
                    Window.Close();
                    Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.MainFiber, ComputerController.Fibers.ReportFiber);
                    break;
                case "Evidence Viewer":
                    Window.Close();
                    Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.MainFiber, ComputerController.Fibers.EvidenceFiber);
                    break;
                case "Victim Information":
                    Window.Close();
                    Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.MainFiber, ComputerController.Fibers.VictimFiber);
                    break;
                case "Warrant Request Form":
                    Window.Close();
                    Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.MainFiber, ComputerController.Fibers.WarrantFiber);
                    break;
                case "View Security Camera Footage":
                    Background.DisableBackground(Background.Type.Computer);
                    Window.Close();
                    Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.MainFiber, ComputerController.Fibers.SecurityCamFiber, _cData);
                    break;
            }
        }

        private void OpenMessageBox()
        {
            Window.Close();
            Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.MainFiber,
                ComputerController.Fibers.MessageBoxFiber);
        }

        private void Main_close_but_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Background.DisableBackground(Background.Type.Computer);
            Window.Close();
            Universal.Computer.AbortController();
        }
        #endregion     
    }
}
