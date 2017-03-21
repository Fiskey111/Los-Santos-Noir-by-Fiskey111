using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Collections.Generic;

namespace LSNoir
{
    [Serializable]
    public class ReportData
    {
        public Service Type { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public List<string> Transcript { get; set; }
        public bool Helpful { get; set; }
        public bool Exists { get; set; }

        public ReportData() { }

        public ReportData(Service service, Ped ped, List<string> report)
        {
            Exists = true;
            Type = service;
            Name = Functions.GetPersonaForPed(ped).FullName;
            Time = DateTime.Now.AddMinutes(4);
            Transcript = report;
        }

        public ReportData(Service service, string name, List<string> report)
        {
            Exists = true;
            Type = service;
            Name = name;
            Time = DateTime.Now.AddMinutes(4);
            Transcript = report;
        }

        public ReportData(Service service, Ped ped, List<string> report, bool helpful)
        {
            Exists = true;
            Type = service;
            Name = Functions.GetPersonaForPed(ped).FullName;
            Time = DateTime.Now.AddMinutes(4);
            Transcript = report;
            Helpful = helpful;
        }

        public enum Service { FO, EMS, Coroner, ME, VicFamily, SusFamily, Lab, Judge }
    }
}
