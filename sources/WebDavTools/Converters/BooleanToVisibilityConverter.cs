using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WebDavTools.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bVal = (value is bool ib) && ib;
            Visibility notVisibility = Visibility.Collapsed;
            if (parameter != null)
            {
                var prms = parameter.ToString().Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string p in prms.Select(p => p.ToLower()))
                {
                    switch (p)
                    {
                        case "not": bVal = !bVal; break;
                        case "hidden": notVisibility = Visibility.Hidden; break;
                    }
                }
            }
            return bVal ? Visibility.Visible : notVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
