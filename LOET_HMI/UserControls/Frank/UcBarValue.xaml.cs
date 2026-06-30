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

namespace LOET_HMI
{

    public partial class UcBarValue : UserControl
    {

        public UcBarValue()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty _sName = DependencyProperty.Register("sName", typeof(string), typeof(UcBarValue), new PropertyMetadata());
        public string sName
        {
            get { return (string)GetValue(_sName); }
            set { SetValue(_sName, value); }
        }

        public static DependencyProperty _dValue = DependencyProperty.Register("dValue", typeof(double), typeof(UcBarValue), new PropertyMetadata());
        public double dValue
        {
            get { return (double)GetValue(_dValue); }
            set { SetValue(_dValue, value);}
        }

        public static readonly DependencyProperty _dFontSize = DependencyProperty.Register("dFontSize", typeof(double), typeof(UcBarValue), new PropertyMetadata((double)12));
        public double dFontSize
        {
            get { return (double)GetValue(_dFontSize); }
            set { SetValue(_dFontSize, value); }
        }

        public static readonly DependencyProperty _dFontSizeVal = DependencyProperty.Register("dFontSizeVal", typeof(double), typeof(UcBarValue), new PropertyMetadata((double)14));
        public double dFontSizeVal
        {
            get { return (double)GetValue(_dFontSizeVal); }
            set { SetValue(_dFontSizeVal, value); }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
