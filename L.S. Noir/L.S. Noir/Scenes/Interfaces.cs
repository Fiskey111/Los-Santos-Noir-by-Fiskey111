using Rage;

namespace LSNoir.Scenes
{
    public interface IScene
    {
        void Create();
        void Dispose();
    }

    interface ISceneActive : IScene
    {
        void Start();
        bool HasFinished { get; }
    }

    interface ISceneActiveWithVehicle : ISceneActive
    {
        Vehicle Veh { get; }
        void EnterVehicle(Ped p);
    }
}
