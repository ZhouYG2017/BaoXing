using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.DDF;
using System.Collections;
using System.Text;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using NPOI.HSSF.Util;
using System.Web.UI;
using System.Web;
using NPOI.HPSF;

namespace NFine.Code.Excel
{
    public class NPOIHelper
    {

        private static ICellStyle GetHeadStyle(HSSFWorkbook workbook)
        {
            //表头样式
            var headStyle = workbook.CreateCellStyle();
            headStyle.Alignment = HorizontalAlignment.Left;//居中对齐
            //表头单元格背景色
            headStyle.FillForegroundColor = HSSFColor.LightGreen.Index;
            headStyle.FillPattern = FillPattern.SolidForeground;
            //表头单元格边框
            headStyle.BorderTop = BorderStyle.Thin;
            headStyle.TopBorderColor = HSSFColor.Black.Index;
            headStyle.BorderRight = BorderStyle.Thin;
            headStyle.RightBorderColor = HSSFColor.Black.Index;
            headStyle.BorderBottom = BorderStyle.Thin;
            headStyle.BottomBorderColor = HSSFColor.Black.Index;
            headStyle.BorderLeft = BorderStyle.Thin;
            headStyle.LeftBorderColor = HSSFColor.Black.Index;
            //表头字体设置
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 12;//字号
            font.Boldweight = 600;//加粗
            //font.Color = HSSFColor.WHITE.index;//颜色
            headStyle.SetFont(font);

            return headStyle;
        }

