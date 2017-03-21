using Rage;
using System.Media;
using System.Windows.Forms;

namespace LSNoir.Callouts.SA.Services
{
    public interface ICollectable
    {
        bool IsCollected { get; }
    }

    public interface IEvidence : ICollectable
    {
        string Id { get; }
        string Description { get; }
        bool Checked { get; }
        bool IsImportant { get; }
        float DistanceEvidenceClose { get; set; }
        Vector3 Position { get; }
        string TextInteractWithEvidence { get; }
        bool PlaySoundPlayerNearby { set; }
        SoundPlayer SoundPlayerNearby { get; set; }
        bool CanBeInspected { get; set; }
        void Interact();
        void Dismiss();


        Keys KeyInteract { get; set; }
        Keys KeyCollect { get; set; }
        Keys KeyLeave { get; set; }
    }
}
