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
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WebDavTools
{
    /// <summary>
    /// Logique d'interaction pour PropertyEditor.xaml
    /// </summary>
    public partial class PropertyEditor : Window
    {
        public PropertyEditor()
        {
            InitializeComponent();
        }

        void SetProperty(DavProperty property)
        {
            Property = property;
            tbName.Text = property?.NodeName?.ToString() ?? string.Empty;
            tbContent.Text = property?.ToString() ?? string.Empty;
            tbName.IsEnabled = Property == null;
        }

        public static DavProperty EditProperty(DavProperty property, Window owner)
        {
            PropertyEditor editor = new PropertyEditor
            {
                Owner = owner
            };
            editor.SetProperty(property);
            if (editor.ShowDialog() == true)
            {
                return editor.Property;
            }
            return null;
        }

        private void btnValid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tbName.Text))
                    throw new InvalidOperationException("Le nom est obligatoire");
                XName pname = XName.Get(tbName.Text);
                XElement xContent = new XElement(pname);

                if (!string.IsNullOrWhiteSpace(tbContent.Text))
                {
                    try
                    {
                        var x = XElement.Parse(tbContent.Text);
                        if (x.Name == pname)
                        {
                            foreach (var n in x.Nodes())
                                xContent.Add(n);
                        }
                        else
                        {
                            xContent.Add(x);
                        }
                    }
                    catch
                    {
                        xContent.Add(tbContent.Text);
                    }
                }

                Property = DavProperties.LoadProperty(xContent);
                
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public DavProperty Property { get; private set; }
    }
}
