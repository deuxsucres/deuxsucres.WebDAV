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
using WebDavTools.Models;

namespace WebDavTools.Views
{
    /// <summary>
    /// Logique d'interaction pour TextContentViewer.xaml
    /// </summary>
    public partial class TextContentViewer : UserControl, IViewer
    {
        public TextContentViewer()
        {
            InitializeComponent();
        }

        async Task IViewer.LoadViewerAsync(BrowseItem item, HttpResponseMessage head, IWebDavService webdav)
        {
            headers.LoadHeaders(head);
            txtContent.Text = string.Empty;
            try
            {
                var result = await webdav.CallRequestAsync(c => c.GetAsync(item.Path));
                txtContent.Text = await result.Result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                txtContent.Text = $"Erreur de chargement :{ex.Message}";
            }
        }

    }
}
