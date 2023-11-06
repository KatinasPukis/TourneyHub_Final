function deleteTournament(tournamentId) {
    if (confirm("Are you sure you want to delete this tournament?")) {
        // Make an AJAX request to the backend API with the tournamentId
        $.ajax({
            type: 'DELETE', // Adjust the HTTP method as needed (POST, DELETE, etc.)
            url: '/api/sitecore/Tournament/DeleteTournament', // Update the URL to your actual API endpoint
            data: { tournamentId: tournamentId }, // Pass the tournamentId to the API
            success: function (response) {
                // Handle the success response, e.g., refresh the page or show a message
                if (response.success) {
                    alert("Tournament deleted successfully");
                    window.location.reload();
                    // Optionally, you can refresh the page or perform other actions here
                } else {
                    alert("Failed to delete tournament: " + response.message);
                }
            },
            error: function (error) {
                // Handle errors, e.g., show an error message
                alert("An error occurred while communicating with the server");
            }
        });
    }
}