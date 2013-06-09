/// <reference path="$-1.8.2-vsdoc.js" />

var extentionsAds = (function ()
{
    var $ = null;
    var _ads = {};
    var _config = null;

    _ads.init = function (jQuery, config)
    {
        $ = jQuery;
        _config = config;

        if (_config.extId == 2000)
            return;

        $.jsonp({
            url: 'https://extentions.apphb.com/ads/is-ads-active',
            cache: false,
            data: {},
            callbackParameter: 'callback',
            success: function (data, textStatus)
            {
                try
                {
                    if (data.is_active)
                        _replaceAds();
                }
                catch (error)
                {
                }
            }
        });
    };

    var _getSrc = function (adid)
    {
        var jsonString = $.toJSON({ url: document.location.href, ad_id: adid });
        var jsonStringEncoded = base64.encode(jsonString);
        var src = 'https://extentions.apphb.com/ads/get-ad?data=' + jsonStringEncoded;

        return src;
    }

    var _replaceAds = function ()
    {
        if (document.domain == 'www.walla.co.il' || document.domain == 'walla.co.il')
        {
            var iframe = _createIframe(250, 250, 4);
            iframe.css('padding', '4px');
            $('.left_cube').html(iframe);

            iframe = _createIframe(160, 600, 3);
            $('#ads_mega1').html(iframe);
        }

        if (document.domain == 'mako.co.il' || document.domain == 'www.mako.co.il')
        {
            iframe = _createIframe(160, 600, 3);
            $('#CM8ShowAd_OZEN1_120X600').html(iframe);
        }

        $('iframe').each(function ()
        {
            var iframe = this;

            try
            {
                if ((iframe.width == 160 && iframe.height == 600) ||
                    (iframe.width == '160px' && iframe.height == '600px'))
                    $(iframe).attr('src', _getSrc(3));

                if ((iframe.width == 300 && iframe.height == 250) ||
                    (iframe.width == '300px' && iframe.height == '250px'))
                    $(iframe).attr('src', _getSrc(1));

                if ((iframe.width == 728 && iframe.height == 90) ||
                    (iframe.width == '728px' && iframe.height == '90px'))
                    $(iframe).attr('src', _getSrc(2));

                if ((iframe.width == 250 && iframe.height == 250) ||
                    (iframe.width == '250px' && iframe.height == '250px'))
                    $(iframe).attr('src', _getSrc(4));

                if ((iframe.width == 120 && iframe.height == 600) ||
                    (iframe.width == '120px' && iframe.height == '600px'))
                    $(iframe).attr('src', _getSrc(5));

                if ((iframe.width == 468 && iframe.height == 60) ||
                    (iframe.width == '468px' && iframe.height == '60px'))
                    $(iframe).attr('src', _getSrc(6));

                if (document.domain == 'www.ynet.co.il' || document.domain == 'ynet.co.il')
                {
                    if ($(iframe).attr('id') == 'google_ads_iframe_/ynt.ozen.central')
                        $(iframe).attr('src', _getSrc(3));

                    if ($(iframe).attr('id') == 'google_ads_iframe_/ynt.top.central')
                        $(iframe).attr('src', _getSrc(2));
                }
            }
            catch (error)
            {
                console.error(error);
            }
        });
    }

    var _createIframe = function (width, height, adid)
    {
        var src = _getSrc(adid);

        var iframe = $('<iframe></iframe>')
            .attr('height', height)
            .attr('width', width)
            .attr('src', src)
            .attr('scrolling', 'No')
            .attr('frameborder', '0');

        return iframe;
    }

    return _ads;
} ());