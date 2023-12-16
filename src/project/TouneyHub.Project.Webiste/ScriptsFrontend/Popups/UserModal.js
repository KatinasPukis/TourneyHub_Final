$(document).ready(function () {
    $('#editUserButton').click(function () {
        var Id = $('#user-card').data('user-id');
        console.log(Id);
        $('#editUserModal').data('user-id', Id);
        $('#editUserModal').modal('show');
    });

    $('#saveUserButton').click(function () {
        var Id = $('#editUserForm').data('user-id');;
        console.log(Id);
        if ($('#editUserForm')[0].checkValidity()) {
            var formData = new FormData($('#editUserForm')[0]);
            formData.append('Id', Id);
            formData.append('name', $('#name').val());
            formData.append('surname', $('#surname').val());
            formData.append('email', $('#email').val());
            formData.append('password', $('#password').val());
            formData.append('repeatpassword', $('#repeatpassword').val());

            $.ajax({
                type: 'POST',
                url: '/api/sitecore/Tournament/EditUserProfile',
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    if (response.success) {
                        window.location.reload();
                    } else {
                        alert(response.message);
                    }
                },
                error: function (error) {
                    alert("An error occurred while communicating with the server");
                }
            });
            $('#editUserModal').modal('hide');
        } else {
        }
    });
});
