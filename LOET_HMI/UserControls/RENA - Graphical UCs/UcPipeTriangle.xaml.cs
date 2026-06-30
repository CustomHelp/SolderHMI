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

namespace LOET_HMI.UserControls.Graphical_UCs
{



    /// <summary>
    /// Interaktionslogik für UcPipeTriangle.xaml
    /// </summary>
    public partial class UcPipeTriangle : UserControl, INotifyPropertyChanged
    {

        public static readonly DependencyProperty _DpPipeState = DependencyProperty.Register(
            "DpPipeState", typeof(bool), typeof(UcPipeTriangle), new PropertyMetadata(new bool()));

        public bool DpPipeState
        {
            get { return (bool)GetValue(_DpPipeState); }
            set { SetValue(_DpPipeState, value); }
        }


        public static readonly DependencyProperty _DirectionOfFluid = DependencyProperty.Register(
        "DirectionOfFluid", typeof(FluidDirection), typeof(UcPipeTriangle), new PropertyMetadata(FluidDirection.FluidIn));

        public FluidDirection DirectionOfFluid
        {
            get { return (FluidDirection)GetValue(_DirectionOfFluid); }
            set { SetValue(_DirectionOfFluid, value); }
        }




        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private double _PipeRotateAngle;
        public double PipeRotateAngle
        {
            get
            {
                return _PipeRotateAngle;
            }
            set
            {
                _PipeRotateAngle = value;
                OnPropertyChanged();
            }
        }

        private string _FluidName;
        public string FluidName
        {
            get
            {
                return _FluidName;
            }
            set
            {
                _FluidName = value;
                OnPropertyChanged();
            }
        }


        public UcPipeTriangle()
        {


            InitializeComponent();



        }

        private void UcPipe_Loaded(object sender, RoutedEventArgs e)
        {
            RotateTransform rotateTransform1 = new RotateTransform(PipeRotateAngle);
            RotateTransform rotateTransform2 = new RotateTransform(-PipeRotateAngle);

            //myGrid.RenderTransform = rotateTransform1;
            //tbFluidName.RenderTransform = rotateTransform2;
            mainGrid.LayoutTransform = rotateTransform1;
            tbFluidName.LayoutTransform = rotateTransform2;

            /*
            if (PipeRotateAngle == 90 || PipeRotateAngle == 270 || PipeRotateAngle == -90 || PipeRotateAngle == -270)
            {                
                //row0.Height = new GridLength( tbFluidName.ActualWidth + 14);
            }
            else if (PipeRotateAngle == 0 || PipeRotateAngle == 360 || PipeRotateAngle == -360 )
            {
            }
            */

            if (DirectionOfFluid == FluidDirection.FluidOut)
            {
                triangle.SetValue(Grid.RowProperty, 0);
                tbFluidName.SetValue(Grid.RowProperty, 1);
            }

            if (DirectionOfFluid == FluidDirection.FluidIn)
            {
                triangle.SetValue(Grid.RowProperty, 1);
                tbFluidName.SetValue(Grid.RowProperty, 0);
            }


        }
    }

    public enum FluidDirection { FluidIn, FluidOut }

}
