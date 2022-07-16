using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClosedXML.Excel;
using User.Gateway.DTO;
using User.Gateway.Utils;

namespace User.Gateway.Extensions
{
    public class EportImport<T> where T : new()
    {
        public byte[] ExportCSV<TEntiry>(IEnumerable<TEntiry> data, string[] fields)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Join(",", fields));
            foreach (var d in data)
            {
                var value = "";
                foreach (var field in fields)
                {
                    if (value == "")
                    {
                        value = d.GetType().GetProperty(field).GetValue(d, null).ToString();
                    }
                    else
                    {
                        value = value + "," + d.GetType().GetProperty(field).GetValue(d, null).ToString();
                    }
                }

                builder.AppendLine(value);
            }
            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public byte[] ExportExcel<TEntiry>(IEnumerable<TEntiry> data, string[] fields)
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
                    foreach (var field in fields)
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

        public (List<T>, ErrorDto) ImportExcel(string filePath, string[] columns)
        {
            using (var excelWorkbook = new XLWorkbook(filePath))
            {
                var nonEmptyDataRows = excelWorkbook.Worksheet(1).RowsUsed();

                List<T> Users = new List<T>();

                foreach (var dataRow in nonEmptyDataRows)
                {
                    if (dataRow.RowNumber() == 1)
                    {
                        int col = 1;
                        foreach (var column in columns)
                        {
                            if (dataRow.Cell(col).Value.ToString() != column)
                            {
                                return (null, new ErrorDto()
                                {
                                    Status = 400,
                                    Message = $"Colulmn {column} not found"
                                });
                            }
                            col++;
                        }
                    }
                    else
                    {
                        int col = 1;
                        IDictionary<string, string> userArray = new Dictionary<string, string>();
                        foreach (var column in columns)
                        {
                            userArray[column] = dataRow.Cell(col).Value.ToString();
                            col++;
                        }

                        var data = new T();
                        var userObjectType = data.GetType();

                        foreach (var item in userArray)
                        {
                            var type = userObjectType.GetProperty(item.Key).PropertyType.Name;

                            if(type == "String")
                            {
                                if(item.Key == "Password" && !String.IsNullOrEmpty(item.Value))
                                {
                                    userObjectType.GetProperty(item.Key).SetValue(data, Hash.EncryptSHA2(item.Value), null);
                                }
                                else
                                {
                                    userObjectType.GetProperty(item.Key).SetValue(data, item.Value, null);
                                }
                            }
                            else if(type == "Number")
                            {
                                userObjectType.GetProperty(item.Key).SetValue(data, Int32.Parse(item.Value), null);
                            }
                            else if(type == "DateTime")
                            {
                                userObjectType.GetProperty(item.Key).SetValue(data, DateTime.Parse(item.Value), null);
                            }
                        }

                        Users.Add(data);
                    }
                }
                return (Users, null);
            }
        }
    }
}
