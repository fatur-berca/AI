/*
 * Excel Import Helper
 * by Zecchan Silverlake
 * Sep 28th, 2018
*/
using System;
using System.Collections;
using System.Collections.Generic;
using Excel;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using TOM.Master.BusinessLogics;
using DFIS.Contracts;
using System.Web.Mvc;

namespace hms_tom_dev.Helper
{
    public class ExcelImportHelper
    {
        public Dictionary<string, ExcelImportMappingEntry> ExcelHeaderMap { get; private set; }
        public Dictionary<string, ExcelImportMappingEntry> FieldHeaderMap { get; private set; }
        public ExcelImportHelper()
        {
            ExcelHeaderMap = new Dictionary<string, ExcelImportMappingEntry>();
            FieldHeaderMap = new Dictionary<string, ExcelImportMappingEntry>();
        }
        public static DataSet ImportExcelAsDataSet(Stream excelStream, bool openXml = true, bool isFirstRowAsColumnNames = true)
        {
            IExcelDataReader excel;
            if (!openXml)
                excel = ExcelReaderFactory.CreateBinaryReader(excelStream);
            else
                excel = ExcelReaderFactory.CreateOpenXmlReader(excelStream);
            try
            {
                excel.IsFirstRowAsColumnNames = isFirstRowAsColumnNames;
                return excel.AsDataSet();
            }
            finally
            {
                excel.Close();
            }
        }
        public ExcelImportHelper Map(string excelHeader, string fieldHeader, IEnumerable<string> enums = null)
        {
            var eime = new ExcelImportMappingEntry(enums == null ? null : enums.ToList())
            {
                ExcelColumnHeader = excelHeader,
                FieldColumnHeader = fieldHeader
            };
            ExcelHeaderMap[excelHeader] = eime;
            ExcelHeaderMap[fieldHeader] = eime;
            return this;
        }
        public List<ExcelImportResult<T>> Deserialize<T>(DataTable table, Func<T, T> validator = null)
        {
            InitDeserializer();

            var res = new List<ExcelImportResult<T>>();
            var entType = typeof(T);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                int rowNumber = i + 2; // follows Excel
                if (IsEmptyRow(row))
                    continue;
                DataColumn lastCol = null;
                try
                {
                    var val = Activator.CreateInstance<T>();
                    foreach (DataColumn dcol in table.Columns)
                    {
                        if (!ExcelHeaderMap.ContainsKey(dcol.ColumnName)) continue;
                        lastCol = dcol;
                        var mapTo = ExcelHeaderMap[dcol.ColumnName];
                        var trgPropInfo = entType.GetProperty(mapTo.FieldColumnHeader);
                        var trgFldInfo = entType.GetField(mapTo.FieldColumnHeader);

                        if (trgPropInfo != null && trgPropInfo.CanWrite)
                        {
                            trgPropInfo.SetValue(val, ConvertTo(trgPropInfo.PropertyType, row[dcol]));
                        }
                        if (trgFldInfo != null && trgFldInfo.IsPublic)
                        {
                            trgFldInfo.SetValue(val, ConvertTo(trgFldInfo.FieldType, row[dcol]));
                        }

                        // enum check
                        if (val != null && mapTo.ValidValues != null)
                        {
                            string enmCheck = ConvertTo<string>(row[dcol]);
                            if (!mapTo.ValidValues.Contains(enmCheck))
                                throw new RowValidationByFieldException(dcol.ColumnName, enmCheck);
                        }
                    }

                    // validator
                    if (validator != null)
                        val = validator.Invoke(val);

                    res.Add(new ExcelImportResult<T>(val, rowNumber));
                }
                catch (Exception ex)
                {
                    if (lastCol != null && !(ex is RowValidationByFieldException))
                        ex = new RowValidationByFieldException(lastCol.ColumnName, ex.Message);
                    res.Add(new ExcelImportResult<T>(ex.Message, ex, rowNumber));
                }
            }

