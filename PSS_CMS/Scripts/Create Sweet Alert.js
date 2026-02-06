////function submitForm(formSelector, successRedirectUrl, successLottiePath) {
////    var form = $(formSelector);
////    var $submitButton = $('#submitButton');
////    var $submitButton = $('#submitButton2');
    

////    // Change button text on form submission
////    $submitButton.prop('disabled', true).val('Saving...');
    

////    $.ajax({
////        type: 'POST',
////        url: form.attr('action'),
////        data: form.serialize(),
////        success: function (response) {
////            if (response.status === 'success') {
////                Swal.fire({
////                    title: 'Success!',
////                    html: `<div id="lottie-container" style="width: 150px; height: 150px; margin: 0 auto;"></div>
////                            <p style="font-size: 14px; font-weight: bold;">${response.message}</p>`,
////                    showConfirmButton: false,
////                    timer: 2000,
////                    didOpen: () => {
////                        lottie.loadAnimation({
////                            container: document.getElementById('lottie-container'),
////                            renderer: 'svg',
////                            loop: true,
////                            autoplay: true,
////                            path: successLottiePath
////                        });
////                    },
////                    willClose: () => {
////                        window.location.href = successRedirectUrl;
////                    }
////                });
////            } else {
////                Swal.fire({
////                    title: 'info',
////                    text: response.message || 'An error occurred. Please fill in all details',
////                    icon: 'info',
////                    confirmButtonText: 'Ok',
////                    customClass: {
////                        confirmButton: 'custom-confirm-button'
////                    }
////                });
////                resetButton();
////            }
////        },
////        error: function (xhr, status, error) {
////            Swal.fire({
////                title: 'info',
////                text: response.message ||'An unexpected error occurred. Please fill in all details and try again later.',
////                icon: 'info',
////                confirmButtonText: 'Ok',
////                customClass: {
////                    confirmButton: 'custom-confirm-button'
////                }
////            });
////            resetButton();
////        }
////    });

////    function resetButton() {
////        $submitButton.prop('disabled', false).val('Save');
////        $loader.hide();
////    }
////}

function submitForm(formSelector, successRedirectUrl, successLottiePath) {
    var form = $(formSelector);
    var $submitButton = $('#submitButton, #submitButton2'); // combine both in one line

    // Disable button and show "Saving..."
    $submitButton.prop('disabled', true).val('Saving...');

    $.ajax({
        type: 'POST',
        url: form.attr('action'),
        data: form.serialize(),
        success: function (response) {
            if (response.status === 'success') {
                Swal.fire({
                    title: 'Success!',
                    html: `<div id="lottie-container" style="width: 150px; height: 150px; margin: 0 auto;"></div>
                           <p style="font-size: 14px; font-weight: bold;">${response.message}</p>`,
                    showConfirmButton: false,
                    timer: 2000,
                    didOpen: () => {
                        lottie.loadAnimation({
                            container: document.getElementById('lottie-container'),
                            renderer: 'svg',
                            loop: true,
                            autoplay: true,
                            path: successLottiePath
                        });
                    },
                    willClose: () => {
                        window.location.href = successRedirectUrl;
                    }
                });
            } else {
                Swal.fire({
                    title: 'Message',
                    text: response.message || 'An error occurred. Please fill in all details.',
                    icon: 'info',
                    confirmButtonText: 'Ok',
                    customClass: {
                        confirmButton: 'custom-confirm-button'
                    }
                });
                resetButton(); // ✅ re-enable button on error
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: 'Error',
                text: 'An unexpected error occurred. Please fill in all details and try again later.',
                icon: 'error',
                confirmButtonText: 'Ok',
                customClass: {
                    confirmButton: 'custom-confirm-button'
                }
            });
            resetButton(); // ✅ re-enable button on AJAX error
        }
    });

    function resetButton() {
        $submitButton.prop('disabled', false).val('Save');
    }
}
