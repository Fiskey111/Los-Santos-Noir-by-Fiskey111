using System.Collections.Generic;
using System.Windows.Forms;
using LSNoir.Extensions;
using Rage;
using Rage.Native;

namespace LSNoir.Callouts.Universal
{
    public class DialogLine
    {
        public int PedId;
        public string Text;

        public DialogLine() { }

        public DialogLine(int pedId, string txt)
        {
            PedId = pedId;
            Text = txt;
        }
    }

    public class Dialog
    {
        public DialogLine[] Lines;
        private Dictionary<int, Ped> PedDictionary { get; set; } = new Dictionary<int, Ped>();

        public bool HasEnded { get; set; }
        public bool IsRunning { get; set; }
        public Vector3 Position { get; set; }
        public float DistanceToStop { get; set; }
        public int TimeToDisplay { get; set; } = 3500;
        public List<string> Dialogue { get; set; } = new List<string>();
        private GameFiber _lineFiber;
        private List<Animations> _animation;
        public Keys InteractKey { get; set; } = Keys.Y;
        public bool DisableFirstKeypress { get; set; } = true;

        private void AnimationAdd()
        {
            _animation = new List<Animations>
            {
                new Animations("gestures@m@car@van@casual@ps", "gesture_hand_left_three"),
                new Animations("gestures@m@car@van@casual@ps", "gesture_hand_right_three"),
                new Animations("gestures@m@car@van@casual@ps", "gesture_why"),
                new Animations("gestures@m@sitting@generic@casual", "gesture_bring_it_on"),
                new Animations("gestures@m@standing@casual", "gesture_easy_now")
            };
        }

        public Dialog(DialogLine[] dialog, Vector3 pos, float stopDistance = 2f)
        {
            Lines = dialog;
            Position = pos;
            DistanceToStop = stopDistance;

            foreach (var d in dialog)
                Dialogue.Add(d.Text);
        }

        public void AddPed(int id, Ped p) => PedDictionary.Add(id, p);

        public void StartDialog()
        {
            IsRunning = true;

            AnimationAdd();

            ("Total lines: " + Lines.Length).AddLog();

            PedsFacePlayer();

            _lineFiber = new GameFiber(ShowLine);
            _lineFiber.Start();
        }

        private void PedsFacePlayer()
        {
            var pedList = new List<Ped>(PedDictionary.Values);
            foreach (var p in pedList)
            {
                if (p == Game.LocalPlayer.Character) continue;

                "Facing ped to Player".AddLog();
                NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(p, Game.LocalPlayer.Character, -1);
            }
        }

        private void ShowLine()
        {
            var currentLine = 0;
            var totalLines = Lines.Length;
            var notified = false;

            while (true)
            {
                GameFiber.Yield();

                if (DistToPlayer(Position) > DistanceToStop)
                    continue;

                if (IsKeyPressRequired(GetPedByLineNo(currentLine), currentLine))
                {
                    if (!notified && currentLine > 0)
                    {
                        notified = true;
                        Game.DisplayHelp("Press ~y~" + InteractKey + "~w~ to continue the dialog", true);
                    }
                    else if (!notified && currentLine == 0)
                    {
                        notified = true;
                        Game.DisplayHelp("Press ~y~" + InteractKey + "~w~ to start the dialog", true);
                    }

                    if (!Game.IsKeyDown(InteractKey)) continue;
                    Game.HideHelp();
                }

                Game.HideHelp();

                notified = false;

                ("Line: " + (currentLine + 1) + " out of " + totalLines).AddLog();

                PedPlayRndAnim(GetPedByLineNo(currentLine));

                PlayFacialAnim(GetPedByLineNo(currentLine));

                var line = Lines[currentLine].Text;

                int wordCount = 0, index = 0;

                while (index < line.Length)
                {
                    // check if current char is part of a word
                    while (index < line.Length && !char.IsWhiteSpace(line[index]))
                        index++;

                    wordCount++;

                    // skip whitespace until next word
                    while (index < line.Length && char.IsWhiteSpace(line[index]))
                        index++;
                    GameFiber.Yield();
                }

                int displayTime = ((wordCount / 2) * 1000);
                if (displayTime < 3500)
                    displayTime = 4000;

                Game.DisplaySubtitle(Lines[currentLine].Text, displayTime);

                GameFiber.Sleep(displayTime + 100);

                GetPedByLineNo(currentLine).Tasks.Clear();

                if (currentLine == totalLines - 1) //indexes start from 0
                {
                    break;
                }

                currentLine++;
            }

            End();
        }

        private bool IsKeyPressRequired(Ped nextPed, int currentLine)
        {
            if (currentLine == 0 && DisableFirstKeypress)
            {
                return false;
            }
            else
            {
                return nextPed == Game.LocalPlayer.Character;
            }
        }

        private static void PlayFacialAnim(Ped p)
        {
            p.Tasks.PlayAnimation("mp_facial", "mic_chatter", 3f, AnimationFlags.SecondaryTask);
        }

        private void PedPlayRndAnim(Ped p)
        {
            int rnd = MathHelper.GetRandomInteger(_animation.Count - 1);
            Animations anim = _animation[rnd];
            p.Tasks.PlayAnimation(anim.ScenarioName, anim.FirstAnimation, anim.BlendInSpeed, anim.AnimationFlag);
        }

        private float DistToPlayer(Vector3 pos)
        {
            return Vector3.Distance(Game.LocalPlayer.Character.Position, pos);
        }

        private Ped GetPedByLineNo(int line)
        {
            return PedDictionary[Lines[line].PedId];
        }

        private void End()
        {
            "Dialog Ended".AddLog();
            HasEnded = true;
            IsRunning = false;
            _lineFiber.Abort();
        }
    }
}
