namespace LSNoir
{
    partial class MessageBoxForm
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
            this.return_but = new System.Windows.Forms.Button();
            this.Message_Text = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // return_but
            // 
            this.return_but.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.return_but.Location = new System.Drawing.Point(218, 124);
            this.return_but.Name = "return_but";
            this.return_but.Size = new System.Drawing.Size(128, 25);
            this.return_but.TabIndex = 51;
            this.return_but.Text = "OK";
            this.return_but.UseVisualStyleBackColor = true;
            // 
            // Message_Text
            // 
            this.Message_Text.AutoSize = true;
            this.Message_Text.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.Message_Text.Location = new System.Drawing.Point(53, 31);
            this.Message_Text.Name = "Message_Text";
            this.Message_Text.Size = new System.Drawing.Size(489, 57);
            this.Message_Text.TabIndex = 53;
            this.Message_Text.Text = "111111111111111111111111111111111111111111111111111111111111\r\n1111111111111111111" +
    "11111111111111111111111111111111111111111\r\n1111111111111111111111111111111111111" +
    "11111111111111111111111";
            // 
            // MessageBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(584, 161);
            this.Controls.Add(this.Message_Text);
            this.Controls.Add(this.return_but);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "MessageBoxForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "!   Message";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button return_but;
        private System.Windows.Forms.Label Message_Text;
    }
}