using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LOET_HMI
{
    class YesNoTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value.GetType()==typeof(bool))
            {
                string stateText = "";

                bool boolVal = (bool)value;
                if (boolVal)
                    stateText = "ja";
                else
                    stateText = "nein";

                return stateText;
            }

            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
