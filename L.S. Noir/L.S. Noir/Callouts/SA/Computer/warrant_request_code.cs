namespace LSNoir.Callouts.SA.Computer
{/*
    public class WarrantRequestCode : GwenForm
    {
        // System
        private Stopwatch _sw = new Stopwatch();

        // Gwen
        private Label req_sending_lbl, req_reason_lbl;
        private Button req_return_but;
        private ProgressBar req_progress_bar;
        
        // Data
        private CaseData _cData;
        internal static PedData SData;

        public WarrantRequestCode()
            : base(typeof(WarrantRequestForm))
        {

        }

        public override void InitializeLayout()
        {
            base.InitializeLayout();
            Window.IsClosable = false;
            Position = new Point(Game.Resolution.Width / 2 - Window.Width / 2, Game.Resolution.Height / 2 - Window.Height / 2);
            "Initializing San Andreas Joint Records System Warrant Request Form".AddLog();

            _cData = LtFlash.Common.Serialization.Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);

            req_return_but.Hide();
            req_reason_lbl.Text = _cData.WarrantReason;
            _sw.Start();
            req_progress_bar.Value = 0.01f;

            GameFiber.StartNew(delegate
            {
                var txt = 0f;
                for (var i = 0f; i < 1f; i = i + MathHelper.GetRandomSingle(0.01f, 0.10f))
                {
                    req_progress_bar.Value = i;

                    var remainder = IsDivisibleByFour(txt);
                    var text = "Sending Request";
                    if (remainder == 0.25f) text = "Sending Request.";
                    else if (remainder == 0.50f) text = "Sending Request..";
                    else if (remainder == 0.75f) text = "Sending Request...";

                    req_sending_lbl.Text = text;
                    txt++;

                    GameFiber.Sleep(MathHelper.GetRandomInteger(0500, 1000));
                }
                req_return_but.Show();
                req_progress_bar.Value = 1f;
                _cData.WarrantRequested = true;
                LtFlash.Common.Serialization.Serializer.SaveItemToXML<CaseData>(_cData, Main.CDataPath);
                WarrantRequestor.RequestWarrant(_cData);
                req_sending_lbl.Text = "Request Sent!";
                _sw.Stop();


                "Displaying MessageBox".AddLog();
                MessageBoxCode.Message = $"Known place of employment: Strawberry Mortuary\nRequest a raid with warrant squad?\n(Based on warrant approval)";
                Window.Close();
                Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.WarrantRequestFiber, ComputerController.Fibers.MessageBoxFiber);
            });
        }

        private static float IsDivisibleByFour(float value) => value % 4;
    }*/
}
