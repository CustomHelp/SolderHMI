using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Runtime.InteropServices;

namespace LOET_HMI.PLC_Com_Classes
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_ValveFluid
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool bFluidValveOpen;
        [MarshalAs(UnmanagedType.I1)]
        public bool bManualAllowed;
        [MarshalAs(UnmanagedType.I2)]
        public Int16 iFluidValveType;



        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string sValveDescr;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string sElectrDescr;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string sRIDesignation;

    }

    public partial class StFluidValve : INotifyPropertyChanged
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private ADSItem Item = new ADSItem();
        public string ADSName { get; set; }



        /// <summary>
        /// Set/Get
        /// </summary>
        /// 
        private bool _bFluidValveOpenHMI;
        public bool bFluidValveOpenHMI
        {
            get
            {
                return _bFluidValveOpenHMI;
            }

            set
            {
                _bFluidValveOpenHMI = value;
                OnPropertyChanged();
            }
        }

        private bool _bManualAllowedHMI;
        public bool bManualAllowedHMI
        {
            get
            {
                return _bManualAllowedHMI;
            }

            set
            {
                _bManualAllowedHMI = value;
                OnPropertyChanged();
            }
        }

        private Int32 _iFluidValveTypeHMI;
        public Int32 iFluidValveTypeHMI
        {
            get
            {
                return _iFluidValveTypeHMI;
            }

            set
            {
                _iFluidValveTypeHMI = value;
                OnPropertyChanged();
            }
        }


        private string _sValveDescrHMI;
        public string sValveDescrHMI
        {
            get
            {
                return _sValveDescrHMI;
            }

            set
            {
                _sValveDescrHMI = value;
                OnPropertyChanged();
            }
        }

        private string _sElectrDescrHMI;
        public string sElectrDescrHMI
        {
            get
            {
                return _sElectrDescrHMI;
            }

            set
            {
                _sElectrDescrHMI = value;
                OnPropertyChanged();
            }
        }

        private string _sRIDesignationHMI;
        public string sRIDesignationHMI
        {
            get
            {
                return _sRIDesignationHMI;
            }

            set
            {
                _sRIDesignationHMI = value;
                OnPropertyChanged();
            }
        }


        
        public void WriteVal(bool new_Val)
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".val", new_Val);
            else
                ;
        }


        /// <summary>
        /// 
        /// </summary>
        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        ////////////////////////////////////
        /////////// Connection /////////////

        public void Register(string sName)
        {
            ADSName = sName;
            Item.sName = sName;
            Item.Type = typeof(Stream); //!!!!!!!!!!!!!! Oder typeof(StFluidValve) ??
            //Item.Type = typeof(ST_ValveFluid);



            Item = VarCon.AddItem(ADSName + "", typeof(ST_ValveFluid));



            VarCon.EnableCallbackEvent();
            VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;
        }



        private void VarCon_ItemChangeEvent(object sender, ADSItemChangeEventArgs e)
        {
            for (int j = 0; j < e.Item.Count; j++)
            {
                if (Item.iHandle == e.Item[j].iHandle)
                {
                    Item = e.Item[j];

                }
            }

            //object tmp = null;


            ST_ValveFluid tmp = (ST_ValveFluid)Item.Value;
            if (tmp != null)
            {
                try
                {
                    bFluidValveOpenHMI = tmp.bFluidValveOpen;
                    bManualAllowedHMI = tmp.bManualAllowed;
                    iFluidValveTypeHMI = tmp.iFluidValveType;
                    sValveDescrHMI = tmp.sValveDescr;
                    sElectrDescrHMI = tmp.sElectrDescr;
                    sRIDesignationHMI = tmp.sRIDesignation;

                }
                catch
                {

                }
            }
        }

        public void Deregister()
        {
            VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;
            VarCon.RemoveItem(Item);
        }

    }
}
