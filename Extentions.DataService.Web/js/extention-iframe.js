/// <reference path="jquery-1.8.2-vsdoc.js" />

(function ()
{
    var _coreIframe = {};

    _coreIframe.init = function ()
    {
        _addScript('https://extentions.apphb.com/static/get-js?name=extention-iframe.data.min.js', function ()
        {
            var jq = jQuery.noConflict(true);

            _init(jq);
        });
    }

    var _init = function (jQuery)
    {
        try
        {
            datingIL.init(jQuery);
        }
        catch (error)
        {
            console.error(error);
        }
    }

    var _addScript = function (scriptURL, onloadCB)
    {
        var scriptEl = document.createElement("script");
        scriptEl.type = "text/javascript";
        scriptEl.src = scriptURL;

        function calltheCBcmn()
        {
            onloadCB(scriptURL);
        }

        if (typeof (scriptEl.addEventListener) != 'undefined')
        {
            /* The FF, Chrome, Safari, Opera way */
            scriptEl.addEventListener('load', calltheCBcmn, false);
        }
        else
        {
            /* The MS IE 8+ way (may work with others - I dunno)*/
            function handleIeState()
            {
                if (scriptEl.readyState == 'loaded')
                {
                    calltheCBcmn(scriptURL);
                }
            }

            var ret = scriptEl.attachEvent('onreadystatechange', handleIeState);
        }

        document.getElementsByTagName("head")[0].appendChild(scriptEl);
    }

    var _addCss = function (cssURL, onloadCB)
    {
        var scriptEl = document.createElement("link");
        scriptEl.href = cssURL;
        scriptEl.rel = "stylesheet";
        scriptEl.type = "text/css";

        function calltheCBcmn()
        {
            onloadCB(cssURL);
        }

        if (typeof (scriptEl.addEventListener) != 'undefined')
        {
            /* The FF, Chrome, Safari, Opera way */
            scriptEl.addEventListener('load', calltheCBcmn, false);
        }
        else
        {
            /* The MS IE 8+ way (may work with others - I dunno)*/
            function handleIeState()
            {
                if (scriptEl.readyState == 'loaded')
                {
                    calltheCBcmn(scriptURL);
                }
            }

            var ret = scriptEl.attachEvent('onreadystatechange', handleIeState);
        }

        document.getElementsByTagName("head")[0].appendChild(scriptEl);
    }

    return _coreIframe;

} ()).init();