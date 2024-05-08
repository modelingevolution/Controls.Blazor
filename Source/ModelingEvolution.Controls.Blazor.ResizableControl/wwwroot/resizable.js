// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

window.getControlsRect = function (elementId) {
    const element = document.getElementById(elementId);
    return element.getBoundingClientRect();
};
window.trackBoundingRect = function (dotNetReference, elementId) {
    const element = document.getElementById(elementId);

    if (!element) {
        console.error("Element not found for selector: " + selector);
        return;
    }

    let lastRect = element.getBoundingClientRect();

    const callback = function () {
        const currentRect = element.getBoundingClientRect();

        if (currentRect.width !== lastRect.width || currentRect.height !== lastRect.height ||
            currentRect.x !== lastRect.x || currentRect.y !== lastRect.y) {
            lastRect = currentRect;
           
            dotNetReference.invokeMethodAsync('OnChanged', [currentRect.x, currentRect.y, currentRect.width, currentRect.height]);
        }
    };

    const observer = new ResizeObserver(callback);
    observer.observe(element);

    let result = {
        dispose: () => {
            observer.disconnect();
            console.log('Observer was successfully disconnected.')
        }
    };
    
    return result;
};
