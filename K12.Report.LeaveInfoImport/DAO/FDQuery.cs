﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace K12.Report.LeaveInfoImport.DAO
{
    /// <summary>
    /// 使用 FISCA.Data Query
    /// </summary>
    public class FDQuery
    {
        public const string _StudentStatus = "1";

        /// <summary>
        /// 取得所有學生學號 key:StudentNumber ; value:StudentID
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllStudenNumberDict()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            StringBuilder sb = new StringBuilder();

            sb.Append("select student.student_number,student.id");
            sb.Append(" from student");
            sb.Append(" where student.status='" + _StudentStatus + "'");

            if (Program.IsDebug == true) Console.WriteLine("[GetAllStudenNumberDict] sql: [" + sb.ToString() + "]");
            
            DataTable dt = Global._Q.Select(sb.ToString());
            foreach (DataRow row in dt.Rows)
            {
                string key = (""+row["student_number"]).Trim();
                string value = ("" + row["id"]).Trim();

                if (string.IsNullOrEmpty(key)) continue;

                if (!retVal.ContainsKey(key))
                    retVal.Add(key, value);
            }

            return retVal;
        }

    }
}
