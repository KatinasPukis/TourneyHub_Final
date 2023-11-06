$(document).ready(function () {
    // When the "Edit" button is clicked, open the modal
    $('#editButton').click(function () {
        var Id = $('.card').data('participant-id'); // Get the participant ID from the data attribute
        $('#editModal').data('participant-id', Id); // Store the participant ID in the modal
        $('#editModal').modal('show');
    });

    // When the "Save" button is clicked, retrieve the participant ID from the modal
    $('#saveButton').click(function () {
        var Id = $('#editModal').data('participant-id');

        // Check if the Name and Age fields are valid
        if ($('#editForm')[0].checkValidity()) {
            // Create a FormData object to handle file uploads
            var formData = new FormData($('#editForm')[0]);

            // Access the uploaded image
            var imageFile = $('#editimage')[0].files[0];

            // Append the image file to the form data
            formData.append('editimage', imageFile);

            // Append the participant ID to the form data
            formData.append('Id', Id);

            // Get the edited data from the form fields within the modal
            formData.append('name', $('#name').val());
            formData.append('surname', $('#surname').val());
            formData.append('age', $('#age').val());
            formData.append('info', $('#info').val());

            // Send an AJAX request to the backend
            $.ajax({
                type: 'POST',
                url: '/api/sitecore/Tournament/EditTournamentParticipant',
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
            $('#editModal').modal('hide');
        } else {
            // Display an error message or take appropriate action if the form is invalid
        }
    });
});
