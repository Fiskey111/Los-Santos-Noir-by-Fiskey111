namespace LSNoir.Callouts.SA.Commons
{/*
    internal class WarrantRequestor
    {
        internal static void RequestWarrant(CaseData data)
        {
            GameFiber.StartNew(delegate
            {
                "Requesting warrant".AddLog();
                data.WarrantApprovedDate = TimeCheckObject.RandomTimeCreator().ToLocalTime();
                Evid_War_TimeChecker.AddObject(new TimeCheckObject(TimeCheckObject.Type.Warrant, "Warrant", data.WarrantApprovedDate));
                "Warrant submitted".AddLog();
                data.WarrantSubmitted = true;
                LtFlash.Common.Serialization.Serializer.SaveItemToXML(data, Main.CDataPath);
            });
        }
    }*/
}
