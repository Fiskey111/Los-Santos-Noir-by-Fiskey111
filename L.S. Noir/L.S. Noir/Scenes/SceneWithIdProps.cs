using LSNoir.Data;
using Rage;
using System.Collections.Generic;
using System.Linq;

namespace LSNoir.Scenes
{
    class SceneWithIdProps : SceneBase, IScene
    {
        //TODO:

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
                var prop = GenerateItem(data.Items[i]);

                if(prop is Ped ped)
                {
                    ped.RelationshipGroup = RelationshipGroup.Cop;
                    peds.Add(new Prop<Ped>(data.Items[i], ped));
                }
                else if(prop is Vehicle veh)
                {
                    vehs.Add(new Prop<Vehicle>(data.Items[i], veh));
                }
                else if(prop is Rage.Object obj)
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
