namespace LSNoir.Callouts.SA.Stages
{/*
    public class Sa_3b_Wait : BasicScript
    {
        // Data
        private CaseData _cData;

        protected override bool Initialize()
        {
            "Initializing L.S. Noir Callout: Sexual Assault -- Stage 3b [Wait]".AddLog();

            _cData = Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);

            "Sexual Assault Case Update".DisplayNotification("Get information on the ~r~suspect", _cData.Number);

            return true;
        }

        protected override void Process() => IsSuspect();

        protected override void End() { }

        protected void SetScriptFinished()
        {
            _cData.CurrentStage = CaseData.LastStage.Wait;
            _cData.LastCompletedStage = CaseData.LastStage.Wait;
            _cData.CompletedStages.Add(CaseData.LastStage.Wait);
            _cData.StartingStage = this.Attributes.NextScripts.FirstOrDefault();
            Serializer.SaveItemToXML<CaseData>(_cData, Main.CDataPath);
            SetScriptFinished(true);
        }

        private void IsSuspect()
        {
            _cData = Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);
            if (string.IsNullOrWhiteSpace(_cData.CurrentSuspect)) return;

            var sus = Serializer.GetSelectedListElementFromXml<PedData>(Main.SDataPath,
                s => s.First());
            
            if (!sus.Exists || !sus.IsPerp || _cData.CurrentSuspect != sus.Name.ToLower()) return;

            $"Suspect matches: {sus.IsPerp}".AddLog();
            "Sexual Assault Case Update".DisplayNotification("Suspect ~g~confirmed~w~\nGPS coordinates downloading...", _cData.Number);

            SetScriptFinished();
        }
    }*/
}
