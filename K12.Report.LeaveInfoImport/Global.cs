using FISCA.Data;
using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Report.LeaveInfoImport
{
    public static class Global
    {
        public static AccessHelper _A = new AccessHelper();
        public static QueryHelper _Q = new QueryHelper();

        public static readonly string NewLine = "\r\n";

        #region 匯入的欄位名稱
        /// <summary>
        /// 學號
        /// </summary>
        public static readonly string _ColStudentNumber = "學號";
        /// <summary>
        /// 離校學年度
        /// </summary>
        public static readonly string _ColLeaveScholYear = "離校學年度";
        /// <summary>
        /// 離校類別
        /// </summary>
        public static readonly string _ColLeaveCategory = "離校類別";
        /// <summary>
        /// 離校科別
        /// </summary>
        public static readonly string _ColLeaveDept = "離校科別";
        /// <summary>
        /// 離校班級
        /// </summary>
        public static readonly string _ColLeaveClassName = "離校班級";
        #endregion

        /// <summary>
        /// 所有學生學號與ID的暫存 key:StudentNumber; value:StudentID
        /// </summary>
        public static Dictionary<string, string> _AllStudentNumberIDTemp = new Dictionary<string, string>();
    }
}
