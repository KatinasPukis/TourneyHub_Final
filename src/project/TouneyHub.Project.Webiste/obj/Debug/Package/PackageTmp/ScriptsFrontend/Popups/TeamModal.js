$(document).ready(function () {
    // When the "Edit" button is clicked, open the modal
    $('#editTeamButton').click(function () {
        var Id = $('.col-md-8').data('team-id'); // Get the participant ID from the data attribute
        $('#editTeamModal').data('team-id', Id); // Store the participant ID in the modal
        $('#editTeamModal').modal('show');
    });

    // When the "Save" button is clicked, retrieve the participant ID from the modal
    $('#saveTeamButton').click(function () {
        var Id = $('#editTeamModal').data('team-id');

        // Check if the Name and Age fields are valid
        if ($('#editTeamForm')[0].checkValidity()) {
            // Create a FormData object to handle file uploads
            var formData = new FormData($('#editTeamForm')[0]);

            // Access the uploaded image
            var imageFile = $('#teamimage')[0].files[0];

            // Append the image file to the form data
            formData.append('teamimage', imageFile);

            // Append the participant ID to the form data
            formData.append('Id', Id);

            // Get the edited data from the form fields within the modal
            formData.append('TeamName', $('#TeamName').val());
            formData.append('TeamDescription', $('#TeamDescription').val());


            // Send an AJAX request to the backend
            $.ajax({
                type: 'POST',
                url: '/api/sitecore/Tournament/EditTournamentTeam',
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    if (response.success) {

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
