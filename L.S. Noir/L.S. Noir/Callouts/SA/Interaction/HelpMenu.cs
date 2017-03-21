/*namespace LSNoir.Callouts.SA.Commons
{
    public static class CsiScreen
    {
        // System
        public static string CsiAccepted = "Hidden", CsiOnscene = "Hidden", CsiTalkedtofo = "Hidden", CsiExaminedbody = "Hidden",
            CsiEmsdone = "Hidden", CsiCoronerdone = "Hidden", CsiEvidencedone = "Hidden", CsiCasedone = "Hidden";
        public static string CsiAcceptedResult = "", CsiOnsceneResult = "", CsiTalkedtofoResult = "", CsiExaminedbodyResult = "",
            CsiEmsdoneResult = "", CsiCoronerdoneResult = "", CsiEvidencedoneResult = "", CsiCasedoneResult = "", CsiCompletedResult = "";
        private static bool _viewed, _loadedMenu;

        // Gwen
        private static TabView _tabView;
        private static List<MissionInformation> _missionsInfo;
        private static TabMissionSelectItem _missionSelectTab;

        public static void Main()
        {
            Game.FrameRender += Process;

            "Starting Menu".AddLog();
            _tabView = new TabView("L.S. Noir Help Menu");
            "Starting Menu".AddLog();
            
            //UpdateMenu();

            List<MissionInformation> missionsInfo = new List<MissionInformation>()
                    {
                    new MissionInformation("Crime Scene Investigation" + CsiCompletedResult, new Tuple<string, string>[] { new Tuple<string, string>(CsiAccepted, CsiAcceptedResult), new Tuple<string, string>(CsiOnscene, CsiOnsceneResult),
                        new Tuple<string, string>(CsiTalkedtofo, CsiTalkedtofoResult), new Tuple<string, string>(CsiExaminedbody, CsiExaminedbodyResult), new Tuple<string, string>(CsiEmsdone, CsiEmsdoneResult),
                        new Tuple<string, string>(CsiCoronerdone, CsiCoronerdoneResult), new Tuple<string, string>(CsiEvidencedone, CsiEvidencedoneResult), new Tuple<string, string>(CsiCasedone, CsiCasedoneResult)}),
                    };
            _tabView.Tabs.Add(_missionSelectTab = new TabMissionSelectItem("Case #: ~y~" + _case.Number.ToString(), missionsInfo));

            /*
            missionsInfo = new List<MissionInformation>()
            {
                new MissionInformation("Crime Scene Investigation", new Tuple<string, string>[] { new Tuple<string, string>("Case Accepted", CSI_accepted), new Tuple<string, string>("Case Accepted", CSI_accepted) }),
            };
            tabView.Tabs.Add(missionSelectTab = new TabMissionSelectItem("Case #: ~y~" + Serializer.Serializer.GetCaseNumber().ToString(), missionsInfo));
            #1#

            _tabView.RefreshIndex();
            _tabView.Update();

            while (true)
            {
                GameFiber.Yield();
            }
        }

        //new Tuple<string, string>("Arrived on Scene:", CSI_onscene), new Tuple<string, string>("Arrived on Scene:", CSI_onscene)
        public static void Process(object sender, GraphicsEventArgs e)
        {
            if (LSNoir.Main.MenuViewed == true && _loadedMenu == false) // Our menu on/off switch.
            {
                _loadedMenu = true;
                _viewed = true;
                "F5 Pressed".AddLog();

                _tabView.Visible = !_tabView.Visible;
            }
            _tabView.Update();
            if (!_tabView.Visible && _viewed == true)
            {
                "Disabling Process".AddLog();
                _viewed = false;
                LSNoir.Main.MenuViewed = false;
                ("Main.menuViewed = " + LSNoir.Main.MenuViewed.ToString()).AddLog();
                _loadedMenu = false;
                Game.FrameRender -= Process;
            }
        }

        /*
        private static void UpdateMenu()
        {
            if (Serializer.Serializer.GetCurrentStage() == "CSI")
            {
                CSI_completed_result = "- ~y~In Progress";
                if (Serializer.Serializer.GetStageHelper() == Serializer.Serializer.EHelper.Dispatched)
                {
                    CSI_accepted = "Case Accepted";
                    CSI_accepted_result = "~r~Not Completed";
                }
                else if (Serializer.Serializer.GetStageHelper() == Serializer.Serializer.EHelper.Accepted)
                {
                    CSI_accepted = "Case Accepted";
                    CSI_onscene = "Arrived on Scene";
                    CSI_accepted_result = "~g~Completed";
                    CSI_onscene_result = "~r~Not Completed";
                }
                else if (Serializer.Serializer.GetStageHelper() == Serializer.Serializer.EHelper.OnScene)
                {
                    CSI_accepted = "Case Accepted";
                    CSI_onscene = "Arrived on Scene";
                    CSI_talkedtofo = "Spoke to First Officer";
                    CSI_accepted_result = "~g~Completed";
                    CSI_onscene_result = "~g~Completed";
                    CSI_talkedtofo_result = "~r~Not Completed";
                }
                else if (Serializer.Serializer.GetStageHelper() == Serializer.Serializer.EHelper.FO)
                {
                    CSI_accepted = "Case Accepted";
                    CSI_onscene = "Arrived on Scene";
                    CSI_talkedtofo = "Spoke to First Officer";
                    CSI_examinedbody = "Examined Victim's Body";
                    CSI_accepted_result = "~g~Completed";
                    CSI_onscene_result = "~g~Completed";
                    CSI_talkedtofo_result = "~g~Completed";
                    CSI_examinedbody_result = "~r~Not Completed";
                }
                else if (Serializer.Serializer.GetStageHelper() == Serializer.Serializer.EHelper.ExaminedBody)
                {
                    CSI_accepted = "Case Accepted";
                    CSI_onscene = "Arrived on Scene";
                    CSI_talkedtofo = "Spoke to First Officer";
                    CSI_examinedbody = "Examined Victim's Body";
                    CSI_emsdone = "Spoke to EMS";
                    CSI_accepted_result = "~g~Completed";
                    CSI_onscene_result = "~g~Completed";
                    CSI_talkedtofo_result = "~g~Completed";
                    CSI_examinedbody_result = "~g~Completed";
                    CSI_emsdone_result = "~r~Not Completed";
                }
                else if (Serializer.Serializer.GetStageHelper() == Serializer.Serializer.EHelper.EMS)
                {
                    CSI_accepted = "Case Accepted";
                    CSI_onscene = "Arrived on Scene";
                    CSI_talkedtofo = "Spoke to First Officer";
                    CSI_examinedbody = "Examined Victim's Body";
                    CSI_emsdone = "Spoke to EMS";
                    if (SA_1_CSI.emstransport)
                    {
                        CSI_coronerdone = "Called Coroner";
                        CSI_coronerdone_result = "~y~Not required";
                    }
                    else
                    {
                        CSI_coronerdone = "Called Coroner";
                        CSI_coronerdone_result = "~r~Not completed";
                    }
                    CSI_accepted_result = "~g~Completed";
                    CSI_onscene_result = "~g~Completed";
                    CSI_talkedtofo_result = "~g~Completed";
                    CSI_examinedbody_result = "~g~Completed";
                    CSI_emsdone_result = "~g~Completed";
                }
                else if (Serializer.Serializer.GetStageHelper() == Serializer.Serializer.EHelper.Coroner)
                {
                    CSI_accepted = "Case Accepted";
                    CSI_onscene = "Arrived on Scene";
                    CSI_talkedtofo = "Spoke to First Officer";
                    CSI_examinedbody = "Examined Victim's Body";
                    CSI_emsdone = "Spoke to EMS";
                    if (SA_1_CSI.emstransport)
                    {
                        CSI_coronerdone = "Called Coroner";
                        CSI_coronerdone_result = "~y~Not required";
                    }
                    else
                    {
                        CSI_coronerdone = "Called Coroner";
                        CSI_coronerdone_result = "~g~Completed";
                    }
                    CSI_evidencedone = "Searched for Evidence";
                    CSI_accepted_result = "~g~Completed";
                    CSI_onscene_result = "~g~Completed";
                    CSI_talkedtofo_result = "~g~Completed";
                    CSI_examinedbody_result = "~g~Completed";
                    CSI_emsdone_result = "~g~Completed";
                    CSI_evidencedone_result = "~y~In Progress";
                }
                else if (Serializer.Serializer.GetStageHelper() == Serializer.Serializer.EHelper.Evidence)
                {
                    CSI_accepted = "Case Accepted";
                    CSI_onscene = "Arrived on Scene";
                    CSI_talkedtofo = "Spoke to First Officer";
                    CSI_examinedbody = "Examined Victim's Body";
                    CSI_emsdone = "Spoke to EMS";
                    if (SA_1_CSI.emstransport)
                    {
                        CSI_coronerdone = "Called Coroner";
                        CSI_coronerdone_result = "~y~Not required";
                    }
                    else
                    {
                        CSI_coronerdone = "Called Coroner";
                        CSI_coronerdone_result = "~g~Completed";
                    }
                    CSI_evidencedone = "Searched for Evidence";
                    CSI_casedone = "Left the Scene";
                    CSI_accepted_result = "~g~Completed";
                    CSI_onscene_result = "~g~Completed";
                    CSI_talkedtofo_result = "~g~Completed";
                    CSI_examinedbody_result = "~g~Completed";
                    CSI_emsdone_result = "~g~Completed";
                    CSI_evidencedone_result = "~g~Completed";
                    CSI_casedone_result = "~r~Not Completed";
                }
                else if (Serializer.Serializer.GetStageHelper() == Serializer.Serializer.EHelper.CaseDone)
                {
                    CSI_accepted = "Case Accepted";
                    CSI_onscene = "Arrived on Scene";
                    CSI_talkedtofo = "Spoke to First Officer";
                    CSI_examinedbody = "Examined Victim's Body";
                    CSI_emsdone = "Spoke to EMS";
                    if (SA_1_CSI.emstransport)
                    {
                        CSI_coronerdone = "Called Coroner";
                        CSI_coronerdone_result = "~y~Not required";
                    }
                    else
                    {
                        CSI_coronerdone = "Called Coroner";
                        CSI_coronerdone_result = "~g~Completed";
                    }
                    CSI_evidencedone = "Searched for Evidence";
                    CSI_casedone = "Left the Scene";
                    CSI_accepted_result = "~g~Completed";
                    CSI_onscene_result = "~g~Completed";
                    CSI_talkedtofo_result = "~g~Completed";
                    CSI_examinedbody_result = "~g~Completed";
                    CSI_emsdone_result = "~g~Completed";
                    CSI_evidencedone_result = "~g~Completed";
                    CSI_casedone_result = "~g~Completed";
                    CSI_completed_result = "- ~g~Completed";
                }
                else
                {

                }
            }
            #1#
        }
    }*/