using LSNoir.Data;
using LtFlash.Common.EvidenceLibrary;
using Rage;
using Rage.Native;
using System;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LSNoir.Stages
{
    internal class Interrogation : IDialog
    {
        public int Questions { get; private set; }
        public int GoodAnswers { get; private set; }

        public bool HasEnded { get; set; }
        public float DistanceToStart { get; set; } = 2.0f;
        public Keys KeyInteract { get; set; } = Keys.Y;

        public Keys KeyTruth { get; set; } = Keys.D1;
        public Keys KeyDoubt { get; set; } = Keys.D2;
        public Keys KeyLie { get; set; } = Keys.D3;

        private Ped Interrogator => Game.LocalPlayer.Character;
        private Ped Interrogee { get; }

        private GameFiber lineFiber;

        private static readonly Animations[] Animation =
        {
            new Animations("gestures@m@car@van@casual@ps", "gesture_hand_left_three"),
            new Animations("gestures@m@car@van@casual@ps", "gesture_hand_right_three"),
            new Animations("gestures@m@car@van@casual@ps", "gesture_why"),
            new Animations("gestures@m@sitting@generic@casual", "gesture_bring_it_on"),
            new Animations("gestures@m@standing@casual", "gesture_easy_now"),
        };

        private SoundPlayer soundCorrect = new SoundPlayer(Settings.Paths.PATH_INTERROGATION_CORRECT_ANSWER);
        private SoundPlayer soundIncorrect = new SoundPlayer(Settings.Paths.PATH_INTERROGATION_INCORRECT_ANSWER);

        private const string Help = "Using your expert detective skills, pick one of the following:\n~y~ 1 ~s~  Truth\n~y~ 2 ~s~  Doubt\n~y~ 3 ~s~  Lie";
        private const string MSG_PRESS_TO_TALK = "Press ~y~{0}~s~ to continue the interrogation.";

        private InterrogationLineData[] lines;

        private Camera camPlayerToPed;
        private Camera camPedToPlayer;
        private Camera gameCam;

        private ResponseType playerResponse;
        private int currentLineNo = 0;

        public Interrogation(InterrogationLineData[] dialog, Ped perp, float startDistance = 2f)
        {
            lines = dialog;
            Questions = lines.Length;
            Interrogee = perp;
            DistanceToStart = startDistance;
        }

        public void StartDialog()
        {
            lineFiber = new GameFiber(ShowLine);
            lineFiber.Start();
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

            Interrogator.Face(Interrogee);
            Interrogee.Face(Interrogator);

            GameFiber.Sleep(1000);

            camPlayerToPed = CreatePointingCam(Interrogator, Interrogee);

            camPedToPlayer = CreatePointingCam(Interrogee, Interrogator);

            Interrogator.IsPositionFrozen = true;
            Interrogee.IsPositionFrozen = true;

            gameCam = CreateGameCam();

            //TODO: set weapon to none
            NativeFunction.Natives.SET_PED_CAN_SWITCH_WEAPON(Interrogator, false);

            while (true)
            {
                GameFiber.Yield();

                if (currentLineNo > 0)
                {
                    Game.DisplayHelp(string.Format(MSG_PRESS_TO_TALK, KeyInteract));

                    while (!Game.IsKeyDown(KeyInteract))
                    {
                        GameFiber.Yield();
                    }

                    Game.HideHelp();
                }

                var current = lines[currentLineNo];

                Interrogee.IsVisible = false;

                PedPlayRndAnim(Interrogator);
                PlayFacialAnim(Interrogator);

                DisplayMultipleLinesForPed(Interrogator, current.Question, camPedToPlayer);

                Interrogee.IsVisible = true;
                Interrogator.IsVisible = false;

                PedPlayRndAnim(Interrogee);

                PlayFacialAnim(Interrogee);

                DisplayMultipleLinesForPed(Interrogee, current.Answer, camPlayerToPed);

                PlayInterrogeeFacialAnim(current.CorrectAnswer, Interrogee);

                Game.DisplayHelp(Help, true);

                playerResponse = WaitForPlayersResponse();

                Game.HideHelp();

                PlayAnswerSound(current.CorrectAnswer == playerResponse);

                if (playerResponse == current.CorrectAnswer)
                {
                    GoodAnswers++;
                }

                Interrogator.IsVisible = true;

                //TODO: add animations and set visibility
                if(playerResponse == ResponseType.Truth)
                {
                    DisplayMultipleLinesForPed(Interrogator, current.PlayerResponseTruth, camPedToPlayer);

                    DisplayMultipleLinesForPed(Interrogee, current.InterrogeeReactionTruth, camPlayerToPed);
                }
                else if(playerResponse == ResponseType.Doubt)
                {
                    DisplayMultipleLinesForPed(Interrogator, current.PlayerResponseDoubt, camPedToPlayer);

                    DisplayMultipleLinesForPed(Interrogee, current.InterrogeeReactionDoubt, camPlayerToPed);
                }
                else if(playerResponse == ResponseType.Lie)
                {
                    DisplayMultipleLinesForPed(Interrogator, current.PlayerResponseLie, camPedToPlayer);

                    DisplayMultipleLinesForPed(Interrogee, current.InterrogeeReactionLie, camPlayerToPed);
                }

                Interrogee.Tasks.Clear();

                currentLineNo++;

                if (currentLineNo == lines.Length)
                {
                    break;
                }
            }
        }

        private ResponseType WaitForPlayersResponse()
        {
            while (true)
            {
                GameFiber.Yield();

                ResponseType? answer = null;

                if (Game.IsKeyDown(KeyTruth)) answer = ResponseType.Truth;
                else if (Game.IsKeyDown(KeyDoubt)) answer = ResponseType.Doubt;
                else if (Game.IsKeyDown(KeyLie)) answer = ResponseType.Lie;

                if (!answer.HasValue) continue;
                return answer.Value;
            }
        }

        private void PlayAnswerSound(bool isCorrect)
        {
            if (isCorrect) soundCorrect.Play();
            else soundIncorrect.Play();
        }

        private void DisplayMultipleLinesForPed(Ped p, string[] line, Camera cam)
        {
            Array.ForEach(line, l => DisplaySingleLineForPed(p, l, cam));
        }

        private void DisplaySingleLineForPed(Ped p, string line, Camera cam)
        {
            cam.Active = true;

            var wordCount = CountWords(line);

            var time = GetDisplayTime(wordCount);

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

        private static void PlayInterrogeeFacialAnim(ResponseType t, Ped interrogee)
        {
            if (t == ResponseType.Truth)
                interrogee.Tasks.PlayAnimation("gestures@m@standing@casual", "gesture_pleased", 4f, AnimationFlags.Loop);
            else if (t == ResponseType.Doubt)
                interrogee.Tasks.PlayAnimation("facials@gen_male@variations@injured", "mood_injured_3", 4f, AnimationFlags.Loop);
            else if (t == ResponseType.Lie)
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
            var anim = MathHelper.Choose(Animation);
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

            if (camPedToPlayer.Exists())
            {
                camPedToPlayer.Active = false;
                camPedToPlayer.Delete();
            }
            if (camPlayerToPed.Exists())
            {
                gameCam.Active = true;
                camPlayerToPed.Active = true;

                CamInterpolate(camPlayerToPed, gameCam, 2000, true, true, true);

                camPlayerToPed.Active = false;
                camPlayerToPed.Delete();
            }

            HasEnded = true;
            lineFiber.Abort();
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

    public class Animations
    {
        public string Name { get; protected set; }
        public string ScenarioName { get; protected set; }
        public string FirstAnimation { get; protected set; }
        public string SecondAnimation { get; protected set; }
        public string EnterOne { get; protected set; }
        public string EnterTwo { get; protected set; }
        public string ExitOne { get; protected set; }
        public string ExitTwo { get; protected set; }
        public Model Model { get; protected set; }
        public float BlendInSpeed { get; protected set; }
        public AnimationFlags AnimationFlag { get; protected set; }

        public Animations() { }

        public Animations(string scenarioName)
        {
            ScenarioName = scenarioName;
        }

        public Animations(string scenarioName, string animationName, float blendSpeed = 3f, AnimationFlags flag = AnimationFlags.None)
        {
            ScenarioName = scenarioName;
            FirstAnimation = animationName;
            BlendInSpeed = blendSpeed;
            AnimationFlag = flag;
        }

        public Animations(string name, string anim1, string anim2, string enter1, string enter2, string exit1, string exit2)
        {
            Name = name;
            FirstAnimation = anim1;
            SecondAnimation = anim2;
            EnterOne = enter1;
            EnterTwo = enter2;
            ExitOne = exit1;
            ExitTwo = exit1;
        }

        public Animations(string name, string anim1, string anim2, string enter1, string enter2, string exit1, string exit2, Model model)
        {
            Name = name;
            FirstAnimation = anim1;
            SecondAnimation = anim2;
            EnterOne = enter1;
            EnterTwo = enter2;
            ExitOne = exit1;
            ExitTwo = exit1;
            Model = model;
        }
    }
}
