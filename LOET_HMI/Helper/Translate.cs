using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LOET_HMI.UserControls;

namespace LOET_HMI
{
    public interface ICHPTranslate
    {
        string TransTxt(string strTxtToTransl, eFBType controlType); // MBA 01.4.2021  
    }

    public enum Languages
    {
        token = 1,
        englisch = 2,
        slovakian = 3,
        chinesisch = 4,
        arabisch = 5,
        french = 6,
        spanish = 7,
        
        deutsch = 99, // MBA 21.05.2021: bei der Lötanlage aktuell nicht verwendet
    }

    public enum eFBType
    {
        NoType      = 0,
        fb_Message  = 1,
        fb_Popup    = 2,
        fb_Cyl      = 3,
        fb_Axis     = 4,
        fb_DevOnOff = 5,
        fc_Param    = 6
    }

    public partial class ST_Message
    {
        public string strTextFix { get; set; }
        public string strTextTName { get; set; }
        public string strTextCName { get; set; }
        public string strTextVar { get; set; }
        public int    iType { get; set; }
    }



    static public class CHPTrans
    {
        static private CHPTranslater _CHPTranslate;
        static public CHPTranslater CHPTranslate
        {
            get { return _CHPTranslate; }
        }

        static CHPTrans()
        {
            _CHPTranslate = new CHPTranslater();
        }
    }

    public class CHPTransService : ICHPTranslate
    {
        string ICHPTranslate.TransTxt(string strTxtToTransl, eFBType controlType) //MBA 01.04.2021   
        {
            return CHPTrans.CHPTranslate.TransTxt(strTxtToTransl, controlType);//MBA 01.04.2021  
        }
    }

    public class CHPTranslater
    {
        public List<string[]> listCSV_All       = new List<string[]>(); // MBA 31.05.2021
        public List<string[]> listCSV_Message   = new List<string[]>();
        public List<string[]> listCSV_PopUp     = new List<string[]>();
        public List<string[]> listCSV_Cyl       = new List<string[]>();
        public List<string[]> listCSV_Axis      = new List<string[]>();
        public List<string[]> listCSV_DevOnOff  = new List<string[]>();
        public List<string[]> listCSV_Param     = new List<string[]>();

        public void ReadFile(string filepath, bool bTanslateEnabled)
        {
            if (bTanslateEnabled)
            {               
                try
                {
                    //StreamReader readFile = new StreamReader(filepath, Encoding.Default);
                    StreamReader readFile = new StreamReader(filepath, Encoding.UTF8); //MBA 25.3.2021: wegen chinesischen Zeichen

                    string strLineRaw;
                    string[] arrstrLineSplit;

                    try
                    {
                        while ((strLineRaw = readFile.ReadLine()) != null)
                        {
                            strLineRaw.Skip(1); // Header überspringen

                            arrstrLineSplit = strLineRaw.Split(';'); // MBA: VORSICHT! Seperatorzeichen beachten, vllt. wird in der CSV Datei Komma statt Semikolon 
                            //arrstrLineSplit = strLineRaw.Split('\t');
                            if (arrstrLineSplit[0] == "fb_Message" || arrstrLineSplit[0] == "Single Text")
                            {
                                listCSV_Message.Add(arrstrLineSplit);
                                listCSV_All.Add(arrstrLineSplit);
                            }                               
                            else if (arrstrLineSplit[0] == "fb_Popup")
                            {
                                listCSV_PopUp.Add(arrstrLineSplit);
                                listCSV_All.Add(arrstrLineSplit);
                            }                               
                            else if (arrstrLineSplit[0] == "fb_Cyl")
                            {
                                listCSV_Cyl.Add(arrstrLineSplit);
                                listCSV_All.Add(arrstrLineSplit);
                            }
                            else if (arrstrLineSplit[0] == "fb_Axis")
                            {
                                listCSV_Axis.Add(arrstrLineSplit);
                                listCSV_All.Add(arrstrLineSplit);
                            }
                            else if (arrstrLineSplit[0] == "fb_DevOnOff")
                            {
                                listCSV_DevOnOff.Add(arrstrLineSplit);
                                listCSV_All.Add(arrstrLineSplit);
                            }
                            else if (arrstrLineSplit[0] == "fc_Param")
                            {
                                listCSV_Param.Add(arrstrLineSplit);
                                listCSV_All.Add(arrstrLineSplit);
                            }
                                
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Die Datei Translated.csv wurde geöffnet, aber die Einträge konnten nicht vollständig eingelesen werden.\n", // Text 
                                        "Fehler Translated.csv", // Überschrift 
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);

                        //System.Windows.Application.Current.Shutdown(); // HMI schließen
                    }
                }
                catch
                {
                    MessageBox.Show("Die Datei Translated.csv konnte durch das HMI nicht geöffnet werden.\n" +
                                    "Wenn die CSV-Datei außerhalb des HMIs geöffnet wurde, schließen Sie die Datei und starten Sie das HMI erneut.", // Text 
                                    "Fehler Translated.csv", // Überschrift 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Error);

                    //System.Windows.Application.Current.Shutdown(); // HMI schließen
                }
            }
        }


        public string TransTxt(string strTxtToTrans, eFBType controlType)
        {
            int index;
            int iToken = (int)Languages.token;
            int iLanguage = (int)GlobalVar.Language;
            string strTxtTranslated = strTxtToTrans; //Init.

            List<string[]> listLookUp = new List<string[]>();

            switch (controlType)
            {
                case eFBType.fb_Message:
                    listLookUp = listCSV_Message;
                    break;
                case eFBType.fb_Popup:
                    listLookUp = listCSV_PopUp;
                    break;
                case eFBType.fb_Cyl:
                    listLookUp = listCSV_Cyl;
                    break;
                case eFBType.fb_Axis:
                    listLookUp = listCSV_Axis;
                    break;
                case eFBType.fb_DevOnOff:
                    listLookUp = listCSV_DevOnOff;
                    break;
                case eFBType.fc_Param:
                    listLookUp = listCSV_Param;
                    break;
            }

            if (strTxtToTrans != "")
            {
                try
                {
                    //*************************************************************************************************
                    // HINWEIS: wenn der Text nicht gefunden wird, prüfen, ob das Separater-Zeichen der CSV-Datei passt!
                    //          evt. wird "," statt ";" verwendet oder umgekehrt.
                    //*************************************************************************************************
                    index = listLookUp.FindIndex(s => (s[iToken] == strTxtToTrans)); // ein der eingelesenen Translated.csv nach dem Text suchen und den Index speichern
                    if (index > -1) // der zu übersetzende Text wurde in der Translated.csv gefunden
                    {
                        if (listLookUp[index][iLanguage] != "")
                            strTxtTranslated = listLookUp[index][iLanguage]; // die Übersetzung liefern 
                        else
                        {
                            try //MBA 31.05.2021
                            {
                                int indexInListAll = listCSV_All.FindIndex(s => (s[iToken] == strTxtToTrans)); // bei allen Einträgen suchen, nicht nur in der jeweiligen spezifischen Liste
                                strTxtTranslated   = listCSV_All[indexInListAll][iLanguage];
                            }
                            catch
                            {
                                strTxtTranslated = strTxtToTrans; // den ursprünglichen Text ohne Übersetzung liefern
                            }
                        }                            
                    }
                    else
                        strTxtTranslated = strTxtToTrans; // den ursprünglichen Text ohne Übersetzung liefern
                }
                catch
                {
                    ;//Problem mit dem Suchen
                }
            }
            return strTxtTranslated;
        }

    }
}