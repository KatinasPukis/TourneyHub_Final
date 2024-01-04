function deleteTournament(tournamentId) {
    if (confirm("Are you sure you want to delete this tournament?")) {
        $.ajax({
            type: 'DELETE',
            url: '/api/sitecore/Tournament/DeleteTournament',
            data: { tournamentId: tournamentId },
            success: function (response) {
                if (response.success) {
                    window.location.reload();
                } else {
                    alert("Failed to delete tournament: " + response.message);
                }
            },
            error: function (error) {
                alert("An error occurred while communicating with the server");
            }
        });
    }
}