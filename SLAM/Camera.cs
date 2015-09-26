using System;
using System.Diagnostics;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace SLAM
{
    public class Camera
    {
        #region Ctor

        private readonly VideoCapture _capture;
        private readonly Mat _frame = new Mat();

        private static VideoCapture Capture()
        {
            return VideoCapture.FromCamera(Config.CameraId);
            //return VideoCapture.FromFile(Config.CameraUrl);
        }

        public int Width { get { return _capture.FrameHeight; } }
        public int Height { get { return _capture.FrameWidth; } }

        public Mat Frame { get { return _frame; } }

        public Camera()
        {
            _capture = Capture();
            while (!_capture.IsOpened())
            {
                Logger.Warn("Не могу подключиться к камере");
                Thread.Sleep(1000);
                _capture = Capture();
            }

            Logger.Success(String.Format("Подключился к камере ({0}x{1})",
                _capture.FrameWidth, _capture.FrameHeight));
        }

        #endregion

        public void StartStreaming()
        {
            var tFrame = new Mat();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (true)
            {
                _capture.Read(tFrame);
                if (tFrame.Empty())
                    break;

                Cv2.Transpose(tFrame, tFrame);
                Cv2.Flip(tFrame, _frame, FlipMode.X);

                var fps = string.Format("FPS: {0:F1}", (double)1000 / stopwatch.ElapsedMilliseconds);
                AppGlobals.Form.SetStreamFrame(_frame, fps);

                stopwatch.Restart();

                Thread.Sleep(10);
            }
        }

        public Mat CaptureFrame()
        {
            using (var tFrame = new Mat())
            {
                _capture.Read(tFrame);

                Cv2.Transpose(tFrame, tFrame);
                Cv2.Flip(tFrame, _frame, FlipMode.X);
            }

            return _frame;
        }
    }
}
