﻿using deuxsucres.WebDAV;
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
        class RequestResult<T>
        {
            public Uri ServerUri { get; set; }
            public T Result { get; set; }
        }
        Dictionary<HttpRequestMessage, RequestHistory> _requests = new Dictionary<HttpRequestMessage, RequestHistory>();

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

        void Log(string line = null)
        {
            tbLog.AppendText($"{line}\n");
            tbLog.ScrollToEnd();
        }

        void Log(RequestResult<DavMultistatus> result)
        {
            var response = result.Result;
            Log($"{response.Responses.Length} response(s)");
            Log();
            if (response.ResponseDescription != null)
            {
                Log(response.ResponseDescription.Description);
                Log();
            }
            foreach (var resp in response.Responses)
            {
                Log(new RequestResult<DavResponse> { ServerUri = result.ServerUri, Result = resp });
                Log();
            }
        }

        void Log(RequestResult<DavResponse> result)
        {
            var response = result.Result;
            Log($"# {response.Href.Href}");
            if (response.ResponseDescription != null)
            {
                Log(response.ResponseDescription.Description);
                Log(string.Empty);
            }
            if ((response.Status != null && response.Status.StatusCode != 200) || response.Error != null)
            {
                Log($"Error: {response.Error?.ToString() ?? response.Status.StatusDescription}");
            }
            else
            {
                foreach (var propstat in response.Propstats)
                {
                    //Log(info, $"# {prop.NodeName}");
                    if (propstat.ResponseDescription != null)
                    {
                        Log(propstat.ResponseDescription.Description);
                        Log(string.Empty);
                    }
                    if ((propstat.Status != null && propstat.Status.StatusCode != 200) || propstat.Error != null)
                    {
                        Log($"Error: {propstat.Error?.ToString() ?? propstat.Status.StatusDescription}");
                    }
                    else
                    {
                        foreach (var prop in propstat.Prop.Properties)
                        {
                            Log($"- {prop.NodeName}");
                            if(prop.GetType() != typeof(DavProperty))
                            {
                                string vs = prop.ToString();
                                if(!string.IsNullOrEmpty(vs))
                                    Log(vs);
                            }
                            else
                            {
                                if (prop.Node.HasElements)
                                    Log(prop.Node.ToString());
                            }
                        }
                    }
                }
            }
        }

        async Task<RequestResult<T>> CallRequestAsync<T>(Func<WebDavClient, Task<T>> call)
        {
            var connection = cbConnection.SelectedItem as ServerConnection;
            if (connection == null) return null;

            var client = new WebDavClient(connection.Uri.ToString(), connection.UserName, connection.Password);

            Log("-------------------------------------------------------------");

            client.BeforeExecuteRequest += async (s, e) =>
            {
                var history = new RequestHistory
                {
                    Url = e.Request.RequestUri.ToString(),
                    ServerUri = client.ServerUri,
                    Path = client.ServerUri.MakeRelativeUri(e.Request.RequestUri).ToString(),
                    Method = e.Request.Method.Method
                };
                var request = e.Request;

                Log($"Call {history.Method} {history.Url}");

                _requests[request] = history;

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
                history.Request = l.ToString();
            };
            client.AfterExecuteRequest += async (s, e) =>
            {
                var response = e.Response;
                var request = response.RequestMessage;

                if (!_requests.TryGetValue(request, out RequestHistory history))
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

                _requests.Remove(request);
                lbHistory.Items.Insert(0, history);
                lbHistory.SelectedIndex = 0;
            };

            return new RequestResult<T>
            {
                ServerUri = client.ServerUri,
                Result = await call?.Invoke(client)
            };
        }

        async Task CallOptionsAsync(string path)
        {
            var options = (await CallRequestAsync(c => c.OptionsAsync(path)))?.Result;
            if (options != null)
            {
                Log("# Compliance classes");
                foreach (var cclass in options.ComplianceClasses)
                    Log($"- {cclass.Value}");
                Log();
                Log("# Allows");
                foreach (string meth in options.Allow)
                    Log($"- {meth}");
            }
            Log();
        }

        async Task CallPropListAsync(string path)
        {
            var result = await CallRequestAsync(c => c.PropListAsync(path));
            Log(result);
        }

        async Task CallPropFindAsync(string path)
        {
            var result = await CallRequestAsync(c => c.PropFindAsync(path, true, DepthValue.One));
            Log(result);
        }

        BrowseItem BuildBrowseItem(Uri serverUri, DavResponse response)
        {
            Uri resUri = new Uri(serverUri, response?.Href?.Href ?? string.Empty);
            string resPath = serverUri.MakeRelativeUri(resUri).ToString();
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

        private void RefreshBrowserDetail(BrowseItem item)
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

        private void RefreshBrowser(RequestResult<DavMultistatus> result)
        {
            var items = result.Result.Responses.Select(r => BuildBrowseItem(result.ServerUri, r)).ToList();
            //string uri = new Uri(requestInfo.Client.ServerUri, (requestInfo.Path ?? string.Empty).Trim('/')).ToString().TrimEnd('/');
            var rootItem = items.FirstOrDefault();
            if (rootItem != null)
                tbPath.Text = rootItem.Path;
            RefreshBrowserDetail(rootItem);
            lbBrowser.Items.Clear();
            foreach (var item in items.Where(r => r != rootItem))
                lbBrowser.Items.Add(item);
        }

        async Task BrowseAsync(string path)
        {
            var result = await CallRequestAsync(c => c.PropFindAsync(path, true, DepthValue.One));
            RefreshBrowser(result);
        }

        private async void btnGetProperties_Click(object sender, RoutedEventArgs e)
        {
            await CallPropFindAsync(tbPath.Text);
        }

        private async void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            await CallOptionsAsync(tbPath.Text);
        }

        private async void btnListProperties_Click(object sender, RoutedEventArgs e)
        {
            await CallPropListAsync(tbPath.Text);
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
            RefreshBrowserDetail(null);
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

        private void btnPropertyEdit_Click(object sender, RoutedEventArgs e)
        {
            //if ((sender as Button)?.DataContext is DavProperty property)
            //{
            //    property = PropertyEditor.EditProperty(property, this);
            //}
        }

        private void btnPropertyDelete_Click(object sender, RoutedEventArgs e)
        {
            //if ((sender as Button)?.DataContext is DavProperty property)
            //{
            //    if (MessageBox.Show(
            //        $"Etes vous sur de vouloir supprimer la propriété '{property.NodeName}' ?"
            //        , "Suppression d'une propriété"
            //        , MessageBoxButton.YesNo
            //        , MessageBoxImage.Stop
            //        , MessageBoxResult.No
            //        ) == MessageBoxResult.Yes)
            //    {
            //    }
            //}
        }
    }
}
