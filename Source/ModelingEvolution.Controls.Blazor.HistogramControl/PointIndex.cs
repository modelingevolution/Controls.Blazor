using Microsoft.AspNetCore.Components.Web;
using ModelingEvolution.Drawing;

namespace ModelingEvolution.Controls.Blazor.HistogramControl;

class PointIndex(HistogramSeriesVm parent, int ix, List<Point<double>> items)
{
    public HistogramSeriesVm Parent { get; set; } = parent;
    public int Index { get; set; } = ix;
    public Point<double> Location
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
    public bool CanMove(Point<double> arg, out Point<double> result)
    {
        result = arg;
        if (Parent.Parent.SelectedTool == Parent.Parent.RemovePointTool) return false;

        if (Index > 0)
        {
            if (arg.X <= items[Index - 1].X)
                return false;
        }
        else 
            result = new Point<double>(0, result.Y);

        if (Index < items.Count - 1)
        {
            if (arg.X >= items[Index + 1].X)
                return false;
        }
        else
        {
            var last = items.Last();
            result = new Point<double>(last.X, result.Y);
        }

        return true;
    }

    public Task OnMouseDown(MouseEventArgs arg)
    {
        Parent.Parent.SelectedTool.HandleMouseDown(this, arg);
        return Task.CompletedTask;
    }
}