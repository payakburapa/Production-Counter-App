using System;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;

namespace ProductionCounterApp
{
    public class ProductionCounterForm : Form
    {
        private int completed = 0, inProgress = 0, pending = 0, dailyTarget = 0, dailyTargetTemp = 0, reject = 0; 
        private Label lblCompleted, lblInProgress, lblPending, lblClock, lblTarget, lblDailyTargetDisplay, lblReject, lblGuide; 
        private Button btnAddCompleted, btnAddInProgress, btnAddPending, btnDelCompleted, btnDelInProgress, btnDelPending, btnExit;
        private TextBox txtTarget;
        private System.Windows.Forms.Timer clockTimer;

        public ProductionCounterForm()
        {
            this.Text = "Production Counter App";
            this.Size = new Size(500, 400);

            // Target input
            lblTarget = new Label() { Text = "🎯 จำนวนเป้าหมายของวันนี้:", Left = 20, Top = 20, Width = 150 };
            lblGuide = new Label() { Text = "(ใส่จำนวนเป้าหมายแล้วกด Enter )", Left = 20, Top = 50, Width = 190 };
            txtTarget = new TextBox() { Left = 200, Top = 20, Width = 100 };
            lblDailyTargetDisplay = new Label() { Text = "เป้าหมายวันนี้: 0", Left = 320, Top = 20, Width = 150, ForeColor = Color.Blue };

            txtTarget.KeyPress += (sender, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (int.TryParse(txtTarget.Text, out dailyTarget) && dailyTarget > 0)
                    {
                        lblDailyTargetDisplay.Text = $"เป้าหมายวันนี้: {dailyTarget}";
                        dailyTargetTemp = dailyTarget;
                        txtTarget.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("⚠️ กรุณาเพิ่มจำนวนเป้าหมายของวันให้ถูกต้อง.", "Invalid Target", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dailyTarget = 0;
                    }
                }
            };

            // Status Labels
            lblCompleted = new Label() { Text = "✅ ผลิตสำเร็จ: 0", Left = 20, Top = 100, Width = 200 };
            lblInProgress = new Label() { Text = "🔧 อยู่ในกระบวนการ: 0", Left = 20, Top = 130, Width = 200 };
            lblPending = new Label() { Text = "⏳ รอคิวผลิต: 0", Left = 20, Top = 160, Width = 200 };
            lblReject = new Label() { Text = "❌ ยกเลิก: 0", Left = 20, Top = 190, Width = 200, ForeColor = Color.Red };

            // Buttons
            btnAddCompleted = new Button() { Text = "+ Completed", Left = 240, Top = 100, Width = 100 };
            btnDelCompleted = new Button() { Text = "- Completed", Left = 360, Top = 100, Width = 100 };

            btnAddInProgress = new Button() { Text = "+ In Progress", Left = 240, Top = 130, Width = 100 };
            btnDelInProgress = new Button() { Text = "- In Progress", Left = 360, Top = 130, Width = 100 };

            btnAddPending = new Button() { Text = "+ Pending", Left = 240, Top = 160, Width = 100 };
            btnDelPending = new Button() { Text = "- Pending", Left = 360, Top = 160, Width = 100 };

            btnExit = new Button() { Text = "ออกจากโปรแกรม", Left = 200, Top = 250, Width = 100 };

            // Clock
            lblClock = new Label() { Left = 150, Top = 300, Width = 300 };

            clockTimer = new System.Windows.Forms.Timer() { Interval = 1000 };
            clockTimer.Tick += (sender, e) => UpdateClock();
            clockTimer.Start();

            // Button click events
            btnAddCompleted.Click += (sender, e) => { if (inProgress > 0) { completed++; inProgress--; UpdateLabels(); } else MessageBox.Show("⚠️ ไม่มีชิ้นงานในกระบวนการ!"); };
            btnDelCompleted.Click += (sender, e) => { RejectItem(ref completed); };

            btnAddInProgress.Click += (sender, e) => { if (pending > 0) { inProgress++; pending--; UpdateLabels(); } else MessageBox.Show("⚠️ ไม่มีชิ้นงานในรอคิว!"); };
            btnDelInProgress.Click += (sender, e) => { RejectItem(ref inProgress); };

            btnAddPending.Click += (sender, e) => { if ((completed + inProgress + pending + reject) < dailyTargetTemp) { pending++; dailyTarget--; lblDailyTargetDisplay.Text = $"เป้าหมายวันนี้: {dailyTarget}"; UpdateLabels(); } else MessageBox.Show("🚨 เกินเป้าหมายของวันแล้ว!"); };
            btnDelPending.Click += (sender, e) => { RejectItem(ref pending); };

            btnExit.Click += (sender, e) => { Application.Exit(); };

            // Add controls to form
            this.Controls.Add(lblTarget);
            this.Controls.Add(lblGuide);
            this.Controls.Add(txtTarget);
            this.Controls.Add(lblDailyTargetDisplay);
            this.Controls.Add(lblCompleted);
            this.Controls.Add(lblInProgress);
            this.Controls.Add(lblPending);
            this.Controls.Add(lblReject);
            this.Controls.Add(btnAddCompleted);
            this.Controls.Add(btnDelCompleted);
            this.Controls.Add(btnAddInProgress);
            this.Controls.Add(btnDelInProgress);
            this.Controls.Add(btnAddPending);
            this.Controls.Add(btnDelPending);
            this.Controls.Add(btnExit);
            this.Controls.Add(lblClock);
        }

        private void RejectItem(ref int count)
        {
            if (count > 0)
            {
                count--;
                reject++;
            }
            else MessageBox.Show("⚠️ ไม่สามารถลดจำนวนต่ำกว่า 0!");
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            lblCompleted.Text = $"✅ ผลิตสำเร็จ: {completed}";
            lblInProgress.Text = $"🔧 อยู่ในกระบวนการ: {inProgress}";
            lblPending.Text = $"⏳ รอคิวผลิต: {pending}";
            lblReject.Text = $"❌ ยกเลิก: {reject}";
        }

        private void UpdateClock()
        {
            var thaiCulture = new CultureInfo("th-TH");
            lblClock.Text = DateTime.Now.ToString("HH:mm:ss  วันที่ dd MMMM yyyy", thaiCulture);
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new ProductionCounterForm());
        }
    }
}
