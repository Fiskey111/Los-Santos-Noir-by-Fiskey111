using System.ComponentModel;

namespace LSNoir.Callouts.SA.Computer
{
    partial class WarrantForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.witness_return_but = new System.Windows.Forms.Button();
            this.wit_select_combobox = new System.Windows.Forms.ComboBox();
            this.wit_select_lbl = new System.Windows.Forms.Label();
            this.wit_statement_lbl = new System.Windows.Forms.Label();
            this.wit_name_lbl = new System.Windows.Forms.Label();
            this.wit_gender_lbl = new System.Windows.Forms.Label();
            this.wit_takenby_lbl = new System.Windows.Forms.Label();
            this.wit_taken_value = new System.Windows.Forms.Label();
            this.wit_gender_value = new System.Windows.Forms.Label();
            this.wit_name_value = new System.Windows.Forms.Label();
            this.reason_box = new System.Windows.Forms.ComboBox();
            this._reqBut = new System.Windows.Forms.Button();
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
            this.main_intro_label.Size = new System.Drawing.Size(118, 19);
            this.main_intro_label.TabIndex = 50;
            this.main_intro_label.Text = "Warrant Request";
            // 
            // witness_return_but
            // 
            this.witness_return_but.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.witness_return_but.Location = new System.Drawing.Point(629, 314);
            this.witness_return_but.Name = "witness_return_but";
            this.witness_return_but.Size = new System.Drawing.Size(143, 35);
            this.witness_return_but.TabIndex = 51;
            this.witness_return_but.Text = "Return to Main Menu";
            this.witness_return_but.UseVisualStyleBackColor = true;
            // 
            // wit_select_combobox
            // 
            this.wit_select_combobox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_select_combobox.FormattingEnabled = true;
            this.wit_select_combobox.Location = new System.Drawing.Point(310, 37);
            this.wit_select_combobox.Name = "wit_select_combobox";
            this.wit_select_combobox.Size = new System.Drawing.Size(167, 22);
            this.wit_select_combobox.TabIndex = 55;
            // 
            // wit_select_lbl
            // 
            this.wit_select_lbl.AutoSize = true;
            this.wit_select_lbl.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_select_lbl.Location = new System.Drawing.Point(160, 36);
            this.wit_select_lbl.Name = "wit_select_lbl";
            this.wit_select_lbl.Size = new System.Drawing.Size(144, 19);
            this.wit_select_lbl.TabIndex = 56;
            this.wit_select_lbl.Text = "Request Warrant for:";
            // 
            // wit_statement_lbl
            // 
            this.wit_statement_lbl.AutoSize = true;
            this.wit_statement_lbl.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_statement_lbl.Location = new System.Drawing.Point(160, 287);
            this.wit_statement_lbl.Name = "wit_statement_lbl";
            this.wit_statement_lbl.Size = new System.Drawing.Size(210, 19);
            this.wit_statement_lbl.TabIndex = 58;
            this.wit_statement_lbl.Text = "Reason for requesting warrant:";
            // 
            // wit_name_lbl
            // 
            this.wit_name_lbl.AutoSize = true;
            this.wit_name_lbl.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_name_lbl.Location = new System.Drawing.Point(160, 110);
            this.wit_name_lbl.Name = "wit_name_lbl";
            this.wit_name_lbl.Size = new System.Drawing.Size(51, 19);
            this.wit_name_lbl.TabIndex = 59;
            this.wit_name_lbl.Text = "Name:";
            // 
            // wit_gender_lbl
            // 
            this.wit_gender_lbl.AutoSize = true;
            this.wit_gender_lbl.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_gender_lbl.Location = new System.Drawing.Point(160, 150);
            this.wit_gender_lbl.Name = "wit_gender_lbl";
            this.wit_gender_lbl.Size = new System.Drawing.Size(60, 19);
            this.wit_gender_lbl.TabIndex = 60;
            this.wit_gender_lbl.Text = "Gender:";
            // 
            // wit_takenby_lbl
            // 
            this.wit_takenby_lbl.AutoSize = true;
            this.wit_takenby_lbl.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_takenby_lbl.Location = new System.Drawing.Point(160, 195);
            this.wit_takenby_lbl.Name = "wit_takenby_lbl";
            this.wit_takenby_lbl.Size = new System.Drawing.Size(43, 19);
            this.wit_takenby_lbl.TabIndex = 61;
            this.wit_takenby_lbl.Text = "DOB:";
            // 
            // wit_taken_value
            // 
            this.wit_taken_value.AutoSize = true;
            this.wit_taken_value.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_taken_value.Location = new System.Drawing.Point(289, 195);
            this.wit_taken_value.Name = "wit_taken_value";
            this.wit_taken_value.Size = new System.Drawing.Size(68, 19);
            this.wit_taken_value.TabIndex = 64;
            this.wit_taken_value.Text = "John Doe";
            // 
            // wit_gender_value
            // 
            this.wit_gender_value.AutoSize = true;
            this.wit_gender_value.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_gender_value.Location = new System.Drawing.Point(289, 150);
            this.wit_gender_value.Name = "wit_gender_value";
            this.wit_gender_value.Size = new System.Drawing.Size(95, 19);
            this.wit_gender_value.TabIndex = 63;
            this.wit_gender_value.Text = "Female/Male";
            // 
            // wit_name_value
            // 
            this.wit_name_value.AutoSize = true;
            this.wit_name_value.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_name_value.Location = new System.Drawing.Point(289, 110);
            this.wit_name_value.Name = "wit_name_value";
            this.wit_name_value.Size = new System.Drawing.Size(68, 19);
            this.wit_name_value.TabIndex = 62;
            this.wit_name_value.Text = "John Doe";
            // 
            // reason_box
            // 
            this.reason_box.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.reason_box.FormattingEnabled = true;
            this.reason_box.Location = new System.Drawing.Point(376, 288);
            this.reason_box.Name = "reason_box";
            this.reason_box.Size = new System.Drawing.Size(211, 22);
            this.reason_box.TabIndex = 65;
            // 
            // _reqBut
            // 
            this._reqBut.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._reqBut.Location = new System.Drawing.Point(310, 316);
            this._reqBut.Name = "_reqBut";
            this._reqBut.Size = new System.Drawing.Size(167, 35);
            this._reqBut.TabIndex = 66;
            this._reqBut.Text = "Request Warrant";
            this._reqBut.UseVisualStyleBackColor = true;
            // 
            // WarrantForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this._reqBut);
            this.Controls.Add(this.reason_box);
            this.Controls.Add(this.wit_taken_value);
            this.Controls.Add(this.wit_gender_value);
            this.Controls.Add(this.wit_name_value);
            this.Controls.Add(this.wit_takenby_lbl);
            this.Controls.Add(this.wit_gender_lbl);
            this.Controls.Add(this.wit_name_lbl);
            this.Controls.Add(this.wit_statement_lbl);
            this.Controls.Add(this.wit_select_lbl);
            this.Controls.Add(this.wit_select_combobox);
            this.Controls.Add(this.witness_return_but);
            this.Controls.Add(this.main_intro_label);
            this.Controls.Add(this.FiskeyLabel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "WarrantForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " San Andreas Joint Records System";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label FiskeyLabel;
        private System.Windows.Forms.Label main_intro_label;
        private System.Windows.Forms.Button witness_return_but;
        private System.Windows.Forms.ComboBox wit_select_combobox;
        private System.Windows.Forms.Label wit_select_lbl;
        private System.Windows.Forms.Label wit_statement_lbl;
        private System.Windows.Forms.Label wit_name_lbl;
        private System.Windows.Forms.Label wit_gender_lbl;
        private System.Windows.Forms.Label wit_takenby_lbl;
        private System.Windows.Forms.Label wit_taken_value;
        private System.Windows.Forms.Label wit_gender_value;
        private System.Windows.Forms.Label wit_name_value;
        private System.Windows.Forms.ComboBox reason_box;
        private System.Windows.Forms.Button _reqBut;
    }
}