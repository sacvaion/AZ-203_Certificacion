using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppAz203.Businness;

namespace WebAppAz203.Controllers
{
    public class LoadFileController : Controller
    {
        // GET: LoadFile
        public ActionResult Index()
        {
            return View();
        }

        // GET: LoadFile/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LoadFile/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.Message = "List contains: " + StorageBlobService.GetBlobList("images").Count();
                ViewBag.ListImages = StorageBlobService.GetBlobList("images");
            }
            catch (Exception ex)
            {

                ViewBag.Message = "Error " + ex.Message;
            }


            return View();
        }
        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    string urlImg = "";
                    string storageConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConString"];
                    byte[] contentBlob = new byte[file.ContentLength];
                    file.InputStream.Read(contentBlob, 0, file.ContentLength);
                    string url = StorageBlobService.SaveBlob(contentBlob, Path.GetFileName(file.FileName), "images", "images2");
                    //ViewBag.Message = string.Format("You Image is upload! URL = {0}", url);
                    ViewBag.ListImages = StorageBlobService.GetBlobList("images");

                    /*
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer containerRef = blobClient.GetContainerReference("imagenes".ToLower());
                    containerRef.CreateIfNotExists();
                    containerRef.SetPermissions(new BlobContainerPermissions()
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
                    CloudBlockBlob blob = containerRef.GetBlockBlobReference("foto.png");
                    blob.DeleteIfExists();
                    blob.Properties.ContentType = "blob";
                    using (MemoryStream stream = new MemoryStream(contentBlob))
                    {
                        blob.UploadFromStream(stream);
                    }
                    urlImg = blob.Uri.ToString();
                    */
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR: " + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View();
        }


        // GET: LoadFile/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LoadFile/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: LoadFile/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LoadFile/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /*
         * "foto.png", "imagenes", "blob"
        public string getImage(string blobName, string containerName, string contentType)
        {
            string urlImg = "";
            try
            {
                string storageConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConString"];
                string pathImg = @"C:\Users\carlos.siabato\Pictures\Img.png";
                if (File.Exists(pathImg))
                {
                    byte[] contentBlob = File.ReadAllBytes(pathImg);

                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer containerRef = blobClient.GetContainerReference(containerName.ToLower());
                    containerRef.CreateIfNotExists();
                    containerRef.SetPermissions(new BlobContainerPermissions()
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
                    CloudBlockBlob blob = containerRef.GetBlockBlobReference(blobName);
                    blob.DeleteIfExists();
                    blob.Properties.ContentType = contentType;
                    using (MemoryStream stream = new MemoryStream(contentBlob))
                    {
                        blob.UploadFromStream(stream);
                    }
                    urlImg = blob.Uri.ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return urlImg;
        }*/

    }
}
