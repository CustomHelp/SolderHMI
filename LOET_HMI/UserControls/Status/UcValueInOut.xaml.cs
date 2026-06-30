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
using LOET_HMI.PLC_Com_Classes;


namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcValueInOut.xaml
    /// </summary>
    public partial class UcValueInOut : UserControl, INotifyPropertyChanged
    {
        public enum eMode { Write, Read }
        //public enum eLabelHorizAlignment{ Left, Right }


        #region Properties  
            
        public static readonly DependencyProperty _UCMode = DependencyProperty.Register(nameof(UCMode), typeof(eMode), typeof(UcValueInOut), new PropertyMetadata());
        public eMode UCMode
        {
            get { return (eMode)GetValue(_UCMode); }
            set { SetValue(_UCMode, value); }
        }
     

        public static DependencyProperty _UCValue = DependencyProperty.Register(nameof(UCValue), typeof(string), typeof(UserControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string UCValue
        {
            get { return (string)GetValue(_UCValue); }
            set { SetValue(_UCValue, value); }
        }



        /*
        private HorizontalAlignment _UCLabelHorizAlignment;
        public HorizontalAlignment UCLabelHorizAlignment
        {
            get
            {
                return _UCLabelHorizAlignment;
            }
            set
            {
                _UCLabelHorizAlignment = value;
                OnPropertyChanged();
            }
        } */

        public static readonly DependencyProperty _UCLabelHorizAlignment = DependencyProperty.Register(nameof(UCLabelHorizAlignment), typeof(HorizontalAlignment), typeof(UcValueInOut), new PropertyMetadata(HorizontalAlignment.Left));
        public HorizontalAlignment UCLabelHorizAlignment
        {
            get { return (HorizontalAlignment)GetValue(_UCLabelHorizAlignment); }
            set { SetValue(_UCLabelHorizAlignment, value); }
        }

        /*
        private string _UCTextLabel = "text_text";
        public string UCTextLabel
        {
            get
            {
                return _UCTextLabel;
            }
            set
            {
                _UCTextLabel = value;
                OnPropertyChanged();
            }
        } */

        public static readonly DependencyProperty _UCTextLabel = DependencyProperty.Register(nameof(UCTextLabel), typeof(string), typeof(UcValueInOut), new PropertyMetadata());
        public string UCTextLabel
        {
            get { return (string)GetValue(_UCTextLabel); }
            set { SetValue(_UCTextLabel, value); }
        }

        /*
        private double _UCFontSize;
        public double UCFontSize
        {
            get
            {
                return _UCFontSize;
            }
            set
            {
                _UCFontSize = value;
                OnPropertyChanged();
            }
        }*/
        public static readonly DependencyProperty _UCFontSize = DependencyProperty.Register(nameof(UCFontSize), typeof(double), typeof(UcValueInOut), new PropertyMetadata((double)12));
        public double UCFontSize
        {
            get { return (double)GetValue(_UCFontSize); }
            set { SetValue(_UCFontSize, value); }
        }


        /*
        private double _UCHeight;
        public double UCHeight
        {
            get
            {
                return _UCHeight;
            }
            set
            {
                _UCHeight = value;
                OnPropertyChanged();
            }
        }
        */

        //public static readonly DependencyProperty _UCHeight = 
        //    DependencyProperty.Register(nameof(UCHeight), typeof(double), typeof(UcValueInOut), new PropertyMetadata((double)12));
        public static readonly DependencyProperty _UCHeight =
            DependencyProperty.Register(nameof(UCHeight), typeof(double), typeof(UcValueInOut), null);
        public double UCHeight
        {
            get { return (double)GetValue(_UCHeight); }
            set { SetValue(_UCHeight, value); }
        }
        

        /*
        private double _UCWidthValue;
        public double UCWidthValue
        {
            get
            {
                return _UCWidthValue;
            }
            set
            {
                _UCWidthValue = value;
                OnPropertyChanged();
            }
        }*/
        public static readonly DependencyProperty _UCWidthValue = DependencyProperty.Register(nameof(UCWidthValue), typeof(double), typeof(UcValueInOut), new PropertyMetadata((double)30));
        public double UCWidthValue
        {
            get { return (double)GetValue(_UCWidthValue); }
            set { SetValue(_UCWidthValue, value); }
        }

        /*
        private double _UCWidthLabel;
        public double UCWidthLabel
        {
            get
            {
                return _UCWidthLabel;
            }
            set
            {
                _UCWidthLabel = value;
                OnPropertyChanged();
            }
        }*/

        public static readonly DependencyProperty _UCWidthLabel = DependencyProperty.Register(
            nameof(UCWidthLabel), typeof(double), typeof(UcValueInOut), new PropertyMetadata((double)40));
        public double UCWidthLabel
        {
            get { return (double)GetValue(_UCWidthLabel); }
            set { SetValue(_UCWidthLabel, value); }
        }


        /*
        public class VisibleHelpe : DependencyObject
        {
            public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached(
                "IsVisible", typeof(bool), typeof(VisibleHelpe), new PropertyMetadata(false));


            public static void SetIsVisible(DependencyObject target, Boolean value)
            {
                target.SetValue(IsVisibleProperty, value);
            }

            public static bool GetIsVisible(DependencyObject target)
            {
                return (bool)target.GetValue(IsVisibleProperty);
            }
        }*/

        public static readonly DependencyProperty IsTestOnProperty = DependencyProperty.RegisterAttached(
            "IsTestOn", typeof(bool), typeof(UcValueInOut), new PropertyMetadata(false));


        public static void SetIsVisible(DependencyObject target, Boolean value)
        {
            target.SetValue(IsTestOnProperty, value);
        }

        public static bool GetIsVisible(DependencyObject target)
        {
            return (bool)target.GetValue(IsTestOnProperty);
        }












        /*
        private double _UCDistanceValueLabel;
        public double UCDistanceValueLabel
        {
            get
            {
                return _UCDistanceValueLabel;
            }
            set
            {
                _UCDistanceValueLabel = value;
                OnPropertyChanged();
            }
        }*/
        public static readonly DependencyProperty _UCDistanceValueLabel = DependencyProperty.Register(nameof(UCDistanceValueLabel), typeof(double), typeof(UcValueInOut), new PropertyMetadata((double)10));
        public double UCDistanceValueLabel
        {
            get { return (double)GetValue(_UCDistanceValueLabel); }
            set { SetValue(_UCDistanceValueLabel, value); }
        }





        #endregion


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public UcValueInOut()
        {
            
            InitializeComponent();
            UCHeight = new double();
        }

        private void UcValueInOut_Loaded(object sender, RoutedEventArgs e)
        {

            tblLabel.HorizontalAlignment = UCLabelHorizAlignment;

            if (UCMode == eMode.Read)
            {
                tbxValue.IsReadOnly = true;
            }
            else if(UCMode == eMode.Write)
            {

            }
        }

        private void UcValueInOut_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
