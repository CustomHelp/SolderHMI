using System.Windows;
using System.Windows.Controls;


namespace LOET_HMI
{

    public partial class UcLampText : UserControl
    {

        public UcLampText()
        {
            InitializeComponent();
        }

        public static DependencyProperty _LEDColor = DependencyProperty.Register("eLEDColor", typeof(LEDColors),  typeof(UcLampText), new PropertyMetadata(LEDColors.CHP));
        public LEDColors eLEDColor
        {
            get { return (LEDColors)GetValue(_LEDColor); }
            set { SetValue(_LEDColor, value);
      
            }
        }

        public static readonly DependencyProperty _sText = DependencyProperty.Register("sText", typeof(string), typeof(UcLampText), new PropertyMetadata());
        public string sText
        {
            get { return (string)GetValue(_sText); }
            set { SetValue(_sText, value); }
        }

        public static DependencyProperty _IsON = DependencyProperty.Register("bIsON", typeof(bool), typeof(UcLampText), new PropertyMetadata());
        public bool bIsON
        {
            get { return (bool)GetValue(_IsON); }
            set { SetValue(_IsON, value);}
        }

        public static readonly DependencyProperty _dFontSize = DependencyProperty.Register("dFontSize", typeof(double), typeof(UcLampText), new PropertyMetadata((double)12));
        public double dFontSize
        {
            get { return (double)GetValue(_dFontSize); }
            set { SetValue(_dFontSize, value); }
        }

        public static readonly DependencyProperty _dLEDHeight = DependencyProperty.Register("dLEDHeight", typeof(double), typeof(UcLampText), new PropertyMetadata((double)15));
        public double dLEDHeight
        {
            get { return (double)GetValue(_dLEDHeight); }
            set { SetValue(_dLEDHeight, value); }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Led.Height = dLEDHeight;
            tbText.FontSize = dFontSize;
            Style style;
            int C = (int)eLEDColor;
            switch (C)
            {
                case 0:
                    style = this.FindResource("LEDGreen") as Style;
                    Led.Style = style;
                    break;

                case 1:
                    style = this.FindResource("LEDGreen") as Style;
                    Led.Style = style;
                    break;

                case 2:
                    style = this.FindResource("LEDRed") as Style;
                    Led.Style = style;
                    break;

                case 3:
                    style = this.FindResource("LEDYellow") as Style;
                    Led.Style = style;
                    break;
                case 4:
                    style = this.FindResource("LEDBlue") as Style;
                    Led.Style = style;
                    break;
                case 5:
                    style = this.FindResource("LEDCHP") as Style;
                    Led.Style = style;
                    break;
            }
        }
    }
}
