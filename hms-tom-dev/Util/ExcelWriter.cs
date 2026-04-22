using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace hms_tom_dev.Util
{
    public class ExcelWriter
    {
        private Stream stream;

        private BinaryWriter writer;
        private ushort[] clBegin = {
		0x809,
		8,
		0,
		0x10,
		0,
		0
	};
        private ushort[] clEnd = {
		0xa,
		0

	};

        private void WriteUshortArray(ushort[] value)
        {
            for (int i = 0; i <= value.Length - 1; i++)
            {
                writer.Write(value[i]);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public ExcelWriter(Stream stream)
        {
            this.stream = stream;
            writer = new BinaryWriter(stream);
        }

        /// <summary>
        /// Writes the text cell value.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <param name="value">The string value.</param>
        public void WriteCell(int row, int col, string value)
        {
            ushort[] clData = {
			0x204,
			0,
			0,
			0,
			0,
			0
		};
            int iLen = value.Length;
            byte[] plainText = Encoding.ASCII.GetBytes(value);
            clData[1] = Convert.ToUInt16(8 + iLen);
            clData[2] = Convert.ToUInt16(row);
            clData[3] = Convert.ToUInt16(col);
            clData[5] = Convert.ToUInt16(iLen);
            WriteUshortArray(clData);
            writer.Write(plainText);
        }

        /// <summary>
        /// Writes the integer cell value.
        /// </summary>
        /// <param name="row">The row number.</param>
        /// <param name="col">The column number.</param>
        /// <param name="value">The value.</param>
        public void WriteCell(int row, int col, int value)
        {
            ushort[] clData = {
			0x27e,
			10,
			0,
			0,
			0
		};
            clData[2] = Convert.ToUInt16(row);
            clData[3] = Convert.ToUInt16(col);
            WriteUshortArray(clData);
            int iValue = (value << 2) | 2;
            writer.Write(iValue);
        }

        /// <summary>
        /// Writes the double cell value.
        /// </summary>
        /// <param name="row">The row number.</param>
        /// <param name="col">The column number.</param>
        /// <param name="value">The value.</param>
        public void WriteCell(int row, int col, double value)
        {
            ushort[] clData = {
			0x203,
			14,
			0,
			0,
			0
		};
            clData[2] = Convert.ToUInt16(row);
            clData[3] = Convert.ToUInt16(col);
            WriteUshortArray(clData);
            writer.Write(value);
        }

        /// <summary>
        /// Writes the empty cell.
        /// </summary>
        /// <param name="row">The row number.</param>
        /// <param name="col">The column number.</param>
        public void WriteCell(int row, int col)
        {
            ushort[] clData = {
			0x201,
			6,
			0,
			0,
			0x17
		};
            clData[2] = Convert.ToUInt16(row);
            clData[3] = Convert.ToUInt16(col);
            WriteUshortArray(clData);
        }

        /// <summary>
        /// Must be called once for creating XLS file header
        /// </summary>
        public void BeginWrite()
        {
            WriteUshortArray(clBegin);
        }

        /// <summary>
        /// Ends the writing operation, but do not close the stream
        /// </summary>
        public void EndWrite()
        {
            WriteUshortArray(clEnd);
            writer.Flush();
        }
    }
}