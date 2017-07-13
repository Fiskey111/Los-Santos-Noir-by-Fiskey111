using Rage;

namespace LSNoir.Callouts.Universal
{
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
