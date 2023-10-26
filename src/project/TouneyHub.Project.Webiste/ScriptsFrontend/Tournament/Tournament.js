$(document).ready(function () {
    const form = $("#multiStepForm");
    const steps = form.find(".form-step");
    const nextButtons = form.find(".next-step");
    const prevButtons = form.find(".prev-step");
    let currentStep = 1;

    function updateStepIndicator() {
        const indicators = $(".step-indicator li");
        indicators.each(function (index) {
            if (index === currentStep - 1) {
                $(this).addClass("active");
            } else {
                $(this).removeClass("active");
            }
        });
    }

    steps.hide();
    steps.eq(currentStep - 1).show();
    updateStepIndicator();

    nextButtons.on("click", function (e) {
        e.preventDefault();

        if (currentStep < steps.length) {
            steps.eq(currentStep - 1).hide();
            currentStep++;
            steps.eq(currentStep - 1).show();
            updateStepIndicator();
        }
    });

    prevButtons.on("click", function (e) {
        e.preventDefault();

        if (currentStep > 1) {
            steps.eq(currentStep - 1).hide();
            currentStep--;
            steps.eq(currentStep - 1).show();
            updateStepIndicator();
        }
    });

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

    // AJAX form submission
    $("#submitForm").click(function (e) {
        e.preventDefault();
        const formData = form.serialize();
        $.ajax({
            type: "POST",
            url: "/api/sitecore/Tournament/GetTournamentFormData", // Updated controller action
            data: formData,
            success: function (response) {
                if (response.success) {
                    window.location.href = response.redirectUrl; // Redirect on successful submission
                } else {
                    // Display error message on failure
                    $("#errorAlert").text(response.message).show();
                }
            },
            error: function () {
                // Handle errors here
                $("#errorAlert").text("An error occurred while processing your request.").show();
            }
        });
    });
});
