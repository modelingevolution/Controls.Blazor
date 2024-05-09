using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace ModelingEvolution.Controls.Blazor.HistogramControl;

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