using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevenueAnalytics
{
    public partial class frmRevenueAnalytics : Form
    {
        private CancellationTokenSource cts;
        private bool closeAfterCancel = false;

        public frmRevenueAnalytics()
        {
            InitializeComponent();

            this.FormClosing += frmRevenueAnalytics_FormClosing;
        }

        // =========================================
        // THONG KE DOANH THU - ASYNC/AWAIT
        // =========================================
        private async void btnStatistics_Click(
            object sender, EventArgs e)
        {
            // Khong cho chay trung nhieu tac vu
            if (cts != null)
                return;

            // Kiem tra ngay
            DateTime fromDate = dtpFromDate.Value.Date;
            DateTime toDate = dtpToDate.Value.Date;

            if (fromDate > toDate)
            {
                MessageBox.Show(
                    "Từ ngày không được lớn hơn đến ngày!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Luu cac tieu chi truoc khi xu ly
            string selectedBranch =
                cboBranch.SelectedItem == null
                ? "Tất cả chi nhánh"
                : cboBranch.SelectedItem.ToString();

            // Tao CancellationTokenSource
            CancellationTokenSource currentCts =
                new CancellationTokenSource();

            cts = currentCts;

            CancellationToken token = currentCts.Token;

            // Cap nhat giao dien
            btnStatistics.Enabled = false;
            btnCancel.Enabled = true;

            pgbLoading.Value = 0;
            lblProcessingStatus.Text =
                "Đang chuẩn bị thống kê...";

            dgvRevenueData.DataSource = null;
            dgvRevenueData.Rows.Clear();

            // IProgress cap nhat giao dien tren UI Thread
            IProgress<int> progress = new Progress<int>(
                percent =>
                {
                    // Bo qua cap nhat cu sau khi da huy
                    if (token.IsCancellationRequested)
                        return;

                    if (!ReferenceEquals(cts, currentCts))
                        return;

                    pgbLoading.Value = Math.Max(
                        0, Math.Min(100, percent));

                    lblProcessingStatus.Text =
                        "Đang thống kê doanh thu: "
                        + percent + "%";
                });

            try
            {
                // =================================
                // BAT BUOC: TASK.RUN + AWAIT
                // Cong viec chay tren Background Thread
                // =================================
                List<RevenueRecord> result =
                    await Task.Run(async () =>
                    {
                        List<RevenueRecord> data =
                            new List<RevenueRecord>(1000);

                        Random random = new Random();

                        string[] branches =
                        {
                            "Chi nhánh Hà Nội",
                            "Chi nhánh TP.HCM",
                            "Chi nhánh Đà Nẵng"
                        };

                        string[] descriptions =
                        {
                            "Bán hàng",
                            "Thu phí dịch vụ",
                            "Khám bệnh",
                            "Xét nghiệm",
                            "Bán thuốc và vật tư"
                        };

                        int totalDays =
                            (toDate - fromDate).Days;

                        // 100 dot x 50ms = khoang 5 giay
                        for (int step = 1; step <= 100; step++)
                        {
                            // Kiem tra yeu cau huy
                            token.ThrowIfCancellationRequested();

                            // Gia lap truy xuat CSDL
                            await Task.Delay(50, token);

                            // Moi dot tao 10 giao dich
                            for (int j = 0; j < 10; j++)
                            {
                                token.ThrowIfCancellationRequested();

                                DateTime transactionDate =
                                    fromDate.AddDays(
                                        random.Next(totalDays + 1));

                                string transactionBranch;

                                if (selectedBranch ==
                                    "Tất cả chi nhánh")
                                {
                                    transactionBranch =
                                        branches[random.Next(
                                            branches.Length)];
                                }
                                else
                                {
                                    transactionBranch =
                                        selectedBranch;
                                }

                                RevenueRecord record =
                                    new RevenueRecord();

                                record.TransactionId =
                                    "GD" +
                                    (data.Count + 1).ToString("0000");

                                record.TransactionDate =
                                    transactionDate;

                                record.Branch =
                                    transactionBranch;

                                record.Description =
                                    descriptions[random.Next(
                                        descriptions.Length)];

                                record.Amount =
                                    random.Next(100000, 10000000);

                                data.Add(record);
                            }

                            // Cap nhat tien do tu 1% den 99%
                            // 100% chi hien thi khi hoan tat
                            if (step < 100)
                            {
                                progress.Report(step);
                            }
                        }

                        // Kiem tra huy lan cuoi
                        token.ThrowIfCancellationRequested();

                        return data;

                    }, token);

                // Khong hien thi ket qua neu vua bi huy
                token.ThrowIfCancellationRequested();

                // Tro ve UI Thread de hien thi ket qua
                dgvRevenueData.DataSource =
                    new BindingList<RevenueRecord>(result);

                pgbLoading.Value = 100;

                lblProcessingStatus.Text =
                    "Hoàn thành! Tổng số giao dịch: "
                    + result.Count.ToString("N0");

            }
            catch (OperationCanceledException)
            {
                // Dung thanh tien trinh
                pgbLoading.Value = 0;

                lblProcessingStatus.Text =
                    "Đã hủy thao tác!";

                if (!closeAfterCancel)
                {
                    MessageBox.Show(
                        "Đã hủy thao tác!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                pgbLoading.Value = 0;

                lblProcessingStatus.Text =
                    "Có lỗi xảy ra!";

                if (!closeAfterCancel)
                {
                    MessageBox.Show(
                        "Lỗi khi thống kê: " + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            finally
            {
                // Giai phong CancellationTokenSource
                if (ReferenceEquals(cts, currentCts))
                    cts = null;

                currentCts.Dispose();

                if (!IsDisposed)
                {
                    btnStatistics.Enabled = true;
                    btnCancel.Enabled = false;
                }

                // Neu dang dong Form, chi dong sau khi
                // tac vu nen da ket thuc
                if (closeAfterCancel && !IsDisposed)
                {
                    closeAfterCancel = false;

                    BeginInvoke(new Action(() =>
                    {
                        Close();
                    }));
                }
            }
        }

        // =========================================
        // HUY THAO TAC
        // =========================================
        private void btnCancel_Click(
            object sender, EventArgs e)
        {
            if (cts != null &&
                !cts.IsCancellationRequested)
            {
                // De nghi tac vu nen dung lai
                cts.Cancel();

                btnCancel.Enabled = false;

                // Dung hien thi tien do ngay lap tuc
                pgbLoading.Value = 0;

                lblProcessingStatus.Text =
                    "Đang hủy thao tác...";
            }
        }

        // =========================================
        // DONG FORM AN TOAN KHI DANG XU LY
        // =========================================
        private void frmRevenueAnalytics_FormClosing(
            object sender, FormClosingEventArgs e)
        {
            if (cts != null)
            {
                // Tam hoan dong Form cho den khi huy xong
                e.Cancel = true;

                closeAfterCancel = true;

                cts.Cancel();

                btnCancel.Enabled = false;

                lblProcessingStatus.Text =
                    "Đang hủy tác vụ để đóng cửa sổ...";
            }
        }
    }

    // =============================================
    // LOP DU LIEU GIAO DICH
    // =============================================
    public class RevenueRecord
    {
        public string TransactionId { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Branch { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }
    }
}