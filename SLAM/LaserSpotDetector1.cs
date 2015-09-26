using System;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace SLAM
{
    public class LaserSpotDetector1 : ILaserSpotDetector
    {
        private static Mat[] GetContours(Mat frame)
        {
            using (var hsvImage = new Mat())
            {
                //frame.ConvertTo(frame, -1, 2.0, -250);
                //frame.ConvertTo(frame, -1, 1.0, -200);
                //frame.ConvertTo(frame, -1, 1000.0, 0);

                Cv2.CvtColor(frame, hsvImage, ColorConversion.RgbToHsv);

                Mat[] hsvImageChannels;
                Cv2.Split(hsvImage, out hsvImageChannels);
                GC.Collect();

                Cv2.InRange(hsvImageChannels[2], 250, 255, hsvImageChannels[2]);

                var temp = new Mat();
                Cv2.Dilate(hsvImageChannels[2], hsvImageChannels[2], new Mat());
                Cv2.Erode(hsvImageChannels[2], temp, new Mat());

                hsvImageChannels[2] = hsvImageChannels[2] - temp;

                Mat[] contours;
                Cv2.FindContours(hsvImageChannels[2], out contours, temp, ContourRetrieval.List, ContourChain.ApproxNone, null);

                return contours;
            }
        }

        public Point2f? Get(Mat frame)
        {
            var contours = GetContours(frame);
            if (!contours.Any()) return null;

            Point2f? result = null;

            var maxRadius = float.MinValue;
            foreach (var contour in contours)
            {
                Point2f point; float radius;
                Cv2.MinEnclosingCircle(contour, out point, out radius);

                if (maxRadius > radius) 
                    continue;

                maxRadius = radius;
                result = point;
            }

            return result;
        }

        public Point2f? Get(Mat frame, float x, float delta)
        {
            throw new NotImplementedException();
            //var contours = GetContours(frame);
            //if (!contours.Any()) return null;

            //var result = new Point2f(float.MaxValue, float.MaxValue);
            //foreach (var contour in contours)
            //{
            //    Point2f point; float radius;
            //    Cv2.MinEnclosingCircle(contour, out point, out radius);

            //    if (Math.Abs(point.X - x) < Math.Abs(result.X - x))
            //        result = point;
            //}

            //return Math.Abs(result.X - x) < delta ? (Point2f?)result : null;
        }
    }
}
