using LSNoir.Data;
using LSPD_First_Response.Mod.API;
using LtFlash.Common.ScriptManager.Scripts;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSNoir.Scenes
{
    class SceneWithIdProps : SceneBase, IScene
    {
        //TODO:
        // - use data.Weapon

        class Prop<T>
        {
            public SceneItem data;
            public T entity;
            public Prop(SceneItem s, T e)
            {
                data = s;
                entity = e;
            }
        }

        public List<Vehicle> Vehicles => vehs.Select(v => v.entity).ToList();
        public List<Ped> Peds => peds.Select(p => p.entity).ToList();

        private List<Prop<Ped>> peds = new List<Prop<Ped>>();
        private List<Prop<Vehicle>> vehs = new List<Prop<Vehicle>>();
        private List<Rage.Object> objects = new List<Rage.Object>();

        private SceneData data;

        public SceneWithIdProps(SceneData sceneData)
        {
            data = sceneData;
        }

        public void Create()
        {
            for (int i = 0; i < data.Items.Length; i++)
            {
                var p = GenerateItem(data.Items[i]);

                if(p is Ped ped)
                {
                    ped.RelationshipGroup = RelationshipGroup.Cop;
                    peds.Add(new Prop<Ped>(data.Items[i], ped));
                }
                else if(p is Vehicle veh)
                {
                    vehs.Add(new Prop<Vehicle>(data.Items[i], veh));
                }
                else if(p is Rage.Object obj)
                {
                    objects.Add(obj);
                }
            }
        }

        public Ped GetPedById(string id)
        {
            return peds.SingleOrDefault(p => p.data.ID == id).entity;
        }

        public Vehicle GetVehById(string id)
        {
            return vehs.SingleOrDefault(p => p.data.ID == id).entity;
        }

        public SceneItem GetDataByEntity(Ped p)
        {
            return peds.FirstOrDefault(x => x.entity == p).data;
        }

        public void PedsEnterTheirVeh()
        {
            peds.ForEach(p => PedEnterHisVeh(p));
        }

        private void PedEnterHisVeh(Prop<Ped> p)
        {
            var veh = GetVehById(p.data.VehicleID);
            var seat = p.data.VehicleSeatID;
            p.entity.Tasks.EnterVehicle(veh, seat);
        }

        public void Dispose()
        {
            vehs.ForEach(v => { if (v.entity) v.entity.Delete(); });
            peds.ForEach(p => { if (p.entity) p.entity.Delete(); });
            objects.ForEach(o => { if (o) o.Delete(); });
        }
    }
}
