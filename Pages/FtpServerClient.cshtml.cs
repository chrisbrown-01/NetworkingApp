using FluentFTP;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using Windows.System;

namespace NetworkingApp.Pages

{
    //string host = "ftp://ftp.dlptest.com/";
    //string user = "dlpuser";
    //string password = "rNrKYTX9g7z3RgJRmxWuGHbeu";

    [BindProperties]
    public class FtpServerClientModel : PageModel
    {
        public FtpListItem[] FtpServerFileListing;
        private FtpClient FtpClient;



        public IFormFile FormFile { get; set; }



        public string FtpHostSite { get; set; }
        public string FtpLoginUsername { get; set; }
        public string FtpLoginPassword { get; set; }



        private readonly IWebHostEnvironment Environment;

        public FtpServerClientModel(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }

        public async Task<PageResult> OnPostLoginAsync()
        {
            try
            {
                using (FtpClient = new FtpClient(FtpHostSite, FtpLoginUsername, FtpLoginPassword))
                {
                    await FtpClient.ConnectAsync();
                    FtpServerFileListing = await FtpClient.GetListingAsync();

                    HttpContext.Session.SetString("FtpHostSite", FtpHostSite);
                    HttpContext.Session.SetString("FtpLoginUsername", FtpLoginUsername);
                    HttpContext.Session.SetString("FtpLoginPassword", FtpLoginPassword);
                }

                return Page();
            }
            catch (FtpException ftpException)
            {
                TempData["ExceptionMessage"] = ftpException.Message;
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ExceptionMessage"] = ex.Message;
                return Page();
            }
        }

        public async Task<PageResult> OnPostRefreshFileDirectoryAsync()
        {
            if (HttpContext.Session.GetString("FtpHostSite") == null)
            {
                TempData["ExceptionMessage"] = "FTP site or login has not been entered";
                return Page();
            }

            try
            {
                LoginToSavedFtpServer();

                using (FtpClient = new FtpClient(FtpHostSite, FtpLoginUsername, FtpLoginPassword))
                {
                    await FtpClient.ConnectAsync();
                    FtpServerFileListing = await FtpClient.GetListingAsync();
                }

                return Page();
            }
            catch (FtpException ftpException)
            {
                TempData["ExceptionMessage"] = ftpException.Message;
                return null;
            }
            catch (Exception ex)
            {
                TempData["ExceptionMessage"] = ex.Message;
                return null;
            }
        }

        public async Task<FileResult> OnPostDownloadFileAsync(string fileName)
        {
            try
            {
                LoginToSavedFtpServer();

                using (FtpClient = new FtpClient(FtpHostSite, FtpLoginUsername, FtpLoginPassword))
                {
                    await FtpClient.ConnectAsync();
                    FtpServerFileListing = await FtpClient.GetListingAsync(); // Update the file directory listing so it will be redisplayed in event of exception
                    FtpClient.RetryAttempts = 3;

                    if (await FtpClient.FileExistsAsync(fileName))
                    {
                        string downloadPath = Path.Combine(GetFileFolderLocation(), fileName); // Save the file to the web app directory location. Keep the existing file name from FTP server

                        await FtpClient.DownloadFileAsync(downloadPath, fileName, FtpLocalExists.Overwrite, FtpVerify.Retry); // Download file to web app directory

                        byte[] bytes = System.IO.File.ReadAllBytes(downloadPath);

                        //Send the File to Download.
                        return File(bytes, "application/octet-stream", fileName); // Download the file from the web app directory to the user's internet browser
                    }
                    else
                    {
                        TempData["ExceptionMessage"] = "File is no longer available for download from the server.";
                        return null;
                    }
                }
            }
            catch (FtpException ftpException)
            {
                TempData["ExceptionMessage"] = ftpException.Message;
                return null;
            }
            catch (Exception ex)
            {
                TempData["ExceptionMessage"] = ex.Message;
                return null;
            }
        }

        public async Task<PageResult> OnPostDeleteFileAsync(string fileName)
        {
            try
            {
                LoginToSavedFtpServer();

                using (FtpClient = new FtpClient(FtpHostSite, FtpLoginUsername, FtpLoginPassword))
                {
                    await FtpClient.ConnectAsync();
                    FtpServerFileListing = await FtpClient.GetListingAsync(); // Update the file directory listing so it will be redisplayed in event of exception
                    FtpClient.RetryAttempts = 3;

                    if (await FtpClient.FileExistsAsync(fileName))
                    {
                        await FtpClient.DeleteFileAsync(fileName);

                        FtpServerFileListing = await FtpClient.GetListingAsync();
                        return Page();
                    }
                    else
                    {
                        TempData["ExceptionMessage"] = "File is no longer available for deletion from the server.";
                        return Page();
                    }
                }
            }
            catch (FtpException ftpException)
            {
                TempData["ExceptionMessage"] = ftpException.Message;
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ExceptionMessage"] = ex.Message;
                return Page();
            }
        }

        public async Task<PageResult> OnPostUploadFileAsync()
        {
            if (FormFile == null || (FormFile.Length <= 0))
            {
                TempData["ExceptionMessage"] = "File was not selected or has size of 0.";
                return Page();
            }

            if (HttpContext.Session.GetString("FtpHostSite") == null)
            {
                TempData["ExceptionMessage"] = "FTP site or login has not been entered";
                return Page();
            }

            try
            {
                LoginToSavedFtpServer();

                using (FtpClient = new FtpClient(FtpHostSite, FtpLoginUsername, FtpLoginPassword))
                {
                    await FtpClient.ConnectAsync();
                    FtpServerFileListing = await FtpClient.GetListingAsync(); // Update the file directory listing so it will be redisplayed in event of exception
                    FtpClient.RetryAttempts = 3;

                    var filePath = Path.Combine(GetFileFolderLocation(), FormFile.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await FormFile.CopyToAsync(stream);
                    }

                    await FtpClient.UploadFileAsync(filePath, FormFile.FileName, FtpRemoteExists.Overwrite, false, FtpVerify.Retry);
                    FtpServerFileListing = await FtpClient.GetListingAsync(); // Refresh file directory listing with newly uploaded file
                }

                return Page();
            }
            catch (FtpException ftpException)
            {
                TempData["ExceptionMessage"] = ftpException.Message;
                return null;
            }
            catch (Exception ex)
            {
                TempData["ExceptionMessage"] = ex.Message;
                return null;
            }
        }

        private void LoginToSavedFtpServer()
        {
            FtpHostSite = HttpContext.Session.GetString("FtpHostSite");
            FtpLoginUsername = HttpContext.Session.GetString("FtpLoginUsername");
            FtpLoginPassword = HttpContext.Session.GetString("FtpLoginPassword");
        }

        private string GetFileFolderLocation()
        {
            //string webRootPath = Environment.WebRootPath;
            return Environment.ContentRootPath + @"\DOWNLOADS";
        }
    }
}




