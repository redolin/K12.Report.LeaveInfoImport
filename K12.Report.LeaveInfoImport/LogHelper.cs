using K12.Data;
using SHSchool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Report.LeaveInfoImport
{
    public class LogHelper
    {
        // key: 學生系統ID; value: 新舊資料
        private Dictionary<string, ObjList> leaveInfoPairDic = new Dictionary<string, ObjList>();

        /// <summary>
        /// Key: UID, Value: 原始跟更新後的社團學期成績
        /// </summary>
        public Dictionary<string, ObjList> LeaveInfoPairDic
        {
            get
            {
                return leaveInfoPairDic;
            }
        }

        /// <summary>
        /// 新增尚未更新的原始資料
        /// </summary>
        /// <param name="rec"></param>
        public void SaveOldRecForLog(SHLeaveInfoRecord rec)
        {
            if (!leaveInfoPairDic.ContainsKey(rec.RefStudentID))
                leaveInfoPairDic.Add(rec.RefStudentID, new ObjList());
            leaveInfoPairDic[rec.RefStudentID]._OldRec = Utility.CopySHLeaveInfoRecord(rec);
        }

        /// <summary>
        /// 新增更新後的資料
        /// </summary>
        /// <param name="rec"></param>
        public void SaveNewRecForLog(SHLeaveInfoRecord rec)
        {
            if (!leaveInfoPairDic.ContainsKey(rec.RefStudentID))
                leaveInfoPairDic.Add(rec.RefStudentID, new ObjList());
            leaveInfoPairDic[rec.RefStudentID]._NewRec = Utility.CopySHLeaveInfoRecord(rec);
        }

        /// <summary>
        /// 儲存學生資料
        /// </summary>
        /// <param name="rec"></param>
        public void SaveStudentRecForLog(StudentRecord rec)
        {
            
            if (!leaveInfoPairDic.ContainsKey(rec.ID))
                leaveInfoPairDic.Add(rec.ID, new ObjList());
            leaveInfoPairDic[rec.ID]._StudentRec = rec;
        }

        ///// <summary>
        ///// 取得新增的log字串
        ///// </summary>
        ///// <param name="rec"></param>
        ///// <param name="studentNumber"></param>
        ///// <returns></returns>
        //public string ComposeInsertLogString(DAO.ResultScoreRecord rec, string studentNumber)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append(Global._ColScholYear).Append("「").Append(rec.SchoolYear).Append("」");
        //    sb.Append(Global._ColSemester).Append("「").Append(rec.Semester).Append("」");
        //    sb.Append(Global._ColClubName).Append("「").Append(rec.ClubName).Append("」");
        //    sb.Append(Global.NewLine);

        //    sb.Append(Global._ColStudentNumber).Append("「").Append(studentNumber).Append("」");
        //    sb.Append(Global.NewLine);

        //    if (rec.ResultScore.HasValue)
        //    {
        //        sb.Append(Global._ColClubScore).Append("「").Append(rec.ResultScore.Value).Append("」");
        //        sb.Append(Global.NewLine);
        //    }
        //    else
        //    {
        //        sb.Append(Global._ColClubScore).Append("「").Append("」");
        //        sb.Append(Global.NewLine);
        //    }

        //    sb.Append(Global._ColCadreName).Append("「").Append(rec.CadreName).Append("」");
        //    sb.Append(Global.NewLine);

        //    return sb.ToString();
        //}

        /// <summary>
        /// 取得更新的log字串
        /// </summary>
        /// <param name="recUid"></param>
        /// <param name="studentNumber"></param>
        /// <returns></returns>
        public string ComposeUpdateLogString(ObjList pair)
        {
            //檢查與確認資料是否被修改
            StringBuilder sb = new StringBuilder();

            sb.Append(Global._ColStudentNumber).Append("「").Append(pair._StudentRec.StudentNumber).Append("」");
            sb.Append("學生姓名「").Append(pair._StudentRec.Name).Append("」");
            sb.Append(Global.NewLine);

            // 多判斷新資料假如是null, 表示沒有更新
            if ((pair._OldRec.SchoolYear != pair._NewRec.SchoolYear) && (pair._NewRec.SchoolYear.HasValue))
                sb.AppendLine(ByOne(Global._ColLeaveScholYear, pair._OldRec.SchoolYear, pair._NewRec.SchoolYear));

            // 多判斷新資料假如是"", 表示沒有更新
            if ((pair._OldRec.Reason != pair._NewRec.Reason) && (!string.IsNullOrEmpty(pair._NewRec.Reason)))
                sb.AppendLine(ByOne(Global._ColLeaveReason, pair._OldRec.Reason, pair._NewRec.Reason));

            // 多判斷新資料假如是"", 表示沒有更新
            if ((pair._OldRec.DepartmentName != pair._NewRec.DepartmentName) && (!string.IsNullOrEmpty(pair._NewRec.DepartmentName)))
                sb.AppendLine(ByOne(Global._ColLeaveDept, pair._OldRec.DepartmentName, pair._NewRec.DepartmentName));

            // 多判斷新資料假如是"", 表示沒有更新
            if ((pair._OldRec.ClassName != pair._NewRec.ClassName) && (!string.IsNullOrEmpty(pair._NewRec.ClassName)))
                sb.AppendLine(ByOne(Global._ColLeaveClassName, pair._OldRec.ClassName, pair._NewRec.ClassName));

            // 多判斷新資料假如是"", 表示沒有更新
            if ((pair._OldRec.DiplomaNumber != pair._NewRec.DiplomaNumber) && (!string.IsNullOrEmpty(pair._NewRec.DiplomaNumber)))
                sb.AppendLine(ByOne(Global._ColDiplomaNumber, pair._OldRec.DiplomaNumber, pair._NewRec.DiplomaNumber));

            return sb.ToString();
            
        }

        private string ByOne(string name, string oldValue, string newValue)
        {
            return string.Format("「{0}」由「{1}」修改為「{2}」", name, oldValue, newValue);
        }

        private string ByOne(string name, int? oldValue, int? newValue)
        {
            return ByOne(name, oldValue.HasValue ? "" + oldValue.Value : "", newValue.HasValue ? "" + newValue.Value : "");
        }
    }

    public class ObjList
    {
        public SHLeaveInfoRecord _OldRec;
        public SHLeaveInfoRecord _NewRec;
        public StudentRecord _StudentRec;
    }
}
