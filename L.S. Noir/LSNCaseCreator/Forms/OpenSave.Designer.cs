namespace LSNCaseCreator.Forms
{
    partial class OpenSave
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
            this.OpenFile = new System.Windows.Forms.OpenFileDialog();
            this.SaveFile = new System.Windows.Forms.SaveFileDialog();
            this.OpenSaveLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OpenSaveLabel
            // 
            this.OpenSaveLabel.AutoSize = true;
            this.OpenSaveLabel.Location = new System.Drawing.Point(13, 13);
            this.OpenSaveLabel.Name = "OpenSaveLabel";
            this.OpenSaveLabel.Size = new System.Drawing.Size(35, 13);
            this.OpenSaveLabel.TabIndex = 0;
            this.OpenSaveLabel.Text = "label1";
            // 
            // OpenSave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 261);
            this.Controls.Add(this.OpenSaveLabel);
            this.Name = "OpenSave";
            this.Text = "OpenSave";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label OpenSaveLabel;
        internal System.Windows.Forms.OpenFileDialog OpenFile;
        internal System.Windows.Forms.SaveFileDialog SaveFile;
    }
}