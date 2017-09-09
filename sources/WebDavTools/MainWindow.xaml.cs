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

        RequestInfo CreateClient(string method)
        {
            var connection = cbConnection.SelectedItem as ServerConnection;
            if (connection == null) return null;
            var client = new WebDavClient(connection.Uri.ToString(), connection.UserName, connection.Password);
            var history = new RequestHistory
            {
                Method = method
            };
            client.BeforeExecuteRequest += async (s, e) =>
            {
                // Request
                var request = e.Request;
                history.Url = request.RequestUri.ToString();

                // Si ce n'est pas la méthode attendue on passe
                if (!string.Equals(request.Method.Method, history.Method, StringComparison.OrdinalIgnoreCase))
                    return;

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
                history.Request = l.ToString();
                //tbRequest.Text = history.Request;
            };
            client.AfterExecuteRequest += async (s, e) =>
            {
                // Response
                var response = e.Response;

                // Si ce n'est pas la méthode attendue on passe
                if (!string.Equals(response.RequestMessage.Method.Method, history.Method, StringComparison.OrdinalIgnoreCase))
                    return;

                StringBuilder l = new StringBuilder();
                l.AppendLine($"Response: HTTP {response.Version} {(int)response.StatusCode} {response.ReasonPhrase}");
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
                else if (response.Content != null && (response.Content.Headers.ContentType.MediaType == "text/plain" || response.Content.Headers.ContentType.MediaType == "text/html"))
                {
                    var buffer = new System.IO.MemoryStream();
                    var str = await response.Content.ReadAsStreamAsync();
                    await str.CopyToAsync(buffer);
                    buffer.Seek(0, System.IO.SeekOrigin.Begin);

                    using (var src = new MemoryStream(buffer.ToArray()))
                    using (var rdr = new StreamReader(src))
                        l.AppendLine(await rdr.ReadToEndAsync());

                    buffer.Seek(0, System.IO.SeekOrigin.Begin);
                    var newContent = new StreamContent(buffer);
                    foreach (var header in response.Content.Headers)
                        newContent.Headers.Add(header.Key, header.Value);

                    response.Content = newContent;
                }
                history.Response = l.ToString();
                //tbResponse.Text = history.Response;
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

        async Task<TResult> DoRequest<TResult>(string method, Func<RequestInfo, Task<TResult>> callRequest, Action<RequestInfo, TResult> processResult)
        {
            TResult result = default(TResult);
            RequestInfo requestInfo = null;
            try
            {
                requestInfo = CreateClient(method);
                if (requestInfo == null) return result;

                pbProgress.Visibility = Visibility.Visible;
                lbHistory.IsEnabled = false;

                Log(requestInfo, $"Send request {method} on {requestInfo.Client.ServerUri}{tbPath.Text}");

                Stopwatch sw = new Stopwatch();
                sw.Start();
                result = await callRequest(requestInfo);
                sw.Stop();
                Log(requestInfo, $"Request executed in {sw.Elapsed}");
                processResult(requestInfo, result);
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

            return result;
        }

        async Task DoRequest(HttpMethod method)
        {
            if (method == WebDavConstants.PropFind)
            {
                await DoRequest(method.Method,
                    requestInfo => requestInfo.Client.DoPropFindAsync(tbPath.Text, true, DepthValue.One),
                    (requestInfo, response) => Log(requestInfo, $"Result {(int)response.StatusCode} {response.ReasonPhrase}")
                    );
            }
            else if (method == WebDavConstants.Options)
            {
                await DoRequest(method.Method,
                    requestInfo => requestInfo.Client.GetOptionsAsync(tbPath.Text),
                    (requestInfo, options) => {
                        Log(requestInfo, "# Compliance classes");
                        foreach (var cclass in options.ComplianceClasses)
                            Log(requestInfo, $"- {cclass.Value}");
                        Log(requestInfo, "");
                        Log(requestInfo, "# Allows");
                        foreach (string meth in options.Allow)
                            Log(requestInfo, $"- {meth}");
                    });
            }
            else
            {
                await DoRequest(method.Method,
                    requestInfo => requestInfo.Client.ExecuteWebRequestAsync(tbPath.Text, method),
                    (requestInfo, response) => Log(requestInfo, $"Result {(int)response.StatusCode} {response.ReasonPhrase}")
                    );
            }
        }

        private async void btnGetProperties_Click(object sender, RoutedEventArgs e)
        {
            await DoRequest(WebDavConstants.PropFind);
        }

        private async void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            await DoRequest(WebDavConstants.Options);
        }

        private async void btnListProperties_Click(object sender, RoutedEventArgs e)
        {
            await DoRequest(WebDavConstants.PropFind.Method,
                requestInfo => requestInfo.Client.GetPropertyNamesAsync(tbPath.Text),
                (requestInfo, response) => Log(requestInfo, $"Result {response.StatusCode} {response.ReasonPhrase}")
                );
        }
    }
}
