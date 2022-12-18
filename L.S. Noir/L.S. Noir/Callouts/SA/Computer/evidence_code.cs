namespace LSNoir.Callouts.SA.Computer
{/*
    public class EvidenceCode : GwenForm
    {
        // Gwen
        private Button evidence_return_but, evidence_lab_but;
        private ListBox evidence_values_box;
        
        // Data
        private List<EvidenceData> _eList;
        private CaseData _cData;

        public EvidenceCode()
            : base(typeof(EvidenceForm))
        {

        }

        public override void InitializeLayout()
        {
            base.InitializeLayout();
            Window.IsClosable = false;
            Position = new Point(Game.Resolution.Width / 2 - Window.Width / 2, Game.Resolution.Height / 2 - Window.Height / 2);
            "Initializing San Andreas Joint Records System Evidence Viewer".AddLog();

            _eList = Serializer.LoadItemFromXML<List<EvidenceData>>(Main.EDataPath);
            _cData = Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);

            if (_cData.EvidenceTested) this.evidence_lab_but.Hide();

            CreateMethods();

            FillData();
        }

        private void CreateMethods()
        {
            evidence_return_but.Clicked += Evidence_return_but_Clicked;
            if (!_cData.EvidenceTested)
            {
                evidence_lab_but.Clicked += Evidence_lab_but_Clicked;
                _cData.EvidenceTested = true;
                Serializer.SaveItemToXML<CaseData>(_cData, Main.CDataPath);
            }
            else
            {
                evidence_lab_but.Hide();
            }
        }

        private void FillData()
        {
            evidence_values_box.AddRow(String.Format("{0}{1}{2}{3}{4}", "Evidence Name", " | ",
                "Traces found on Evidence", " | ", "Evidence Importance Level"));
            foreach (var data in _eList)
            {
                if (!data.Collected) continue;;

                var testResult = data.IsTested ? data.Importance.ToString() : "Not tested";

                evidence_values_box.AddRow(String.Format("{0}{1}{2}{3}{4}", data.Name.PadRight(24), "   ",
                    data.Trace.ToString().PadRight(34), "   ", testResult));
            }
        }

        private void Evidence_lab_but_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Window.Close();
            Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.EvidenceFiber, ComputerController.Fibers.LabFiber);
        }

        private void Evidence_return_but_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Window.Close();
            Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.EvidenceFiber, ComputerController.Fibers.MainFiber);
        }
    }*/
}
