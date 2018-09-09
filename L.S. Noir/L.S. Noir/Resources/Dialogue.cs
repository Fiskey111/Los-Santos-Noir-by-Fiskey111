using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Rage;

namespace LSNoir.Resources
{
    [Serializable]
    public class DialogueLine
    {
        [XmlAttribute("PedID")]
        public int PedId { get; set; }

        public string Text { get; set; }

        public DialogueLine() {}

        public DialogueLine(int pedId, string txt)
        {
            PedId = pedId;
            Text = txt;
        }
    }

    public class Dialogue
    {
        public bool HasEnded { get; set; }
        public Vector3 Position { get; set; }
        public float DistanceToStop { get; set; }
        public int TimeToDisplay { get; set; } = 3500;
        public Keys InteractKey { get; set; } = Keys.Y;

        private DialogueLine[] _lines;
        private Dictionary<int, Ped> _pedDictionary = new Dictionary<int, Ped>();
        private GameFiber _lineFiber;
        private List<Animations> _animation = new List<Animations>
        {
            new Animations("gestures@m@car@van@casual@ps", "gesture_hand_left_three"),
            new Animations("gestures@m@car@van@casual@ps", "gesture_hand_right_three"),
            new Animations("gestures@m@car@van@casual@ps", "gesture_why"),
            new Animations("gestures@m@sitting@generic@casual", "gesture_bring_it_on"),
            new Animations("gestures@m@standing@casual", "gesture_easy_now")
        };

        public Dialogue(DialogueLine[] dialog, Vector3 pos, float stopDistance = 2f)
        {
            _lines = dialog;
            Position = pos;
            DistanceToStop = stopDistance;
        }

        public void AddPed(int id, Ped p)
        {
            _pedDictionary.Add(id, p);
        }

        public void StartDialog()
        {
            Game.LogTrivial("Total lines: " + _lines.Length);

            PedsFacePlayer();

            _lineFiber = new GameFiber(ShowLine);
            _lineFiber.Start();
        }

        private void PedsFacePlayer()
        {
            List<Ped> pedList = new List<Ped>(_pedDictionary.Values);
            foreach (Ped p in pedList)
            {
                Game.LogTrivial("Facing ped to Player");
                p.Face(Game.LocalPlayer.Character);
            }
        }

        private void ShowLine()
        {
            int currentLine = 0;
            int totalLines = _lines.Length;
            bool notified = false;

            while (true)
            {
                GameFiber.Yield();

                if (DistToPlayer(Position) > DistanceToStop)
                {
                    continue;
                }

                if (isKeyPressRequired(GetPedByLineNo(currentLine)))
                {
                    if (!notified)
                    {
                        notified = true;
                        Game.DisplayHelp("Press ~y~" + InteractKey + "~w~ to continue the dialog", true);
                    }

                    if (!Game.IsKeyDown(InteractKey))
                    {
                        continue;
                    }
                }

                Game.HideHelp();

                notified = false;

                Game.LogTrivial("Line: " + (currentLine + 1) + " out of " + totalLines);

                PedPlayRndAnim(GetPedByLineNo(currentLine));

                PlayFacialAnim(GetPedByLineNo(currentLine));

                Game.DisplaySubtitle(_lines[currentLine].Text, TimeToDisplay);

                GameFiber.Sleep(TimeToDisplay + 100);

                GetPedByLineNo(currentLine).Tasks.Clear();

                if (currentLine == totalLines - 1) //indexes start from 0
                {
                    break;
                }

                currentLine++;
            }

            End();
        }

        private bool isKeyPressRequired(Ped nextPed)
        {
            return nextPed == Game.LocalPlayer.Character;
        }

        private static void PlayFacialAnim(Ped p)
        {
            p.Tasks.PlayAnimation("mp_facial", "mic_chatter", 3f, AnimationFlags.SecondaryTask);
        }

        private void PedPlayRndAnim(Ped p)
        {
            int rnd = MathHelper.GetRandomInteger(_animation.Count);
            Animations anim = _animation[rnd];
            p.Tasks.PlayAnimation(anim.AnimationDictionary, anim.AnimationName, anim.BlendInSpeed, anim.AnimationFlag);
        }

        private float DistToPlayer(Vector3 pos)
        {
            return Vector3.Distance(Game.LocalPlayer.Character.Position, pos);
        }

        private Ped GetPedByLineNo(int line)
        {
            return _pedDictionary[_lines[line].PedId];
        }

        private void End()
        {
            Game.LogTrivial("Dialog Ended");
            HasEnded = true;
            _lineFiber.Abort();
        }
    }
}
