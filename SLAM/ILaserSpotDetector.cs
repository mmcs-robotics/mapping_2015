using OpenCvSharp.CPlusPlus;

namespace SLAM
{
    public interface ILaserSpotDetector
    {
        Point2f? Get(Mat frame);

        Point2f? Get(Mat frame, float x, float delta);
    }
}
