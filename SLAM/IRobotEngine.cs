using System;

namespace SLAM
{
    public interface IRobotEngine
    {
        void Connect();

        void LookAround();

        void LookAroundAsync2(Action stratCallback, Action finishCallback);

        void LookAroundAsync(Action callback);

        double GetCameraAngle();

        void Turn(double angle);

        void Run(double units);
    }
}
