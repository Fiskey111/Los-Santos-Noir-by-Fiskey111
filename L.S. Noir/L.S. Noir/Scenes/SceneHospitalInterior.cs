using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSNoir.Data;
using Rage.Native;
using Rage;

namespace LSNoir.Scenes
{
    class SceneHospitalInterior : Scene
    {
        private int handleInterior;

        public SceneHospitalInterior(SceneData data) : base(data)
        {
        }

        public override void Create()
        {
            base.Create();

            handleInterior = LoadHospialInterior();

            entities.AddRange(SpawnHospitalFloor());
        }

        public override void Dispose()
        {
            base.Dispose();

            NativeFunction.Natives.SET_INTERIOR_ACTIVE(handleInterior, false);
        }

        private static int LoadHospialInterior()
        {
            NativeFunction.Natives.x0888C3502DBBEEF5();
            NativeFunction.Natives.x9BAE5AD2508DF078(1);
            NativeFunction.Natives.SET_STREAMING(true);
            NativeFunction.Natives.REQUEST_IPL("RC12B_Default");
            NativeFunction.Natives.REQUEST_IPL("RC12B_Destroyed");
            NativeFunction.Natives.REQUEST_IPL("RC12B_Fixed");
            NativeFunction.Natives.REMOVE_IPL("RC12B_HospitalInterior");
            NativeFunction.CallByName<bool>("IS_IPL_ACTIVE", "RC12B_Fixed");
            NativeFunction.CallByName<bool>("IS_IPL_ACTIVE", "RC12B_HospitalInterior");
            NativeFunction.CallByName<bool>("IS_IPL_ACTIVE", "RC12B_Default");
            NativeFunction.CallByName<bool>("IS_IPL_ACTIVE", "RC12B_Destroyed");
            NativeFunction.Natives.REQUEST_COLLISION_AT_COORD(311.4596f, -588.8196f, 41.3174f);
            var hosp = NativeFunction.CallByName<int>("GET_INTERIOR_AT_COORDS", 311.4596f, -588.8196f, 44.3174f);
            NativeFunction.Natives.x2CA429C029CCF247(hosp);
            NativeFunction.Natives.SET_INTERIOR_ACTIVE(hosp, true);
            NativeFunction.Natives.LOAD_SCENE(330.4596f, -584.8196f, 42.3174f, true);

            return hosp;
        }

        private static List<Rage.Object> SpawnHospitalFloor()
        {
            var result = new List<Rage.Object>();

            var model = new Model("prop_container_01a");
            var q = -583f;

            for (var i = 307f; i > 287f; i = i - 1f)
            {
                var obj = new Rage.Object(model, new Vector3(i, q, 39.441f));
                obj.Heading = 252f;
                q = q - 2.5f;

                result.Add(obj);
            }

            var x = 319f;
            for (var w = -586f; w > -610f; w = w - 2.5f)
            {
                var obj = new Rage.Object(model, new Vector3(x, w, 39.441f));
                obj.Heading = 252f;
                x = x - 1f;

                result.Add(obj);
            }

            var x2 = 326f;
            for (var w = -587f; w > -610f; w = w - 2.5f)
            {
                var obj = new Rage.Object(model, new Vector3(x2, w, 39.441f));
                obj.Heading = 252f;
                x2 = x2 - 1f;

                result.Add(obj);
            }
            return result;
        }
    }
}
