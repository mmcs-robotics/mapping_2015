using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

namespace SLAM
{
    public partial class MainForm : Form
    {
        private readonly Thread _engineThread;
        
        public void AbortEngineThread()
        {
            _engineThread.Abort();
        }

        public MainForm()
        {
            // Инициализируем форму
            InitializeComponent();
            AppGlobals.Form = this;

            // Инициализируем рисование
            InitializeGraphics();

            // Инициализируем камеру
            new Thread(() => {
                AppGlobals.Camera = new Camera();
                AppGlobals.Camera.StartStreaming();
            }).Start();

            // Инициализируем движок
            _engineThread = new Thread(() => AppGlobals.Engine = new Engine());
            _engineThread.Start();
        }

        #region Element Setters

        public void DrawSpot(Point2f spot)
        {
            using (var graphics = Graphics.FromImage(streamBox.Image))
            {
                graphics.DrawEllipse(Pens.CornflowerBlue, spot.X - 2, spot.Y - 2, 4, 4);

                //streamBox.Image = new Bitmap(bitmap, streamBox.Size);
            }
        }

        public void SetStreamFrame(Mat frame, string text)
        {
            using (var bitmap = frame.ToBitmap())
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var h = bitmap.Height / 2;
                var w = bitmap.Width / 2;

                graphics.DrawRectangle(Pens.Crimson, Config.MainX - Config.MainXDelta, h, 2 * Config.MainXDelta, h);
                graphics.DrawRectangle(Pens.Chartreuse, 0, h, w - 20, h);

                streamBox.Image = new Bitmap(bitmap, streamBox.Size);
            }

            cameraInfo.Invoke((MethodInvoker)(() => cameraInfo.Text = text));
        }

        delegate void AppendLogCallback(string text);
        public void AppendLog(string text)
        {
            if (logBox.InvokeRequired)
            {
                var d = new AppendLogCallback(AppendLog);
                Invoke(d, text);
            }
            else
            {
                logBox.Items.Add(text);
            }
        }

        #endregion
    }
}
