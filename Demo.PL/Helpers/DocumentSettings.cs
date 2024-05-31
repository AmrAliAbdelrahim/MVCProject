using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace Demo.PL.Helpers
{
    public static class DocumentSettings
    {
        //create
        public static string UploadFile(IFormFile file, string FolderName)
        {
            //1 Get LocatedFolderPath
            string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName);
            //2 Get The FileName And Make It Unique
            string FileName = $"{Guid.NewGuid()}{file.FileName}";

            //3. Get File Path[folder Path+FileName]
            string FilePath = Path.Combine(FolderPath, FileName);

            //4 Save File As streams 
            using var FileSave = new FileStream(FilePath, FileMode.Create);
            file.CopyTo(FileSave);
            //5 Return File Name
            return FileName;

            // "C: \Users\mosa\source\repos\WebApplication2\Demo.PL\Files\Images\"
            //C: \Users\mosa\source\repos\WebApplication2\Demo.PL\wwwroot\Files\Images\
        }
        //Delete
        public static void DeleteFile(string FileName, string FolderName)
        {
            //.Get FilePath
            string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName, FileName);
            //2. Check File Is Exist Or No
            if (File.Exists(FilePath))
            {
                //if Exist Remove it
                File.Delete(FilePath);
            }

        }
    }
}

            