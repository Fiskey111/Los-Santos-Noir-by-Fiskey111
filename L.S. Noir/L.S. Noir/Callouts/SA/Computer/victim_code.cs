using Rage;
using Rage.Forms;
using System;
using System.Drawing;
using System.Linq;
using Gwen.Control;
using LSNoir.Callouts.SA;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using static LtFlash.Common.Serialization.Serializer;

namespace LSNoir
{
    public class VictimCode : GwenForm
    {
        // System
        private string _dob, _name, _gender;

        // Gwen
        private Button _returnBut, _familyBut, _relativemeetBut, _socialBut;
        private Label _relativeLbl, _relativenamLbl, _relativerelLbl, _relativeaddLbl, _nameLbl, _genderLbl, _dobLbl, _injuryLbl, _tracesLbl,
            _relativenamVal, _relativerelVal, _relativeaddVal, _nameVal, _genderVal, _dobVal, _injuryVal, _tracesVal;
        
        // Data
        private PedData _vData, _vfData, _sData;

        public VictimCode()
            : base(typeof(VictimForm))
        {

        }

        public override void InitializeLayout()
        {
            base.InitializeLayout();
            Window.IsClosable = false;
            Position = new Point(Game.Resolution.Width / 2 - Window.Width / 2, Game.Resolution.Height / 2 - Window.Height / 2);
            "Initializing Victim Info Viewer".AddLog();
            
            _vData = GetSelectedListElementFromXml<PedData>(Main.PDataPath,
                c => c.FirstOrDefault(p => p.Type == PedType.Victim));
            _vfData = GetSelectedListElementFromXml<PedData>(Main.PDataPath,
                c => c.FirstOrDefault(p => p.Type == PedType.VictimFamily));
            _sData = GetSelectedListElementFromXml<PedData>(Main.SDataPath,
                c => c.FirstOrDefault(p => p.Type == PedType.Suspect));

            this.Window.Show();

            StartMethods();

            FillData();

            HideStuff();     
        }

        private void StartMethods()
        {
            _returnBut.Clicked += Return_Button_Clicked;
            _familyBut.Clicked += FamilyBut_Clicked;
            _relativemeetBut.Clicked += RelativemeetBut_Clicked;
            _socialBut.Clicked += SocialBut_Clicked;
        }

        private void FillData()
        {
            _nameVal.Text = _vData.Name;
            _genderVal.Text = _vData.Gender.ToString();
            _dobVal.Text = _vData.Dob.ToShortDateString();
            _injuryVal.Text = String.Format("Bruise at: {0}" + Environment.NewLine + "Cut at: {1}\nMark at: {2}", _vData.BruiseLocation, _vData.CutLocation, _vData.MarkLocation).ToString();
            _tracesVal.Text = _vData.Traces.ToString();

            _relativenamVal.Text = _vfData.Name;
            _relativerelVal.Text = _vfData.Relationship;
            _relativeaddVal.Text = "1000 LSPDFR Lane";
        }

        private void HideStuff()
        {
            _relativeLbl.Hide();
            _relativeaddLbl.Hide();
            _relativeaddVal.Hide();
            _relativemeetBut.Hide();
            _relativenamLbl.Hide();
            _relativenamVal.Hide();
            _relativerelLbl.Hide();
            _relativerelVal.Hide();
            _relativemeetBut.Hide();
        }

        private void ShowStuff()
        {
            _relativeLbl.Show();
            _relativeaddLbl.Show();
            _relativeaddVal.Show();
            _relativemeetBut.Show();
            _relativenamLbl.Show();
            _relativenamVal.Show();
            _relativerelLbl.Show();
            _relativerelVal.Show();
            _relativemeetBut.Show();
        }

        private void Return_Button_Clicked(Base sender, ClickedEventArgs arguments)
        {
            "Returning to main form".AddLog();
            Window.Close();
            Computer.Controller.SwitchFibers(Computer.Controller.VictimFiber, ComputerController.Fibers.MainFiber);
        }

        private void FamilyBut_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ShowStuff();
        }

        private void RelativemeetBut_Clicked(Base sender, ClickedEventArgs arguments)
        {
            "Displaying MessageBox".AddLog();
            MessageBoxCode.Message = String.Format("An email has been sent to {0}." + Environment.NewLine + "You will be updated when they are ready to meet.", _vfData.Name).ToString();
            Window.Close();
            Computer.Controller.SwitchFibers(Computer.Controller.VictimFiber, ComputerController.Fibers.MessageBoxFiber);
        }

        private void SocialBut_Clicked(Base sender, ClickedEventArgs arguments)
        {
            GameFiber.StartNew(delegate
            {
                SocialMediaDrawer.DrawSocialMediaPage(_vData, _sData, Window);
                Window.Close();
                GameFiber.Sleep(10100);
                Computer.Controller.SwitchFibers(Computer.Controller.VictimFiber, ComputerController.Fibers.MainFiber);
            });
        }
    }
}
