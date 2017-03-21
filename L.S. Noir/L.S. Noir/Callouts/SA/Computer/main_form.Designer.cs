using Rage;

namespace Notsolethalpolicing.MDT
{
    partial class MainForm
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
        /// the contents of this method wit
        /// h the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FiskeyLabel = new System.Windows.Forms.Label();
            this.main_intro_label = new System.Windows.Forms.Label();
            this.main_casenumber_label = new System.Windows.Forms.Label();
            this.main_casenumber_value = new System.Windows.Forms.Label();
            this.main_laststage_label = new System.Windows.Forms.Label();
            this.main_laststage_value = new System.Windows.Forms.Label();
            this.main_casestatus_value = new System.Windows.Forms.Label();
            this.main_casestatus_label = new System.Windows.Forms.Label();
            this.main_update_label = new System.Windows.Forms.Label();
            this.main_completedcase_label = new System.Windows.Forms.Label();
            this.main_completedcases_value = new System.Windows.Forms.Label();
            this.main_suspect_label = new System.Windows.Forms.Label();
            this.main_witness_label = new System.Windows.Forms.Label();
            this.main_witness_value = new System.Windows.Forms.Label();
            this.main_updates_value = new System.Windows.Forms.ListBox();
            this.main_close_but = new System.Windows.Forms.Button();
            this.main_warrant_value_lbl = new System.Windows.Forms.Label();
            this.main_warrant_lbl = new System.Windows.Forms.Label();
            this.menu_combo = new System.Windows.Forms.ComboBox();
            this.navigate_to_label = new System.Windows.Forms.Label();
            this.nav_button = new System.Windows.Forms.Button();
            this._susBox = new System.Windows.Forms.TextBox();
            this._infoBut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FiskeyLabel
            // 
            this.FiskeyLabel.AutoSize = true;
            this.FiskeyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)(((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
                | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FiskeyLabel.ForeColor = System.Drawing.SystemColors.InfoText;
            this.FiskeyLabel.Location = new System.Drawing.Point(515, 5);
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
            this.main_intro_label.Location = new System.Drawing.Point(5, 5);
            this.main_intro_label.Name = "main_intro_label";
            this.main_intro_label.Size = new System.Drawing.Size(378, 19);
            this.main_intro_label.TabIndex = 50;
            this.main_intro_label.Text = "Welcome to SAJRS.  Your current case is displayed below.";
            // 
            // main_casenumber_label
            // 
            this.main_casenumber_label.AutoSize = true;
            this.main_casenumber_label.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_casenumber_label.Location = new System.Drawing.Point(106, 80);
            this.main_casenumber_label.Name = "main_casenumber_label";
            this.main_casenumber_label.Size = new System.Drawing.Size(87, 17);
            this.main_casenumber_label.TabIndex = 55;
            this.main_casenumber_label.Text = "Case Number:";
            // 
            // main_casenumber_value
            // 
            this.main_casenumber_value.AutoSize = true;
            this.main_casenumber_value.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_casenumber_value.Location = new System.Drawing.Point(239, 80);
            this.main_casenumber_value.Name = "main_casenumber_value";
            this.main_casenumber_value.Size = new System.Drawing.Size(43, 17);
            this.main_casenumber_value.TabIndex = 56;
            this.main_casenumber_value.Text = "99999";
            // 
            // main_laststage_label
            // 
            this.main_laststage_label.AutoSize = true;
            this.main_laststage_label.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_laststage_label.Location = new System.Drawing.Point(380, 120);
            this.main_laststage_label.Name = "main_laststage_label";
            this.main_laststage_label.Size = new System.Drawing.Size(134, 17);
            this.main_laststage_label.TabIndex = 57;
            this.main_laststage_label.Text = "Last Completed Stage:";
            // 
            // main_laststage_value
            // 
            this.main_laststage_value.AutoSize = true;
            this.main_laststage_value.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_laststage_value.Location = new System.Drawing.Point(535, 120);
            this.main_laststage_value.Name = "main_laststage_value";
            this.main_laststage_value.Size = new System.Drawing.Size(43, 17);
            this.main_laststage_value.TabIndex = 58;
            this.main_laststage_value.Text = "99999";
            // 
            // main_casestatus_value
            // 
            this.main_casestatus_value.AutoSize = true;
            this.main_casestatus_value.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_casestatus_value.ForeColor = System.Drawing.Color.Black;
            this.main_casestatus_value.Location = new System.Drawing.Point(535, 80);
            this.main_casestatus_value.Name = "main_casestatus_value";
            this.main_casestatus_value.Size = new System.Drawing.Size(43, 17);
            this.main_casestatus_value.TabIndex = 60;
            this.main_casestatus_value.Text = "99999";
            // 
            // main_casestatus_label
            // 
            this.main_casestatus_label.AutoSize = true;
            this.main_casestatus_label.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_casestatus_label.Location = new System.Drawing.Point(380, 80);
            this.main_casestatus_label.Name = "main_casestatus_label";
            this.main_casestatus_label.Size = new System.Drawing.Size(76, 17);
            this.main_casestatus_label.TabIndex = 59;
            this.main_casestatus_label.Text = "Case Status:";
            // 
            // main_update_label
            // 
            this.main_update_label.AutoSize = true;
            this.main_update_label.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_update_label.Location = new System.Drawing.Point(106, 210);
            this.main_update_label.Name = "main_update_label";
            this.main_update_label.Size = new System.Drawing.Size(130, 17);
            this.main_update_label.TabIndex = 62;
            this.main_update_label.Text = "Last Update to SAJRS:";
            // 
            // main_completedcase_label
            // 
            this.main_completedcase_label.AutoSize = true;
            this.main_completedcase_label.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_completedcase_label.Location = new System.Drawing.Point(106, 120);
            this.main_completedcase_label.Name = "main_completedcase_label";
            this.main_completedcase_label.Size = new System.Drawing.Size(107, 17);
            this.main_completedcase_label.TabIndex = 63;
            this.main_completedcase_label.Text = "Completed Cases:";
            // 
            // main_completedcases_value
            // 
            this.main_completedcases_value.AutoSize = true;
            this.main_completedcases_value.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_completedcases_value.Location = new System.Drawing.Point(239, 120);
            this.main_completedcases_value.Name = "main_completedcases_value";
            this.main_completedcases_value.Size = new System.Drawing.Size(43, 17);
            this.main_completedcases_value.TabIndex = 64;
            this.main_completedcases_value.Text = "99999";
            // 
            // main_suspect_label
            // 
            this.main_suspect_label.AutoSize = true;
            this.main_suspect_label.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_suspect_label.Location = new System.Drawing.Point(106, 413);
            this.main_suspect_label.Name = "main_suspect_label";
            this.main_suspect_label.Size = new System.Drawing.Size(101, 17);
            this.main_suspect_label.TabIndex = 65;
            this.main_suspect_label.Text = "Current Suspect:";
            // 
            // main_witness_label
            // 
            this.main_witness_label.AutoSize = true;
            this.main_witness_label.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_witness_label.Location = new System.Drawing.Point(106, 373);
            this.main_witness_label.Name = "main_witness_label";
            this.main_witness_label.Size = new System.Drawing.Size(122, 17);
            this.main_witness_label.TabIndex = 66;
            this.main_witness_label.Text = "Current Witness(es):";
            // 
            // main_witness_value
            // 
            this.main_witness_value.AutoSize = true;
            this.main_witness_value.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_witness_value.Location = new System.Drawing.Point(239, 373);
            this.main_witness_value.Name = "main_witness_value";
            this.main_witness_value.Size = new System.Drawing.Size(43, 17);
            this.main_witness_value.TabIndex = 67;
            this.main_witness_value.Text = "99999";
            // 
            // main_updates_value
            // 
            this.main_updates_value.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main_updates_value.BackColor = System.Drawing.SystemColors.Window;
            this.main_updates_value.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_updates_value.FormattingEnabled = true;
            this.main_updates_value.ItemHeight = 17;
            this.main_updates_value.Location = new System.Drawing.Point(242, 210);
            this.main_updates_value.Name = "main_updates_value";
            this.main_updates_value.Size = new System.Drawing.Size(300, 140);
            this.main_updates_value.TabIndex = 69;
            // 
            // main_close_but
            // 
            this.main_close_but.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_close_but.Location = new System.Drawing.Point(667, 20);
            this.main_close_but.Name = "main_close_but";
            this.main_close_but.Size = new System.Drawing.Size(105, 33);
            this.main_close_but.TabIndex = 70;
            this.main_close_but.Text = "Close Window";
            this.main_close_but.UseVisualStyleBackColor = true;
            // 
            // main_warrant_value_lbl
            // 
            this.main_warrant_value_lbl.AutoSize = true;
            this.main_warrant_value_lbl.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_warrant_value_lbl.Location = new System.Drawing.Point(239, 164);
            this.main_warrant_value_lbl.Name = "main_warrant_value_lbl";
            this.main_warrant_value_lbl.Size = new System.Drawing.Size(43, 17);
            this.main_warrant_value_lbl.TabIndex = 72;
            this.main_warrant_value_lbl.Text = "99999";
            // 
            // main_warrant_lbl
            // 
            this.main_warrant_lbl.AutoSize = true;
            this.main_warrant_lbl.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.main_warrant_lbl.Location = new System.Drawing.Point(106, 164);
            this.main_warrant_lbl.Name = "main_warrant_lbl";
            this.main_warrant_lbl.Size = new System.Drawing.Size(98, 17);
            this.main_warrant_lbl.TabIndex = 71;
            this.main_warrant_lbl.Text = "Warrant Status:";
            // 
            // menu_combo
            // 
            this.menu_combo.Font = new System.Drawing.Font("Calibri", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.menu_combo.FormattingEnabled = true;
            this.menu_combo.Location = new System.Drawing.Point(273, 478);
            this.menu_combo.Name = "menu_combo";
            this.menu_combo.Size = new System.Drawing.Size(235, 26);
            this.menu_combo.TabIndex = 73;
            // 
            // navigate_to_label
            // 
            this.navigate_to_label.AutoSize = true;
            this.navigate_to_label.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.navigate_to_label.Location = new System.Drawing.Point(189, 482);
            this.navigate_to_label.Name = "navigate_to_label";
            this.navigate_to_label.Size = new System.Drawing.Size(78, 17);
            this.navigate_to_label.TabIndex = 74;
            this.navigate_to_label.Text = "Navigate to:";
            // 
            // nav_button
            // 
            this.nav_button.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.nav_button.Location = new System.Drawing.Point(514, 477);
            this.nav_button.Name = "nav_button";
            this.nav_button.Size = new System.Drawing.Size(109, 29);
            this.nav_button.TabIndex = 75;
            this.nav_button.Text = "Navigate";
            this.nav_button.UseVisualStyleBackColor = true;
            // 
            // _susBox
            // 
            this._susBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this._susBox.Location = new System.Drawing.Point(213, 408);
            this._susBox.Name = "_susBox";
            this._susBox.Size = new System.Drawing.Size(170, 24);
            this._susBox.TabIndex = 76;
            // 
            // _infoBut
            // 
            this._infoBut.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this._infoBut.Location = new System.Drawing.Point(389, 407);
            this._infoBut.Name = "_infoBut";
            this._infoBut.Size = new System.Drawing.Size(109, 29);
            this._infoBut.TabIndex = 77;
            this._infoBut.Text = "Lookup";
            this._infoBut.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this._infoBut);
            this.Controls.Add(this._susBox);
            this.Controls.Add(this.nav_button);
            this.Controls.Add(this.navigate_to_label);
            this.Controls.Add(this.menu_combo);
            this.Controls.Add(this.main_warrant_value_lbl);
            this.Controls.Add(this.main_warrant_lbl);
            this.Controls.Add(this.main_close_but);
            this.Controls.Add(this.main_witness_value);
            this.Controls.Add(this.main_witness_label);
            this.Controls.Add(this.main_suspect_label);
            this.Controls.Add(this.main_completedcases_value);
            this.Controls.Add(this.main_completedcase_label);
            this.Controls.Add(this.main_update_label);
            this.Controls.Add(this.main_casestatus_value);
            this.Controls.Add(this.main_casestatus_label);
            this.Controls.Add(this.main_laststage_value);
            this.Controls.Add(this.main_laststage_label);
            this.Controls.Add(this.main_casenumber_value);
            this.Controls.Add(this.main_casenumber_label);
            this.Controls.Add(this.main_intro_label);
            this.Controls.Add(this.FiskeyLabel);
            this.Controls.Add(this.main_updates_value);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " San Andreas Joint Records System";
            this.Load += new System.EventHandler(this.main_form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label FiskeyLabel;
        private System.Windows.Forms.Label main_intro_label;
        private System.Windows.Forms.Label main_casenumber_label;
        private System.Windows.Forms.Label main_casenumber_value;
        private System.Windows.Forms.Label main_laststage_label;
        private System.Windows.Forms.Label main_laststage_value;
        private System.Windows.Forms.Label main_casestatus_value;
        private System.Windows.Forms.Label main_casestatus_label;
        private System.Windows.Forms.Label main_update_label;
        private System.Windows.Forms.Label main_completedcase_label;
        private System.Windows.Forms.Label main_completedcases_value;
        private System.Windows.Forms.Label main_suspect_label;
        private System.Windows.Forms.Label main_witness_label;
        private System.Windows.Forms.Label main_witness_value;
        private System.Windows.Forms.ListBox main_updates_value;
        private System.Windows.Forms.Button main_close_but;
        private System.Windows.Forms.Label main_warrant_value_lbl;
        private System.Windows.Forms.Label main_warrant_lbl;
        private System.Windows.Forms.ComboBox menu_combo;
        private System.Windows.Forms.Label navigate_to_label;
        private System.Windows.Forms.Button nav_button;
        private System.Windows.Forms.TextBox _susBox;
        private System.Windows.Forms.Button _infoBut;
    }
}