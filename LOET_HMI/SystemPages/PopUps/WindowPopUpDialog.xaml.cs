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
using LOET_HMI.PLC_Com_Classes;
using System.Globalization;
using System.Reflection;
using System.Threading;



namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für WindowPopUpSetOutput.xaml
    /// </summary>
    public partial class WindowPopUpDialog : Window
    {
        public ST_PopUp DataRawPLC;
        public ST_PopUp DataTranslated;
        public ST_PopUp DataShow;

        public List<TextBlock> textBlocks;

        ICHPTranslate Translater = new CHPTransService();

        public WindowPopUpDialog(ST_PopUp data)
        {
            DataRawPLC = data;

            InitializeComponent();

            tbTextInput1.GotFocus += TbTextInput_GotFocus;
            tbTextInput2.GotFocus += TbTextInput_GotFocus;
            tbTextInput3.GotFocus += TbTextInput_GotFocus;
            tbTextInput4.GotFocus += TbTextInput_GotFocus;
            tbTextInput5.GotFocus += TbTextInput_GotFocus;
            tbTextInput6.GotFocus += TbTextInput_GotFocus;
            tbTextInput7.GotFocus += TbTextInput_GotFocus;
            tbTextInput8.GotFocus += TbTextInput_GotFocus;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTranslated = TranslateList(DataRawPLC);
            LoadTextToWindow(DataTranslated);
        }



        private ST_PopUp TranslateList(ST_PopUp dataToTranlate)
        {
            ST_PopUp dataTranslated = new ST_PopUp();

            //dataTranslated.strDialogHead = Translater.TransPopupTxt(dataToTranlate.strDialogHead, GlobalVar.Language);
            //
            //dataTranslated.strDialogText1 = Translater.TransPopupTxt(dataToTranlate.strDialogText1, GlobalVar.Language);
            //dataTranslated.strDialogText2 = Translater.TransPopupTxt(dataToTranlate.strDialogText2, GlobalVar.Language);
            //dataTranslated.strDialogText3 = Translater.TransPopupTxt(dataToTranlate.strDialogText3, GlobalVar.Language);
            //dataTranslated.strDialogText4 = Translater.TransPopupTxt(dataToTranlate.strDialogText4, GlobalVar.Language);
            //dataTranslated.strDialogText5 = Translater.TransPopupTxt(dataToTranlate.strDialogText5, GlobalVar.Language);
            //
            //dataTranslated.strBtnName_1 = Translater.TransPopupTxt(dataToTranlate.strBtnName_1, GlobalVar.Language);
            //dataTranslated.strBtnName_2 = Translater.TransPopupTxt(dataToTranlate.strBtnName_2, GlobalVar.Language);
            //dataTranslated.strBtnName_3 = Translater.TransPopupTxt(dataToTranlate.strBtnName_3, GlobalVar.Language);
            //dataTranslated.strBtnName_4 = Translater.TransPopupTxt(dataToTranlate.strBtnName_4, GlobalVar.Language);


            dataTranslated.strDialogHead = Translater.TransTxt(dataToTranlate.strDialogHead, eFBType.fb_Popup);

            dataTranslated.strDialogText1 = Translater.TransTxt(dataToTranlate.strDialogText1, eFBType.fb_Popup);
            dataTranslated.strDialogText2 = Translater.TransTxt(dataToTranlate.strDialogText2, eFBType.fb_Popup);
            dataTranslated.strDialogText3 = Translater.TransTxt(dataToTranlate.strDialogText3, eFBType.fb_Popup);
            dataTranslated.strDialogText4 = Translater.TransTxt(dataToTranlate.strDialogText4, eFBType.fb_Popup);
            dataTranslated.strDialogText5 = Translater.TransTxt(dataToTranlate.strDialogText5, eFBType.fb_Popup);

            dataTranslated.strBtnName_1 = Translater.TransTxt(dataToTranlate.strBtnName_1, eFBType.fb_Popup);
            dataTranslated.strBtnName_2 = Translater.TransTxt(dataToTranlate.strBtnName_2, eFBType.fb_Popup);
            dataTranslated.strBtnName_3 = Translater.TransTxt(dataToTranlate.strBtnName_3, eFBType.fb_Popup);
            dataTranslated.strBtnName_4 = Translater.TransTxt(dataToTranlate.strBtnName_4, eFBType.fb_Popup);
            dataTranslated.strBtnName_5 = Translater.TransTxt(dataToTranlate.strBtnName_5, eFBType.fb_Popup);


            return dataTranslated;
        }

        private void LoadTextToWindow(ST_PopUp dataToShow)
        {
            tbHeader.Text = dataToShow.strDialogHead;

            tbText1.Visibility = Visibility.Collapsed;
            tbText2.Visibility = Visibility.Collapsed;
            tbText3.Visibility = Visibility.Collapsed;
            tbText4.Visibility = Visibility.Collapsed;
            tbText5.Visibility = Visibility.Collapsed;

            if (dataToShow.strDialogText1 != "")
            {
                tbText1.Visibility = Visibility.Visible;
                tbText1.Text = dataToShow.strDialogText1;
            }
            if (dataToShow.strDialogText2 != "")
            {
                tbText2.Visibility = Visibility.Visible;
                tbText2.Text = dataToShow.strDialogText2;
            }
            if (dataToShow.strDialogText3 != "")
            {
                tbText3.Visibility = Visibility.Visible;
                tbText3.Text = dataToShow.strDialogText3;
            }
            if (dataToShow.strDialogText4 != "")
            {
                tbText4.Visibility = Visibility.Visible;
                tbText4.Text = dataToShow.strDialogText4;
            }
            if (dataToShow.strDialogText5 != "")
            {
                tbText5.Visibility = Visibility.Visible;
                tbText5.Text = dataToShow.strDialogText5;
            }

            tbTextInput1.Visibility = Visibility.Collapsed;
            tbTextInput2.Visibility = Visibility.Collapsed;
            tbTextInput3.Visibility = Visibility.Collapsed;
            tbTextInput4.Visibility = Visibility.Collapsed;
            tbTextInput5.Visibility = Visibility.Collapsed;
            tbTextInput6.Visibility = Visibility.Collapsed;
            tbTextInput7.Visibility = Visibility.Collapsed;
            tbTextInput8.Visibility = Visibility.Collapsed;

            if (dataToShow.bTextInputEnable)
            {
                if (dataToShow.iCountTextInput == 1)
                {
                    tbTextInput1.Visibility = Visibility.Visible;
                }
                else if (dataToShow.iCountTextInput == 8)
                {
                    tbTextInput1.Visibility = Visibility.Visible;
                    tbTextInput2.Visibility = Visibility.Visible;
                    tbTextInput3.Visibility = Visibility.Visible;
                    tbTextInput4.Visibility = Visibility.Visible;
                    tbTextInput5.Visibility = Visibility.Visible;
                    tbTextInput6.Visibility = Visibility.Visible;
                    tbTextInput7.Visibility = Visibility.Visible;
                    tbTextInput8.Visibility = Visibility.Visible;
                }
            }

            Btn1.Visibility = Visibility.Collapsed;
            Btn2.Visibility = Visibility.Collapsed;
            Btn3.Visibility = Visibility.Collapsed;
            Btn4.Visibility = Visibility.Collapsed;
            Btn5.Visibility = Visibility.Collapsed;

            if (dataToShow.strBtnName_1 != "")
            {
                Btn1.Visibility = Visibility.Visible;
                Btn1.Content = dataToShow.strBtnName_1;
            }
            if (dataToShow.strBtnName_2 != "")
            {
                Btn2.Visibility = Visibility.Visible;
                Btn2.Content = dataToShow.strBtnName_2;
            }
            if (dataToShow.strBtnName_3 != "")
            {
                Btn3.Visibility = Visibility.Visible;
                Btn3.Content = dataToShow.strBtnName_3;
            }
            if (dataToShow.strBtnName_4 != "")
            {
                Btn4.Visibility = Visibility.Visible;
                Btn4.Content = dataToShow.strBtnName_4;
            }

            if (dataToShow.strBtnName_5 != "")
            {
                Btn5.Visibility = Visibility.Visible;
                Btn5.Content = dataToShow.strBtnName_5;
            }

            //labelOffset2ForBinding.Visibility = Visibility.Collapsed;

            //winDialog.Height = spMyStackP.ActualHeight - labelOffset2ForBinding.Height;

            labelOffset2ForBinding.Visibility = Visibility.Collapsed;
            winDialog.Height = spMyStackP.Height;
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {                    
            if(DataRawPLC.bTextInputEnable) 
            {
                if(tbTextInput1.Text == "") // das leere String macht in der SPS Probleme, daher hier nicht erlauben...
                {
                    tbTextInput1.BorderBrush = Brushes.Red;
                }
                else // OK
                {
                    ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.strTextInput1", tbTextInput1.Text);
                    ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.strTextInput2", tbTextInput2.Text);
                    ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.strTextInput3", tbTextInput3.Text);
                    ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.strTextInput4", tbTextInput4.Text);
                    ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.strTextInput5", tbTextInput5.Text);
                    ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.strTextInput6", tbTextInput6.Text);
                    ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.strTextInput7", tbTextInput7.Text);
                    ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.strTextInput8", tbTextInput8.Text);


                    //                     Thread.Sleep(300);
                    ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.iAnswer", 1);
                    this.Close();
                }

            }
            else if( !DataRawPLC.bTextInputEnable)
            {
                ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.iAnswer", 1);
                this.Close();
            }  
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.iAnswer", 2);
            this.Close();
        }

        private void Btn3_Click(object sender, RoutedEventArgs e)
        {
            ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.iAnswer", 3);
            this.Close();
        }

        private void Btn4_Click(object sender, RoutedEventArgs e)
        {
            ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.iAnswer", 4);
            this.Close();
        }

        private void Btn5_Click(object sender, RoutedEventArgs e)
        {
            ADSMain.ADSComServer.WriteItem("GVL_Popup.gstDialog.HMI_to_PLC.iAnswer", 5);
            this.Close();
        }

        private void TbTextInput_GotFocus(object sender, RoutedEventArgs e)
        {
            /*
            TextBox textBoxSelected = sender as TextBox;
            textBoxSelected.Background = Brushes.SkyBlue;
            GlobalFunc.VirtualKeyboardInputToTextBox(Mouse.GetPosition(this), textBoxSelected);

            // Test:
            System.Threading.Thread.Sleep(200);

            //textBoxSelected.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); // Damit bei wiederholtem Click in den Textbox das Event ausgelöst wird.         

            textBoxSelected.Background = Brushes.White;
            */
            ////////////////////////////////////
            ///

            TextBox textBoxSelected = sender as TextBox;
            textBoxSelected.Background = Brushes.SkyBlue;

            VirtualKeyboard virtualKeyboard = new VirtualKeyboard(Mouse.GetPosition(this), textBoxSelected.Text, typeof(TextBox));
            virtualKeyboard.ShowDialog();

            if (virtualKeyboard.DialogResult == true)
                textBoxSelected.Text = virtualKeyboard.AnswerTextBox;

            virtualKeyboard.FirstClickIsDone = false;
            textBoxSelected.Background = Brushes.White;

            //System.Threading.Thread.Sleep(200);
            //textBoxSelected.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); // Damit bei wiederholtem Click in den Textbox das Event ausgelöst wird.         

        }




        private void TbTextInput_LayoutUpdated(object sender, EventArgs e)
        {
         //   TextBox textBoxSelected = sender as TextBox;

            
          //  if (textBoxSelected.Text != "")
          //  {
          //      textBoxSelected.BorderBrush = Brushes.Gray;
           // }
            

        }
    }
}
