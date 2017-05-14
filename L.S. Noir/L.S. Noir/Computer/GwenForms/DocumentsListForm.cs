using Gwen.Control;
using LSNoir.Data;
using Rage;
using System;
using System.Linq;

namespace LSNoir.Computer
{
    class DocumentsListForm : Rage.Forms.GwenForm
    {
        private readonly CaseData data;
        private ListBox documentsList;
        private Button close, request;
        private TextBox title, to, status;
        private MultilineTextBox text;

        public DocumentsListForm(CaseData caseData) : base(typeof(DocumentsList_Form))
        {
            data = caseData;
        }

        public override void InitializeLayout()
        {
            int x = (Game.Resolution.Width / 2) - (Size.Width / 2);
            int y = (Game.Resolution.Height / 2) - (Size.Height / 2);

            Position = new System.Drawing.Point(x, y);

            title.KeyboardInputEnabled = false;
            title.Disable();
            to.KeyboardInputEnabled = false;
            to.Disable();
            status.KeyboardInputEnabled = false;
            status.Disable();

            var d = data.GetRequestableDocuments();
            d.ForEach(r => documentsList.AddRow(r.Title, r.ID, r));

            documentsList.RowSelected += DocumentsList_RowSelected;

            request.Disable();
            request.KeyboardInputEnabled = false;
            request.Clicked += Request_Clicked;

            base.InitializeLayout();
        }

        private void DocumentsList_RowSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            var dd = documentsList?.SelectedRow?.UserData as DocumentData;
            title.Text = dd.Title;
            to.Text = dd.To;

            var t = dd.Text.Split(new string[] { "{n}" }, StringSplitOptions.None);
            for (int i = 0; i < t.Length; i++)
            {
                text.SetTextLine(i, t[i]);
            }

            request.Enable();
            request.KeyboardInputEnabled = true;

            var rd = data.GetDocuRequestData(dd.ID);
            if (rd == null)
            {
                status.Text = "Available to request";
            }
            else
            { 
                if (!rd.Considered)
                {
                    request.Disable();
                    request.KeyboardInputEnabled = false;
                    status.Text = "Awaiting decision";
                }
                else if (rd.Considered)
                {
                    request.Disable();
                    request.KeyboardInputEnabled = false;
                    if (rd.Accepted) status.Text = "Accepted";
                    else status.Text = "Refused";
                    data.ModifyCaseProgress(m => m.RequestedDocuments.Where(d => d.ID == dd.ID).FirstOrDefault().DecisionSeenByPlayer = true);
                }
            }
        }

        private void Request_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (!request.KeyboardInputEnabled) return;
            var cd = documentsList?.SelectedRow?.UserData as DocumentData;
            var drd = data.GetDocuRequestData(cd.ID);

            if (cd == null)
            {
                var mb = new Gwen.Control.MessageBox(this, "No warrant!", "WARNING");
                return;
            }

            data.ModifyCaseProgress(m => m.RequestedDocuments.Add(new DocumentRequestData(cd)));

            request.Disable();

            status.Text = "Awaiting decision";
        }

        private DocumentData GetSelectedDocData()
        {
            return documentsList?.SelectedRow?.UserData as DocumentData;
        }
    }
}
