using System.Collections.Generic;
using System.IO;
using System.Text;
using ClosedXML.Excel;
using User.Gateway.Services.Interfaces;

namespace User.Gateway.Services
{
    public class ExportFileService : IExportFileService
    {
        public byte[] ExportCSV<T>(IEnumerable<T> data, string[] fields)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Join(",", fields));
            foreach(var d in data)
            {
                var value = "";
                foreach (var field in fields)
                {
                    if(value == "")
                    {
                        value = d.GetType().GetProperty(field).GetValue(d, null).ToString();
                    } else
                    {
                        value = value + "," + d.GetType().GetProperty(field).GetValue(d, null).ToString();
                    }
                }

                builder.AppendLine(value);
            }
            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public byte[] ExportExcel<T>(IEnumerable<T> data, string[] fields)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("User");

                int currentRow = 1;
                int columnHeader = 1;
                foreach (var field in fields)
                {
                    worksheet.Cell(currentRow, columnHeader).Value = field;
                    columnHeader++;
                }
             

                foreach (var d in data)
                {
                    currentRow++;
                    int columnBody = 1;
                    foreach(var field in fields)
                    {
                        worksheet.Cell(currentRow, columnBody).Value = d.GetType().GetProperty(field).GetValue(d, null);
                        columnBody++;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
