using SHSchool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Report.LeaveInfoImport
{
    public class Utility
    {
        ///// <summary>
        ///// 透過輸入的欄位名稱, 取得匯入資料的值
        ///// </summary>
        ///// <param name="row"></param>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public static string GetIRowValueString(IRowStream row, string name)
        //{
        //    if (row.Contains(name))
        //    {
        //        if (string.IsNullOrEmpty(row.GetValue(name)))
        //            return "";

        //        return row.GetValue(name).Trim();
        //    }
        //    else
        //        return "";
        //}

        ///// <summary>
        ///// 透過輸入的欄位名稱, 取得匯入資料的值, 並轉成int
        ///// </summary>
        ///// <param name="row"></param>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public static int? GetIRowValueInt(IRowStream row, string name)
        //{
        //    if (row.Contains(name))
        //    {
        //        if (string.IsNullOrEmpty(row.GetValue(name)))
        //            return null;
        //        int retVal;
        //        if (int.TryParse(row.GetValue(name).Trim(), out retVal))
        //            return retVal;
        //        else
        //            return null;
        //    }
        //    else
        //        return null;
        //}

        public static int? ConvertStringToInt(string str)
        {
            int result;
            if (string.IsNullOrEmpty(str))
                return null;

            if (int.TryParse(str, out result))
                return result;
            else
                return null;
        }

        /// <summary>
        /// 產生新的物件, 複製 "學號, 離校學年度, 離校類別, 離校科別, 離校班級, 畢業證書字號" 的資訊
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public static SHLeaveInfoRecord CopySHLeaveInfoRecord(SHLeaveInfoRecord rec)
        {
            SHLeaveInfoRecord newRec = new SHLeaveInfoRecord();

            newRec.RefStudentID = rec.RefStudentID;
            newRec.SchoolYear = rec.SchoolYear;
            newRec.Reason = rec.Reason;
            newRec.DepartmentName = rec.DepartmentName;
            newRec.ClassName = rec.ClassName;
            newRec.DiplomaNumber = rec.DiplomaNumber;
            
            return newRec;
        }
    }
}
