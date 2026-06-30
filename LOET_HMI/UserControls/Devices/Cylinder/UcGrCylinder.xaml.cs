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
    /// Interaktionslogik für UcGrCylinder.xaml
    /// </summary>
    public partial class UcGrCylinder : UserControl, INotifyPropertyChanged
    {

        public static readonly DependencyProperty _stCylCom = DependencyProperty.Register(
            "DpCylinderHMI", typeof(StCylinder), typeof(UcGrCylinder), new PropertyMetadata(new StCylinder()));

        public StCylinder DpCylinderHMI
        {
            get { return (StCylinder)GetValue(_stCylCom); }
            set { SetValue(_stCylCom, value); }
        }



        private double _TextRotateAngle;
        public double TextRotateAngle
        {
            get
            {
                return _TextRotateAngle;
            }
            set
            {
                _TextRotateAngle = value;
                OnPropertyChanged();
            }
        }



        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        private Thickness _GridToMoveMiddlePos;
        public Thickness GridToMoveMiddlePos
        {
            get
            {
                return _GridToMoveMiddlePos;
            }
            set
            {
                _GridToMoveMiddlePos = value;
                OnPropertyChanged();
            }
        }

        public Thickness GridToMoveForerunPos { get; set; }
        public Thickness GridToMoveForerunPos2 { get; set; }

        public Visibility ImageNextToCylVisibility { get; set; }

        private bool _ImageNextToCylVisbilityTrueFalse;
        public bool ImageNextToCylVisbilityTrueFalse
        {
            get
            {
                return _ImageNextToCylVisbilityTrueFalse;
            }
            set
            {
                _ImageNextToCylVisbilityTrueFalse = value;
                OnPropertyChanged();
            }
        }

        private bool _IsMovable; //init
        public bool IsMovable
        {
            get
            {
                return _IsMovable;
            }
            set
            {
                _IsMovable = value;
                OnPropertyChanged();
            }
        }



        public UcGrCylinder()
        {
            InitializeComponent();
            

            GridToMoveMiddlePos  = new Thickness( (rectCylFrame.Width - rectPiston.Width)/2, 0, 0, 0);
            GridToMoveForerunPos = new Thickness(  rectCylFrame.Width - rectPiston.Width,    0, 0, 0);

            // Initwerte löschen:
            rectValveOn.ClearValue(Rectangle.FillProperty);
            rectValveOn.ClearValue(Rectangle.HorizontalAlignmentProperty);
            rectValveOn.ClearValue(Rectangle.MarginProperty);


           
        }

        private void GrCylinder_Loaded(object sender, RoutedEventArgs e)
        {
            btnManFunc.Visibility = Visibility.Collapsed; // Siehe Komment im XAML-Code


            col2GridSens.Width = new GridLength(rectCylFrame.Width - 4 * col0GridSens.Width.Value);

            // ******************************
            // rectValveOn: Animiertes grünes Rechtek für die Ventilansteuerung

            rectValveOn.Width = (rectCylFrame.Width - rectPiston.Width) / 2  +  2* rectCylFrame.StrokeThickness;
            rectValveOn.StrokeThickness = rectCylFrame.StrokeThickness;
            //*************************************




            RotateTransform rotateTransform1 = new RotateTransform(TextRotateAngle);

            if(TextRotateAngle==180 || TextRotateAngle==-180)
            {
                try
                {
                    tbCylName.RenderTransformOrigin = new Point(0.5, 0.5);
                    tbCylName.LayoutTransform = rotateTransform1;
                    tbCylName.VerticalAlignment = VerticalAlignment.Bottom;

                    //gridOfSens.SetValue(Grid.RowProperty, 2);
                    //gridOfSens.VerticalAlignment = VerticalAlignment.Bottom;
                }
                catch {; }
            }

            //7.11.2019
            if (TextRotateAngle == -90 )
            {
                try
                {
                    tbSensAname.RenderTransformOrigin = new Point(0.5, 0.5);
                    tbSensAname.LayoutTransform = new RotateTransform(90);

                    tbSensBname.RenderTransformOrigin = new Point(0.5, 0.5);
                    tbSensBname.LayoutTransform = new RotateTransform(90);
                }
                catch {; }
            }



        }
    }



}