            return res;
        }
        public List<ExcelDatabasePushResult> PushToDatabase<T>(IEnumerable<ExcelImportResult<T>> data, IImporterBLL<T> importer, bool allowInvalidData = false)
        {
            var res = new List<ExcelDatabasePushResult>();
            if (!allowInvalidData)
            {
                foreach (var d in data)
                    if (!d.Valid)
                    {
                        res.Add(new ExcelDatabasePushResult(d.ErrorMessage, d.Exception, d.RowNumber));
                    }
                if (res.Count > 0)
                    return res;
            }
            if (importer != null)
            {
                var iobj = importer as IImporterBLL<T>;
                iobj.BeginImport();
                var rowNum = 0;
                try
                {
                    foreach (var d in data)
                    {
                        rowNum = d.RowNumber;
                        iobj.ImportRow(d.Result);
                        res.Add(new ExcelDatabasePushResult(d.RowNumber));
                    }
                    iobj.FinalizeImport();
                }
                catch (Exception ex)
                {
                    iobj.CancelImport();
                    res.Add(new ExcelDatabasePushResult(ex.Message, ex, rowNum));
                }
            }
            return res;
        }
        public string Import<T>(Dictionary<string, List<ExcelImportResult<T>>> data, IImporterBLL<T> importer, bool allowInvalidData = false)
        {
            // push the data to database
            Dictionary<string, string> errorTracker = new Dictionary<string, string>();
            foreach (var ptdb in data)
            {
                var res = PushToDatabase(ptdb.Value, importer);
                var errs = res.Where(re => !re.Valid).Select(msg => "Row #" + msg.RowNumber + ": " + msg.ErrorMessage);
                if (errs.Count() > 0)
                {
                    var errmsg = "&bull; " + string.Join("<br />&bull; ", errs);
                    errorTracker.Add(ptdb.Key, errmsg);
                }
            }

            // error found
            if (errorTracker.Count > 0)
            {
                string cmpError = "";
                foreach (var et in errorTracker)
                {
                    cmpError += "Failed to upload <strong>" + et.Key + "</strong>:<br />";
                    cmpError += et.Value + "<br /><br />";
                }
                return cmpError;
            }

            // import completed successfully
            return null;
        }

