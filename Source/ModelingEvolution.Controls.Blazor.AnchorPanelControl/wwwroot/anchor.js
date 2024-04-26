
window.toggleFullScreen = function (id) {
    const elem = document.getElementById(id);
    if (!document.fullscreenElement) {
        elem.requestFullscreen();
    } else {
        document.exitFullscreen();
    }
};
