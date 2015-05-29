// Javascript used on the Foods page
var g_urls;

function foodlist_Init(options) {
    g_urls = options;
}

function foodlist_processLinkClick(p_linkData) {
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
function foodlist_setEvents() {
    //when a link is clicked
    $("a").on("click", function () {
        foodtracker_processLinkClick($(this).data("linkaction"));
    });
    //check that a number field is valid
    $(".number").change(function (event) {
        var field = $(event.currentTarget);
        field.removeClass("validateError");
        field.closest("#error").remove();
        if (field.var == "")
        {
            event.currentTarget.value = 0;
            return;
        }
        if (isNaN(event.currentTarget.value))
        {
            field.addClass("validateError");
            field.after("<p id=\"error\">*Error: Data is not a valid numeric value</p>")
        }

       
    });

    // Gets data from db and builds table containing matching data 
    $("#Search").click(function (e) {
        var data = {name: $("#searchName").val(), description: $("#searchDesc").val(), calories: $("#searchCalories").val(), sugar: $("#searchSugar").val(), fat: $("#searchFat").val(), saturates: $("#searchSaturates").val(), salt: $("#searchSalt").val()};
        if (data.name == "" && data.description == "" && data.calories == "" && data.sugar == "" && data.fat == "" && data.saturates == "" && data.salt == "")
        {
            
            var buttons = [{
                
                text: "Yes",
                class: 'btn btn-primary btn-xs',
                    click: function () {
                        $(this).dialog("close");
                        $("#FoodTable").load(g_urls.getAllUrl);
                        return;
                    }
            },
                {
                    text: "No",
                    class: 'btn btn-default btn-xs',
                click: function () {
                    $(this).dialog("close");
                    return;
                }
            }]
            foodtracker_createDialog("Perform Blank Retrieve?",'No Criteria Specified. This will retrieve all foods and may take a long time.</br> Are you sure you wish to continue?',buttons,{modal:true,resizable:false,dialogClass:"no-close",closeOnEscape:false});
            
        }
        else
        {
           $("#FoodTable").load(g_urls.searchUrl, data);
        }
        
    });

    
   
}


