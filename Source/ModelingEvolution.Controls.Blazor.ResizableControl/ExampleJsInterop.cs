using Microsoft.JSInterop;

namespace ModelingEvolution.Controls.Blazor.ResizableControl
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    using SizeF = ModelingEvolution.Drawing.Size<float>;
    using RectangleF = ModelingEvolution.Drawing.Rectangle<float>;
    using RectangleAreaF = ModelingEvolution.Drawing.RectangleArea<float>;
    static class JsExtensions
    {
        public static async Task<DOMRect> GetBoundingClientRectAsync(this IJSRuntime r, string elementId)
        {
            return await r.InvokeAsync<DOMRect>("getControlsRect", elementId);
        }
        public static async Task<IJsBoundsObserver> ObserveBoundingRect(this IJSRuntime r, string elementId)
        {
            JsBoundsObserver tmp = new JsBoundsObserver(elementId);

            var js = await r.InvokeAsync<IJSObjectReference>("trackBoundingRect", tmp.Self, elementId);
            tmp.Configure(js);

            //await r.InvokeVoidAsync("trackBoundingRect", tmp.Self, elementId);

            return tmp;
        }
    }
    class DOMRect
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
        public static implicit operator RectangleF(DOMRect r)
        {
            return new RectangleF((float)r.Left, (float)r.Top, (float)r.Width, (float)r.Height);
        }
    }
    public interface IJsBoundsObserver : IAsyncDisposable
    {
        event Func<object, RectangleF, Task> Changed;
        string ElementId { get; }
    }

    class JsBoundsObserver : IJsBoundsObserver
    {
        private DotNetObjectReference<JsBoundsObserver>? _self;
        public event Func<object, RectangleF, Task>? Changed;
        private IJSObjectReference _js;
        public string ElementId { get; }

        internal DotNetObjectReference<JsBoundsObserver> Self
        {
            get { return _self ??= DotNetObjectReference.Create(this); }
        }
        public JsBoundsObserver(string elementId)
        {
            ElementId = elementId;
        }

        internal void Configure(IJSObjectReference jsObserver)
        {
            this._js = jsObserver;
        }

        [JSInvokable]
        public async Task OnChanged(RectangleF rect)
        {
            var f = Changed;
            if (f != null)
                await f.Invoke(this, rect);
        }

        public async ValueTask DisposeAsync()
        {
            await _js.InvokeVoidAsync("dispose");
            await _js.DisposeAsync();
            _self?.Dispose();
        }
    }
}
