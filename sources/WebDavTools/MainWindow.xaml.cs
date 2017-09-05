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

        WebDavClient CreateClient()
        {
            var connection = cbConnection.SelectedItem as ServerConnection;
            if (connection == null) return null;
            var client = new WebDavClient(connection.Uri.ToString(), connection.UserName, connection.Password);
            client.BeforeExecuteRequest += async (s, e) =>
            {
                // Request
                var request = e.Request;
                tbRequest.AppendText($"HTTP {request.Version} {request.Method} {request.RequestUri}\n");
                tbRequest.AppendText("\n");
                foreach (var header in request.Headers)
                    tbRequest.AppendText($"{header.Key}: {string.Join(";", header.Value)}\n");
                tbRequest.AppendText("\n");
                foreach (var prop in request.Properties)
                    tbRequest.AppendText($"{prop.Key}: {prop.Value}\n");
                tbRequest.AppendText("\n");
                if (request.Content != null)
                {
                    foreach (var header in request.Content.Headers)
                        tbRequest.AppendText($"{header.Key}: {string.Join(";", header.Value)}\n");
                    tbRequest.AppendText("\n");
                }
                if (request.Content != null && (request.Content.Headers.ContentType.MediaType == "text/xml" || request.Content.Headers.ContentType.MediaType == "application/xml" || request.Content.Headers.ContentType.MediaType == "text/plain"))
                {
                    //var buffer = new System.IO.MemoryStream();
                    //var str = await response.Content.ReadAsStreamAsync();
                    //await str.CopyToAsync(buffer);
                    //buffer.Seek(0, System.IO.SeekOrigin.Begin);

                    //XDocument doc = XDocument.Load(buffer);
                    tbRequest.AppendText(await request.Content.ReadAsStringAsync());

                    //buffer.Seek(0, System.IO.SeekOrigin.Begin);
                    //var newContent = new StreamContent(buffer);
                    //foreach (var header in response.Content.Headers)
                    //    newContent.Headers.Add(header.Key, header.Value);

                    //response.Content = newContent;
                }
            };
            client.AfterExecuteRequest += async (s, e) =>
            {
                // Response
                var response = e.Response;
                tbResponse.AppendText($"Response: HTTP {response.Version} {response.StatusCode} {response.ReasonPhrase}\n");
                tbResponse.AppendText("\n");
                foreach (var header in response.Headers)
                    tbResponse.AppendText($"{header.Key}: {string.Join(";", header.Value)}\n");
                tbResponse.AppendText("\n");
                tbResponse.AppendText("# Content\n");
                if (response.Content != null)
                {
                    foreach (var header in response.Content.Headers)
                        tbResponse.AppendText($"{header.Key}: {string.Join(";", header.Value)}\n");
                    tbResponse.AppendText("\n");
                }

                if (response.Content != null && (response.Content.Headers.ContentType.MediaType == "text/xml" || response.Content.Headers.ContentType.MediaType == "application/xml"))
                {
                    var buffer = new System.IO.MemoryStream();
                    var str = await response.Content.ReadAsStreamAsync();
                    await str.CopyToAsync(buffer);
                    buffer.Seek(0, System.IO.SeekOrigin.Begin);

                    XDocument doc = XDocument.Load(buffer);
                    tbResponse.AppendText(doc.ToString());

                    buffer.Seek(0, System.IO.SeekOrigin.Begin);
                    var newContent = new StreamContent(buffer);
                    foreach (var header in response.Content.Headers)
                        newContent.Headers.Add(header.Key, header.Value);

                    response.Content = newContent;
                }
            };
            return client;
        }

        async Task DoRequest(HttpMethod method)
        {
            try
            {
                WebDavClient client = CreateClient();
                if (client == null) return;

                pbProgress.Visibility = Visibility.Visible;
                tbLog.Clear();
                tbRequest.Clear();
                tbResponse.Clear();

                tbLog.AppendText($"Send request {method} on {client.Uri}{tbPath.Text}\n");

                HttpResponseMessage response = null;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                if (method == WebDavConstants.PropFind)
                {
                    response = await client.DoPropFindAsync(tbPath.Text, DepthValue.One);
                }
                else if (method == WebDavConstants.Options)
                {
                    response = await client.DoOptionsAsync(tbPath.Text);
                }
                else
                {
                    response = await client.ExecuteWebRequestAsync(tbPath.Text, method);
                }
                sw.Stop();
                tbLog.AppendText($"Request executed in {sw.Elapsed}\n");
                tbLog.AppendText($"Result {response.StatusCode} {response.ReasonPhrase}\n");
            }
            catch (Exception ex)
            {
                tbLog.AppendText($"Request failed: {ex.GetBaseException().Message}\n");
            }
            finally
            {
                pbProgress.Visibility = Visibility.Hidden;
            }
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
