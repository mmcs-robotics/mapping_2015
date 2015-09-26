using System;
using OpenCvSharp;
using SLAM;
using OpenCvSharp.CPlusPlus;

namespace DistanceTest
{
    class Program
    {
        static void Main()
        {
            const string windowName = "Stream";
            Cv2.NamedWindow(windowName, WindowMode.AutoSize);

            AppGlobals.Camera = new Camera();
            var logic = new Logic(new LaserSpotDetector1());

            while (true)
            {
                var frame = AppGlobals.Camera.CaptureFrame();

                Mat[] hsvImageChannels;
                Cv2.Split(frame, out hsvImageChannels);

                var laserSpot = logic.GetMainSpot(frame);
                if (laserSpot.HasValue)
                {
                    var point = new Point(laserSpot.Value.X, laserSpot.Value.Y);
                    Cv2.Circle(frame, point, 10, new Scalar(255, 0, 0), 2);

                    var distance = Logic.CountDistance(frame.Height, laserSpot.Value.Y);
                    Console.WriteLine(distance);
                }

                Cv2.ImShow(windowName, frame);
                Cv2.WaitKey(100);
            }
        }

        static void ProcessFrame(Mat frame)
        {
            Cv2.NamedWindow("Test", WindowMode.AutoSize);

            using (var hsvImage = new Mat())
            {
                Cv2.CvtColor(frame, hsvImage, ColorConversion.RgbToHsv);

                Mat[] hsvImageChannels;
                Cv2.Split(hsvImage, out hsvImageChannels);

                Cv2.InRange(hsvImageChannels[2], 250, 255, hsvImageChannels[2]);
                Cv2.ImShow("Test", hsvImageChannels[2]);

                var temp = new Mat();
                Cv2.Dilate(hsvImageChannels[2], hsvImageChannels[2], new Mat());
                Cv2.Erode(hsvImageChannels[2], temp, new Mat());

                hsvImageChannels[2] = hsvImageChannels[2] - temp;

                Mat[] contours;
                Cv2.FindContours(hsvImageChannels[2], out contours, temp, ContourRetrieval.List, ContourChain.ApproxNone,
                    null);

                foreach (var contour in contours)
                {
                    Point2f point; float radius;
                    Cv2.MinEnclosingCircle(contour, out point, out radius);

                    Cv2.Circle(frame, point, (int)radius, new Scalar(255, 0, 0), 2);
                }

                GC.Collect();
            }
        }
    }
}
