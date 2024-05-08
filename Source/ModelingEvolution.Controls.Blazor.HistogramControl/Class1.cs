using ModelingEvolution.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using PointD = ModelingEvolution.Drawing.Point<double>;
using VectorD = ModelingEvolution.Drawing.Vector<double>;
using BezierCurveD = ModelingEvolution.Drawing.BezierCurve<double>;
using Color = MudBlazor.Color;

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
    }

    class HistogramTool
    {
        private bool _isSelected;
        public Color Color { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;

                _isSelected = value;
                Color = _isSelected ? Color.Primary : Color.Default;
                if (_parent != null! && value)
                    _parent.SelectedTool = this;
                if (_parent is { SelectedTool.IsSelected: false })
                    _parent.SelectedTool.IsSelected = true;
            }
        }
        public string CanvasClass { get; }
        public string PointClass { get; }

        public void Select()
        {
            IsSelected = true;
        }
        private readonly HistogramVm _parent;
        public HistogramTool(HistogramVm parent, 
            bool isDefault = false, 
            string canvasClass=null, 
            string pointClass=null,
            Action<PointIndex, MouseEventArgs> onMouseDown = null)
        {
            IsSelected = isDefault;
            _parent = parent;
            CanvasClass = canvasClass;
            PointClass = pointClass;
            _onMouseDown = onMouseDown;
        }

        private readonly Action<PointIndex, MouseEventArgs>? _onMouseDown;

        public void HandleMouseDown(PointIndex point, MouseEventArgs args)
        {
            _onMouseDown?.Invoke(point, args);
        }
    }
    class HistogramVm
    {
        public HistogramTool SelectedTool
        {
            get => _selectedTool;
            set
            {
                if(_selectedTool == value) return;
                _selectedTool = value;
                if (value != AddPointTool) AddPointTool.IsSelected = false;
                if (value != RemovePointTool) RemovePointTool.IsSelected = false;
                if (value != SelectTool) SelectTool.IsSelected = false;
            }
        }

        public HistogramTool SelectTool { get;  } 
        public HistogramTool AddPointTool { get;  } 
        public HistogramTool RemovePointTool { get;  } 

        private readonly List<HistogramSeriesVm> _series = new();
        private HistogramSeriesVm _selected;
        private double _width;
        private HistogramTool _selectedTool;
        
        public HistogramVm()
        {
            SelectTool = _selectedTool = new HistogramTool(this,true, "cursor-default", "cursor-grabbing");
            AddPointTool = new(this, false,"cursor-pointer", "cursor-grabbing");
            RemovePointTool = new(this, false, "cursor-default", "cursor-pointer", (point,args)=> point.Parent.Remove(point));
        }
        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                foreach (var i in _series)
                    i.Width = value;
            }
        }

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
    
    class PointIndex(HistogramSeriesVm parent, int ix, List<PointD> items)
    {
        public HistogramSeriesVm Parent { get; set; } = parent;
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
            if (Parent.Parent.SelectedTool == Parent.Parent.RemovePointTool) return false;

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

        public Task OnMouseDown(MouseEventArgs arg)
        {
            Parent.Parent.SelectedTool.HandleMouseDown(this, arg);
            return Task.CompletedTask;
        }
    }
}
