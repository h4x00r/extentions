/// <reference path="jquery-1.8.2-vsdoc.js" />

(function ()
{
    var _core = {};
    var _extId = $extId$;

    _core.init = function ()
    {
        _addScript('https://extentions.apphb.com/static/get-js?name=extention.data.min.js', function ()
        {
            var jq = jQuery.noConflict(true);

            _init(jq);
        });
    }

    var _init = function (jQuery)
    {
        try
        {
            extentionsTracking.init(jQuery, { extId: _extId });
            extentionsAds.init(jQuery, { extId: _extId });
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

    return _core;

} ()).init();

var _gaq = _gaq || [];

_gaq.push(['x._setAccount', '$gaId$']);
_gaq.push(['x._setAllowLinker', true]);
_gaq.push(['x._setDomainName', location.host]);
_gaq.push(['x._trackPageview', document.location.href]);

(function ()
{
    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
})();