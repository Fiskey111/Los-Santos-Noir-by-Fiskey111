namespace LSNoir.SDK.WinForms
{
    partial class SDK_Main
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
            this.btn_CreateCase = new System.Windows.Forms.Button();
            this.btn_CreateStage = new System.Windows.Forms.Button();
            this.cb_NewStageType = new System.Windows.Forms.ComboBox();
            this.lb_Case = new System.Windows.Forms.Label();
            this.lb_Stage = new System.Windows.Forms.Label();
            this.cb_Case = new System.Windows.Forms.ComboBox();
            this.tb_NewCaseID = new System.Windows.Forms.TextBox();
            this.tb_NewStageID = new System.Windows.Forms.TextBox();
            this.lb_NewCaseID = new System.Windows.Forms.Label();
            this.lb_NewStageID = new System.Windows.Forms.Label();
            this.lb_NewStageType = new System.Windows.Forms.Label();
            this.btn_EditCase = new System.Windows.Forms.Button();
            this.btn_EditStage = new System.Windows.Forms.Button();
            this.lb_NewCase = new System.Windows.Forms.Label();
            this.lb_NewStage = new System.Windows.Forms.Label();
            this.lbx_Stages = new System.Windows.Forms.ListBox();
            this.btn_StageUp = new System.Windows.Forms.Button();
            this.btn_StageDown = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_CreateCase
            // 
            this.btn_CreateCase.Location = new System.Drawing.Point(253, 191);
            this.btn_CreateCase.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btn_CreateCase.Name = "btn_CreateCase";
            this.btn_CreateCase.Size = new System.Drawing.Size(102, 23);
            this.btn_CreateCase.TabIndex = 0;
            this.btn_CreateCase.Text = "Create case";
            this.btn_CreateCase.UseVisualStyleBackColor = true;
            // 
            // btn_CreateStage
            // 
            this.btn_CreateStage.Location = new System.Drawing.Point(253, 253);
            this.btn_CreateStage.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btn_CreateStage.Name = "btn_CreateStage";
            this.btn_CreateStage.Size = new System.Drawing.Size(102, 23);
            this.btn_CreateStage.TabIndex = 1;
            this.btn_CreateStage.Text = "Create stage";
            this.btn_CreateStage.UseVisualStyleBackColor = true;
            // 
            // cb_NewStageType
            // 
            this.cb_NewStageType.FormattingEnabled = true;
            this.cb_NewStageType.Location = new System.Drawing.Point(79, 285);
            this.cb_NewStageType.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.cb_NewStageType.Name = "cb_NewStageType";
            this.cb_NewStageType.Size = new System.Drawing.Size(168, 25);
            this.cb_NewStageType.TabIndex = 6;
            // 
            // lb_Case
            // 
            this.lb_Case.AutoSize = true;
            this.lb_Case.Location = new System.Drawing.Point(14, 9);
            this.lb_Case.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lb_Case.Name = "lb_Case";
            this.lb_Case.Size = new System.Drawing.Size(44, 17);
            this.lb_Case.TabIndex = 9;
            this.lb_Case.Text = "Case:";
            // 
            // lb_Stage
            // 
            this.lb_Stage.AutoSize = true;
            this.lb_Stage.Location = new System.Drawing.Point(14, 40);
            this.lb_Stage.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lb_Stage.Name = "lb_Stage";
            this.lb_Stage.Size = new System.Drawing.Size(49, 17);
            this.lb_Stage.TabIndex = 10;
            this.lb_Stage.Text = "Stage:";
            // 
            // cb_Case
            // 
            this.cb_Case.FormattingEnabled = true;
            this.cb_Case.Location = new System.Drawing.Point(79, 6);
            this.cb_Case.Name = "cb_Case";
            this.cb_Case.Size = new System.Drawing.Size(168, 25);
            this.cb_Case.TabIndex = 11;
            // 
            // tb_NewCaseID
            // 
            this.tb_NewCaseID.Location = new System.Drawing.Point(79, 191);
            this.tb_NewCaseID.Name = "tb_NewCaseID";
            this.tb_NewCaseID.Size = new System.Drawing.Size(168, 23);
            this.tb_NewCaseID.TabIndex = 13;
            // 
            // tb_NewStageID
            // 
            this.tb_NewStageID.Location = new System.Drawing.Point(79, 253);
            this.tb_NewStageID.Name = "tb_NewStageID";
            this.tb_NewStageID.Size = new System.Drawing.Size(168, 23);
            this.tb_NewStageID.TabIndex = 14;
            // 
            // lb_NewCaseID
            // 
            this.lb_NewCaseID.AutoSize = true;
            this.lb_NewCaseID.Location = new System.Drawing.Point(12, 194);
            this.lb_NewCaseID.Name = "lb_NewCaseID";
            this.lb_NewCaseID.Size = new System.Drawing.Size(25, 17);
            this.lb_NewCaseID.TabIndex = 15;
            this.lb_NewCaseID.Text = "ID:";
            // 
            // lb_NewStageID
            // 
            this.lb_NewStageID.AutoSize = true;
            this.lb_NewStageID.Location = new System.Drawing.Point(12, 256);
            this.lb_NewStageID.Name = "lb_NewStageID";
            this.lb_NewStageID.Size = new System.Drawing.Size(25, 17);
            this.lb_NewStageID.TabIndex = 16;
            this.lb_NewStageID.Text = "ID:";
            // 
            // lb_NewStageType
            // 
            this.lb_NewStageType.AutoSize = true;
            this.lb_NewStageType.Location = new System.Drawing.Point(12, 288);
            this.lb_NewStageType.Name = "lb_NewStageType";
            this.lb_NewStageType.Size = new System.Drawing.Size(44, 17);
            this.lb_NewStageType.TabIndex = 17;
            this.lb_NewStageType.Text = "Type:";
            // 
            // btn_EditCase
            // 
            this.btn_EditCase.Location = new System.Drawing.Point(253, 6);
            this.btn_EditCase.Name = "btn_EditCase";
            this.btn_EditCase.Size = new System.Drawing.Size(102, 25);
            this.btn_EditCase.TabIndex = 18;
            this.btn_EditCase.Text = "Edit case";
            this.btn_EditCase.UseVisualStyleBackColor = true;
            // 
            // btn_EditStage
            // 
            this.btn_EditStage.Location = new System.Drawing.Point(253, 37);
            this.btn_EditStage.Name = "btn_EditStage";
            this.btn_EditStage.Size = new System.Drawing.Size(102, 25);
            this.btn_EditStage.TabIndex = 19;
            this.btn_EditStage.Text = "Edit stage";
            this.btn_EditStage.UseVisualStyleBackColor = true;
            // 
            // lb_NewCase
            // 
            this.lb_NewCase.AutoSize = true;
            this.lb_NewCase.Location = new System.Drawing.Point(12, 166);
            this.lb_NewCase.Name = "lb_NewCase";
            this.lb_NewCase.Size = new System.Drawing.Size(73, 17);
            this.lb_NewCase.TabIndex = 20;
            this.lb_NewCase.Text = "New case:";
            // 
            // lb_NewStage
            // 
            this.lb_NewStage.AutoSize = true;
            this.lb_NewStage.Location = new System.Drawing.Point(12, 233);
            this.lb_NewStage.Name = "lb_NewStage";
            this.lb_NewStage.Size = new System.Drawing.Size(78, 17);
            this.lb_NewStage.TabIndex = 21;
            this.lb_NewStage.Text = "New stage:";
            // 
            // lbx_Stages
            // 
            this.lbx_Stages.FormattingEnabled = true;
            this.lbx_Stages.ItemHeight = 17;
            this.lbx_Stages.Location = new System.Drawing.Point(79, 40);
            this.lbx_Stages.Name = "lbx_Stages";
            this.lbx_Stages.Size = new System.Drawing.Size(168, 106);
            this.lbx_Stages.TabIndex = 22;
            // 
            // btn_StageUp
            // 
            this.btn_StageUp.Location = new System.Drawing.Point(253, 66);
            this.btn_StageUp.Name = "btn_StageUp";
            this.btn_StageUp.Size = new System.Drawing.Size(102, 25);
            this.btn_StageUp.TabIndex = 23;
            this.btn_StageUp.Text = "Up";
            this.btn_StageUp.UseVisualStyleBackColor = true;
            // 
            // btn_StageDown
            // 
            this.btn_StageDown.Location = new System.Drawing.Point(253, 97);
            this.btn_StageDown.Name = "btn_StageDown";
            this.btn_StageDown.Size = new System.Drawing.Size(102, 25);
            this.btn_StageDown.TabIndex = 24;
            this.btn_StageDown.Text = "Down";
            this.btn_StageDown.UseVisualStyleBackColor = true;
            // 
            // SDK_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 413);
            this.Controls.Add(this.btn_StageDown);
            this.Controls.Add(this.btn_StageUp);
            this.Controls.Add(this.lbx_Stages);
            this.Controls.Add(this.lb_NewStage);
            this.Controls.Add(this.lb_NewCase);
            this.Controls.Add(this.btn_EditStage);
            this.Controls.Add(this.btn_EditCase);
            this.Controls.Add(this.lb_NewStageType);
            this.Controls.Add(this.lb_NewStageID);
            this.Controls.Add(this.lb_NewCaseID);
            this.Controls.Add(this.tb_NewStageID);
            this.Controls.Add(this.tb_NewCaseID);
            this.Controls.Add(this.cb_Case);
            this.Controls.Add(this.lb_Stage);
            this.Controls.Add(this.lb_Case);
            this.Controls.Add(this.cb_NewStageType);
            this.Controls.Add(this.btn_CreateStage);
            this.Controls.Add(this.btn_CreateCase);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SDK_Main";
            this.Text = "SDK_Main";
            this.Load += new System.EventHandler(this.SDK_Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_CreateCase;
        private System.Windows.Forms.Button btn_CreateStage;
        private System.Windows.Forms.ComboBox cb_NewStageType;
        private System.Windows.Forms.Label lb_Case;
        private System.Windows.Forms.Label lb_Stage;
        private System.Windows.Forms.ComboBox cb_Case;
        private System.Windows.Forms.TextBox tb_NewCaseID;
        private System.Windows.Forms.TextBox tb_NewStageID;
        private System.Windows.Forms.Label lb_NewCaseID;
        private System.Windows.Forms.Label lb_NewStageID;
        private System.Windows.Forms.Label lb_NewStageType;
        private System.Windows.Forms.Button btn_EditCase;
        private System.Windows.Forms.Button btn_EditStage;
        private System.Windows.Forms.Label lb_NewCase;
        private System.Windows.Forms.Label lb_NewStage;
        private System.Windows.Forms.ListBox lbx_Stages;
        private System.Windows.Forms.Button btn_StageUp;
        private System.Windows.Forms.Button btn_StageDown;
    }
}