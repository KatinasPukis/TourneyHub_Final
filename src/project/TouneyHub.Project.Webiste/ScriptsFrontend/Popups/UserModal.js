$(document).ready(function () {
    // When the "Edit" button is clicked, open the modal
    $('#editUserButton').click(function () {
        var Id = $('.container').data('user-id'); // Get the participant ID from the data attribute
        $('#editUserModal').data('user-id', Id); // Store the participant ID in the modal
        $('#editUserModal').modal('show');
    });

    // When the "Save" button is clicked, retrieve the participant ID from the modal
    $('#saveUserButton').click(function () {
        var Id = $('#editUserModal').data('user-id');

        // Check if the Name and Age fields are valid
        if ($('#editUserModal')[0].checkValidity()) {
            // Create a FormData object to handle file uploads
            var formData = new FormData($('#editUserForm')[0]);

            // Append the user ID to the form data
            formData.append('Id', Id);

            // Get the edited data from the form fields within the modal
            formData.append('usernamne', $('#usernamne').val());
            formData.append('surname', $('#surname').val());
            formData.append('email', $('#email').val());
            formData.append('password', $('#password').val());

            // Send an AJAX request to the backend
            $.ajax({
                type: 'POST',
                url: '/api/sitecore/Tournament/EditUserProfile',
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    if (response.success) {
                        // Handle success, e.g., show a success message
                        alert(response.message);
                        // Reload the page
                        window.location.reload();
                    } else {
                        // Handle the error, e.g., show an error message
                        alert(response.message);
                    }
                },
                error: function (error) {
                    // Handle other types of errors, e.g., network issues
                    alert("An error occurred while communicating with the server");
                }
            });

            // Close the modal after saving
            $('#editTeamModal').modal('hide');
        } else {
            // Display an error message or take appropriate action if the form is invalid
        }
    });
});
