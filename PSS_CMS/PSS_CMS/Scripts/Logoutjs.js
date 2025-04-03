// logout-prevent.js

document.addEventListener('DOMContentLoaded', function () {
    // Confirm logout functionality
    var confirmLogoutBtn = document.getElementById('confirmLogoutBtn');
    if (confirmLogoutBtn) {
        confirmLogoutBtn.addEventListener('click', function () {
            // When the user confirms, redirect them to the logout action
            window.location.href = '/Login/Logout'; // Adjust the URL if needed
        });
    }

    // Prevent back navigation
    function preventBack() {
        window.history.forward();
    }

    setTimeout(preventBack, 0);

    // Ensure the user cannot go back after unload
    window.onunload = function () {
        null;
    };
});
