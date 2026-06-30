using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using LOET_HMI.SystemPages.PopUps;
using LOET_HMI.PLC_Com_Classes;


namespace LOET_HMI.UserControls
{

    public partial class UcBeckhoffAxis : UserControl
    {
        public int i = 0;
        public UcBeckhoffAxis()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty _PLCToHMI = DependencyProperty.Register("PLCToHMI", typeof(StBeckhoffAxis), typeof(UcBeckhoffAxis), new PropertyMetadata(new StBeckhoffAxis()));
        public StBeckhoffAxis PLCToHMI
        {
            get { return (StBeckhoffAxis)GetValue(_PLCToHMI); }
            set { SetValue(_PLCToHMI, value); }
        }


        // Minus Minus
        private void BtnMinusMinus_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PLCToHMI.SetMinusMinus(true);
        }

        private void BtnMinusMinus_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PLCToHMI.SetMinusMinus(false);
        }
        private void BtnMinusMinus_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PLCToHMI.SetMinusMinus(false);
        }

        // Minus
        private void BtnMinus_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PLCToHMI.SetMinus(true);
        }

        private void BtnMinus_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PLCToHMI.SetMinus(false);
        }

        // Plus
        private void BtnPlus_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 1. Das Element (den Button) greifen
            var btn = sender as IInputElement;
            if (btn != null)
            {
                // 2. Den Fokus und das "Capture" erzwingen. 
                // Das sagt Windows: "Egal wo der Finger hingeht, das Event gehört zu diesem Button!"
                Mouse.Capture(btn);
            }

            // 3. SPS-Variable setzen
            PLCToHMI.SetPlus(true);

            // 4. WICHTIG: Event als erledigt markieren, damit Windows keine Gesten (Scrollen) startet
            e.Handled = true;
        }

        private void BtnPlus_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 1. Das Capture wieder freigeben, sonst reagiert die Maus nirgendwo anders mehr
            if (Mouse.Captured == sender)
            {
                Mouse.Capture(null);
            }

            // 2. SPS-Variable zurücksetzen
            PLCToHMI.SetPlus(false);

            e.Handled = true;
        }

        // PlusPlus
        private void BtnPlusPlus_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PLCToHMI.SetPlusPlus(true);
        }

        private void BtnPlusPlus_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PLCToHMI.SetPlusPlus(false);
        }



        private void DirectionButton_StylusDown(object sender, StylusDownEventArgs e)
        {
            // Das ist der magische Befehl: 
            // Er sagt Windows, dass dieser Touch-Punkt KEIN Rechtsklick-Viereck auslösen darf.
            Stylus.SetIsPressAndHoldEnabled((DependencyObject)sender, false);

            // Wir sagen dem System: "Event ist verarbeitet!"
            // Das verhindert oft, dass Windows überhaupt mit dem Timer für das Viereck anfängt.
            e.Handled = true; // Falls der Motor gar nicht zuckt, nimm das hier mal raus oder rein.
        }

        private void DirectionButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            // Capture ist extrem wichtig, damit der "Kontakt" nicht abreißt
            Mouse.Capture(btn);
            btn.Focus();

            if (btn.Name == "repbtnPlus") PLCToHMI.SetPlus(true);
            else if (btn.Name == "repbtnPlusPlus") PLCToHMI.SetPlusPlus(true);
            else if (btn.Name == "repbtnMinus") PLCToHMI.SetMinus(true);
            else if (btn.Name == "repbtnMinusMinus") PLCToHMI.SetMinusMinus(true);

            e.Handled = true;
        }


        // Eine Methode für ALLE Richtungs-Buttons (MouseUp / Leave)
        private void DirectionButton_Release(object sender, EventArgs e)
        {
            // Capture lösen
            if (Mouse.Captured == sender)
            {
                Mouse.Capture(null);
            }

            // Alles auf false setzen (Sicherheit geht vor)
            PLCToHMI.SetPlus(false);
            PLCToHMI.SetPlusPlus(false);
            PLCToHMI.SetMinus(false);
            PLCToHMI.SetMinusMinus(false);
        }


        // Sollposition
        private void TargetPos_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();

            StParamPLCDB<double> paramTmpTargetPos = new StParamPLCDB<double>();
            paramTmpTargetPos.Val = PLCToHMI.lrPosAct;
            paramTmpTargetPos.Min = PLCToHMI.lrPosTargetMin;
            paramTmpTargetPos.Max = PLCToHMI.lrPosTargetMax;

            Window_InputNum InputNum = new Window_InputNum(paramTmpTargetPos);
            //InputNum.Owner = Application.Current.MainWindow;
            InputNum.ShowDialog(); // Das Setting-Window wird geöffnet.

            if (InputNum.DialogResult == true) // Das DialogResult Property wird durch das OK-Button des Setting-Windows gesetzt (wenn ein gültiger Wert angegeben wird)
            {
                PLCToHMI.WriteTargetPos(double.Parse(InputNum.Answer, CultureInfo.InvariantCulture.NumberFormat));
            }

            GlobalFunc.PopUp_SetMainWBackgrNormal();
        }

        // Sollgeschwindigkeit
        private void TargetSpeed_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();

            StParamPLCDB<double> paramTmpTargetSpeed = new StParamPLCDB<double>();
            paramTmpTargetSpeed.Val = PLCToHMI.lrSpeedAct;
            paramTmpTargetSpeed.Min = PLCToHMI.lrSpeedSetPointMin;
            paramTmpTargetSpeed.Max = PLCToHMI.lrSpeedSetPointMax;

            Window_InputNum InputNum = new Window_InputNum(paramTmpTargetSpeed);
            //InputNum.Owner = Application.Current.MainWindow;
            InputNum.ShowDialog(); // Das Setting-Window wird geöffnet.

            if (InputNum.DialogResult == true) // Das DialogResult Property wird durch das OK-Button des Setting-Windows gesetzt (wenn ein gültiger Wert angegeben wird)
            {
                PLCToHMI.WriteTargetSpeed(double.Parse(InputNum.Answer, CultureInfo.InvariantCulture.NumberFormat));
            }

            GlobalFunc.PopUp_SetMainWBackgrNormal();
        }



        private void BtnReleaseContr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PLCToHMI.SetTrigReleaseContr(true);
        }

        private void RepbtnPlus_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Mouse.Captured == sender)
            {
                Mouse.Capture(null);
            }
            PLCToHMI.SetPlus(false);
        }
    }
}