        public string AutoImport<T>(HttpFileCollectionBase files, IImporterBLL<T> importer, Func<T, T> validator, bool allowInvalidData = false)
        {
            // count files
            if (files == null || files.Count <= 0)
                return "No file specified.";

            // get all valid data
            var dataCol = new Dictionary<string, List<ExcelImportResult<T>>>();
            for (int i = 0; i < files.Count; i++)
            {
                var file = files.Get(i);
                // get workbook for each file
                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        var ds = ExcelImportHelper.ImportExcelAsDataSet(file.InputStream, file.FileName.ToLower().EndsWith(".xlsx"));
                        // get worksheet in every workbook
                        foreach (DataTable table in ds.Tables)
                        {
                            var col = new List<ExcelImportResult<T>>();
                            dataCol.Add(file.FileName + "#" + table.TableName, col);

                            var excelRes = Deserialize<T>(table, validator);

                            col.AddRange(excelRes);
                        }
                    }
                    catch { }
                }
            }

            return Import<T>(dataCol, importer);
        }

        public static bool IsEmptyRow(DataRow row, DataColumnCollection cols = null)
        {
            if (row == null)
                return true;
            bool allEmpty = true;
            if (cols == null)
                cols = row.Table.Columns;
            foreach (DataColumn coldef in cols)
            {
                var val = row[coldef];
                if (val == null || val is DBNull) continue;
                if (string.IsNullOrWhiteSpace(val.ToString())) continue;
                allEmpty = false;
                break;
            }
            return allEmpty;
        }
        public static Dictionary<Type, Func<object, object>> DeserializerDataConverters { get; private set; }
        public static void InitDeserializer()
        {
            if (DeserializerDataConverters != null) return;
            DeserializerDataConverters = new Dictionary<Type, Func<object, object>>();

            DeserializerDataConverters.Add(typeof(string), data =>
            {
                if (data == null) return null;
                if (data is DBNull) return null;

                return data.ToString();
            });
            DeserializerDataConverters.Add(typeof(double), data =>
            {
                if (data == null) return null;
                if (data is DBNull) return null;
                return Convert.ToDouble(data);
            });
            DeserializerDataConverters.Add(typeof(double?), data =>
            {
                if (data == null) return null;
                if (data is DBNull) return null;
                return Convert.ToDouble(data);
            });
            DeserializerDataConverters.Add(typeof(int), data =>
            {
                if (data == null) return null;
                if (data is DBNull) return null;
                return Convert.ToInt32(data);
            });
            DeserializerDataConverters.Add(typeof(int?), data =>
            {
                if (data == null) return null;
                if (data is DBNull) return null;
                return Convert.ToInt32(data);
            });
            DeserializerDataConverters.Add(typeof(decimal), data =>
            {
                if (data == null) return null;
                if (data is DBNull) return null;
                return Convert.ToDecimal(data);
            });
            DeserializerDataConverters.Add(typeof(decimal?), data =>
            {
                if (data == null) return null;
                if (data is DBNull) return null;
                return Convert.ToDecimal(data);
            });
            DeserializerDataConverters.Add(typeof(DateTime), data =>
            {
                if (data == null) return null;
                if (data is DBNull) return null;

                if (data is string)
                {
                    try
                    {
                        return DateTime.Parse(data.ToString());
                    }
                    catch { }
                }

                return Convert.ToDateTime(data);
            });
            DeserializerDataConverters.Add(typeof(DateTime?), data =>
            {
                if (data == null) return null;
                if (data is DBNull) return null;

                if (data is string)
                {
                    try
                    {
                        return DateTime.Parse(data.ToString());
                    }
                    catch { }
                }

                return Convert.ToDateTime(data);
            });
            DeserializerDataConverters.Add(typeof(byte), data =>
            {
                if (data == null) return null;
                if (data is DBNull) return null;

                if (data is string)
                {
                    try
                    {
                        return byte.Parse(data.ToString());
                    }
                    catch { }
                }

                return Convert.ToByte(data);
            });
        }
        public static T ConvertTo<T>(object value)
        {
            if (DeserializerDataConverters.ContainsKey(typeof(T)))
            {
                var converter = DeserializerDataConverters[typeof(T)];
                return (T)converter.Invoke(value);
            }
            return (T)value;
        }
        public static object ConvertTo(Type trgType, object value)
        {
            if (DeserializerDataConverters.ContainsKey(trgType))
            {
                var converter = DeserializerDataConverters[trgType];
                return converter.Invoke(value);
            }
            return value;
        }

        public static bool CheckExistsIn<T>(T item, IEnumerable<T> collection, string propName, bool allowNull = false, bool throwError = true)
        {
            if ( collection.Contains(item))
            {
                return true;
            }
            if (!allowNull)
            {
                if (!CheckHasValue(item, propName, throwError))
                    return false;
            }
            if (throwError)
                throw new RowValidationByFieldException(propName, item == null ? null : item.ToString());
            return false;
        }
        public static bool CheckHasValue<T>(T item, string propName, bool throwError = true)
        {
            if ((item == null
                || (item is string && string.IsNullOrWhiteSpace(item as string)))
                || (item is DateTime && (item as DateTime?) == DateTime.MinValue)
                )
            {
                if (throwError)
                    throw new RowValidationByFieldException(propName);
                return false;
            }
            return true;
        }
    }

    public class ExcelImportMappingEntry
    {
        public string ExcelColumnHeader { get; set; }
        public string FieldColumnHeader { get; set; }

        public List<string> ValidValues { get; private set; }

        public ExcelImportMappingEntry(List<string> enums)
        {
            ValidValues = enums;
        }
    }

    public class ExcelImportResult<T>
    {
        public T Result { get; private set; }
        public int RowNumber { get; private set; }
        public bool Valid { get; private set; }
        public Exception Exception { get; private set; }
        public string ErrorMessage { get; private set; }

        public ExcelImportResult(T result, int rowNumber)
        {
            if (result == null) throw new ArgumentNullException("result");
            RowNumber = rowNumber;
            Result = result;
            Valid = true;
        }
        public ExcelImportResult(string errorMessage, Exception exception, int rowNumber)
        {
            Exception = exception;
            RowNumber = rowNumber;
            Valid = false;
            ErrorMessage = errorMessage;
        }
    }

    public class ExcelDatabasePushResult
    {
        public int RowNumber { get; private set; }
        public bool Valid { get; private set; }
        public Exception Exception { get; private set; }
        public string ErrorMessage { get; private set; }

        public ExcelDatabasePushResult(int rowNumber)
        {
            RowNumber = rowNumber;
            Valid = true;
        }
        public ExcelDatabasePushResult(string errorMessage, Exception exception, int rowNumber)
        {
            Exception = exception;
            RowNumber = rowNumber;
            Valid = false;
            ErrorMessage = errorMessage;
        }
    }

}