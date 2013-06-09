using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace FbFollow
{
    public partial class User : UserControl
    {
        private string _profileUrl = null;

        public User()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(_profileUrl);
        }

        public long UserId
        {
            get;
            set;
        }

        public override void Refresh()
        {
            string pictureUrl = string.Format("http://graph.facebook.com/{0}/picture?type=square", UserId);
            _profileUrl = string.Format("http://www.facebook.com/{0}", UserId);
            string graphUrl = string.Format("http://graph.facebook.com/{0}", UserId);

            var client = new RestClient();
            var request = new RestRequest(graphUrl, Method.GET);
            var response = (RestResponse)client.Execute(request);
            var jsonObj = JObject.Parse(response.Content);

            pictureBox1.Load(pictureUrl);
            linkLabel1.Text = jsonObj["name"].Value<string>();
        }
    }
}