        private static ICellStyle GetDataStyle(HSSFWorkbook workbook)
        {
            //数据样式
            var dataStyle = workbook.CreateCellStyle();
            dataStyle.Alignment = HorizontalAlignment.Left;//左对齐
            //数据单元格的边框
            dataStyle.BorderTop = BorderStyle.Thin;
            dataStyle.TopBorderColor = HSSFColor.Black.Index;
            dataStyle.BorderRight = BorderStyle.Thin;
            dataStyle.RightBorderColor = HSSFColor.Black.Index;
            dataStyle.BorderBottom = BorderStyle.Thin;
            dataStyle.BottomBorderColor = HSSFColor.Black.Index;
            dataStyle.BorderLeft = BorderStyle.Thin;
            dataStyle.LeftBorderColor = HSSFColor.Black.Index;
            //数据的字体
            var datafont = workbook.CreateFont();
            datafont.FontHeightInPoints = 11;//字号
            dataStyle.SetFont(datafont);

            return dataStyle;
        }
        #region 从DataSet导出到MemoryStream流2007 
        /// <summary> 
        /// 从DataSet导出到MemoryStream流2007 
        /// </summary> 
        /// <param name="SaveFileName">文件保存路径</param> 
        /// <param name="SheetName">Excel文件中的Sheet名称</param> 
        /// <param name="ds">存储数据的DataSet</param> 
        /// <param name="startRow">从哪一行开始写入，从0开始</param> 
        /// <param name="datatypes">DataSet中的各列对应的数据类型</param> 
        public bool CreateExcel2007(string SaveFileName, string SheetName, DataTable ds, int startRow, params NpoiDataType[] datatypes)
        {
            try
            {
                if (startRow < 0) startRow = 0;
                HSSFWorkbook wb = new HSSFWorkbook();
                ISheet sheet = wb.CreateSheet(SheetName);
                //sheet.SetColumnWidth(0, 50 * 256); 
                //sheet.SetColumnWidth(1, 100 * 256); 
                IRow row;
                ICell cell;
                DataRow dr;
                int j;
                int maxLength = 0;
                int curLength = 0;
                object columnValue;
                DataTable dt = ds;
                if (datatypes.Length < dt.Columns.Count)
                {
                    datatypes = new NpoiDataType[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string dtcolumntype = dt.Columns[i].DataType.Name.ToLower();
                        switch (dtcolumntype)
                        {
                            case "string":
                                datatypes[i] = NpoiDataType.String;
                                break;
                            case "datetime":
                                datatypes[i] = NpoiDataType.Datetime;
                                break;
                            case "boolean":
                                datatypes[i] = NpoiDataType.Bool;
                                break;
                            case "double":
                                datatypes[i] = NpoiDataType.Numeric;
                                break;
                            default:
                                datatypes[i] = NpoiDataType.String;
                                break;
                        }
                    }
                }

                #region 创建表头 
                row = sheet.CreateRow(0);//创建第i行 
                ICellStyle style1 = wb.CreateCellStyle();//样式 
                IFont font1 = wb.CreateFont();//字体 

                font1.Color = HSSFColor.White.Index;//字体颜色 
                font1.Boldweight = (short)FontBoldWeight.Bold;//字体加粗样式 
                                                              //style1.FillBackgroundColor = HSSFColor.WHITE.index;//GetXLColour(wb, LevelOneColor);// 设置图案色 
                style1.FillForegroundColor = HSSFColor.Green.Index;//GetXLColour(wb, LevelOneColor);// 设置背景色 
                style1.FillPattern = FillPattern.SolidForeground;
                style1.SetFont(font1);//样式里的字体设置具体的字体样式 
                style1.Alignment = HorizontalAlignment.General;//文字水平对齐方式 
                style1.VerticalAlignment = VerticalAlignment.Center;//文字垂直对齐方式 
                row.HeightInPoints = 25;
                for (j = 0; j < dt.Columns.Count; j++)
                {
                    columnValue = dt.Columns[j].ColumnName;
                    curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                    maxLength = (maxLength < curLength ? curLength : maxLength);
                    int colounwidth = 256 * maxLength;
                    sheet.SetColumnWidth(j, colounwidth);
                    try
                    {
                        cell = row.CreateCell(j);//创建第0行的第j列 
                        cell.CellStyle = style1;//单元格式设置样式 

                        try
                        {
                            //cell.SetCellType(CellType.STRING); 
                            cell.SetCellValue(columnValue.ToString());
                        }
                        catch { }

                    }
                    catch
                    {
                        continue;
                    }
                }
                #endregion

                #region 创建每一行 
                for (int i = startRow; i < ds.Rows.Count; i++)
                {
                    dr = ds.Rows[i];
                    row = sheet.CreateRow(i + 1);//创建第i行 
                    for (j = 0; j < dt.Columns.Count; j++)
                    {
                        columnValue = dr[j];
                        curLength = Encoding.Default.GetByteCount(columnValue.ToString());
                        maxLength = (maxLength < curLength ? curLength : maxLength);
                        int colounwidth = 256 * maxLength;
                        sheet.SetColumnWidth(j, colounwidth);
                        try
                        {
                            cell = row.CreateCell(j);//创建第i行的第j列 
                            #region 插入第j列的数据 
                            try
                            {
                                NpoiDataType dtype = datatypes[j];
                                switch (dtype)
                                {
                                    case NpoiDataType.String:
                                        {
                                            //cell.SetCellType(CellType.STRING); 
                                            cell.SetCellValue(columnValue.ToString());
                                        }
                                        break;
                                    case NpoiDataType.Datetime:
                                        {
                                            // cell.SetCellType(CellType.STRING); 
                                            cell.SetCellValue(columnValue.ToString());
                                        }
                                        break;
                                    case NpoiDataType.Numeric:
                                        {
                                            //cell.SetCellType(CellType.NUMERIC); 
                                            cell.SetCellValue(Convert.ToDouble(columnValue));
                                        }
                                        break;
                                    case NpoiDataType.Bool:
                                        {
                                            //cell.SetCellType(CellType.BOOLEAN); 
                                            cell.SetCellValue(Convert.ToBoolean(columnValue));
                                        }
                                        break;
                                    case NpoiDataType.Richtext:
                                        {
                                            // cell.SetCellType(CellType.FORMULA); 
                                            cell.SetCellValue(columnValue.ToString());
                                        }
                                        break;
                                }
                            }
                            catch
                            {
                                //cell.SetCellType(HSSFCell.CELL_TYPE_STRING); 
                                cell.SetCellValue(columnValue.ToString());
                            }
                            #endregion

                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                #endregion

                //using (FileStream fs = new FileStream(@SaveFileName, FileMode.OpenOrCreate))//生成文件在服务器上 
                //{ 
                //    wb.Write(fs); 
                //} 
                //string SaveFileName = "output.xlsx"; 
                using (FileStream fs = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write))//生成文件在服务器上 
                {
                    wb.Write(fs);
                    Console.WriteLine("文件保存成功！" + SaveFileName);
                }

                return true;
            }
            catch (Exception er)
            {
                Console.WriteLine("文件保存失败！" + SaveFileName);
                return false;
            }

        }
        #endregion
        /// <summary>
        /// 向客户端输出文件。
        /// </summary>
        /// <param name="table">数据表。</param>
        /// <param name="headerText">头部文本。</param>
        /// <param name="sheetName"></param>
        /// <param name="columnName">数据列名称。</param>
        /// <param name="columnTitle">表标题。</param>
        /// <param name="fileName">文件名称。</param>
        public void Write(DataTable table, string headerText, string sheetName, string[] columnName, string[] columnTitle, string fileName)
        {
            HttpContext context = HttpContext.Current;
            context.Response.ContentType = "application/vnd.ms-excel";
            context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", HttpUtility.UrlEncode(fileName, Encoding.UTF8)));
            context.Response.Clear();
            HSSFWorkbook hssfworkbook = GenerateData(table, headerText, sheetName, columnName, columnTitle);
            context.Response.BinaryWrite(WriteToStream(hssfworkbook).GetBuffer());
            context.Response.End();
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="table"></param>
        /// <param name="headerText"></param>
        /// <param name="sheetName"></param>
        /// <param name="columnName"></param>
        /// <param name="columnTitle"></param>
        /// <returns></returns>
        public static HSSFWorkbook GenerateData(DataTable table, string headerText, string sheetName, string[] columnName, string[] columnTitle)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet(sheetName);

            #region 设置文件属性信息

            //创建一个文档摘要信息实体。
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Weilog Team"; //公司名称
            hssfworkbook.DocumentSummaryInformation = dsi;

            //创建一个摘要信息实体。
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "本文档由 Weilog 系统生成";
            si.Author = " Weilog 系统";
            si.Title = headerText;
            si.Subject = headerText;
            si.CreateDateTime = DateTime.Now;
            hssfworkbook.SummaryInformation = si;

            #endregion

            ICellStyle dateStyle = hssfworkbook.CreateCellStyle();
            IDataFormat format = hssfworkbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            #region 取得列宽

            int[] colWidth = new int[columnName.Length];
            for (int i = 0; i < columnName.Length; i++)
            {
                colWidth[i] = Encoding.GetEncoding(936).GetBytes(columnTitle[i]).Length;
            }
            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < columnName.Length; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(table.Rows[i][columnName[j]].ToString()).Length;
                    if (intTemp > colWidth[j])
                    {
                        colWidth[j] = intTemp;
                    }
                }
            }

            #endregion

            int rowIndex = 0;
            foreach (DataRow row in table.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = hssfworkbook.CreateSheet(sheetName + ((int)rowIndex / 65535).ToString());
                    }

                    #region 表头及样式
                    //if (!string.IsNullOrEmpty(headerText))
                    {
                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(headerText);

                        ICellStyle headStyle = hssfworkbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = hssfworkbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        headerRow.GetCell(0).CellStyle = headStyle;
                        //sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, table.Columns.Count - 1));
                    }
                    #endregion

                    #region 列头及样式
                    {
                        //HSSFRow headerRow = sheet.CreateRow(1);
                        IRow headerRow;
                        //if (!string.IsNullOrEmpty(headerText))
                        //{
                        //    headerRow = sheet.CreateRow(0);
                        //}
                        //else
                        //{
                        headerRow = sheet.CreateRow(1);
                        //}
                        ICellStyle headStyle = hssfworkbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = hssfworkbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        for (int i = 0; i < columnName.Length; i++)
                        {
                            headerRow.CreateCell(i).SetCellValue(columnTitle[i]);
                            headerRow.GetCell(i).CellStyle = headStyle;
                            //设置列宽
                            if ((colWidth[i] + 1) * 256 > 30000)
                            {
                                sheet.SetColumnWidth(i, 10000);
                            }
                            else
                            {
                                sheet.SetColumnWidth(i, (colWidth[i] + 1) * 256);
                            }
                        }
                        /*
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
    
                            //设置列宽   
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }
                         * */
                    }
                    #endregion
                    //if (!string.IsNullOrEmpty(headerText))
                    //{
                    //    rowIndex = 1;
                    //}
                    //else
                    //{
                    rowIndex = 2;
                    //}

                }
                #endregion

