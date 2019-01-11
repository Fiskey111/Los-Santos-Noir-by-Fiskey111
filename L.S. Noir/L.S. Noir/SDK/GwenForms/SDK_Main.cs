using LSNoir.Data;
using Rage;
using Rage.Forms;
using System.Linq;

namespace LSNoir.SDK.GwenForms
{
    class SDK_Main : GwenForm
    {
        private Gwen.Control.Label lb_Case;
        private Gwen.Control.Label lb_Stage;
        private Gwen.Control.Label lb_NewCaseID;
        private Gwen.Control.Label lb_NewStageID;
        private Gwen.Control.Label lb_NewStageType;
        private Gwen.Control.Label lb_NewCase;
        private Gwen.Control.Label lb_NewStage;

        private Gwen.Control.TextBox tb_NewCaseID;
        private Gwen.Control.TextBox tb_NewStageID;

        private Gwen.Control.Button btn_CreateCase;
        private Gwen.Control.Button btn_CreateStage;
        private Gwen.Control.Button btn_EditCase;
        private Gwen.Control.Button btn_EditStage;
        private Gwen.Control.Button btn_StageUp;
        private Gwen.Control.Button btn_StageDown;

        private Gwen.Control.ComboBox cb_Case;
        //private Gwen.Control.ComboBox cb_Stage;
        private Gwen.Control.ComboBox cb_NewStageType;

        private Gwen.Control.ListBox lbx_Stages;

        private SDK_Host host;

        public SDK_Main(SDK_Host sHost) : base(typeof(WinForms.SDK_Main))
        {
            host = sHost;
        }
        public override void InitializeLayout()
        {
            GameFiber.StartNew(() => 
            {
                while (true)
                {
                    if (!Window.IsVisible)
                    {
                        host.DisplayPreviousWnd();
                        break;
                    }
                    GameFiber.Yield();
                }
            });

            Position =
                new System.Drawing.Point(
                    1,
                    Game.Resolution.Height / 2 -
                    this.Size.Height / 2);

            var cases = host.cases;
            cases.ForEach(c => cb_Case.AddItem(c.ID, UserData: c));

            FillStages();

            cb_Case.ItemSelected += Cb_Case_ItemSelected;
            lbx_Stages.RowSelected += Lbx_Stages_RowSelected;
            //select first, load stages for selected
            btn_CreateCase.Clicked += Btn_CreateCase_Clicked;
            btn_EditCase.Clicked += Btn_EditCase_Clicked;
            
            base.InitializeLayout();
        }

        private void Btn_EditCase_Clicked(Gwen.Control.Base sender, Gwen.Control.ClickedEventArgs arguments)
        {
                //var w = new Case_Main(host);
            Window.Close();
            //Window.Hide();
            host.DisplayChildForm(this, typeof(Case_Main));
        }

        private void Btn_CreateCase_Clicked(Gwen.Control.Base sender, Gwen.Control.ClickedEventArgs arguments)
        {
            if(string.IsNullOrEmpty(tb_NewCaseID.Text))
            {
                Gwen.Control.MessageBox mb = new Gwen.Control.MessageBox(this, "Insert a unique ID!");
                mb.Show();
            }
        }

        private void Lbx_Stages_RowSelected(Gwen.Control.Base sender, Gwen.Control.ItemSelectedEventArgs arguments)
        {
            if(sender == lbx_Stages)
            {
                //host.SetCurrentStage()
            }
        }

        private void FillStages()
        {
            lbx_Stages.DeleteAll();
            var selCase = cb_Case.SelectedItem.Text;
            host.cases.FirstOrDefault(c => c.ID == selCase).Stages.ToList().ForEach(s => lbx_Stages.AddRow(s));

        }

        private void Cb_Case_ItemSelected(Gwen.Control.Base sender, Gwen.Control.ItemSelectedEventArgs arguments)
        {
            if(sender == cb_Case)
            {
                FillStages();
                host.SetCurrentCase(cb_Case.SelectedItem.UserData as CaseData);
            }
        }
    }
}
