
function confirmDelete(element) {
    Swal.fire({
        title: '<span style="font-size: 20px;">Are you sure you want to delete?</span>',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#6ED3D0',
        cancelButtonColor: '#F0A72D',
        confirmButtonText: '<span style="font-size: 15px;">Confirm</span>',
        cancelButtonText: '<span style="font-size: 15px;">Cancel</span>',
        customClass: {
            popup: 'my-swal-popup'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            var deleteUrl = element.getAttribute('href');
            $.ajax({
                url: deleteUrl,
                type: 'POST',
                success: function (response) {
                    if (response.status === 'success') {
                        Swal.fire({
                            toast: true,
                            position: 'center',
                            icon: 'success',
                            title: response.message,
                            showConfirmButton: false,
                            timer: 3000,
                            customClass: {
                                container: 'swal-toast-center',
                                 popup: 'swal-toast-popup'
                            }
                        }).then(() => {
                            window.location.href = response.redirectUrl;
                        });
                    } else {
                        Swal.fire({
                            toast: true,
                            position: 'bottom',
                            icon: 'error',
                            title: 'Error: ' + response.message,
                            showConfirmButton: false,
                            timer: 4000,
                            customClass: {
                                container: 'swal-toast-center'
                            }
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        toast: true,
                        position: 'bottom',
                        icon: 'error',
                        title: 'An error occurred while deleting the record',
                        showConfirmButton: false,
                        timer: 3000,
                        customClass: {
                            container: 'swal-toast-center'
                        }
                    });
                }
            });
        }
    });

    return false; // Prevent default link action
}



