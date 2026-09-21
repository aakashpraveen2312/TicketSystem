let warningTimeout;
let logoutTimeout;
let warningShown = false;

function resetTimers() {

    console.log("resetTimers called");

    clearTimeout(warningTimeout);
    clearTimeout(logoutTimeout);

    // Keep ASP.NET session alive
    fetch('/InActivity/KeepSessionAlive', {
        method: 'POST',
        credentials: 'same-origin'
    })
        .then(response => {
            console.log("KeepSessionAlive:", response.status);
        })
        .catch(error => {
            console.error("KeepSessionAlive error:", error);
        });

    // Clear warning if user becomes active
    if (warningShown) {
        toastr.clear();
        warningShown = false;
    }

    // Show warning after 2 minutes of inactivity
    warningTimeout = setTimeout(showToastrWarning, 2 * 60 * 1000);
}

function showToastrWarning() {

    console.log("showToastrWarning called");

    warningShown = true;

    toastr.warning(
        'You will be logged out in 30 seconds due to inactivity.',
        'Session Timeout',
        {
            timeOut: 30000,
            extendedTimeOut: 0,
            closeButton: true
        }
    );

    // Logout after 30 seconds
    logoutTimeout = setTimeout(logoutUser, 30000);
}

function logoutUser() {

    console.log("logoutUser called");

    fetch('/InActivity/ClearSession', {
        method: 'POST',
        credentials: 'same-origin'
    })
        .then(() => {
            location.href = '/Login/Index';
        })
        .catch(error => {
            console.error("Logout error:", error);
        });
}

window.onload = resetTimers;

document.onmousemove = resetTimers;
document.onkeydown = resetTimers;
document.onclick = resetTimers;
document.onscroll = resetTimers;