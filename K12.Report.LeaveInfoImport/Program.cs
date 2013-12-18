using Campus.DocumentValidator;
using FISCA.Permission;
using FISCA.Presentation;
using K12.Report.LeaveInfoImport.Forms;
using K12.Report.LeaveInfoImport.ValidationRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.LeaveInfoImport
{
    public class Program
    {
        public static readonly bool IsDebug = false;

        [FISCA.MainMethod]
        public static void main()
        {
            #region 自訂驗證規則
            FactoryProvider.FieldFactory.Add(new LeaveInfoFieldValidatorFactory());
            #endregion

            RibbonBarButton btnReport = MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"];
            btnReport["學籍相關匯入"]["匯入離校資訊"].Enable = Permissions.IsEnableLeaveInfoImport;

            btnReport["學籍相關匯入"]["匯入離校資訊"].Click += delegate
            {
                // 準備所有一般生的學生ID, 之後驗證資料時會用到
                Global._AllStudentNumberIDTemp = DAO.FDQuery.GetAllStudenNumberDict();
                Forms.FrmLeaveInfoImport frmImport = new Forms.FrmLeaveInfoImport();
                frmImport.Execute();
            };

            // 在權限畫面出現"匯入社團學期成績"權限
            Catalog catalog1 = RoleAclSource.Instance["學生"]["匯出/匯入"];
            catalog1.Add(new RibbonFeature(Permissions.KeyLeaveInfoImport, "匯入離校資訊"));
        }
    }
}
