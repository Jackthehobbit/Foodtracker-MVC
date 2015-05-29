// Contains functions to be used on multiple pages

function foodtracker_createDialog(title, html, buttons, options) {
    var dialog = $("#dialog");

    if (title == "")
    {
        title = "Dialog"
    }

    if (options.modal != false)
    {
        options.modal = true;
    }

    if (options.resizable != true)
    {
        options.resizable = false;
    }

    if (buttons == undefined)
    {
        buttons = {
            text: "Ok",
            click: function () {
                $(this).dialog("close");
            }
        }
    }
    options.width = 'auto';
    options.height = 'auto';
    options.buttons = buttons;
    options.dialogClass += " modal-content"
    
    dialog.attr('title', title);
    dialog.html(html);
    dialog.dialog(options
    );

}