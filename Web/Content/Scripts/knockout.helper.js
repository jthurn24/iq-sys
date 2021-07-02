
var koHelper = {

    setModel: function (model) {
        koHelper._Model = model;
    },

    setSourceData: function (source) {
        koHelper._Source = source;
    },

    getModel: function () {
        return koHelper._Model;
    },

    getSourceData: function () {
        return koHelper._Source;
    },

    ensureArray: function (propertyName, model) {
        if (!model[propertyName]) {
            model[propertyName] = new Array();
        }
    },

    addMapping: function (propertyName, className, model) {
        koHelper.ensureArray('_mappings', model);

        var func;
        eval('func = function(options) { return new ' + className + '(options.data,options.parent);  }');

        model._mappings[propertyName] = {
            create: func
        };
    },

    mapJS: function (data, model) {
        ko.mapping.fromJS(data, model._mappings, model);
    },

    addChildArrayHelpers: function (model, parent) {
        model.Parent = parent;

        model.Index = ko.dependentObservable(
            function () {
                return ko.utils.arrayIndexOf(this.Parent, this)
            },
            model);
    },

    generateID: function () {
        var timeStamp = new Date().getTime();
        return 'N' + timeStamp;
    },

    getEventTarget: function (e) {
        var targ;
        if (e.target) targ = e.target;
        else if (e.srcElement) targ = e.srcElement;
        return targ;
    },

    collectFieldValues: function (e) {

        var data = [];

        var valueInputs = $(e).find('input[type=text],select');
        var checkInput = $(e).find('input[type=checkbox]');


        for (i = 0; i < valueInputs.length; i++) {
            var key = $(valueInputs[i]).attr('name');
            var value = $(valueInputs[i]).val();
            data.push({ key: key, value: value });
        }

        for (i = 0; i < checkInput.length; i++) {
            var key = $(checkInput[i]).attr('name');
            var value = $(checkInput[i]).is(':checked').toString();
            data.push({ key: key, value: value });
        }

        return data;
    },

    clearFieldValues: function (e) {

        var valueInputs = $(e).find('input[type=text],select');
        var checkInput = $(e).find('input[type=checkbox]');

        for (i = 0; i < valueInputs.length; i++) {
            $(valueInputs[i]).val('');
        }

        for (i = 0; i < checkInput.length; i++) {
            $(checkInput[i]).removeAttr('checked');
        }
    }
};

ko.bindingHandlers.boolValue = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
       
        var value = valueAccessor(), allBindings = allBindingsAccessor();
        var valueUnwrapped = ko.utils.unwrapObservable(value);

        if (valueUnwrapped)
            $(element).val('true') 
        else
            $(element).val('false')
    }
};

ko.bindingHandlers.fadeVisible = {
    init: function (element, valueAccessor) {
        // Initially set the element to be instantly visible/hidden depending on the value
        var value = valueAccessor();
        $(element).toggle(ko.utils.unwrapObservable(value)); // Use "unwrapObservable" so we can handle values that may or may not be observable
    },
    update: function (element, valueAccessor) {
        // Whenever the value subsequently changes, slowly fade the element in or out
        var value = valueAccessor();
        ko.utils.unwrapObservable(value) ? $(element).fadeIn() : $(element).fadeOut();
    }
};

ko.bindingHandlers.slideOutVisible = {
    init: function (element, valueAccessor) {
        // Initially set the element to be instantly visible/hidden depending on the value
        var value = valueAccessor();
        $(element).toggle(ko.utils.unwrapObservable(value)); // Use "unwrapObservable" so we can handle values that may or may not be observable
    },
    update: function (element, valueAccessor) {
        // Whenever the value subsequently changes, slowly fade the element in or out
        var value = valueAccessor();
        ko.utils.unwrapObservable(value) ? $(element).show() : $(element).slideUp();
    }
};