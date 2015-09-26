using System;
using System.Threading;
using OpenCvSharp.CPlusPlus;

namespace SLAM
{
    public class Engine
    {
        #region Ctor

        private Scene _scene;
        private MainForm _form;
        private Robot _robot;

        private void TestRobotMovement()
        {
            _robot.TurnTest(360);

            const double len = 2;

            //AppGlobals.Robot.MoveTo(new Point2d(0, -3));
            //AppGlobals.Robot.MoveTo(new Point2d(0, 0));

            //AppGlobals.Robot.MoveTo(new Point2d(len, len));
            //AppGlobals.Robot.MoveTo(new Point2d(0, 2 * len));
            //AppGlobals.Robot.MoveTo(new Point2d(-len, len));
            //AppGlobals.Robot.MoveTo(new Point2d(0, 0));

            //AppGlobals.Robot.MoveTo(new Point2d(0, len / 2));

            //AppGlobals.Robot.LookAround();
        }

        public Engine()
        {
            InitializeAll();

            Run();
        }

        private void InitializeAll()
        {
            AppGlobals.Logic = new Logic(new LaserSpotDetector1());
            AppGlobals.Robot = new Robot(new RobotEngineMc());

            _scene = new Scene();;

            _form = AppGlobals.Form;
            _robot = AppGlobals.Robot;

            while (AppGlobals.Camera == null || AppGlobals.Robot == null)
                Thread.Sleep(100);
        }

        #endregion

        private void Run()
        {
            var mainSpotX = AppGlobals.Logic.GetAvgMainSpotX();
            if (mainSpotX == null)
                throw new Exception("Не могу распознать точку коррекции");

            Config.MainX = (int)mainSpotX.Value;


            //TestRobotMovement();

            // 1
            var scan = Scanner.Scan();
            //_scene.AddScan(scan);

            scan.Optimize();
            _scene.AddScan(scan);

            _form.RedrawScene(_robot.Position, _scene);

            // 2
            var nextScanPoint = GetNextScanPoint();
            if (!nextScanPoint.HasValue)
                throw new Exception("УРА! Отсканировали");

            _robot.MoveTo(nextScanPoint.Value);
            scan = Scanner.Scan();
            scan.Optimize();

            //_scene.AddScan(scan);
            //_form.RedrawScene(_robot.Position, _scene);
        }

        private Point2d? GetNextScanPoint()
        {
            Gap nextGap = null;
            var delta = int.MaxValue;

            foreach (var gap in _scene.Gaps)
            {
                if (gap.Width() <= Config.RobotWidth * Config.UnitsInMeter)
                    continue;

                var tmpDelta = Math.Abs(gap.PositionIndex() - _robot.PositionIndex);
                if (tmpDelta >= delta) 
                    continue;

                delta = tmpDelta;
                nextGap = gap;

                if (delta == 0)
                    break;
            }

            if (nextGap == null)
                return null;

            //TODO: Возможен баг, если угол Gap больше 180 градусов
            var gapWidth = nextGap.Width();
            //if (gapWidth <= Config.RobotWidth * Config.UnitsInMeter)
            //    continue;

            //var l1 = Logic.Distance(_robot.Position, nextGap.Item1.GetPoint2D());
            //var p1 = Logic.GetDividingPoint(_robot.Position, nextGap.Item1.GetPoint2D(), 1 / l1);

            //var l2 = Logic.Distance(_robot.Position, nextGap.Item2.GetPoint2D());
            //var p2 = Logic.GetDividingPoint(_robot.Position, nextGap.Item2.GetPoint2D(), 1 / l2);

            //var intersection = Logic.LineIntersection(p1, p2, nextGap.Item1.GetPoint2D(), nextGap.Item2.GetPoint2D());
            
            //var gapWidth = Logic.Distance(p1, p2);

            //TODO: подумать
            //var gapWidth = nextGap.Width();
            //if (gapWidth <= Config.RobotWidth * Config.UnitsInMeter)
            //    throw new Exception("Ширина пробела меньше ширины робота");

            var coef = Math.Min(Config.CameraMaxDistance * Config.UnitsInMeter / gapWidth, 0.5);
            var vector = new Point2d(nextGap.Item2.X - nextGap.Item1.X, nextGap.Item2.Y - nextGap.Item1.Y);

            return new Point2d(nextGap.Item1.X + coef * vector.X, nextGap.Item1.Y + coef * vector.Y);
        }
    }
}
