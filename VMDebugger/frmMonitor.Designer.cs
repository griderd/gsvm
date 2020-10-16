namespace VMDebugger
{
    partial class frmMonitor
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
            this.tmrRefresh = new System.Windows.Forms.Timer(this.components);
            this.monitor = new GSVM.Peripherals.Monitors.MonitorControl();
            this.SuspendLayout();
            // 
            // tmrRefresh
            // 
            this.tmrRefresh.Enabled = true;
            this.tmrRefresh.Interval = 16;
            this.tmrRefresh.Tick += new System.EventHandler(this.tmrRefresh_Tick);
            // 
            // monitor
            // 
            this.monitor.Adapter = null;
            this.monitor.BackColor = System.Drawing.Color.Black;
            this.monitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monitor.Location = new System.Drawing.Point(0, 0);
            this.monitor.Name = "monitor";
            this.monitor.Size = new System.Drawing.Size(666, 461);
            this.monitor.TabIndex = 0;
            this.monitor.VSync = false;
            // 
            // frmMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 461);
            this.Controls.Add(this.monitor);
            this.Name = "frmMonitor";
            this.Text = "Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMonitor_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmrRefresh;
        private GSVM.Peripherals.Monitors.MonitorControl monitor;
    }
}