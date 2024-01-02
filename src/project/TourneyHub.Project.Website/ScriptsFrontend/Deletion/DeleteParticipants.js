////function removeParticipant(participantId) {
////    var confirmation = confirm("Are you sure you want to delete the selected participant?");

////    if (confirmation) {
////        $.ajax({
////            type: 'DELETE',
////            url: '/api/sitecore/Tournament/DeleteParticipant',
////            data: { participantId: participantId },
////            success: function (data) {
////                location.reload();
////            },
////            error: function (error) {
////                console.error('Error removing participant:', error);
////            }
////        });
////    }
////}

////function removeTeam(teamId) {
////    var confirmation = confirm("Are you sure you want to delete the selected team ?");

////    if (confirmation) {
////        $.ajax({
////            type: 'DELETE',
////            url: '/api/sitecore/Tournament/DeleteTeam',
////            data: { teamId: teamId },
////            success: function (data) {
////                location.reload();
////            },
////            error: function (error) {
////                console.error('Error removing team:', error);
////            }
////        });
////    }
////}