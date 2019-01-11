using LSNoir.Data;
using LtFlash.Common.EvidenceLibrary.Serialization;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using System.Collections.Generic;

namespace LSNoir.Stages.Base
{
    abstract class StageCalloutScript : CalloutScript
    {
        //TODO:
        // - add filePath validation
        //create a static entity factory

        protected static Ped Player => Game.LocalPlayer.Character;

        public static float DistToPlayer(ISpatial e) => Vector3.Distance(Player.Position, e.Position);
        public static float DistToPlayer(Vector3 e) => Vector3.Distance(Player.Position, e);
    }
}
