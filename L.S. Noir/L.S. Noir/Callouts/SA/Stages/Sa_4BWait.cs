using System.Linq;
using System.Windows.Forms;
using LSNoir.Callouts.SA.Data;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;

namespace LSNoir.Callouts.SA.Stages
{
    public class Sa_4BWait : BasicScript
    {
        // Data
        private CaseData _cData;

        protected override bool Initialize()
        {
            "Initializing L.S. Noir Callout: Sexual Assault -- Stage 4b [Wait]".AddLog();

            _cData = LtFlash.Common.Serialization.Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);

            "Sexual Assault Case Update".DisplayNotification("Request a ~r~warrant~w~ using the SAJRS computer", _cData.Number);


            return true;
        }

        protected override void Process() => IsWarrantApproved();

        protected override void End()
        {

        }

        protected void SetScriptFinished()
        {
            _cData.CurrentStage = CaseData.LastStage.Wait;
            _cData.LastCompletedStage = CaseData.LastStage.Wait;
            _cData.CompletedStages.Add(CaseData.LastStage.Wait);
            _cData.StartingStage = this.Attributes.NextScripts.FirstOrDefault();
            LtFlash.Common.Serialization.Serializer.SaveItemToXML<CaseData>(_cData, Main.CDataPath);
            SetScriptFinished(true);
        }

        private void IsWarrantApproved()
        {
            _cData = LtFlash.Common.Serialization.Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);
            if (!_cData.WarrantAccess) return;
            if (!_cData.WarrantHeard) return;
            if (_cData.WarrantApproved)
            {
                "Warrant Approved".AddLog();
                "Sexual Assault Case Update".DisplayNotification("Warrant ~g~approved", _cData.Number);
                SetScriptFinished();
            }
            else
            {
                "Warrant Not Approved".AddLog();
                Attributes.NextScripts.Clear();
                GameFiber.StartNew(delegate
                {
                    var failedScreen = new MissionFailedScreen("Warrant denied");
                    failedScreen.Show();
                    while (!Game.IsKeyDown(Keys.Enter))
                    {
                        failedScreen.Draw();
                        GameFiber.Yield();
                    }
                });
                "Sexual Assault Case Update".DisplayNotification("Warrant ~r~denied~w~. \nCase has gone cold, better luck next time.", _cData.Number);
                SetScriptFinished();
            }
        }
    }
}
