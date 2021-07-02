

var GLOBAL_FLOORMAPS = [];

var SmartFloorMap = function (target, resize) {

    this.Layers = new Array();
    this.Target = target;
    this.CurrentLayerKey = '';
    $(this.Target).data('sfm_instance', this);
    this.AutoRotate = false;
    this.AutoRotateSeconds = 5;
    this.AutoRotateFunc = '';
    this.ResizeMultiplier = resize;



    this.AddIcon = function(layerid, className, x, y)
    {
        if (this.Layers[layerid] == undefined) {
            this.Layers[layerid] = new Array();
        }

        this.Layers[layerid].push({ x: x, y: y, className: className });
    };

    this.RenderLayer = function (layerid) {

        this.CurrentLayerKey = layerid;

        var on_sfm_render_layer = $(this.Target).data('sfm_on_render_layer');

        if (typeof on_sfm_render_layer == 'function') {
            on_sfm_render_layer(layerid);
        }

        $(this.Target).find('.sfm_icon').remove();

        for (var i = 0; i < this.Layers[layerid].length; i++) {
 
            var icon = $("<div />");
            $(this.Target).append(icon);

            icon.addClass('pulse');
            icon.addClass('sfm_icon');
            icon.addClass(this.Layers[layerid][i]["className"]);
            icon.css('position', 'absolute');
            icon.css('top', this.Layers[layerid][i]["y"] * this.ResizeMultiplier - 15);
            icon.css('left', this.Layers[layerid][i]["x"] * this.ResizeMultiplier + 5);
        }

    }

    this.AutoRotateLayers = function (seconds) {

        this.AutoRotate = true;
        this.AutoRotateSeconds = seconds;

        var myId = "#" + $(this.Target).attr('id');
        this.AutoRotateFunc = "$('" + myId + "').data('sfm_instance').NextLayer()";

        sCoreExiTaskmgr.scheduleTask(myId, this.AutoRotateFunc, this.AutoRotateSeconds);
    }



    this.NextLayer = function()
    {
        var keys = Object.keys(this.Layers);
        var index = jQuery.inArray(this.CurrentLayerKey, keys);

        if (index < 0 || index >= keys.length -1 ) {
            this.RenderLayer(keys[0]);
        }
        else {
            this.RenderLayer(keys[index + 1]);
        }

        if (this.AutoRotate) {
            var myId = "#" + $(this.Target).attr('id');
            sCoreExiTaskmgr.markTaskComplete(myId);
            sCoreExiTaskmgr.scheduleTask(myId, this.AutoRotateFunc, this.AutoRotateSeconds);
        }
    }

};
