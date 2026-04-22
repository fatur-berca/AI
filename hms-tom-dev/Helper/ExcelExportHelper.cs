using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace hms_tom_dev.Helper
{
    public class ExcelExportHelper : IDisposable
    {
        public ExcelPackage Package { get; private set; }
        public List<ObjectToExcelMapper> Mappers { get; private set; }

        public ExcelExportHelper(string path = null)
        {
            Package = new ExcelPackage(path == null ? null : new System.IO.FileInfo(path));
            Mappers = new List<ObjectToExcelMapper>();
            HeaderBackgroundColor = System.Drawing.Color.DarkGray;
            HeaderForegroundColor = System.Drawing.Color.Black;
        }

        public ObjectToExcelMapper Map(string excelHeader, string objPropName)
        {
            var map = new ObjectToExcelMapper()
            {
                ExcelHeader = excelHeader,
                ObjectPropName = objPropName,
                IsActive = true
            };
            Mappers.Add(map);
            return map;
        }
        public ObjectToExcelMapper Map(string excelHeader)
        {
            var map = new ObjectToExcelMapper()
            {
                ExcelHeader = excelHeader,
                ObjectPropName = excelHeader.Trim().Replace(" ", "").Replace("\t", ""),
                IsActive = true
            };
            Mappers.Add(map);
            return map;
        }

        public void SetHeaderDisplay(string[] hdrNames)
        {
            if (hdrNames == null)
                return;
            var _mappers = new List<ObjectToExcelMapper>();
            _mappers.AddRange(Mappers);

            Mappers.Clear();

            foreach (var hdr in hdrNames)
            {
                try
                {
                    var mapper = _mappers.First(m => m.ExcelHeader == hdr);
                    mapper.IsActive = true;
                    Mappers.Add(mapper);
                }
                catch { }
            }

            // reinsert other mappers but set it inactive
            var _oldMapper = _mappers.Where(m => !Mappers.Contains(m)).ToList();
            for (var i = 0; i < _oldMapper.Count; i++)
            {
                var mapper = _oldMapper[i];
                mapper.IsActive = false;
            }
            Mappers.AddRange(_oldMapper);
        }

        public ExcelWorksheet CreateWorksheet(string name)
        {
            return Package.Workbook.Worksheets.Add(name);
        }

        public System.Drawing.Color HeaderBackgroundColor { get; set; }
        public System.Drawing.Color HeaderForegroundColor { get; set; }

        public void WriteHeader(ExcelWorksheet ws, int rowIndex = 1, int colIndex = 1)
        {
            for (var c = 0; c < Mappers.Count; c++)
            {
                if (Mappers[c].IsActive)
                    ws.Cells[rowIndex, c + colIndex].Value = Mappers[c].ExcelHeader;
            }
            using (var range = ws.Cells[rowIndex, colIndex, rowIndex, Mappers.Count(m => m.IsActive) + colIndex - 1])
            {
                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;

                range.Style.Fill.BackgroundColor.SetColor(HeaderBackgroundColor);
                range.Style.Font.Color.SetColor(HeaderForegroundColor);
                range.Style.Font.Bold = true;
                range.Style.ShrinkToFit = false;
            }
        }

        public void Dispose()
        {
            Package.Dispose();
        }


        public void Save()
        {
            if (Package.Workbook.Worksheets.Count > 0)
                Package.Save();
        }

        public Stream GetStream()
        {
            MemoryStream ms = new MemoryStream();
            Package.SaveAs(ms);
            return ms;
        }
        public byte[] GetBin()
        {
            return Package.GetAsByteArray();
        }

        public void AutoSizeColumns(ExcelWorksheet ws)
        {
            for (var i = 1; i <= ws.Dimension.Columns; i++)
                ws.Column(i).AutoFit();
        }

        private ExcelNestedObjectMapper<T> GetNestedObjectMapper<T>(object obj, string[] path)
        {
            if (obj == null) return null;
            if (path == null || path.Length < 1) return null;
            var otype = obj.GetType();
            var pi = otype.GetProperty(path[0]);
            if (path.Length == 1)
                return new ExcelNestedObjectMapper<T>()
                {
                    PropertyInfo = pi,
                    Object = obj,
                    PropertyName = path[0]
                };
            if (pi != null && pi.CanRead)
            {
                var ochild = pi.GetValue(obj);
                var npath = new string[path.Length - 1];

                for (var i = 1; i < path.Length; i++)
                    npath[i - 1] = path[i];

                return GetNestedObjectMapper<T>(ochild, npath);
            }
            return null;
        }

        /// <summary>
        /// Write a collection of object to specified worksheet. If you are working with nested parameters, use WriteRowsNested instead.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="ws">The worksheet to write into.</param>
        /// <param name="obj">The collection of object to read the data from.</param>
        /// <param name="setter">Validation function that must return the actual data to write into the worksheet.</param>
        /// <param name="rowIndex">Row offset of the table.</param>
        /// <param name="colIndex">Columns offset of the table.</param>
        public void WriteRows<T>(ExcelWorksheet ws, IEnumerable<T> obj, Func<T, ExcelRange, PropertyInfo, object> setter = null, int rowIndex = 2, int colIndex = 1)
        {
            if (obj == null || ws == null) return;
            var typA = typeof(T);

            foreach (var o in obj)
            {
                for (var ci = 0; ci < this.Mappers.Count; ci++)
                {
                    if (!this.Mappers[ci].IsActive) continue;
                    var map = this.Mappers[ci];
                    var pi = typA.GetProperty(map.ObjectPropName);
                    var col = ci + colIndex;
                    var cell = ws.Cells[rowIndex, col];

                    if (pi != null)
                    {
                        if (setter != null)
                        {
                            cell.Value = setter.Invoke(o, cell, pi);
                        }
                        else
                            cell.Value = pi.GetValue(o);
                    }
                }
                rowIndex++;
            }
        }

        /// <summary>
        /// Write a collection of object to specified worksheet. This function support nested parameters.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="ws">The worksheet to write into.</param>
        /// <param name="obj">The collection of object to read the data from.</param>
        /// <param name="validator">Validation function that must return the actual data to write into the worksheet.</param>
        /// <param name="rowIndex">Row offset of the table.</param>
        /// <param name="colIndex">Columns offset of the table.</param>
        public void WriteRowsNested<T>(ExcelWorksheet ws, IEnumerable<T> obj, Func<ExcelNestedObjectMapper<T>, object> validator = null, Action<ExcelRange, string, object> formatter = null, int rowIndex = 2, int colIndex = 1)
        {
            if (obj == null || ws == null) return;
            var typA = typeof(T);

            foreach (var o in obj)
            {
                for (var ci = 0; ci < this.Mappers.Count; ci++)
                {
                    if (!this.Mappers[ci].IsActive) continue;
                    var map = this.Mappers[ci];
                    var pi = GetNestedObjectMapper<T>(o, map.NestedName);
                    var col = ci + colIndex;
                    var cell = ws.Cells[rowIndex, col];

                    if (pi != null)
                    {
                        pi.OriginalObject = o;
                        pi.Cell = ws.Cells[rowIndex, col];
                        pi.ExcelHeaderName = map.ExcelHeader;
                        if (validator != null)
                        {
                            cell.Value = validator.Invoke(pi);
                        }
                        else
                            cell.Value = pi.Value;
                        if (formatter != null)
                            formatter.Invoke(cell, map.ExcelHeader, cell.Value);
                    }
                }
                rowIndex++;
            }
        }

        /// <summary>
        /// Write a collection of object to specified worksheet. This function must have a mapper function
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="ws">The worksheet to write into.</param>
        /// <param name="obj">The collection of object to read the data from.</param>
        /// <param name="mapper">Mapper function that must fill the supplied dictionary to fill the row.</param>
        /// <param name="rowIndex">Row offset of the table.</param>
        /// <param name="colIndex">Columns offset of the table.</param>
        public void WriteRowsCustom<T>(ExcelWorksheet ws, IEnumerable<T> obj, Action<object, Dictionary<string, object>> mapper, int rowIndex = 2, int colIndex = 1)
        {
            if (mapper == null)
                throw new ArgumentNullException("mapper");
            if (obj == null || ws == null) return;
            var typA = typeof(T);

            foreach (var o in obj)
            {
                var res = new Dictionary<string, object>();
                for (var ci = 0; ci < this.Mappers.Count; ci++)
                {
                    if (!this.Mappers[ci].IsActive) continue;
                    res.Add(this.Mappers[ci].ExcelHeader, null);
                }
                mapper.Invoke(o, res);

                for (var ci = 0; ci < this.Mappers.Count; ci++)
                {
                    if (!this.Mappers[ci].IsActive) continue;
                    var col = ci + colIndex;
                    if (res.ContainsKey(this.Mappers[ci].ExcelHeader))
                    {
                        var cell = ws.Cells[rowIndex, col];
                        cell.Value = res[this.Mappers[ci].ExcelHeader];
                    }
                }

                rowIndex++;
            }
        }
    }

    public class ExcelNestedObjectMapper<T>
    {
        /// <summary>
        /// The name of target property.
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// The name of excel header.
        /// </summary>
        public string ExcelHeaderName { get; set; }
        /// <summary>
        /// PropertyInfo of target property.
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }
        /// <summary>
        /// The object that contains the property.
        /// </summary>
        public object Object { get; set; }
        /// <summary>
        /// The object that is passed as row. Also the root object of the value.
        /// </summary>
        public T OriginalObject { get; set; }
        /// <summary>
        /// Gets the value using PropertyInfo
        /// </summary>
        public object Value
        {
            get
            {
                if (Object == null)
                    return null;
                if (PropertyInfo == null)
                    return null;
                try
                {
                    return PropertyInfo.GetValue(Object);
                }
                catch { return null; }
            }
        }

        /// <summary>
        /// The excel cell that the result of validator call will write into.
        /// You can add formatting and styling using this property.
        /// </summary>
        public ExcelRange Cell { get; set; }
    }

    public class ObjectToExcelMapper
    {
        public string ObjectPropName;
        public string ExcelHeader;

        /// <summary>
        /// Gets the nested name as array of string
        /// </summary>
        public string[] NestedName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ObjectPropName)) return new string[0];

                return ObjectPropName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public bool IsActive;
    }
}