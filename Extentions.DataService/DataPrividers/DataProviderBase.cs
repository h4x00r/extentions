using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquareSnail.Infrastructure;

namespace Extentions.DataService.DataPrividers
{
    public class DataProviderBase
    {
        protected const string EXTENTIONS_DB_NAME = "extentions";
        protected const string DATING_IL_DB_NAME = "datingbook";

        protected const string USERS_COLLECTION_NAME = "users";
        protected const string EXT_USERS_COLLECTION_NAME = "ext_users";
        protected const string LOGIN_COLLECTION_NAME = "login";
        protected const string USERS_MAIL_ACTIVITY_COLLECTION_NAME = "users_mail_activity";
        protected const string DATING_IL_ACTIVITY_COLLECTION_NAME = "datingil_activity";
        protected const string USERS_MAILING_COLLECTION_NAME = "users_mailing";
        protected const string PAGE_VIEWS_COLLECTION_NAME = "page_views";
        protected const string ADS_IMPRESSIONS_COLLECTION_NAME = "ads_impressions";
        protected const string DATING_IL_USERS_COLLECTION_NAME = "Infra.DaitingBook.UserInfo";
        protected const string HEBREW_NAMES_COLLECTION_NAME = "names";

        protected const string BOOSTME_USERS_MAIL_ACTIVITY_COLLECTION_NAME = "boostme_users_mail_activity";
        protected const string BOOSTME_USERS_MAILING_COLLECTION_NAME = "boostme_users_mailing";

        protected readonly MongoDBServerConnectionWrapper _serverWrapper = null;

        public DataProviderBase()
        {
            _serverWrapper = MongoDBServerConnectionWrapper.GetInstance();
        }
    }
}
