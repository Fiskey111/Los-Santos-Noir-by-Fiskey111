namespace LSNoir.SDK.WinForms
{
    partial class CSI_Main
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
            this.btn_Notif = new System.Windows.Forms.Button();
            this.btn_CallPos = new System.Windows.Forms.Button();
            this.lb_ID = new System.Windows.Forms.Label();
            this.lb_Name = new System.Windows.Forms.Label();
            this.lb_DelMax = new System.Windows.Forms.Label();
            this.lb_Address = new System.Windows.Forms.Label();
            this.lb_CallRad = new System.Windows.Forms.Label();
            this.lb_DelMin = new System.Windows.Forms.Label();
            this.btn_Scene = new System.Windows.Forms.Button();
            this.btn_Victims = new System.Windows.Forms.Button();
            this.btn_Wit = new System.Windows.Forms.Button();
            this.btn_Evid = new System.Windows.Forms.Button();
            this.btn_Officer = new System.Windows.Forms.Button();
            this.tb_ID = new System.Windows.Forms.TextBox();
            this.tb_Name = new System.Windows.Forms.TextBox();
            this.tb_Address = new System.Windows.Forms.TextBox();
            this.tb_CallRad = new System.Windows.Forms.TextBox();
            this.tb_DelMin = new System.Windows.Forms.TextBox();
            this.tb_DelMax = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_Notif
            // 
            this.btn_Notif.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.btn_Notif.Location = new System.Drawing.Point(93, 195);
            this.btn_Notif.Name = "btn_Notif";
            this.btn_Notif.Size = new System.Drawing.Size(75, 44);
            this.btn_Notif.TabIndex = 0;
            this.btn_Notif.Text = "Customize notification";
            this.btn_Notif.UseVisualStyleBackColor = true;
            // 
            // btn_CallPos
            // 
            this.btn_CallPos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.btn_CallPos.Location = new System.Drawing.Point(12, 195);
            this.btn_CallPos.Name = "btn_CallPos";
            this.btn_CallPos.Size = new System.Drawing.Size(75, 44);
            this.btn_CallPos.TabIndex = 1;
            this.btn_CallPos.Text = "Set call position here";
            this.btn_CallPos.UseVisualStyleBackColor = true;
            // 
            // lb_ID
            // 
            this.lb_ID.AutoSize = true;
            this.lb_ID.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.lb_ID.Location = new System.Drawing.Point(13, 6);
            this.lb_ID.Name = "lb_ID";
            this.lb_ID.Size = new System.Drawing.Size(21, 17);
            this.lb_ID.TabIndex = 2;
            this.lb_ID.Text = "ID";
            // 
            // lb_Name
            // 
            this.lb_Name.AutoSize = true;
            this.lb_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.lb_Name.Location = new System.Drawing.Point(13, 35);
            this.lb_Name.Name = "lb_Name";
            this.lb_Name.Size = new System.Drawing.Size(45, 17);
            this.lb_Name.TabIndex = 3;
            this.lb_Name.Text = "Name";
            // 
            // lb_DelMax
            // 
            this.lb_DelMax.AutoSize = true;
            this.lb_DelMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.lb_DelMax.Location = new System.Drawing.Point(12, 151);
            this.lb_DelMax.Name = "lb_DelMax";
            this.lb_DelMax.Size = new System.Drawing.Size(73, 17);
            this.lb_DelMax.TabIndex = 4;
            this.lb_DelMax.Text = "Delay max";
            // 
            // lb_Address
            // 
            this.lb_Address.AutoSize = true;
            this.lb_Address.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.lb_Address.Location = new System.Drawing.Point(12, 64);
            this.lb_Address.Name = "lb_Address";
            this.lb_Address.Size = new System.Drawing.Size(60, 17);
            this.lb_Address.TabIndex = 5;
            this.lb_Address.Text = "Address";
            // 
            // lb_CallRad
            // 
            this.lb_CallRad.AutoSize = true;
            this.lb_CallRad.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.lb_CallRad.Location = new System.Drawing.Point(13, 93);
            this.lb_CallRad.Name = "lb_CallRad";
            this.lb_CallRad.Size = new System.Drawing.Size(107, 17);
            this.lb_CallRad.TabIndex = 6;
            this.lb_CallRad.Text = "Call area radius";
            // 
            // lb_DelMin
            // 
            this.lb_DelMin.AutoSize = true;
            this.lb_DelMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.lb_DelMin.Location = new System.Drawing.Point(12, 122);
            this.lb_DelMin.Name = "lb_DelMin";
            this.lb_DelMin.Size = new System.Drawing.Size(104, 17);
            this.lb_DelMin.TabIndex = 7;
            this.lb_DelMin.Text = "Delay min [sec]";
            // 
            // btn_Scene
            // 
            this.btn_Scene.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.btn_Scene.Location = new System.Drawing.Point(174, 195);
            this.btn_Scene.Name = "btn_Scene";
            this.btn_Scene.Size = new System.Drawing.Size(75, 44);
            this.btn_Scene.TabIndex = 8;
            this.btn_Scene.Text = "Create scene";
            this.btn_Scene.UseVisualStyleBackColor = true;
            // 
            // btn_Victims
            // 
            this.btn_Victims.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.btn_Victims.Location = new System.Drawing.Point(12, 245);
            this.btn_Victims.Name = "btn_Victims";
            this.btn_Victims.Size = new System.Drawing.Size(75, 44);
            this.btn_Victims.TabIndex = 9;
            this.btn_Victims.Text = "Victims";
            this.btn_Victims.UseVisualStyleBackColor = true;
            // 
            // btn_Wit
            // 
            this.btn_Wit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.btn_Wit.Location = new System.Drawing.Point(174, 245);
            this.btn_Wit.Name = "btn_Wit";
            this.btn_Wit.Size = new System.Drawing.Size(75, 44);
            this.btn_Wit.TabIndex = 10;
            this.btn_Wit.Text = "Witnesses";
            this.btn_Wit.UseVisualStyleBackColor = true;
            // 
            // btn_Evid
            // 
            this.btn_Evid.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.btn_Evid.Location = new System.Drawing.Point(93, 245);
            this.btn_Evid.Name = "btn_Evid";
            this.btn_Evid.Size = new System.Drawing.Size(75, 44);
            this.btn_Evid.TabIndex = 11;
            this.btn_Evid.Text = "Evidences";
            this.btn_Evid.UseVisualStyleBackColor = true;
            // 
            // btn_Officer
            // 
            this.btn_Officer.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.btn_Officer.Location = new System.Drawing.Point(12, 295);
            this.btn_Officer.Name = "btn_Officer";
            this.btn_Officer.Size = new System.Drawing.Size(75, 44);
            this.btn_Officer.TabIndex = 12;
            this.btn_Officer.Text = "First officer";
            this.btn_Officer.UseVisualStyleBackColor = true;
            // 
            // tb_ID
            // 
            this.tb_ID.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.tb_ID.Location = new System.Drawing.Point(126, 3);
            this.tb_ID.Name = "tb_ID";
            this.tb_ID.Size = new System.Drawing.Size(142, 23);
            this.tb_ID.TabIndex = 13;
            // 
            // tb_Name
            // 
            this.tb_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.tb_Name.Location = new System.Drawing.Point(126, 32);
            this.tb_Name.Name = "tb_Name";
            this.tb_Name.Size = new System.Drawing.Size(142, 23);
            this.tb_Name.TabIndex = 14;
            // 
            // tb_Address
            // 
            this.tb_Address.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.tb_Address.Location = new System.Drawing.Point(126, 61);
            this.tb_Address.Name = "tb_Address";
            this.tb_Address.Size = new System.Drawing.Size(142, 23);
            this.tb_Address.TabIndex = 15;
            // 
            // tb_CallRad
            // 
            this.tb_CallRad.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.tb_CallRad.Location = new System.Drawing.Point(126, 90);
            this.tb_CallRad.Name = "tb_CallRad";
            this.tb_CallRad.Size = new System.Drawing.Size(142, 23);
            this.tb_CallRad.TabIndex = 16;
            // 
            // tb_DelMin
            // 
            this.tb_DelMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.tb_DelMin.Location = new System.Drawing.Point(126, 119);
            this.tb_DelMin.Name = "tb_DelMin";
            this.tb_DelMin.Size = new System.Drawing.Size(142, 23);
            this.tb_DelMin.TabIndex = 17;
            // 
            // tb_DelMax
            // 
            this.tb_DelMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.tb_DelMax.Location = new System.Drawing.Point(126, 148);
            this.tb_DelMax.Name = "tb_DelMax";
            this.tb_DelMax.Size = new System.Drawing.Size(142, 23);
            this.tb_DelMax.TabIndex = 18;
            // 
            // CSI_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5F, 9F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 407);
            this.Controls.Add(this.tb_DelMax);
            this.Controls.Add(this.tb_DelMin);
            this.Controls.Add(this.tb_CallRad);
            this.Controls.Add(this.tb_Address);
            this.Controls.Add(this.tb_Name);
            this.Controls.Add(this.tb_ID);
            this.Controls.Add(this.btn_Officer);
            this.Controls.Add(this.btn_Evid);
            this.Controls.Add(this.btn_Wit);
            this.Controls.Add(this.btn_Victims);
            this.Controls.Add(this.btn_Scene);
            this.Controls.Add(this.lb_DelMin);
            this.Controls.Add(this.lb_CallRad);
            this.Controls.Add(this.lb_Address);
            this.Controls.Add(this.lb_DelMax);
            this.Controls.Add(this.lb_Name);
            this.Controls.Add(this.lb_ID);
            this.Controls.Add(this.btn_CallPos);
            this.Controls.Add(this.btn_Notif);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "CSI_Main";
            this.Text = "CSI_Main";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Notif;
        private System.Windows.Forms.Button btn_CallPos;
        private System.Windows.Forms.Label lb_ID;
        private System.Windows.Forms.Label lb_Name;
        private System.Windows.Forms.Label lb_DelMax;
        private System.Windows.Forms.Label lb_Address;
        private System.Windows.Forms.Label lb_CallRad;
        private System.Windows.Forms.Label lb_DelMin;
        private System.Windows.Forms.Button btn_Scene;
        private System.Windows.Forms.Button btn_Victims;
        private System.Windows.Forms.Button btn_Wit;
        private System.Windows.Forms.Button btn_Evid;
        private System.Windows.Forms.Button btn_Officer;
        private System.Windows.Forms.TextBox tb_ID;
        private System.Windows.Forms.TextBox tb_Name;
        private System.Windows.Forms.TextBox tb_Address;
        private System.Windows.Forms.TextBox tb_CallRad;
        private System.Windows.Forms.TextBox tb_DelMin;
        private System.Windows.Forms.TextBox tb_DelMax;
    }
}