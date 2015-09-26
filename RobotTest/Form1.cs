using System.Windows.Forms;
using NKH.MindSqualls;
using NKH.MindSqualls.MotorControl;
using SLAM;

namespace RobotTest
{
    public partial class Form1 : Form
    {
        private NxtBrick _brick;
        private NxtMotorSync _motorSync;

        private const sbyte Power = 50;

        public Form1()
        {
            InitializeComponent();

            var brick = new NxtBrick(NxtCommLinkType.Bluetooth, Config.SerialPortNo)
            {
                MotorA = new NxtMotor(),
                MotorB = new NxtMotor(),
                MotorC = new NxtMotor()
            };

            var motorSync = new NxtMotorSync(brick.MotorA, brick.MotorB);

            brick.Connect();

            const int units = 100;

            var realDistance = units / Config.UnitsInMeter;
            var tachoLimit = (ushort)(realDistance * Config.RunTacho);

            motorSync.Run(Config.RunPower, tachoLimit, 0);


            //brick.Disconnect();
            

            ////_brick = new NxtBrick(NxtCommLinkType.Bluetooth, 3)
            ////{
            ////    MotorA = new NxtMotor(),
            ////    MotorB = new NxtMotor()
            ////};

            ////_motorSync = new NxtMotorSync(_brick.MotorA, _brick.MotorB);

            ////do
            ////{
            ////    _brick.Connect();
            ////    Thread.Sleep(1000);
            ////} while (!_brick.IsConnected);

            ////for (int i = 0; i < 1; ++i)
            ////{
            ////    _motorSync.ResetMotorPosition(true);
            ////    _motorSync.Idle();
            ////    _motorSync.Run(50, 4040, 100);

            ////    Thread.Sleep(10*1000);
            ////}

            //McNxtBrick brick = new McNxtBrick(NxtCommLinkType.Bluetooth, 3);

            //// Attach motors to port B and C on the NXT.
            //brick.MotorA = new McNxtMotor();
            //brick.MotorB = new McNxtMotor();

            //// Syncronize the two motors.
            //McNxtMotorSync motorPair = new McNxtMotorSync(brick.MotorA, brick.MotorB);

            //// Connect to the NXT.
            //brick.Connect();

            //// If not already running, start the MotorControl program.
            //if (brick.IsMotorControlRunning())
            //    brick.StartMotorControl();

            //// Run the motor-pair at 75% power, for a 3600 degree run.
            //motorPair.Run(75, 3600, 100);

            ////for (int i = 0; i < 3; ++i)
            ////{
            ////    uint tacho = 2000;
            ////    brick.MotorA.Run(50, tacho);
            ////    brick.MotorB.Run(-50, tacho);

            ////    Thread.Sleep(10 * 1000);
            ////}


            //// Disconnect from the NXT.
            //brick.Disconnect();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //switch (e.KeyCode)
            //{
            //    case Keys.W:
            //        _motorSync.Run(Power, 0, 0);
            //        break;
            //    case Keys.S:
            //        _motorSync.Run((sbyte)-Power, 0, 0);
            //        break;
            //    case Keys.D:
            //        _motorSync.Run(Power, 0, -100);
            //        //_brick.MotorA.Run(Power, 0);
            //        //_brick.MotorB.Run((sbyte)-Power, 0);
            //        break;
            //    case Keys.A:
            //        _brick.MotorA.Run((sbyte)-Power, 0);
            //        _brick.MotorB.Run(Power, 0);
            //        break;
            //}
        }

        //private void Form1_KeyUp(object sender, KeyEventArgs e)
        //{
        //    _motorSync.Idle();
        //}

        //private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    _brick.Disconnect();
        //}
    }
}
