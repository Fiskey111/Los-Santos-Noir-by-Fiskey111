using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSNoir.Resources;
using LtFlash.Common;
using Rage;

namespace LSNoir.AA_NewMod.Entities
{
    internal class Witness
    {
        internal Ped Ped { get; private set; }
        internal string ID { get; }
        internal string Model { get; }
        internal string Name { get; }
        internal SpawnPoint Spawnpoint { get; }
        internal SpawnPoint PickupPosition { get; }
        internal Dialogue Dialogue { get; }
        internal bool IsCompliant { get; }
        internal string Scenario { get; }

        internal Witness(string model, string id, string name, SpawnPoint spawn, SpawnPoint pickupPos, Dialogue dialogue,
            bool isCompliant, string scenario)
        {
            ID = id;
            Model = model;
            Name = name;
            Spawnpoint = spawn;
            PickupPosition = pickupPos;
            Dialogue = dialogue;
            IsCompliant = isCompliant;
            Scenario = scenario;
        }

        internal bool Create()
        {
            Ped = new Ped(Model, Spawnpoint.Position, Spawnpoint.Heading);
            if (!Ped)
            {
                Game.DisplaySubtitle($"~r~Error spawning witness {ID}");
                Game.LogTrivial($"ERROR :: Witness.Create | ID:{ID}");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(Scenario))
            {
                PedScenarioLoop p = new PedScenarioLoop(Ped, Scenario);
                p.IsActive = true;
            }

            GameFiber dialogueFiber = new GameFiber(Process);
            dialogueFiber.Start();

            return true;
        }

        internal void End()
        {
            this.Ped.Delete();
            this.Dialogue.Abort();
        }

        private void Process()
        {
            while (Ped)
            {
                GameFiber.Yield();

                if (Game.LocalPlayer.Character.DistanceTo(Ped) > 1.5f) continue;

                if (!Dialogue.IsStarted) Dialogue.StartDialog();
            }
        }
    }
}
