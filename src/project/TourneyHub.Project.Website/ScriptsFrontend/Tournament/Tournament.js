$(document).ready(function () {
    $("input[name='tournamentType']").change(function () {
        const selectedType = $("input[name='tournamentType']:checked").val();
        if (selectedType === "Individual") {
            $("#individualParticipants").show();
            $("#teamSelection").hide();
        } else {
            $("#individualParticipants").hide();
            $("#teamSelection").show();
        }
    });

    $(document).ready(function () {
        $("#submitForm").click(function (e) {
            e.preventDefault();
            $.ajax({
                type: "POST",
                url: "/api/sitecore/Tournament/GetTournamentFormData",
                data: {
                    TournamentType: $("input[name='tournamentType']:checked").val(),
                    NumberOfParticipants: $("#numOfParticipants").val(),
                    NumberOfTeams: $("#numOfTeams").val(),
                    NumberOfMembersPerTeam: $("#numOfMembersPerTeam").val(),
                    TournamentFormat: $("#tournamentFormat").val(),
                    SportName: $("#sportName").val(),
                    TournamentName: $("#tournamentName").val(),
                },
                success: function (response) {
                    if (response.success) {
                        window.location.href = response.redirectUrl;
                    } else {
                        $("#errorAlert").text(response.message).show();
                    }
                },
                error: function () {
                    $("#errorAlert").text("An error occurred while processing your request.").show();
                }
            });

        });
    });

    // Step 1: Check for the presence of the unique identifier
    const tempTournamentIdentifier = document.cookie.replace(/(?:(?:^|.*;\s*)tempTournamentIdentifier\s*=\s*([^;]*).*$)|^.*$/, '$1');


    const uniqueIdentifier = generateUniqueIdentifier(); // Replace this with your logic to generate a unique identifier
    document.cookie = `tempTournamentIdentifier=${uniqueIdentifier}`;

    function generateUniqueIdentifier() {
        // Replace this with your logic to generate a unique identifier (e.g., using GUID)
        // For simplicity, using a timestamp here
        let timestamp = new Date().getTime().toString();

        // Ensure that the generated identifier follows Sitecore item naming conventions
        // Replace invalid characters with underscores, and limit the length
        const invalidCharactersRegex = /[^a-zA-Z0-9_]/g;
        const maxIdentifierLength = 50; // You can adjust this based on your requirements

        let cleanedIdentifier = timestamp.replace(invalidCharactersRegex, '_').substring(0, maxIdentifierLength);

        return cleanedIdentifier;
    }



});
