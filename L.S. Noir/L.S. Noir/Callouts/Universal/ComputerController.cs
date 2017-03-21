using System.Linq;
using LSNoir.Callouts.SA.Commons;
using LSNoir.Callouts.SA.Computer;
using LSNoir.Extensions;
using Rage;

namespace LSNoir.Callouts.Universal
{
    internal class ComputerController
    {
        internal bool IsRunning { get; set; }
        internal static string PlateNumber { get; set; }

        internal GameFiber EvidenceFiber = new GameFiber(EvidenceVoid),
            LabFiber = new GameFiber(LabVoid),
            MainFiber = new GameFiber(MainVoid),
            MessageBoxFiber = new GameFiber(MessageVoid),
            ReportFiber = new GameFiber(ReportVoid),
            VictimFiber = new GameFiber(VictimVoid),
            WarrantFiber = new GameFiber(WarrantVoid),
            WarrantRequestFiber = new GameFiber(WarrantReqVoid),
            WitnessFiber = new GameFiber(WitnessVoid),
            SecurityCamFiber = new GameFiber(SecurityVoid);

        private static CaseData CData;

        internal ComputerController() { }

        internal void StartFiber(Fibers fiber, CaseData data = null)
        {
            Background.EnableBackground("form_background.jpg");
            if (data != null) CData = data;
            $"Starting {fiber}".AddLog(true);
            switch (fiber)
            {
                case Fibers.EvidenceFiber:
                    EvidenceFiber = new GameFiber(EvidenceVoid);
                    EvidenceFiber.Start();
                    break;
                case Fibers.LabFiber:
                    LabFiber = new GameFiber(LabVoid);
                    LabFiber.Start();
                    break;
                case Fibers.MainFiber:
                    MainFiber = new GameFiber(MainVoid);
                    MainFiber.Start();
                    break;
                case Fibers.MessageBoxFiber:
                    MessageBoxFiber = new GameFiber(MessageVoid);
                    MessageBoxFiber.Start();
                    break;
                case Fibers.ReportFiber:
                    ReportFiber = new GameFiber(ReportVoid);
                    ReportFiber.Start();
                    break;
                case Fibers.VictimFiber:
                    VictimFiber = new GameFiber(VictimVoid);
                    VictimFiber.Start();
                    break;
                case Fibers.WarrantFiber:
                    WarrantFiber = new GameFiber(WarrantVoid);
                    WarrantFiber.Start();
                    break;
                case Fibers.WarrantRequestFiber:
                    WarrantRequestFiber = new GameFiber(WarrantReqVoid);
                    WarrantRequestFiber.Start();
                    break;
                case Fibers.WitnessFiber:
                    WitnessFiber = new GameFiber(WitnessVoid);
                    WitnessFiber.Start();
                    break;
                case Fibers.SecurityCamFiber:
                    SecurityCamFiber = new GameFiber(SecurityVoid);
                    SecurityCamFiber.Start();
                    break;
            }
            IsRunning = true;
        }

        internal enum Fibers { EvidenceFiber, LabFiber, MainFiber, MessageBoxFiber, ReportFiber, VictimFiber, WarrantFiber, WarrantRequestFiber, WitnessFiber, SecurityCamFiber }

        internal void SwitchFibers(GameFiber stopFiber, Fibers startFiber, CaseData data = null)
        {
            $"Switching fiber {stopFiber} to {startFiber}".AddLog();
            if (stopFiber.IsAlive) stopFiber.Abort();
            StartFiber(startFiber, data);
        }

        internal void AbortFiber(GameFiber abortFiber)
        {
            $"Aborting fiber {abortFiber.Name}".AddLog();
            Background.DisableBackground(Background.Type.Computer);
            IsRunning = false;
            if (abortFiber.IsAlive) abortFiber.Abort();
        }
            
        private static void EvidenceVoid()
        {
            var form = new EvidenceCode();
            form.Show();
        }

        private static void LabVoid()
        {
            var form = new LabCode();
            form.Show();
        }

        private static void MainVoid()
        {
            var form = new MainCode();
            form.Show();
        }

        private static void MessageVoid()
        {
            var form = new MessageBoxCode();
            form.Show();
        }

        private static void ReportVoid()
        {
            var form = new ReportsCode();
            form.Show();
        }

        private static void VictimVoid()
        {
            var form = new VictimCode();
            form.Show();
        }

        private static void WarrantVoid()
        {
            var form = new WarrantCode();
            form.Show();
        }

        private static void WarrantReqVoid()
        {
            var form = new WarrantRequestCode();
            form.Show();
        }

        private static void WitnessVoid()
        {
            var form = new WitnessCode();
            form.Show();
        }

        private static void SecurityVoid()
        {
            Background.DisableBackground(Background.Type.Computer);
            GameFiber.StartNew(delegate
            {
                if (CData == null) return;

                SecurityCamera.SecurityCameraStart(CData);

                while (SecurityCamera.IsRunning)
                    GameFiber.Yield();

                Background.EnableBackground("form_background.jpg");
                if (!string.IsNullOrWhiteSpace(PlateNumber))
                {
                    var susData =
                        LtFlash.Common.Serialization.Serializer.GetSelectedListElementFromXml<PedData>(Main.SDataPath,
                            c => c.FirstOrDefault(s => s.IsPerp));
                    MessageBoxCode.Message =
                        $"License Plate Detected: {PlateNumber}\nRegisterd to: {susData.Name}  Gender: {susData.Gender}";
                    Computer.Controller.SwitchFibers(Computer.Controller.SecurityCamFiber, Fibers.MessageBoxFiber);
                }
                else
                {
                    Computer.Controller.SwitchFibers(Computer.Controller.SecurityCamFiber, Fibers.MainFiber);
                }
            });
        }
    }
}
