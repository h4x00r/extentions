using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Extentions.DataService.DataPrividers
{
    public class AdsDataProvider : DataProviderBase
    {
        public void InsertUserPageView(BsonDocument pageView)
        {
            var dataBase = _serverWrapper.GetServer(EXTENTIONS_DB_NAME).GetDatabase(EXTENTIONS_DB_NAME);
            var collection = dataBase.GetCollection<BsonDocument>(ADS_IMPRESSIONS_COLLECTION_NAME);

            collection.EnsureIndex(new IndexKeysBuilder().Descending("date_created"),
                IndexOptions.SetTimeToLive(new TimeSpan(1, 0, 0, 0)));

            collection.Insert(pageView);
        }
    }
}
