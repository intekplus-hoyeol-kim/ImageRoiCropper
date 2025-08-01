using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

using OpenCvSharp;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading;

namespace ImageRoiCropper
{
    public partial class Form1 : Form
    {
        private List<string> imageFiles = new List<string>();
        private int currentIndex = 0;
        private Rectangle roiRect = Rectangle.Empty;
        private bool isDragging = false;
        private System.Drawing.Point dragStart;
        private string configPath = "config.txt";



        public Form1()
        {
            InitializeComponent();
            LoadConfig();
        }

        // using Microsoft.WindowsAPICodePack.Dialogs; // 이 줄을 주석 처리 또는 삭제

        // btnSelectFolder_Click, btnSelectSaveFolder_Click에서 CommonOpenFileDialog 사용 부분을 FolderBrowserDialog로 대체

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string folder = dlg.SelectedPath;
                    var exts = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tif" };
                    imageFiles = Directory.GetFiles(folder)
                        .Where(f => exts.Contains(Path.GetExtension(f).ToLower()))
                        .ToList();
                    lstImages.Items.Clear();
                    foreach (var f in imageFiles) lstImages.Items.Add(Path.GetFileName(f));
                    if (imageFiles.Count > 0)
                    {
                        currentIndex = 0;
                        lstImages.SelectedIndex = 0;
                        ShowImage(currentIndex);
                    }
                }
            }
        }

        // 이미지 리스트 선택
        private void lstImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstImages.SelectedIndex >= 0)
            {
                currentIndex = lstImages.SelectedIndex;
                ShowImage(currentIndex);
            }
        }

        // 저장 위치 선택
        private void btnSelectSaveFolder_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtSaveFolder.Text = dlg.SelectedPath;
            }
        }

        // 이미지 표시 (900x600)
        private void ShowImage(int idx)
        {
            if (idx < 0 || idx >= imageFiles.Count) return;
            using (var src = Cv2.ImRead(imageFiles[idx]))
            {
                var bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(src);
                pictureBox.Image?.Dispose();
                pictureBox.Image = new Bitmap(bmp, pictureBox.Width, pictureBox.Height);
                //roiRect = Rectangle.Empty; // 이미지 바꿀 때 ROI 초기화
                pictureBox.Refresh();
            }
        }

        // ROI 그리기/편집 (간단 버전: 드래그 박스)
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStart = e.Location;
                roiRect = new Rectangle(e.Location, new System.Drawing.Size(0, 0));
            }
        }
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                roiRect.Width = e.X - dragStart.X;
                roiRect.Height = e.Y - dragStart.Y;
                pictureBox.Refresh();
            }
        }
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                roiRect = NormalizeRect(roiRect);
                pictureBox.Refresh();
            }
        }
        private Rectangle NormalizeRect(Rectangle r)
        {
            int x = r.Width < 0 ? r.X + r.Width : r.X;
            int y = r.Height < 0 ? r.Y + r.Height : r.Y;
            int w = Math.Abs(r.Width);
            int h = Math.Abs(r.Height);
            return new Rectangle(x, y, w, h);
        }
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (roiRect != Rectangle.Empty && roiRect.Width > 0 && roiRect.Height > 0)
                e.Graphics.DrawRectangle(Pens.Red, roiRect);
        }

        // ROI 초기화
        private void btnResetRoi_Click(object sender, EventArgs e)
        {
            roiRect = Rectangle.Empty;
            pictureBox.Refresh();
        }

        // ROI 크롭 저장
        private void btnSave_Click(object sender, EventArgs e)
        {
            // 저장 폴더가 비어있다면 선택 창 띄우기
            if (string.IsNullOrWhiteSpace(txtSaveFolder.Text))
            {
                using (var dlg = new FolderBrowserDialog())
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                        txtSaveFolder.Text = dlg.SelectedPath;
                    else
                        return; // 저장 취소
                }
            }

            if (imageFiles.Count == 0 || roiRect == Rectangle.Empty || txtSaveFolder.Text == "")
            {
                lblStatus.Text = "이미지, ROI, 저장위치 확인!";
                return;
            }

            using (var src = Cv2.ImRead(imageFiles[currentIndex]))
            {
                // ROI 좌표 변환 (PictureBox → 원본)
                double xRatio = (double)src.Width / pictureBox.Width;
                double yRatio = (double)src.Height / pictureBox.Height;
                var roiCv = new OpenCvSharp.Rect(
                    (int)(roiRect.X * xRatio),
                    (int)(roiRect.Y * yRatio),
                    Math.Min((int)(roiRect.Width * xRatio), src.Width - (int)(roiRect.X * xRatio)),
                    Math.Min((int)(roiRect.Height * yRatio), src.Height - (int)(roiRect.Y * yRatio))
                );
                if (roiCv.Width <= 0 || roiCv.Height <= 0)
                {
                    lblStatus.Text = "ROI가 너무 작음";
                    return;
                }
                var cropped = new Mat(src, roiCv);

                string append = txtAppendName.Text;
                string format = cmbFormat.SelectedItem?.ToString() ?? "BMP";
                string ext = "." + format.ToLower();
                string fname = Path.GetFileNameWithoutExtension(imageFiles[currentIndex]) + append + ext;
                string savePath = Path.Combine(txtSaveFolder.Text, fname);

                int[] param = null;
                if (format == "JPG") param = new[] { (int)OpenCvSharp.ImwriteFlags.JpegQuality, 95 };
                if (format == "PNG") param = new[] { (int)OpenCvSharp.ImwriteFlags.PngCompression, 3 };

                Cv2.ImWrite(savePath, cropped, param ?? new int[0]);
                lblStatus.Text = $"저장됨: {savePath}";
            }
        }

        // 설정 저장/복원
        private void SaveConfig()
        {
            File.WriteAllLines(configPath, new[]
            {
                txtSaveFolder.Text,
                txtAppendName.Text,
                cmbFormat.SelectedItem?.ToString() ?? "BMP"
            });
        }
        private void LoadConfig()
        {
            if (!File.Exists(configPath)) return;
            var lines = File.ReadAllLines(configPath);
            if (lines.Length > 0) txtSaveFolder.Text = lines[0];
            if (lines.Length > 1) txtAppendName.Text = lines[1];
            if (lines.Length > 2 && cmbFormat.Items.Contains(lines[2])) cmbFormat.SelectedItem = lines[2];
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveConfig();
            base.OnFormClosing(e);
        }
        // ① 핸들러 시그니처를 async로 변경
        private async void btnBatchSave_Click(object sender, EventArgs e)
        {
            if (imageFiles.Count == 0 || roiRect == Rectangle.Empty || string.IsNullOrWhiteSpace(txtSaveFolder.Text))
            {
                lblStatus.Text = "이미지, ROI, 저장위치 확인!";
                return;
            }

            // ② UI 값/설정은 백그라운드 진입 전에 캡처
            string saveFolder = txtSaveFolder.Text;
            string append = txtAppendName.Text;
            string format = cmbFormat.SelectedItem?.ToString() ?? "BMP";
            string ext = "." + format.ToLower();
            int[] param = null;
            if (format == "JPG") param = new[] { (int)OpenCvSharp.ImwriteFlags.JpegQuality, 95 };
            if (format == "PNG") param = new[] { (int)OpenCvSharp.ImwriteFlags.PngCompression, 3 };

            // ③ 진행상태 보고용 Progress (UI 스레드에서 lblStatus 갱신)
            var progress = new Progress<string>(msg => lblStatus.Text = msg);

            btnBatchSave.Enabled = false; // 중복 클릭 방지
            int savedCount = 0;

            try
            {
                // ④ CPU 바운드 작업을 백그라운드에서 실행
                savedCount = await Task.Run(() =>
                {
                    int localSaved = 0;

                    for (int i = 0; i < imageFiles.Count; i++)
                    {
                        // === 기존 for 루프 본문 시작 ===
                        using (var src = Cv2.ImRead(imageFiles[i]))
                        {
                            // ROI 좌표 변환 (PictureBox → 원본)
                            double xRatio = (double)src.Width / pictureBox.Width;
                            double yRatio = (double)src.Height / pictureBox.Height;
                            var roiCv = new OpenCvSharp.Rect(
                                (int)(roiRect.X * xRatio),
                                (int)(roiRect.Y * yRatio),
                                Math.Min((int)(roiRect.Width * xRatio), src.Width - (int)(roiRect.X * xRatio)),
                                Math.Min((int)(roiRect.Height * yRatio), src.Height - (int)(roiRect.Y * yRatio))
                            );

                            if (roiCv.Width <= 0 || roiCv.Height <= 0)
                            {
                                // 진행 메시지 보고 후 다음 항목
                                (progress as IProgress<string>)?.Report("ROI가 너무 작음");
                                continue;
                            }

                            using (var cropped = new Mat(src, roiCv))
                            {
                                string fname = Path.GetFileNameWithoutExtension(imageFiles[i]) + append + ext;
                                string savePath = Path.Combine(saveFolder, fname);
                                Cv2.ImWrite(savePath, cropped, param ?? Array.Empty<int>());
                                localSaved++;
                            }
                        }
                        // UI 업데이트 빈도 조절
                        Thread.Sleep(10);
                        (progress as IProgress<string>)?.Report($"{i + 1}/{imageFiles.Count} 처리");
                        // === 기존 for 루프 본문 끝 ===
                    }

                    return localSaved;
                });

                lblStatus.Text = $"일괄 저장 완료: {savedCount}개";
                MessageBox.Show($"일괄 저장 완료: {savedCount}개", "일괄 저장");
            }
            catch (Exception ex)
            {
                lblStatus.Text = "오류: " + ex.Message;
            }
            finally
            {
                btnBatchSave.Enabled = true;
            }
        }


    }
}
