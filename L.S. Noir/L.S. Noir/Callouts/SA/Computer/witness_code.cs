namespace LSNoir.Callouts.SA.Computer
{/*
    public class WitnessCode : GwenForm
    {
        // System
        private bool _wit2Exists;

        // Gwen
        private ComboBox wit_select_combobox;
        private Button witness_return_but;
        private Label wit_name_value, wit_gender_value, wit_taken_value;
        private Label wit_select_lbl, wit_name_lbl, wit_gender_lbl, wit_takenby_lbl, wit_statement_lbl;
        private MultilineTextBox wit_statement_box;
        
        // Data
        private PedData _w1Data, _w2Data;
        private CaseData _cData;

        public WitnessCode()
            : base(typeof(WitnessForm))
        {

        }

        public override void InitializeLayout()
        {
            base.InitializeLayout();
            Window.IsClosable = false;
            Position = new Point(Game.Resolution.Width / 2 - Window.Width / 2, Game.Resolution.Height / 2 - Window.Height / 2);
            "Initializing San Andreas Joint Records System Witness Statement Viewer".AddLog();

            HideStuff();

            _cData = Serializer.LoadItemFromXML<CaseData>(Main.CDataPath);

            if (FillData()) { }
            else
                wit_select_combobox.AddItem("No Witnesses");

            wit_select_combobox.ItemSelected += Wit_select_combobox_ItemSelected;
            witness_return_but.Clicked += Witness_return_but_Clicked;
        }

        private bool FillData()
        {
            try
            {
                var witDataList = Serializer.LoadItemFromXML<List<PedData>>(Main.WDataPath);

                if (witDataList.Count < 1) return false;

                if (witDataList.Count == 1)
                {
                    _w1Data = Serializer.GetSelectedListElementFromXml<PedData>(Main.WDataPath,
                        c => Enumerable.FirstOrDefault<PedData>(c, p => p.Type == PedType.Witness1));
                    wit_select_combobox.AddItem(_w1Data.Name);
                }
                else
                {
                    _w1Data = Serializer.GetSelectedListElementFromXml<PedData>(Main.WDataPath,
                        c => Enumerable.FirstOrDefault<PedData>(c, p => p.Type == PedType.Witness1));
                    wit_select_combobox.AddItem(_w1Data.Name);
                    _w2Data = Serializer.GetSelectedListElementFromXml<PedData>(Main.WDataPath,
                        c => Enumerable.FirstOrDefault<PedData>(c, p => p.Type == PedType.Witness2));
                    wit_select_combobox.AddItem(_w2Data.Name);
                }
                return true;
            }
            catch (Exception ex)
            {
                $"Error loading witnesses: {ex.ToString()}".AddLog(true);
                return false;
            }
        }

        private void Wit_select_combobox_ItemSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            if (wit_select_combobox.SelectedItem.Text == _w1Data.Name)
            {
                wit_name_value.Text = _w1Data.Name;
                wit_gender_value.Text = _w1Data.Gender.ToString();
                wit_taken_value.Text = Settings.Settings.OfficerName();
                wit_statement_box.Text = ConversationSplitter(_w1Data.Conversation);

                HideStuff(false);
            }
            else if (wit_select_combobox.SelectedItem.Text == _w2Data.Name)
            {
                wit_name_value.Text = _w2Data.Name;
                wit_gender_value.Text = _w2Data.Gender.ToString();
                wit_taken_value.Text = Settings.Settings.OfficerName();
                wit_statement_box.Text = ConversationSplitter(_w2Data.Conversation);

                HideStuff(false);
            }
            else
            {
                HideStuff();
            }
        }

        private void HideStuff(bool hide = true)
        {
            if (!hide)
            {
                wit_name_value.Show();
                wit_gender_value.Show();
                wit_taken_value.Show();
                wit_name_lbl.Show();
                wit_gender_lbl.Show();
                wit_takenby_lbl.Show();
                wit_statement_lbl.Show();
                wit_statement_box.Show();
            }
            else
            {
                wit_name_value.Hide();
                wit_gender_value.Hide();
                wit_taken_value.Hide();
                wit_name_lbl.Hide();
                wit_gender_lbl.Hide();
                wit_takenby_lbl.Hide();
                wit_statement_lbl.Hide();
                wit_statement_box.Hide();
            }
        }

        private void Witness_return_but_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Window.Close();
            Universal.Computer.Controller.SwitchFibers(Universal.Computer.Controller.WitnessFiber, ComputerController.Fibers.MainFiber);
        }

        private string ConversationSplitter(List<string> conversation)
        {
            string dialogue = string.Join("\n", conversation.ToArray());
            return dialogue;
        }
    }*/
}
