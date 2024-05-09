using ModelingEvolution.Drawing;

namespace ModelingEvolution.Controls.Blazor.HistogramControl;

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

    public void Add(Point<double> point)
    {
        _selected.Add(point);
    }

    public Task OnMouseUp() => _selected.Complete();
}