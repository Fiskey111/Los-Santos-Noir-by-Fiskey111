using LtFlash.Common.EvidenceLibrary;
using LtFlash.Common.EvidenceLibrary.Serialization;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSNoir.Data
{
    public class InterrogationData : IIdentifiable
    {
        public string ID { get; set; }
        public InterrogationLineData[] Lines;

        public InterrogationData()
        {
        }
    }

    public enum ResponseType
    {
        Truth,
        Doubt,
        Lie,
    }

    public class InterrogationLineData
    {
        public ResponseType CorrectAnswer;
        public string[] Question;
        public string[] Answer;

        public string[] PlayerResponseTruth;
        public string[] InterrogeeReactionTruth;
        public string[] PlayerResponseDoubt;
        public string[] InterrogeeReactionDoubt;
        public string[] PlayerResponseLie;
        public string[] InterrogeeReactionLie;

        public InterrogationLineData()
        {
        }
    }
}
