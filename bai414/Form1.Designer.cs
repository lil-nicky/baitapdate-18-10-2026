namespace HospitalMDI
{
    partial class frmHospitalMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Name = "frmHospitalMain";
            this.Text = "QUẢN LÝ BỆNH VIỆN";
            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;
            this.IsMdiContainer = true;
            this.WindowState =
                System.Windows.Forms.FormWindowState.Maximized;

            this.ResumeLayout(false);
        }
    }
}