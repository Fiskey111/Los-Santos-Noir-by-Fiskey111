namespace LSNoir.Callouts.SA.Computer
{/*
    public class LabCode : GwenForm
    {
        // System
        private Stopwatch _sw = new Stopwatch();

        // Gwen
        private Label lab_sending_lbl;
        private Button lab_return_but;
        private ListBox lab_item_box;
        private ProgressBar lab_progress_bar;

        // Data
        private List<EvidenceData> _eList;

        public LabCode()
            : base(typeof(LabForm))
        {

        }

        public override void InitializeLayout()
        {
            base.InitializeLayout();
            Window.IsClosable = false;
            Position = new Point(Game.Resolution.Width / 2 - Window.Width / 2, Game.Resolution.Height / 2 - Window.Height / 2);
            "Initializing San Andreas Joint Records System Lab Request Form".AddLog();

            _eList = Serializer.LoadItemFromXML<List<EvidenceData>>(Main.EDataPath);

            StartMethods();

            FillData();

            StartCountdown();
        }

        private void StartMethods()
        {
            lab_return_but.Clicked += Lab_return_but_Clicked;
        }

        private void FillData()
        {
            lab_progress_bar.Value = 0.01f;
            foreach (var data in _eList)
            {
                if (!data.Collected) continue;

                lab_item_box.AddRow(String.Format("{0}{1}",
                    Rand.RandomNumber(300, 333333).ToString().PadRight(15),
                    data.Name));
            }
        }

        private void StartCountdown()
        {
            lab_return_but.Hide();

            _sw.Start();
            GameFiber.StartNew(delegate
            {
                var txt = 0f;
                for (var i = 0f; i < 1f; i = i + MathHelper.GetRandomSingle(0.01f, 0.10f))
                {
                    lab_progress_bar.Value = i;

                    var remainder = IsDivisibleByFour(txt);
                    var text = "Sending Request";
                    if (remainder == 0.25f) text = "Sending Request.";
                    else if (remainder == 0.50f) text = "Sending Request..";
                    else if (remainder == 0.75f) text = "Sending Request...";

                    lab_sending_lbl.Text = text;
                    txt++;

                    GameFiber.Sleep(MathHelper.GetRandomInteger(0500, 1000));
                }
                lab_progress_bar.Hide();
                lab_progress_bar.Value = 1f;

                foreach (var data in _eList)
                {
                    if (!data.Collected) continue;
                    data.TestingFinishTime = TimeCheckObject.RandomTimeCreator().ToLocalTime();
                    Evid_War_TimeChecker.AddObject(new TimeCheckObject(TimeCheckObject.Type.Evidence, data.Name, data.TestingFinishTime));
                }

                Serializer.SaveItemToXML<List<EvidenceData>>(_eList, Main.EDataPath);        

                lab_sending_lbl.Text = "Request Sent!";
                _sw.Stop();
                lab_return_but.Show();
            });
        }

        private static float IsDivisibleByFour(float value) => value % 4;

        private void Lab_return_but_Clicked(Base sender, ClickedEventArgs arguments)
        {
            "Displaying MessageBox".AddLog();
            MessageBoxCode.Message = "The request for testing has been placed. \nPlease be patient while lab technicians test the evidence. \n\nYou will be notified when the evidence is tested.";
            Window.Close();
            Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.LabFiber, ComputerController.Fibers.MainFiber);
        }
    }*/
}
