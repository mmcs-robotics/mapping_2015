using System;
using NKH.MindSqualls;
using NKH.MindSqualls.MotorControl;
using SLAM;

namespace RunTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var brick = new NxtBrick(NxtCommLinkType.Bluetooth, Config.SerialPortNo)
            //{
            //    MotorA = new NxtMotor(),
            //    MotorB = new NxtMotor(),
            //    MotorC = new NxtMotor()
            //};

            //var motorSync = new NxtMotorSync(brick.MotorA, brick.MotorB);

            //brick.Connect();


            ////const int units = 10;

            ////var realDistance = units / Config.UnitsInMeter;
            ////var tachoLimit = (ushort)(realDistance * Config.RunTacho);

            ////motorSync.Run(Config.RunPower, tachoLimit, 0);

            //const double angle = 90;
            //var tachoLimit = (ushort)(Math.Abs(angle) * Config.TurnTacho);

            //motorSync.Run(Config.TurnPower, tachoLimit, (sbyte)(Math.Sign(angle) * 100));


            var brick = new McNxtBrick(NxtCommLinkType.Bluetooth, Config.SerialPortNo)
            {
                MotorA = new McNxtMotor(),
                MotorB = new McNxtMotor(),
                MotorC = new McNxtMotor()
            };

            var motorSync = new McNxtMotorSync(brick.MotorA, brick.MotorB);

            brick.Connect();


            //const int units = 10;

            //var realDistance = units / Config.UnitsInMeter;
            //var tachoLimit = (ushort)(realDistance * Config.RunTacho);

            //motorSync.Run(Config.RunPower, tachoLimit, 0);

            const double angle = 370;
            var tachoLimit = (ushort)(Math.Abs(angle) * Config.TurnTacho);

            brick.MotorA.Run((sbyte)(Math.Sign(angle) * Config.TurnPower), tachoLimit);
            brick.MotorB.Run((sbyte)(-Math.Sign(angle) * Config.TurnPower), tachoLimit);

        }
    }
}
