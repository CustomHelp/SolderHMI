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
using System.Globalization;

namespace LOET_HMI
{
    public class ObjectToTypeStringConverter : IValueConverter
    {
        public object Convert(
            object value, 
            Type targetType,
            object parameter, 
            System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string strTypeName = value.GetType().Name; 
                return strTypeName;
                ;
            }               
            else
            {
                string bla = "String";
                return bla.GetType().Name;
            }
        }

        public object ConvertBack(
         object value, Type targetType,
         object parameter, System.Globalization.CultureInfo culture)
        {
            // I don't think you'll need this
            throw new Exception("Can't convert back");
        }
    }

}
