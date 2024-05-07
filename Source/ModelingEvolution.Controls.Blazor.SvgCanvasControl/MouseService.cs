using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ModelingEvolution.Drawing;

namespace ModelingEvolution.Controls.Blazor.SvgCanvasControl
{
    

    class MouseService 
    {
        public event Func<MouseEventArgs, Task>? OnMove;
        public event Func<MouseEventArgs, Task>? OnUp;
        public event Func<MouseEventArgs, Task>? OnLeave;

        public Task FireMove(MouseEventArgs evt) => OnMove?.Invoke(evt) ?? Task.CompletedTask;
        public Task FireUp(MouseEventArgs evt) => OnUp?.Invoke(evt) ?? Task.CompletedTask;
        public Task FireLeave(MouseEventArgs evt) => OnLeave?.Invoke(evt) ?? Task.CompletedTask;
    }
    public enum Direction
    {
        Top,
        Right,
        Bottom,
        Left
    }
    public delegate bool MoveDelegate(Point<double> actual, out Point<double> expected);
    public static class MouseExtensions
    {
        public static Vector<double> ClientMouseCoordinates(this MouseEventArgs e)
        {
            return new Vector<double>(e.ClientX, e.ClientY);
        }
    }

    public class CanvasMouseEventArgs
    {
        public Point<double> Location { get; init; }
        public MouseEventArgs Args { get; init; }
    }
    public record ViewBox
    {
        public static ViewBox Create(double x, double y, double w, double h)
        {
            return new ViewBox() { Offset = new Vector<double>(x, y), Size = new Size<double>(w, h) };
        }
        public Size<double> Size { get; set; }
        public Vector<double> Offset { get; set; }

        public Size<double> ElementSize { get; internal set; }
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", Offset.X, Offset.Y, Size.Width, Size.Height);
        }

        public Point<double> Calculate(double x, double y)
        {
            var dstX = x * Size.Width / ElementSize.Width;
            var dstY = y * Size.Height / ElementSize.Height;
            return new Point<double>(dstX, dstY);
        }
        public Point<double> Calculate(Point<double> point)
        {
            return point * Size / ElementSize;
            
        }

        public Vector<double> Calculate(Vector<double> point)
        {
            return point * Size / ElementSize;
        }
    }
}
