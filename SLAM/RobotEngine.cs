using System;
using System.Threading;
using NKH.MindSqualls;

namespace SLAM
{
    public class RobotEngine //: IRobotEngine
    {
        #region Ctor

        private readonly NxtBrick _brick;
        private readonly NxtMotor _motorCam;
        private readonly NxtMotorSync _motorSync;

        public RobotEngine()
        {
            _brick = new NxtBrick(NxtCommLinkType.Bluetooth, Config.SerialPortNo)
            {
                MotorA = new NxtMotor(),
                MotorB = new NxtMotor(),
                MotorC = new NxtMotor()
            };

            _motorCam = _brick.MotorC;
            _motorSync = new NxtMotorSync(_brick.MotorA, _brick.MotorB);
        }

        #endregion

        #region Private Methods

        private void ConnectSafe()
        {
            try
            {
                _brick.Connect();
            }
            catch (Exception e)
            {
                Logger.Warn(string.Format("Ошибка подключения к роботу: {0}", e.Message));
                AppGlobals.Form.AbortEngineThread();
            }
        }

        private void ResetMotorSync()
        {
            _motorSync.Idle();
            _motorSync.ResetMotorPosition(true);
        }

        private static void Wait(ushort tachoLimit, sbyte power)
        {
            const double koef = 1.2;
            var waitTime = tachoLimit * Config.WaitTime / power;

            Thread.Sleep((int) (waitTime * koef * 1000));
        }

        #endregion

        #region IRobotEngine Interface Implementation

        public void Connect()
        {
            ConnectSafe();
            while (!_brick.IsConnected)
            {
                Logger.Warn("Не могу подключиться!");
                Thread.Sleep(1000);
                ConnectSafe();
            }

            Logger.Success("Подключился к роботу");

            ResetMotorSync();
        }

        public bool IsConnected()
        {
            return _brick.IsConnected;
        }

        public void LookAround()
        {
            Logger.Write("Осматриваюсь");

            _motorCam.Run(Config.ShootPower, 360);
            Wait(360, Config.ShootPower);
        }

        public void LookAroundAsync(Action action)
        {
            throw new NotImplementedException();
        }

        public double GetCameraAngle()
        {
            throw new NotImplementedException();
        }

        public void Turn(double angle)
        {
            var tachoLimit = (ushort) (Math.Abs(angle) * Config.TurnTacho);

            _motorSync.Run(Config.TurnPower, tachoLimit, (sbyte) (Math.Sign(angle) * 100));

            Wait(tachoLimit, Config.TurnPower);
            ResetMotorSync();
        }

        public void Run(double units)
        {
            var realDistance = units / Config.UnitsInMeter;
            var tachoLimit = (ushort) (realDistance * Config.RunTacho);

            _motorSync.Run(Config.RunPower, tachoLimit, 0);

            Wait(tachoLimit, Config.RunPower);
            ResetMotorSync();
        }

        #endregion
    }
}
