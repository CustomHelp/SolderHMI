using System.Windows;
using System.Windows.Controls;


namespace LOET_HMI
{

    public partial class UcLamp : UserControl
    {

        public UcLamp()
        {
            InitializeComponent();
        }

        public static DependencyProperty _LEDColor = DependencyProperty.Register("eLEDColor", typeof(LEDColors),  typeof(UcLamp), new PropertyMetadata());
        public LEDColors eLEDColor
        {
            get { return (LEDColors)GetValue(_LEDColor); }
            set { SetValue(_LEDColor, value);
      
            }
        }
        public static DependencyProperty _IsON = DependencyProperty.Register("bIsON", typeof(bool), typeof(UcLamp), new PropertyMetadata());
        public bool bIsON
        {
            get { return (bool)GetValue(_IsON); }
            set { SetValue(_IsON, value);}
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

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
            }
        }
    }
}
