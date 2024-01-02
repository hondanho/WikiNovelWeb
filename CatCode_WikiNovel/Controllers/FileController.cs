using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WowCore;
using System.IO.Compression;
using System.Drawing;
using WowSQL;
using WowCommon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace CatCode_WikiNovel.Controllers
{

    [AllowAnonymous]
    public class FileController : BaseController
    {
        public string WebRootPath { get { return _webHostEnvironment.WebRootPath; } }

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileController(IWebHostEnvironment webHostEnvironment)  
        {
            _webHostEnvironment = webHostEnvironment;
        }
        bool showError = true;
        // GET: File
        [HttpPost]
        public async Task<string> UploadImage(IFormFile file, string parentTable, string tableName
            , int id, string fileName, string fieldName, string deleteId, int uid)
        {

            if (fileName == string.Empty)
            {
                return !showError ? "" : "[Exception] Some parame is empty";
            }
            string path = this.GetPathSaveImage( parentTable, tableName, id, fileName);
            if (file != null && file.Length > 0)
            {
                try
                {
                    if (!string.IsNullOrEmpty(deleteId) && int.TryParse(deleteId, out int idDelete))
                    {
                        DeleteImageWithPath(fieldName, tableName, idDelete, uid);
                    }
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return !showError ? "" : "[Success] Upload Success :" + fileName;
                }
                catch (Exception ex)
                {
                    if (System.IO.File.Exists(path))
                    {
                        return !showError ? "" : "[Success] Upload Success :" + fileName;
                    }
                    return !showError ? "" : "[Exception] " + ex.Message;
                }
            }
            else
            {
                return !showError ? "" : "[Exception] file.Length = 0";
            }
        }
        // GET: File
        [HttpPost]
        public async Task<string> UploadImageFieldNameParent(IFormFile file
            , string tableName
            , int id
            , string fieldName
            , string fileName
            , string uid
            , string mode
            )
        {
            int userID = -1;
            if (string.IsNullOrEmpty(uid)) int.TryParse(uid, out userID);

            if (tableName == string.Empty || fileName == string.Empty)
            {
                return !showError ? "" : "Some parame is empty";
            }

            string path = this.GetPathSaveImage( "", tableName, id, fileName);

            if (file != null && file.Length > 0)
            {
                try
                {
                    

                    string image_path = path.Replace(this.WebRootPath, "");
                    string FieldName_image_path = fieldName + "_image_path";
                    string FieldName_image_name = fieldName + "_image_name";
                    if (id == -1)// insert child table
                    {
                        //insert
                        SqlHelper.ExecuteNonQuery("INSERT INTO " + tableName + " (" + FieldName_image_path + "," + FieldName_image_name + "  )   VALUES (@path, @name ) ", "@path", image_path, "@name", fileName);
                    }
                    else
                    {
                        //delete old image
                        DeleteImageWithPath(FieldName_image_path, tableName, id, userID);
                        //update
                        SqlHelper.ExecuteNonQuery("UPDATE " + tableName + " SET " + FieldName_image_path + " = @path, " + FieldName_image_name + " = @name  WHERE ID = @id", "@path", image_path, "@name", fileName, "@id", id);
                    } 
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    if (mode == "create_small")
                    {
                        CreateSmallImage(tableName, id, fieldName, userID, path);
                    }

                    return !showError ? "" : "Upload Success :" + fileName;
                }
                catch (Exception ex)
                {
                    return !showError ? "" : "Exception :" + ex.Message + "\n" + ex.StackTrace;
                }
            }
            else
            {
                return !showError ? "" : "file.Length = 0";
            }
        }

        private void CreateSmallImage(string tableName, int id, string fieldName, int userID, string path, int w = 75, int h = 75)
        {
            if (fieldName.ToLower().Contains("chuky"))
            {

                w = 300;
                h = 100;
            }
            FileInfo fileInfoPath = new FileInfo(path);
            if (!WowCommon.Common.EXT_PHOTO.Any(f => f.Contains(fileInfoPath.Extension.ToLower())))
                return;

            var small_filePath = SqlHelper.ResizeImage(path, w, h, this.WebRootPath + "/tmp_Photo/");

            string FieldName_Small_image_path = fieldName + "_small_path";
            DeleteImageWithPath(FieldName_Small_image_path, tableName, id, userID);
            FileInfo fileInfo = new FileInfo(small_filePath);
            string table_small_path = new FileInfo(path).Directory + "/" + fileInfo.Name;
            if (System.IO.File.Exists(table_small_path))
            {
                System.IO.File.Delete(table_small_path);
            }
            System.IO.File.Move(small_filePath, table_small_path);

            string queryUpdatePathFile = "UPDATE [" + tableName + "] " +
                " SET  [" + FieldName_Small_image_path + "] = @newURL  " +
                " WHERE [ID] = " + id;
            table_small_path = table_small_path.Replace(this.WebRootPath, "");
            SqlHelper.ExecuteNonQuery(queryUpdatePathFile, "@newURL", table_small_path);

        }

        [HttpPost]
        public async Task<string> UploadImageFieldNameChild(IFormFile file
            , string tableName
            , int id
            , string fieldName
            , string fileName
            , string uid
            , string mode

            , string parentTableName
            , int parentId)
        {
            int userID = -1;
            if (string.IsNullOrEmpty(uid)) int.TryParse(uid, out userID);
            if (tableName == string.Empty || fileName == string.Empty || tableName == string.Empty || parentTableName == string.Empty)
            {
                return !showError ? "" : "Some parame is empty";
            }

            string path = this.GetPathSaveImage( parentTableName, tableName, parentId, fileName);

            if (file != null && file.Length > 0)
            {
                try
                {
                    string image_path = path.Replace(this.WebRootPath, "");
                    string FieldName_image_path = fieldName + "_image_path";
                    string FieldName_image_name = fieldName + "_image_name";
                    if (id == -1)// insert child table
                    {
                        //insert
                        SqlHelper.ExecuteNonQuery("INSERT INTO " + tableName + " (" + FieldName_image_path + "," + FieldName_image_name + ",refID  )   VALUES (@path, @name ,@refID) ", "@path", image_path, "@name", fileName, "@refID", parentId);
                    }
                    else
                    {
                        //delete old image
                        DeleteImageWithPath(FieldName_image_path, tableName, id, userID);
                        //update
                        SqlHelper.ExecuteNonQuery("UPDATE " + tableName + " SET " + FieldName_image_path + "= @path, " + FieldName_image_name + " = @name  WHERE ID = @id", "@path", image_path, "@name", fileName, "@id", id);
                    }

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    if (mode == "create_small")
                    {
                        CreateSmallImage(tableName, id, fieldName, userID, path);
                    }

                    return !showError ? "" : "Upload Success :" + fileName;
                }
                catch (Exception ex)
                {
                    return !showError ? "" : "Exception :" + ex.Message + "\n" + ex.StackTrace; ;
                }
            }
            else
            {
                return !showError ? "" : "file.Length = 0";
            }
        }

        [HttpPost]
        public async Task<string> UploadImage_Zip(IFormFile file, string parentTable, string tableName
            , int id, string fileName, string fieldName, string deleteId, int uid)
        {
            try
            {
                SqlHelper.Log(WowCommon.ModeLog.API, "HttpPost UploadImage_Zip", tableName, id, null
                        , new Dictionary<string, object>() {
                              { "parentTable", parentTable }
                            , { "fieldName", fieldName }
                            , { "fileName", fileName }
                        }, "UploadImage_Zip", uid);


                if (fileName == string.Empty)
                {
                    return !showError ? "" : "[Exception] fileName is empty";
                }
                string path = this.GetPathSaveImage( parentTable, tableName, id, fileName);
                if (file != null && file.Length > 0)
                {
                    string rsPath = this.GetDirectorySaveImage( parentTable, tableName, id);
                    try
                    {
                        if (!string.IsNullOrEmpty(deleteId) && int.TryParse(deleteId, out int idDelete))
                        {
                            DeleteImageWithPath(fieldName, tableName, idDelete, uid);
                        }
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        DeleteFile(rsPath, uid);
                        try
                        {
                            ZipFile.ExtractToDirectory(path, rsPath);
                        }
                        catch
                        {
                        }
                        return !showError ? "" : "[Success] Upload & Unzip Success :" + path;
                    }
                    catch (Exception ex)
                    {
                        return !showError ? "" : "[Exception] " + ex.Message;
                    }
                }
                else
                {
                    return !showError ? "" : "[Exception] file.Length = 0";
                }
            }
            catch (Exception ex)
            {
                return !showError ? "" : "[Exception] " + ex.Message + "\n" + ex.StackTrace;
            }

        }

        [HttpPost]
        public async Task<string> GetCurrentVersion()
        {
            // QuerySnapshot snapshot = await Connection.GetCollection(Connection.COLLECTION_VERSION).WhereEqualTo("id", Connection.projectId).GetSnapshotAsync();
            return await Connection.GetSeverVersion();
        }

        [HttpPost]
        public async Task<string> UploadZipFile(IFormFile file, string folder, string fileName)
        {
            string path = GetPathSave(folder, fileName);
            if (file != null && file.Length > 0)
            {
                try
                {
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return !showError ? "" : "Upload Success :" + fileName;
                }
                catch (Exception ex)
                {
                    return !showError ? "" : "Exception :" + ex.Message;
                }
            }
            else
            {
                return !showError ? "" : "file.Length = 0";
            }
        }

        public string GetPathSave(string folder, string fileName)
        {
            string basePath = this.WebRootPath;
            string path = basePath + "/{0}/{1}";
            path = string.Format(path, folder, fileName);
            string newDirec = basePath;
            foreach (var item in folder.Split(new string[] { "/", @"\" }, StringSplitOptions.None))
            {
                newDirec += "/" + item;
                if (!Directory.Exists(newDirec))
                {
                    Directory.CreateDirectory(newDirec);
                }
            }
            return newDirec + "/" + fileName;
        }

  
        private void DeleteImageWithPath(string fieldName, string tableName, int id, int uid)
        {
            try
            {

                //SqlHelper.Log("fieldName: " + fieldName, "DeleteImageWithPath");
                //SqlHelper.Log("tableName: " + tableName, "DeleteImageWithPath");
                //SqlHelper.Log("id: " + id, "DeleteImageWithPath");
                //delete old image
                string query = "SELECT " + fieldName + " FROM " + tableName + "  WHERE ID =  " + id;
                //SqlHelper.Log("query: " + query, "DeleteImageWithPath");
                var dtoldPath = SqlHelper.ExecuteDataTable(query);
                //SqlHelper.Log("dtoldPath.Row.Count: " + dtoldPath.Rows.Count, "DeleteImageWithPath");
                if (dtoldPath.Rows.Count == 0) return;
                string? oldPath = dtoldPath.Rows[0][0]?.ToString();
                //SqlHelper.Log("oldPath: " + oldPath ?? "", "DeleteImageWithPath");
                if (string.IsNullOrEmpty(oldPath?.ToString())) return;
                string path = oldPath.ToString();
                //SqlHelper.Log("path: " + path, "DeleteImageWithPath");
                path = this.WebRootPath +"/"+  path;
                SqlHelper.Log(WowCommon.ModeLog.API, "DeleteImageWithPath: " + path, "DeleteImageWithPath", uid);
                if (!System.IO.File.Exists(path))
                {
                    //SqlHelper.Log("File.Exists: " + false, "DeleteImageWithPath");
                    return;
                }
                //SqlHelper.Log("File.Delete: " + path, "DeleteImageWithPath");
                System.IO.File.Delete(path);
            }
            catch (Exception ex)
            {
                SqlHelper.Log(WowCommon.ModeLog.API, "Exception: " + ex.Message, "DeleteImageWithPath", uid);
            }
        }

        [HttpPost]
        public void DeleteFile(string filePath, int uid)
        {
            try
            {

                if (string.IsNullOrEmpty(filePath)) return;
                if (filePath.StartsWith("images"))
                {
                    filePath = this.WebRootPath + "/" + filePath;
                }
                SqlHelper.Log(WowCommon.ModeLog.API, "DeleteFile: " + filePath, "DeleteFile", uid);
                if (!System.IO.File.Exists(filePath))
                {
                    return;
                }
                System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                SqlHelper.Log(WowCommon.ModeLog.API, "Exception: " + ex.Message, "DeleteFile", uid);
            }
        }



        #region FILE IMAGE
        public string GetPathSaveImage(string parentTable, string tableName, int id, string fileName)
        {
            try
            {
                string pathFile = GetDirectorySaveImage(parentTable, tableName, id) + @"/" + fileName;
                return pathFile;
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            return string.Empty;
        }
        public string GetDirectorySaveImage(string parentTable, string tableName, int id, string startFolder = "")
        {

            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;


            string path;
            if (string.IsNullOrEmpty(startFolder))
            {
                path = Path.Combine(webRootPath, Common.HOST_PHOTO_SUB);
            }
            else
            {
                path = string.Format("{0}/{1}", startFolder, Common.HOST_PHOTO_SUB);
                path = Path.Combine(webRootPath, path);
            }

            if (!string.IsNullOrEmpty(parentTable))
            {
                path = path + @"/" + parentTable;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = path + @"/" + id;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = path + @"/" + tableName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            else
            {
                path = path + @"/" + tableName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = path + @"/" + id;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            return path;
        }
        public async Task<string> SavePhoto(IFormFile file, string parentTable, string tableName, int id, string fileName)
        {

            if (fileName == string.Empty)
            {
                return "Some parame is empty";
            }
            string path = GetPathSaveImage(parentTable, tableName, id, fileName);

            if (file != null && file.Length > 0)
            {
                try
                {
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return "Upload Success :" + path;
                }
                catch (Exception ex)
                {
#if DEBUG
                    return "Exception :" + ex.Message;

#else
                    ex.Log();
                    return "Dữ liệu không đúng cấu trúc";
#endif
                }
            }
            else
            {
                return "file.ContentLength = 0";
            }
        }
        [HttpPost]
        public string DeleteImage(string parentTable, string tableName, int id, string fileName)
        {
            string path = this.GetPathSaveImage(parentTable, tableName, id, fileName);
            if (!System.IO.File.Exists(path))
            {
                return "File is not exists:" + path;
            }
            try
            {
                try
                {
                    string pathBackup = GetPathSaveImage(parentTable, tableName, id, "BackupDelete") + "/" + fileName;
                    System.IO.File.Copy(path, pathBackup);
                }
                catch
                {
                }
                System.IO.File.Delete(path);
                return "Delete Success, file move to folder :" + path;
            }
            catch (Exception ex)
            {
#if DEBUG
                return "Exception :" + ex.Message;

#else
                ex.Log();
                return "Dữ liệu không đúng cấu trúc";
#endif
            }
        }
        #endregion
    }
}