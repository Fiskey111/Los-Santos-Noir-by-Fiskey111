using System.ComponentModel;

namespace LSNoir.Callouts.SA.Computer
{
    partial class WitnessForm
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
            this.wit_statement_box = new System.Windows.Forms.TextBox();
            this.wit_statement_lbl = new System.Windows.Forms.Label();
            this.wit_name_lbl = new System.Windows.Forms.Label();
            this.wit_gender_lbl = new System.Windows.Forms.Label();
            this.wit_takenby_lbl = new System.Windows.Forms.Label();
            this.wit_name_value = new System.Windows.Forms.Label();
            this.wit_gender_value = new System.Windows.Forms.Label();
            this.wit_taken_value = new System.Windows.Forms.Label();
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
            this.main_intro_label.Size = new System.Drawing.Size(139, 19);
            this.main_intro_label.TabIndex = 50;
            this.main_intro_label.Text = "Witness Statements";
            // 
            // witness_return_but
            // 
            this.witness_return_but.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.witness_return_but.Location = new System.Drawing.Point(629, 514);
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
            this.wit_select_lbl.Location = new System.Drawing.Point(179, 38);
            this.wit_select_lbl.Name = "wit_select_lbl";
            this.wit_select_lbl.Size = new System.Drawing.Size(125, 19);
            this.wit_select_lbl.TabIndex = 56;
            this.wit_select_lbl.Text = "Selected Witness:";
            // 
            // wit_statement_box
            // 
            this.wit_statement_box.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_statement_box.Location = new System.Drawing.Point(173, 194);
            this.wit_statement_box.Multiline = true;
            this.wit_statement_box.Name = "wit_statement_box";
            this.wit_statement_box.Size = new System.Drawing.Size(461, 289);
            this.wit_statement_box.TabIndex = 57;
            // 
            // wit_statement_lbl
            // 
            this.wit_statement_lbl.AutoSize = true;
            this.wit_statement_lbl.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_statement_lbl.Location = new System.Drawing.Point(40, 193);
            this.wit_statement_lbl.Name = "wit_statement_lbl";
            this.wit_statement_lbl.Size = new System.Drawing.Size(79, 19);
            this.wit_statement_lbl.TabIndex = 58;
            this.wit_statement_lbl.Text = "Statement:";
            // 
            // wit_name_lbl
            // 
            this.wit_name_lbl.AutoSize = true;
            this.wit_name_lbl.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_name_lbl.Location = new System.Drawing.Point(40, 95);
            this.wit_name_lbl.Name = "wit_name_lbl";
            this.wit_name_lbl.Size = new System.Drawing.Size(51, 19);
            this.wit_name_lbl.TabIndex = 59;
            this.wit_name_lbl.Text = "Name:";
            // 
            // wit_gender_lbl
            // 
            this.wit_gender_lbl.AutoSize = true;
            this.wit_gender_lbl.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_gender_lbl.Location = new System.Drawing.Point(40, 124);
            this.wit_gender_lbl.Name = "wit_gender_lbl";
            this.wit_gender_lbl.Size = new System.Drawing.Size(60, 19);
            this.wit_gender_lbl.TabIndex = 60;
            this.wit_gender_lbl.Text = "Gender:";
            // 
            // wit_takenby_lbl
            // 
            this.wit_takenby_lbl.AutoSize = true;
            this.wit_takenby_lbl.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_takenby_lbl.Location = new System.Drawing.Point(40, 155);
            this.wit_takenby_lbl.Name = "wit_takenby_lbl";
            this.wit_takenby_lbl.Size = new System.Drawing.Size(117, 19);
            this.wit_takenby_lbl.TabIndex = 61;
            this.wit_takenby_lbl.Text = "Report Taken By:";
            // 
            // wit_name_value
            // 
            this.wit_name_value.AutoSize = true;
            this.wit_name_value.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_name_value.Location = new System.Drawing.Point(169, 95);
            this.wit_name_value.Name = "wit_name_value";
            this.wit_name_value.Size = new System.Drawing.Size(68, 19);
            this.wit_name_value.TabIndex = 62;
            this.wit_name_value.Text = "John Doe";
            // 
            // wit_gender_value
            // 
            this.wit_gender_value.AutoSize = true;
            this.wit_gender_value.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_gender_value.Location = new System.Drawing.Point(169, 124);
            this.wit_gender_value.Name = "wit_gender_value";
            this.wit_gender_value.Size = new System.Drawing.Size(95, 19);
            this.wit_gender_value.TabIndex = 63;
            this.wit_gender_value.Text = "Female/Male";
            // 
            // wit_taken_value
            // 
            this.wit_taken_value.AutoSize = true;
            this.wit_taken_value.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.wit_taken_value.Location = new System.Drawing.Point(169, 155);
            this.wit_taken_value.Name = "wit_taken_value";
            this.wit_taken_value.Size = new System.Drawing.Size(68, 19);
            this.wit_taken_value.TabIndex = 64;
            this.wit_taken_value.Text = "John Doe";
            // 
            // witness_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.wit_taken_value);
            this.Controls.Add(this.wit_gender_value);
            this.Controls.Add(this.wit_name_value);
            this.Controls.Add(this.wit_takenby_lbl);
            this.Controls.Add(this.wit_gender_lbl);
            this.Controls.Add(this.wit_name_lbl);
            this.Controls.Add(this.wit_statement_lbl);
            this.Controls.Add(this.wit_statement_box);
            this.Controls.Add(this.wit_select_lbl);
            this.Controls.Add(this.wit_select_combobox);
            this.Controls.Add(this.witness_return_but);
            this.Controls.Add(this.main_intro_label);
            this.Controls.Add(this.FiskeyLabel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "WitnessForm";
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
        private System.Windows.Forms.TextBox wit_statement_box;
        private System.Windows.Forms.Label wit_statement_lbl;
        private System.Windows.Forms.Label wit_name_lbl;
        private System.Windows.Forms.Label wit_gender_lbl;
        private System.Windows.Forms.Label wit_takenby_lbl;
        private System.Windows.Forms.Label wit_name_value;
        private System.Windows.Forms.Label wit_gender_value;
        private System.Windows.Forms.Label wit_taken_value;
    }
}