$(document).ready(function () {
    $('#registerForm').submit(function (e) {
        e.preventDefault(); 

        let errorMessage = '';
        const email = $('#email').val();
        const password = $('#password').val();
        const repeatPassword = $('#repeatpassword').val();
        const username = $('#username').val();
        const surname = $('#surname').val();

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            errorMessage += 'Invalid email address.<br>';
            $('#email').addClass('is-invalid');
        } else {
            $('#email').removeClass('is-invalid');
        }

        if (password.length < 6) {
            errorMessage += 'Password must be at least 6 characters long.<br>';
            $('#password').addClass('is-invalid');
        } else {
            $('#password').removeClass('is-invalid');
        }

        if (password !== repeatPassword) {
            errorMessage += 'Passwords do not match.<br>';
            $('#repeatpassword').addClass('is-invalid');
        } else {
            $('#repeatpassword').removeClass('is-invalid');
        }

        if (!username.trim()) {
            errorMessage += 'Username is required.<br>';
            $('#username').addClass('is-invalid');
        } else {
            $('#username').removeClass('is-invalid');
        }

        if (!surname.trim()) {
            errorMessage += 'Surname is required.<br>';
            $('#surname').addClass('is-invalid');
        } else {
            $('#surname').removeClass('is-invalid');
        }

        const errorAlert = $('#errorAlert');
        errorAlert.html(errorMessage);

        if (errorMessage) {
            errorAlert.show();
        } else {
            errorAlert.hide(); 
        }

        if (!errorMessage) {
            const formData = $(this).serialize();
            $.ajax({
                type: 'POST',
                url: '/api/sitecore/Registration/UserData',
                data: formData,
                success: function (response) {
                    // Handle the response here
                    if (response.success) {
                        console.log('Form submitted successfully');
                        window.location.href = "https://tourneyhub.sc/Login";
                        console.error('Form submission failed');
                        errorAlert.html(response.message);
                        errorAlert.show();
                    }
                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });
        }
    });
});
