$(document).ready(function () {
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

    // Handle click on the "Add Score" header in the Team Modal
    $(document).on('click', '#teamModal th.add-score-button', function () {
        // Find the table body where new "Score" columns will be added
        var tableBody = $('#teamModal table tbody');

        // Find the last row in the table
        var lastRow = tableBody.find('tr:last');

        // Clone the last "Score" column in the last row and append it
        var lastScoreColumn = lastRow.find('td:nth-child(2)');
        lastRow.append(lastScoreColumn.clone());

        // Add a new "Add Score" header for further additions
        var addScoreHeader = $('<th class="add-score-button" style="cursor: pointer;">Add Score</th>');
        lastRow.append(addScoreHeader);
    });

    // Add an initial "Score" column
    $('#teamModal th.add-score-button').click();
});
