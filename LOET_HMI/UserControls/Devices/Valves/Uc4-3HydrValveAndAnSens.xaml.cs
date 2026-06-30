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
using LOET_HMI.PLC_Com_Classes;


namespace LOET_HMI.UserControls.Graphical_UCs
{
    /// <summary>
    /// Interaktionslogik für Uc4_3HydrValveAndAnSens.xaml
    /// </summary>
    public partial class Uc4_3HydrValveAndAnSens : UserControl, INotifyPropertyChanged
    {
        
        public static DependencyProperty _Header = DependencyProperty.Register("Header", typeof(string), typeof(Uc4_3HydrValveAndAnSens), new PropertyMetadata());
        public string Header
        {
            get { return (string)GetValue(_Header); }
            set
            {
                SetValue(_Header, value);
                //tbHeader.Text = value;
            }
        }
        
        public bool ValveLeftClicked { get; set; }
        public bool ValveRightClicked { get; set; }


        private double _UC_AnSensVal;
        public double UC_AnSensVal
        {
            get
            {
                return _UC_AnSensVal;
            }

            set
            {
                _UC_AnSensVal = value;
                OnPropertyChanged();
            }
        }

        public static DependencyProperty _Cylname = DependencyProperty.Register("Cylname", typeof(string), typeof(Uc4_3HydrValveAndAnSens), new PropertyMetadata());
        public string Cylname
        {
            get { return (string)GetValue(_Cylname); }
            set
            {
                SetValue(_Cylname, value);
                OnPropertyChanged();
            }
        }




        //private string _UC_ValveNameLeft;
        public string UC_ValveNameLeft
        {
            get
            {
                return DpHydCylContr.sCylNameHMI;
            }

            set
            {
               // DpDevValveLeft.sDeviceName = value;
                OnPropertyChanged();
            }
        }



        private StDeviceOnOff _DpCylValve;
        public StDeviceOnOff DpCylValve
        {
            get
            {
                return _DpCylValve;
            }

            set
            {
                _DpCylValve = value;
                OnPropertyChanged();
            }
        }


        # region Dependency Properties - Alt
        //*****************************************************************************************
        // ***************************** Devices - Ventile  ***************************************
        public static readonly DependencyProperty _DpValveLeft = DependencyProperty.Register(
            "DpDevValveLeft", typeof(StDeviceOnOff), typeof(Uc4_3HydrValveAndAnSens), new PropertyMetadata(new StDeviceOnOff()));

        public StDeviceOnOff DpDevValveLeft
        {
            get { return (StDeviceOnOff)GetValue(_DpValveLeft); }
            set { SetValue(_DpValveLeft, value); }
        }

        public static readonly DependencyProperty _DpDevValveRight = DependencyProperty.Register(
            "DpDevValveRight", typeof(StDeviceOnOff), typeof(Uc4_3HydrValveAndAnSens), new PropertyMetadata(new StDeviceOnOff()));

        public StDeviceOnOff DpDevValveRight
        {
            get { return (StDeviceOnOff)GetValue(_DpDevValveRight); }
            set { SetValue(_DpDevValveRight, value); }
        }

        //*****************************************************************************************
        // ***************************** Analoger Sensor  *****************************************
        public static readonly DependencyProperty _DpSensAn = DependencyProperty.Register(
            "DpSensAn", typeof(IStSensor), typeof(Uc4_3HydrValveAndAnSens), new PropertyMetadata(new StSensor<double>()));

        public IStSensor DpSensAn
        {
            get { return (IStSensor)GetValue(_DpSensAn); }
            set { SetValue(_DpSensAn, value); }
        }
        #endregion

        //*****************************************************************************************
        // *********************************** Zylinder  *****************************************
        public static readonly DependencyProperty _DpHydCylContr = DependencyProperty.Register(
                "DpHydCylContr", typeof(StCylinder), typeof(Uc4_3HydrValveAndAnSens), new PropertyMetadata(new StCylinder()));
        
        public StCylinder DpHydCylContr
        {
            get { return (StCylinder)GetValue(_DpHydCylContr); }
            set { SetValue(_DpHydCylContr, value);  } 
        }




        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        

        public void SetValve()
        {
            //DpCylValve.sDeviceName = DpHydCylContr.sCylNameHMI + "Valve";
            //DpDevValveLeft.sDeviceName = DpHydCylContr.sCylNameHMI + "ValVEE";
        }


        public Uc4_3HydrValveAndAnSens()
        {
            //DpDevValveLeft = new StDeviceOnOff();
            /// Hinweis: dieser new-Befehl darf für ein DependencyProperty als eine einzige Variable (in diesem Fall vom Typ StDeviceOnOffHMI)
            /// nicht ausgeführt werden. Ansonsten gibt es Probleme mit dem Binding.
            /// Allerdings MUSS die Definition mit dem new-Befehl für ein DependencyProperty als eine Liste unbedingt erfolgen. 
            /// Da gibt es Probleme, wenn es nicht gemacht wird. (Siehe z.B. bei UcFillingDispl())

            InitializeComponent();

            /*
            Binding myBinding = new Binding();
            myBinding.Source = DpHydCylContr;
            myBinding.Path = new PropertyPath("sCylNameHMI");
            //myBinding.Mode = BindingMode.TwoWay;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(tbValue, TextBox.TextProperty, myBinding);
            */

            Binding myBinding = new Binding();
            myBinding.Source = this;
            myBinding.Path = new PropertyPath("DpHydCylContr.sCylNameHMI");
            //myBinding.Mode = BindingMode.TwoWay;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(tbValue , TextBox.TextProperty, myBinding);
            

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            tbHeader.Text = Header;
            
        }

        private void RectValveLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ValveLeftClicked = true;
            XOR();
        }

        private void RectValveRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ValveRightClicked = true;
            XOR();
        }

        public void XOR() // Die logische XOR-Funktion wird realisiert.
        {
            

            if(DpDevValveLeft.bOut==false && DpDevValveRight.bOut==false)
            {
                // den Ausgang auf 1 setzen, der geklickt wurde
                if (ValveLeftClicked == true)
                    DpDevValveLeft.WriteManualOn();
                else if (ValveRightClicked == true)
                    DpDevValveRight.WriteManualOn();


            }
            else if(DpDevValveLeft.bOut || DpDevValveRight.bOut ) //Wenn ein Ventil von den beiden 1 ist...
            {
                //...beide Ausgänge sollen 0 sein.
                if (DpDevValveLeft.bOut == true) // wenn das linke Ventil 1 ist, dieses auf 0 setzen
                    DpDevValveLeft.WriteManualOn();
                else if (DpDevValveRight.bOut == true) // wenn das rechte Ventil 1 ist, dieses auf 0 setzen
                    DpDevValveRight.WriteManualOn();

            }



            ValveLeftClicked = false;
            ValveRightClicked = false;
        }

        private void UcHydVandAnSens_Unloaded(object sender, RoutedEventArgs e)
        {


        }
    }


}
