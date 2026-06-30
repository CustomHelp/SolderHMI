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

namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgDVS.xaml
    /// </summary>
    public partial class PgDVS : Page, INotifyPropertyChanged
    {
        // ********************************************************************************************
        // *****************************   UserControl-Inputs   ***************************************
        // ********************************************************************************************
        private int _iGVLArrInd_Axis;
        public int iGVLArrInd_Axis
        {
            get
            {
                return _iGVLArrInd_Axis;
            }
            set
            {
                _iGVLArrInd_Axis = value;
                OnPropertyChanged();
            }
        }

        private int _iGVLArrInd_Encoder;
        public int iGVLArrInd_Encoder
        {
            get
            {
                return _iGVLArrInd_Encoder;
            }
            set
            {
                _iGVLArrInd_Encoder = value;
                OnPropertyChanged();
            }
        }

        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        // ********************************************************************************************
        // *****************************   Achsen   ***************************************************
        // ********************************************************************************************
        // Achse
        public static readonly DependencyProperty _DpAxis = DependencyProperty.Register(
            "DpAxis", typeof(StBeckhoffAxis), typeof(PgDVS), new PropertyMetadata(new StBeckhoffAxis()));

        public StBeckhoffAxis DpAxis
        {
            get { return (StBeckhoffAxis)GetValue(_DpAxis); }
            set { SetValue(_DpAxis, value); }
        }

        // Encoder
        public static readonly DependencyProperty _DpEncoder = DependencyProperty.Register(
            "DpEncoder", typeof(StBeckhoffAxis), typeof(PgDVS), new PropertyMetadata(new StBeckhoffAxis()));

        public StBeckhoffAxis DpEncoder
        {
            get { return (StBeckhoffAxis)GetValue(_DpEncoder); }
            set { SetValue(_DpEncoder, value); }
        }
        //********************************************************************************************
        //********************************************************************************************



        public PgDVS(int in_iGVLArrInd_Axis, int in_iGVLArrInd_Encoder)
        {
            iGVLArrInd_Axis     = in_iGVLArrInd_Axis;
            iGVLArrInd_Encoder  = in_iGVLArrInd_Encoder;

            InitializeComponent();
        }

        private void PageDVS_Loaded(object sender, RoutedEventArgs e)
        {
            DpAxis.Register(    "GVL_Axis.g_arrAxisHMI[" + iGVLArrInd_Axis.ToString()       + "]");
            DpEncoder.Register( "GVL_Axis.g_arrAxisHMI[" + iGVLArrInd_Encoder.ToString()    + "]");
        }

        private void PageDVS_Unloaded(object sender, RoutedEventArgs e)
        {
            // Achse
            DpAxis.Deregister();
            DpEncoder.Deregister();
        }
    }
}
