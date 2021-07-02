
var SmartPrompt = function () {

    this.buttons = {};
    this.prompt = '';
    this.context = null;


    this.setPrompt = function(p)
    {
        this.prompt = p;
    }

    this.setContext = function(c)
    {
        this.context = c;
    }

    this.addButton = function(name,func)
    {
        var funcWrapper = $.proxy(function () {
            func.apply(this.context);
            $("#confirm-dialog").dialog("close");
        }, this);

        this.buttons[name] = funcWrapper;
    }

    this.show = function()
    {
        $('#confirm-dialog-content').html('<img src="/Content/Images/prompt.jpg" style="float:left;">' + this.prompt);

        $("#confirm-dialog").dialog({
            resizable: false,
            height: 180,
            width: 450,
            modal: true,
            buttons: this.buttons
        });

        $('#confirm-dialog').siblings(".ui-dialog-titlebar").hide();
    }

}