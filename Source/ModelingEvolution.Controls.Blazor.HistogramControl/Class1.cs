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

        public HistogramSeriesVm()
        {
            _points.Add(new Point<double>(0, 0));
            _points.Add(new Point<double>(255, 255));
            _indexes.Add(new PointIndex(0, _points));
            _indexes.Add(new PointIndex(1, _points));
        }

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
    }

    class HistogramVm
    {
        private readonly List<HistogramSeriesVm> _series = new();
        private HistogramSeriesVm _selected;
        public void Register(HistogramSeriesVm series)
        {
            _series.Add(_selected = series);
        }

        public void Add(PointD point)
        {
            _selected.Add(point);
        }
    }
    class PointIndex(int ix, List<PointD> items)
    {
        public int Index { get; set; } = ix;
        public PointD Location
        {
            get => items[Index];
            set => items[Index] = value;
        }
    }
}
