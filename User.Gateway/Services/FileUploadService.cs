using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using User.Gateway.Services.Interfaces;

namespace User.Gateway.Services
{
    public class FileUploadService : IFileUploadService
    {
        public (bool, string) deleteImage(string fileName)
        {
            string path = "Resources//Images//" + fileName;
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    return (false, "The deletion failed: " + e.Message);
                }

                return (true, null);
            }
            else
            {
                return (false, "File doesn't exists!");
            }
        }

        public string genImageName(IFormFileCollection Files, string index)
        {
            string renameFile = null;

            var file = Files[index];

            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                renameFile = Convert.ToString(Guid.NewGuid()) + "." + fileName.Split('.').Last();
            }

            return renameFile;
        }

        public string saveImage(IFormFileCollection Files, string index, string fileName)
        {
            var file = Files[index];
            var folderName = Path.Combine("Resources", "Images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var fullPath = Path.Combine(pathToSave, fileName);
            var dbPath = Path.Combine(folderName, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
                return dbPath;
            }
        }

        public object uploadImage(IFormFileCollection Files, string index)
        {
            var file = Files[index];
            var folderName = Path.Combine("Resources", "Images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                string renameFile = Convert.ToString(Guid.NewGuid()) + "." + fileName.Split('.').Last();
                var fullPath = Path.Combine(pathToSave, renameFile);
                var dbPath = Path.Combine(folderName, renameFile);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return new
                {
                    status = HttpStatusCode.Created,
                    message = "Upload file successfully",
                    path = dbPath
                };
            }
            else
            {
                return new
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Upload file successfully",
                };
            }
        }

        public object uploadMultipleImage(IFormFileCollection Files)
        {

            var files = Files;

            if (files.Any(f => f.Length == 0))
            {
                return new
                {
                    status = HttpStatusCode.BadRequest,
                    message = "No image found!",
                }; ;
            }

            List<string> dbPaths = new List<string>();

            foreach (var file in files)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                string renameFile = Convert.ToString(Guid.NewGuid()) + "." + fileName.Split('.').Last();

                var fixName = file.Name.Replace("[]", "");
                char[] folder = fixName.ToCharArray();
                folder[0] = char.ToUpper(folder[0]);

                var folderName = Path.Combine("Resources", new string(folder));
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                var fullPath = Path.Combine(pathToSave, renameFile);
                var dbPath = Path.Combine(folderName, renameFile);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    dbPaths.Add(dbPath);
                }
            }

            return new
            {
                status = HttpStatusCode.Created,
                message = dbPaths.Count() > 0 ? "All the files are successfully uploade" : "No Image was uploaded",
                path = dbPaths.ToArray()
            };

        }
    }
}
