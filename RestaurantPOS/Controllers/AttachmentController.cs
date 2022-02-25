using System;
using System.Linq;
using RestaurantPOS.Models;
using RestaurantPOS.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Repository;
using Microsoft.AspNetCore.Hosting;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Configuration;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;

namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("AllowOrigin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public AttachmentController(IWebHostEnvironment hostingEnvironment, IErrorLogRepository errorLogRepository, IAttachmentRepository attachmentRepository, IConfiguration config)
        {
            _hostingEnvironment = hostingEnvironment;
            _config = config;
            _attachmentRepository = attachmentRepository;
            _errorLogRepository = errorLogRepository;
        }
        ////Save Attachments ...
        [Consumes("multipart/form-data")]
        [HttpPost("SaveApplicantAttachment")]
        [SwaggerOperation(Summary = "Save attachment of Retaurent POS", OperationId = "Save Attachment")]
        public async Task<IActionResult> AddApplicantAttachments([FromForm] AttachmentRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            ImageHelper imageHelper = new ImageHelper();
            int loginUserId = Convert.ToInt32(User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault());
            long ApplicantAttachmentsId = 0;
            var filesize = _config["FileConfig:FileSizeConfig"];
            var fileType = _config["FileConfig:RestaurantFileTypeConfig"];
            ImageResponseDTO imageResponceDTO = new ImageResponseDTO();
            try
            {
                if (response.IsSuccess && model.Attachment != null && model.Attachment.Length > 0)
                {
                    imageResponceDTO = imageHelper.ImageStore(model.Attachment, new ConfigurationHelper(_config, _hostingEnvironment).GetSaveImagePath("UploadFolders:Attachments"));
                    if (Convert.ToInt32(imageResponceDTO.ImageSize) >= Convert.ToInt32(filesize))
                    {
                        response.StatusCode = 500;
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.ImageSizeToLargeToUpload;
                        //Attachment applicantAttachments = new Attachment
                        //{
                        //    Id = 0,
                        //    FileToUpLoad = imageResponceDTO.ImageUpdatedName,
                        //    FileType = "image",
                        //    IsDeleted = false,
                        //    IsSynchronized = false,
                        //};
                        //ApplicantAttachmentsId = await _attachmentRepository.Save(applicantAttachments);
                    }
                    else if (!fileType.Contains(imageResponceDTO.ImageExtension))
                    {
                        response.StatusCode = 500;
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.UplodedFileIsNotCorrect;
                    }
                }
                if (response.IsSuccess)
                {
                    Attachment applicantAttachments = new Attachment
                    {
                        Id = 0,
                        FileToUpLoad = imageResponceDTO.ImageUpdatedName,
                        FileType = imageResponceDTO.ImageExtension,
                        IsDeleted = false,
                        IsSynchronized = false,
                    };
                    ApplicantAttachmentsId = await _attachmentRepository.Save(applicantAttachments);
                }
                if (ApplicantAttachmentsId > 0)
                {
                    var result = await _attachmentRepository.GetById(Convert.ToInt32(ApplicantAttachmentsId), new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
                    response.StatusCode = 200;
                    response.Data = result;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
                //else
                //{
                //    response.Message = ResponseMessageHelper.ActionFailed;
                //}
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, Convert.ToString(loginUserId), "AttachmentController", "Save");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
    }
}
