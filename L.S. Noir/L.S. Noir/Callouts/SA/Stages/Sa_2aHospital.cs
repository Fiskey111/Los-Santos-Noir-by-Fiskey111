/*namespace LSNoir.Callouts
{
    public class Sa_2aHospital : BasicScript
    {
        private Vector3 _position;
        private Blip _areaBlip;
        private Ped _doc;
        private string _vicTraces;
        private bool _vicImportant = false;
        private string[] _docreport;
        private ELocation _state;
        private EDialog _dialogstate;
        private Object _notepad;
        private LtFlash.Common.EvidenceLibrary.Dialog _dialog;
        private PedData _victim;
        private CaseData _case;

        protected override bool Initialize()
        {
            "Initializing L.S. Noir Callout: Sexual Assault -- Stage 2a [Hospital]".AddLog();
            ExtensionMethods.LogDistanceFromCallout(_position);
            _position = GetNearestHospital(Game.LocalPlayer.Character.Position);
            _areaBlip = new Blip(_position, 10f);
            _areaBlip.Color = System.Drawing.Color.Green;
            _areaBlip.EnableRoute(System.Drawing.Color.Yellow);

            _victim = PedSerializer.GetPedData(PedType.Victim);
            _case = CaseSerializer.GetData();

            "Sexual Assault Case Update".DisplayNotification("Visit Hospital for Update");

            _doc = new Ped("s_m_m_doctor_01", _position, 2f);
            _doc.RandomizeVariation();
            
            _state = ELocation.Dispatched;

            AddStage(EnRoute);
            AddStage(Close);
            AddStage(VeryClose);
            AddStage(Leaving);
            return true;
        }

        public static Vector3 GetNearestHospital(Vector3 target)
        {
            Vector3 myHospital = new Vector3();
            List<Vector3> hospitals = new List<Vector3>();
            hospitals.Add(new Vector3(-454, -340, 34));
            hospitals.Add(new Vector3(296, -1442, 29));
            hospitals.Add(new Vector3(1827, 3693, 34));
            hospitals.Add(new Vector3(-242, -6337, 32));
            float closestRange = 100000;

            foreach (Vector3 sp in hospitals)
            {
                if (sp.DistanceTo(target) < closestRange)
                {
                    closestRange = sp.DistanceTo(target);
                    myHospital = sp;
                }
            }
            return myHospital;
        }

        protected override void Process()
        {
            if (Vector3.Distance(Game.LocalPlayer.Character.Position, _position) < 80f && _state == ELocation.Dispatched)
            {
                ActivateStage(EnRoute);
            }
            if (Vector3.Distance(Game.LocalPlayer.Character.Position, _position) <15f && _state == ELocation.Within80)
            {
                ActivateStage(Close);
            }
            if (Vector3.Distance(Game.LocalPlayer.Character.Position, _position) < 5f && _state == ELocation.Within15 && Game .LocalPlayer.Character.IsOnFoot)
            {
                ActivateStage(VeryClose);
            }
        }

        #region Stages
        public void EnRoute()
        {
            _state = ELocation.Within80;
            _doc.Tasks.PlayAnimation("amb@medic@standing@timeofdeath@idle_a", "idle_b", 4, AnimationFlags.Loop);
            "Within 80".AddLog();
            _notepad = new Object("prop_notepad_01", _doc.Position);
            int boneId = _doc.GetBoneIndex(PedBoneId.LeftPhHand);
            NativeFunction.CallByName<uint>("ATTACH_ENTITY_TO_ENTITY", _notepad, _doc, boneId, 0f, 0f, 0f, 0f, 0f, 0f, true, false, false, false, 2, 1);
            DeactivateStage(EnRoute);
        }

        public void Close()
        {
            _state = ELocation.Within15;
            _areaBlip.Delete();
            Game.DisplayHelp("Approach the ~g~doctor~w~ to get information regarding the victim.");
            _dialogstate = EDialog.Pre;
            DeactivateStage(Close);
        }

        public void PrettyClose()
        {
            GameFiber.StartNew(delegate
            {
                if (_areaBlip.Exists()) _areaBlip.Delete();
                _state = ELocation.Within5;
                _doc.Tasks.PlayAnimation("amb@medic@standing@timeofdeath@exit", "exit", 4, AnimationFlags.None);
                GameFiber.Sleep(7000);
                GameFiber.Sleep(7000);
                if (_notepad.Exists()) _notepad.Delete();
                NativeFunction.CallByName<uint>("TASK_TURN_PED_TO_FACE_ENTITY", _doc, Game.LocalPlayer.Character, 1000);
                "Swapping Stages Now".AddLog();
                SwapStages(PrettyClose, VeryClose);
            });
        }

        public void VeryClose()
        {
            if (_dialogstate == EDialog.Pre)
            {
                _dialogstate = EDialog.During;
                "Dialog Starting".AddLog();
                _dialog.StartDialog(_doc, Game.LocalPlayer.Character);
            }

            if (_dialog.HasEnded && _dialogstate == EDialog.During)
            {
                "Dialog Ending".AddLog();
                _dialogstate = EDialog.Post;
                int l = Fiskey111Common.RandomNumberGenerator.RandomNumber().Next(1, 10);
                if (l == 1)
                {
                    _doc.Tasks.PlayAnimation("amb@world_human_smoking@male@male_a@idle_a", "idle_b", 4, AnimationFlags.Loop);
                    "Leaving".AddLog();
                    _notepad = new Object("prop_cs_ciggy_01", _doc.Position);
                    int boneId = _doc.GetBoneIndex(PedBoneId.RightPhHand);
                    NativeFunction.CallByName<uint>("ATTACH_ENTITY_TO_ENTITY", _notepad, _doc, boneId, 0f, 0f, 0f, 0f, 0f, 0f, true, false, false, false, 2, 1);
                }
                else
                {
                    _doc.Tasks.PlayAnimation("amb@medic@standing@timeofdeath@idle_a", "idle_b", 4, AnimationFlags.Loop);
                    "Leaving".AddLog();
                    _notepad = new Object("prop_notepad_01", _doc.Position);
                    int boneId = _doc.GetBoneIndex(PedBoneId.LeftPhHand);
                    NativeFunction.CallByName<uint>("ATTACH_ENTITY_TO_ENTITY", _notepad, _doc, boneId, 0f, 0f, 0f, 0f, 0f, 0f, true, false, false, false, 2, 1);
                }
                SwapStages(VeryClose, Leaving);
            }
        }

        public void Leaving()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(_doc) < 30f)
            {
                if (_doc.Exists()) _doc.Tasks.Clear();
                if (_doc.Exists()) _doc.Dismiss();
                if (_notepad.Exists()) _notepad.Delete();
                SetScriptFinished();
            }
        }
        #endregion

        protected override void End()
        {

        }

        protected void SetScriptFinished()
        {
            _case.LastCompletedStage = CaseData.LastStage.Hospital;
            _case.SajrsUpdates.Add("Hospital Physician Report Added");
            _case.StartingStage = this.Attributes.NextScripts.FirstOrDefault();
            CaseSerializer.SaveCaseData(CaseSerializer.DataType.Case, _case);
            "Sexual Assault Case Update".DisplayNotification("Hospital Conversation \nAdded to ~b~SAJRS");
            DeactivateStage(Leaving);
            SetScriptFinished(true);
        }

        public enum ELocation
        {
            Dispatched, Within80, Within15, Within5, Done
        }
        public enum EDialog
        {
            Pre, During, Post
        }
    }
}*/
