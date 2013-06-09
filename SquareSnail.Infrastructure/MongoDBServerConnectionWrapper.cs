using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Configuration;

namespace SquareSnail.Infrastructure
{
    public class MongoDBServerConnectionWrapper
    {
        private Dictionary<string, MongoServer> _servers = null;

        private static MongoDBServerConnectionWrapper _instance = null;
        private static object _locker = new object();

        private MongoDBServerConnectionWrapper()
        {
            _servers = new Dictionary<string, MongoServer>();

            string connectionStringExtentions = ConfigurationManager.AppSettings["MongoDB-extentions"];
            string connectionStringDatingBook = ConfigurationManager.AppSettings["MongoDB-datingbook"];
            string connectionStringProxies = ConfigurationManager.AppSettings["MongoDB-proxies"];

            _servers.Add("extentions", new MongoClient(connectionStringExtentions).GetServer());
            _servers.Add("datingbook", new MongoClient(connectionStringDatingBook).GetServer());
            _servers.Add("proxies", new MongoClient(connectionStringProxies).GetServer());
        }

        public static MongoDBServerConnectionWrapper GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new MongoDBServerConnectionWrapper();
                    }
                }
            }

            return _instance;
        }

        public MongoServer GetServer(string serverKey)
        {
            return _servers[serverKey];
        }
    }
}
