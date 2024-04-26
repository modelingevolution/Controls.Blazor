using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace ModelingEvolution.Controls.Blazor
{
    public static class Extensions
    {
        public static async Task ToggleFullScreen(this IJSRuntime runtime, string controlId)
        {
            await runtime.InvokeVoidAsync("toggleFullScreen", controlId);
        }
    }
}
