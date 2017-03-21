using Rage;
using System;
using System.Collections.Generic;

namespace LSNoir.Callouts.SA.Data
{
    [Serializable]
    class InterrogationData
    {
        public Type InterrogationType { get; set; }
        public List<Questions> QuestionsAsked { get; set; } = new List<Questions>();
        public int NumberCorrect { get; set; }
        public int NumberAsked { get; set; }
        public Ped Ped { get; set; }

        public InterrogationData() { }

        public InterrogationData(Questions question, Ped p)
        {
            QuestionsAsked.Add(question);
            Ped = p;
        }

        public void IncreaseCorrect(int value = 1)
        {
            NumberCorrect = NumberCorrect + value;
        }
        
        public void IncreaseAsked(int value = 1)
        {
            NumberAsked = NumberAsked + value;
        }

        public enum Type { VictimFamily, Suspect }
        public enum Questions { Vic1, Vic2, Vic3, Sus1, Sus2, Sus3 }
    }
}
