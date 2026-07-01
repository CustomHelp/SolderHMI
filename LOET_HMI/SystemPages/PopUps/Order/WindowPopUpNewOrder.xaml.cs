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
using LOET_HMI.PLC_Com_Classes;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;

namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für WindowPopUpNewOrder.xaml
    /// </summary>
    public partial class WindowPopUpNewOrder : Window
    {
        private int iParamsetID_Popup { get; set; }
        private int iQuant_Popup { get; set; }
        private int iCartonsQuantProPallet_Popup { get; set; }

        public StParamPLCDB<Int32> paramPLC_Carton { get; set; }
        public db_parameter         paramDB_Cartoon { get; set; }

        public WindowPopUpNewOrder(int _iParamsetID)
        {
            
            InitializeComponent();
            iParamsetID_Popup = _iParamsetID;
        }


        private void WinNewOrder_Loaded(object sender, RoutedEventArgs e)
        {

            paramDB_Cartoon = new db_parameter();

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                List<db_parameter> listParam = DBParam.Handler.GetParamListOverId(iParamsetID_Popup);
                try
                {
                    paramDB_Cartoon = listParam.Single(x => x.sADSName == GlobalVar.Orders.sCartonProPallet_ADSName);
                }
                catch
                {
                    
                    MessageBox.Show("Der zum Auftrag verwendete Sonderparameter 'Kartonanzahl pro Palette' wurde im Parametersatz nicht gefunden.", // Text
                                    "Typparameter nicht gefunden", // Überschrift
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                                    
                }
            }

            try
            {
                paramPLC_Carton = new StParamPLCDB<int>();
                paramPLC_Carton.Register(paramDB_Cartoon.sADSName);
                paramPLC_Carton.ValDB = (int)Convert.ChangeType(paramDB_Cartoon.sValue, typeof(int), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                AppLogger.LogError("WindowPopUpNewOrder", ex);
                MessageBox.Show("Die Kartonanzahl pro Palette konnte nicht geladen werden.\n" + ex.Message,
                                "Auftrag anlegen", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            tbCartonsProPalette.Text = paramDB_Cartoon.sValue;
            //GlobalVar.bInputTextBlockIsClickedMoreThanOnce = false;
            //GlobalVar.iCountBtnPressedByTouch = 0; // Zähler beim Loaded-Event zurücksetzen
        }


        //***************************************************************************************
        //********************** Eingabefeld Menge anklicken (Maus/Touch) ***********************
        /*
        private void TbQuantity_GotFocus(object sender, RoutedEventArgs e)
        {
                    Window_InputNum winNumInput = new Window_InputNum(1, int.MaxValue, false);
            winNumInput.ShowDialog();

            if (winNumInput.DialogResult == true)
            {
                iQuant_Popup = int.Parse(winNumInput.Answer);
                tbQuantity.Text = winNumInput.Answer;
            }

            GlobalVar.bInputTextBlockIsClickedMoreThanOnce = true; // ab dem 2. Aufruf von ClickLogic() ist diese Variable schon true

        }
        */

        private void TbQuantity_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Quantity_ClickLogic();

            //FirstClickIsDone = true;
        }

        private void TbQuantity_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            //e.Handled = true;
            //Quantity_ClickLogic();
        }

        public void Quantity_ClickLogic()
        {
            /*
            Window_InputNum winNumInput = new Window_InputNum(1, int.MaxValue, false);
            winNumInput.Owner = this;
            winNumInput.ShowDialog();

            if (winNumInput.DialogResult == true)
            {
                iQuant_Popup = int.Parse(winNumInput.Answer);
                tbQuantity.Text = winNumInput.Answer;
            }

            GlobalVar.bInputTextBlockIsClickedMoreThanOnce = true; // ab dem 2. Aufruf von ClickLogic() ist diese Variable schon true

            //BtnOk.Focus(); 
            */



            DispatcherOperation dispOperation = Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    Window_InputNum winNumInput = new Window_InputNum(1, int.MaxValue, false,"");
                    winNumInput.Owner = this;
                    winNumInput.ShowDialog();

                    if (winNumInput.DialogResult == true)
                    {
                        iQuant_Popup = int.Parse(winNumInput.Answer);
                        tbQuantity.Text = winNumInput.Answer;
                    }

                    //GlobalVar.bInputTextBlockIsClickedMoreThanOnce = true; // ab dem 2. Aufruf von ClickLogic() ist diese Variable schon true

                }
                ), DispatcherPriority.ContextIdle, null);





        }

        //***************************************************************************************
        //*********************** Eingabefeld Karton anklicken (Maus/Touch) *********************
        private void TbCartonsProPalette_GotFocus(object sender, RoutedEventArgs e)
        {
            /*
            Window_InputNum winNumInput = new Window_InputNum(paramPLC_Carton);
            winNumInput.ShowDialog();

            if (winNumInput.DialogResult == true)
            {
                iCartonsQuantProPallet_Popup = int.Parse(winNumInput.Answer);
                tbCartonsProPalette.Text     = winNumInput.Answer;
            }

            GlobalVar.bInputTextBlockIsClickedMoreThanOnce = true; // ab dem 2. Aufruf von ClickLogic() ist diese Variable schon true
            */
        }

        private void TbCartonsProPalette_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Cartons_ClickLogic();
        }

        private void TbCartonsProPalette_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            //e.Handled = true;
            //Cartons_ClickLogic();
        }

        public void Cartons_ClickLogic()
        {
            /*
            Window_InputNum winNumInput = new Window_InputNum(paramPLC_Carton);
            winNumInput.Owner = this;
            winNumInput.ShowDialog();

            if (winNumInput.DialogResult == true)
            {
                iCartonsQuantProPallet_Popup = int.Parse(winNumInput.Answer);
                tbCartonsProPalette.Text = winNumInput.Answer;
            }

            GlobalVar.bInputTextBlockIsClickedMoreThanOnce = true; // ab dem 2. Aufruf von ClickLogic() ist diese Variable schon true
            */


            DispatcherOperation dispOperation = Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    Window_InputNum winNumInput = new Window_InputNum(paramPLC_Carton);
                    winNumInput.Owner = this;
                    winNumInput.ShowDialog();

                    if (winNumInput.DialogResult == true)
                    {
                        iCartonsQuantProPallet_Popup = int.Parse(winNumInput.Answer);
                        tbCartonsProPalette.Text = winNumInput.Answer;
                    }

                    //GlobalVar.bInputTextBlockIsClickedMoreThanOnce = true; // ab dem 2. Aufruf von ClickLogic() ist diese Variable schon true

                }
                ), DispatcherPriority.ContextIdle, null);



        }
        //***************************************************************************************
        //***************************************************************************************


        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            iCartonsQuantProPallet_Popup = 1;//int.Parse(tbCartonsProPalette.Text);
            bool bResult = DBOrder.Handler.AddNewOrder(iParamsetID_Popup, iQuant_Popup, iCartonsQuantProPallet_Popup);

            if (bResult)
            {
                // ****** Karton in Parametersatz speichern
                string sNewVal_Carton = (string)Convert.ChangeType(tbCartonsProPalette.Text, typeof(string), CultureInfo.InvariantCulture);
                DBParam.Handler.SaveParameterInDB(paramDB_Cartoon.iParamSetId, paramDB_Cartoon.sADSName, sNewVal_Carton); // Param. mit der 2. Konvertierungsoption speichern
                // ******

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Der Auftrag konnte nicht aufgenommen werden", // Text
                                "Neuer Aftrag", // Überschrift
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }


        private void WinNewOrder_Unloaded(object sender, RoutedEventArgs e)
        {
            paramPLC_Carton.Deregister();
        }


    }
}
