using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenCvSharp.CPlusPlus;

namespace SLAM
{
    public class Logic
    {
        #region Ctor

        private readonly ILaserSpotDetector _laserSpotDetector;

        public Logic(ILaserSpotDetector laserSpotDetector)
        {
            _laserSpotDetector = laserSpotDetector;
        }

        #endregion

        /// <summary>
        /// Выделить лазерную точку на кадре
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public Point2f? GetMainSpot(Mat frame)
        {
            var h = AppGlobals.Camera.Height / 2;
            var area = new Rect(Config.MainX - Config.MainXDelta, h, 2 * Config.MainXDelta, h);

            var spot = _laserSpotDetector.Get(new Mat(frame, area));
            if (spot == null)
                return null;

            return new Point2f(spot.Value.X + area.Left, spot.Value.Y + area.Top);
        }

        public Point2f? GetCorrectionSpot(Mat frame)
        {
            var h = AppGlobals.Camera.Height / 2;
            var area = new Rect(0, h, Config.MainX - 40, h);
            //var area = new Rect(Config.CorrectionX - Config.CorrectionXDelta, h, 2 * Config.CorrectionXDelta, h);

            var spot = _laserSpotDetector.Get(new Mat(frame, area));
            if (spot == null)
                return null;

            return new Point2f(spot.Value.X + area.Left, spot.Value.Y + area.Top);
        }

        public double? GetAvgCorrectionSpotX()
        {
            const int n = 10;
            var values = new List<double>(n);

            for (var i = 0; i < n; i++)
            {
                var laserSpot = GetCorrectionSpot(AppGlobals.Camera.Frame);
                if (!laserSpot.HasValue)
                    continue;

                values.Add(laserSpot.Value.X);
                Thread.Sleep(50);
            }

            var len = values.Count;
            if (len < n / 2)
                return null;

            return values.OrderBy(e => e)
                .Skip(len / 4).Take(len / 2).Average();
        }

        public double? GetAvgMainSpotX()
        {
            const int n = 10;
            var values = new List<double>(n);

            for (var i = 0; i < n; i++)
            {
                var laserSpot = GetMainSpot(AppGlobals.Camera.Frame);
                if (!laserSpot.HasValue)
                    continue;

                values.Add(laserSpot.Value.X);
                Thread.Sleep(50);
            }

            var len = values.Count;
            if (len < n / 2)
                return null;

            return values.OrderBy(e => e)
                .Skip(len / 4).Take(len / 2).Average();
        }

        public static double CountCorrectionAngle(double startCorX, double endCorX)
        {
            var delta = endCorX - startCorX;
            var width = AppGlobals.Camera.Width / 2;
            var ctan = Config.MainX * width / delta;
            return Math.Atan(1 / ctan);
        }

        /// <summary>
        /// Вычислить расстрояние до объекта (м)
        /// </summary>
        /// <param name="frameHeight"></param>
        /// <param name="pointYCoord"></param>
        /// <returns></returns>
        public static double CountDistance(int frameHeight, double pointYCoord)
        {
            var h = (double)frameHeight / 2;
            var t = Math.Abs(pointYCoord - h);      //рассторяние от центра камеры до лазерной точки

            return Config.CameraAngle * Config.CameraAltitude * (h / t) / 1000;
        }

        /// <summary>
        /// Расстрояние между точками на кооржинатной плоскости 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance(Point2d p1, Point2d p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        /// <summary>
        /// Находит точку пересечения между отрезками
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        public static Point2d? SegmentIntersection(Point2d p1, Point2d p2, Point2d p3, Point2d p4)
        {
            var dx12 = p2.X - p1.X;
            var dy12 = p2.Y - p1.Y;
            var dx34 = p4.X - p3.X;
            var dy34 = p4.Y - p3.Y;

            var denominator = (dy12 * dx34 - dx12 * dy34);

            var t1 = ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34) / denominator;

            if (double.IsInfinity(t1))
                return null;

            var t2 = ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12) / -denominator;

            if ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1))
                return new Point2d(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            return null;
        }

        public static Point2d? LineIntersection(Point2d p1, Point2d p2, Point2d p3, Point2d p4)
        {
            var dx12 = p2.X - p1.X;
            var dy12 = p2.Y - p1.Y;
            var dx34 = p4.X - p3.X;
            var dy34 = p4.Y - p3.Y;

            var denominator = (dy12 * dx34 - dx12 * dy34);

            var t1 = ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34) / denominator;

            if (double.IsInfinity(t1))
                return null;

            return new Point2d(p1.X + dx12 * t1, p1.Y + dy12 * t1);
        }

        public static Point2d GetDividingPoint(Point2d a, Point2d b, double koef)
        {
            var vector = new Point2d(b.X - a.X, b.Y - a.Y);
            return new Point2d(a.X + koef * vector.X, a.X + koef * vector.Y);
        }
    }
}
