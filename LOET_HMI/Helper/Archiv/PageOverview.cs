using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOET_HMI.Klassen
{
    public class PageOverviewClass
    {
        public class Status
        {
            public bool Power { get; set; } // On/Off
            public OperatingModes OperatingMode { get; set; } // Manual/Automatik
        }

        public double AxisPos { get; set; }

        public class State
        {
            private bool _Ready;
            public bool Ready
            {
                get { return _Ready; }
                set { _Ready = value; }
            }

            private bool _PowerIsOn;
            public bool PowerIsOn
            {
                get { return _PowerIsOn; }
                set { _PowerIsOn = value; }
            }

            private bool _Error;
            public bool Error
            {
                get { return _Error; }
                set { _Error = value; }
            }

            private bool _SafteySTOActive;
            public bool SafteySTOActive
            {
                get { return _SafteySTOActive; }
                set { _SafteySTOActive = value; }
            }


        }

        public enum OperatingModes
        {
            Manual,
            Automatik,
            AutomatikHaltStart,
            AutomatikFinish
        }
    }
}
