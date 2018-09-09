using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Data.NewData
{
    public class InterrogationData : IData
    {
        // IData
        public string ID { get; set; }
        public string Name { get; set; }

        // String
        public List<InterrogationLine> InterrogationLines { get; set; }
    }

    public class InterrogationLine
    {
        public enum ResponseType { Truth, Doubt, Lie }

        public ResponseType CorrectAnswer { get; set; }
        public string[] Question { get; set; }
        public string[] Answer { get; set; }

        public string[] PlayerResponseTruth { get; set; }
        public string[] InterrogeeReactionTruth { get; set; }
        public string[] PlayerResponseDoubt { get; set; }
        public string[] InterrogeeReactionDoubt { get; set; }
        public string[] PlayerResponseLie { get; set; }
        public string[] InterrogeeReactionLie { get; set; }
    }
}
