using System;
using System.Collections.Generic;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AnimationDatabase;
using LSNoir.Callouts.SA.Data;
using LSNoir.Extensions;
using Rage;
using Rage.Native;

namespace LSNoir.Callouts.Universal
{
    public class Interrogation
    {
        public bool HasEnded { get; set; }
        public float DistanceToStart { get; set; } = 2.0f;
        public Keys InteractKey { get; set; } = Keys.Y;
        public Dictionary<int, bool> QuestionList { get; set; }
        public List<string> InterrgoationText { get; set; }

        public Keys KeyTruth { get; set; } = Keys.D1;
        public Keys KeyDoubt { get; set; } = Keys.D2;
        public Keys KeyLie { get; set; } = Keys.D3;

        private Ped Interrogator => Game.LocalPlayer.Character;
        private Ped Interrogee { get; }

        private GameFiber _lineFiber;

        private static readonly List<Animations> Animation = new List<Animations>
        {
            new Animations("gestures@m@car@van@casual@ps", "gesture_hand_left_three"),
            new Animations("gestures@m@car@van@casual@ps", "gesture_hand_right_three"),
            new Animations("gestures@m@car@van@casual@ps", "gesture_why"),
            new Animations("gestures@m@sitting@generic@casual", "gesture_bring_it_on"),
            new Animations("gestures@m@standing@casual", "gesture_easy_now"),
        };

        private SoundPlayer _correct = new SoundPlayer(@"Plugins\LSPDFR\LSNoir\Audio\Correct.wav");
        private SoundPlayer _incorrect = new SoundPlayer(@"Plugins\LSPDFR\LSNoir\Audio\Incorrect.wav");

        private static readonly string Help = "Using your expert detective skills, pick one of the following:\n~y~ 1 ~w~  Truth\n~y~ 2 ~w~  Doubt\n~y~ 3 ~w~  Lie";

        private InterrogationLine[] _lines;

        private Camera _playerTOsus;
        private Camera _susTOplayer;
        private Camera _gameCam;

        private InterrogationLine.Type _lineTypeCheck = InterrogationLine.Type.Question;
        private int _currentLine = 0;

        public Interrogation(InterrogationLine[] dialog, Ped perp, float startDistance = 2f)
        {
            _lines = dialog;
            Interrogee = perp;
            DistanceToStart = startDistance;
            QuestionList = new Dictionary<int, bool>();
            InterrgoationText = new List<string>();
        }

        public void StartDialog()
        {
            _lineFiber = new GameFiber(ShowLine);
            _lineFiber.Start();
        }

        private void ShowLine()
        {
            try
            {
                ProcessLines();

                Game.LogTrivial("Interpolating camera");
                                
                End();
            }
            catch (Exception e)
            {
                Game.Console.Print("catch: " + e.Message);
                End();
            }
        }

        private void ProcessLines()
        {
            while (Dist(Interrogator, Interrogee) > DistanceToStart)
            {
                GameFiber.Yield();
            }
            
            Interrogator.KeepTasks = true;
            Interrogee.KeepTasks = true;
            NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(Game.LocalPlayer.Character, Interrogee, -1);
            NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(Interrogee, Game.LocalPlayer.Character, - 1);

            GameFiber.Sleep(1000);

            _playerTOsus = CreatePointingCam(Interrogator, Interrogee);

            _susTOplayer = CreatePointingCam(Interrogee, Interrogator);

            Interrogator.IsPositionFrozen = true;

            Interrogee.IsPositionFrozen = true;

            _gameCam = CreateGameCam();

            NativeFunction.Natives.SET_PED_CAN_SWITCH_WEAPON(Interrogator, false);

            while (true)
            {
                GameFiber.Yield();

                if (_lines[_currentLine].LineType != _lineTypeCheck)
                {
                    _currentLine++;
                    continue;
                }

                Game.DisplayHelp("Press ~y~" + InteractKey.ToString() + "~w~ to continue the interrogation.");

                while (true)
                {
                    GameFiber.Yield();
                    if (Game.IsKeyDown(InteractKey))
                    {
                        "Interact Key Pressed".AddLog();
                        Game.HideHelp();
                        break;
                    }
                }

                var cl = _lines[_currentLine];

                Interrogee.IsVisible = false;

                PedPlayRndAnim(Interrogator);

                PlayFacialAnim(Interrogator);

                DisplayLineForPed(Interrogator, cl.PlayerLine, _susTOplayer);

                Interrogee.IsVisible = true;
                Interrogator.IsVisible = false;

                PedPlayRndAnim(Interrogee);

                PlayFacialAnim(Interrogee);

                DisplayLineForPed(Interrogee, cl.PerpLine, _playerTOsus);
                
                if (cl.LineType == InterrogationLine.Type.Question)
                {
                    PlayInterrogeeFacialAnim(cl.CorrectType, Interrogee);

                    Game.DisplayHelp(Help, true);

                    _lineTypeCheck = WaitForPlayersResponse(cl.CorrectType);
                }
                else
                {
                    _lineTypeCheck = InterrogationLine.Type.Question;
                }

                Interrogee.Tasks.Clear();

                Interrogator.IsVisible = true;

                _currentLine++;

                if (_currentLine == _lines.Length)
                {
                    break;
                }
            }
        }

