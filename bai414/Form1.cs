
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HospitalMDI
{
    public partial class frmHospitalMain : Form
    {
        private MenuStrip menu;
        private StatusStrip status;
        private ToolStripStatusLabel lblStatus;
        private ToolStripProgressBar progress;

        public frmHospitalMain()
        {
            InitializeComponent();
            TaoMenu();
        }

        private void TaoMenu()
        {
            menu = new MenuStrip();
            menu.Dock = DockStyle.Top;
            menu.Visible = true;
            menu.BackColor = Color.LightSteelBlue;

            // MENU HE THONG
            ToolStripMenuItem heThong = new ToolStripMenuItem("Hệ thống");
            ToolStripMenuItem dangXuat = new ToolStripMenuItem("Đăng xuất");
            ToolStripMenuItem napLai = new ToolStripMenuItem("Nạp lại dữ liệu");
            ToolStripMenuItem thoat = new ToolStripMenuItem("Thoát");

            dangXuat.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn muốn đăng xuất?",
                    "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DongTatCa();
                    lblStatus.Text = "Đã đăng xuất";
                }
            };

            napLai.Click += (s, e) =>
            {
                progress.Value = 50;
                Application.DoEvents();
                progress.Value = 100;
                lblStatus.Text = "Đã nạp lại dữ liệu";
                MessageBox.Show("Nạp lại dữ liệu hoàn tất!");
                progress.Value = 0;
            };

            thoat.Click += (s, e) =>
            {
                Close();
            };

            heThong.DropDownItems.Add(dangXuat);
            heThong.DropDownItems.Add(napLai);
            heThong.DropDownItems.Add(thoat);

            // MENU QUAN LY
            ToolStripMenuItem quanLy = new ToolStripMenuItem("Quản lý");

            ToolStripMenuItem benhNhan =
                new ToolStripMenuItem("Hồ sơ bệnh nhân");

            ToolStripMenuItem bacSi =
                new ToolStripMenuItem("Danh mục bác sĩ");

            ToolStripMenuItem lichTruc =
                new ToolStripMenuItem("Lịch trực bác sĩ");

            ToolStripMenuItem khoaPhong =
                new ToolStripMenuItem("Danh mục khoa/phòng");

            benhNhan.Click += (s, e) => MoForm<frmPatientProfiles>();
            bacSi.Click += (s, e) => MoForm<frmDoctorDirectory>();
            lichTruc.Click += (s, e) => MoForm<frmDoctorSchedule>();
            khoaPhong.Click += (s, e) => MoForm<frmDepartmentList>();

            quanLy.DropDownItems.Add(benhNhan);
            quanLy.DropDownItems.Add(bacSi);
            quanLy.DropDownItems.Add(lichTruc);
            quanLy.DropDownItems.Add(khoaPhong);

            // MENU CUA SO
            ToolStripMenuItem cuaSo = new ToolStripMenuItem("Cửa sổ");

            ToolStripMenuItem tang = new ToolStripMenuItem("Xếp tầng");
            ToolStripMenuItem ngang = new ToolStripMenuItem("Xếp ngang");
            ToolStripMenuItem doc = new ToolStripMenuItem("Xếp dọc");
            ToolStripMenuItem dongAll = new ToolStripMenuItem("Đóng tất cả");

            tang.Click += (s, e) => LayoutMdi(MdiLayout.Cascade);
            ngang.Click += (s, e) => LayoutMdi(MdiLayout.TileHorizontal);
            doc.Click += (s, e) => LayoutMdi(MdiLayout.TileVertical);
            dongAll.Click += (s, e) => DongTatCa();

            cuaSo.DropDownItems.Add(tang);
            cuaSo.DropDownItems.Add(ngang);
            cuaSo.DropDownItems.Add(doc);
            cuaSo.DropDownItems.Add(new ToolStripSeparator());
            cuaSo.DropDownItems.Add(dongAll);

            // THEM MENU VAO FORM
            menu.Items.Add(heThong);
            menu.Items.Add(quanLy);
            menu.Items.Add(cuaSo);

            this.MainMenuStrip = menu;
            this.Controls.Add(menu);
            menu.BringToFront();

            // THANH TRANG THAI
            status = new StatusStrip();

            lblStatus = new ToolStripStatusLabel(
                "Bác sĩ trực: Nguyễn Văn A");

            lblStatus.Spring = true;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;

            progress = new ToolStripProgressBar();
            progress.Minimum = 0;
            progress.Maximum = 100;
            progress.Value = 0;

            status.Items.Add(lblStatus);
            status.Items.Add(progress);
            this.Controls.Add(status);
        }

        private void MoForm<T>() where T : Form, new()
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType() == typeof(T))
                {
                    if (f.WindowState == FormWindowState.Minimized)
                        f.WindowState = FormWindowState.Normal;

                    f.Activate();
                    return;
                }
            }

            T form = new T();
            form.MdiParent = this;
            form.Show();
        }

        private void DongTatCa()
        {
            foreach (Form f in this.MdiChildren)
                f.Close();
        }
    }

    public class frmPatientProfiles : Form
    {
        public frmPatientProfiles()
        {
            Text = "Hồ sơ bệnh nhân";
            Size = new Size(800, 450);

            Label title = new Label();
            title.Text = "DANH SÁCH HỒ SƠ BỆNH NHÂN";
            title.Dock = DockStyle.Top;
            title.Height = 50;
            title.Font = new Font("Arial", 16, FontStyle.Bold);
            title.TextAlign = ContentAlignment.MiddleCenter;

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnCount = 4;

            dgv.Columns[0].HeaderText = "Mã bệnh nhân";
            dgv.Columns[1].HeaderText = "Họ tên";
            dgv.Columns[2].HeaderText = "Ngày sinh";
            dgv.Columns[3].HeaderText = "Khoa";

            dgv.Rows.Add("BN001", "Nguyễn Văn Bình", "12/05/2000", "Nội khoa");
            dgv.Rows.Add("BN002", "Trần Thị Lan", "08/10/1998", "Ngoại khoa");

            Controls.Add(dgv);
            Controls.Add(title);
        }
    }

    public class frmDoctorSchedule : Form
    {
        public frmDoctorSchedule()
        {
            Text = "Lịch trực bác sĩ";
            Size = new Size(750, 400);

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnCount = 4;

            dgv.Columns[0].HeaderText = "Mã bác sĩ";
            dgv.Columns[1].HeaderText = "Họ tên";
            dgv.Columns[2].HeaderText = "Ngày trực";
            dgv.Columns[3].HeaderText = "Ca trực";

            dgv.Rows.Add("BS001", "Nguyễn Văn A", "18/10/2026", "Ca sáng");
            dgv.Rows.Add("BS002", "Trần Văn B", "18/10/2026", "Ca chiều");

            Controls.Add(dgv);
        }
    }

    public class frmDoctorDirectory : Form
    {
        public frmDoctorDirectory()
        {
            Text = "Danh mục bác sĩ";
            Size = new Size(700, 400);

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnCount = 3;

            dgv.Columns[0].HeaderText = "Mã bác sĩ";
            dgv.Columns[1].HeaderText = "Họ tên";
            dgv.Columns[2].HeaderText = "Chuyên khoa";

            dgv.Rows.Add("BS001", "Nguyễn Văn A", "Nội khoa");
            dgv.Rows.Add("BS002", "Trần Văn B", "Ngoại khoa");

            Controls.Add(dgv);
        }
    }

    public class frmDepartmentList : Form
    {
        public frmDepartmentList()
        {
            Text = "Danh mục khoa/phòng";
            Size = new Size(700, 400);

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnCount = 3;

            dgv.Columns[0].HeaderText = "Mã khoa";
            dgv.Columns[1].HeaderText = "Tên khoa/phòng";
            dgv.Columns[2].HeaderText = "Vị trí";

            dgv.Rows.Add("K001", "Khoa Nội", "Tầng 1");
            dgv.Rows.Add("K002", "Khoa Ngoại", "Tầng 2");

            Controls.Add(dgv);
        }
    }
}
