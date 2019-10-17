window.interop = {
    getElementByName: function (name) {
        var elements = document.getElementsByName(name);
        if (elements.length) {
            return elements[0].value;
        } else {
            return "";
        }
    },
    submitForm: function (path, fields) {
        const form = document.createElement('form');
        form.method = 'post';
        form.action = path;

        for (const key in fields) {
            if (fields.hasOwnProperty(key)) {
                const hiddenField = document.createElement('input');
                hiddenField.type = 'hidden';
                hiddenField.name = key;
                hiddenField.value = fields[key];
                form.appendChild(hiddenField);
            }
        }

        document.body.appendChild(form);
        form.submit();
    },
    renderPickers: function () {
        $('.time-picker').AnyPicker({
            onInit: function () {
                apo = this;
            },
            onSetOutput: function () {              
                var event = new Event('change');
                this.elem.dispatchEvent(event);
            },
            mode: 'datetime',
            dateTimeFormat: 'HH:mm',
            lang: 'it',
        });
    },
    showSwallAlert: function (type, title, message, confirmBtnText, cancelBtnText, callback) {
        debugger
        swal({
            title: title,
            text: message,
            icon: type,
            buttons: {
                cancel: cancelBtnText != null ? cancelBtnText : false,
                confirm: confirmBtnText != null ? confirmBtnText : false,
            },
        }).then(callback);
    },
    hideOverlay: function (callback) {
        if ($('#overlay')) {
            $('#overlay').hide(callback);
        };
    },
    showOverlay: function (callback) {
        if ($('#overlay')) {
            $('#overlay').show(callback);
        };
    },
};



