
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;

namespace bai412
{
    // ==========================================
    // MODEL HỘI VIÊN
    // ==========================================
    public class MemberModel : INotifyPropertyChanged
    {
        private string memberId = "";
        private string fullName = "";
        private string phone = "";
        private string email = "";
        private string packageName = "Theo tháng";
        private DateTime startDate = DateTime.Today;
        private DateTime expiryDate = DateTime.Today.AddMonths(1);
        private string feeText = "";

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(
                    this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SetField<T>(
            ref T field, T value, string propertyName)
        {
            if (object.Equals(field, value))
                return;

            field = value;
            Notify(propertyName);
        }

        public string MemberId
        {
            get { return memberId; }
            set { SetField(ref memberId, value, "MemberId"); }
        }

        public string FullName
        {
            get { return fullName; }
            set { SetField(ref fullName, value, "FullName"); }
        }

        public string Phone
        {
            get { return phone; }
            set { SetField(ref phone, value, "Phone"); }
        }

        public string Email
        {
            get { return email; }
            set { SetField(ref email, value, "Email"); }
        }

        public string Package
        {
            get { return packageName; }
            set { SetField(ref packageName, value, "Package"); }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { SetField(ref startDate, value, "StartDate"); }
        }

        public DateTime ExpiryDate
        {
            get { return expiryDate; }
            set { SetField(ref expiryDate, value, "ExpiryDate"); }
        }

        // Lưu nội dung nhập để kiểm tra trước khi lưu.
        public string FeeText
        {
            get { return feeText; }
            set
            {
                if (feeText == value)
                    return;

                feeText = value;
                Notify("FeeText");
                Notify("Fee");
            }
        }

        // Giá trị số hiển thị bằng định dạng N0 trên DataGridView.
        public decimal Fee
        {
            get
            {
                decimal result;

                if (decimal.TryParse(
                    feeText,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out result))
                {
                    return result;
                }

                return 0M;
            }
        }
    }

    // ==========================================
    // FORM QUẢN LÝ HỘI VIÊN
    // ==========================================
    public partial class Form1 : Form
    {
        private readonly BindingList<MemberModel> members =
            new BindingList<MemberModel>();

        private readonly BindingSource membersSource =
            new BindingSource();

        private readonly BindingList<MemberModel> editBuffer =
            new BindingList<MemberModel>();

        private readonly BindingSource editSource =
            new BindingSource();

        private readonly ErrorProvider errorProvider =
            new ErrorProvider();

        private MemberModel selectedMember;
        private bool isAdding = true;
        private bool loadingDraft = false;

        private TextBox txtMemberId;
        private TextBox txtFullName;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtFee;

        private ComboBox cboPackage;

        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpExpiryDate;

        private DataGridView dgvMembers;

        private ToolStripStatusLabel lblMemberCount;
        private ToolStripStatusLabel lblTotalFee;

        public Form1()
        {
            InitializeComponent();
            CreateInterface();
            ConfigureBinding();
            LoadSampleData();
            StartNewMember();
            UpdateStatusBar();
        }

        // ==========================================
        // 1. TẠO GIAO DIỆN BẰNG CODE
        // ==========================================
        private void CreateInterface()
        {
            errorProvider.ContainerControl = this;

            MenuStrip menuStrip = new MenuStrip();

            ToolStripMenuItem menuSystem =
                new ToolStripMenuItem("Hệ thống");

            ToolStripMenuItem menuExit =
                new ToolStripMenuItem("Thoát");

            menuExit.Click += delegate
            {
                this.Close();
            };

            menuSystem.DropDownItems.Add(menuExit);
            menuStrip.Items.Add(menuSystem);

            StatusStrip statusStrip = new StatusStrip();

            lblMemberCount = new ToolStripStatusLabel();
            lblTotalFee = new ToolStripStatusLabel();

            statusStrip.Items.Add(lblMemberCount);
            statusStrip.Items.Add(
                new ToolStripStatusLabel("|"));
            statusStrip.Items.Add(lblTotalFee);

            TableLayoutPanel mainLayout =
                new TableLayoutPanel();

            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Padding = new Padding(8);
            mainLayout.ColumnCount = 2;
            mainLayout.RowCount = 1;

            mainLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 38F));

            mainLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 62F));

            // Khu vực nhập thông tin
            GroupBox inputGroup = new GroupBox();
            inputGroup.Text = "Thông tin hội viên";
            inputGroup.Dock = DockStyle.Fill;

            TableLayoutPanel inputLayout =
                new TableLayoutPanel();

            inputLayout.Dock = DockStyle.Fill;
            inputLayout.Padding = new Padding(6);
            inputLayout.AutoScroll = true;
            inputLayout.ColumnCount = 2;
            inputLayout.RowCount = 0;

            inputLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 36F));

            inputLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 64F));

            txtMemberId = new TextBox();
            txtMemberId.ReadOnly = true;
            txtMemberId.BackColor = Color.LightGray;

            txtFullName = new TextBox();
            txtPhone = new TextBox();
            txtEmail = new TextBox();
            txtFee = new TextBox();

            cboPackage = new ComboBox();
            cboPackage.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboPackage.Items.AddRange(new object[]
            {
                "Theo tháng",
                "6 tháng",
                "1 năm"
            });

            cboPackage.SelectedIndex = 0;

            dtpStartDate = new DateTimePicker();
            dtpStartDate.Format = DateTimePickerFormat.Short;
            dtpStartDate.Value = DateTime.Today;

            dtpExpiryDate = new DateTimePicker();
            dtpExpiryDate.Format = DateTimePickerFormat.Short;
            dtpExpiryDate.Value = DateTime.Today.AddMonths(1);

            AddInputRow(inputLayout,
                "Mã hội viên:", txtMemberId, 38);

            AddInputRow(inputLayout,
                "Họ và tên:", txtFullName, 38);

            AddInputRow(inputLayout,
                "Số điện thoại:", txtPhone, 38);

            AddInputRow(inputLayout,
                "Email:", txtEmail, 38);

            AddInputRow(inputLayout,
                "Gói tập:", cboPackage, 38);

            AddInputRow(inputLayout,
                "Ngày đăng ký:", dtpStartDate, 38);

            AddInputRow(inputLayout,
                "Ngày hết hạn:", dtpExpiryDate, 38);

            AddInputRow(inputLayout,
                "Phí dịch vụ:", txtFee, 38);

            // Các nút chức năng
            FlowLayoutPanel buttonPanel =
                new FlowLayoutPanel();

            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.WrapContents = true;
            buttonPanel.AutoScroll = true;

            Button btnAdd = CreateButton("Thêm mới", 80);
            Button btnUpdate = CreateButton("Sửa thông tin", 105);
            Button btnDelete = CreateButton("Xóa hội viên", 100);
            Button btnClear = CreateButton("Làm mới", 80);

            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnUpdate);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnClear);

            int buttonRow = inputLayout.RowCount;
            inputLayout.RowCount++;

            inputLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 80));

            inputLayout.Controls.Add(buttonPanel, 0, buttonRow);
            inputLayout.SetColumnSpan(buttonPanel, 2);
            inputGroup.Controls.Add(inputLayout);

            // Khu vực danh sách hội viên
            GroupBox gridGroup = new GroupBox();
            gridGroup.Text = "Danh sách hội viên";
            gridGroup.Dock = DockStyle.Fill;

            dgvMembers = new DataGridView();

            dgvMembers.Dock = DockStyle.Fill;
            dgvMembers.AutoGenerateColumns = false;
            dgvMembers.ReadOnly = true;
            dgvMembers.AllowUserToAddRows = false;
            dgvMembers.AllowUserToDeleteRows = false;
            dgvMembers.MultiSelect = false;
            dgvMembers.RowHeadersVisible = false;

            dgvMembers.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvMembers.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvMembers.BackgroundColor = Color.White;

            // Khai báo cột thủ công.
            AddGridColumn("Mã HV", "MemberId", 65);
            AddGridColumn("Tên hội viên", "FullName", 115);
            AddGridColumn("SĐT", "Phone", 85);
            AddGridColumn("Gói tập", "Package", 75);

            AddGridColumn(
                "Ngày ĐK", "StartDate", 80, "dd/MM/yyyy");

            AddGridColumn(
                "Phí dịch vụ", "Fee", 90, "N0");

            gridGroup.Controls.Add(dgvMembers);

            mainLayout.Controls.Add(inputGroup, 0, 0);
            mainLayout.Controls.Add(gridGroup, 1, 0);

            this.Controls.Add(mainLayout);
            this.Controls.Add(statusStrip);
            this.Controls.Add(menuStrip);

            this.MainMenuStrip = menuStrip;

            // Đăng ký sự kiện
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            btnClear.Click += delegate
            {
                StartNewMember();
            };

            dgvMembers.CellClick += DgvMembers_CellClick;

            cboPackage.SelectedIndexChanged += delegate
            {
                UpdateExpiryDate();
            };

            dtpStartDate.ValueChanged += delegate
            {
                UpdateExpiryDate();
            };
        }

        private Button CreateButton(string text, int width)
        {
            Button button = new Button();
            button.Text = text;
            button.Width = width;
            button.Height = 32;
            button.Margin = new Padding(3, 5, 3, 3);
            return button;
        }

        private void AddInputRow(
            TableLayoutPanel layout,
            string labelText,
            Control input,
            int height)
        {
            int row = layout.RowCount;
            layout.RowCount++;

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, height));

            Label label = new Label();
            label.Text = labelText;
            label.Dock = DockStyle.Fill;
            label.TextAlign =
                System.Drawing.ContentAlignment.MiddleLeft;

            input.Dock = DockStyle.Fill;
            input.Margin = new Padding(3, 5, 3, 5);

            layout.Controls.Add(label, 0, row);
            layout.Controls.Add(input, 1, row);
        }

        private void AddGridColumn(
            string header,
            string propertyName,
            float weight,
            string format = null)
        {
            DataGridViewTextBoxColumn column =
                new DataGridViewTextBoxColumn();

            column.HeaderText = header;
            column.DataPropertyName = propertyName;
            column.FillWeight = weight;
            column.SortMode =
                DataGridViewColumnSortMode.Automatic;

            if (!string.IsNullOrEmpty(format))
            {
                column.DefaultCellStyle.Format = format;
            }

            dgvMembers.Columns.Add(column);
        }

        // ==========================================
        // 2. DATA BINDING
        // ==========================================
        private void ConfigureBinding()
        {
            // BindingSource cho DataGridView
            membersSource.DataSource = members;
            dgvMembers.DataSource = membersSource;

            // Buffer riêng để kiểm tra dữ liệu trước khi lưu.
            editBuffer.Add(new MemberModel());
            editSource.DataSource = editBuffer;

            txtMemberId.DataBindings.Add(
                "Text", editSource, "MemberId",
                true, DataSourceUpdateMode.Never);

            txtFullName.DataBindings.Add(
                "Text", editSource, "FullName",
                true, DataSourceUpdateMode.OnPropertyChanged);

            txtPhone.DataBindings.Add(
                "Text", editSource, "Phone",
                true, DataSourceUpdateMode.OnPropertyChanged);

            txtEmail.DataBindings.Add(
                "Text", editSource, "Email",
                true, DataSourceUpdateMode.OnPropertyChanged);

            cboPackage.DataBindings.Add(
                "SelectedItem", editSource, "Package",
                true, DataSourceUpdateMode.OnPropertyChanged);

            dtpStartDate.DataBindings.Add(
                "Value", editSource, "StartDate",
                true, DataSourceUpdateMode.OnPropertyChanged);

            dtpExpiryDate.DataBindings.Add(
                "Value", editSource, "ExpiryDate",
                true, DataSourceUpdateMode.OnPropertyChanged);

            txtFee.DataBindings.Add(
                "Text", editSource, "FeeText",
                true, DataSourceUpdateMode.OnPropertyChanged);
        }

        // ==========================================
        // 3. DỮ LIỆU MẪU
        // ==========================================
        private void LoadSampleData()
        {
            members.Add(new MemberModel
            {
                MemberId = "HV001",
                FullName = "Nguyễn Văn An",
                Phone = "0912345678",
                Email = "an@example.com",
                Package = "Theo tháng",
                StartDate = DateTime.Today.AddDays(-5),
                ExpiryDate = DateTime.Today.AddMonths(1).AddDays(-5),
                FeeText = "500000"
            });

            members.Add(new MemberModel
            {
                MemberId = "HV002",
                FullName = "Trần Thị Bình",
                Phone = "0987654321",
                Email = "binh@example.com",
                Package = "6 tháng",
                StartDate = DateTime.Today,
                ExpiryDate = DateTime.Today.AddMonths(6),
                FeeText = "2500000"
            });
        }

        // ==========================================
        // 4. TỰ ĐỘNG SINH MÃ HỘI VIÊN
        // ==========================================
        private string GenerateMemberId()
        {
            int maxNumber = 0;

            foreach (MemberModel member in members)
            {
                if (member.MemberId == null ||
                    !member.MemberId.StartsWith(
                        "HV", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                int number;

                if (int.TryParse(
                    member.MemberId.Substring(2), out number) &&
                    number > maxNumber)
                {
                    maxNumber = number;
                }
            }

            return "HV" + (maxNumber + 1).ToString("000");
        }

        private MemberModel CreateNewDraft()
        {
            DateTime start = DateTime.Today;

            return new MemberModel
            {
                MemberId = GenerateMemberId(),
                FullName = "",
                Phone = "",
                Email = "",
                Package = "Theo tháng",
                StartDate = start,
                ExpiryDate = start.AddMonths(1),
                FeeText = ""
            };
        }

        private MemberModel CloneMember(MemberModel source)
        {
            return new MemberModel
            {
                MemberId = source.MemberId,
                FullName = source.FullName,
                Phone = source.Phone,
                Email = source.Email,
                Package = source.Package,
                StartDate = source.StartDate,
                ExpiryDate = source.ExpiryDate,
                FeeText = source.FeeText
            };
        }

        private MemberModel GetDraft()
        {
            return editBuffer.Count > 0 ? editBuffer[0] : null;
        }

        private void SetDraft(MemberModel draft)
        {
            loadingDraft = true;

            if (editBuffer.Count == 0)
                editBuffer.Add(draft);
            else
                editBuffer[0] = draft;

            editSource.ResetBindings(false);
            editSource.Position = 0;

            loadingDraft = false;
        }

        private void ReadControlsToDraft()
        {
            MemberModel draft = GetDraft();

            if (draft == null)
                return;

            draft.FullName = txtFullName.Text.Trim();
            draft.Phone = txtPhone.Text.Trim();
            draft.Email = txtEmail.Text.Trim();
            draft.Package = cboPackage.Text;
            draft.StartDate = dtpStartDate.Value.Date;
            draft.ExpiryDate = dtpExpiryDate.Value.Date;
            draft.FeeText = txtFee.Text.Trim();
        }

        // ==========================================
        // 5. TÍNH NGÀY HẾT HẠN THEO GÓI TẬP
        // ==========================================
        private void UpdateExpiryDate()
        {
            if (loadingDraft ||
                dtpStartDate == null ||
                dtpExpiryDate == null ||
                cboPackage == null)
            {
                return;
            }

            DateTime start = dtpStartDate.Value.Date;
            DateTime expiry = start;

            if (cboPackage.Text == "Theo tháng")
                expiry = start.AddMonths(1);
            else if (cboPackage.Text == "6 tháng")
                expiry = start.AddMonths(6);
            else if (cboPackage.Text == "1 năm")
                expiry = start.AddYears(1);

            dtpExpiryDate.Value = expiry;
        }

        // ==========================================
        // 6. CHỌN HỘI VIÊN TRÊN LƯỚI
        // ==========================================
        private void DgvMembers_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.RowIndex >= dgvMembers.Rows.Count)
            {
                return;
            }

            MemberModel member =
                dgvMembers.Rows[e.RowIndex].DataBoundItem
                as MemberModel;

            if (member == null)
                return;

            selectedMember = member;
            isAdding = false;

            SetDraft(CloneMember(member));
            errorProvider.Clear();
        }

        // ==========================================
        // 7. THÊM HỘI VIÊN
        // ==========================================
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!isAdding)
            {
                MessageBox.Show(
                    "Bấm Làm mới trước khi thêm hội viên mới.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            ReadControlsToDraft();

            if (!ValidateInput())
            {
                MessageBox.Show(
                    "Vui lòng kiểm tra các trường bị báo lỗi.",
                    "Dữ liệu không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            MemberModel draft = GetDraft();

            if (draft == null)
                return;

            if (FindMemberById(draft.MemberId) != null)
            {
                errorProvider.SetError(
                    txtMemberId, "Mã hội viên đã tồn tại.");
                return;
            }

            members.Add(CloneMember(draft));

            UpdateStatusBar();
            StartNewMember();

            MessageBox.Show(
                "Thêm hội viên thành công.",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ==========================================
        // 8. SỬA THÔNG TIN HỘI VIÊN
        // ==========================================
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (isAdding || selectedMember == null)
            {
                MessageBox.Show(
                    "Hãy chọn một hội viên trong bảng trước khi sửa.",
                    "Chưa chọn hội viên",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            ReadControlsToDraft();

            if (!ValidateInput())
            {
                MessageBox.Show(
                    "Vui lòng kiểm tra các trường bị báo lỗi.",
                    "Dữ liệu không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            MemberModel draft = GetDraft();

            if (draft == null)
                return;

            MemberModel duplicate = FindMemberById(draft.MemberId);

            if (duplicate != null &&
                !object.ReferenceEquals(duplicate, selectedMember))
            {
                errorProvider.SetError(
                    txtMemberId, "Mã hội viên đã được sử dụng.");
                return;
            }

            // Cập nhật đối tượng đang nằm trong BindingList.
            // INotifyPropertyChanged báo cho lưới thay đổi thuộc tính.
            selectedMember.FullName = draft.FullName;
            selectedMember.Phone = draft.Phone;
            selectedMember.Email = draft.Email;
            selectedMember.Package = draft.Package;
            selectedMember.StartDate = draft.StartDate;
            selectedMember.ExpiryDate = draft.ExpiryDate;
            selectedMember.FeeText = draft.FeeText;

            UpdateStatusBar();

            SetDraft(CloneMember(selectedMember));
            SelectMemberRow(selectedMember);

            MessageBox.Show(
                "Cập nhật hội viên thành công.",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void SelectMemberRow(MemberModel member)
        {
            dgvMembers.ClearSelection();

            for (int i = 0; i < dgvMembers.Rows.Count; i++)
            {
                MemberModel rowMember =
                    dgvMembers.Rows[i].DataBoundItem as MemberModel;

                if (object.ReferenceEquals(rowMember, member))
                {
                    dgvMembers.Rows[i].Selected = true;

                    if (dgvMembers.Rows[i].Cells.Count > 0)
                    {
                        dgvMembers.CurrentCell =
                            dgvMembers.Rows[i].Cells[0];
                    }

                    break;
                }
            }
        }

        // ==========================================
        // 9. XÓA HỘI VIÊN
        // ==========================================
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (isAdding || selectedMember == null)
            {
                MessageBox.Show(
                    "Hãy chọn hội viên cần xóa.",
                    "Chưa chọn hội viên",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa hội viên "
                + selectedMember.MemberId + " - "
                + selectedMember.FullName + "?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            members.Remove(selectedMember);

            UpdateStatusBar();
            StartNewMember();
        }

        private MemberModel FindMemberById(string memberId)
        {
            foreach (MemberModel member in members)
            {
                if (string.Equals(
                    member.MemberId,
                    memberId,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return member;
                }
            }

            return null;
        }

        // ==========================================
        // 10. KIỂM TRA DỮ LIỆU
        // ==========================================
        private bool ValidateInput()
        {
            errorProvider.Clear();
            bool valid = true;

            string name = txtFullName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                errorProvider.SetError(
                    txtFullName,
                    "Họ tên không được để trống.");

                valid = false;
            }
            else if (!Regex.IsMatch(
                name,
                @"^[\p{L}\p{M}]+(?:[ '\-][\p{L}\p{M}]+)*$"))
            {
                errorProvider.SetError(
                    txtFullName,
                    "Tên chỉ được chứa chữ cái, khoảng trắng, dấu nháy hoặc gạch nối.");

                valid = false;
            }

            if (!Regex.IsMatch(
                txtPhone.Text.Trim(),
                @"^[0-9]{10}$"))
            {
                errorProvider.SetError(
                    txtPhone,
                    "Số điện thoại phải gồm đúng 10 chữ số.");

                valid = false;
            }

            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email) ||
                !Regex.IsMatch(
                    email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errorProvider.SetError(
                    txtEmail, "Email không đúng định dạng.");

                valid = false;
            }

            decimal fee;

            if (!decimal.TryParse(
                txtFee.Text.Trim(),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out fee) ||
                fee <= 0M ||
                fee != decimal.Truncate(fee))
            {
                errorProvider.SetError(
                    txtFee,
                    "Phí dịch vụ phải là số nguyên dương.");

                valid = false;
            }

            if (cboPackage.SelectedIndex < 0)
            {
                errorProvider.SetError(
                    cboPackage, "Hãy chọn gói tập.");

                valid = false;
            }

            if (dtpExpiryDate.Value.Date < dtpStartDate.Value.Date)
            {
                errorProvider.SetError(
                    dtpExpiryDate,
                    "Ngày hết hạn không được trước ngày đăng ký.");

                valid = false;
            }

            return valid;
        }

        // ==========================================
        // 11. LÀM MỚI
        // ==========================================
        private void StartNewMember()
        {
            selectedMember = null;
            isAdding = true;

            errorProvider.Clear();

            SetDraft(CreateNewDraft());
            dgvMembers.ClearSelection();
        }

        // ==========================================
        // 12. STATUSSTRIP
        // ==========================================
        private void UpdateStatusBar()
        {
            decimal totalFee = 0M;

            foreach (MemberModel member in members)
            {
                totalFee += member.Fee;
            }

            lblMemberCount.Text =
                "Số hội viên: " + members.Count;

            lblTotalFee.Text =
                "Tổng phí dịch vụ: " + totalFee.ToString("N0");
        }
    }
}
