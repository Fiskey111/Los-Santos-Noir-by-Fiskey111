using LtFlash.Common.ScriptManager.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSNoir.Stages
{
    class WaitForWarrant : BasicScript
    {
        //NOTES:
        // - check if WarrantRequestData.TimeDecision >= Now and 
        //   finish the case or continue with an arrest stage
        // - can be also used to check if given evidence was analyzed by a lab

        protected override bool Initialize()
        {
            return true;
        }

        protected override void Process()
        {
        }

        protected override void End()
        {
        }
    }
}
