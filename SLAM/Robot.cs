using System;
using OpenCvSharp.CPlusPlus;

namespace SLAM
{
    public class Robot
    {
        #region Robot State Variables

        public Point2d Position { get; private set; }
        public double Direction { get; private set; }       // in relation to X-axis anti-clockwise
        public double CameraDirection {                     // in relation to X-axis clockwise
            get { return _cameraDirection + _engine.GetCameraAngle(); }
            set { _cameraDirection = value; }
        }   

        public bool IsMoving { get; private set; }
        public bool IsLookingAround { get; private set; }

        private double _cameraDirection;

        private void InitializeState()
        {
            Position = new Point2d(0, 0);
            Direction = 90;     // робот по умолчанию смотрит вдоль оси Oy
            IsMoving = false;
            IsLookingAround = false;
        }

        #endregion

        #region Tracking

        public int PositionIndex { get; private set; }


        #endregion

        #region Ctor

        private readonly IRobotEngine _engine;

        public Robot(IRobotEngine engine)
        {
            _engine = engine;
            InitializeState();

            try
            {
                _engine.Connect();
            }
            catch (Exception e)
            {
                Logger.Warn(string.Format("Ошибка подключения к роботу: {0}", e.Message));
                AppGlobals.Form.AbortEngineThread();
            }

            Logger.Success("Подключился к роботу");

            PositionIndex = 0;
        }

        #endregion

        #region Private Methods

        private static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private static double NormalizeAngle(double angle)
        {
            if (angle > 180)
                return angle - 360;
            if (angle < -180)
                return angle + 360;
            return angle;
        }

        #endregion

        public void MoveTo(Point2d targetPosition)
        {
            Logger.Write(string.Format("Отправляюсь из ({0:F1}, {1:F1}) в ({2:F1}, {3:F1})", 
                Position.X, Position.Y, targetPosition.X, targetPosition.Y));

            var deltaX = targetPosition.X - Position.X;
            var deltaY = targetPosition.Y - Position.Y;

            var temp = Math.Atan2(deltaY, deltaX);
            var targetDirection = RadianToDegree(temp);
            var distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));

            IsMoving = true;

            _engine.Turn(NormalizeAngle(Direction - targetDirection));
            _engine.Run(distance);

            IsMoving = false;

            Direction = targetDirection;
            Position = new Point2d(targetPosition.X, targetPosition.Y);

            PositionIndex++;
        }

        public void MoveToPosition(int index)
        {
            
        }

        public void LookAroundAsync()
        {
            Logger.Write("Осматриваюсь");

            //IsLookingAround = true;
            //_engine.LookAroundAsync(() => IsLookingAround = false);

            _engine.LookAroundAsync2(() => IsLookingAround = true, () => IsLookingAround = false);
        }

        public void TurnTest(double angle)
        {
            _engine.Turn(angle);
        }
    }
}