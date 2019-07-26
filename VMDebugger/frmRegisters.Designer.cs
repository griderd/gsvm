namespace VMDebugger
{
    partial class frmRegisters
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
            this.components = new System.ComponentModel.Container();
            this.lstRegisters = new System.Windows.Forms.ListView();
            this.colRegister = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnStep = new System.Windows.Forms.Button();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.lstInfo = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lstRegisters
            // 
            this.lstRegisters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colRegister,
            this.colValue});
            this.lstRegisters.Location = new System.Drawing.Point(12, 12);
            this.lstRegisters.Name = "lstRegisters";
            this.lstRegisters.Size = new System.Drawing.Size(258, 494);
            this.lstRegisters.TabIndex = 0;
            this.lstRegisters.UseCompatibleStateImageBehavior = false;
            this.lstRegisters.View = System.Windows.Forms.View.Details;
            // 
            // colRegister
            // 
            this.colRegister.Text = "Register";
            this.colRegister.Width = 120;
            // 
            // colValue
            // 
            this.colValue.Text = "Value";
            this.colValue.Width = 100;
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(413, 514);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(120, 33);
            this.btnStep.TabIndex = 1;
            this.btnStep.Text = "Step (F10)";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Enabled = true;
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // lstInfo
            // 
            this.lstInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lstInfo.Location = new System.Drawing.Point(276, 12);
            this.lstInfo.Name = "lstInfo";
            this.lstInfo.Size = new System.Drawing.Size(258, 494);
            this.lstInfo.TabIndex = 2;
            this.lstInfo.UseCompatibleStateImageBehavior = false;
            this.lstInfo.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Field";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 100;
            // 
            // frmRegisters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 559);
            this.Controls.Add(this.lstInfo);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.lstRegisters);
            this.Name = "frmRegisters";
            this.Text = "Registers";
            this.Load += new System.EventHandler(this.frmRegisters_Load);
            this.Shown += new System.EventHandler(this.frmRegisters_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmRegisters_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frmRegisters_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lstRegisters;
        private System.Windows.Forms.ColumnHeader colRegister;
        private System.Windows.Forms.ColumnHeader colValue;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.Timer tmrUpdate;
        private System.Windows.Forms.ListView lstInfo;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}

