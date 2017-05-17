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
            this.close = new System.Windows.Forms.Button();
            this.reportTitle = new System.Windows.Forms.TextBox();
            this.reportText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // reportsList
            // 
            this.reportsList.FormattingEnabled = true;
            this.reportsList.ItemHeight = 20;
            this.reportsList.Location = new System.Drawing.Point(14, 16);
            this.reportsList.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.reportsList.Name = "reportsList";
            this.reportsList.Size = new System.Drawing.Size(200, 544);
            this.reportsList.TabIndex = 0;
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(678, 16);
            this.close.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(100, 100);
            this.close.TabIndex = 1;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // reportTitle
            // 
            this.reportTitle.Location = new System.Drawing.Point(224, 16);
            this.reportTitle.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.reportTitle.Name = "reportTitle";
            this.reportTitle.Size = new System.Drawing.Size(444, 26);
            this.reportTitle.TabIndex = 3;
            // 
            // reportText
            // 
            this.reportText.Location = new System.Drawing.Point(224, 56);
            this.reportText.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.reportText.Multiline = true;
            this.reportText.Name = "reportText";
            this.reportText.Size = new System.Drawing.Size(444, 501);
            this.reportText.TabIndex = 4;
            this.reportText.Text = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r" +
    "\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
            // 
            // ReportsList_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.reportText);
            this.Controls.Add(this.reportTitle);
            this.Controls.Add(this.close);
            this.Controls.Add(this.reportsList);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ReportsList_Form";
            this.Text = "Received reports";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox reportsList;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.TextBox reportTitle;
        private System.Windows.Forms.TextBox reportText;
    }
}