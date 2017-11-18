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

namespace WebDavTools.Views
{
    /// <summary>
    /// Logique d'interaction pour ResponseHeaders.xaml
    /// </summary>
    public partial class ResponseHeaders : UserControl
    {
        public ResponseHeaders()
        {
            InitializeComponent();
        }

        public void LoadHeaders(HttpResponseMessage message)
        {
            var items = message == null ? new List<Tuple<string, string>>()
                : message.Headers.Concat(message.Content.Headers)
                .Select(h => Tuple.Create(h.Key, string.Join(";", h.Value)))
                .ToList();
            lstHeaders.ItemsSource = items;
        }

    }
}
