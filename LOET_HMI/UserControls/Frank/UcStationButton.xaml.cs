using System.Windows;
using System.Windows.Controls;


namespace LOET_HMI
{

    public partial class UcStationButton : UserControl
    {

        public UcStationButton()
        {
            InitializeComponent();
        }


        public static DependencyProperty _iState = DependencyProperty.Register("iState", typeof(int), typeof(UcStationButton), new PropertyMetadata());
        public int iState
        {
            get { return (int)GetValue(_iState); }
            set { SetValue(_iState, value);}
        }

        public static DependencyProperty _sText = DependencyProperty.Register("sText", typeof(string), typeof(UcStationButton), new PropertyMetadata());
        public string sText
        {
            get { return (string)GetValue(_sText); }
            set { SetValue(_sText, value); }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public event RoutedEventHandler Click;

        void onButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.Click != null)
            {
                this.Click(this, e);
            }
        }
    }
}
