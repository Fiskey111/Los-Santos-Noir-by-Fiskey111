namespace LSNoir.Callouts.SA.Computer
{
    partial class EvidenceForm
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
            this.FiskeyLabel = new System.Windows.Forms.Label();
            this.main_intro_label = new System.Windows.Forms.Label();
            this.evidence_return_but = new System.Windows.Forms.Button();
            this.evidence_label = new System.Windows.Forms.Label();
            this.evidence_values_box = new System.Windows.Forms.ListBox();
            this.evidence_lab_but = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FiskeyLabel
            // 
            this.FiskeyLabel.AutoSize = true;
            this.FiskeyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)(((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
                | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FiskeyLabel.ForeColor = System.Drawing.SystemColors.InfoText;
            this.FiskeyLabel.Location = new System.Drawing.Point(511, 9);
            this.FiskeyLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FiskeyLabel.Name = "FiskeyLabel";
            this.FiskeyLabel.Size = new System.Drawing.Size(262, 12);
            this.FiskeyLabel.TabIndex = 49;
            this.FiskeyLabel.Text = "Created by Fiskey111 and LtFlash of LCPDFR.com";
            // 
            // main_intro_label
            // 
            this.main_intro_label.AutoSize = true;
            this.main_intro_label.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_intro_label.Location = new System.Drawing.Point(12, 9);
            this.main_intro_label.Name = "main_intro_label";
            this.main_intro_label.Size = new System.Drawing.Size(168, 19);
            this.main_intro_label.TabIndex = 50;
            this.main_intro_label.Text = "Evidence Collected Form";
            // 
            // evidence_return_but
            // 
            this.evidence_return_but.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.evidence_return_but.Location = new System.Drawing.Point(629, 514);
            this.evidence_return_but.Name = "evidence_return_but";
            this.evidence_return_but.Size = new System.Drawing.Size(143, 35);
            this.evidence_return_but.TabIndex = 51;
            this.evidence_return_but.Text = "Return to Main Menu";
            this.evidence_return_but.UseVisualStyleBackColor = true;
            // 
            // evidence_label
            // 
            this.evidence_label.AutoSize = true;
            this.evidence_label.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.evidence_label.Location = new System.Drawing.Point(44, 92);
            this.evidence_label.Name = "evidence_label";
            this.evidence_label.Size = new System.Drawing.Size(136, 19);
            this.evidence_label.TabIndex = 53;
            this.evidence_label.Text = "Collected Evidence:";
            // 
            // evidence_values_box
            // 
            this.evidence_values_box.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.evidence_values_box.FormattingEnabled = true;
            this.evidence_values_box.ItemHeight = 17;
            this.evidence_values_box.Location = new System.Drawing.Point(186, 92);
            this.evidence_values_box.Name = "evidence_values_box";
            this.evidence_values_box.Size = new System.Drawing.Size(409, 327);
            this.evidence_values_box.TabIndex = 54;
            // 
            // evidence_lab_but
            // 
            this.evidence_lab_but.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.evidence_lab_but.Location = new System.Drawing.Point(308, 425);
            this.evidence_lab_but.Name = "evidence_lab_but";
            this.evidence_lab_but.Size = new System.Drawing.Size(143, 35);
            this.evidence_lab_but.TabIndex = 55;
            this.evidence_lab_but.Text = "Request Lab Testing";
            this.evidence_lab_but.UseVisualStyleBackColor = true;
            // 
            // EvidenceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.evidence_lab_but);
            this.Controls.Add(this.evidence_values_box);
            this.Controls.Add(this.evidence_label);
            this.Controls.Add(this.evidence_return_but);
            this.Controls.Add(this.main_intro_label);
            this.Controls.Add(this.FiskeyLabel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "EvidenceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " San Andreas Joint Records System";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label FiskeyLabel;
        private System.Windows.Forms.Label main_intro_label;
        private System.Windows.Forms.Button evidence_return_but;
        private System.Windows.Forms.Label evidence_label;
        private System.Windows.Forms.ListBox evidence_values_box;
        private System.Windows.Forms.Button evidence_lab_but;
    }
}