                #region 填充数据

                IRow dataRow = sheet.CreateRow(rowIndex);
                for (int i = 0; i < columnName.Length; i++)
                {
                    ICell newCell = dataRow.CreateCell(i);

                    string drValue = row[columnName[i]].ToString();

                    switch (table.Columns[columnName[i]].DataType.ToString())
                    {
                        case "System.String"://字符串类型  
                            if (drValue.ToUpper() == "TRUE")
                                newCell.SetCellValue("是");
                            else if (drValue.ToUpper() == "FALSE")
                                newCell.SetCellValue("否");
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型   
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle;//格式化显示   
                            break;
                        case "System.Boolean"://布尔型   
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            if (boolV)
                                newCell.SetCellValue("是");
                            else
                                newCell.SetCellValue("否");
                            break;
                        case "System.Int16"://整型   
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型   
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理   
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }

                #endregion

                rowIndex++;
            }

            return hssfworkbook;
        }
        private static MemoryStream WriteToStream(HSSFWorkbook hssfworkbook)
        {
            //Write the stream data of workbook to the root directory
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);
            return file;
        }
        #region 读取Excel文件内容转换为DataSet 
        /// <summary> 
        /// 读取Excel文件内容转换为DataSet,列名依次为 "c0"……c[columnlength-1] 
        /// </summary> 
        /// <param name="FileName">文件绝对路径</param> 
        /// <param name="startRow">数据开始行数(1为第一行)</param> 
        /// <param name="ColumnDataType">每列的数据类型</param> 
        /// <returns></returns> 
        public DataSet ReadExcel(string FileName, int startRow, params NpoiDataType[] ColumnDataType)
        {
            int ertime = 0;
            int intime = 0;
            DataSet ds = new DataSet("ds");
            DataTable dt = new DataTable("dt");
            DataRow dr;
            StringBuilder sb = new StringBuilder();
            string extension = System.IO.Path.GetExtension(FileName);
            using (FileStream stream = new FileStream(@FileName, FileMode.Open, FileAccess.Read))
            {

                //IWorkbook workbook = WorkbookFactory.Create(stream);//使用接口，自动识别excel2003/2007格式 
                IWorkbook wk = null;
                if (extension.Equals(".xls"))
                {
                    //把xls文件中的数据写入wk中
                    wk = new HSSFWorkbook(stream);
                }
                //else
                //{
                //    //支持2007以上版本的导入
                //    wk = new XSSFWorkbook(stream);
                //}
                ISheet sheet = wk.GetSheetAt(0);//得到里面第一个sheet 
                int j;
                IRow row;
                #region ColumnDataType赋值 
                if (ColumnDataType.Length <= 0)
                {
                    row = sheet.GetRow(startRow - 1);//得到第i行 
                    ColumnDataType = new NpoiDataType[row.LastCellNum];
                    for (int i = 0; i < row.LastCellNum; i++)
                    {
                        ICell hs = row.GetCell(i);
                        ColumnDataType[i] = GetCellDataType(hs);
                    }
                }
                #endregion
                for (j = 0; j < ColumnDataType.Length; j++)
                {
                    Type tp = GetDataTableType(ColumnDataType[j]);
                    dt.Columns.Add("c" + j, tp);
                }
                for (int i = startRow - 1; i <= sheet.PhysicalNumberOfRows; i++)
                {
                    row = sheet.GetRow(i);//得到第i行 
                    if (row == null) continue;
                    try
                    {
                        dr = dt.NewRow();

                        for (j = 0; j < ColumnDataType.Length; j++)
                        {
                            dr["c" + j] = GetCellData(ColumnDataType[j], row, j);
                        }
                        dt.Rows.Add(dr);
                        intime++;
                    }
                    catch (Exception er)
                    {
                        ertime++;
                        sb.Append(string.Format("第{0}行出错：{1}\r\n", i + 1, er.Message));
                        continue;
                    }
                }
                ds.Tables.Add(dt);
            }
            if (ds.Tables[0].Rows.Count == 0 && sb.ToString() != "") throw new Exception(sb.ToString());
            return ds;
        }
        //public DataTable ReadExcel(string FileName, int startRow, string sheetName)
        //{
        //    FileStream stream = File.Open(FileName, FileMode.Open, FileAccess.Read);
        //    string extension = System.IO.Path.GetExtension(FileName);
        //    IExcelDataReader excelReader;
        //    if (extension.Equals(".xls"))
        //    {
        //        //把xls文件中的数据写入wk中
        //        excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
        //    }
        //    else
        //    {
        //        //支持2007以上版本的导入
        //        excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        //    }

        //    //Choose one of either 3, 4, or 5
        //    //3. DataSet - The result of each spreadsheet will be created in the result.Tables
        //    try
        //    {
        //        return excelReader.AsDataSet().Tables[sheetName];
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        #endregion
        #region 读Excel-得到不同数据类型单元格的数据  
        /// <summary>  
        /// 读Excel-得到不同数据类型单元格的数据  
        /// </summary>  
        /// <param name="datatype">数据类型</param>  
        /// <param name="row">数据中的一行</param>  
        /// <param name="column">哪列</param>  
        /// <returns></returns>  
        private object GetCellData(NpoiDataType datatype, IRow row, int column)
        {

            switch (datatype)
            {
                case NpoiDataType.String:
                    try
                    {
                        return row.GetCell(column).StringCellValue;
                    }
                    catch { }
                    try
                    {
                        return row.GetCell(column).RichStringCellValue;
                    }
                    catch { }
                    try
                    {
                        return row.GetCell(column).NumericCellValue;
                    }
                    catch
                    {
                        return "";
                    }
                case NpoiDataType.Bool:
                    try { return row.GetCell(column).BooleanCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Datetime:
                    try { return row.GetCell(column).DateCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Numeric:
                    try { return row.GetCell(column).NumericCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Richtext:
                    try { return row.GetCell(column).RichStringCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Error:
                    try { return row.GetCell(column).ErrorCellValue; }
                    catch { return row.GetCell(column).StringCellValue; }
                case NpoiDataType.Blank:
                    try { return row.GetCell(column).StringCellValue; }
                    catch { return ""; }
                default: return "";
            }
        }
        #endregion

        #region 获取单元格数据类型 
        /// <summary> 
        /// 获取单元格数据类型 
        /// </summary> 
        /// <param name="hs"></param> 
        /// <returns></returns> 
        private NpoiDataType GetCellDataType(ICell hs)
        {
            NpoiDataType dtype;
            DateTime t1;
            string cellvalue = "";

            switch (hs.CellType)
            {
                case CellType.Blank:
                    dtype = NpoiDataType.String;
                    cellvalue = hs.StringCellValue;
                    break;
                case CellType.Boolean:
                    dtype = NpoiDataType.Bool;
                    break;
                case CellType.Numeric:
                    dtype = NpoiDataType.Numeric;
                    cellvalue = hs.NumericCellValue.ToString();
                    break;
                case CellType.String:
                    dtype = NpoiDataType.String;
                    cellvalue = hs.StringCellValue;
                    break;
                case CellType.Error:
                    dtype = NpoiDataType.Error;
                    break;
                case CellType.Formula:
                default:
                    dtype = NpoiDataType.Datetime;
                    break;
            }
            if (cellvalue != "" && DateTime.TryParse(cellvalue, out t1)) dtype = NpoiDataType.Datetime;
            return dtype;
        }
        #endregion
        #region 读Excel-根据NpoiDataType创建的DataTable列的数据类型  
        /// <summary>  
        /// 读Excel-根据NpoiDataType创建的DataTable列的数据类型  
        /// </summary>  
        /// <param name="datatype"></param>  
        /// <returns></returns>  
        private Type GetDataTableType(NpoiDataType datatype)
        {
            Type tp = typeof(string);//Type.GetType("System.String")  
            switch (datatype)
            {
                case NpoiDataType.Bool:
                    tp = typeof(bool);
                    break;
                case NpoiDataType.Datetime:
                    tp = typeof(DateTime);
                    break;
                case NpoiDataType.Numeric:
                    tp = typeof(double);
                    break;
                case NpoiDataType.Error:
                    tp = typeof(string);
                    break;
                case NpoiDataType.Blank:
                    tp = typeof(string);
                    break;
            }
            return tp;
        }
        #endregion

    }
    #region 枚举(Excel单元格数据类型) 
    /// <summary> 
    /// 枚举(Excel单元格数据类型) 
    /// </summary> 
    public enum NpoiDataType
    {
        /// <summary> 
        /// 字符串类型-值为1 
        /// </summary> 
        String,
        /// <summary> 
        /// 布尔类型-值为2 
        /// </summary> 
        Bool,
        /// <summary> 
        /// 时间类型-值为3 
        /// </summary> 
        Datetime,
        /// <summary> 
        /// 数字类型-值为4 
        /// </summary> 
        Numeric,
        /// <summary> 
        /// 复杂文本类型-值为5 
        /// </summary> 
        Richtext,
        /// <summary> 
        /// 空白 
        /// </summary> 
        Blank,
        /// <summary> 
        /// 错误 
        /// </summary> 
        Error
    }
    #endregion
}
