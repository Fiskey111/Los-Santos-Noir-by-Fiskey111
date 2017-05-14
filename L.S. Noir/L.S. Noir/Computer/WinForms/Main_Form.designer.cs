namespace LSNoir.Computer
{
    partial class Main_Form
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
            this.reports = new System.Windows.Forms.Button();
            this.warrants = new System.Windows.Forms.Button();
            this.evidence = new System.Windows.Forms.Button();
            this.victimLb = new System.Windows.Forms.Label();
            this.victim = new System.Windows.Forms.Label();
            this.caseNoLb = new System.Windows.Forms.Label();
            this.caseNo = new System.Windows.Forms.Label();
            this.notes = new System.Windows.Forms.Button();
            this.casesLb = new System.Windows.Forms.Label();
            this.listCases = new System.Windows.Forms.ListBox();
            this.cityLb = new System.Windows.Forms.Label();
            this.addressLb = new System.Windows.Forms.Label();
            this.firstOfficerLb = new System.Windows.Forms.Label();
            this.city = new System.Windows.Forms.Label();
            this.address = new System.Windows.Forms.Label();
            this.firstOfficer = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // reports
            // 
            this.reports.Location = new System.Drawing.Point(191, 384);
            this.reports.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.reports.Name = "reports";
            this.reports.Size = new System.Drawing.Size(100, 100);
            this.reports.TabIndex = 0;
            this.reports.Text = "Reports";
            this.reports.UseVisualStyleBackColor = true;
            // 
            // warrants
            // 
            this.warrants.Location = new System.Drawing.Point(299, 384);
            this.warrants.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.warrants.Name = "warrants";
            this.warrants.Size = new System.Drawing.Size(100, 100);
            this.warrants.TabIndex = 1;
            this.warrants.Text = "Documents";
            this.warrants.UseVisualStyleBackColor = true;
            // 
            // evidence
            // 
            this.evidence.Location = new System.Drawing.Point(407, 384);
            this.evidence.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.evidence.Name = "evidence";
            this.evidence.Size = new System.Drawing.Size(100, 100);
            this.evidence.TabIndex = 2;
            this.evidence.Text = "Evidence";
            this.evidence.UseVisualStyleBackColor = true;
            // 
            // victimLb
            // 
            this.victimLb.AutoSize = true;
            this.victimLb.Location = new System.Drawing.Point(191, 85);
            this.victimLb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.victimLb.Name = "victimLb";
            this.victimLb.Size = new System.Drawing.Size(57, 15);
            this.victimLb.TabIndex = 3;
            this.victimLb.Text = "Victim(s):";
            // 
            // victim
            // 
            this.victim.AutoSize = true;
            this.victim.Location = new System.Drawing.Point(256, 85);
            this.victim.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.victim.Name = "victim";
            this.victim.Size = new System.Drawing.Size(24, 15);
            this.victim.TabIndex = 4;
            this.victim.Text = "n/a";
            // 
            // caseNoLb
            // 
            this.caseNoLb.AutoSize = true;
            this.caseNoLb.Location = new System.Drawing.Point(191, 45);
            this.caseNoLb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.caseNoLb.Name = "caseNoLb";
            this.caseNoLb.Size = new System.Drawing.Size(45, 15);
            this.caseNoLb.TabIndex = 5;
            this.caseNoLb.Text = "Case#:";
            // 
            // caseNo
            // 
            this.caseNo.AutoSize = true;
            this.caseNo.Location = new System.Drawing.Point(244, 45);
            this.caseNo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.caseNo.Name = "caseNo";
            this.caseNo.Size = new System.Drawing.Size(24, 15);
            this.caseNo.TabIndex = 6;
            this.caseNo.Text = "n/a";
            // 
            // notes
            // 
            this.notes.Location = new System.Drawing.Point(515, 384);
            this.notes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.notes.Name = "notes";
            this.notes.Size = new System.Drawing.Size(100, 100);
            this.notes.TabIndex = 7;
            this.notes.Text = "Notes";
            this.notes.UseVisualStyleBackColor = true;
            // 
            // casesLb
            // 
            this.casesLb.AutoSize = true;
            this.casesLb.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.casesLb.Location = new System.Drawing.Point(17, 15);
            this.casesLb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.casesLb.Name = "casesLb";
            this.casesLb.Size = new System.Drawing.Size(75, 15);
            this.casesLb.TabIndex = 10;
            this.casesLb.Text = "Open cases:";
            // 
            // listCases
            // 
            this.listCases.FormattingEnabled = true;
            this.listCases.ItemHeight = 15;
            this.listCases.Location = new System.Drawing.Point(17, 45);
            this.listCases.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listCases.Name = "listCases";
            this.listCases.Size = new System.Drawing.Size(166, 439);
            this.listCases.TabIndex = 11;
            // 
            // cityLb
            // 
            this.cityLb.AutoSize = true;
            this.cityLb.Location = new System.Drawing.Point(191, 125);
            this.cityLb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cityLb.Name = "cityLb";
            this.cityLb.Size = new System.Drawing.Size(29, 15);
            this.cityLb.TabIndex = 12;
            this.cityLb.Text = "City:";
            // 
            // addressLb
            // 
            this.addressLb.AutoSize = true;
            this.addressLb.Location = new System.Drawing.Point(188, 165);
            this.addressLb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.addressLb.Name = "addressLb";
            this.addressLb.Size = new System.Drawing.Size(54, 15);
            this.addressLb.TabIndex = 13;
            this.addressLb.Text = "Address:";
            // 
            // firstOfficerLb
            // 
            this.firstOfficerLb.AutoSize = true;
            this.firstOfficerLb.Location = new System.Drawing.Point(188, 205);
            this.firstOfficerLb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.firstOfficerLb.Name = "firstOfficerLb";
            this.firstOfficerLb.Size = new System.Drawing.Size(118, 15);
            this.firstOfficerLb.TabIndex = 14;
            this.firstOfficerLb.Text = "First officer at scene:";
            // 
            // city
            // 
            this.city.AutoSize = true;
            this.city.Location = new System.Drawing.Point(228, 125);
            this.city.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.city.Name = "city";
            this.city.Size = new System.Drawing.Size(24, 15);
            this.city.TabIndex = 15;
            this.city.Text = "n/a";
            // 
            // address
            // 
            this.address.AutoSize = true;
            this.address.Location = new System.Drawing.Point(250, 165);
            this.address.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.address.Name = "address";
            this.address.Size = new System.Drawing.Size(24, 15);
            this.address.TabIndex = 16;
            this.address.Text = "n/a";
            this.address.UseMnemonic = false;
            // 
            // firstOfficer
            // 
            this.firstOfficer.AutoSize = true;
            this.firstOfficer.Location = new System.Drawing.Point(314, 205);
            this.firstOfficer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.firstOfficer.Name = "firstOfficer";
            this.firstOfficer.Size = new System.Drawing.Size(24, 15);
            this.firstOfficer.TabIndex = 17;
            this.firstOfficer.Text = "n/a";
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 499);
            this.Controls.Add(this.firstOfficer);
            this.Controls.Add(this.address);
            this.Controls.Add(this.city);
            this.Controls.Add(this.firstOfficerLb);
            this.Controls.Add(this.addressLb);
            this.Controls.Add(this.cityLb);
            this.Controls.Add(this.listCases);
            this.Controls.Add(this.casesLb);
            this.Controls.Add(this.notes);
            this.Controls.Add(this.caseNo);
            this.Controls.Add(this.caseNoLb);
            this.Controls.Add(this.victim);
            this.Controls.Add(this.victimLb);
            this.Controls.Add(this.evidence);
            this.Controls.Add(this.warrants);
            this.Controls.Add(this.reports);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.Name = "Main_Form";
            this.Text = "SAJRS Terminal * AUTHORISED PERSONNEL ONLY";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button reports;
        private System.Windows.Forms.Button warrants;
        private System.Windows.Forms.Button evidence;
        private System.Windows.Forms.Label victimLb;
        private System.Windows.Forms.Label victim;
        private System.Windows.Forms.Label caseNoLb;
        private System.Windows.Forms.Label caseNo;
        private System.Windows.Forms.Button notes;
        private System.Windows.Forms.Label casesLb;
        private System.Windows.Forms.ListBox listCases;
        private System.Windows.Forms.Label cityLb;
        private System.Windows.Forms.Label addressLb;
        private System.Windows.Forms.Label firstOfficerLb;
        private System.Windows.Forms.Label city;
        private System.Windows.Forms.Label address;
        private System.Windows.Forms.Label firstOfficer;
    }
}