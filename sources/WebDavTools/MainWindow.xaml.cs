using deuxsucres.WebDAV;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        private async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WebDavClient client = new WebDavClient("http://www.ygrenier.com/sites/baikal/html/cal.php", "yanos", "shayna");
                var response = await client.ExecuteWebRequestAsync("/", "OPTIONS");
                response.EnsureSuccessStatusCode();
                string content = await response?.Content.ReadAsStringAsync();
                tbLog.Text = content ?? string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
