
namespace BaiTap5
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Label lblBTCPrice;
        private System.Windows.Forms.DataGridView dgvHistoryLog;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblLastUpdate;
        private System.Windows.Forms.ToolStripStatusLabel lblSyncCount;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.lblBTCPrice = new System.Windows.Forms.Label();
            this.dgvHistoryLog = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblLastUpdate = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSyncCount = new System.Windows.Forms.ToolStripStatusLabel();

            ((System.ComponentModel.ISupportInitialize)
                (this.dgvHistoryLog)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Giám sát tỷ giá BTC/USD";
            this.Name = "Form1";
            this.MinimumSize = new System.Drawing.Size(700, 450);

            // btnStart
            this.btnStart.Location = new System.Drawing.Point(20, 20);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(180, 40);
            this.btnStart.Text = "Bắt đầu Giám sát";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click +=
                new System.EventHandler(this.btnStart_Click);

            // btnPause
            this.btnPause.Location = new System.Drawing.Point(220, 20);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(130, 40);
            this.btnPause.Text = "Tạm dừng";
            this.btnPause.Enabled = false;
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click +=
                new System.EventHandler(this.btnPause_Click);

            // lblBTCPrice
            this.lblBTCPrice.Location = new System.Drawing.Point(20, 80);
            this.lblBTCPrice.Name = "lblBTCPrice";
            this.lblBTCPrice.Size = new System.Drawing.Size(840, 60);
            this.lblBTCPrice.Font = new System.Drawing.Font(
                "Arial", 22F, System.Drawing.FontStyle.Bold);
            this.lblBTCPrice.ForeColor = System.Drawing.Color.Green;
            this.lblBTCPrice.Text = "BTC/USD: $67,000.00";
            this.lblBTCPrice.TextAlign =
                System.Drawing.ContentAlignment.MiddleLeft;

            // dgvHistoryLog
            this.dgvHistoryLog.Location = new System.Drawing.Point(20, 160);
            this.dgvHistoryLog.Name = "dgvHistoryLog";
            this.dgvHistoryLog.Size = new System.Drawing.Size(840, 380);
            this.dgvHistoryLog.Anchor =
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right;
            this.dgvHistoryLog.ReadOnly = true;
            this.dgvHistoryLog.AllowUserToAddRows = false;
            this.dgvHistoryLog.AllowUserToDeleteRows = false;
            this.dgvHistoryLog.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistoryLog.RowHeadersVisible = false;
            this.dgvHistoryLog.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            this.dgvHistoryLog.Columns.Add("colTime", "Thời gian");
            this.dgvHistoryLog.Columns.Add("colPrice", "Giá BTC/USD");
            this.dgvHistoryLog.Columns.Add("colChange", "Biến động");

            // StatusStrip
            this.statusStrip1.Items.AddRange(
                new System.Windows.Forms.ToolStripItem[] {
                    this.lblLastUpdate,
                    this.lblSyncCount
                });

            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Dock =
                System.Windows.Forms.DockStyle.Bottom;

            // lblLastUpdate
            this.lblLastUpdate.Name = "lblLastUpdate";
            this.lblLastUpdate.Text = "Cập nhật cuối: --:--:--";

            // lblSyncCount
            this.lblSyncCount.Name = "lblSyncCount";
            this.lblSyncCount.Text = "Đồng bộ thành công: 0";
            this.lblSyncCount.Margin =
                new System.Windows.Forms.Padding(30, 3, 0, 2);

            // Add controls
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.lblBTCPrice);
            this.Controls.Add(this.dgvHistoryLog);
            this.Controls.Add(this.statusStrip1);

            ((System.ComponentModel.ISupportInitialize)
                (this.dgvHistoryLog)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
