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
            public string Path { get; set; }
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
                },
                new ServerConnection
                {
                    Name = "Serveur Web2 deuxsucres",
                    Uri = new Uri("http://web-access.sites.deuxsucres.com/"),
                    UserName = "yanos",
                    Password = "Shay16na"
                }
            };
            cbConnection.SelectedIndex = 0;
        }

        async Task<string> ExtractTextContent(HttpResponseMessage response)
        {
            var buffer = new MemoryStream();
            var str = await response.Content.ReadAsStreamAsync();
            await str.CopyToAsync(buffer);
            buffer.Seek(0, System.IO.SeekOrigin.Begin);

            string result;
            using (var src = new MemoryStream(buffer.ToArray()))
            using (var rdr = new StreamReader(src))
                result = await rdr.ReadToEndAsync();

            buffer.Seek(0, System.IO.SeekOrigin.Begin);
            var newContent = new StreamContent(buffer);
            foreach (var header in response.Content.Headers)
                newContent.Headers.Add(header.Key, header.Value);

            response.Content = newContent;

            return result;
        }

        RequestInfo CreateClient(string method, string path)
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

                if (response.Content != null && (
                    response.Content.Headers.ContentType.MediaType == "text/plain" 
                    || response.Content.Headers.ContentType.MediaType == "text/html"
                    || response.Content.Headers.ContentType.MediaType == "text/xml"
                    || response.Content.Headers.ContentType.MediaType == "application/xml"
                    ))
                {
                    string textContent = await ExtractTextContent(response);

                    if (response.Content.Headers.ContentType.MediaType == "text/xml" || response.Content.Headers.ContentType.MediaType == "application/xml")
                    {
                        try
                        {
                            XDocument doc = XDocument.Parse(textContent);
                            textContent = doc.ToString();
                        }
                        catch (Exception lex)
                        {
                            l.AppendLine($"XML parse error: {lex.GetBaseException().Message}");
                            l.AppendLine();
                        }
                    }
                    l.AppendLine(textContent);
                }
                history.Response = l.ToString();
                //tbResponse.Text = history.Response;
            };
            return new RequestInfo
            {
                Client = client,
                Path = path,
                History = history
            };
        }

        void Log(RequestInfo info, string line)
        {
            info?.Log(line);
            tbLog.AppendText($"{line}\n");
            tbLog.ScrollToEnd();
        }

        void Log(RequestInfo info, DavMultistatus response)
        {
            Log(info, $"{response.Responses.Length} response(s)");
            Log(info, string.Empty);
            if (response.ResponseDescription != null)
            {
                Log(info, response.ResponseDescription.Description);
                Log(info, string.Empty);
            }
            foreach (var resp in response.Responses)
            {
                Log(info, resp);
                Log(info, string.Empty);
            }
        }

        void Log(RequestInfo info, DavResponse response)
        {
            Log(info, $"# {response.Href.Href}");
            if (response.ResponseDescription != null)
            {
                Log(info, response.ResponseDescription.Description);
                Log(info, string.Empty);
            }
            if ((response.Status != null && response.Status.StatusCode != 200) || response.Error != null)
            {
                Log(info, $"Error: {response.Error?.ToString() ?? response.Status.StatusDescription}");
            }
            else
            {
                foreach (var propstat in response.Propstats)
                {
                    //Log(info, $"# {prop.NodeName}");
                    if (propstat.ResponseDescription != null)
                    {
                        Log(info, propstat.ResponseDescription.Description);
                        Log(info, string.Empty);
                    }
                    if ((propstat.Status != null && propstat.Status.StatusCode != 200) || propstat.Error != null)
                    {
                        Log(info, $"Error: {propstat.Error?.ToString() ?? propstat.Status.StatusDescription}");
                    }
                    else
                    {
                        foreach (var prop in propstat.Prop.Properties)
                        {
                            Log(info, $"- {prop.NodeName}");
                            if(prop.GetType() != typeof(DavProperty))
                            {
                                string vs = prop.ToString();
                                if(!string.IsNullOrEmpty(vs))
                                    Log(info, vs);
                            }
                            else
                            {
                                if (prop.Node.HasElements)
                                    Log(info, prop.Node.ToString());
                            }
                        }
                    }
                }
            }
        }

        async Task<TResult> DoRequest<TResult>(string method, string path, Func<RequestInfo, Task<TResult>> callRequest, Action<RequestInfo, TResult> processResult)
        {
            tbLog.Clear();
            TResult result = default(TResult);
            RequestInfo requestInfo = null;
            try
            {
                requestInfo = CreateClient(method, path);
                if (requestInfo == null) return result;

                pbProgress.Visibility = Visibility.Visible;
                lbHistory.IsEnabled = false;

                Log(requestInfo, $"Send request {method} on {requestInfo.Client.ServerUri}{path}");

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

        async Task DoRequest(HttpMethod method, string path)
        {
            if (method == WebDavConstants.PropFind)
            {
                await DoRequest(method.Method, path,
                    requestInfo => requestInfo.Client.PropFindAsync(tbPath.Text, true, DepthValue.One),
                    (requestInfo, response) => Log(requestInfo, response));
            }
            else if (method == WebDavConstants.Options)
            {
                await DoRequest(method.Method, path,
                    requestInfo => requestInfo.Client.OptionsAsync(tbPath.Text),
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
                await DoRequest(method.Method, path,
                    requestInfo => requestInfo.Client.ExecuteWebRequestAsync(tbPath.Text, method),
                    (requestInfo, response) => Log(requestInfo, $"Result {(int)response.StatusCode} {response.ReasonPhrase}")
                    );
            }
        }

        BrowseItem BuildBrowseItem(RequestInfo requestInfo, DavResponse response)
        {
            Uri resUri = new Uri(requestInfo.Client.ServerUri, response?.Href?.Href ?? string.Empty);
            string resPath = requestInfo.Client.ServerUri.MakeRelativeUri(resUri).ToString();
            var pItems = resPath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            List<PathItem> path = new List<PathItem>();
            for (int i = 0, pLen = pItems.Length; i < pLen; i++)
            {
                path.Add(new PathItem {
                    Name = pItems[i],
                    Path = string.Join("/", pItems.Take(i + 1)) + "/"
                });
            }
            return new BrowseItem
            {
                Uri = resUri,
                Path = resPath,
                PathItems = path,
                DisplayName = response.GetProperty<DavDisplayName>()?.DisplayName ?? System.IO.Path.GetFileName(resPath.TrimEnd('/')),
                CreationDate = response.GetProperty<DavCreationDate>()?.CreationDate,
                LastModified = response.GetProperty<DavGetLastModified>()?.LastModified,
                ContentType = response.GetProperty<DavGetContentType>()?.ContentType,
                ContentLength = response.GetProperty<DavGetContentLength>()?.ContentLength,
                IsCollection = response.GetProperty<DavResourceType>()?.IsCollection ?? false,
                Properties = response?.GetProperties()?.ToList()
            };
        }

        private void RefreshBrowserDetail(RequestInfo requestInfo, BrowseItem item)
        {
            tbDetailDisplayName.Text = item?.DisplayName ?? string.Empty;
            tbDetailUrl.Text = item?.Uri?.ToString() ?? "-";
            tbDetailPath.Text = item?.Path ?? "-";
            tbDetailCreationDate.Text = item?.CreationDate?.ToString() ?? "-";
            tbDetailLastModified.Text = item?.LastModified?.ToString() ?? "-";
            tbDetailContentType.Text = item?.ContentType ?? "-";
            tbDetailContentLength.Text = item?.ContentLength?.ToString() ?? "-";
            tbDetailProperties.ItemsSource = item?.Properties;
            pathSelector.ItemsSource = item?.PathItems;
        }

        private void RefreshBrowser(RequestInfo requestInfo, DavMultistatus result)
        {
            var items = result.Responses.Select(r=>BuildBrowseItem(requestInfo, r)).ToList();
            string uri = new Uri(requestInfo.Client.ServerUri, (requestInfo.Path ?? string.Empty).Trim('/')).ToString().TrimEnd('/');
            var rootItem = items.FirstOrDefault(i => i.Uri.ToString().TrimEnd('/') == uri);
            if (rootItem != null)
                tbPath.Text = rootItem.Path;
            RefreshBrowserDetail(requestInfo, rootItem);
            lbBrowser.Items.Clear();
            foreach (var item in items.Where(r => r != rootItem))
                lbBrowser.Items.Add(item);
        }

        async Task BrowseAsync(string path)
        {
            var result = await DoRequest(WebDavConstants.PropFind.Method, path,
                    requestInfo => requestInfo.Client.PropFindAsync(path, true, DepthValue.One),
                    RefreshBrowser);
        }

        private async void btnGetProperties_Click(object sender, RoutedEventArgs e)
        {
            await DoRequest(WebDavConstants.PropFind, tbPath.Text);
        }

        private async void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            await DoRequest(WebDavConstants.Options, tbPath.Text);
        }

        private async void btnListProperties_Click(object sender, RoutedEventArgs e)
        {
            await DoRequest(WebDavConstants.PropFind.Method, tbPath.Text,
                requestInfo => requestInfo.Client.PropListAsync(tbPath.Text),
                (requestInfo, response) => Log(requestInfo, response));
        }

        private async void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            await BrowseAsync(tbPath.Text);
        }

        private async void lbBrowser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var visual = VisualTreeHelper.HitTest(lbBrowser, Mouse.GetPosition(lbBrowser))?.VisualHit as FrameworkElement;
            var item = visual?.DataContext as BrowseItem;
            if (item == null) return;
            if (item.IsCollection)
            {
                try
                {
                    await BrowseAsync(item.Path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void cbConnection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbPath.Text = string.Empty;
            lbBrowser.Items.Clear();
            RefreshBrowserDetail(null, null);
        }

        private async void btnPathItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as PathItem;
            if (item == null)
                await BrowseAsync(string.Empty);
            else
                await BrowseAsync(item.Path);
        }

        private void btnAddProperty_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
