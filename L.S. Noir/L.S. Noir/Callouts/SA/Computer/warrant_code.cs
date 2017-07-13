using System.Drawing;
using System.Linq;
using Gwen.Control;
using LSNoir.Callouts.SA.Data;
using LSNoir.Callouts.Universal;
using LSNoir.Extensions;
using LtFlash.Common.Serialization;
using Rage;
using Rage.Forms;

namespace LSNoir.Callouts.SA.Computer
{
    public class WarrantCode : GwenForm
    {
        // System
        private string _dob, _name, _gender;


        // Gwen
        private ComboBox wit_select_combobox, reason_box;
        private Button witness_return_but, _reqBut;
        private Label wit_name_lbl, wit_gender_lbl, wit_takenby_lbl;
        private Label wit_select_lbl, wit_name_value, wit_gender_value, wit_taken_value, wit_statement_lbl;
        

        // Data
        private static PedData _sData;
        private CaseData _cData;

        public WarrantCode()
            : base(typeof(WarrantForm))
        {

        }

        public override void InitializeLayout()
        {
            base.InitializeLayout();
            Window.IsClosable = false;
            Position = new Point(Game.Resolution.Width / 2 - Window.Width / 2, Game.Resolution.Height / 2 - Window.Height / 2);
            "Initializing San Andreas Joint Records System Warrant Statement Viewer".AddLog();

            _cData = Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);
            _sData = Serializer.GetSelectedListElementFromXml<PedData>(Main.SDataPath,
                c => Enumerable.FirstOrDefault<PedData>(c, s => s.Type == PedType.Suspect));

            StartMethods();

            FillData();

            HideStuff();     
        }

        private void StartMethods()
        {
            wit_select_combobox.ItemSelected += Wit_select_combobox_ItemSelected;
            _reqBut.Pressed += _reqBut_Pressed;
            witness_return_but.Clicked += Witness_return_but_Clicked;

        }

        private void _reqBut_Pressed(Base sender, System.EventArgs arguments)
        {
            switch (reason_box.SelectedItem.Text)
            {
                case "Evidence":
                case "Testimony -- Victim Family":
                case "Testimony -- Suspect":
                case "Security Footage":
                    _cData.WarrantReason = reason_box.SelectedItem.Text;
                    "Loading Warrant Request Form".AddLog();
                    Window.Close();
                    Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.WarrantFiber, ComputerController.Fibers.WarrantRequestFiber);
                    break;
                default:
                    _cData.WarrantReason = reason_box.SelectedItem.Text;
                    "Loading Warrant Request Form".AddLog();
                    Window.Close();
                    Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.WarrantFiber, ComputerController.Fibers.WarrantRequestFiber);
                    break;
            }

            Serializer.SaveItemToXML<CaseData>(_cData, Main.CDataPath);
        }

        private void FillData()
        {
            _name = _sData.Name;
            _gender = _sData.Gender.ToString();
            _dob = _sData.Dob.ToShortDateString();

            wit_select_combobox.AddItem("");
            wit_select_combobox.AddItem(_name);
        }

        private void HideStuff()
        {
            wit_name_value.Hide();
            wit_gender_value.Hide();
            wit_taken_value.Hide();
            wit_name_lbl.Hide();
            wit_gender_lbl.Hide();
            wit_takenby_lbl.Hide();
            wit_statement_lbl.Hide();
            reason_box.Hide();
        }

        private void Wit_select_combobox_ItemSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            if (wit_select_combobox.SelectedItem.Text == _name)
            {
                wit_name_value.Text = _name;
                wit_gender_value.Text = _gender;
                wit_taken_value.Text = _dob;
                reason_box.AddItem("None");
                reason_box.AddItem("Evidence");
                reason_box.AddItem("Testimony -- Victim Family");
                reason_box.AddItem("Testimony -- Suspect");
                reason_box.AddItem("Security Footage");
                reason_box.AddItem("Gut Feeling");

                wit_name_value.Show();
                wit_gender_value.Show();
                wit_taken_value.Show();
                wit_name_lbl.Show();
                wit_gender_lbl.Show();
                wit_takenby_lbl.Show();
                wit_statement_lbl.Show();
                reason_box.Show();
            }
            else
            {
                HideStuff();
            }
        }
        
        private void Witness_return_but_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Window.Close();
            Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.WarrantFiber, ComputerController.Fibers.MainFiber);
        }
    }
}
