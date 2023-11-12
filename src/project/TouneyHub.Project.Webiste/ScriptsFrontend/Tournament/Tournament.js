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
    // Step 1: Check for the presence of the unique identifier
    const tempTournamentIdentifier = document.cookie.replace(/(?:(?:^|.*;\s*)tempTournamentIdentifier\s*=\s*([^;]*).*$)|^.*$/, '$1');


    const uniqueIdentifier = generateUniqueIdentifier(); // Replace this with your logic to generate a unique identifier
    document.cookie = `tempTournamentIdentifier=${uniqueIdentifier}`;

    // Rest of your main page logic...

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