        private InterrogationLine.Type WaitForPlayersResponse(InterrogationLine.Type correctType)
        {
            while (true)
            {
                GameFiber.Yield();

                if (Game.IsKeyDown(KeyTruth))
                {
                    Game.HideHelp();
                    PlayAnswerSound(correctType == InterrogationLine.Type.Truth);
                    return InterrogationLine.Type.Truth;
                }
                else if (Game.IsKeyDown(KeyDoubt))
                {
                    Game.HideHelp();
                    PlayAnswerSound(correctType == InterrogationLine.Type.Doubt);
                    return InterrogationLine.Type.Doubt;
                }
                else if (Game.IsKeyDown(KeyLie))
                {
                    Game.HideHelp();
                    PlayAnswerSound(correctType == InterrogationLine.Type.Lie);
                    return InterrogationLine.Type.Lie;
                }
            }
        }

        private void PlayAnswerSound(bool isCorrect)
        {
            var number = QuestionList.Count + 1;
            QuestionList.Add(number, isCorrect);
            if (isCorrect) _correct.Play();
            else _incorrect.Play();
        }

        private void DisplayLineForPed(Ped p, string line, Camera cam)
        {
            InterrgoationText.Add(line);

            var wordCount = CountWords(line);

            var time = GetDisplayTime(wordCount);

            cam.Active = true;

            Game.DisplaySubtitle(line, time);

            GameFiber.Sleep(time + 100);
            
            p.Tasks.Clear();
        }

        private Camera CreateGameCam()
        {
            var gameCam = new Camera(false);
            gameCam.FOV = NativeFunction.Natives.GET_GAMEPLAY_CAM_FOV<float>();
            gameCam.Position = NativeFunction.Natives.GET_GAMEPLAY_CAM_COORD<Vector3>();
            Vector3 rot = NativeFunction.Natives.GET_GAMEPLAY_CAM_ROT<Vector3>(0);
            var rot1 = new Rotator(rot.X, rot.Y, rot.Z);
            gameCam.Rotation = rot1;
            return gameCam;
        }

        private Camera CreatePointingCam(Ped p1, Ped p2)
        {
            Camera playerTOsus = new Camera(false);
            Vector3 p = p1.LeftPosition;
            Vector3 playerpos = new Vector3(p.X, p.Y, (p.Z + 0.25f));
            playerTOsus.Position = playerpos;
            playerTOsus.PointAtEntity(p2, Vector3.Zero, false);
            return playerTOsus;
        }

        private static void PlayFacialAnim(Ped p)
        {
            p.Tasks.PlayAnimation("mp_facial", "mic_chatter", 3f, AnimationFlags.SecondaryTask);
        }

        private static void PlayInterrogeeFacialAnim(InterrogationLine.Type t, Ped interrogee)
        {
            if (t == InterrogationLine.Type.Truth)
                interrogee.Tasks.PlayAnimation("gestures@m@standing@casual", "gesture_pleased", 4f, AnimationFlags.Loop);
            else if (t == InterrogationLine.Type.Doubt)
                interrogee.Tasks.PlayAnimation("facials@gen_male@variations@injured", "mood_injured_3", 4f, AnimationFlags.Loop);
            else if (t == InterrogationLine.Type.Lie)
                interrogee.Tasks.PlayAnimation("facials@p_m_two@variations@injured", "mood_injured_3", 4f, AnimationFlags.Loop);
        }

        public static int CountWords(string s)
        {
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }

        private static int GetDisplayTime(int wordCount)
        {
            int t = wordCount / 2 * 1000;
            if (t < 3500) return 4000;
            else return t;
        }

        private void PedPlayRndAnim(Ped p)
        {
            int rnd = MathHelper.GetRandomInteger(Animation.Count - 1);
            Animations anim = Animation[rnd];
            p.Tasks.PlayAnimation(anim.ScenarioName, anim.FirstAnimation, anim.BlendInSpeed, anim.AnimationFlag);
        }

        private float DistToPlayer(Vector3 pos)
        {
            return Vector3.Distance(Interrogator.Position, pos);
        }

        private static float Dist(Ped p1, Ped p2)
        {
            return p1.Position.DistanceTo(p2.Position);
        }

        private void End()
        {
            NativeFunction.Natives.SET_PED_CAN_SWITCH_WEAPON(Interrogator, true);

            if (Interrogator) Interrogator.IsPositionFrozen = false;

            if (Interrogee) Interrogee.IsPositionFrozen = false;
            
            if (_susTOplayer.Exists())
            {
                _susTOplayer.Active = false;
                _susTOplayer.Delete();
            }
            if (_playerTOsus.Exists())
            {
                _gameCam.Active = true;
                _playerTOsus.Active = true;

                CamInterpolate(_playerTOsus, _gameCam, 2000, true, true, true);

                _playerTOsus.Active = false;
                _playerTOsus.Delete();
            }

            HasEnded = true;
            "Interrogation Ended".AddLog();
            _lineFiber.Abort();
        }

        private static void CamInterpolate(
            Camera camfrom, Camera camto,
            int totaltime,
            bool easeLocation, bool easeRotation, bool waitForCompletion,
            float x = 0f, float y = 0f, float z = 0f)
        {
            NativeFunction.Natives.SET_CAM_ACTIVE_WITH_INTERP(
                camto, camfrom,
                totaltime, easeLocation, easeRotation);

            if (waitForCompletion) GameFiber.Sleep(totaltime);
        }
    }
}

