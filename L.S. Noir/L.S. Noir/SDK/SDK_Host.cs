using LSNoir.Data;
using LSNoir.Settings;
using Rage;
using Rage.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LSNoir.SDK
{
    class SDK_Host
    {
        private List<Type> wnd = new List<Type>();
        private CaseData currentCase;
        private StageData currentStage;
        private bool enabled = true;
        private GwenForm currentWnd;
        internal List<CaseData> cases = new List<CaseData>();

        internal Main root;

        public SDK_Host(Main rt)
        {
            cases = Cases.CasesController.GetAllCasesFromFolder(Paths.PATH_FOLDER_CASES, Paths.FILENAME_CASEDATA);

            root = rt;

            GameFiber.StartNew(() =>
            {
                while (enabled)
                {
                    GameFiber.Yield();
                    Process();
                }
            });
        }

        private void Process()
        {
            if (currentWnd is null) Game.DisplaySubtitle("LS Noir SDK - Press F12 to display menu.", 100);

            if(Game.IsKeyDown(System.Windows.Forms.Keys.F12))
            {
                GameFiber.StartNew(() =>
                {
                    var main = new GwenForms.SDK_Main(this);
                    main.Show();
                    currentWnd = main;
                });
            }
        }

        public void DisplayChildForm(GwenForm caller, Type child)
        {
            var x = Activate(child);
            wnd.Add(caller.GetType());
            x.Show();
            //child.Show();
        }

        public void DisplayPreviousWnd()
        {
            if (wnd.Count == 0)
            {
                currentWnd = null;
                return;
            }
            var prev = Activator.CreateInstance(wnd.Last(), this);
            (prev as GwenForm).Show();
            currentWnd = prev as GwenForm;
            wnd.Remove(wnd.Last());

        }

        private GwenForm Activate(Type t)
        {
            return Activator.CreateInstance(t, this) as GwenForm;
        }

        public void SetCurrentCase(CaseData cdata)
        {
            currentCase = cdata;
        }

        public void SetCurrentStage(StageData stage)
        {
            currentStage = stage;
        }

        public void Stop()
        {
            enabled = false;
        }
    }
}
