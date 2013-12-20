using FISCA.Permission;
using FISCA.Presentation;
using K12.Report.LeaveInfoImport.Forms;
using SmartSchool.API.PlugIn.Import;
using SmartSchool.StudentRelated.RibbonBars.Import;
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
            RibbonBarButton btnReport = MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"];
            btnReport["學籍相關匯入"]["匯入離校資訊"].Enable = Permissions.IsEnableLeaveInfoImport;

            btnReport["學籍相關匯入"]["匯入離校資訊"].Click += delegate
            {
                Importer importer = new FrmLeaveInfoImport();
                ImportStudentV2 wizard = new ImportStudentV2(importer.Text, importer.Image);
                importer.InitializeImport(wizard);
                wizard.ShowDialog();
            };

            // 在權限畫面出現"匯入社團學期成績"權限
            Catalog catalog1 = RoleAclSource.Instance["學生"]["匯出/匯入"];
            catalog1.Add(new RibbonFeature(Permissions.KeyLeaveInfoImport, Global._Title));
        }
    }
}
