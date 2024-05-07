using ModelingEvolution.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using PointD = ModelingEvolution.Drawing.Point<double>;
using VectorD = ModelingEvolution.Drawing.Vector<double>;
using BezierCurveD = ModelingEvolution.Drawing.BezierCurve<double>;

namespace ModelingEvolution.Controls.Blazor.HistogramControl
{
    class HistogramSeriesVm
    {
        readonly List<PointD> _points = new();
        readonly List<PointIndex> _indexes = new();
        public event Func<byte[], Task> Completed; 
        public double Width
        {
            get => _points[^1].X;
            set => _points[^1] = new PointD(value, _points[^1].Y);
        }
        public HistogramSeriesVm()
        {
            _points.Add(new Point<double>(0, 0));
            _points.Add(new Point<double>(255, 255));
            _indexes.Add(new PointIndex(0, _points));
            _indexes.Add(new PointIndex(1, _points));
        }
        public IEnumerable<BezierCurveD> Curves => BezierCurveD.Create(Points, 0.3d);
        public IReadOnlyList<PointD> Points => _points;
        public IReadOnlyList<PointIndex> Indexes => _indexes;

        public void Add(PointD point)
        {
            int index = _points.FindLastIndex(x => x.X < point.X);
            _points.Insert(index+1, point);
            _indexes.Insert(index+1, new PointIndex(index+1, _points));
            for (int i = index; i < _indexes.Count; i++)
            {
                _indexes[i].Index = i;
            }
        }

        public async Task Complete()
        {
            if (Indexes.Any(x => x.IsDirty))
            {
                byte[] result = new byte[255];

                var d = Width;
                foreach (var c in Curves)
                {
                    for (double t = 0; t <= 1; t += 0.002d)
                    {
                        var p = c.Evaluate(t);
                        var x = (int)(p.X / d * 255d);
                        result[x] = (byte)p.Y;
                    }
                }

                foreach(var i in Indexes)
                    i.Clear();

                await Completed(result);
            }
        }
    }

    class HistogramVm
    {
        private readonly List<HistogramSeriesVm> _series = new();
        private HistogramSeriesVm _selected;
        public double Width { get; set; }
        public double Height { get; set; }
        public HistogramSeriesVm Selected => _selected;
        public void Register(HistogramSeriesVm series)
        {
            _series.Add(_selected = series);
        }

        public void Add(PointD point)
        {
            _selected.Add(point);
        }

        public Task OnMouseUp() => _selected.Complete();
    }
    
    class PointIndex(int ix, List<PointD> items)
    {
        public int Index { get; set; } = ix;
        public PointD Location
        {
            get => items[Index];
            set
            {
                items[Index] = value;
                IsDirty = true;
            }
        }

        public bool IsDirty { get; private set; }
        public void Clear() => IsDirty = false;
        public bool CanMove(PointD arg, out PointD result)
        {
            result = arg;
            if (Index > 0)
            {
                if (arg.X <= items[Index - 1].X)
                    return false;
            }
            else 
                result = new PointD(0, result.Y);

            if (Index < items.Count - 1)
            {
                if (arg.X >= items[Index + 1].X)
                    return false;
            }
            else
            {
                var last = items.Last();
                result = new PointD(last.X, result.Y);
            }

            return true;
        }
    }
}
