using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using Fiskey111Common;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSNoir.Callouts.SA.Commons;
using LSNoir.Extensions;
using LtFlash.Common.ScriptManager.Scripts;

namespace LSNoir
{
    [Serializable]
    public class PedData
    {
        public PedType Type { get; set; }
        public string Name { get; set; }
        public bool IsImportant { get; set; }
        public bool IsCompliant { get; set; }
        public bool Exists { get; set; }
        public DateTime Dob { get; set; }
        public string BruiseLocation { get; set; }
        public string MarkLocation { get; set; }
        public string CutLocation { get; set; }
        public Traces Traces { get; set; }
        public bool Checked { get; set; }
        public ExtensionMethods.Gender Gender { get; set; }
        public bool IsPerp { get; set; }
        public string Model { get; set; }
        public int WhatSeenInt { get; set; }
        public string WhatSeenString { get; set; }
        public List<string> Conversation { get; set; }
        public string Relationship { get; set; }
        public bool SocialMedia { get; set; }
        public string VehModel { get; set; }

        public PedData() { }

        public PedData(Ped ped, PedType type, bool isImportant = false, bool isPerp = false, string bruiseLoc = null, 
            string markLoc = null, string cutLoc = null, Traces traces = Traces.Blood, bool socialMedia = false)
        {
            Type = type;
            Exists = true;
            Persona p = Functions.GetPersonaForPed(ped);
            Name = p.FullName;
            Dob = p.BirthDay;
            Gender = ExtensionMethods.GetGender(ped);

            if (type == PedType.VictimFamily)
            {
                if (Gender == ExtensionMethods.Gender.Male)
                    Relationship = "Father";
                else
                    Relationship = "Mother";
                Model = ped.Model.Name;
            }
            else if (type == PedType.Suspect)
            {
                IsPerp = isPerp;
                Model = ped.Model.Name;
                var models = Rage.Model.VehicleModels.Where(c => c.IsCar && !c.IsBigVehicle && !c.IsBike && !c.IsEmergencyVehicle).ToList();
                VehModel = models[Rand.RandomNumber(models.Count)].Name;
            }
            else if (type == PedType.Victim)
            {
                IsImportant = isImportant;
                BruiseLocation = bruiseLoc;
                MarkLocation = markLoc;
                CutLocation = cutLoc;
                Traces = traces;
                SocialMedia = socialMedia;
            }
            else if (type == PedType.Witness1 || type == PedType.Witness2)
            {
                IsImportant = isImportant;
            }
        }
    }

    public enum PedType { Suspect, Victim, VictimFamily, Witness1, Witness2 }

    public enum Traces { Blood, Semen, Urine, Saliva, Wearer, Hair, Fingerprint, Weapon }  
}
