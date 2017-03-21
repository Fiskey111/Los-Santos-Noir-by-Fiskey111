using Rage;
using System;

namespace LSNoir
{
    [Serializable]
    public class EvidenceData
    {
        public DataType Type { get; set; }
        public string Name { get; set; }
        public Model Model { get; set; }
        public bool IsImportant { get; set; }
        public HowImportant Importance { get; set; }
        public bool Collected { get; set; }
        public bool IsTested { get; set; }
        public bool MatchesSuspect { get; set; }
        public Traces Trace { get; set; }
        public DateTime TestingFinishTime { get; set; }
        
        public EvidenceData() { }

        public EvidenceData(DataType type, string name, Model model, bool isImportant)
        {
            Type = type;
            Name = name;
            Model = model;
            IsImportant = isImportant;
        }

        public enum HowImportant { NotImportant, Low, Medium, High }
        public enum Traces { Dna, Fingerprint, Semen, ClothingFiber, FacebookProfile, TwitterProfile, Note, TireTrack }
        public enum DataType { Electronic, Food, Drink }
    }
}
