using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace SquareSnail.Infrastructure
{
    public class ProxyDataProvider
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string PROXIES_DB_NAME = "proxies";
        private const string PROXIES_COLLECTION_NAME = "proxies";

        private readonly MongoDBServerConnectionWrapper _serverWrapper = null;

        public ProxyDataProvider()
        {
            _serverWrapper = MongoDBServerConnectionWrapper.GetInstance();
        }

        public List<BsonDocument> GetProxies()
        {
            var dataBase = _serverWrapper.GetServer(PROXIES_DB_NAME).GetDatabase(PROXIES_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(PROXIES_COLLECTION_NAME);
            var result = collection.FindAll().ToList();

            return result;
        }
        public List<BsonDocument> GetActiveProxies(double maxResponseTime)
        {
            var dataBase = _serverWrapper.GetServer(PROXIES_DB_NAME).GetDatabase(PROXIES_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(PROXIES_COLLECTION_NAME);
            var result = collection
                .Find(Query.And(Query.EQ("is_active", true), Query.LTE("response_time", maxResponseTime)))
                .SetSortOrder(SortBy.Descending("date_modified"))
                .ToList();

            return result;
        }
        public List<BsonDocument> GetActiveBestProxies(int limit, params string[] fields)
        {
            var dataBase = _serverWrapper.GetServer(PROXIES_DB_NAME).GetDatabase(PROXIES_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(PROXIES_COLLECTION_NAME);

            var result = collection
                .Find(Query.And(Query.EQ("is_active", true)))
                .SetSortOrder(SortBy.Ascending("response_time").Descending("date_modified"))
                .SetLimit(limit)
                .SetFields(Fields.Include(fields))
                .ToList();

            return result;
        }
        public List<BsonDocument> GetActiveWorstProxies(int limit, params string[] fields)
        {
            var dataBase = _serverWrapper.GetServer(PROXIES_DB_NAME).GetDatabase(PROXIES_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(PROXIES_COLLECTION_NAME);

            var result = collection
                .Find(Query.And(Query.EQ("is_active", true)))
                .SetSortOrder(SortBy.Descending("response_time").Descending("date_modified"))
                .SetLimit(limit)
                .SetFields(Fields.Include(fields))
                .ToList();

            return result;
        }
        public void UpdateProxy(ObjectId proxyId, bool isActive, double responseTime, int responseCode, string responseDescription)
        {
            var dataBase = _serverWrapper.GetServer(PROXIES_DB_NAME).GetDatabase(PROXIES_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(PROXIES_COLLECTION_NAME);

            var updateBuilder = new UpdateBuilder();

            updateBuilder.Set("response_time", responseTime);
            updateBuilder.Set("response_code", responseCode);
            updateBuilder.Set("response_description", responseDescription);
            updateBuilder.Set("date_modified", DateTime.Now);
            updateBuilder.Set("is_active", isActive);

            collection.Update(Query.EQ("_id", proxyId), updateBuilder);
        }
        public void AddProxy(string ip, string port, string username, string password)
        {
            var dataBase = _serverWrapper.GetServer(PROXIES_DB_NAME).GetDatabase(PROXIES_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(PROXIES_COLLECTION_NAME);

            var proxy = new BsonDocument();

            proxy["ip"] = ip;
            proxy["port"] = port;
            proxy["username"] = username;
            proxy["password"] = password;

            collection.Insert(proxy);
        }
        public void AddProxies(List<BsonDocument> proxies)
        {
            var dataBase = _serverWrapper.GetServer(PROXIES_DB_NAME).GetDatabase(PROXIES_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(PROXIES_COLLECTION_NAME);

            collection.InsertBatch(proxies);
        }
        public void RemoveAllProxies()
        {
            var dataBase = _serverWrapper.GetServer(PROXIES_DB_NAME).GetDatabase(PROXIES_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(PROXIES_COLLECTION_NAME);

            collection.RemoveAll();
        }
    }
}
