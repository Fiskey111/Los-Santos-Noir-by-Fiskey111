namespace LSNoir.Computer.WinForms
{
    partial class EvidenceList_Form
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
            this.close = new System.Windows.Forms.Button();
            this.request = new System.Windows.Forms.Button();
            this.evidence = new System.Windows.Forms.ListBox();
            this.description = new System.Windows.Forms.TextBox();
            this.status = new System.Windows.Forms.TextBox();
            this.analysis = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(679, 124);
            this.close.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(100, 100);
            this.close.TabIndex = 0;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // request
            // 
            this.request.Location = new System.Drawing.Point(679, 14);
            this.request.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.request.Name = "request";
            this.request.Size = new System.Drawing.Size(100, 100);
            this.request.TabIndex = 1;
            this.request.Text = "Analyze";
            this.request.UseVisualStyleBackColor = true;
            // 
            // evidence
            // 
            this.evidence.FormattingEnabled = true;
            this.evidence.ItemHeight = 20;
            this.evidence.Location = new System.Drawing.Point(12, 14);
            this.evidence.Name = "evidence";
            this.evidence.Size = new System.Drawing.Size(200, 544);
            this.evidence.TabIndex = 2;
            // 
            // description
            // 
            this.description.Location = new System.Drawing.Point(218, 14);
            this.description.Multiline = true;
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(454, 100);
            this.description.TabIndex = 3;
            this.description.Text = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(218, 124);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(454, 26);
            this.status.TabIndex = 4;
            // 
            // analysis
            // 
            this.analysis.Location = new System.Drawing.Point(218, 156);
            this.analysis.Multiline = true;
            this.analysis.Name = "analysis";
            this.analysis.Size = new System.Drawing.Size(454, 402);
            this.analysis.TabIndex = 5;
            this.analysis.Text = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
            // 
            // EvidenceList_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.analysis);
            this.Controls.Add(this.status);
            this.Controls.Add(this.description);
            this.Controls.Add(this.evidence);
            this.Controls.Add(this.request);
            this.Controls.Add(this.close);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "EvidenceList_Form";
            this.Text = "Collected evidence";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Button request;
        private System.Windows.Forms.ListBox evidence;
        private System.Windows.Forms.TextBox description;
        private System.Windows.Forms.TextBox status;
        private System.Windows.Forms.TextBox analysis;
    }
}