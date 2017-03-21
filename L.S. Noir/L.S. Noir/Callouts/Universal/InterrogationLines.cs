using Rage;

namespace LSNoir.Callouts.SA.Data
{
    public class InterrogationLine
    {
        public Type LineType;
        public string PlayerLine;
        public string PerpLine;
        public Type CorrectType;
        public Ped P;

        public InterrogationLine() { }

        public InterrogationLine(Type lineType, string playerText, string perpText, Type correctType = Type.None)
        {
            LineType = lineType;
            PlayerLine = playerText;
            PerpLine = perpText;
            CorrectType = correctType;
        }

        public InterrogationLine(Type lineType, string playerText, string perpText, Ped p, Type correctType = Type.None)
        {
            LineType = lineType;
            PlayerLine = playerText;
            PerpLine = perpText;
            CorrectType = correctType;
        }

        public enum Type { Question, Truth, Doubt, Lie, None }
    }
}
