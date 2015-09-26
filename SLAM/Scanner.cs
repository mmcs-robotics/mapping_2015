using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using Chain = System.Collections.Generic.LinkedList<OpenCvSharp.CPlusPlus.Point2d>;
using Gap = System.Collections.Generic.LinkedList<OpenCvSharp.CPlusPlus.Point2d>;

namespace SLAM
{
    public static class Scanner
    {
        #region Ctor

        private static readonly Robot Robot;
        private static readonly Camera Camera;
        private static readonly Logic Logic;

        static Scanner()
        {
            Robot = AppGlobals.Robot;
            Camera = AppGlobals.Camera;
            Logic = AppGlobals.Logic;
        }

        #endregion

        public static double? GetCorrectionSpotX(Point2f? spot)
        {
            return spot.HasValue ? (double?)spot.Value.X : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance">Расстояние (м)</param>
        /// <returns></returns>
        private static Point2d GetScenePoint(double distance, double angle)
        {
            //Console.WriteLine(angle);

            return new Point2d(
                Robot.Position.X + distance * Config.UnitsInMeter * Math.Sin(angle),
                Robot.Position.Y + distance * Config.UnitsInMeter * Math.Cos(angle));
        }

        /// <summary>
        /// Произвести сканирование зоны вокруг робота определенного радиуса (из Config)
        /// </summary>
        /// <returns></returns>
        public static Scan Scan()
        {
            var scan = new Scan();

            // Начальное положение корректировочного луча
            var startCorSpotX = Logic.GetAvgCorrectionSpotX();
            if (!startCorSpotX.HasValue)
                throw new Exception("Не могу распознать точку коррекции");


            // Уточняем положение башни
            var startMainSpotX = Logic.GetAvgMainSpotX();
            if (!startMainSpotX.HasValue)
                throw new Exception("Не могу распознать точку");

            Robot.CameraDirection = Logic.CountCorrectionAngle(startCorSpotX.Value, startMainSpotX.Value);
            Logger.Warn(string.Format("Начало: {0}", Robot.CameraDirection / Math.PI * 180));


            // Запуск вращения башни
            Robot.LookAroundAsync();


            // Дожидаемся момента начала вращения башни
            var misses = 0;
            while (true)
            {
                var corSpot = Logic.GetCorrectionSpot(Camera.Frame);
                var corSpotX = GetCorrectionSpotX(corSpot);

                if (!corSpotX.HasValue)
                    Logger.Warn("НЕТ");

                if (!corSpotX.HasValue || Math.Abs(startCorSpotX.Value - corSpotX.Value) > 10)
                    misses++;

                if (misses > 3)
                    break;
            }


            // Сканируем
            misses = 0;
            double? prevCorX = null;
            var distances = new List<double>(250);
            var flag = false;

            while (true)
            {
                var frame = Camera.Frame;

                var laserSpot = Logic.GetMainSpot(frame);
                if (!laserSpot.HasValue)
                    continue;   // вот тут неплохо бы null сохранять, так как потом возникнет искажение всё карты

                //AppGlobals.Form.DrawSpot(laserSpot.Value);

                var corSpot = Logic.GetCorrectionSpot(frame);
                
                var distance = Logic.CountDistance(Camera.Height, laserSpot.Value.Y);
                distances.Add(Math.Min(distance, Config.CameraMaxDistance));

                var corSpotX = GetCorrectionSpotX(corSpot);
                flag |= !corSpotX.HasValue;

                if (corSpotX.HasValue && prevCorX.HasValue)
                    if (flag && Math.Abs(corSpotX.Value - prevCorX.Value) < 10)
                        misses++;

                if (misses > 1)
                    break;
                
                prevCorX = corSpotX;

                Thread.Sleep(100);
            }

            Logger.Warn(misses.ToString());
            Thread.Sleep(1000);

            // Конечное положение корректировочного луча
            var endCorSpotX = Logic.GetAvgCorrectionSpotX();

            double angleStep;
            if (!startCorSpotX.HasValue || !endCorSpotX.HasValue)
            {
                Logger.Warn("Не могу распознать коррекционную точку");
                angleStep = 2 * Math.PI / distances.Count;
            }
            else
            {
                var corAngle = Logic.CountCorrectionAngle(startCorSpotX.Value, endCorSpotX.Value);
                angleStep = (2 * Math.PI - corAngle) / distances.Count;
                Robot.CameraDirection = corAngle;
            }


            // Построение Scan
            ScanPoint gapStart = null;
            ScanPoint prevPoint = null;
            
            for (var i = 0; i < distances.Count; ++i)
            {
                var point = GetScenePoint(distances[i], angleStep * i);

                ScanPoint scanPoint;
                if (distances[i] < Config.CameraMaxDistance)
                {
                    scanPoint = new ScanPoint(point, PointType.Scanned, Robot.PositionIndex);
                    if (gapStart != null && prevPoint != gapStart)
                    {
                        scan.Gaps.Add(new Gap(gapStart, prevPoint));
                        gapStart = null;
                    }
                }
                else
                {
                    scanPoint = new ScanPoint(point, PointType.Unreachable, Robot.PositionIndex);
                    if (gapStart == null)
                        gapStart = scanPoint;
                }

                scan.Points.AddLast(scanPoint);
                prevPoint = scanPoint;
            }

            return scan;
        }


        //public static Scan Scan()
        //{
        //    var scan = new Scan();

        //    ScanPoint gapStart = null;
        //    ScanPoint prevPoint = null;

        //    Robot.LookAroundAsync();
        //    while (Robot.IsLookingAround)
        //    {
        //        var angle = Robot.CameraDirection + Robot.Direction;
        //        var frame = Camera.Frame;

        //        //if (angle > Math.PI * 2)
        //        //    break;

        //        var laserSpot = Logic.GetMainSpot(frame);
        //        if (!laserSpot.HasValue)
        //            continue;

        //        var distance = Logic.CountDistance(Camera.Height, laserSpot.Value.Y);
        //        distance = Math.Min(distance, Config.CameraMaxDistance);

        //        var point = GetScenePoint(distance, angle);

        //        ScanPoint scanPoint;
        //        if (distance < Config.CameraMaxDistance)
        //        {
        //            scanPoint = new ScanPoint(point, PointType.Scanned, Robot.PositionIndex);
        //            if (gapStart != null && prevPoint != gapStart)
        //            {
        //                scan.Gaps.Add(new Gap(gapStart, prevPoint));
        //                gapStart = null;
        //            }
        //        }
        //        else
        //        {
        //            scanPoint = new ScanPoint(point, PointType.Unreachable, Robot.PositionIndex);
        //            if (gapStart == null)
        //                gapStart = scanPoint;
        //        }

        //        scan.Points.AddLast(scanPoint);
        //        prevPoint = scanPoint;

        //        Thread.Sleep(50);
        //    }

        //    return scan;
        //}
    }
}