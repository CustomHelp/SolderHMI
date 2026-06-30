using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LOET_HMI
{
    // Quelle: https://stackoverflow.com/questions/24894788/how-to-display-a-friendly-enum-name-in-wpf-datagrid

    class EnumOrderStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string stateText = "";

                switch((eOrderArchivStates)value) // int in enum casten
                {
                    case eOrderArchivStates.OA_00_NoState:
                        stateText = "Fehler (kein Zustand)";
                        break;
                    case eOrderArchivStates.OA_10_Sent:
                        stateText = "In SPS geladen";
                        break;
                    case eOrderArchivStates.OA_20_Started:
                        stateText = "Gestartet";
                        break;
                    case eOrderArchivStates.OA_30_Finished:
                        stateText = "Beendet";
                        break;
                    case eOrderArchivStates.OA_90_Cancelled:
                        stateText = "Abgebrochen";
                        break;
                }

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
