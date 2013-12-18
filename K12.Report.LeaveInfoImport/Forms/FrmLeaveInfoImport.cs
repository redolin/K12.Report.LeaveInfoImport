using Campus.DocumentValidator;
using Campus.Import;
using SHSchool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Report.LeaveInfoImport.Forms
{
    public class FrmLeaveInfoImport : ImportWizard
    {
        // 設定檔
        private ImportOption _Option;

        private LogHelper _LogHelper;

        // 更新
        private List<SHLeaveInfoRecord> _UpdateRecList;

        public FrmLeaveInfoImport()
        {
            this.IsLog = false;
            this.IsSplit = false;
        }

        public override ImportAction GetSupportActions()
        {
            //只能更新
            return ImportAction.Update;
        }

        public override string GetValidateRule()
        {
            return Properties.Resources.ImportLeaveInfoRule;
        }

        public override void Prepare(ImportOption Option)
        {
            _Option = Option;
            _UpdateRecList = new List<SHLeaveInfoRecord>();
            _LogHelper = new LogHelper();
        }

        public override string Import(List<Campus.DocumentValidator.IRowStream> Rows)
        {
            if (_Option.Action == ImportAction.Update)
            {
                _UpdateRecList.Clear();

                // Key: 學生系統ID, Value: 學號
                Dictionary<string, string> studentIdDic = new Dictionary<string, string>();
                // key: 學生系統ID ; Value: 離校資訊
                Dictionary<string, SHLeaveInfoRecord> leaveInfoDic = new Dictionary<string, SHLeaveInfoRecord>();

                // 取得學生的系統ID
                foreach (IRowStream row in Rows)
                {
                    string studentNumber = Utility.GetIRowValueString(row, Global._ColStudentNumber);

                    if (string.IsNullOrEmpty(studentNumber)) continue;

                    if (Global._AllStudentNumberIDTemp.ContainsKey(studentNumber))
                    {
                        string studentId = Global._AllStudentNumberIDTemp[studentNumber];

                        if (!studentIdDic.ContainsKey(studentId))
                            studentIdDic.Add(studentId, studentNumber);
                    }
                }

                // 透過學生ID取得離校資訊
                List<SHLeaveInfoRecord> leaveInfoList = SHLeaveInfo.SelectByStudentIDs(studentIdDic.Keys.ToArray());
                foreach (SHLeaveInfoRecord rec in leaveInfoList)
                {
                    if (!leaveInfoDic.ContainsKey(rec.RefStudentID))
                        leaveInfoDic.Add(rec.RefStudentID, rec);
                }

                int totalCount = 0;
                #region 處理每一筆資料

                // 判斷每一筆資料是要新增還是更新
                foreach (IRowStream row in Rows)
                {
                    totalCount++;
                    this.ImportProgress = totalCount;

                    string studentId = "";
                    string studentNumber = Utility.GetIRowValueString(row, Global._ColStudentNumber);
                    SHLeaveInfoRecord rec;

                    // 透過學號換成學生ID
                    if (Global._AllStudentNumberIDTemp.ContainsKey(studentNumber))
                    {
                        studentId = Global._AllStudentNumberIDTemp[studentNumber];
                    }
                    else
                    {
                        // 如果無法取得學生ID, 就換到下一筆
                        continue;
                    }

                    // 理論上, 一定要有
                    if (leaveInfoDic.ContainsKey(studentId))
                    {
                        rec = leaveInfoDic[studentId];
                    }
                    else
                    {
                        // TODO, 這要怎麼處理
                        continue;
                    }

                    // 先儲存原本的資料
                    _LogHelper.SaveOldRecForLog(rec);

                    #region 處理更新資料
                    // 學生系統ID 無法更新

                    // 離校學年度
                    if (_Option.SelectedFields.Contains(Global._ColLeaveScholYear))
                    {
                        int? tmp = Utility.GetIRowValueInt(row, Global._ColLeaveScholYear);
                        if (tmp.HasValue)
                            rec.SchoolYear = tmp;
                    }

                    // 離校類別
                    if (_Option.SelectedFields.Contains(Global._ColLeaveCategory))
                    {
                        string tmp = Utility.GetIRowValueString(row, Global._ColLeaveCategory);
                        if (!string.IsNullOrEmpty(tmp))
                            rec.Reason = tmp;
                    }

                    // 離校科別
                    if (_Option.SelectedFields.Contains(Global._ColLeaveDept))
                    {
                        string tmp = Utility.GetIRowValueString(row, Global._ColLeaveDept);
                        if (!string.IsNullOrEmpty(tmp))
                            rec.DepartmentName = tmp;
                    }

                    // 離校班級
                    if (_Option.SelectedFields.Contains(Global._ColLeaveClassName))
                    {
                        string tmp = Utility.GetIRowValueString(row, Global._ColLeaveClassName);
                        if (!string.IsNullOrEmpty(tmp))
                            rec.ClassName = tmp;
                    }

                    #endregion

                    // 儲存更新後的資料
                    _LogHelper.SaveNewRecForLog(rec);

                    // 加入準備更新的列表
                    _UpdateRecList.Add(rec);

                }   // end of 判斷每一筆資料是要新增還是更新

                #endregion

                #region 更新資料
                
                // 執行更新
                if (_UpdateRecList.Count > 0)
                {
                    // 更新DB資料
                    SHLeaveInfo.Update(_UpdateRecList);

                    // 更新系統暫存資料
                    SmartSchool.Broadcaster.Events.Items["學生/資料變更"].Invoke(studentIdDic.Keys.ToArray());

                    #region 處理Log
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("更新匯入離校資訊：");
                    foreach (SHLeaveInfoRecord rec in _UpdateRecList)
                    {
                        string studentNumber = "";
                        if (studentIdDic.ContainsKey(rec.RefStudentID))
                            studentNumber = studentIdDic[rec.RefStudentID];
                        if (_LogHelper.LeaveInfoPairDic.ContainsKey(rec.RefStudentID))
                            sb.AppendLine(_LogHelper.ComposeUpdateLogString(_LogHelper.LeaveInfoPairDic[rec.RefStudentID], studentNumber));
                    }

                    FISCA.LogAgent.ApplicationLog.Log("學生.匯入離校資訊-匯入", "更新匯入", sb.ToString());
                    #endregion
                }
                #endregion

            }   // end of if (_Option.Action == ImportAction.Update)

            return "";
        }

    }
}
