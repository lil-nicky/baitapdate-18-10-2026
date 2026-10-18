
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace bai413
{
   
    // MODEL SÁCH
    
    public class BookModel : INotifyPropertyChanged
    {
        private string bookId = "";
        private string title = "";
        private string author = "";
        private string category = "";
        private int quantity;
        private decimal unitPrice;
        private string coverPath = "";

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
            ref T field,
            T value,
            string propertyName)
        {
            if (object.Equals(field, value))
                return;

            field = value;
            Notify(propertyName);
        }

        public string BookId
        {
            get { return bookId; }
            set { SetField(ref bookId, value, "BookId"); }
        }

        public string Title
        {
            get { return title; }
            set { SetField(ref title, value, "Title"); }
        }

        public string Author
        {
            get { return author; }
            set { SetField(ref author, value, "Author"); }
        }

        public string Category
        {
            get { return category; }
            set { SetField(ref category, value, "Category"); }
        }

        public int Quantity
        {
            get { return quantity; }
            set { SetField(ref quantity, value, "Quantity"); }
        }

        public decimal UnitPrice
        {
            get { return unitPrice; }
            set { SetField(ref unitPrice, value, "UnitPrice"); }
        }

        public string CoverPath
        {
            get { return coverPath; }
            set
            {
                if (coverPath == value)
                    return;

                coverPath = value;
                Notify("CoverPath");
                Notify("CoverFileName");
            }
        }

        // Chỉ hiển thị tên ảnh trong bảng, không hiển thị đường dẫn dài.
        public string CoverFileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CoverPath))
                    return "";

                return Path.GetFileName(CoverPath);
            }
        }
    }

    // =========================================
    // FORM QUẢN LÝ SÁCH
    // =========================================
    public partial class Form1 : Form
    {
        private readonly BindingList<BookModel> books =
            new BindingList<BookModel>();

        private readonly BindingList<BookModel> filteredBooks =
            new BindingList<BookModel>();

        private readonly BindingSource bookSource =
            new BindingSource();

        private readonly ErrorProvider errorProvider =
            new ErrorProvider();

        private BookModel selectedBook;
        private string selectedCoverPath = "";

        private bool isAdding = true;

        private TextBox txtBookId;
        private TextBox txtTitle;
        private TextBox txtAuthor;
        private TextBox txtSearch;
        private TextBox txtUnitPrice;

        private ComboBox cboCategory;

        private NumericUpDown nudQuantity;

        private PictureBox picBookCover;
        private DataGridView dgvBooks;

        private ToolStripStatusLabel lblBookCount;
        private ToolStripStatusLabel lblTotalQuantity;
        private ToolStripStatusLabel lblInventoryValue;
        private ToolStripStatusLabel lblCurrentTime;

        private System.Windows.Forms.Timer clockTimer;

        public Form1()
        {
            InitializeComponent();

            CreateInterface();
            ConfigureDataBinding();

            books.ListChanged += Books_ListChanged;

            LoadSampleData();
            StartNewBook();
            RefreshGrid();
            UpdateStatusStrip();

            clockTimer.Start();
        }

        // =========================================
        // 1. TẠO GIAO DIỆN
        // =========================================
        private void CreateInterface()
        {
            errorProvider.ContainerControl = this;

            // MENU STRIP
            MenuStrip menuStrip = new MenuStrip();

            ToolStripMenuItem menuFile =
                new ToolStripMenuItem("File");

            ToolStripMenuItem menuOpen =
                new ToolStripMenuItem("Mở file CSV");

            ToolStripMenuItem menuSave =
                new ToolStripMenuItem("Lưu file CSV");

            ToolStripMenuItem menuExit =
                new ToolStripMenuItem("Thoát");

            menuOpen.Click += delegate
            {
                OpenCsv();
            };

            menuSave.Click += delegate
            {
                SaveCsv();
            };

            menuExit.Click += delegate
            {
                Close();
            };

            menuFile.DropDownItems.Add(menuOpen);
            menuFile.DropDownItems.Add(menuSave);
            menuFile.DropDownItems.Add(new ToolStripSeparator());
            menuFile.DropDownItems.Add(menuExit);

            ToolStripMenuItem menuEdit =
                new ToolStripMenuItem("Edit");

            ToolStripMenuItem menuAdd =
                new ToolStripMenuItem("Thêm");

            ToolStripMenuItem menuDelete =
                new ToolStripMenuItem("Xóa");

            ToolStripMenuItem menuSearch =
                new ToolStripMenuItem("Tìm kiếm");

            menuAdd.Click += delegate
            {
                AddBook();
            };

            menuDelete.Click += delegate
            {
                DeleteBook();
            };

            menuSearch.Click += delegate
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            };

            menuEdit.DropDownItems.Add(menuAdd);
            menuEdit.DropDownItems.Add(menuDelete);
            menuEdit.DropDownItems.Add(menuSearch);

            menuStrip.Items.Add(menuFile);
            menuStrip.Items.Add(menuEdit);

            // TOOL STRIP CÓ ICON
            ToolStrip toolStrip = new ToolStrip();

            ToolStripButton btnOpenFile =
                new ToolStripButton("Mở file");

            btnOpenFile.Image = SystemIcons.Application.ToBitmap();
            btnOpenFile.DisplayStyle =
                ToolStripItemDisplayStyle.ImageAndText;

            ToolStripButton btnSaveFile =
                new ToolStripButton("Lưu file");

            btnSaveFile.Image = SystemIcons.Information.ToBitmap();
            btnSaveFile.DisplayStyle =
                ToolStripItemDisplayStyle.ImageAndText;

            ToolStripButton btnAddBook =
                new ToolStripButton("Thêm mới");

            btnAddBook.Image = SystemIcons.Asterisk.ToBitmap();
            btnAddBook.DisplayStyle =
                ToolStripItemDisplayStyle.ImageAndText;

            btnOpenFile.Click += delegate
            {
                OpenCsv();
            };

            btnSaveFile.Click += delegate
            {
                SaveCsv();
            };

            btnAddBook.Click += delegate
            {
                StartNewBook();
            };

            toolStrip.Items.Add(btnOpenFile);
            toolStrip.Items.Add(btnSaveFile);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(btnAddBook);

            // STATUS STRIP
            StatusStrip statusStrip = new StatusStrip();

            lblBookCount = new ToolStripStatusLabel();
            lblTotalQuantity = new ToolStripStatusLabel();
            lblInventoryValue = new ToolStripStatusLabel();
            lblCurrentTime = new ToolStripStatusLabel();

            statusStrip.Items.Add(lblBookCount);
            statusStrip.Items.Add(new ToolStripStatusLabel("|"));
            statusStrip.Items.Add(lblTotalQuantity);
            statusStrip.Items.Add(new ToolStripStatusLabel("|"));
            statusStrip.Items.Add(lblInventoryValue);
            statusStrip.Items.Add(new ToolStripStatusLabel("|"));
            statusStrip.Items.Add(lblCurrentTime);

            clockTimer = new System.Windows.Forms.Timer();
            clockTimer.Interval = 1000;

            clockTimer.Tick += delegate
            {
                lblCurrentTime.Text =
                    "Thời gian: " +
                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            };

            // BỐ CỤC CHÍNH
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

            // =========================================
            // THÔNG TIN SÁCH
            // =========================================
            GroupBox inputGroup = new GroupBox();
            inputGroup.Text = "Chi tiết sách";
            inputGroup.Dock = DockStyle.Fill;

            TableLayoutPanel inputLayout =
                new TableLayoutPanel();

            inputLayout.Dock = DockStyle.Fill;
            inputLayout.Padding = new Padding(6);
            inputLayout.AutoScroll = true;
            inputLayout.ColumnCount = 2;
            inputLayout.RowCount = 0;

            inputLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 35F));

            inputLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 65F));

            txtBookId = new TextBox();
            txtBookId.ReadOnly = true;
            txtBookId.BackColor = Color.LightGray;

            txtTitle = new TextBox();
            txtAuthor = new TextBox();
            txtUnitPrice = new TextBox();

            cboCategory = new ComboBox();
            cboCategory.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboCategory.Items.AddRange(new object[]
            {
                "Văn học",
                "Kinh tế",
                "Kỹ năng sống",
                "Giáo trình",
                "Thiếu nhi",
                "Khoa học",
                "Khác"
            });

            cboCategory.SelectedIndex = 0;

            nudQuantity = new NumericUpDown();
            nudQuantity.Minimum = 0;
            nudQuantity.Maximum = 1000000;
            nudQuantity.Value = 1;
            nudQuantity.ThousandsSeparator = true;

            picBookCover = new PictureBox();
            picBookCover.Width = 145;
            picBookCover.Height = 165;
            picBookCover.BorderStyle = BorderStyle.FixedSingle;
            picBookCover.SizeMode = PictureBoxSizeMode.Zoom;
            picBookCover.BackColor = Color.White;

            Button btnLoadCover = CreateButton("Tải ảnh bìa", 105);
            btnLoadCover.Height = 30;

            FlowLayoutPanel coverPanel =
                new FlowLayoutPanel();

            coverPanel.Dock = DockStyle.Fill;
            coverPanel.WrapContents = false;
            coverPanel.AutoScroll = true;

            picBookCover.Margin = new Padding(3, 3, 8, 3);
            btnLoadCover.Margin = new Padding(3, 55, 3, 3);

            coverPanel.Controls.Add(picBookCover);
            coverPanel.Controls.Add(btnLoadCover);

            AddInputRow(inputLayout, "Mã sách:",
                txtBookId, 38);

            AddInputRow(inputLayout, "Tên sách:",
                txtTitle, 38);

            AddInputRow(inputLayout, "Tác giả:",
                txtAuthor, 38);

            AddInputRow(inputLayout, "Thể loại:",
                cboCategory, 38);

            AddInputRow(inputLayout, "Số lượng:",
                nudQuantity, 38);

            AddInputRow(inputLayout, "Đơn giá:",
                txtUnitPrice, 38);

            AddInputRow(inputLayout, "Ảnh bìa:",
                coverPanel, 175);

            FlowLayoutPanel buttonPanel =
                new FlowLayoutPanel();

            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.WrapContents = true;
            buttonPanel.AutoScroll = true;

            Button btnAdd = CreateButton("Thêm sách", 85);
            Button btnUpdate = CreateButton("Cập nhật", 85);
            Button btnDelete = CreateButton("Xóa sách", 75);
            Button btnClear = CreateButton("Làm mới", 75);

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

            // =========================================
            // DANH SÁCH SÁCH
            // =========================================
            TableLayoutPanel rightLayout =
                new TableLayoutPanel();

            rightLayout.Dock = DockStyle.Fill;
            rightLayout.ColumnCount = 1;
            rightLayout.RowCount = 2;

            rightLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 42));

            rightLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel searchPanel =
                new FlowLayoutPanel();

            searchPanel.Dock = DockStyle.Fill;
            searchPanel.WrapContents = false;
            searchPanel.AutoScroll = true;

            Label lblSearch = new Label();
            lblSearch.Text = "Tìm sách:";
            lblSearch.AutoSize = true;
            lblSearch.Margin = new Padding(3, 8, 5, 3);

            txtSearch = new TextBox();
            txtSearch.Width = 300;
            txtSearch.Margin = new Padding(3, 5, 3, 3);

            searchPanel.Controls.Add(lblSearch);
            searchPanel.Controls.Add(txtSearch);

            dgvBooks = new DataGridView();

            dgvBooks.Dock = DockStyle.Fill;
            dgvBooks.AutoGenerateColumns = false;
            dgvBooks.ReadOnly = true;
            dgvBooks.AllowUserToAddRows = false;
            dgvBooks.AllowUserToDeleteRows = false;
            dgvBooks.MultiSelect = false;
            dgvBooks.RowHeadersVisible = false;

            dgvBooks.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvBooks.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvBooks.BackgroundColor = Color.White;
            dgvBooks.BorderStyle = BorderStyle.Fixed3D;

            // Tất cả các cột được khai báo thủ công.
            AddGridColumn("Mã sách", "BookId", 70);
            AddGridColumn("Tên sách", "Title", 130);
            AddGridColumn("Tác giả", "Author", 100);
            AddGridColumn("Thể loại", "Category", 90);
            AddGridColumn("Số lượng", "Quantity", 65);
            AddGridColumn("Đơn giá", "UnitPrice", 90, "N0");
            AddGridColumn("Ảnh bìa", "CoverFileName", 85);

            rightLayout.Controls.Add(searchPanel, 0, 0);
            rightLayout.Controls.Add(dgvBooks, 0, 1);

            mainLayout.Controls.Add(inputGroup, 0, 0);
            mainLayout.Controls.Add(rightLayout, 1, 0);

            // Dock layout
            Controls.Add(mainLayout);
            Controls.Add(statusStrip);
            Controls.Add(toolStrip);
            Controls.Add(menuStrip);

            MainMenuStrip = menuStrip;

            // Gắn sự kiện
            btnLoadCover.Click += BtnLoadCover_Click;
            btnAdd.Click += delegate { AddBook(); };
            btnUpdate.Click += delegate { UpdateBook(); };
            btnDelete.Click += delegate { DeleteBook(); };
            btnClear.Click += delegate { StartNewBook(); };

            txtSearch.TextChanged += delegate
            {
                RefreshGrid();
            };

            dgvBooks.CellClick += DgvBooks_CellClick;

            FormClosed += delegate
            {
                clockTimer.Stop();
                clockTimer.Dispose();
                ClearCoverImage();
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
            label.Margin = new Padding(3);

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
                column.DefaultCellStyle.Format = format;

            dgvBooks.Columns.Add(column);
        }

        // =========================================
        // 2. DATA BINDING
        // =========================================
        private void ConfigureDataBinding()
        {
            bookSource.DataSource = filteredBooks;
            dgvBooks.DataSource = bookSource;
        }

        // =========================================
        // 3. DỮ LIỆU MẪU
        // =========================================
        private void LoadSampleData()
        {
            books.Add(new BookModel
            {
                BookId = "BK001",
                Title = "Cho tôi xin một vé đi tuổi thơ",
                Author = "Nguyễn Nhật Ánh",
                Category = "Văn học",
                Quantity = 10,
                UnitPrice = 85000M,
                CoverPath = ""
            });

            books.Add(new BookModel
            {
                BookId = "BK002",
                Title = "Đắc nhân tâm",
                Author = "Dale Carnegie",
                Category = "Kỹ năng sống",
                Quantity = 15,
                UnitPrice = 90000M,
                CoverPath = ""
            });
        }

        // =========================================
        // 4. SINH MÃ SÁCH
        // =========================================
        private string GenerateBookId()
        {
            int maxNumber = 0;

            foreach (BookModel book in books)
            {
                if (book.BookId == null ||
                    !book.BookId.StartsWith(
                        "BK",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                int number;

                if (int.TryParse(book.BookId.Substring(2), out number) &&
                    number > maxNumber)
                {
                    maxNumber = number;
                }
            }

            return "BK" + (maxNumber + 1).ToString("000");
        }

        // =========================================
        // 5. TẢI ẢNH BÌA
        // =========================================
        private void BtnLoadCover_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn ảnh bìa sách";
                dialog.Filter =
                    "Ảnh bìa (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|"
                    + "Tất cả tệp ảnh|*.png;*.jpg;*.jpeg;*.bmp";

                dialog.Multiselect = false;

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    DisplayCover(dialog.FileName);
                    selectedCoverPath = dialog.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Không thể mở ảnh bìa.\n" + ex.Message,
                        "Lỗi ảnh",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void DisplayCover(string path)
        {
            if (string.IsNullOrWhiteSpace(path) ||
                !File.Exists(path))
            {
                ClearCoverImage();
                return;
            }

            Image newImage;

            // Nạp bản sao để không khóa file ảnh gốc.
            using (Image source = Image.FromFile(path))
            {
                newImage = new Bitmap(source);
            }

            Image oldImage = picBookCover.Image;

            picBookCover.Image = newImage;
            picBookCover.SizeMode = PictureBoxSizeMode.Zoom;

            if (oldImage != null)
                oldImage.Dispose();
        }

        private void ClearCoverImage()
        {
            Image oldImage = picBookCover.Image;
            picBookCover.Image = null;

            if (oldImage != null)
                oldImage.Dispose();
        }

        // =========================================
        // 6. CHỌN SÁCH TRÊN LƯỚI
        // =========================================
        private void DgvBooks_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.RowIndex >= dgvBooks.Rows.Count)
                return;

            BookModel book =
                dgvBooks.Rows[e.RowIndex].DataBoundItem
                as BookModel;

            if (book == null)
                return;

            selectedBook = book;
            isAdding = false;

            txtBookId.Text = book.BookId;
            txtTitle.Text = book.Title;
            txtAuthor.Text = book.Author;
            txtUnitPrice.Text =
                book.UnitPrice.ToString("0.##", CultureInfo.CurrentCulture);

            nudQuantity.Value = Math.Max(
                nudQuantity.Minimum,
                Math.Min(nudQuantity.Maximum, book.Quantity));

            int index = cboCategory.Items.IndexOf(book.Category);

            if (index >= 0)
                cboCategory.SelectedIndex = index;
            else
                cboCategory.SelectedIndex = -1;

            selectedCoverPath = book.CoverPath;

            if (!string.IsNullOrWhiteSpace(book.CoverPath) &&
                File.Exists(book.CoverPath))
            {
                try
                {
                    DisplayCover(book.CoverPath);
                }
                catch
                {
                    ClearCoverImage();
                }
            }
            else
            {
                ClearCoverImage();
            }

            errorProvider.Clear();
        }

        // =========================================
        // 7. THÊM SÁCH
        // =========================================
        private void AddBook()
        {
            if (!isAdding)
            {
                MessageBox.Show(
                    "Bấm Làm mới hoặc nút Thêm mới trên ToolStrip trước.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!ValidateInput())
                return;

            BookModel book = CreateBookFromInput();
            book.BookId = GenerateBookId();

            books.Add(book);

            UpdateStatusStrip();
            StartNewBook();

            MessageBox.Show(
                "Thêm sách thành công.",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // =========================================
        // 8. CẬP NHẬT SÁCH
        // =========================================
        private void UpdateBook()
        {
            if (selectedBook == null || isAdding)
            {
                MessageBox.Show(
                    "Hãy chọn một sách trong bảng để cập nhật.",
                    "Chưa chọn sách",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput())
                return;

            selectedBook.Title = txtTitle.Text.Trim();
            selectedBook.Author = txtAuthor.Text.Trim();
            selectedBook.Category = cboCategory.Text;
            selectedBook.Quantity = (int)nudQuantity.Value;

            decimal price;

            decimal.TryParse(
                txtUnitPrice.Text.Trim(),
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out price);

            selectedBook.UnitPrice = price;
            selectedBook.CoverPath = selectedCoverPath;

            // Model phát PropertyChanged, BindingList báo thay đổi
            // và lưới cập nhật dòng đã chọn.
            UpdateStatusStrip();

            MessageBox.Show(
                "Cập nhật sách thành công.",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // =========================================
        // 9. XÓA SÁCH
        // =========================================
        private void DeleteBook()
        {
            if (selectedBook == null)
            {
                MessageBox.Show(
                    "Hãy chọn sách cần xóa trong bảng.",
                    "Chưa chọn sách",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa sách \""
                + selectedBook.Title + "\"?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            books.Remove(selectedBook);

            UpdateStatusStrip();
            StartNewBook();
        }

        private BookModel CreateBookFromInput()
        {
            decimal price = 0M;

            decimal.TryParse(
                txtUnitPrice.Text.Trim(),
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out price);

            return new BookModel
            {
                BookId = GenerateBookId(),
                Title = txtTitle.Text.Trim(),
                Author = txtAuthor.Text.Trim(),
                Category = cboCategory.Text,
                Quantity = (int)nudQuantity.Value,
                UnitPrice = price,
                CoverPath = selectedCoverPath
            };
        }

        private bool ValidateInput()
        {
            errorProvider.Clear();
            bool valid = true;

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                errorProvider.SetError(
                    txtTitle, "Tên sách không được để trống.");
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                errorProvider.SetError(
                    txtAuthor, "Tác giả không được để trống.");
                valid = false;
            }

            if (cboCategory.SelectedIndex < 0)
            {
                errorProvider.SetError(
                    cboCategory, "Hãy chọn thể loại.");
                valid = false;
            }

            decimal price;

            if (!decimal.TryParse(
                txtUnitPrice.Text.Trim(),
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out price) ||
                price < 0M)
            {
                errorProvider.SetError(
                    txtUnitPrice,
                    "Đơn giá phải là số không âm.");
                valid = false;
            }

            if (nudQuantity.Value < 0)
            {
                errorProvider.SetError(
                    nudQuantity,
                    "Số lượng không được âm.");
                valid = false;
            }

            if (!valid)
            {
                MessageBox.Show(
                    "Vui lòng kiểm tra các trường bị báo lỗi.",
                    "Dữ liệu không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return valid;
        }

        // =========================================
        // 10. LÀM MỚI FORM
        // =========================================
        private void StartNewBook()
        {
            selectedBook = null;
            isAdding = true;
            selectedCoverPath = "";

            txtBookId.Text = GenerateBookId();
            txtTitle.Clear();
            txtAuthor.Clear();
            txtUnitPrice.Text = "0";

            if (cboCategory.Items.Count > 0)
                cboCategory.SelectedIndex = 0;

            nudQuantity.Value = 1;

            ClearCoverImage();
            errorProvider.Clear();
            dgvBooks.ClearSelection();
        }

        // =========================================
        // 11. TÌM KIẾM TỨC THỜI
        // =========================================
        private void RefreshGrid()
        {
            if (txtSearch == null || bookSource == null)
                return;

            string keyword = txtSearch.Text.Trim();

            filteredBooks.RaiseListChangedEvents = false;
            filteredBooks.Clear();

            foreach (BookModel book in books)
            {
                bool matches =
                    string.IsNullOrEmpty(keyword) ||
                    ContainsText(book.BookId, keyword) ||
                    ContainsText(book.Title, keyword) ||
                    ContainsText(book.Author, keyword) ||
                    ContainsText(book.Category, keyword);

                if (matches)
                    filteredBooks.Add(book);
            }

            filteredBooks.RaiseListChangedEvents = true;
            bookSource.ResetBindings(false);
        }

        private bool ContainsText(string source, string keyword)
        {
            if (source == null)
                return false;

            return source.IndexOf(
                keyword,
                StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void Books_ListChanged(
            object sender,
            ListChangedEventArgs e)
        {
            RefreshGrid();
            UpdateStatusStrip();
        }

        // =========================================
        // 12. XUẤT FILE CSV
        // =========================================
        private void SaveCsv()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Lưu danh sách sách";
                dialog.Filter = "CSV (*.csv)|*.csv";
                dialog.FileName = "DanhSachSach.csv";
                dialog.DefaultExt = "csv";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    using (StreamWriter writer = new StreamWriter(
                        dialog.FileName,
                        false,
                        new UTF8Encoding(true)))
                    {
                        writer.WriteLine(
                            "BookId,Title,Author,Category,Quantity,UnitPrice,CoverPath");

                        // Xuất toàn bộ danh sách sách, không chỉ kết quả tìm kiếm.
                        foreach (BookModel book in books)
                        {
                            writer.WriteLine(
                                CsvEscape(book.BookId) + "," +
                                CsvEscape(book.Title) + "," +
                                CsvEscape(book.Author) + "," +
                                CsvEscape(book.Category) + "," +
                                book.Quantity.ToString(
                                    CultureInfo.InvariantCulture) + "," +
                                book.UnitPrice.ToString(
                                    CultureInfo.InvariantCulture) + "," +
                                CsvEscape(book.CoverPath));
                        }
                    }

                    MessageBox.Show(
                        "Lưu file CSV thành công.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Không thể lưu file CSV.\n" + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =========================================
        // 13. MỞ VÀ ĐỌC FILE CSV
        // =========================================
        private void OpenCsv()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Mở danh sách sách";
                dialog.Filter = "CSV (*.csv)|*.csv|Tất cả tệp (*.*)|*.*";
                dialog.Multiselect = false;

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    List<BookModel> importedBooks =
                        new List<BookModel>();

                    int skippedRows = 0;
                    string[] lines = File.ReadAllLines(
                        dialog.FileName, Encoding.UTF8);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i]))
                            continue;

                        List<string> fields = ParseCsvLine(lines[i]);

                        // Bỏ qua dòng tiêu đề.
                        if (i == 0 &&
                            fields.Count > 0 &&
                            fields[0].Trim().Equals(
                                "BookId",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        if (fields.Count != 7)
                        {
                            skippedRows++;
                            continue;
                        }

                        int quantity;
                        decimal unitPrice;

                        bool validQuantity = int.TryParse(
                            fields[4],
                            NumberStyles.Integer,
                            CultureInfo.InvariantCulture,
                            out quantity);

                        bool validPrice = decimal.TryParse(
                            fields[5],
                            NumberStyles.Number,
                            CultureInfo.InvariantCulture,
                            out unitPrice);

                        if (!validQuantity ||
                            !validPrice ||
                            quantity < 0 ||
                            unitPrice < 0M ||
                            string.IsNullOrWhiteSpace(fields[0]) ||
                            string.IsNullOrWhiteSpace(fields[1]))
                        {
                            skippedRows++;
                            continue;
                        }

                        importedBooks.Add(new BookModel
                        {
                            BookId = fields[0],
                            Title = fields[1],
                            Author = fields[2],
                            Category = fields[3],
                            Quantity = quantity,
                            UnitPrice = unitPrice,
                            CoverPath = fields[6]
                        });
                    }

                    // Chỉ thay danh sách sau khi đã đọc file.
                    books.RaiseListChangedEvents = false;
                    books.Clear();

                    foreach (BookModel book in importedBooks)
                    {
                        books.Add(book);
                    }

                    books.RaiseListChangedEvents = true;

                    // Báo cho BindingSource và lưới cập nhật sau import.
                    RefreshGrid();
                    UpdateStatusStrip();
                    StartNewBook();

                    string message =
                        "Đã nhập " + importedBooks.Count
                        + " đầu sách.";

                    if (skippedRows > 0)
                    {
                        message += "\nBỏ qua "
                            + skippedRows
                            + " dòng dữ liệu không hợp lệ.";
                    }

                    MessageBox.Show(
                        message,
                        "Nhập CSV hoàn tất",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    books.RaiseListChangedEvents = true;

                    MessageBox.Show(
                        "Không thể đọc file CSV.\n" + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // Phân tích CSV có dấu phẩy và dấu ngoặc kép được escape.
        private List<string> ParseCsvLine(string line)
        {
            List<string> fields = new List<string>();
            StringBuilder field = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char current = line[i];

                if (current == '"')
                {
                    if (inQuotes &&
                        i + 1 < line.Length &&
                        line[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (current == ',' && !inQuotes)
                {
                    fields.Add(field.ToString());
                    field.Clear();
                }
                else
                {
                    field.Append(current);
                }
            }

            fields.Add(field.ToString());
            return fields;
        }

        private string CsvEscape(string value)
        {
            if (value == null)
                value = "";

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        // =========================================
        // 14. CẬP NHẬT STATUS STRIP
        // =========================================
        private void UpdateStatusStrip()
        {
            if (lblBookCount == null ||
                lblTotalQuantity == null ||
                lblInventoryValue == null ||
                lblCurrentTime == null)
            {
                return;
            }

            int totalQuantity = 0;
            decimal inventoryValue = 0M;

            foreach (BookModel book in books)
            {
                totalQuantity += book.Quantity;
                inventoryValue += book.Quantity * book.UnitPrice;
            }

            lblBookCount.Text =
                "Tổng số đầu sách: " + books.Count;

            lblTotalQuantity.Text =
                "Tổng số cuốn: " + totalQuantity;

            lblInventoryValue.Text =
                "Giá trị kho: " + inventoryValue.ToString("N0");

            lblCurrentTime.Text =
                "Thời gian: " +
                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
