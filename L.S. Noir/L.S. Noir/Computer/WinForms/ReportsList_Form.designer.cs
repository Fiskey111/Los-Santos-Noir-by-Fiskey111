namespace LSNoir.Computer
{
    partial class ReportsList_Form
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
            this.reportsList = new System.Windows.Forms.ListBox();
            this.view = new System.Windows.Forms.Button();
            this.reportTitle = new System.Windows.Forms.TextBox();
            this.reportText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // reportsList
            // 
            this.reportsList.FormattingEnabled = true;
            this.reportsList.ItemHeight = 15;
            this.reportsList.Location = new System.Drawing.Point(17, 20);
            this.reportsList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.reportsList.Name = "reportsList";
            this.reportsList.Size = new System.Drawing.Size(175, 454);
            this.reportsList.TabIndex = 0;
            // 
            // view
            // 
            this.view.Location = new System.Drawing.Point(679, 20);
            this.view.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.view.Name = "view";
            this.view.Size = new System.Drawing.Size(100, 100);
            this.view.TabIndex = 1;
            this.view.Text = "Close";
            this.view.UseVisualStyleBackColor = true;
            // 
            // reportTitle
            // 
            this.reportTitle.Location = new System.Drawing.Point(202, 20);
            this.reportTitle.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.reportTitle.Name = "reportTitle";
            this.reportTitle.Size = new System.Drawing.Size(469, 21);
            this.reportTitle.TabIndex = 3;
            // 
            // reportText
            // 
            this.reportText.Location = new System.Drawing.Point(202, 58);
            this.reportText.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.reportText.Multiline = true;
            this.reportText.Name = "reportText";
            this.reportText.Size = new System.Drawing.Size(469, 416);
            this.reportText.TabIndex = 4;
            this.reportText.Text = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r" +
    "\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
            // 
            // ReportsList_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 483);
            this.Controls.Add(this.reportText);
            this.Controls.Add(this.reportTitle);
            this.Controls.Add(this.view);
            this.Controls.Add(this.reportsList);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.Name = "ReportsList_Form";
            this.Text = "Received reports";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox reportsList;
        private System.Windows.Forms.Button view;
        private System.Windows.Forms.TextBox reportTitle;
        private System.Windows.Forms.TextBox reportText;
    }
}