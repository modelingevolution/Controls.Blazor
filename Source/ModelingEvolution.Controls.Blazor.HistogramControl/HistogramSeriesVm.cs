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
using static MudBlazor.Colors;

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
            set
            {
                var d = _points[^1].X;
                Size<double>  ratio = new Size<double>(value / d, 1);
                
                for (int i = 0; i < _points.Count; i++)
                {
                    _points[i] *= ratio;
                }
            }
        }

        public HistogramSeriesVm()
        {
            _points.Add(new Point<double>(0, 0));
            _points.Add(new Point<double>(255, 255));
            _indexes.Add(new PointIndex(this,0, _points));
            _indexes.Add(new PointIndex(this,1, _points));
        }
        public IEnumerable<BezierCurveD> Curves => BezierCurveD.Create(Points, 0.3d);
        public IReadOnlyList<PointD> Points => _points;
        public IReadOnlyList<PointIndex> Indexes => _indexes;

        internal HistogramVm Parent { get; set; }

        public void Add(PointD point)
        {
            int index = _points.FindLastIndex(x => x.X < point.X);
            _points.Insert(index+1, point);
            _indexes.Insert(index+1, new PointIndex(this,index+1, _points));
            for (int i = index; i < _indexes.Count; i++) 
                _indexes[i].Index = i;
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

        public void Remove(PointIndex point)
        {
            _indexes.RemoveAt(point.Index);
            _points.RemoveAt(point.Index);
            for (int i = point.Index; i < _indexes.Count; i++)
                _indexes[i].Index = i;
        }

        public void Load(PointD[] points)
        {
            Size<double> ratio = new Size<double>(Width / 255, 1);
            _indexes.Clear();
            _points.Clear();
            for (var index = 0; index < points.Length; index++)
            {
                var i = points[index] * ratio;
                _points.Add(i);
                _indexes.Add(new PointIndex(this, index,_points));
            }
        }
    }
}
