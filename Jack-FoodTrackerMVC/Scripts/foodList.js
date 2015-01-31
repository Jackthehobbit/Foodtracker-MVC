$(document).ready(function () {
//set any events needed for the screen
foodtracker_setEvents();

});
function foodtracker_processLinkClick(p_linkData) {
    switch (p_linkData.toLowerCase) {
        case "edit":
            alert("EDIT CLICK TO BE DONE");
            break;
        case "delete":
            alert("Delete Click To Be Done");
            break;
        case "details":
            alert("Details Click to be Done");
            break;
        default:
            return;
            break;
    }
    event.preventDefault();
}
function foodtracker_setEvents() {
    //when a link is clicked
    $("a").on("click", function () {
        foodtracker_processLinkClick($(this).data("linkaction"));
    });
    //check that a number field is valid
    $(".number").change(function (event) {
        var field = $(event.currentTarget);
        field.removeClass("validateError");
        if(field.var == "")
        {
            event.currentTarget.value = 0;
            return;
        }
        if (isNaN(event.currentTarget.value))
        {
            field.addClass("validateError");
        }
    });
}


