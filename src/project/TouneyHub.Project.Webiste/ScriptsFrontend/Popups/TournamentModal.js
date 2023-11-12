$(document).ready(function () {


    var maxScoreColumns = 5; // Maximum number of Score columns


    //-------------------Modal Open Close-----------------------
    // Handle click on the edit buttons in participant-match
    $('.participant-match .edit-button').click(function () {
        $('#participantModal').modal('show');
    });

    // Handle click on the edit buttons in team-match
    $('.team-match .edit-button').click(function () {
        $('#teamModal').modal('show');
    });

    // Handle click on the close button in participant-modal
    $('#participantModal .close-modal-button').click(function () {
        $('#participantModal').modal('hide');
    });

    // Handle click on the close button in team-modal
    $('#teamModal .close-modal-button').click(function () {
        $('#teamModal').modal('hide');
    });
    //-------------------Modal Open Close-----------------------

    //-------------------Team Add Score--------------------------
    $('#teamModal th#addTeamScoreButton').click(function () {
        // Find the table body where new "Score" columns will be added
        var tableBody = $('#teamModal table tbody');

        // Create a new "Score" column for each row in the table
        tableBody.find('tr').each(function () {
            // Add a cell for the "Score" input
            var newScoreColumn = $('<td><input type="number" class="form-control score-input" placeholder="Score"></td>');

            // Append the new "Score" column to the row
            $(this).append(newScoreColumn);
        });

        // Find the header row and count existing "Score" columns
        var headerRow = $('#teamModal table thead tr');
        var scoreColumnsCount = headerRow.find('th:contains("Score")').length;

        // Create a new "Score" header and append it to the header row
        var newScoreHeader = '<th>Score</th>';
        headerRow.find('th#addTeamScoreButton').before(newScoreHeader);

        // If there are already 5 "Score" columns, disable the "Add Score" button
        if (scoreColumnsCount >= maxScoreColumns) {
            $('#teamModal th#addTeamScoreButton').prop('disabled', true);
        }
    });
    //-------------------Team Add Score--------------------------

    //-------------------Participant Add Score--------------------------
    $('#participantModal th#addParticipantScoreButton').click(function () {
        // Find the table body where new "Score" columns will be added
        var tableBody = $('#participantModal table tbody');

        // Create a new "Score" column for each row in the table
        tableBody.find('tr').each(function () {
            // Add a cell for the "Score" input
            var newScoreColumn = $('<td><input type="number" class="form-control score-input" placeholder="Score"></td>');

            // Append the new "Score" column to the row
            $(this).append(newScoreColumn);
        });

        // Find the header row and count existing "Score" columns
        var headerRow = $('#participantModal table thead tr');
        var scoreColumnsCount = headerRow.find('th:contains("Score")').length;

        // Create a new "Score" header and append it to the header row
        var newScoreHeader = '<th>Score</th>';
        headerRow.find('th#addParticipantScoreButton').before(newScoreHeader);

        // If there are already 5 "Score" columns, disable the "Add Score" button
        if (scoreColumnsCount >= maxScoreColumns) {
            $('#participantModal th#addParticipantScoreButton').prop('disabled', true);
        }
    });
    //-------------------Participant Add Score--------------------------

    //-------------------Participant Save Data--------------------------
    // Add this code to the end of your existing JavaScript

    $(document).ready(function () {
        // ... (Your existing code)

        //-------------------Participant Save Data--------------------------
        $('#participantModal .btn-primary').click(function () {
            var matchId = $('#participantModal').attr('data-match-id');
            var scores = [];

            $('#participantModal table tbody tr').each(function () {
                var rowScores = [];
                var participantId = $(this).find('td[data-participant-id]').data('participant-id');

                // Check if the participant is the winner
                var isWinner = $('#winnerSelect').val() === participantId;

                $(this).find('.score-input').each(function () {
                    rowScores.push($(this).val());
                });

                scores.push({ participantId: participantId, scores: rowScores, isWinner: isWinner });
            });

            // Get the selected participant's ID based on the selected option in the winnerSelect dropdown
            var winnerId = $('#winnerSelect').find('option:selected').data('participant-id');

            var data = {
                matchId: matchId,
                scores: scores,
                winnerId: winnerId
            };

            $.ajax({
                type: "POST",
                url: "/api/sitecore/Tournament/ParticipantMatch",
                data: JSON.stringify(data),
                contentType: "application/json",
                success: function (response) {
                    console.log(response);

                    // Reload the page after a short delay (adjust the delay as needed)
                    setTimeout(function () {
                        location.reload();
                    }, 1000); // 1000 milliseconds (1 second)
                },
                error: function (error) {
                    console.error(error);
                }
            });
        });
        //-------------------Participant Save Data--------------------------

        // ... (Your existing code)
    });


    //-------------------Participant Save Data--------------------------

    //--------------------------Team Save Data--------------------------

    $('#teamModal .btn-primary').click(function () {
        var matchId = $('#teamModal').attr('data-match-id');
        var scores = [];

        $('#teamModal table tbody tr').each(function () {
            var rowScores = [];
            var participantId = $(this).find('td[data-participant-id]').data('participant-id');

            // Check if the participant is the winner
            var isWinner = $('#winnerSelect').val() === participantId;

            $(this).find('.score-input').each(function () {
                rowScores.push($(this).val());
            });

            scores.push({ participantId: participantId, scores: rowScores, isWinner: isWinner });
        });

        // Get the selected participant's ID based on the selected option in the winnerSelect dropdown
        var winnerId = $('#winnerSelect').find('option:selected').data('participant-id');

        var data = {
            matchId: matchId,
            scores: scores,
            winnerId: winnerId
        };

        $.ajax({
            type: "POST",
            url: "/api/sitecore/Tournament/TeamMatch",
            data: JSON.stringify(data),
            contentType: "application/json",
            success: function (response) {
                console.log(response);

                // Reload the page after a short delay (adjust the delay as needed)
                setTimeout(function () {
                    location.reload();
                }, 1000); // 1000 milliseconds (1 second)
            },
            error: function (error) {
                console.error(error);
            }
        });
    });
        //-----

    //--------------------------Team Save Data--------------------------

    // -----------------------------Participant Data FillUp-----------------------------------------
    $('.participant-match .edit-button').click(function () {
        var matchId = $(this).closest('.participant-match').data('participant-match-id');
        fetchParticipantMatchData(matchId);
    });

    function fetchParticipantMatchData(matchId) {
        $.ajax({
            type: "GET",
            url: `/api/sitecore/Tournament/GetParticipantMatchData?matchId=${matchId}`,
            success: function (response) {
                populateModalWithData(response, matchId);
            },
            error: function (error) {
                console.error(error);
            }
        });
    }

    function populateModalWithData(data, matchId) {
        var modal = $('.modal.fade');

        modal.attr('data-match-id', matchId);

        $('#participant1Name').text(data.firstParticipantName);
        $('#participant1Name').attr('data-participant-id', data.firstParticipantId);

        $('#participant2Name').text(data.secondParticipantName);
        $('#participant2Name').attr('data-participant-id', data.secondParticipantId);

        $('#participant1Selection').attr('data-participant-id', data.firstParticipantId);
        $('#participant2Selection').attr('data-participant-id', data.secondParticipantId);

        $('#winnerSelect').empty();
        $('#winnerSelect').append(`<option data-participant-id="${data.firstParticipantId}"  value="${data.firstParticipantName}">${data.firstParticipantName}</option>`);
        $('#winnerSelect').append(`<option data-participant-id="${data.secondParticipantId}" value="${data.secondParticipantName}">${data.secondParticipantName}</option>`);

    }
    // -----------------------------Participant Data FillUp-----------------------------------------

    //----------------------------- Team Data FillUp-----------------------------------------
    $('.team-match .edit-button').click(function () {
        var matchId = $(this).closest('.team-match').data('team-match-id');
        fetchTeamMatchData(matchId);
    });
    function fetchTeamMatchData(matchId) {
        $.ajax({
            type: "GET",
            url: `/api/sitecore/Tournament/GetTeamMatchData?matchId=${matchId}`,
            success: function (response) {
                populateModalWithData(response, matchId);
            },
            error: function (error) {
                console.error(error);
            }
        });
    }
    //----------------------------- Team Data FillUp-----------------------------------------
    //-------------------------------Delete Result--------------------------------------------
    $('.participant-match .delete-result').click(function () {
        var matchId = $(this).closest('.participant-match').data('participant-match-id');

        // Extracting firstParticipantId using attr method
        var firstParticipantId = $(this).closest('.participant-match').find('.participant[data-participant-type="first"]').attr('data-participant-id');
        console.log('First Participant id: ', firstParticipantId);

        // Extracting secondParticipantId using attr method
        var secondParticipantId = $(this).closest('.participant-match').find('.participant[data-participant-type="second"]').attr('data-participant-id');
        console.log('Second Participant id: ', secondParticipantId);

        // Create a data object to send to the backend
        var data = {
            matchId: matchId,
            firstParticipantId: firstParticipantId,
            secondParticipantId: secondParticipantId
        };

        // Rest of your AJAX request remains unchanged
        $.ajax({
            type: "POST",
            url: "/api/sitecore/Tournament/DeleteResult",
            data: JSON.stringify(data),
            contentType: "application/json",
            success: function (response) {
                console.log(response);
                setTimeout(function () {
                    location.reload();
                }, 1000);
            },
            error: function (error) {
                console.error(error);
            }
        });
    });

    $('.team-match .delete-result').click(function () {
        var matchId = $(this).closest('.team-match').data('team-match-id');
        var firstParticipantId = $(this).closest('.team-match').find('.participant[data-participant-type="first"]').data('participant-id');
        console.log('First Participant id: ', firstParticipantId);
        var secondParticipantId = $(this).closest('.team-match').find('.participant[data-participant-type="second"]').data('participant-id');
        console.log('second Participant id: ', secondParticipantId);


        // Create a data object to send to the backend
        var data = {
            matchId: matchId,
            firstParticipantId: firstParticipantId,
            secondParticipantId: secondParticipantId
        };

        // Send an AJAX request to the backend
        $.ajax({
            type: "POST", // Adjust the HTTP method as needed
            url: "/api/sitecore/Tournament/DeleteResult", // Adjust the URL to your backend endpoint
            data: JSON.stringify(data),
            contentType: "application/json",
            success: function (response) {
                // Handle the response from the backend, if needed
                console.log(response);
                setTimeout(function () {
                    location.reload();
                }, 1000);
            },
            error: function (error) {
                // Handle errors, if any
                console.error(error);
            }
        });
    });
    

});
