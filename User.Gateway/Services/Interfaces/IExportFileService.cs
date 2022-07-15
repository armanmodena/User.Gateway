using System.Collections.Generic;
using System.IO;

namespace User.Gateway.Services.Interfaces
{
    public interface IExportFileService
    {
        public byte[] ExportCSV<T>(IEnumerable<T> data, string[] fields);
        public byte[] ExportExcel<T>(IEnumerable<T> data, string[] fields);
    }
}
