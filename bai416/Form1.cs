
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiTap5
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource cts;
        private Task monitoringTask;

        private decimal currentPrice = 67000m;
        private int syncCount = 0;

        private readonly Random random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (monitoringTask != null && !monitoringTask.IsCompleted)
                return;

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            btnStart.Enabled = false;
            btnPause.Enabled = true;

            monitoringTask = Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        decimal oldPrice = currentPrice;

                        decimal change = (decimal)(
                            random.NextDouble() * 200 - 100);

                        decimal newPrice = Math.Max(
                            1m, oldPrice + change);

                        currentPrice = newPrice;

                        string direction;

                        if (newPrice > oldPrice)
                            direction = "Tăng";
                        else if (newPrice < oldPrice)
                            direction = "Giảm";
                        else
                            direction = "Không đổi";

                        DateTime updateTime = DateTime.Now;
                        int count = Interlocked.Increment(ref syncCount);

                        // Không cập nhật Control trực tiếp
                        // từ Background Thread.
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() =>
                            {
                                UpdateUI(
                                    oldPrice,
                                    newPrice,
                                    direction,
                                    updateTime,
                                    count);
                            }));
                        }
                        else
                        {
                            UpdateUI(
                                oldPrice,
                                newPrice,
                                direction,
                                updateTime,
                                count);
                        }

                        await Task.Delay(2000, token)
                            .ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Đã yêu cầu dừng giám sát.
                }
            });

            try
            {
                await monitoringTask;
            }
            catch (OperationCanceledException)
            {
                // Bảo vệ khi tác vụ bị hủy.
            }
            finally
            {
                if (cts != null)
                {
                    cts.Dispose();
                    cts = null;
                }

                monitoringTask = null;

                if (!this.IsDisposed && !this.Disposing)
                {
                    btnStart.Enabled = true;
                    btnPause.Enabled = false;
                }
            }
        }

        private void UpdateUI(
            decimal oldPrice,
            decimal newPrice,
            string direction,
            DateTime updateTime,
            int count)
        {
            lblBTCPrice.Text =
                "BTC/USD: $" + newPrice.ToString("N2");

            if (newPrice > oldPrice)
                lblBTCPrice.ForeColor = Color.Green;
            else if (newPrice < oldPrice)
                lblBTCPrice.ForeColor = Color.Red;
            else
                lblBTCPrice.ForeColor = Color.Gray;

            lblLastUpdate.Text =
                "Cập nhật cuối: " +
                updateTime.ToString("HH:mm:ss");

            lblSyncCount.Text =
                "Đồng bộ thành công: " + count;

            dgvHistoryLog.Rows.Add(
                updateTime.ToString("HH:mm:ss"),
                "$" + newPrice.ToString("N2"),
                direction);

            if (dgvHistoryLog.Rows.Count > 0)
            {
                dgvHistoryLog.FirstDisplayedScrollingRowIndex =
                    dgvHistoryLog.Rows.Count - 1;
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (cts != null)
            {
                btnPause.Enabled = false;
                cts.Cancel();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (cts != null)
                cts.Cancel();

            base.OnFormClosing(e);
        }
    }
}
