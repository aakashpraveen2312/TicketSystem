
let warningTimeout;
let logoutTimeout;
let warningShown = false;

function resetTimers() {
    clearTimeout(warningTimeout);
    clearTimeout(logoutTimeout);

    // Refresh session
    fetch('/InActivity/KeepSessionAlive', { method: 'POST' });

    // Clear warning toast if shown
    if (warningShown) {
        toastr.clear();
        warningShown = false;
    }

    // Start warning timer: 1 min 45s
    warningTimeout = setTimeout(showToastrWarning, 105000);
}

function showToastrWarning() {
    warningShown = true;
    toastr.warning('You will be logged out in 15 seconds due to inactivity.', 'Session Timeout', {
        timeOut: 15000,
        extendedTimeOut: 0,
        closeButton: true,
        onHidden: function () {
            if (warningShown) {
                logoutUser(); // only logout if user didn't interact
            }
        }
    });

    // Set logout fallback in case toast doesn't trigger onHidden 15 sec
    logoutTimeout = setTimeout(logoutUser, 15000);
}

function logoutUser() {
    fetch('/InActivity/ClearSession', { method: 'POST' })
        .then(() => location.href = '/Login/Index');
}

// User activity events
window.onload = resetTimers;
document.onmousemove = resetTimers;
document.onkeydown = resetTimers;
document.onclick = resetTimers;
document.onscroll = resetTimers;

