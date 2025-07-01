namespace ImageRoiCropper
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ListBox lstImages;
        private System.Windows.Forms.Button btnSelectFolder, btnSave, btnSelectSaveFolder, btnResetRoi;
        private System.Windows.Forms.TextBox txtAppendName, txtSaveFolder;
        private System.Windows.Forms.ComboBox cmbFormat;
        private System.Windows.Forms.Label lblStatus, lbl1, lbl2, lbl3, lbl4;
        private System.Windows.Forms.Button btnBatchSave;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lstImages = new System.Windows.Forms.ListBox();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.btnSelectSaveFolder = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnResetRoi = new System.Windows.Forms.Button();
            this.txtAppendName = new System.Windows.Forms.TextBox();
            this.txtSaveFolder = new System.Windows.Forms.TextBox();
            this.cmbFormat = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lbl1 = new System.Windows.Forms.Label();
            this.lbl2 = new System.Windows.Forms.Label();
            this.lbl3 = new System.Windows.Forms.Label();
            this.lbl4 = new System.Windows.Forms.Label();

            this.btnBatchSave = new System.Windows.Forms.Button();
            this.btnBatchSave.Location = new System.Drawing.Point(1000, 570); // 위치 조정
            this.btnBatchSave.Size = new System.Drawing.Size(100, 30);
            this.btnBatchSave.Text = "일괄 저장";
            this.btnBatchSave.Click += new System.EventHandler(this.btnBatchSave_Click);
            this.Controls.Add(this.btnBatchSave);

            // ListBox
            this.lstImages.Location = new System.Drawing.Point(20, 60);
            this.lstImages.Size = new System.Drawing.Size(220, 500);
            this.lstImages.SelectedIndexChanged += new System.EventHandler(this.lstImages_SelectedIndexChanged);

            // PictureBox
            this.pictureBox.Location = new System.Drawing.Point(260, 20);
            this.pictureBox.Size = new System.Drawing.Size(900, 500);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.BackColor = System.Drawing.Color.Black;
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);

            // 버튼/라벨/입력 UI 배치
            this.btnSelectFolder.Location = new System.Drawing.Point(20, 20);
            this.btnSelectFolder.Size = new System.Drawing.Size(100, 30);
            this.btnSelectFolder.Text = "이미지폴더";
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);

            this.lbl1.Location = new System.Drawing.Point(20, 570);
            this.lbl1.Size = new System.Drawing.Size(80, 20);
            this.lbl1.Text = "저장위치:";

            this.txtSaveFolder.Location = new System.Drawing.Point(90, 570);
            this.txtSaveFolder.Size = new System.Drawing.Size(280, 20);
            this.txtSaveFolder.ReadOnly = true;

            this.btnSelectSaveFolder.Location = new System.Drawing.Point(380, 570);
            this.btnSelectSaveFolder.Size = new System.Drawing.Size(70, 20);
            this.btnSelectSaveFolder.Text = "선택";
            this.btnSelectSaveFolder.Click += new System.EventHandler(this.btnSelectSaveFolder_Click);

            this.lbl2.Location = new System.Drawing.Point(470, 570);
            this.lbl2.Size = new System.Drawing.Size(70, 20);
            this.lbl2.Text = "포맷:";

            this.cmbFormat.Location = new System.Drawing.Point(510, 570);
            this.cmbFormat.Size = new System.Drawing.Size(80, 20);
            this.cmbFormat.Items.AddRange(new object[] { "BMP", "PNG", "JPG" });
            this.cmbFormat.SelectedIndex = 0;

            this.lbl3.Location = new System.Drawing.Point(610, 570);
            this.lbl3.Size = new System.Drawing.Size(100, 20);
            this.lbl3.Text = "파일명붙임:";

            this.txtAppendName.Location = new System.Drawing.Point(700, 570);
            this.txtAppendName.Size = new System.Drawing.Size(80, 20);

            this.btnResetRoi.Location = new System.Drawing.Point(800, 570);
            this.btnResetRoi.Size = new System.Drawing.Size(70, 20);
            this.btnResetRoi.Text = "ROI초기화";
            this.btnResetRoi.Click += new System.EventHandler(this.btnResetRoi_Click);

            this.btnSave.Location = new System.Drawing.Point(880, 570);
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.Text = "저장";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.lblStatus.Location = new System.Drawing.Point(260, 630);
            this.lblStatus.Size = new System.Drawing.Size(900, 23);

            // Form
            this.ClientSize = new System.Drawing.Size(1180, 670);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.lstImages);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.txtSaveFolder);
            this.Controls.Add(this.btnSelectSaveFolder);
            this.Controls.Add(this.cmbFormat);
            this.Controls.Add(this.txtAppendName);
            this.Controls.Add(this.btnResetRoi);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.lbl2);
            this.Controls.Add(this.lbl3);
            this.Text = "이미지 ROI 크롭 저장기";
        }
    }
}
