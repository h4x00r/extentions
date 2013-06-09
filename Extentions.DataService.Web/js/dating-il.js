/// <reference path="$-1.8.2-vsdoc.js" />

var datingIL = (function ()
{
    var $ = null;
    var _datingIL = {};
    var _config = null;
    var _accessToken = null;

    _datingIL.init = function (jQuery, config)
    {
        $ = jQuery;

        $('#LEFT2').live({
            mouseenter: function ()
            {
                var userInfo = _extractUserLeft($(this));

                $(this).append(_createFbElement(
                {
                    width: 174
                }));

                $(this).find('.reveal-facebook-profie').click(function ()
                {
                    var image = $('<img/>')
                            .attr('src', 'https://extentions.apphb.com/static/get-image?name=loader.gif')
                            .css('position', 'absolute')
                            .css('top', '25px')
                            .css('left', '75px');

                    var holder = $(this);

                    $(this).html('מאתר פרופיל');
                    $(this).animate({ height: '60px' }, 400);
                    $(this).append(image);

                    _retrieveFacebookUrl(userInfo, function (response)
                    {
                        $(holder).animate({ height: '24px' }, 400, '', function ()
                        {
                            $(holder).html(_createFbLink(response.facebook_link));
                        });
                    },
                    function ()
                    {
                        $(holder).animate({ height: '24px' }, 400, '', function ()
                        {
                            $(holder).html(_createFbError());
                        });
                    });
                });
            },
            mouseleave: function ()
            {
                $(this).find('.reveal-facebook-profie').remove();
            }
        });

        $('.personCard').live({
            mouseenter: function ()
            {
                var userInfo = _extractUser($(this));

                $(this).find('.userimg').append(_createFbElement(
                {
                    width: 118
                }));

                $(this).find('.reveal-facebook-profie').click(function ()
                {
                    var image = $('<img/>')
                            .attr('src', 'https://extentions.apphb.com/static/get-image?name=loader.gif')
                            .css('position', 'absolute')
                            .css('top', '25px')
                            .css('left', '45px');

                    var holder = $(this);

                    $(this).html('מאתר פרופיל');
                    $(this).animate({ height: '60px' }, 400);
                    $(this).append(image);

                    _retrieveFacebookUrl(userInfo, function (response)
                    {
                        $(holder).animate({ height: '24px' }, 400, '', function ()
                        {
                            $(holder).html(_createFbLink(response.facebook_link));
                        });
                    },
                    function ()
                    {
                        $(holder).animate({ height: '24px' }, 400, '', function ()
                        {
                            $(holder).html(_createFbError());
                        });
                    });
                });
            },
            mouseleave: function ()
            {
                $(this).find('.reveal-facebook-profie').remove();
            }
        });
    };

    var _getAccessToken = function ()
    {
        FB.getLoginStatus(function (response)
        {
            if (response.status === 'connected')
                _accessToken = response.authResponse.accessToken;
        });
    }

    var _extractUser = function (userHtml)
    {
        _getAccessToken();

        var pictureUrl = $(userHtml).find('.userimg img').attr('src');
        var userInfo = $(userHtml).find('.infoHolder').attr('id');

        return { picture_url: pictureUrl, user_info: userInfo, access_token: _accessToken };
    }

    var _extractUserLeft = function (userHtml)
    {
        _getAccessToken();

        var pictureUrl = $(userHtml).find('img').attr('src');

        return { picture_url: pictureUrl, access_token: _accessToken };
    }

    var _retrieveFacebookUrl = function (userInfo, onComplete, onError)
    {
        var jsonString = $.toJSON(userInfo);
        var jsonStringEncoded = base64.encode(jsonString);

        $.jsonp({
            url: 'https://extentions.apphb.com/dating-il/get-facebook-profile',
            cache: false,
            data: { data: jsonStringEncoded },
            callbackParameter: 'callback',
            success: function (data, textStatus)
            {
                setTimeout(function () { onComplete(data); }, 1000);
            },
            error: function (xOptions, textStatus)
            {
                onError()
            }
        });
    }

    var _createFbElement = function (params)
    {
        var result = $('<div>פייסבוק</div>')
                    .addClass('reveal-facebook-profie')
                    .css('width', params.width + 'px')
                    .css('height', '24px')
                    .css('background-color', '#3b5998')
                    .css('position', 'absolute')
                    .css('top', '0px')
                    .css('left', '0px')
                    .css('color', '#ffffff')
                    .css('font-family', 'Tahoma, Geneva, sans-serif')
                    .css('text-align', 'center')
                    .css('font-size', '15px')
                    .css('font-weight', 'bold');

        return result;
    }

    var _createFbLink = function (link)
    {
        var div = $('<div></div>')

        var result = $('<a></a>')
                    .html('פרופיל')
                    .attr('href', link)
                    .attr('target', '_blank')
                    .css('color', '#ffffff')
                    .css('outline', 'none')
                    .css('-moz-outline-style', 'none')
                    .css('font-family', 'Tahoma, Geneva, sans-serif')
                    .css('text-align', 'center')
                    .css('font-size', '15px')
                    .css('font-weight', 'bold');

        div.append(result);

        return div;
    }

    var _createFbError = function ()
    {
        var div = $('<div></div>')

        var result = $('<a></a>')
                    .html('שגיאה')
                    .attr('target', '_blank')
                    .css('color', '#ffffff')
                    .css('font-family', 'Tahoma, Geneva, sans-serif')
                    .css('text-align', 'center')
                    .css('font-size', '15px')
                    .css('font-weight', 'bold');

        div.append(result);

        return div;
    }

    return _datingIL;
} ());