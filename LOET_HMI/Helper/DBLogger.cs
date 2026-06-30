using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOET_HMI
{
    static public class DBLog
    {
        static private DBLogger _Log;
        static public DBLogger Handler
        {
            get { return _Log; }
        }

        static DBLog()
        {
            _Log = new DBLogger();
        }
    }

    public class DBLogger
    {

        public void Parameter(int iParamSetId, string sParamName, string OldValue, string NewValue, string sAction)
        {

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {

                db_paramset ParamSet = context.db_paramset.Single(s => (s.id == iParamSetId));


                db_parameterlog newEntry = new db_parameterlog();

                newEntry.dtTime = DateTime.Now;
                newEntry.sOldValue = OldValue;
                newEntry.sNewValue = NewValue;
                newEntry.sParamSetName = ParamSet.sName;
                newEntry.sParamSetType = ((ParamSetTypes)ParamSet.iType).ToString();
                newEntry.sParameterName = sParamName;
                newEntry.sAction = sAction;
                newEntry.sUserName = GlobalVar.ActUser.sUserName;

                context.db_parameterlog.Add(newEntry);
                context.SaveChanges();
            }
        }

        public void Manual(string sComponentName, string sAction, string sParameter)
        {

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                db_manuallog newEntry = new db_manuallog();

                newEntry.dtTime = DateTime.Now;
                newEntry.sComponentName = sComponentName;
                newEntry.sAction = sAction;
                newEntry.sParameter = sParameter;
                newEntry.sUserName = GlobalVar.ActUser.sUserName;

                context.db_manuallog.Add(newEntry);
                context.SaveChanges();
            }
        }

        public void User(string sUserName, int iUserLevel, bool bLogIn)
        {

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                db_userlog newEntry = new db_userlog();
                newEntry.dtTime = DateTime.Now;
                newEntry.sUserName = sUserName;
                newEntry.iUserLevel = iUserLevel;

                if (bLogIn)
                    newEntry.sAction = "Log IN";
                else
                    newEntry.sAction = "Log OUT";

                context.db_userlog.Add(newEntry);
                context.SaveChanges();
            }
        }
    }
}
