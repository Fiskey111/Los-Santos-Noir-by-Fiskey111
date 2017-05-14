using Rage;
using Rage.Native;

namespace LSNoir.Resources
{
    class PedAnimationLoop
    {
        public bool IsActive
        {
            get => run;
            set
            {
                if(value && !run)
                {
                    if (string.IsNullOrEmpty(dic) || string.IsNullOrEmpty(anim)) return;
                    if (!p) return;

                    run = true;
                    GameFiber.StartNew(Process);
                }
                else
                {
                    run = false;
                }
            }
        }

        private bool run;
        private readonly AnimationDictionary dic;
        private readonly AnimationSet anim;
        private readonly Ped p;

        public PedAnimationLoop(Ped ped, string animDic, string animName)
        {
            if (string.IsNullOrEmpty(animDic) || string.IsNullOrEmpty(animName)) return;
            if (!ped) return;

            p = ped;
            dic = new AnimationDictionary(animDic);
            anim = new AnimationSet(animName);
        }

        private void Process()
        {
            while(run)
            {
                if (!p)
                {
                    run = false;
                    break;
                }

                if (!NativeFunction.Natives.IsEntityPlayingAnim(p, dic.Name, anim.Name, 3))
                {
                    p.Tasks.PlayAnimation(dic, anim, 1, AnimationFlags.Loop);
                }
                GameFiber.Yield();
            }
        }
    }
}
