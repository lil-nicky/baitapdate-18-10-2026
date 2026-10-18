using System.Drawing;
using System.Windows.Forms;

namespace RevenueAnalytics
{
    partial class frmRevenueAnalytics
    {
        private System.ComponentModel.IContainer components = null;

        private TableLayoutPanel mainLayout;
        private TableLayoutPanel criteriaLayout;
        private TableLayoutPanel statusLayout;
        private Panel pnlCriteria;
        private FlowLayoutPanel pnlButtons;

        private Label lblTitle;
        private Label lblFromDate;
        private Label lblToDate;
        private Label lblBranch;
        private Label lblProcessingStatus;

        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cboBranch;

        private Button btnStatistics;
        private Button btnCancel;

        private ProgressBar pgbLoading;
        private DataGridView dgvRevenueData;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            mainLayout = new TableLayoutPanel();
            lblTitle = new Label();
            pnlCriteria = new Panel();
            criteriaLayout = new TableLayoutPanel();
            lblFromDate = new Label();
            dtpFromDate = new DateTimePicker();
            lblToDate = new Label();
            dtpToDate = new DateTimePicker();
            lblBranch = new Label();
            cboBranch = new ComboBox();
            pnlButtons = new FlowLayoutPanel();
            btnStatistics = new Button();
            btnCancel = new Button();
            statusLayout = new TableLayoutPanel();
            pgbLoading = new ProgressBar();
            lblProcessingStatus = new Label();
            dgvRevenueData = new DataGridView();
            mainLayout.SuspendLayout();
            pnlCriteria.SuspendLayout();
            criteriaLayout.SuspendLayout();
            pnlButtons.SuspendLayout();
            statusLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRevenueData).BeginInit();
            SuspendLayout();
            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 1;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(lblTitle, 0, 0);
            mainLayout.Controls.Add(pnlCriteria, 0, 1);
            mainLayout.Controls.Add(statusLayout, 0, 2);
            mainLayout.Controls.Add(dgvRevenueData, 0, 3);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(0, 0);
            mainLayout.Margin = new Padding(0);
            mainLayout.Name = "mainLayout";
            mainLayout.RowCount = 4;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 155F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.Size = new Size(1100, 700);
            mainLayout.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.DarkBlue;
            lblTitle.Location = new Point(0, 0);
            lblTitle.Margin = new Padding(0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(1100, 65);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "THỐNG KÊ DOANH THU";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlCriteria
            // 
            pnlCriteria.BackColor = Color.White;
            pnlCriteria.Controls.Add(criteriaLayout);
            pnlCriteria.Controls.Add(pnlButtons);
            pnlCriteria.Dock = DockStyle.Fill;
            pnlCriteria.Location = new Point(0, 65);
            pnlCriteria.Margin = new Padding(0);
            pnlCriteria.Name = "pnlCriteria";
            pnlCriteria.Padding = new Padding(12);
            pnlCriteria.Size = new Size(1100, 155);
            pnlCriteria.TabIndex = 1;
            // 
            // criteriaLayout
            // 
            criteriaLayout.ColumnCount = 6;
            criteriaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            criteriaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            criteriaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F));
            criteriaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            criteriaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F));
            criteriaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
            criteriaLayout.Controls.Add(lblFromDate, 0, 0);
            criteriaLayout.Controls.Add(dtpFromDate, 1, 0);
            criteriaLayout.Controls.Add(lblToDate, 2, 0);
            criteriaLayout.Controls.Add(dtpToDate, 3, 0);
            criteriaLayout.Controls.Add(lblBranch, 4, 0);
            criteriaLayout.Controls.Add(cboBranch, 5, 0);
            criteriaLayout.Dock = DockStyle.Top;
            criteriaLayout.Location = new Point(12, 12);
            criteriaLayout.Margin = new Padding(0);
            criteriaLayout.Name = "criteriaLayout";
            criteriaLayout.RowCount = 1;
            criteriaLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            criteriaLayout.Size = new Size(1076, 48);
            criteriaLayout.TabIndex = 0;
            // 
            // lblFromDate
            // 
            lblFromDate.Dock = DockStyle.Fill;
            lblFromDate.Location = new Point(3, 3);
            lblFromDate.Margin = new Padding(3);
            lblFromDate.Name = "lblFromDate";
            lblFromDate.Size = new Size(101, 42);
            lblFromDate.TabIndex = 0;
            lblFromDate.Text = "Từ ngày:";
            lblFromDate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dtpFromDate
            // 
            dtpFromDate.Dock = DockStyle.Fill;
            dtpFromDate.Format = DateTimePickerFormat.Short;
            dtpFromDate.Location = new Point(110, 8);
            dtpFromDate.Margin = new Padding(3, 8, 8, 8);
            dtpFromDate.Name = "dtpFromDate";
            dtpFromDate.Size = new Size(204, 27);
            dtpFromDate.TabIndex = 1;
            dtpFromDate.Value = new DateTime(2026, 9, 18, 0, 0, 0, 0);
            // 
            // lblToDate
            // 
            lblToDate.Dock = DockStyle.Fill;
            lblToDate.Location = new Point(325, 3);
            lblToDate.Margin = new Padding(3);
            lblToDate.Name = "lblToDate";
            lblToDate.Size = new Size(112, 42);
            lblToDate.TabIndex = 2;
            lblToDate.Text = "Đến ngày:";
            lblToDate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dtpToDate
            // 
            dtpToDate.Dock = DockStyle.Fill;
            dtpToDate.Format = DateTimePickerFormat.Short;
            dtpToDate.Location = new Point(443, 8);
            dtpToDate.Margin = new Padding(3, 8, 8, 8);
            dtpToDate.Name = "dtpToDate";
            dtpToDate.Size = new Size(204, 27);
            dtpToDate.TabIndex = 3;
            dtpToDate.Value = new DateTime(2026, 10, 18, 0, 0, 0, 0);
            // 
            // lblBranch
            // 
            lblBranch.Dock = DockStyle.Fill;
            lblBranch.Location = new Point(658, 3);
            lblBranch.Margin = new Padding(3);
            lblBranch.Name = "lblBranch";
            lblBranch.Size = new Size(112, 42);
            lblBranch.TabIndex = 4;
            lblBranch.Text = "Chi nhánh:";
            lblBranch.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cboBranch
            // 
            cboBranch.Dock = DockStyle.Fill;
            cboBranch.DropDownStyle = ComboBoxStyle.DropDownList;
            cboBranch.Items.AddRange(new object[] { "Tất cả chi nhánh", "Chi nhánh Hà Nội", "Chi nhánh TP.HCM", "Chi nhánh Đà Nẵng" });
            cboBranch.Location = new Point(776, 8);
            cboBranch.Margin = new Padding(3, 8, 3, 8);
            cboBranch.Name = "cboBranch";
            cboBranch.Size = new Size(297, 28);
            cboBranch.TabIndex = 5;
            // 
            // pnlButtons
            // 
            pnlButtons.Controls.Add(btnStatistics);
            pnlButtons.Controls.Add(btnCancel);
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Location = new Point(12, 91);
            pnlButtons.Margin = new Padding(0);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Padding = new Padding(3, 7, 3, 3);
            pnlButtons.Size = new Size(1076, 52);
            pnlButtons.TabIndex = 1;
            pnlButtons.WrapContents = false;
            // 
            // btnStatistics
            // 
            btnStatistics.BackColor = Color.RoyalBlue;
            btnStatistics.FlatStyle = FlatStyle.Flat;
            btnStatistics.ForeColor = Color.White;
            btnStatistics.Location = new Point(6, 10);
            btnStatistics.Name = "btnStatistics";
            btnStatistics.Size = new Size(190, 35);
            btnStatistics.TabIndex = 0;
            btnStatistics.Text = "Thống Kê Doanh Thu";
            btnStatistics.UseVisualStyleBackColor = false;
            btnStatistics.Click += btnStatistics_Click;
            // 
            // btnCancel
            // 
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(211, 10);
            btnCancel.Margin = new Padding(12, 3, 3, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(145, 35);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Hủy Thao Tác";
            btnCancel.Click += btnCancel_Click;
            // 
            // statusLayout
            // 
            statusLayout.ColumnCount = 2;
            statusLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            statusLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            statusLayout.Controls.Add(pgbLoading, 0, 0);
            statusLayout.Controls.Add(lblProcessingStatus, 1, 0);
            statusLayout.Dock = DockStyle.Fill;
            statusLayout.Location = new Point(0, 220);
            statusLayout.Margin = new Padding(0);
            statusLayout.Name = "statusLayout";
            statusLayout.Padding = new Padding(10, 5, 10, 5);
            statusLayout.RowCount = 1;
            statusLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            statusLayout.Size = new Size(1100, 48);
            statusLayout.TabIndex = 2;
            // 
            // pgbLoading
            // 
            pgbLoading.Dock = DockStyle.Fill;
            pgbLoading.Location = new Point(13, 10);
            pgbLoading.Margin = new Padding(3, 5, 12, 5);
            pgbLoading.Name = "pgbLoading";
            pgbLoading.Size = new Size(633, 28);
            pgbLoading.TabIndex = 0;
            // 
            // lblProcessingStatus
            // 
            lblProcessingStatus.AutoEllipsis = true;
            lblProcessingStatus.Dock = DockStyle.Fill;
            lblProcessingStatus.Location = new Point(661, 8);
            lblProcessingStatus.Margin = new Padding(3);
            lblProcessingStatus.Name = "lblProcessingStatus";
            lblProcessingStatus.Size = new Size(426, 32);
            lblProcessingStatus.TabIndex = 1;
            lblProcessingStatus.Text = "Sẵn sàng thống kê";
            lblProcessingStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dgvRevenueData
            // 
            dgvRevenueData.AllowUserToAddRows = false;
            dgvRevenueData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRevenueData.BackgroundColor = Color.White;
            dgvRevenueData.ColumnHeadersHeight = 29;
            dgvRevenueData.Dock = DockStyle.Fill;
            dgvRevenueData.Location = new Point(8, 276);
            dgvRevenueData.Margin = new Padding(8);
            dgvRevenueData.MultiSelect = false;
            dgvRevenueData.Name = "dgvRevenueData";
            dgvRevenueData.ReadOnly = true;
            dgvRevenueData.RowHeadersVisible = false;
            dgvRevenueData.RowHeadersWidth = 51;
            dgvRevenueData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRevenueData.Size = new Size(1084, 416);
            dgvRevenueData.TabIndex = 3;
            // 
            // frmRevenueAnalytics
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1100, 700);
            Controls.Add(mainLayout);
            MinimumSize = new Size(850, 580);
            Name = "frmRevenueAnalytics";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bảng điều khiển thống kê doanh thu";
            mainLayout.ResumeLayout(false);
            pnlCriteria.ResumeLayout(false);
            criteriaLayout.ResumeLayout(false);
            pnlButtons.ResumeLayout(false);
            statusLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRevenueData).EndInit();
            ResumeLayout(false);
        }
    }
}