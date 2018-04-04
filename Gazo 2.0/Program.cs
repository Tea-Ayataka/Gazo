using Gazo.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gazo {
    class Program : Form {
        // 全モニターの領域
        private Rectangle region = RenderUtil.GetFullRegion();

        // マウスのドラッグポイント
        private Point? startPoint;

        // 選択範囲
        private Rectangle select = Rectangle.Empty;

        // 追加アクション (コンテキストメニュー)
        private ToolStripMenuItem doUpload;
        private ToolStripMenuItem doSave;

        // メイン
        [STAThread]
        static void Main(string[] args) {
            Application.Run(new Program());
        }

        private Program() {
            SuspendLayout();

            // イベント
            PreviewKeyDown += Form_KeyDown;
            MouseDown += Form_MouseDown;
            MouseMove += Form_MouseMove;
            MouseUp += Form_MouseUp;
            
            // Looks and Feel
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            Cursor = Cursors.Cross;
            ShowInTaskbar = false;
            ControlBox = false;
            TopMost = true;

            // コンテキストメニュー
            var contextMenu = new ContextMenuStrip();

            contextMenu.Items.Add(doUpload = new ToolStripMenuItem("アップロード (Shift)"));
            contextMenu.Items.Add(doSave = new ToolStripMenuItem("ファイルに保存 (Ctrl)"));

            doUpload.Click += toggle;
            doSave.Click += toggle;

            void toggle(object sender, EventArgs _) {
                ((ToolStripMenuItem)sender).Checked = !((ToolStripMenuItem)sender).Checked;
            }

            ContextMenuStrip = contextMenu;

            // 最大化
            Location = new Point(region.X, region.Y);
            Size = region.Size;

            // 透明化
            var margins = new int[] { 0, 0, Width, Height };
            RenderUtil.DwmExtendFrameIntoClientArea(Handle, ref margins);

            // レンダリング最適化
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            ResumeLayout(false);
        }

        private void Form_KeyDown(object sender, PreviewKeyDownEventArgs e) {
            if(e.KeyCode == Keys.Escape) {
                Application.Exit();
            }
        }

        // セレクターを表示
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            if (select != null) {
                // ペン・ブラシ
                var rect = new SolidBrush(Color.FromArgb(150, Color.DimGray));
                var border = new Pen(Color.FromArgb(150, Color.White));

                // ドロー
                e.Graphics.FillRectangle(rect, select);
                e.Graphics.DrawRectangle(border, select);
            }
        }

        // 選択範囲を更新する
        private void UpdateArea(bool init) {
            var mouse = PointToClient(Cursor.Position);

            // 初回時
            if (init) {
                startPoint = new Point(mouse.X, mouse.Y);
                select = new Rectangle(mouse.X, mouse.Y, 0, 0);
            }

            // 更新
            var expanded = new Rectangle(select.X - 10, select.Y - 10, select.Width + 20, select.Height + 20);
            select = RenderUtil.GetFixedArea(startPoint.Value.X, startPoint.Value.Y, mouse.X, mouse.Y);

            // レンダリング
            Invalidate(expanded);
        }

        private void Form_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                UpdateArea(true);
            }
        }

        private void Form_MouseMove(object sender, MouseEventArgs e) {
            if (startPoint != null) {
                UpdateArea(false);
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e) {
            if(select == Rectangle.Empty) {
                return;
            }

            Hide();

            // ミスクリック検知
            if(select.Height < 10 && select.Width < 10) {
                Close();
                return;
            }

            // SE再生
            new SoundPlayer(Properties.Resources.sound).Play();

            CaptureScreen();

            // SE待機
            Thread.Sleep(300);
            Close();
        }

        private void CaptureScreen() {
            // スクリーンショットを撮影
            var bitmap = new Bitmap(select.Width, select.Height);

            using (var graphics = Graphics.FromImage(bitmap)) {
                graphics.CopyFromScreen(new Point(region.X + select.X, region.Y + select.Y), new Point(0, 0), bitmap.Size);
            }

            // クリップボードにコピー
            Clipboard.SetImage(bitmap);

            // 追加アクション (Shift -> アップロード, Ctrl -> 保存)
            if ((ModifierKeys & Keys.Control) != 0 || doSave.Checked) {
                Save(bitmap);
            }

            if ((ModifierKeys & Keys.Shift) != 0 || doUpload.Checked) {
                Upload(bitmap);
            }
        }

        private void Save(Bitmap image) {
            var dialog = new SaveFileDialog();

            dialog.FileName = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            dialog.Filter = "PNG File|*.png|JPG File|*.jpg|BMP File|*.bmp";

            if (dialog.ShowDialog() != DialogResult.OK) {
                return;
            }

            var path = dialog.FileName;
            var format = path.EndsWith(".bmp") ? ImageFormat.Bmp : path.EndsWith(".jpg") || path.EndsWith(".jpeg") ? ImageFormat.Jpeg : ImageFormat.Png;

            image.Save(path, format);

            Process.Start("explorer.exe", "/select,\"" + path + "\"");
        }

        private void Upload(Bitmap image) {
            var url = Uploader.Upload(image);
            Process.Start(url);
        }
    }
}
