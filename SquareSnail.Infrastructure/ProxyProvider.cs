using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using System.Web;
using System.Net;
using System.Web.Caching;
using System.Configuration;
using System.Threading;
using RestSharp;
using log4net;
using System.Reflection;
using System.Runtime.Caching;

namespace SquareSnail.Infrastructure
{
    public class ProxyProvider
    {
        private static ProxyProvider _instance = null;
        private static object _locker = new object();
        private readonly bool _fiddlerEnabled = bool.Parse(ConfigurationManager.AppSettings["FiddlerEnabled"]);
        private readonly ProxyDataProvider _proxyDataProvider = null;

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ProxyProvider()
        {
            _proxyDataProvider = new ProxyDataProvider();
        }

        public static ProxyProvider GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                        _instance = new ProxyProvider();
                }
            }

            return _instance;
        }
        public WebProxy GetRandomProxy()
        {
            if (_fiddlerEnabled)
                return new WebProxy("127.0.0.1:8888");

            var webProxies = GetProxies();
            var randomProxy = webProxies[new Random().Next(0, webProxies.Count)];

            return randomProxy;
        }
        public void StartProxyWatcher()
        {
            var proxyWatcherThread = new Thread(StartProxyWatcherAction);

            proxyWatcherThread.IsBackground = true;
            proxyWatcherThread.Start();
        }

        private void StartProxyWatcherAction()
        {
            while (true)
            {
                try
                {
                    var proxies = _proxyDataProvider.GetProxies();

                    var cdn = new CountdownEvent(proxies.Count);

                    foreach (var proxy in proxies)
                    {
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            var oneProxy = state as BsonDocument;

                            CheckIfProxyAlive(oneProxy);

                            cdn.Signal();

                        }, proxy);
                    }

                    cdn.Wait();

                    Thread.Sleep(1000 * int.Parse(ConfigurationManager.AppSettings["ProxyCheckIntervalSec"]));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }
            }
        }
        private void CheckIfProxyAlive(BsonDocument proxy)
        {
            try
            {
                var client = new RestClient();

                client.Timeout = 5000;
                client.Proxy = ParseWebProxy(proxy);
                client.FollowRedirects = false;

                var request = new RestRequest(ConfigurationManager.AppSettings["ProxyCheckUrl"], Method.GET);

                var startTime = DateTime.Now;
                var response = (RestResponse)client.Execute(request);
                var responseTime = DateTime.Now - startTime;

                if (response.StatusCode == HttpStatusCode.MovedPermanently)
                {
                    _proxyDataProvider.UpdateProxy(proxy["_id"].AsObjectId, true,
                        responseTime.TotalSeconds, (int)response.StatusCode, response.StatusDescription);
                }
                else
                {
                    _proxyDataProvider.UpdateProxy(proxy["_id"].AsObjectId,
                        false,
                        responseTime.TotalSeconds,
                        (int)response.StatusCode,
                        response.ErrorException == null ? response.StatusDescription : response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private List<WebProxy> GetProxies()
        {
            if (MemoryCache.Default.Get("GetProxies") != null)
                return (List<WebProxy>)MemoryCache.Default.Get("GetProxies");

            var proxies = _proxyDataProvider.GetActiveProxies();

            _logger.InfoFormat("got [{0}] proxies from db < 0.5 sec response time", proxies.Count);

            var webProxies = new List<WebProxy>();

            foreach (var proxy in proxies)
            {
                var proxyCredentials = new NetworkCredential(proxy["username"].ToString(), proxy["password"].ToString());
                var webProxy = new WebProxy(string.Format("{0}:{1}", proxy["ip"].ToString(), proxy["port"].ToString()));

                webProxy.Credentials = proxyCredentials;
                webProxies.Add(webProxy);
            }

            MemoryCache.Default.Add("GetProxies", webProxies, DateTimeOffset.Now.AddMinutes(1));

            return webProxies;
        }
        private WebProxy ParseWebProxy(BsonDocument proxy)
        {
            var proxyCredentials = new NetworkCredential(proxy["username"].ToString(), proxy["password"].ToString());
            var webProxy = new WebProxy(string.Format("{0}:{1}", proxy["ip"].ToString(), proxy["port"].ToString()));

            webProxy.Credentials = proxyCredentials;

            return webProxy;
        }
    }
}

