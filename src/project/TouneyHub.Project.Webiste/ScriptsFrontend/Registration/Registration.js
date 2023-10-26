$(document).ready(function () {
    // Add a submit event listener to the form
    $('#registerForm').submit(function (e) {
        e.preventDefault(); // Prevent the default form submission

        // Initialize an empty error message
        let errorMessage = '';

        // Perform client-side validation
        const email = $('#email').val();
        const password = $('#password').val();
        const repeatPassword = $('#repeatpassword').val();
        const username = $('#username').val(); // Get the username
        const surname = $('#surname').val(); // Get the surname

        // Check if the email is valid (simple regex check)
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            // Show an error message for invalid email
            errorMessage += 'Invalid email address.<br>';
            $('#email').addClass('is-invalid');
        } else {
            $('#email').removeClass('is-invalid');
        }

        // Check if the password meets your criteria (e.g., minimum length)
        if (password.length < 6) {
            // Show an error message for invalid password
            errorMessage += 'Password must be at least 6 characters long.<br>';
            $('#password').addClass('is-invalid');
        } else {
            $('#password').removeClass('is-invalid');
        }

        // Check if the passwords match
        if (password !== repeatPassword) {
            // Show an error message for password mismatch
            errorMessage += 'Passwords do not match.<br>';
            $('#repeatpassword').addClass('is-invalid');
        } else {
            $('#repeatpassword').removeClass('is-invalid');
        }

        // Check if username and surname are not empty
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

        // Display the error message in the alert div
        const errorAlert = $('#errorAlert');
        errorAlert.html(errorMessage);

        // Check if there are errors and show/hide the alert div accordingly
        if (errorMessage) {
            errorAlert.show(); // Show the error alert if there are errors
        } else {
            errorAlert.hide(); // Hide the error alert if there are no errors
        }

        if (!errorMessage) {
            // No validation errors, proceed with form submission
            // Serialize the form data
            const formData = $(this).serialize();

            // Send a POST request to the backend
            $.ajax({
                type: 'POST',
                url: '/api/sitecore/Registration/UserData',
                data: formData,
                success: function (response) {
                    // Handle the response here
                    if (response.success) {
                        console.log('Form submitted successfully');
                        window.location.href = "https://tourneyhub.sc/Login"; // Replace with the desired URL
                        // You can redirect the user or show a success message
                    } else {
                        console.error('Form submission failed');
                        // Handle errors and show error messages
                        errorAlert.html(response.message);
                        errorAlert.show(); // Show the error alert
                    }
                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });
        }
    });
});
