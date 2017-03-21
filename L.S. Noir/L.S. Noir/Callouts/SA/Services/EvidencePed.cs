using LtFlash.Common.EvidenceLibrary.BaseClasses;
using Rage;

namespace LSNoir.Callouts.SA.Services
{
    public abstract class EvidencePed : EvidenceBase
    {
        public Ped Ped { get; protected set; }
        public override Vector3 Position => Ped ? Ped.Position : Vector3.Zero;
        public override PoolHandle Handle => Ped ? Ped.Handle : new PoolHandle();
        protected override Entity EvidenceEntity => Ped;

        public EvidencePed(
            string id, string description,
            SpawnPoint spawn, Model model) : base(id, description)
        {
            Ped = new Ped(model, spawn.Position, spawn.Heading);
            Ped.RandomizeVariation();
            Ped.BlockPermanentEvents = true;
            Ped.KeepTasks = true;
        }

        protected override void End()
        {
            if (Ped)
            {
                Ped.Tasks.Wander();
                Ped.Dismiss();
            }
        }

        public override void Dismiss()
        {
            End();
            //if (Ped) Ped.Delete();
            base.Dismiss();
        }

        public override bool IsValid() => Ped;
    }
}
