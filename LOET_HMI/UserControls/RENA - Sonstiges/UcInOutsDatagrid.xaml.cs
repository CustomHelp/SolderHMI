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
using LOET_HMI.PLC_Com_Classes;
using LOET_HMI.SystemPages.PopUps;
using System.Globalization;
using LOET_HMI.UserControls.Graphical_UCs;

namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcInOutsDatagrid.xaml
    /// </summary>

    public partial class UcInOutsDatagrid : UserControl
    {
        /*
        public static readonly DependencyProperty _listDatagrid = DependencyProperty.Register(
                "listDatagrid", typeof(List<IStPLCSettingWithDB>), typeof(UcInOutsDatagrid), new PropertyMetadata(new List<IStPLCSettingWithDB>()));

        public List<IStPLCSettingWithDB> SettingList
        {
            get { return (List<IStPLCSettingWithDB>)GetValue(_listDatagrid); }
            set { SetValue(_listDatagrid, value); }
        }
        */


        public List<string> NameList;
        public List<SettingCluster> CombinedList;


        public UcInOutsDatagrid()
        {            
            
            InitializeComponent();
            this.DataContext = this;



        }






        // *****************************************************************
        // ************** DataGrid Selection-Logik  beginnt ****************
        // Wenn in eine Datagrid-Zeile geklickt wird, werden folgende 2 Events ausgelöst:
        // 1.:
        private void DgSettingBool_GotFocus(object sender, RoutedEventArgs e)
        {
            dgSetting.SelectedItem = null;
        }
        // 2.:
        private void DgSettingBool_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSetting.SelectedItem != null) // Siehe auch DataGridSettings_GotFocus()
            {
                // Farbe der geklickte Zeile manuell ändern "highlihted" machen, während das Dialog-Fenster geöffnet ist. Ansonsten wäre die ausgewählte Zeile erst "highlighted", nachdem das DialogFenster geschlossen ist.
                int index = dgSetting.SelectedIndex;
                var row = dgSetting.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
                row.Background = Brushes.SkyBlue;

                /// Typ feststellen
                /// Das ausgewählte Setting-Objekt im DataGrid finden und das SetOutput Dialog-Fenster für dieses Objekt öffnen.
                /// Da die Einträge der Liste generisch erstellt worden sind, ist es beim Klicken auf eine Zeile unbekannt, welchen Typ das Item in der geklickten Zeile genau besitzt.
                /// Diser Typ ist wichtig, wenn, das "as" Keyword nach dem "SelectedItem" verwendet wird.
                /// Folgende Weg hat am besten funktioniert, um es festlegen zu können und keine NullExceptions zu erhalten, wenn anschließend das DialogFenster geöffnet wird.  

                /*
                List<IStPLCSettingWithDB> ObjectListForSelectedItem = new List<IStPLCSettingWithDB>();
                ObjectListForSelectedItem.Add(new StParamPLCDB<bool>());
                ObjectListForSelectedItem.Add(new StParamPLCDB<Int32>());
                ObjectListForSelectedItem.Add(new StParamPLCDB<float>());

                try
                {
                    ObjectListForSelectedItem[0] = dgSetting.SelectedItem as StParamPLCDB<bool>;
                    ObjectListForSelectedItem[1] = dgSetting.SelectedItem as StParamPLCDB<Int32>;
                    ObjectListForSelectedItem[2] = dgSetting.SelectedItem as StParamPLCDB<float>;
                }
                catch {; }
                */

                // Balog 3.8.2020
                List<IStParamPLCDB> ObjectListForSelectedItem = new List<IStParamPLCDB>();
                ObjectListForSelectedItem.Add(new StParamPLCDB<bool>());
                ObjectListForSelectedItem.Add(new StParamPLCDB<Int32>());
                ObjectListForSelectedItem.Add(new StParamPLCDB<float>());

                try
                {
                    ObjectListForSelectedItem[0] = dgSetting.SelectedItem as StParamPLCDB<bool>;
                    ObjectListForSelectedItem[1] = dgSetting.SelectedItem as StParamPLCDB<Int32>;
                    ObjectListForSelectedItem[2] = dgSetting.SelectedItem as StParamPLCDB<float>;
                }
                catch {; }



                dynamic selected_sett_obj = null;
                int ind = -1;
                for (int i = 0; i < ObjectListForSelectedItem.Count; i++)
                {
                    if (ObjectListForSelectedItem[i] != null)
                    {
                        ind = i; //Index, wo die Liste nicht null ist. Die Liste wird mit diesem Index weiterverwendet.
                        selected_sett_obj = ObjectListForSelectedItem[i];

                    }
                }


                /// Dialogfenster öffnen
                /// 
                //((MainWindow)Application.Current.MainWindow).dataLOET.PopUp_SetMainWBackgrDark((MainWindow)Application.Current.MainWindow);

                WindowPopUpSetValue window_setting = new WindowPopUpSetValue(ObjectListForSelectedItem[ind]);
                window_setting.ShowDialog();

                if (window_setting.DialogResult == true) // Das DialogResult Property wird durch das OK-Button des Setting-Windows gesetzt (wenn ein gültiger Wert im Dialog-Fenster angegeben wird)
                {
                    if (selected_sett_obj.GetType() == typeof(StParamPLCDB<bool>))
                        selected_sett_obj.WriteVal(Convert.ToBoolean(window_setting.AnswInSetValPopUp)); // WriteVal-Funktion des Objekts aufrufen (siehe entsprechende Struktur) und über ADS in die SPS schreiben
                    if (selected_sett_obj.GetType() == typeof(StParamPLCDB<Int32>))
                        selected_sett_obj.WriteVal(Convert.ToInt32(window_setting.AnswInSetValPopUp)); // WriteVal-Funktion des Objekts aufrufen (siehe entsprechende Struktur) und über ADS in die SPS schreiben
                    if (selected_sett_obj.GetType() == typeof(StParamPLCDB<float>))
                        selected_sett_obj.WriteVal(float.Parse(window_setting.AnswInSetValPopUp, CultureInfo.InvariantCulture.NumberFormat)); // WriteVal-Funktion des Objekts aufrufen (siehe entsprechende Struktur) und über ADS in die SPS schreiben
                }
                else
                    dgSetting.SelectedItem = null;
                row.ClearValue(DataGridCell.BackgroundProperty);

                //((MainWindow)Application.Current.MainWindow).dataLOET.PopUp_SetMainWBackgrNormal((MainWindow)Application.Current.MainWindow);
            }

        }
        // *************  Selection Logik endet *******************
        // ********************************************************



    }

    // Dieser Converter wird im XAML-Code verwendet
    public class ObjectToTypeStringConverter : IValueConverter
    {
        public object Convert(
         object value, Type targetType,
         object parameter, System.Globalization.CultureInfo culture)
        {
            return value.GetType().Name;
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
