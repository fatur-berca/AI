using Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using TOM.Master.BusinessLogics;

namespace hms_tom_dev.Helper
{
    public static class DataTableHelper
    {

    }

    /// <summary>
    /// Data table wrapper for JQuery.DataTable
    /// </summary>
    public class JQueryDataTable<DataType>
    {
        public bool destroy { get; set; }
        public List<DataType> data { get; set; }
        public bool paging { get; set; }
        public bool sort { get; set; }
        public bool searching { get; set; }
        public List<JQueryDataTableColumn> columns { get; set; }

        public JQueryDataTable()
        {
            data = new List<DataType>();
            columns = new List<JQueryDataTableColumn>();
        }
    }

    public struct JQueryDataTableColumn
    {
        public string data;
        public bool searchable;
        public bool sort;
    }

    public class ExcelHelper
    {
        public DataSet ExcelData { get; private set; }
        public Dictionary<string, string> HeaderMap { get; private set; }
        public Dictionary<int, string> ErrorLog { get; private set; }

        public ExcelHelper(Stream excelStream, bool openXml = true, bool isFirstRowAsColumnNames = true)
        {
            IExcelDataReader excel;
            if (!openXml)
                excel = ExcelReaderFactory.CreateBinaryReader(excelStream);
            else
                excel = ExcelReaderFactory.CreateOpenXmlReader(excelStream);
            excel.IsFirstRowAsColumnNames = isFirstRowAsColumnNames;
            ExcelData = excel.AsDataSet();
            ErrorLog = new Dictionary<int, string>();
            HeaderMap = new Dictionary<string, string>();
            excel.Close();
        }

        public List<TA> MapTable<TA>(DataTable table, Func<TA, TA> callback = null) where TA : class
        {
            List<TA> res = new List<TA>();
            var typA = typeof(TA);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                try
                {
                    var ele = Activator.CreateInstance<TA>();
                    var obj = new Dictionary<string, object>();
                    foreach (DataColumn col in table.Columns)
                    {
                        if (HeaderMap.ContainsKey(col.ColumnName))
                        {
                            try
                            {
                                var pn = HeaderMap[col.ColumnName];
                                var pi = typA.GetProperty(pn);
                                var fi = typA.GetField(pn);
                                var val = row[col];

                                var vtype = val != null ? val.GetType() : null;
                                var ctype = pi != null ? pi.PropertyType : fi != null ? fi.FieldType : null;

                                if (ctype != null)
                                {
                                    if (vtype == typeof(decimal) && ctype == typeof(double))
                                        val = Convert.ToDouble((decimal)val);
                                    if (vtype == typeof(double) && ctype == typeof(decimal))
                                        val = Convert.ToDecimal((double)val);
                                    if (vtype == typeof(double) && ctype == typeof(Int32))
                                        val = Convert.ToInt32((double)val);
                                    if (vtype == typeof(double) && ctype == typeof(byte))
                                        val = Convert.ToByte((double)val);
                                    if (ctype == typeof(string))
                                        val = val is DBNull ? null : val == null ? null : val.ToString();
                                }

                                if (pi != null && pi.CanWrite) pi.SetValue(ele, val);
                                if (fi != null && fi.IsPublic) fi.SetValue(ele, val);
                            }
                            catch (Exception exc)
                            {
                                throw new Exception(col.ColumnName + " is not valid or has an unknown format.", exc);
                            }
                        }
                    }
                    if (callback != null && ele != null)
                    {
                        try
                        {
                            ele = callback.Invoke(ele);
                        }
                        catch (Exception ex)
                        {
                            res.Add(null);
                            ErrorLog.Add(i + 2, ex.Message);
                            continue;
                        }
                    }
                    res.Add(ele);
                }
                catch (Exception ex)
                {
                    res.Add(null);
                    ErrorLog.Add(i + 2, ex.Message);
                }
            }
            return res;
        }

        public void MapHeaderTo(string header, string field)
        {
            HeaderMap[header] = field;
        }

        private string[] _autoImport<Entity>(IEnumerable<Entity> objects, object importer, bool skipNull = false)
        {
            if (!skipNull)
            {
                Dictionary<int, string> msg = new Dictionary<int, string>();
                for (int i = 0; i < objects.Count(); i++)
                    if (objects.ElementAt(i) == null)
                        msg[i] = "Row #" + (i + 2) + " has invalid value(s).\r\n";
                if (msg.Count > 0)
                    return msg.Values.ToArray();
            }
            if (importer is IImporterBLL<Entity>)
            {
                var imp = importer as IImporterBLL<Entity>;
                imp.Import(objects);
            }
            else if (importer is IInsertOrUpdateBLL<Entity>)
            {
                var imp = importer as IInsertOrUpdateBLL<Entity>;
                int _lastRow = 1;
                try
                {
                    for (int i = 0; i < objects.Count(); i++)
                    {
                        var data = objects.ElementAt(i);
                        _lastRow = i + 1;
                        imp.InsertOrUpdate(data);
                    }

                }
                catch (Exception ex)
                {
                    if (_lastRow >= 0)
                    {
                        throw new Exception("Cannot save data at row #" + _lastRow + "!", ex);
                    }
                }
            }
            return null;
        }
        public string[] AutoImport<Entity>(IEnumerable<Entity> objects, IImporterBLL<Entity> importer, bool skipNull = false)
        {
            return _autoImport(objects, importer, skipNull);
        }
        public string[] AutoImport<Entity>(IEnumerable<Entity> objects, IInsertOrUpdateBLL<Entity> importer, bool skipNull = false)
        {
            return _autoImport(objects, importer, skipNull);
        }

        public string CompileLog(string separator, string prefix = "", string suffix = "")
        {
            string res = "";
            foreach (var err in ErrorLog)
            {
                if (res != "") res += separator;
                res += prefix + "Row #" + (err.Key) + " - " + err.Value + suffix;
            }
            return res;
        }
    }
}