namespace LSNoir.Computer.WinForms
{
    partial class NotesMade_Form
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
            this.notes = new System.Windows.Forms.ListBox();
            this.title = new System.Windows.Forms.TextBox();
            this.close = new System.Windows.Forms.Button();
            this.text = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // notes
            // 
            this.notes.FormattingEnabled = true;
            this.notes.ItemHeight = 20;
            this.notes.Location = new System.Drawing.Point(12, 12);
            this.notes.Name = "notes";
            this.notes.Size = new System.Drawing.Size(200, 544);
            this.notes.TabIndex = 0;
            // 
            // title
            // 
            this.title.Location = new System.Drawing.Point(218, 12);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(456, 26);
            this.title.TabIndex = 1;
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(680, 12);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(100, 100);
            this.close.TabIndex = 2;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            // 
            // text
            // 
            this.text.Location = new System.Drawing.Point(218, 44);
            this.text.Multiline = true;
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(456, 512);
            this.text.TabIndex = 3;
            this.text.Text = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
            // 
            // NotesMade_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 565);
            this.Controls.Add(this.text);
            this.Controls.Add(this.close);
            this.Controls.Add(this.title);
            this.Controls.Add(this.notes);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "NotesMade_Form";
            this.Text = "Notes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox notes;
        private System.Windows.Forms.TextBox title;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.TextBox text;
    }
}