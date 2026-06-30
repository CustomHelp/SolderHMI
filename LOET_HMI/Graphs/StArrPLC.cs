using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TwinCAT.Ads;
using System.IO;

namespace LOET_HMI
{
    public enum eArrayType // Beckhoff Array-Typ
    {
        AT_000_NoType = 0,
        AT_100_BOOL = 100,

        AT_200_INT = 200,
        AT_210_UINT = 210,
        AT_220_DINT = 220,

        AT_300_REAL = 300,
        AT_310_LREAL = 310,

        //AT_400_STRING   = 400,
    }



    public class StArrPLC
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        //private ADSItem Item = new ADSItem();

        public string ADSName { get; set; }
        public List<string> sListOfRXData;
        public eArrayType eTypeOfBeckhoffVar { get; set; }

        public  int iArrayLength {get; set;}
        private int iSizeOfVarInBytes; // 

        //private AdsStream dataStream;
        //BinaryReader binRead;


        public StArrPLC(string _sNamePLCVar, eArrayType _eType, int _iArrayLength) // Konstruktor
        {
            ADSName         = _sNamePLCVar;
            eTypeOfBeckhoffVar   = _eType;
            iArrayLength    = _iArrayLength;

            switch (_eType)
            {
                case eArrayType.AT_100_BOOL: // 1 Byte
                    iSizeOfVarInBytes = 1;
                    break;

                case eArrayType.AT_200_INT: // 2 Byte
                    iSizeOfVarInBytes = 2;
                    break;

                case eArrayType.AT_210_UINT: // 2 Byte
                    iSizeOfVarInBytes = 2;
                    break;

                case eArrayType.AT_220_DINT:// 4 Byte
                    iSizeOfVarInBytes = 4;
                    break;

                case eArrayType.AT_300_REAL:// 4 Byte
                    iSizeOfVarInBytes = 4;
                    break;

                case eArrayType.AT_310_LREAL:// 8 Byte
                    iSizeOfVarInBytes = 8;
                    break;
            }

            //dataStream  = new AdsStream(iArrayLength * iSizeOfVarInBytes);
            //binRead = new BinaryReader(dataStream);

            sListOfRXData = new List<string>();
        }

        public void ReadPLCArray()
        {
            try
            {
                if(true ) //ADSMain.ADSComServer.Connected)
                {
                    VarCon.ReadArray(ADSName, iArrayLength, iSizeOfVarInBytes, eTypeOfBeckhoffVar, ref sListOfRXData); // 
                }
                else
                {
                    MessageBox.Show("ADSComServer ist not connected.");
                }


            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }


    }
}
