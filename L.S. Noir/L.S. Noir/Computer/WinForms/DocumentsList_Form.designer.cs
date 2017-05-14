namespace LSNoir.Computer
{
    partial class DocumentsList_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.documentsList = new System.Windows.Forms.ListBox();
            this.request = new System.Windows.Forms.Button();
            this.title = new System.Windows.Forms.TextBox();
            this.status = new System.Windows.Forms.TextBox();
            this.text = new System.Windows.Forms.TextBox();
            this.to = new System.Windows.Forms.TextBox();
            this.close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // documentsList
            // 
            this.documentsList.FormattingEnabled = true;
            this.documentsList.ItemHeight = 15;
            this.documentsList.Location = new System.Drawing.Point(14, 13);
            this.documentsList.Name = "documentsList";
            this.documentsList.Size = new System.Drawing.Size(179, 544);
            this.documentsList.TabIndex = 0;
            // 
            // request
            // 
            this.request.Location = new System.Drawing.Point(679, 13);
            this.request.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.request.Name = "request";
            this.request.Size = new System.Drawing.Size(100, 100);
            this.request.TabIndex = 6;
            this.request.Text = "Request";
            this.request.UseVisualStyleBackColor = true;
            // 
            // title
            // 
            this.title.Location = new System.Drawing.Point(199, 13);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(471, 21);
            this.title.TabIndex = 7;
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(199, 67);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(471, 21);
            this.status.TabIndex = 8;
            // 
            // text
            // 
            this.text.Location = new System.Drawing.Point(199, 94);
            this.text.Multiline = true;
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(471, 463);
            this.text.TabIndex = 9;
            this.text.Text = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
            // 
            // to
            // 
            this.to.Location = new System.Drawing.Point(199, 40);
            this.to.Name = "to";
            this.to.Size = new System.Drawing.Size(471, 21);
            this.to.TabIndex = 10;
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(679, 123);
            this.close.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(100, 100);
            this.close.TabIndex = 11;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // DocumentsList_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.close);
            this.Controls.Add(this.to);
            this.Controls.Add(this.text);
            this.Controls.Add(this.status);
            this.Controls.Add(this.title);
            this.Controls.Add(this.request);
            this.Controls.Add(this.documentsList);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.Name = "DocumentsList_Form";
            this.Text = "Documents";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox documentsList;
        private System.Windows.Forms.Button request;
        private System.Windows.Forms.TextBox title;
        private System.Windows.Forms.TextBox status;
        private System.Windows.Forms.TextBox text;
        private System.Windows.Forms.TextBox to;
        private System.Windows.Forms.Button close;
    }
}