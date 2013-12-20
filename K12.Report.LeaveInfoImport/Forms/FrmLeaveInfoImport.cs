using Aspose.Cells;
using FISCA.Presentation.Controls;
using K12.Data;
using SHSchool.Data;
using SmartSchool.API.PlugIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Report.LeaveInfoImport.Forms
{
    public class FrmLeaveInfoImport : SmartSchool.API.PlugIn.Import.Importer
    {
        private LogHelper _LogHelper;

        // 更新
        private List<SHLeaveInfoRecord> _UpdateRecList;

        public FrmLeaveInfoImport()
        {
            this.Image = null;
            this.Text = Global._Title;
        }

        public override void InitializeImport(SmartSchool.API.PlugIn.Import.ImportWizard wizard)
        {
            // 學生資料, 準備用來判斷學生是否存在
            // key: 學生系統ID ; value: 學生資料
            Dictionary<string, StudentRecord> studentDic = new Dictionary<string, StudentRecord>();
            // key: 學生系統ID ; Value: 離校資訊
            Dictionary<string, SHLeaveInfoRecord> leaveInfoDic = new Dictionary<string, SHLeaveInfoRecord>();
            // 用來檢查是否有重複的資料
            List<string> checkSameList = new List<string>();

            wizard.PackageLimit = 300;
            // 離校學年度, 離校類別, 離校科別, 離校班級, 畢業證書字號
            wizard.ImportableFields.AddRange(new string[] { Global._ColLeaveScholYear,
                                                            Global._ColLeaveReason,
                                                            Global._ColLeaveDept,
                                                            Global._ColLeaveClassName,
                                                            Global._ColDiplomaNumber
                                                            });
            //wizard.RequiredFields.AddRange("欄位名稱");
            
            //#region 說明按鈕的內容
            //wizard.HelpButtonVisible = true;
            //wizard.HelpButtonClick += delegate(object sender, EventArgs e)
            //{
            //    new FrmHelp(new List<string>(),
            //                wizard.ImportableFields.ToList<string>()).ShowDialog();
            //};
            //#endregion 說明按鈕的內容

            #region "準備驗證資料"的事件
            wizard.ValidateStart += delegate(object sender, SmartSchool.API.PlugIn.Import.ValidateStartEventArgs e)
            {
                // 初始資料
                studentDic.Clear();
                leaveInfoDic.Clear();
                checkSameList.Clear();
                _LogHelper = new LogHelper();
                _UpdateRecList = new List<SHLeaveInfoRecord>();

                #region 取得需要的資料
                // 取得學生資料
                foreach (StudentRecord studRec in Student.SelectByIDs(e.List))
                    if (!studentDic.ContainsKey(studRec.ID))
                        studentDic.Add(studRec.ID, studRec);

                // 取得學生的離校資訊
                List<SHLeaveInfoRecord> leaveInfoList = SHLeaveInfo.SelectByStudentIDs(e.List);
                foreach (SHLeaveInfoRecord rec in leaveInfoList)
                {
                    if (!leaveInfoDic.ContainsKey(rec.RefStudentID))
                        leaveInfoDic.Add(rec.RefStudentID, rec);
                }
                #endregion
            };
            #endregion

            #region "開始驗證每一條row"的事件
            wizard.ValidateRow += delegate(object sender, SmartSchool.API.PlugIn.Import.ValidateRowEventArgs e)
            {
                #region 檢查學生是否存在
                if (!studentDic.ContainsKey(e.Data.ID))
                {
                    e.ErrorMessage = "沒有這位學生" + e.Data.ID;
                    return;
                }
                #endregion

                #region 檢查是否有重複的學生系統編號
                if (checkSameList.Contains(e.Data.ID))
                {
                    e.ErrorFields.Add(Global._ColStudentId, e.Data.ID + " 不允許重複");
                }
                else
	              {
                    checkSameList.Add(e.Data.ID);
	              }
                #endregion

                #region 檢查是否有這個學生的離校資訊
                if (!leaveInfoDic.ContainsKey(e.Data.ID))
                {
                    e.ErrorMessage = "無法取得這位學生" + e.Data.ID + "離校資訊";
                    return;
                }
                #endregion

                #region 驗證格式資料
                
                foreach (string field in e.SelectFields)
                {
                    string value = e.Data[field].Trim();
                    switch (field)
                    {
                        case Global._ColLeaveScholYear:
                            if (!string.IsNullOrEmpty(value) &&     // 非空字串
                                (Utility.ConvertStringToInt(value) == null) )   // 轉數字失敗
                            {
                                e.ErrorFields.Add(field, "非數字型態");
                            }
                            
                            break;
                        default:
                            break;
                    }
                }
                #endregion
            };
            #endregion

            #region "開始匯入資料"的事件
            wizard.ImportPackage += delegate(object sender, SmartSchool.API.PlugIn.Import.ImportPackageEventArgs e)
            {
                #region 處理每一筆row
                foreach (RowData row in e.Items)
                {
                    string studentId = row.ID;

                    // 取得該位學生的離校資訊
                    SHLeaveInfoRecord rec = leaveInfoDic[studentId];
                    
                    // [Log]儲存學生資料
                    _LogHelper.SaveStudentRecForLog(studentDic[studentId]);
                    // [Log]先儲存原本的資料
                    _LogHelper.SaveOldRecForLog(rec);

                    #region 處理需要更新資料
                    foreach (string field in e.ImportFields)
                    {
                        string value = row[field];
                        // 假如內容為空, 不處理
                        if (string.IsNullOrEmpty(value)) continue;

                        switch (field)
                        {
                            // 離校學年度
                            case Global._ColLeaveScholYear:
                                int? tmp = Utility.ConvertStringToInt(value);
                                rec.SchoolYear = tmp;
                                break;
                            // 離校類別
                            case Global._ColLeaveReason:
                                rec.Reason = value;
                                break;
                            // 離校科別
                            case Global._ColLeaveDept:
                                rec.DepartmentName = value;
                                break;
                            // 離校班級
                            case Global._ColLeaveClassName:
                                rec.ClassName = value;
                                break;
                            // 畢業證書字號
                            case Global._ColDiplomaNumber:
                                rec.DiplomaNumber = value;
                                break;
                            default:
                                break;
                        }

                    }
                    #endregion 處理需要更新資料

                    // [Log]儲存更新後的資料
                    _LogHelper.SaveNewRecForLog(rec);

                    // 加入更新名單
                    _UpdateRecList.Add(rec);
                }
                #endregion 處理每一筆row

                #region 更新DB資料
                if (_UpdateRecList.Count > 0)
                {
                    // 更新DB資料
                    SHLeaveInfo.Update(_UpdateRecList);

                    #region 處理Log
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("更新" + Global._Title + "：");
                    foreach (SHLeaveInfoRecord rec in _UpdateRecList)
                    {
                        if (_LogHelper.LeaveInfoPairDic.ContainsKey(rec.RefStudentID))
                            sb.AppendLine(_LogHelper.ComposeUpdateLogString(_LogHelper.LeaveInfoPairDic[rec.RefStudentID]));
                    }

                    FISCA.LogAgent.ApplicationLog.Log("學生." + Global._Title + "-匯入", "更新匯入", sb.ToString());
                    #endregion 處理Log
                }
                #endregion 更新DB資料
            };
            #endregion "開始匯入資料"的事件

            #region "匯入結束"的事件
            wizard.ImportComplete += delegate(object sender, EventArgs e)
            {
                // 更新系統暫存資料
                SmartSchool.Broadcaster.Events.Items["學生/資料變更"].Invoke(studentDic.Keys.ToList<string>());
            };
            #endregion "匯入結束"的事件
        }

    }
}
