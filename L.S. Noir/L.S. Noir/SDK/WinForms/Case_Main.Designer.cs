namespace LSNoir.SDK.WinForms
{
    partial class Case_Main
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
            this.lb_ID = new System.Windows.Forms.Label();
            this.lb_Name = new System.Windows.Forms.Label();
            this.lb_City = new System.Windows.Forms.Label();
            this.lb_Address = new System.Windows.Forms.Label();
            this.tb_ID = new System.Windows.Forms.TextBox();
            this.tb_Name = new System.Windows.Forms.TextBox();
            this.tb_City = new System.Windows.Forms.TextBox();
            this.tb_Address = new System.Windows.Forms.TextBox();
            this.btn_Save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lb_ID
            // 
            this.lb_ID.AutoSize = true;
            this.lb_ID.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.lb_ID.Location = new System.Drawing.Point(18, 17);
            this.lb_ID.Name = "lb_ID";
            this.lb_ID.Size = new System.Drawing.Size(21, 17);
            this.lb_ID.TabIndex = 0;
            this.lb_ID.Text = "ID";
            this.lb_ID.Click += new System.EventHandler(this.label1_Click);
            // 
            // lb_Name
            // 
            this.lb_Name.AutoSize = true;
            this.lb_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.lb_Name.Location = new System.Drawing.Point(18, 77);
            this.lb_Name.Name = "lb_Name";
            this.lb_Name.Size = new System.Drawing.Size(45, 17);
            this.lb_Name.TabIndex = 1;
            this.lb_Name.Text = "Name";
            // 
            // lb_City
            // 
            this.lb_City.AutoSize = true;
            this.lb_City.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.lb_City.Location = new System.Drawing.Point(18, 132);
            this.lb_City.Name = "lb_City";
            this.lb_City.Size = new System.Drawing.Size(31, 17);
            this.lb_City.TabIndex = 2;
            this.lb_City.Text = "City";
            // 
            // lb_Address
            // 
            this.lb_Address.AutoSize = true;
            this.lb_Address.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.lb_Address.Location = new System.Drawing.Point(18, 187);
            this.lb_Address.Name = "lb_Address";
            this.lb_Address.Size = new System.Drawing.Size(60, 17);
            this.lb_Address.TabIndex = 3;
            this.lb_Address.Text = "Address";
            // 
            // tb_ID
            // 
            this.tb_ID.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.tb_ID.Location = new System.Drawing.Point(158, 11);
            this.tb_ID.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tb_ID.Name = "tb_ID";
            this.tb_ID.Size = new System.Drawing.Size(158, 23);
            this.tb_ID.TabIndex = 10;
            // 
            // tb_Name
            // 
            this.tb_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.tb_Name.Location = new System.Drawing.Point(158, 66);
            this.tb_Name.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tb_Name.Name = "tb_Name";
            this.tb_Name.Size = new System.Drawing.Size(158, 23);
            this.tb_Name.TabIndex = 11;
            // 
            // tb_City
            // 
            this.tb_City.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.tb_City.Location = new System.Drawing.Point(158, 121);
            this.tb_City.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tb_City.Name = "tb_City";
            this.tb_City.Size = new System.Drawing.Size(158, 23);
            this.tb_City.TabIndex = 12;
            // 
            // tb_Address
            // 
            this.tb_Address.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.tb_Address.Location = new System.Drawing.Point(158, 176);
            this.tb_Address.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tb_Address.Name = "tb_Address";
            this.tb_Address.Size = new System.Drawing.Size(158, 23);
            this.tb_Address.TabIndex = 13;
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(196, 228);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(120, 43);
            this.btn_Save.TabIndex = 14;
            this.btn_Save.Text = "button1";
            this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // Case_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 368);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.tb_Address);
            this.Controls.Add(this.tb_City);
            this.Controls.Add(this.tb_Name);
            this.Controls.Add(this.tb_ID);
            this.Controls.Add(this.lb_Address);
            this.Controls.Add(this.lb_City);
            this.Controls.Add(this.lb_Name);
            this.Controls.Add(this.lb_ID);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Case_Main";
            this.Text = "Case_Main";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_ID;
        private System.Windows.Forms.Label lb_Name;
        private System.Windows.Forms.Label lb_City;
        private System.Windows.Forms.Label lb_Address;
        private System.Windows.Forms.TextBox tb_ID;
        private System.Windows.Forms.TextBox tb_Name;
        private System.Windows.Forms.TextBox tb_City;
        private System.Windows.Forms.TextBox tb_Address;
        private System.Windows.Forms.Button btn_Save;
    }
}