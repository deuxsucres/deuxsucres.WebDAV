using deuxsucres.WebDAV;
using System;
using System.Collections.Generic;
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

            return client;
        }

        async Task DoRequest(HttpMethod method)
        {
            try
            {
                WebDavClient client = CreateClient();
                if (client == null) return;
                pbProgress.Visibility = Visibility.Visible;
                HttpResponseMessage response = null;
                if (method == WebDavClient.PropFind)
                {
                    response = await client.DoPropFindAsync(tbPath.Text, DepthValue.One);
                }
                else
                {
                    response = await client.ExecuteWebRequestAsync(tbPath.Text, method);
                }
                response.EnsureSuccessStatusCode();
                StringBuilder result = new StringBuilder();
                result.AppendLine("# Response");
                result.AppendLine($"{response.StatusCode} {response.ReasonPhrase}");
                foreach (var header in response.Headers)
                    result.AppendLine($"{header.Key}: {string.Join(";", header.Value)}");
                result.AppendLine();
                //result.AppendLine(await response?.Content.ReadAsStringAsync());

                result.AppendLine("# Content");
                if (response.Content != null)
                {
                    foreach (var header in response.Content.Headers)
                        result.AppendLine($"{header.Key}: {string.Join(";", header.Value)}");
                    result.AppendLine();
                }

                if (response.Content != null && (response.Content.Headers.ContentType.MediaType == "text/xml" || response.Content.Headers.ContentType.MediaType == "application/xml"))
                {
                    XDocument doc = XDocument.Load(await response.Content.ReadAsStreamAsync());
                    result.AppendLine(doc.ToString());
                }
                tbLog.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                pbProgress.Visibility = Visibility.Hidden;
            }
        }

        private async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            await DoRequest(WebDavClient.PropFind);
        }

        private async void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            await DoRequest(WebDavClient.Options);
        }
    }
}
