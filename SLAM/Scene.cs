using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using OpenCvSharp.CPlusPlus;

namespace SLAM
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    #region Auxiliary Classes

    public enum PointType
    {
        Scanned,
        OutOfRange,
        Unreachable
    };

    public class ScanPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointType Type { get; set; }
        public int PositionIndex { get; set; }

        public ScanPoint(double x, double y, PointType type, int positionIndex)
        {
            X = x;
            Y = y;
            Type = type;
            PositionIndex = positionIndex;
        }

        public ScanPoint(Point2d point, PointType type, int positionIndex)
        {
            X = point.X;
            Y = point.Y;
            Type = type;
            PositionIndex = positionIndex;
        }

        public Point2d GetPoint2D()
        {
            return new Point2d(X, Y);
        }

        public bool IsScanned()
        {
            return Type == PointType.Scanned;
        }
    }

    public class Gap : Tuple<ScanPoint, ScanPoint>
    {
        public Gap(ScanPoint item1, ScanPoint item2) : base(item1, item2) { }

        public int PositionIndex()
        {
            return Math.Max(Item1.PositionIndex, Item2.PositionIndex);
        }

        public double Width()
        {
            return Logic.Distance(Item1.GetPoint2D(), Item2.GetPoint2D());
        }
    }

    public class Scan
    {
        public LinkedList<ScanPoint> Points = new LinkedList<ScanPoint>();
        public List<Gap> Gaps = new List<Gap>();

        /// <summary>
        /// Оптимизация множества точек алгоритмом Рамера — Дугласа — Пекера
        /// https://en.wikipedia.org/wiki/Ramer%E2%80%93Douglas%E2%80%93Peucker_algorithm
        /// </summary>
        public void Optimize()
        {
            var optimizedPoints = new LinkedList<ScanPoint>();

            var tmpList = new List<ScanPoint>();
            var type = Points.First.Value.Type;

            foreach (var point in Points)
            {
                if (point.Type != type)
                {
                    optimizedPoints.AddRangeLast(DouglasPeucker(tmpList, 0, tmpList.Count - 1));

                    tmpList.Clear();
                    type = point.Type;
                }

                tmpList.Add(point);
            }

            if (tmpList.Any())
                optimizedPoints.AddRangeLast(DouglasPeucker(tmpList, 0, tmpList.Count - 1));

            Points = optimizedPoints;
        }

        #region DouglasPeucker

        private static double PerpendicularDistance(ScanPoint p, ScanPoint a, ScanPoint b)
        {
            return ((a.Y - b.Y) * p.X + (b.X - a.X) * p.Y + (a.X * b.Y - b.X * a.Y)) /
                   Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }

        private static List<ScanPoint> DouglasPeucker(IReadOnlyList<ScanPoint> list, int i0, int i1)
        {
            double dMax = 0;
            var index = i0;

            for (var i = i0 + 1; i <= i1; ++i)
            {
                var d = PerpendicularDistance(list[i], list[i0], list[i1]);
                if (d < dMax) continue;

                index = i;
                dMax = d;
            }

            if (dMax < Config.DouglasPeuckerEps)
                return new List<ScanPoint>(new [] {list[i0], list[i1]});

            var list1 = DouglasPeucker(list, i0, index);
            var list2 = DouglasPeucker(list, index, i1);

            list1.AddRange(list2);
            return list1;
        }

        #endregion

    }

    #endregion

    public class Scene : Scan
    {
        public void AddScan(Scan scan)
        {
            if (Points.Any())
                Union(scan);
            else
            {
                Points = scan.Points;
                Gaps = scan.Gaps;
            }
        }

        private void Union(Scan scan)
        {
            //// Находим объединение полигонов
            //const int scale = 1000 * 1000;
            //var subj1 = new Paths(new[] {Points.Select(e => new IntPoint((int) e.X * scale, (int) e.Y * scale)).ToList()});
            //var subj2 = new Paths(new[] {scan.Points.Select(e => new IntPoint((int) e.X * scale, (int) e.Y * scale)).ToList()});

            //var clipper = new Clipper();
            //clipper.AddPaths(subj1, PolyType.ptSubject, true);
            //clipper.AddPaths(subj2, PolyType.ptSubject, true);

            //var solution = new Paths();
            //clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            //// Конвертируем обратно в Scene
            //throw new NotImplementedException();

            Points.AddRangeLast(scan.Points);
            Gaps.AddRange(scan.Gaps);
        }
    }
}
