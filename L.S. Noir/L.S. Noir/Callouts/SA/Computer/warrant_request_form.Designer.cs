namespace LSNoir.Callouts.SA.Computer
{
    partial class WarrantRequestForm
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
            this.req_return_but = new System.Windows.Forms.Button();
            this.wit_name_lbl = new System.Windows.Forms.Label();
            this.req_progress_bar = new System.Windows.Forms.ProgressBar();
            this.req_sending_lbl = new System.Windows.Forms.Label();
            this.req_reason_lbl = new System.Windows.Forms.Label();
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
            this.main_intro_label.Size = new System.Drawing.Size(194, 19);
            this.main_intro_label.TabIndex = 50;
            this.main_intro_label.Text = "Warrant Request In Progress";
            // 
            // req_return_but
            // 
            this.req_return_but.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.req_return_but.Location = new System.Drawing.Point(304, 314);
            this.req_return_but.Name = "req_return_but";
            this.req_return_but.Size = new System.Drawing.Size(143, 35);
            this.req_return_but.TabIndex = 51;
            this.req_return_but.Text = "Return to Main Menu";
            this.req_return_but.UseVisualStyleBackColor = true;
            this.req_return_but.Click += new System.EventHandler(this.witness_return_but_Click);
            // 
            // wit_name_lbl
            // 
            this.wit_name_lbl.AutoSize = true;
            this.wit_name_lbl.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_name_lbl.Location = new System.Drawing.Point(200, 67);
            this.wit_name_lbl.Name = "wit_name_lbl";
            this.wit_name_lbl.Size = new System.Drawing.Size(360, 19);
            this.wit_name_lbl.TabIndex = 59;
            this.wit_name_lbl.Text = "Warrant Request Being Sent for the Following Reason:";
            // 
            // req_progress_bar
            // 
            this.req_progress_bar.Location = new System.Drawing.Point(220, 217);
            this.req_progress_bar.Name = "req_progress_bar";
            this.req_progress_bar.Size = new System.Drawing.Size(315, 23);
            this.req_progress_bar.Step = 5;
            this.req_progress_bar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.req_progress_bar.TabIndex = 60;
            this.req_progress_bar.Value = 50;
            // 
            // req_sending_lbl
            // 
            this.req_sending_lbl.AutoSize = true;
            this.req_sending_lbl.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.req_sending_lbl.Location = new System.Drawing.Point(318, 195);
            this.req_sending_lbl.Name = "req_sending_lbl";
            this.req_sending_lbl.Size = new System.Drawing.Size(117, 19);
            this.req_sending_lbl.TabIndex = 61;
            this.req_sending_lbl.Text = "Sending Request";
            // 
            // req_reason_lbl
            // 
            this.req_reason_lbl.AutoSize = true;
            this.req_reason_lbl.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.req_reason_lbl.Location = new System.Drawing.Point(318, 115);
            this.req_reason_lbl.Name = "req_reason_lbl";
            this.req_reason_lbl.Size = new System.Drawing.Size(183, 19);
            this.req_reason_lbl.TabIndex = 62;
            this.req_reason_lbl.Text = "Testimony -- Person Family";
            // 
            // WarrantRequestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.req_reason_lbl);
            this.Controls.Add(this.req_sending_lbl);
            this.Controls.Add(this.req_progress_bar);
            this.Controls.Add(this.wit_name_lbl);
            this.Controls.Add(this.req_return_but);
            this.Controls.Add(this.main_intro_label);
            this.Controls.Add(this.FiskeyLabel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "WarrantRequestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " San Andreas Joint Records System";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label FiskeyLabel;
        private System.Windows.Forms.Label main_intro_label;
        private System.Windows.Forms.Button req_return_but;
        private System.Windows.Forms.Label wit_name_lbl;
        private System.Windows.Forms.ProgressBar req_progress_bar;
        private System.Windows.Forms.Label req_sending_lbl;
        private System.Windows.Forms.Label req_reason_lbl;
    }
}