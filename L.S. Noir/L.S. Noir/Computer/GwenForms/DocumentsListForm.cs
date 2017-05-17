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

        private ComputerController host;

        public DocumentsListForm(ComputerController ctrl, CaseData caseData) : base(typeof(DocumentsList_Form))
        {
            data = caseData;

            host = ctrl;
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

            text.KeyboardInputEnabled = false;
            text.Disable();

            var requestableDocs = data.GetRequestableDocuments();
            requestableDocs.ForEach(r => documentsList.AddRow(r.Title, r.ID, r));

            documentsList.RowSelected += DocumentsList_RowSelected;

            request.Disable();
            request.KeyboardInputEnabled = false;
            request.Clicked += Request_Clicked;

            base.InitializeLayout();
        }

        private void DocumentsList_RowSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            var documentData = documentsList?.SelectedRow?.UserData as DocumentData;

            title.Text = documentData.Title;
            to.Text = documentData.To;

            var lines = documentData.Text.Split(new string[] { "{n}" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                text.SetTextLine(i, lines[i]);
            }

            request.Enable();
            request.KeyboardInputEnabled = true;

            DocumentRequestData requestData = data.GetDocuRequestData(documentData.ID);

            if (requestData == null)
            {
                status.Text = "Available to request";
            }
            else
            { 
                if (!requestData.IsConsidered())
                {
                    request.Disable();
                    request.KeyboardInputEnabled = false;

                    status.Text = "Awaiting decision";
                }
                else
                {
                    request.Disable();
                    request.KeyboardInputEnabled = false;

                    if (data.CanDocumentRequestBeAccepted(requestData.ID)) status.Text = "Accepted";
                    else status.Text = "Refused";

                    data.ModifyCaseProgress(m => m.RequestedDocuments.Where(d => d.ID == documentData.ID).FirstOrDefault().DecisionSeenByPlayer = true);
                }
            }
        }

        private void Request_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (!request.KeyboardInputEnabled) return;
            var documentData = documentsList?.SelectedRow?.UserData as DocumentData;
            var requestData = data.GetDocuRequestData(documentData.ID);

            if (documentData == null)
            {
                var mb = new MessageBox(this, "No warrant!", "WARNING");
                return;
            }

            data.ModifyCaseProgress(m => m.RequestedDocuments.Add(new DocumentRequestData(documentData)));

            request.Disable();

            status.Text = "Awaiting decision";

            GameFiber.StartNew(() =>
            {
                var progressWnd = new GwenForms.ProgressForm("Sending request");
                host.AddWnd(progressWnd);
                progressWnd.Show();
            });
        }

        private DocumentData GetSelectedDocData()
        {
            return documentsList?.SelectedRow?.UserData as DocumentData;
        }
    }
}
