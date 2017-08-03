using Gwen.Control;
using LSNoir.Data;
using System.Collections.Generic;
using System.Linq;

namespace LSNoir.Computer.GwenForms
{
    class NotesMadeForm : Rage.Forms.GwenForm
    {
        private readonly CaseData data;

        ListBox notes;
        TextBox title;
        MultilineTextBox text;
        Button close;

        public NotesMadeForm(CaseData caseData) : base(typeof(WinForms.NotesMade_Form))
        {
            data = caseData;
        }

        public override void InitializeLayout()
        {
            title.KeyboardInputEnabled = false;
            title.Disable();

            text.KeyboardInputEnabled = false;
            text.Disable();

            close.Clicked += (s, e) => Window.Close();

            SharedMethods.SetFormPositionCenter(this);

            Window.DisableResizing();

            var notesMadeIDs = data.Progress.GetCaseProgress().NotesMade;

            if (notesMadeIDs == null || notesMadeIDs.Count < 1) return;

            var notesMadeData = GetNotesData(notesMadeIDs).ToList();

            notesMadeData.ForEach(n => notes.AddRow(n.Title, n.ID, n));

            notes.RowSelected += RowSelected;

            base.InitializeLayout();
        }

        private void RowSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            var selectedNote = GetSelectedNoteData();

            if (selectedNote == null) return;

            title.Text = selectedNote.Title;

            for (int i = 0; i < 10; i++)
            {
                text.SetTextLine(i, "");
            }

            SharedMethods.AddSplittedTxtToMultilineTextBox(selectedNote.Text, text);

            //one of those is responsible for displaying text in tb without clickin on it
            text.Disable();
            text.TextPadding = Gwen.Padding.One;
            text.KeyboardInputEnabled = false;
            text.Redraw();
            text.Focus();
        }

        private NoteData GetSelectedNoteData() => notes.SelectedRow.UserData as NoteData;

        private IEnumerable<NoteData> GetNotesData(List<string> ids)
        {
            for (int i = 0; i < ids.Count; i++)
            {
                yield return data.GetNoteData(ids[i]);
            }
        }
    }
}
