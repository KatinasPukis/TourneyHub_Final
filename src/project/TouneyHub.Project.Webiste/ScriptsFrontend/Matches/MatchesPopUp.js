$(document).ready(function () {
    // Hide the error alert initially
    $("#errorAlert").hide();

    // Add a submit event listener to the form
    $("#loginForm").submit(function (event) {
        event.preventDefault(); // Prevent the default form submission

        console.log("Gathering login data");
        // Get the form data
        var formData = {
            username: $("#username").val(),
            password: $("#password").val()
        };

        // Make an AJAX request to the backend
        $.ajax({
            type: "POST",
            url: "/api/sitecore/Login/UserData",
            data: formData,
            success: function (response) {
                // Check the JSON response for success or failure
                if (response.success) {
                    // Add a timeout before redirecting
                    setTimeout(function () {
                        window.location.href = response.redirectUrl;
                    }, 1000); // Adjust the delay time (in milliseconds) as needed
                } else {
                    // Handle unsuccessful login (e.g., display an error message)
                    console.error("Login failed:", response.message);
                    // Display error message to the user
                    $("#errorAlert").text(response.message);
                    $("#errorAlert").show(); // Show the error alert
                }
            },
            error: function (error) {
                // Handle any errors that occur during the AJAX request
                console.error("Login request error:", error);
                // Display an error message for the AJAX error
                $("#errorAlert").text("An error occurred while processing your request.");
                $("#errorAlert").show(); // Show the error alert
            }
        });
    });
});
