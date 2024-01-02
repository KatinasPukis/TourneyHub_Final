$(document).ready(function () {
    $("#errorAlert").hide();
    $("#loginForm").submit(function (event) {
        event.preventDefault();

        console.log("Gathering login data");
        var formData = {
            username: $("#username").val(),
            password: $("#password").val()
        };
        $.ajax({
            type: "POST",
            url: "/api/sitecore/Login/UserData",
            data: formData,
            success: function (response) {
                if (response.success) {
                    setTimeout(function () {
                        window.location.href = response.redirectUrl;
                    }, 1000); 
                } else {
                    console.error("Login failed:", response.message);
                    $("#errorAlert").text(response.message);
                    $("#errorAlert").show();
                }
            },
            error: function (error) {
                console.error("Login request error:", error);
                $("#errorAlert").text("An error occurred while processing your request.");
                $("#errorAlert").show();
            }
        });
    });
});
