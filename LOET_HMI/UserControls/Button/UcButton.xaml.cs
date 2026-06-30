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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using LOET_HMI.SystemPages.PopUps;
using System.Windows.Threading;
using System.Threading;

/// <summary>
/// Ursprünglich, nachdem ein neuer UserControl erstellt wurde
/// </summary>
/* 
namespace LOET_HMI.UserControls.Button
{
    /// <summary>
    /// Interaktionslogik für UcButton.xaml
    /// </summary>
    public partial class UcButton : UserControl
    {
        public UcButton()
        {
            InitializeComponent();
        }
    }
}

*/


namespace LOET_HMI
{

    // Quelle: https://stackoverflow.com/questions/364928/wpf-how-to-get-a-usercontrol-to-inherit-a-button

    /// <summary>
    /// Interaktionslogik für UcButton.xaml
    /// </summary>
    public partial class UcButton : Button, INotifyPropertyChanged
    {
        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //////////////////////////////////////////////////////////
        ////////////////////// PLC_To_HMI  ///////////////////////
        //////////////////////////////////////////////////////////
        public static readonly DependencyProperty _DpButtonHMI = DependencyProperty.Register(
        "DpButtonHMI", typeof(StButton), typeof(UcButton), new PropertyMetadata(new StButton()));

        public StButton DpButtonHMI
        {
            get { return (StButton)GetValue(_DpButtonHMI); }
            set { SetValue(_DpButtonHMI, value); }
        }


        //////////////////////////////////////////////////////////
        ////////////////////// Feedback //////////////////////////
        ////////////////////////////////////////////////////////// 
        // MBA 11.05.2021: wenn die Farben für diese Properties in der Resource-Datei "CHP_Style_2020.xaml" definiert wird, funktioniert das Auflösen der Styles in Runtime nicht
        // Deswegen werden diese Farben hier lokal vorgeschreiben
        /*
        public static readonly DependencyProperty _DpBackgroundFeedbackOff = DependencyProperty.Register(
            "DpBackgroundFeedbackOff", typeof(Brush), typeof(UcButton), new PropertyMetadata( ));

        public Brush DpBackgroundFeedbackOff
        {
            get { return (Brush)GetValue(_DpBackgroundFeedbackOff); }
            set { SetValue(_DpBackgroundFeedbackOff, value); }
        }

        public static readonly DependencyProperty _DpBackgroundFeedbackOn = DependencyProperty.Register(
            "DpBackgroundFeedbackOn", typeof(Brush), typeof(UcButton), new PropertyMetadata( ));

        public Brush DpBackgroundFeedbackOn
        {
            get { return (Brush)GetValue(_DpBackgroundFeedbackOn); }
            set { SetValue(_DpBackgroundFeedbackOn, value); }
        }
        */
        //////////////////////////////////////////////////////////
        ////////////////////// PopUp /////////////////////////////
        //////////////////////////////////////////////////////////


        public static readonly DependencyProperty _DpShowPopUp = DependencyProperty.Register(
            "DpShowPopUp", typeof(bool), typeof(UcButton), new PropertyMetadata(new bool()));

        public bool DpShowPopUp
        {
            get { return (bool)GetValue(_DpShowPopUp); }
            set { SetValue(_DpShowPopUp, value); }
        }


        public static readonly DependencyProperty _DpPopupText = DependencyProperty.Register(
            "DpPopupText", typeof(string), typeof(UcButton), new PropertyMetadata());

        public string DpPopupText
        {
            get { return (string)GetValue(_DpPopupText); }
            set { SetValue(_DpPopupText, value); }
        }



        public static readonly DependencyProperty _DpPopupHeader = DependencyProperty.Register(
            "DpPopupHeader", typeof(string), typeof(UcButton), new PropertyMetadata());

        public string DpPopupHeader
        {
            get { return (string)GetValue(_DpPopupHeader); }
            set { SetValue(_DpPopupHeader, value); }
        }


        public static readonly DependencyProperty _DpPopupOKbtnText = DependencyProperty.Register(
            "DpPopupOKbtnText", typeof(string), typeof(UcButton), new PropertyMetadata());

        public string DpPopupOKbtnText
        {
            get { return (string)GetValue(_DpPopupOKbtnText); }
            set { SetValue(_DpPopupOKbtnText, value); }
        }


        public bool bTestBool { get; set; }

