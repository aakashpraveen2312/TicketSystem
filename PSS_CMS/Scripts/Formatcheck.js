function validateFile() {
    var fileInput = document.getElementById("myfile");
    var filePath = fileInput.value;
    var errorMsg = document.getElementById("fileError");
    var submitBtn = document.getElementById("submitBtn");
    var allowedExtensions = /(\.pdf|\.xlsx|\.jpeg|\.png|\.jpg|\.docx|\.csv)$/i;

    if (filePath === "") {
        errorMsg.style.display = "none"; // Hide error message if no file is selected
        submitBtn.disabled = false; // Keep button disabled until a valid file is chosen
        submitBtn.style.cursor = "pointer";
        return;
    }

    if (!allowedExtensions.test(filePath)) {
        errorMsg.style.display = "block";
        submitBtn.disabled = true; // Disable the button
        submitBtn.style.cursor = "not-allowed";
    } else {
        errorMsg.style.display = "none";
        submitBtn.disabled = false; // Enable the button
        submitBtn.style.cursor = "pointer";
    }
}