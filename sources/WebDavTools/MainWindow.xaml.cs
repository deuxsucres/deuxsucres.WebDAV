using deuxsucres.WebDAV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using WebDavTools.Models;

namespace WebDavTools
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class RequestInfo
        {
            public void Log(string line)
            {
                LogContent.AppendLine(line ?? string.Empty);
            }
            public WebDavClient Client { get; set; }
            public RequestHistory History { get; set; }
            public StringBuilder LogContent { get; set; } = new StringBuilder();
        }

        public MainWindow()
        {
            InitializeComponent();

            pbProgress.Visibility = Visibility.Hidden;

            cbConnection.Items.Clear();
            cbConnection.ItemsSource = new ServerConnection[]
            {
                new ServerConnection
                {
                    Name = "Serveur Baikal Yanos",
                    Uri = new Uri("http://www.ygrenier.com/sites/baikal/html/cal.php/"),
                    UserName = "yanos",
                    Password = "shayna"
                },
                new ServerConnection
                {
                    Name = "Serveur SVN deuxsucres",
                    Uri = new Uri("http://serveur.deuxsucres.com/projects/system2s/svn-repos/"),
                    UserName = "yanos",
                    Password = "Shay16na"
                }
            };
            cbConnection.SelectedIndex = 0;
        }

        RequestInfo CreateClient()
        {
            var connection = cbConnection.SelectedItem as ServerConnection;
            if (connection == null) return null;
            var client = new WebDavClient(connection.Uri.ToString(), connection.UserName, connection.Password);
            var history = new RequestHistory();
            client.BeforeExecuteRequest += async (s, e) =>
            {
                // Request
                var request = e.Request;
                StringBuilder l = new StringBuilder();
                l.AppendLine($"HTTP {request.Version} {request.Method} {request.RequestUri}");
                l.AppendLine();
                foreach (var header in request.Headers)
                    l.AppendLine($"{header.Key}: {string.Join(";", header.Value)}");
                l.AppendLine();
                foreach (var prop in request.Properties)
                    l.AppendLine($"{prop.Key}: {prop.Value}");
                l.AppendLine();
                if (request.Content != null)
                {
                    foreach (var header in request.Content.Headers)
                        l.AppendLine($"{header.Key}: {string.Join(";", header.Value)}");
                    l.AppendLine();
                }
                if (request.Content != null && (request.Content.Headers.ContentType.MediaType == "text/xml" || request.Content.Headers.ContentType.MediaType == "application/xml" || request.Content.Headers.ContentType.MediaType == "text/plain"))
                {
                    l.AppendLine(await request.Content.ReadAsStringAsync());
                }
                history.Method = request.Method.Method;
                history.Url = request.RequestUri.ToString();
                history.Request = l.ToString();
                tbRequest.Text = history.Request;
            };
            client.AfterExecuteRequest += async (s, e) =>
            {
                // Response
                var response = e.Response;
                StringBuilder l = new StringBuilder();
                l.AppendLine($"Response: HTTP {response.Version} {response.StatusCode} {response.ReasonPhrase}");
                l.AppendLine();
                foreach (var header in response.Headers)
                    l.AppendLine($"{header.Key}: {string.Join(";", header.Value)}");
                l.AppendLine();
                l.AppendLine("# Content");
                if (response.Content != null)
                {
                    foreach (var header in response.Content.Headers)
                        l.AppendLine($"{header.Key}: {string.Join(";", header.Value)}");
                    l.AppendLine();
                }

                if (response.Content != null && (response.Content.Headers.ContentType.MediaType == "text/xml" || response.Content.Headers.ContentType.MediaType == "application/xml"))
                {
                    var buffer = new System.IO.MemoryStream();
                    var str = await response.Content.ReadAsStreamAsync();
                    await str.CopyToAsync(buffer);
                    buffer.Seek(0, System.IO.SeekOrigin.Begin);

                    XDocument doc = XDocument.Load(buffer);
                    l.AppendLine(doc.ToString());

                    buffer.Seek(0, System.IO.SeekOrigin.Begin);
                    var newContent = new StreamContent(buffer);
                    foreach (var header in response.Content.Headers)
                        newContent.Headers.Add(header.Key, header.Value);

                    response.Content = newContent;
                }
                history.Response = l.ToString();
                tbResponse.Text = history.Response;
            };
            return new RequestInfo
            {
                Client = client,
                History = history
            };
        }

        void Log(RequestInfo info, string line)
        {
            info?.Log(line);
            tbLog.AppendText($"{line}\n");
            tbLog.ScrollToEnd();
        }

        async Task DoRequest(HttpMethod method)
        {
            RequestInfo requestInfo = null;
            try
            {
                requestInfo = CreateClient();
                if (requestInfo == null) return;

                pbProgress.Visibility = Visibility.Visible;
                lbHistory.IsEnabled = false;
                //tbLog.Clear();
                //tbRequest.Clear();
                //tbResponse.Clear();

                Log(requestInfo, $"Send request {method} on {requestInfo.Client.Uri}{tbPath.Text}");

                HttpResponseMessage response = null;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                if (method == WebDavConstants.PropFind)
                {
                    response = await requestInfo.Client.DoPropFindAsync(tbPath.Text, DepthValue.One);
                    Log(requestInfo, $"Result {response.StatusCode} {response.ReasonPhrase}");
                }
                else if (method == WebDavConstants.Options)
                {
                    var options = await requestInfo.Client.DoOptionsAsync(tbPath.Text);
                    Log(requestInfo, "# Compliance classes");
                    foreach (var cclass in options.ComplianceClasses)
                        Log(requestInfo, $"- {cclass.Value}");
                    Log(requestInfo, "");
                    Log(requestInfo, "# Allows");
                    foreach (var meth in options.Allow)
                        Log(requestInfo, $"- {meth}");
                }
                else
                {
                    response = await requestInfo.Client.ExecuteWebRequestAsync(tbPath.Text, method);
                    Log(requestInfo, $"Result {response.StatusCode} {response.ReasonPhrase}");
                }
                sw.Stop();
                Log(requestInfo, $"Request executed in {sw.Elapsed}");
            }
            catch (Exception ex)
            {
                Log(requestInfo, $"Request failed: {ex.GetBaseException().Message}");
                requestInfo.History.ErrorMessage = ex.GetBaseException().Message;
            }
            finally
            {
                pbProgress.Visibility = Visibility.Hidden;
                lbHistory.IsEnabled = true;
            }
            Log(requestInfo, string.Empty);

            requestInfo.History.Log = requestInfo.LogContent.ToString();
            lbHistory.Items.Insert(0, requestInfo.History);
            lbHistory.SelectedIndex = 0;
        }

        private async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            await DoRequest(WebDavConstants.PropFind);
        }

        private async void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            await DoRequest(WebDavConstants.Options);
        }
    }
}