        // Als Attached Properties: https://stackoverflow.com/questions/18108648/wpf-adding-a-custom-property-in-a-control
        /*
        public class Extensions
        {
                      
            public static readonly DependencyProperty bPopUpShowProperty =
                DependencyProperty.RegisterAttached("bPopUpShow", typeof(bool), typeof(Extensions), new PropertyMetadata(default(bool)));
     
            public static void SetbPopUpShow(UIElement element, bool value)
            {
                element.SetValue(bPopUpShowProperty, value);
            }

            public static bool GetbPopUpShow(UIElement element)
            {
                return (bool)element.GetValue(bPopUpShowProperty);
            }
        }
        */
        /*
        public class Extensions : DependencyObject
        {

            public static readonly DependencyProperty bPopUpShowProperty =
                DependencyProperty.RegisterAttached("bPopUpShow", typeof(bool), typeof(Extensions), new PropertyMetadata(false));

            public static void SetbPopUpShow(DependencyObject element, Boolean value)
            {
                element.SetValue(bPopUpShowProperty, value);
            }

            public static bool GetbPopUpShow(DependencyObject element)
            {
                return (bool)element.GetValue(bPopUpShowProperty);
            }
        }
        */

            /*
        public static readonly DependencyProperty bPopUpShowProperty =
            DependencyProperty.RegisterAttached("bPopUpShow", typeof(bool), typeof(UcButton), new PropertyMetadata(false));

        public static void SetbPopUpShow(DependencyObject element, Boolean value)
        {
            element.SetValue(bPopUpShowProperty, value);
        }

        public static bool GetbPopUpShow(DependencyObject element)
        {
            return (bool)element.GetValue(bPopUpShowProperty);
        }
        */
        /*
        public class PeakHelper : DependencyObject
        {
            public static readonly DependencyProperty IsPeakProperty = DependencyProperty.RegisterAttached(
                "IsPeak", typeof(bool), typeof(PeakHelper), new PropertyMetadata(false));


            public static void SetIsPeak(DependencyObject target, Boolean value)
            {
                target.SetValue(IsPeakProperty, value);
            }

            public static bool GetIsPeak(DependencyObject target)
            {
                return (bool)target.GetValue(IsPeakProperty);
            }
        }
        */




        //////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////        
        /////////////////////////////////////////////////////////////


        public UcButton()
        {

            InitializeComponent();
            //DpShowPopUp = true;

            /*
            Binding myBinding = new Binding();
            myBinding.Source = this;
            myBinding.Path = new PropertyPath("DpButtonHMI.bPopupHMI");
            //myBinding.Mode = BindingMode.TwoWay;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //BindingOperations.SetBinding(Extensions.bPopUpShow, Extensions.bPopUpShowProperty, myBinding);
            BindingOperations.SetBinding(DpShowPopUp, myBinding);
            */

        }

        private void UcBtn_Click(object sender, RoutedEventArgs e)
        {
            // MBA 6.10.2020: immer TRUE an SPS senden -> es muss dann gleich durch die SPS zurückgesetzt werden (=> Flanke)


            if (DpButtonHMI.bPopupHMI) // nach Bestätigung fragen
            {
                GlobalFunc.PopUp_SetMainWBackgrDark();
                //WindowPopUpButton windowPopUp = new WindowPopUpButton(DpPopupHeader, DpPopupText, DpPopupOKbtnText);
                WindowPopUpButton windowPopUp = new WindowPopUpButton(DpButtonHMI.strPopupHeaderHMI, DpButtonHMI.strPopupTextHMI, "Ja");
                windowPopUp.ShowDialog();

                if (windowPopUp.DialogResult == true)
                {
                    DpButtonHMI.WritePressed();
                }
                else
                {
                    ;
                }
                windowPopUp.Close();

                GlobalFunc.PopUp_SetMainWBackgrNormal();
            }
            else // nicht nach Bestätigung fragen
            {
                DpButtonHMI.WritePressed();
            }
            
        }

        private void UcBtn_Loaded(object sender, RoutedEventArgs e)
        {

            //DpButtonHMI.BrushProcessOff = DpBackgroundProcessOff;
            //DpButtonHMI.BrushProcessOn = DpBackgroundProcessOn;

            //DpButtonHMI.BrushProcessOff = Brushes.Purple;
            //DpButtonHMI.BrushProcessOn = Brushes.Brown;

            DispatcherOperation dispOperation = Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    //DpButtonHMI.BrushFeedbackOff = DpBackgroundFeedbackOff;
                    //DpButtonHMI.BrushFeedbackOn = DpBackgroundFeedbackOn;
                    DpButtonHMI.BrushFeedbackOff = Brushes.LightGray;
                    DpButtonHMI.BrushFeedbackOn = Brushes.LawnGreen;

                }
                ), DispatcherPriority.ContextIdle, null);

        }

        private void UcBtn_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }

    public enum BtnTypeOK{ Yes, OK}
}