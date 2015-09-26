using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using NKH.MindSqualls;
using NKH.MindSqualls.MotorControl;

namespace SLAM
{
    class RobotEngineMc : IRobotEngine
    {
        #region Ctor

        private readonly McNxtBrick _brick;
        private readonly McNxtMotor _motorCam;
        private readonly McNxtMotorSync _motorSync;

        public RobotEngineMc()
        {
            _brick = new McNxtBrick(NxtCommLinkType.Bluetooth, Config.SerialPortNo)
            {
                MotorA = new McNxtMotor(),
                MotorB = new McNxtMotor(),
                MotorC = new McNxtMotor()
            };

            _motorCam = (McNxtMotor)_brick.MotorC;
            _motorSync = new McNxtMotorSync(_brick.MotorA, _brick.MotorB);
        }

        #endregion

        #region Private Methods

        private void ResetMotorSync()
        {
            _motorSync.Idle();
            _motorSync.ResetMotorPosition(true);
        }

        private void ResetRunMotors()
        {
            _brick.MotorA.Idle();
            _brick.MotorA.ResetMotorPosition(true);
            _brick.MotorB.Idle();
            _brick.MotorB.ResetMotorPosition(true);
        }

        private static void Wait(ushort tachoLimit, sbyte power)
        {
            const double koef = 1.2;
            var waitTime = tachoLimit * Config.WaitTime / power;

            Thread.Sleep((int)(waitTime * koef * 1000));
        }

        #endregion

        #region IRobotEngine Interface Implementation

        public void Connect()
        {
            do
            {
                _brick.Connect();
            } while (!_brick.IsConnected);
            
            ResetMotorSync();
        }

        public bool IsConnected()
        {
            return _brick.IsConnected;
        }

        public void LookAround()
        {
            _motorCam.Run(Config.ShootPower, (uint)Config.ShootTacho);
            Wait((ushort)Config.ShootTacho, Config.ShootPower);
        }

        public void LookAroundAsync2(Action startCallback, Action finishCallback)
        {
            new Thread(() =>
            {
                _motorCam.Run((sbyte)-Config.ShootPower, (uint)Config.ShootTacho);
                //while (MotorControlProxy.ISMOTORREADY(_brick.CommLink, MotorControlMotorPort.PortC))
                //    Thread.Sleep(10);

                startCallback();

                //_stopwatch = Stopwatch.StartNew();

                //Thread.Sleep(1000);
                //while (!MotorControlProxy.ISMOTORREADY(_brick.CommLink, MotorControlMotorPort.PortC))
                //    Thread.Sleep(10);

                //finishCallback();
            }).Start();
        }

        private Stopwatch _stopwatch;

        public void LookAroundAsync(Action callback)
        {
            new Thread(() =>
            {
                _motorCam.Run((sbyte)-Config.ShootPower, (uint)Config.ShootTacho);
                _stopwatch = Stopwatch.StartNew();

                Thread.Sleep(1000);
                while (!MotorControlProxy.ISMOTORREADY(_brick.CommLink, MotorControlMotorPort.PortC))
                    Thread.Sleep(20);
                
                callback();
            }).Start();
        }

        #region Old Methods

        public double GetCameraAngle()
        {
            if (_stopwatch == null)
                return 0;
            var temp = (double)_stopwatch.ElapsedMilliseconds / 8050 * Math.PI * 2;;
            return temp;
        }

        #endregion

        //public double GetCameraAngle()
        //{
        //    return _motorCam.TachoCount ?? 0;
        //}

        public void Turn(double angle)
        {
            var tachoLimit = (ushort)(Math.Abs(angle) * Config.TurnTacho);

            ////MotorControlProxy.CONTROLLED_MOTORCMD(_brick.CommLink, MotorControlMotorPort.PortA, "30", "30", '2');
            ////MotorControlProxy.CLASSIC_MOTORCMD(_brick.CommLink, MotorControlMotorPort.PortA,
            ////    Config.TurnPower.ToString(), tachoLimit.ToString(), '0');
            //MotorControlProxy.CLASSIC_MOTORCMD(_brick.CommLink, MotorControlMotorPort.PortB,
            //    "130", tachoLimit.ToString(), '0');

            //_motorSync.Run(Config.TurnPower, tachoLimit, (sbyte)(Math.Sign(angle) * 100));

            _brick.MotorA.Run((sbyte)(Math.Sign(angle) * Config.TurnPower), tachoLimit);
            _brick.MotorB.Run((sbyte)(-Math.Sign(angle) * Config.TurnPower), tachoLimit);

            Wait(tachoLimit, Config.TurnPower);
            ResetRunMotors();
        }

        public void Run(double units)
        {
            var realDistance = units / Config.UnitsInMeter;
            var tachoLimit = (ushort)(realDistance * Config.RunTacho);

            _motorSync.Run(Config.RunPower, tachoLimit, 0);

            Wait(tachoLimit, Config.RunPower);
            ResetMotorSync();
        }

        #endregion
    }
}
