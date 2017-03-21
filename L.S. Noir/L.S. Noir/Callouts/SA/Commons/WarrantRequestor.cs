using LSNoir.Extensions;
using Rage;

namespace LSNoir.Callouts.SA.Commons
{
    internal class WarrantRequestor
    {
        internal static void RequestWarrant(CaseData data)
        {
            GameFiber.StartNew(delegate
            {
                "Requesting warrant".AddLog();
                GameFiber.Sleep(10000);
                "Warrant submitted".AddLog();
                data.WarrantSubmitted = true;
                GameFiber.Sleep(10000);
                "Warrant Heard".AddLog();
                GameFiber.Sleep(5000);
                "Warrant decision made".AddLog();
                data.WarrantHeard = true;
                if (data.WarrantReason == "Gut Feeling" || data.WarrantReason == "None") data.WarrantApproved = MathHelper.GetRandomInteger(10) != 1;
                else data.WarrantApproved = MathHelper.GetRandomInteger(250) != 1;
                LtFlash.Common.Serialization.Serializer.SaveItemToXML<CaseData>(data, Main.CDataPath);
            });
        }
    }
}
