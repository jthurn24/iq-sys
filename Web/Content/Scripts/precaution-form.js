
var PrecautionFormModel = function (types,root,updateAction,deleteAction,getAction) {

    this.precautionTypes = types;
    this.productIdCtrl = $(root).find('#ProductId');
    this.patientIdCtrl = $(root).find('#PatientId');
    this.guidCtrl = $(root).find('#Guid');
    this.typeIdCtrl = $(root).find('#PrecautionTypeId');
    this.endDateCtrl = $(root).find('#EndDate');
    this.endDateSpan = $(root).find('#EndDateSpan');
    this.endCheckBox = $(root).find('#Ended');
    this.startDateCtrl = $(root).find('#StartDate');
    this.endedCtrl = $(root).find('#Ended');
    this.additionalDescriptionCtrl = $(root).find('#AdditionalDescription');
    this.saveCtrl = $(root).find('#SaveButton');
    this.removeCtrl = $(root).find('#RemoveButton');
    this.root = root;
    this.updateAction = updateAction;
    this.deleteAction = deleteAction;
    this.getAction = getAction;
    this.removePrecautionSpan = $(root).find('#RemovePrecautionSpan');
    

    this.showPrecautionTypes = function () {

        var selectedProduct = $(this.productIdCtrl).val();
        $(this.typeIdCtrl).find('option').remove();

        for (i = 0; i < this.precautionTypes.length; i++) {
            if (this.precautionTypes[i]["ProductId"] == selectedProduct) {

                var opt = $("<option></option>");
                opt.attr("value", this.precautionTypes[i]["PrecautionId"]);
                opt.html(this.precautionTypes[i]["PrecautionName"]);

                $(this.typeIdCtrl).append(opt);

            }
        }

    };

    this.showProcessing = function()
    {

    }

    this.hideProcessing = function()
    {

    }

    this.displayDialog = function(title)
    {
        $(this.root).dialog({
            resizable: false,
            height: 365,
            width: 750,
            modal: true,
            title: title
        });
    }

    this.updateEndDate = function()
    {
        if ((this.endCheckBox).attr("checked"))
        {
            this.endDateSpan.show();
        }
        else
        {
            this.endDateSpan.hide();
            this.endDateCtrl.val('');
        }
    }

    this.showAdd = function()
    {
        this.guidCtrl.val("");
        this.typeIdCtrl.val("");
        this.productIdCtrl.val("");
        this.endDateCtrl.val("");
        this.startDateCtrl.val(this.formatDate(new Date()));
        this.additionalDescriptionCtrl.val("");
        this.productIdCtrl.show();
        this.displayDialog("Add Precaution");
        this.typeIdCtrl.prop("disabled", false);
        this.showPrecautionTypes();
        this.removePrecautionSpan.hide();
    }

    this.showEdit = function(id)
    {
        this.showProcessing();

        $.ajax({
            context: this,
            type: "get",
            url: this.getAction + '?id=' + id,
            success: function(data) {
                this.hideProcessing();
                this.guidCtrl.val(data["Guid"]);
                this.productIdCtrl.val(data["ProductId"]);

                if (data["EndDate"] != '') {
                    var endDate = (new moment(data["EndDate"])).toDate();
                    this.endDateCtrl.val(this.formatDate(endDate));
                    $(this.endCheckBox).prop("checked", true)
                    this.endDateSpan.show();
                }
                else {
                    this.endDateCtrl.val('');
                    $(this.endCheckBox).prop("checked", false)
                    this.endDateSpan.hide();
                }

                var startDate = (new moment(data["StartDate"])).toDate();
                this.startDateCtrl.val(this.formatDate(startDate));
                this.patientIdCtrl.val(data["PatientId"]);
                this.additionalDescriptionCtrl.val(data["AdditionalDescription"]);
                this.showPrecautionTypes();
                this.typeIdCtrl.val(data["PrecautionTypeId"]);
                this.productIdCtrl.hide();
                this.typeIdCtrl.prop("disabled", true);
                this.displayDialog("Edit Precaution");
                this.removePrecautionSpan.show();

            }
        });

    }

    this.save = function()
    {
        var data = {
            Guid: this.guidCtrl.val(),
            PrecautionTypeId: this.typeIdCtrl.val(),
            ProductId: this.productIdCtrl.val(),
            StartDate: this.startDateCtrl.val(),
            EndDate: this.endDateCtrl.val(),
            PatientId: this.patientIdCtrl.val(),
            AdditionalDescription: this.additionalDescriptionCtrl.val()
        }

        this.showProcessing();

        $.ajax({
            context: this,
            type: "post",
            url: this.updateAction,
            success: this.saveSuccess,
            error: this.saveError,
            data: data
        });
    }



    this.onSaveSuccess = function()    {    }

    this.saveSuccess = function(data)
    {
        if (data['Success'] == false)
        {
            alert(data['Message']);
            return;
        }

        this.hideProcessing();
        this.close();
        this.onSaveSuccess();
    }

    this.close = function()
    {
      $(this.root).dialog("close");
    }

    this.remove = function()
    {


        $.ajax({
            context: this,
            type: "get",
            url: this.deleteAction,
            success: this.saveSuccess,
            error: this.saveError,
            data: { id: this.guidCtrl.val() }
        });
    }

    this.promptRemove = function () {

        var prompt = new SmartPrompt();
        prompt.setContext(this);
        prompt.addButton("Yes", function () { this.remove(); })
        prompt.addButton("No", function () { });
        prompt.setPrompt("Are you sure you want to remove this precaution");
        prompt.show();
    }

    this.saveError = function(jqXHR, textStatus, errorThrown)
    {
        alert(errorThrown);
        this.hideProcessing();
    }

    this.formatDate = function (d) {
        return (d.getMonth() + 1) +
        "/" + d.getDate() +
        "/" + d.getFullYear();
    }

    $(this.productIdCtrl).change($.proxy(function () { this.showPrecautionTypes(); }, this));
    $(this.endCheckBox).change($.proxy(function () { this.updateEndDate(); }, this));
    $(this.saveCtrl).click($.proxy(function () { this.save(); }, this));
    $(this.removeCtrl).click($.proxy(function () { this.promptRemove(); }, this));
}
