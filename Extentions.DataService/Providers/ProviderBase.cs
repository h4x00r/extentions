using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extentions.DataService.DataPrividers;
using SquareSnail.Infrastructure;

namespace Extentions.DataService.Providers
{
    public class ProviderBase
    {
        protected readonly CommonDataProvider _commonDataProvider = null;
        protected readonly BoostMeDataProvider _boostMeDataProvider = null;
        protected readonly FacebookDataProvider _facebookDataProvider = null;
        protected readonly AdsDataProvider _adsDataProvider = null;

        public ProviderBase()
        {
            _commonDataProvider = new CommonDataProvider();
            _facebookDataProvider = new FacebookDataProvider();
            _boostMeDataProvider = new BoostMeDataProvider();
            _adsDataProvider = new AdsDataProvider();
        }
    }
}

