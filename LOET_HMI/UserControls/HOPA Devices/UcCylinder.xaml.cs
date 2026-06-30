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

namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcCylinder.xaml
    /// </summary>
    public partial class UcCylinder : UserControl
    {
        // *******************************************************************************************
        // ********************* DependencyProperty für die Struktur von der SPS *********************
        // *******************************************************************************************
        public static readonly DependencyProperty _stCylCom = DependencyProperty.Register(
            "DpCylinderHMI", typeof(StCylinder), typeof(UcCylinder), new PropertyMetadata(new StCylinder()));

        public StCylinder DpCylinderHMI
        {
            get { return (StCylinder)GetValue(_stCylCom); }
            set { SetValue(_stCylCom, value); }
        }
        // *******************************************************************************************
        // *******************************************************************************************


        /*
        public static readonly DependencyProperty _eNr = DependencyProperty.Register("eNr", typeof(CylinderEnums), typeof(UcCylinder), new PropertyMetadata());
        public CylinderEnums eNr
        {
            get { return (CylinderEnums)GetValue(_eNr); }
            set { SetValue(_eNr, value); }
        }
        */

        public static readonly DependencyProperty _State = DependencyProperty.Register("State", typeof(eComponentState), typeof(UcCylinder), new PropertyMetadata());
        public eComponentState State
        {
            get { return (eComponentState)GetValue(_State); }
            set { SetValue(_State, value); }
        }



        private bool bBtnDown;



        public UcCylinder()
        {
            InitializeComponent();

            /*Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;
            gridMain.Children.Add(border);

            border.SetValue(Grid.RowSpanProperty, 3);
            border.SetValue(Grid.ColumnSpanProperty, 2); */
        }

        private void UcCyl_MouseLeave(object sender, MouseEventArgs e)
        {
            //bBtnDown = false;
        }

        private void UcCyl_MouseEnter(object sender, MouseEventArgs e)
        {
           // bBtnDown = false;
        }

        private void BtnChangeState_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //bBtnDown = true;
        }

        /*
        public enum eComponentState
        {
            // OperationMode
            CS_00_Normal = 0,   // 00 - Komponente OK										// Lila
            CS_10_Fault = 10,  // 10 - Komponente hat Fehler								// Rot
            CS_20_Manual = 20,  // 20 - Bei der Komponente ist eine Handfunktion aktiv		// Blau
            CS_30_Wait = 30,  // 30 - Komponente wartet 									// Gelb
            CS_40_Warn = 40,  // 40 - Komponente meldet Warnung							// Gelb
            CS_50_Mess = 50   // 50 - Komponente Meldet Meldung
        }
        */
    }
}
