namespace SLAM
{
    public static class Config
    {
        #region Robot Movement Configuration

        public static byte SerialPortNo = 3;

        public static sbyte ShootPower = 3;
        public static double ShootTacho = 360;  // in full turn

        public static sbyte RunPower = 50;
        public static double RunTacho = 3400;    // in meter

        public static sbyte TurnPower = 30;
        public static double TurnTacho = 5.065;
        //public static double TurnTacho = 4.9;   // in degree

        public static double WaitTime = 0.2;   // Run time (sec) with speed = 1 and tacho = 1

        #endregion

        #region Camera Configuration

        public static int CameraId = 0;

        public static float CameraAltitude = 47.0f;     //расстояние от центра камеры до лазерной указки (мм)
        public static double CameraAngle = 2.5;         //ctg половины угла захвата камеры

        public static double CameraMaxDistance = 0.6f;

        #endregion

        #region Nav Configuration

        public static double UnitsInMeter = 10;

        public static double MinPointStep = 1;          //in units

        public static double RobotWidth = 0.2;            //in meters

        #endregion

        #region Laser Configuration

        public static int MainX = 380;
        public static int MainXDelta = 8;

        public static int CorrectionXMin = 0;
        public static int CorrectionXMax = 350;

        public static int InitialDelta = 95;

        #endregion

        #region Algo Configuration

        public static double DouglasPeuckerEps = 0.3;   //in units

        #endregion
    }
}
