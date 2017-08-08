namespace LSNoir.Computer.WinForms
{
    partial class FindVehicle_Form
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
            this.found = new System.Windows.Forms.ListBox();
            this.find = new System.Windows.Forms.TextBox();
            this.search = new System.Windows.Forms.Button();
            this.close = new System.Windows.Forms.Button();
            this.description = new System.Windows.Forms.TextBox();
            this.plate_lb = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // found
            // 
            this.found.FormattingEnabled = true;
            this.found.ItemHeight = 20;
            this.found.Location = new System.Drawing.Point(14, 72);
            this.found.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.found.Name = "found";
            this.found.Size = new System.Drawing.Size(200, 484);
            this.found.TabIndex = 0;
            // 
            // find
            // 
            this.find.Location = new System.Drawing.Point(14, 36);
            this.find.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.find.Name = "find";
            this.find.Size = new System.Drawing.Size(658, 26);
            this.find.TabIndex = 1;
            // 
            // search
            // 
            this.search.Location = new System.Drawing.Point(680, 36);
            this.search.Name = "search";
            this.search.Size = new System.Drawing.Size(100, 100);
            this.search.TabIndex = 2;
            this.search.Text = "Search";
            this.search.UseVisualStyleBackColor = true;
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(680, 142);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(100, 100);
            this.close.TabIndex = 3;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // description
            // 
            this.description.Location = new System.Drawing.Point(224, 72);
            this.description.Multiline = true;
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(448, 484);
            this.description.TabIndex = 4;
            this.description.Text = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
            // 
            // plate_lb
            // 
            this.plate_lb.AutoSize = true;
            this.plate_lb.Location = new System.Drawing.Point(10, 9);
            this.plate_lb.Name = "plate_lb";
            this.plate_lb.Size = new System.Drawing.Size(188, 20);
            this.plate_lb.TabIndex = 5;
            this.plate_lb.Text = "Plate, partial plate or VIN:";
            // 
            // FindVehicle_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.plate_lb);
            this.Controls.Add(this.description);
            this.Controls.Add(this.close);
            this.Controls.Add(this.search);
            this.Controls.Add(this.find);
            this.Controls.Add(this.found);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FindVehicle_Form";
            this.Text = "DMV search";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox found;
        private System.Windows.Forms.TextBox find;
        private System.Windows.Forms.Button search;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.TextBox description;
        private System.Windows.Forms.Label plate_lb;
    }
}