using System;
using System.Drawing;
using System.Linq;
using OpenCvSharp.CPlusPlus;
using Point = OpenCvSharp.CPlusPlus.Point;

namespace SLAM
{
    public partial class MainForm
    {
        private Graphics _graphics;
        private int _centerX, _centerY;

        private void InitializeGraphics()
        {
            _graphics = sceneBox.CreateGraphics();
            _graphics.Clear(Color.White);

            _centerX = sceneBox.Width / 2;
            _centerY = sceneBox.Height / 2;
        }

        private const int PointSize = 10;
        private readonly Brush _robotBrush = Brushes.Red;
        private readonly Brush _pointBrush = Brushes.Green;
        private readonly Brush _uPointBrush = Brushes.CornflowerBlue;
        private readonly Pen _linePen = Pens.Green;
        private readonly Pen _uLinePen = Pens.CornflowerBlue;

        private Point GetPoint(Point2d point, Point2d center, double scale)
        {
            return new Point(_centerX + point.X * scale, _centerY - point.Y * scale);
        }

        private void DrawPoint(Brush brush, Point point)
        {
            _graphics.FillEllipse(brush, point.X, point.Y, PointSize, PointSize);
        }

        private void DrawLine(Pen pen, Point p1, Point p2)
        {
            var t = PointSize / 2;
            _graphics.DrawLine(pen, p1.X + t, p1.Y + t, p2.X + t, p2.Y + t);
        }

        public void RedrawScene(Point2d robotPos, Scene scene)
        {
            _graphics.Clear(Color.White);

            double maxX = scene.Points.Max(p => p.X), minX = scene.Points.Min(p => p.X);
            double maxY = scene.Points.Max(p => p.Y), minY = scene.Points.Min(p => p.Y);

            var scaleX = (sceneBox.Width - 20) / (maxX - minX);
            var scaleY = (sceneBox.Height - 20) / (maxY - minY);

            var scale = Math.Min(scaleX, scaleY) / 2;
            var center = new Point2d((minX - maxX) / 2 * scale, (minY - maxX) / 2 * scale);


            DrawPoint(_robotBrush, GetPoint(robotPos, center, scale));
            
            var e = scene.Points.GetEnumerator();
            if (!e.MoveNext())
                return;

            var prev = GetPoint(e.Current.GetPoint2D(), center, scale);
            DrawPoint(e.Current.IsScanned() ? _pointBrush : _uPointBrush, prev);

            while (e.MoveNext())
            {
                var cur = GetPoint(e.Current.GetPoint2D(), center, scale);
                DrawLine(_uLinePen, prev, cur);
                DrawPoint(e.Current.IsScanned() ? _pointBrush : _uPointBrush, cur);

                prev = cur;
            }

            foreach (var gap in scene.Gaps)
            {
                DrawPoint(Brushes.Gold, GetPoint(gap.Item1.GetPoint2D(), center, scale));
                DrawPoint(Brushes.Gold, GetPoint(gap.Item2.GetPoint2D(), center, scale));
            }
        }
    }
}
