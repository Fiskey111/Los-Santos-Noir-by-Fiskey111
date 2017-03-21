using System.Collections.Generic;
using Fiskey111Common;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;

namespace LSNoir.Callouts.SA.Commons
{
    public class Cop
    {
        public Ped Ped { get; set; }
        public Vector3 Position
        {
            get { return Ped.Position; }
            set { Ped.Position = value; }
        }
        public SpawnPoint TargetPosition { get; protected set; }
        public Vehicle Veh { get; set; }
        public bool IsSecondCop { get; protected set; }

        //Primary
        public Cop(string vehmodel, SpawnPoint pos, SpawnPoint targetPos, bool isSwat = false, ICollection<Cop> copList = null)
        {
            string copModel = isSwat ? "s_m_y_swat_01" : "s_m_y_cop_01";
            Veh = new Vehicle(vehmodel, pos.Spawn, pos.Heading);
            Ped = new Ped(copModel, Veh.LeftPosition, Veh.Heading);
            Ped.RelationshipGroup = RelationshipGroup.Cop;
            if (isSwat)
            {
                var w = new Weapon(new WeaponAsset("WEAPON_CARBINERIFLE"), Ped.Position, 120);
                w.GiveTo(Ped);
                NativeFunction.Natives.SetPedPropIndex(Ped, 0, 0, 0, true);
            }
            Functions.SetPedAsCop(Ped);
            Functions.SetCopAsBusy(Ped, true);
            TargetPosition = targetPos;
            Ped.MakeMissionPed();
            Veh.MakeMissionVehicle();
            copList?.Add(this);
        }
        //Secondary
        public Cop(Vehicle veh, bool isSwat = false, ICollection<Cop> copList = null)
        {
            Veh = veh;
            string copModel = isSwat ? "s_m_y_swat_01" : "s_m_y_cop_01";
            Ped = new Ped(copModel, veh.RightPosition, veh.Heading);
            Ped.RelationshipGroup = RelationshipGroup.Cop;
            if (isSwat)
            {
                var w = new Weapon(new WeaponAsset("WEAPON_CARBINERIFLE"), Ped.Position, 120);
                w.GiveTo(Ped);
                NativeFunction.Natives.SetPedPropIndex(Ped, 0, 0, 0, true);
            }
            Functions.SetPedAsCop(Ped);
            Functions.SetCopAsBusy(Ped, true);
            IsSecondCop = true;
            Ped.MakeMissionPed();
            copList?.Add(this);
        }

        public void EnterVehicle()
        {
            if (!Ped.Exists() || !Veh.Exists()) return;
            if (!IsSecondCop)
                Ped.Tasks.EnterVehicle(Veh, -1);
            else
                Ped.Tasks.EnterVehicle(Veh, 0);
        }

        public void ExitVehicle()
        {
            if (!Ped.Exists()) return;
            Ped.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
        }

        public void DriveToTargetPosition()
        {
            if (IsSecondCop) return;
            Ped.Tasks.DriveToPosition(TargetPosition.Spawn, 16f,
                VehicleDrivingFlags.YieldToCrossingPedestrians | VehicleDrivingFlags.AllowWrongWay |
                VehicleDrivingFlags.DriveAroundObjects | VehicleDrivingFlags.DriveAroundVehicles |
                VehicleDrivingFlags.FollowTraffic, 2f);
        }

        public void TurnOnSiren()
        {
            if (!Veh.Exists() || IsSecondCop) return;
            Veh.IsSirenOn = true;
            Veh.IsSirenSilent = true;
        }

        public void DismissPed()
        {
            if (!Ped.Exists()) return;
            Ped.Dismiss();
        }

        public void DismissVeh()
        {
            if (!Veh.Exists()) return;
            Veh.Dismiss();
        }
    }
}
