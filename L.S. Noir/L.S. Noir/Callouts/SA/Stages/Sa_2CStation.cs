using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Fiskey111Common;
using LSNoir.Callouts.SA.Commons;
using LSNoir.Callouts.SA.Data;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using LtFlash.Common.ScriptManager.Scripts;
using LtFlash.Common.Serialization;
using Rage;
using Debug = System.Diagnostics.Debug;
using Marker = Fiskey111Common.Marker;

namespace LSNoir.Callouts.SA.Stages
{
    public class Sa_2CStation : BasicScript
    {
        private Vector3 _position;
        private Blip _stationBlip;
        private static bool _startedComp;
        private static CaseData _cData;
        private Marker _marker;
        private Vector3 _playerPos => Game.LocalPlayer.Character.Position;
        private List<Vector3> _markerPos = new List<Vector3>();
        private PoliceStation _station = new PoliceStation(Common.PlayerPos);
        private int _markNum = 1;

        protected override bool Initialize()
        {
            "Initializing L.S. Noir Callout: Sexual Assault -- Stage 2c [Police Station]".AddLog();
            _position = _station.Position;
            
            _stationBlip = new Blip(_position)
            {
                Sprite = BlipSprite.GangAttackPackage,
                Color = Color.DarkOrange,
                Name = "Visit Station"
            };
            _cData = Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);
            $"Case number: {_cData.Number}".AddLog();

            "Sexual Assault Case Update".DisplayNotification("Visit a ~o~police station~w~ and view the SAJRS", _cData.Number);

            if (_station.Location == StationLocation.Downtown)
            {
                _markerPos.Add(new Vector3(434, -982, 31));
                _markerPos.Add(new Vector3(445, -988, 31));
                _markerPos.Add(new Vector3(447, -994, 31));
                _markerPos.Add(new Vector3(464, -985, 26));

                "MarkerRun".AddLog();
                ActivateStage(MarkerRun);
            }
            else
                ActivateStage(CloseToComputer);

            return true;
        }

        private void MarkerRun()
        {
            if (_playerPos.DistanceTo(_station.Position) > 30f) return;
            if (_playerPos.DistanceTo(_station.Position) <= 5f) SwapStages(MarkerRun, CloseToComputer);
            switch (_markNum)
            {
                case 1:
                    _marker = new Marker(_markerPos[0], Color.Yellow, Marker.MarkerTypes.MarkerTypeUpsideDownCone, true, true,
                    true);
                    "1".AddLog();
                    _markNum++;
                    break;
                case 2:
                    if (_playerPos.DistanceTo(_markerPos[0]) > 1.5f) break;
                    "2".AddLog();
                    if (_marker.Exists) _marker.Position = _markerPos[1];
                    _markNum++;
                    break;
                case 3:
                    if (_playerPos.DistanceTo(_markerPos[1]) > 1.5f) break;
                    "3".AddLog();
                    if (_marker.Exists) _marker.Position = _markerPos[2];
                    _markNum++;
                    break;
                case 4:
                    if (_playerPos.DistanceTo(_markerPos[2]) > 1.5f) break;
                    "4".AddLog();
                    if (_marker.Exists) _marker.Position = _markerPos[3];
                    _markNum++;
                    break;
                case 5:
                    if (_playerPos.DistanceTo(_markerPos[3]) > 1.5f) break;
                    "5".AddLog();
                    _markNum++;
                    break;
                default:
                    "Swapping".AddLog();
                    SwapStages(MarkerRun, CloseToComputer);
                    break;
            }
        }

        private void CloseToComputer()
        {
            if (_playerPos.DistanceTo(_station.Position) > 5f) return;
            if (_marker.Exists) _marker.Stop();
            _marker = new Marker(_station.Position, Color.Yellow, Marker.MarkerTypes.MarkerTypeUpsideDownCone, true, true,
                    true);
            SwapStages(CloseToComputer, AtComputer);
        }

        private void AtComputer()
        {
            if (_playerPos.DistanceTo(_station.Position) > 2f) return;
            if (_marker.Exists) _marker.Stop();
            Game.DisplayHelp($"Press {Settings.Settings.ComputerKey()} to open the computer");
            SwapStages(AtComputer, WaitForComputerStart);
        }

        private void WaitForComputerStart()
        {
            LeftStationCheck();
            if (_playerPos.DistanceTo(_station.Position) > 1.5f) return;


            if (!Game.IsKeyDown(Settings.Settings.ComputerKey()) || _startedComp) return;

            _startedComp = true;
            Game.IsPaused = true;
            Universal.Computer.StartComputerHandler();

            while (Universal.Computer.IsRunning)
                GameFiber.Yield();
            "IsRunning = false".AddLog();
            Game.IsPaused = false;
            _startedComp = false;
        }

        private void LeftStationCheck()
        {
            if (_playerPos.DistanceTo(_station.Position) > 25f)
            {
                SwapStages(WaitForComputerStart, RunEndStage);
            }
        }

        private void RunEndStage()
        {
            var susAcq = false;
            _cData = Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);
            if (!string.IsNullOrWhiteSpace(_cData.CurrentSuspect))
            {
                var susDataList = Serializer.LoadFromXML<PedData>(Main.SDataPath);
                foreach (var s in susDataList)
                {
                    if (s.Name != _cData.CurrentSuspect || !s.IsPerp) continue;

                    this.Attributes.NextScripts.Clear();
                    this.Attributes.NextScripts.Add("Sa_4ASuspectHome");
                    susAcq = true;
                }
            }

            var value = susAcq ? 100 : 50;
            var medal = value < 100
                ? MissionPassedScreen.Medal.Silver
                : MissionPassedScreen.Medal.Gold;

            var handler = new MissionPassedHandler("San Andreas Joint Records System", value, medal);

            var tick = susAcq ? MissionPassedScreen.TickboxState.Tick : MissionPassedScreen.TickboxState.Empty;

            handler.AddItem("Viewed SAJRS", "Details Received", MissionPassedScreen.TickboxState.Tick);
            handler.AddItem("Suspect Acquired", "", tick);

            $"Case number: {_cData.Number}".AddLog();
            handler.Show();

            SetScriptFinished();
        }

        protected override void Process()
        {
        }

        protected override void End()
        {

        }

        protected void SetScriptFinished()
        {
            Game.DisplayHelp(
                "You are able to access this computer at any time at three locations around Los Santos County (LSPD Mission Row Station, Sandy Shores PD Station, and Paleto Bay PD Station)");
            _cData.CurrentStage = CaseData.LastStage.Station;
            _cData.LastCompletedStage = CaseData.LastStage.Station;
            _cData.CompletedStages.Add(CaseData.LastStage.Station);
            _cData.SajrsUpdates.Add("Viewed SAJRS");
            _cData.ComputerAccess = true;
            _cData.StartingStage = this.Attributes.NextScripts.FirstOrDefault();
            Serializer.SaveItemToXML<CaseData>(_cData, Main.CDataPath);
            Main.CompAccess = true;
            SetScriptFinished(true);
        }
    }
}
