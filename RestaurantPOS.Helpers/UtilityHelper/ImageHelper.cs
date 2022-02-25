using Microsoft.AspNetCore.Http;
using RestaurantPOS.Helpers.ResponseDTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;

namespace RestaurantPOS.Helpers.UtilityHelper
{
   public  class ImageHelper
    {
        public ImageResponseDTO ImageStore(IFormFile file, string rootPath)
        {
            string fileNameUpdated = "";
            string fileNameOriginal = "";
            string fileType = "";
            string fileSize = "";
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            string fullPath = Path.Combine(rootPath, string.Empty);
            fileNameOriginal = Path.GetFileName(file.FileName);
            string fileExt = fileNameOriginal.Substring(fileNameOriginal.LastIndexOf('.'), fileNameOriginal.Length - fileNameOriginal.LastIndexOf('.'));
            string fileExtension = Path.GetExtension(fileNameOriginal);
            fileNameUpdated = Guid.NewGuid().ToString() + fileExt;
            fileType = file.ContentType.Split('/')[0].ToString();
            fileSize = file.Length.ToString();
            string savedFileName = Path.Combine(fullPath, fileNameUpdated);
            using (var stream = new FileStream(savedFileName, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            ImageResponseDTO imageResponceDTO = new ImageResponseDTO()
            {
                ImageUpdatedName = fileNameUpdated,
                ImageExtension = fileExtension,
                ImageSize = fileSize
            };
            return imageResponceDTO;
        }
    }
}
