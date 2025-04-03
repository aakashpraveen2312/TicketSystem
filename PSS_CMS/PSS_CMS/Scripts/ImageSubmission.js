document.addEventListener('DOMContentLoaded', function () {
    // Show the preloader when the document is ready
    showLoadingScreen();

    // Hide the preloader when the page is fully loaded
    window.addEventListener('load', hideLoadingScreen);

    // Attach event listeners to file inputs
    const fileInputFormMap = {
        "fileInput": "form2",
        "fileInputcore": "form4",
        "fileInputskill": "form6"
    };

    Object.keys(fileInputFormMap).forEach(function (fileInputId) {
        const formId = fileInputFormMap[fileInputId];
        const fileInput = document.getElementById(fileInputId);

        if (fileInput) {
            fileInput.addEventListener("change", function (event) {
                event.preventDefault(); // Prevent default behavior
                showLoadingScreen(); // Show preloader when the file is selected

                const form = document.getElementById(formId);
                if (form) {
                    form.submit(); // Submit the form
                }
            });
        }
    });
});

function showLoadingScreen() {
    const preloader = document.getElementById('preloader');
    if (preloader) {
        preloader.style.display = 'flex'; // Show the preloader
    }
}

function hideLoadingScreen() {
    const preloader = document.getElementById('preloader');
    if (preloader) {
        preloader.style.display = 'none'; // Hide the preloader
    }
}
