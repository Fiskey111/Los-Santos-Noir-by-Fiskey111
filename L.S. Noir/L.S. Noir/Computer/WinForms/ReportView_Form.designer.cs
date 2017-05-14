namespace LSNoir.Computer
{
    partial class ReportView_Form
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.titleLb = new System.Windows.Forms.Label();
            this.textLb = new System.Windows.Forms.Label();
            this.close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 63);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(456, 327);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(456, 17);
            this.textBox1.TabIndex = 1;
            // 
            // titleLb
            // 
            this.titleLb.AutoSize = true;
            this.titleLb.Location = new System.Drawing.Point(12, 9);
            this.titleLb.Name = "titleLb";
            this.titleLb.Size = new System.Drawing.Size(25, 12);
            this.titleLb.TabIndex = 2;
            this.titleLb.Text = "Title:";
            // 
            // textLb
            // 
            this.textLb.AutoSize = true;
            this.textLb.Location = new System.Drawing.Point(12, 44);
            this.textLb.Name = "textLb";
            this.textLb.Size = new System.Drawing.Size(26, 12);
            this.textLb.TabIndex = 3;
            this.textLb.Text = "Text:";
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(474, 24);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(80, 80);
            this.close.TabIndex = 4;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // ReportView_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5F, 9F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 402);
            this.Controls.Add(this.close);
            this.Controls.Add(this.textLb);
            this.Controls.Add(this.titleLb);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.richTextBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ReportView_Form";
            this.Text = "Report";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label titleLb;
        private System.Windows.Forms.Label textLb;
        private System.Windows.Forms.Button close;
    }
}