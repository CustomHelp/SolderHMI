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
    public enum UcValueType
    {
        Output = 0,
        Input = 1,
    }

    public partial class UcValue : UserControl
    {

        public UcValue()
        {
            InitializeComponent();
        }

        public static DependencyProperty _eUcValueType = DependencyProperty.Register("eUcValueType", typeof(UcValueType), typeof(UcValue), new PropertyMetadata());
        public UcValueType eUcValueType
        {
            get { return (UcValueType)GetValue(_eUcValueType); }
            set
            {
                SetValue(_eUcValueType, value);
            }
        }

        public static readonly DependencyProperty _sName = DependencyProperty.Register("sName", typeof(string), typeof(UcValue), new PropertyMetadata());
        public string sName
        {
            get { return (string)GetValue(_sName); }
            set { SetValue(_sName, value); }
        }

        public static readonly DependencyProperty _sUnit = DependencyProperty.Register("sUnit", typeof(string), typeof(UcValue), new PropertyMetadata());
        public string sUnit
        {
            get { return (string)GetValue(_sUnit); }
            set { SetValue(_sUnit, value); }
        }

        public static DependencyProperty _sValue = DependencyProperty.Register("sValue", typeof(string), typeof(UcValue), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string sValue
        {
            get { return (string)GetValue(_sValue); }
            set { SetValue(_sValue, value);}
        }

        public static readonly DependencyProperty _dFontSize = DependencyProperty.Register("dFontSize", typeof(double), typeof(UcValue), new PropertyMetadata((double)12));
        public double dFontSize
        {
            get { return (double)GetValue(_dFontSize); }
            set { SetValue(_dFontSize, value); }
        }

        public static readonly DependencyProperty _dFontSizeVal = DependencyProperty.Register("dFontSizeVal", typeof(double), typeof(UcValue), new PropertyMetadata((double)12));
        public double dFontSizeVal
        {
            get { return (double)GetValue(_dFontSizeVal); }
            set { SetValue(_dFontSizeVal, value); }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Style style;
            int C = (int)eUcValueType;
            switch (C)
            {
                case 0:
                    style = this.FindResource("CHP_TBOutput") as Style;
                    tbValue.Style = style;
                    tbValue.IsReadOnly = true;
                    break;

                case 1:
                    style = this.FindResource("CHP_TBInput") as Style;
                    tbValue.Style = style;
                    break;
            }
        }

        private void TbValue_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (eUcValueType == UcValueType.Input)
                {
                    sValue = tbValue.Text;
                }
            }
        }
    }
}
