using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Report.LeaveInfoImport
{
    public class Permissions
    {
        public const string KeyLeaveInfoImport = "K12.Report.LeaveInfoImport.cs";

        public static bool IsEnableLeaveInfoImport
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[KeyLeaveInfoImport].Executable;
            }
        }
    }
}
