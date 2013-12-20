using Aspose.Cells;
using FISCA.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace K12.Report.LeaveInfoImport.Forms
{
    public partial class FrmHelp : BaseForm
    {
        private List<string> _RequestFields;
        private List<string> _ImportableFields;
        
        public FrmHelp(List<string> requestFields, List<string> importableFields)
        {
            InitializeComponent();
            this._ImportableFields = importableFields;
            this._RequestFields = requestFields;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];
            Cells cells = sheet.Cells;

            // 輸出標題
            int columnIndex = 0;
            cells[0, columnIndex++].PutValue(Global._ColStudentId);
            cells[0, columnIndex++].PutValue(Global._ColStudentNumber);

            foreach (string str in _RequestFields)
            {
                cells[0, columnIndex++].PutValue(str);
            }

            foreach (string str in _ImportableFields)
            {
                cells[0, columnIndex++].PutValue(str);
            }

            // 儲存Excel
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "另存新檔";
            saveFileDialog1.FileName = Global._Title + "(空白格式)";
            saveFileDialog1.Filter = "Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SaveExcel(saveFileDialog1.FileName, wb);
            }
        }

        private void FrmHelp_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[必要欄位]");
            sb.Append(Global._ColStudentId).Append(" 或 ").Append(Global._ColStudentNumber).Append("、");
            foreach (string str in _RequestFields)
            {
                sb.Append(str).Append("、");
            }
            sb.Length = sb.Length - 1;
            sb.AppendLine();
            sb.AppendLine("[可匯入欄位]");
            foreach (string str in _ImportableFields)
            {
                sb.Append(str).Append("、");
            }
            sb.Length = sb.Length - 1;
            sb.AppendLine();
            sb.AppendLine("[備註]");
            sb.AppendLine("1. 欄位內容假如為空, 該欄位就不會被更新!");
            sb.AppendLine("2. 離校學年度假如不為空, 請輸入數字!");

            this.lblDescription.Text = sb.ToString();
        }

        private void SaveExcel(string path, Workbook wb)
        {
            #region 假如已有檔案, 嘗試刪除, 若無法刪除, 就在後面加數字
            if (File.Exists(path))
            {
                bool needCount = true;
                try
                {
                    File.Delete(path);
                    needCount = false;
                }
                catch { }
                int i = 1;
                while (needCount)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                    else
                    {
                        try
                        {
                            File.Delete(newPath);
                            path = newPath;
                            break;
                        }
                        catch { }
                    }
                }
            }
            #endregion

            #region 產生檔案
            try
            {
                File.Create(path).Close();
            }
            catch
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = Path.GetFileNameWithoutExtension(path) + ".xls";
                sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Create(sd.FileName);
                        path = sd.FileName;
                    }
                    catch
                    {
                        MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            #endregion
            
            // 儲存Excel
            wb.Save(path, FileFormatType.Excel2003);

            System.Diagnostics.Process.Start(path);
        }
    }
}
