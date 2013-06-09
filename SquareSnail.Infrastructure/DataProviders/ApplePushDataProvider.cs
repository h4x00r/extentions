using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PushSharp;
using System.IO;
using PushSharp.Apple;
using log4net;
using System.Reflection;
using System.Configuration;
using System.Web;

namespace SquareSnail.Infrastructure
{
    public class ApplePushDataProvider
    {
        private readonly PushService _boostMeILPushService = null;
        private readonly PushService _boostMeUSPushService = null;
        private readonly PushService _towPushService = null;
        private static ApplePushDataProvider _instance = null;
        private static object _locker = new object();

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ApplePushDataProvider()
        {
            _boostMeILPushService = new PushService();
            _boostMeUSPushService = new PushService();
            _towPushService = new PushService();

            string certificateType = ConfigurationManager.AppSettings["CertificateType"];

            if (certificateType == "dev")
                InitDev();
            else
                Init();
        }

        public static ApplePushDataProvider GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new ApplePushDataProvider();
                    }
                }
            }

            return _instance;
        }

        public void PushNotification(string deviceToken, string text, int badge, string appName)
        {
            switch (appName)
            {
                case "boostme-us":
                    {
                        _boostMeUSPushService.QueueNotification(NotificationFactory.Apple()
                        .ForDeviceToken(deviceToken)
                        .WithAlert(text)
                        .WithSound("sound.caf")
                        .WithBadge(badge));
                    }
                    break;
                case "boostme-il":
                    {
                        _boostMeILPushService.QueueNotification(NotificationFactory.Apple()
                        .ForDeviceToken(deviceToken)
                        .WithAlert(text)
                        .WithSound("sound.caf")
                        .WithBadge(badge));
                    }
                    break;
                case "tow":
                    {
                        _towPushService.QueueNotification(NotificationFactory.Apple()
                        .ForDeviceToken(deviceToken)
                        .WithAlert(text)
                        .WithSound("sound.caf")
                        .WithBadge(badge));
                    }
                    break;
            }
        }

        private void Init()
        {
            string dir = ConfigurationManager.AppSettings["CertificatesDir"];
            string certificatePass = ConfigurationManager.AppSettings["CertificatePass"];
            string boostMeILDir = Path.Combine(dir, "boostme-il");
            string boostMeUSDir = Path.Combine(dir, "boostme-us");
            string towDir = Path.Combine(dir, "talkorwalk");

            var appleCert = File.ReadAllBytes(Path.Combine(boostMeILDir, "PushSharp.PushCert.Boostme.Production.p12"));
            _boostMeILPushService.StartApplePushService(new ApplePushChannelSettings(true, appleCert, "Aa123456"));

            appleCert = File.ReadAllBytes(Path.Combine(boostMeUSDir, "PushSharp.PushCert.Boostme.US.Production.p12"));
            _boostMeUSPushService.StartApplePushService(new ApplePushChannelSettings(true, appleCert, "Aa123456"));

            appleCert = File.ReadAllBytes(Path.Combine(towDir, "PushSharp.PushCert.TalkOrWalk.Production.p12"));
            _towPushService.StartApplePushService(new ApplePushChannelSettings(true, appleCert, "T@lkOrW@lk"));
        }
        private void InitDev()
        {
            //string certificatesDir = ConfigurationManager.AppSettings["CertificatesDir"];
            //string certificateName = ConfigurationManager.AppSettings["CertificateName"];
            //string certificatePass = ConfigurationManager.AppSettings["CertificatePass"];

            //var appleCert = File.ReadAllBytes(Path.Combine(certificatesDir, certificateName));
            //_pushService.StartApplePushService(new ApplePushChannelSettings(false, appleCert, certificatePass));
        }
    }
}
