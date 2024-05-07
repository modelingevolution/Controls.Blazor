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

    public static class MouseExtensions
    {
        public static Vector<double> ClientMouseCoordinates(this MouseEventArgs e)
        {
            return new Vector<double>(e.ClientX, e.ClientY);
        }
    }
}
