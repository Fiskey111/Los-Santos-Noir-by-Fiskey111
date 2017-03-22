using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSNoir.Callouts.SA.Commons;
using LSNoir.Callouts.SA.Computer;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using static LtFlash.Common.Serialization.Serializer;
using Marker = Fiskey111Common.Marker;

namespace LSNoir.Callouts
{
    public class Sa_2CStation : BasicScript
    {
        private Vector3 _position;
        private Blip _StationBlip;
        public static bool OpenedForm, _startedComp, notified;
        private static CaseData _cData;

        protected override bool Initialize()
        {
            "Initializing L.S. Noir Callout: Sexual Assault -- Stage 2c [Police Station]".AddLog();
            _position = GetNearestStation(Game.LocalPlayer.Character.Position);
            ExtensionMethods.LogDistanceFromCallout(_position);

            var caseData = LoadItemFromXML<CaseData>(Main.CDataPath);
            caseData.ComputerAccess = false;
            SaveItemToXML<CaseData>(caseData, Main.CDataPath);

            _StationBlip = new Blip(_position)
            {
                Sprite = BlipSprite.GangAttackPackage,
                Color = Color.DarkOrange,
                Name = "Visit Station"
            };
            _cData = LoadItemFromXML<CaseData>(Main.CDataPath);

            "Sexual Assault Case Update".DisplayNotification("Visit a ~o~police station~w~ and view the SAJRS");

            return true;
        }

        private Vector3 GetNearestStation(Vector3 position)
        {
            Vector3 station = new Vector3();
            List<Vector3> stations = new List<Vector3>();
            stations.Add(new Vector3(1853, 3690, 34)); // PB?
            stations.Add(new Vector3(-449, 6012, 32)); // SS?
            stations.Add(new Vector3(460, -989, 25)); // Main LSPD Station
            float closest = 100000f;

            foreach (Vector3 sp in stations)
            {
                if (!(sp.DistanceTo(position) < closest)) continue;
                closest = sp.DistanceTo(position);
                station = sp;
            }
            return station;
        }
        int _marker = 1;
        Vector3 _one = new Vector3(434, -982, 31);
        Vector3 _two = new Vector3(445, -988, 31);
        Vector3 _three = new Vector3(447, -994, 31);
        Vector3 _four = new Vector3(464, -985, 26);
        bool _breakit, _onscene;

        protected override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(_position) < 35f && _position == new Vector3(460, -989, 25) && !_onscene)
            {
                _onscene = true;
                if (_StationBlip) _StationBlip.Delete();
                while (true)
                {
                    switch (_marker)
                    {
                        case 1:
                            {
                                Marker.StartMarker(_one, Color.Yellow, true);
                                _marker = 2;
                                goto case 2;
                            }
                        case 2:
                        {
                            if (Game.LocalPlayer.Character.Position.DistanceTo(_one) < 2f)
                                {
                                    Marker.CreateMarker.Abort();
                                    _marker = 3;
                                    goto case 3;
                                }
                            break;
                        }
                        case 3:
                            {
                                Marker.StartMarker(_two, Color.Yellow, true);
                                _marker = 4;
                                goto case 4;
                            }
                        case 4:
                        {
                            if (Game.LocalPlayer.Character.Position.DistanceTo(_two) < 2f)
                                {
                                    Marker.CreateMarker.Abort();
                                    _marker = 5;
                                    goto case 5;
                                }
                            break;
                        }
                        case 5:
                            {
                                Marker.StartMarker(_three, Color.Yellow, true);
                                _marker = 6;
                                goto case 6;
                            }
                        case 6:
                        {
                            if (Game.LocalPlayer.Character.Position.DistanceTo(_three) < 2f)
                                {
                                    Marker.CreateMarker.Abort();
                                    _marker = 7;
                                    goto case 7;
                                }
                            break;
                        }
                        case 7:
                            {
                                Marker.StartMarker(_four, Color.Yellow, true);
                                _marker = 8;
                                goto case 8;
                            }
                        case 8:
                        {
                            if (Game.LocalPlayer.Character.Position.DistanceTo(_four) < 2f)
                                {
                                    Marker.CreateMarker.Abort();
                                    _marker = 9;
                                    _breakit = true;
                                }
                            break;
                        }
                    }
                    if (_breakit)
                    {
                        Marker.StartMarker(_position, Color.Yellow, true);
                        break;
                    }
                    GameFiber.Yield();
                }
            }

            if (Game.LocalPlayer.Character.Position.DistanceTo(_position) <= 25 && !_onscene && _position != new Vector3(460, -989, 25))
            {
                _onscene = true;
                if (_StationBlip) _StationBlip.Scale = 0.75f;
                Marker.StartMarker(_position, Color.Yellow);
            }

            if (Game.LocalPlayer.Character.Position.DistanceTo(_position) < 1.5f)
            {
                if (!notified)
                {
                    notified = true;
                    Game.DisplayHelp($"Press {Settings.ComputerKey()} to open the computer", true);
                }
                if (Game.IsKeyDown(Settings.ComputerKey()) && !_startedComp)
                {
                    OpenedForm = true;
                    _startedComp = true;
                    Game.IsPaused = true;
                    Computer.StartComputerHandler();

                    while (Computer.IsRunning)
                        GameFiber.Yield();
                    "IsRunning = false".AddLog();
                    Game.IsPaused = false;
                    _startedComp = false;
                }
            }
            if (OpenedForm && Game.LocalPlayer.Character.Position.DistanceTo(_position) >= 30f)
            {
                Game.HideHelp();
                var susAcq = false;
                _cData = LoadItemFromXML<CaseData>(Main.CDataPath);
                if (!string.IsNullOrWhiteSpace(_cData.CurrentSuspect))
                {
                    var susDataList = LoadFromXML<PedData>(Main.SDataPath);
                    foreach (var s in susDataList)
                    { 
                        if (s.Name != _cData.CurrentSuspect || !s.IsPerp) continue;

                        this.Attributes.NextScripts.Clear();
                        this.Attributes.NextScripts.Add("Sa_4ASuspectHome");
                        susAcq = true;
                    }
                }

                var value = susAcq ? 100 : 50;
                var medal = value < 100 ? MissionPassedScreen.Medal.Silver :
                MissionPassedScreen.Medal.Gold;

                var handler = new MissionPassedHandler("San Andreas Joint Records System", value, medal);

                var tick = susAcq ? MissionPassedScreen.TickboxState.Tick : MissionPassedScreen.TickboxState.Empty;

                handler.AddItem("Viewed SAJRS", "Details Received", MissionPassedScreen.TickboxState.Tick);
                handler.AddItem("Suspect Acquired", "", tick);

                handler.Show();

                SetScriptFinished();
            }
        }

        protected override void End()
        {

        }

        protected void SetScriptFinished()
        {
            Game.DisplayHelp("You are able to access this computer at any time at three locations around Los Santos County (LSPD Mission Row Station, Sandy Shores PD Station, and Paleto Bay PD Station).");
            _cData.CurrentStage = CaseData.LastStage.Station;
            _cData.LastCompletedStage = CaseData.LastStage.Station;
            _cData.CompletedStages.Add(CaseData.LastStage.Station);
            _cData.SajrsUpdates.Add("Viewed SAJRS");
            _cData.ComputerAccess = true;
            _cData.StartingStage = this.Attributes.NextScripts.FirstOrDefault();
            SaveItemToXML<CaseData>(_cData, Main.CDataPath);
            Main.CompAccess = true;
            SetScriptFinished(true);
        }

    }
}
