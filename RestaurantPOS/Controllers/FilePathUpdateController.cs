using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using RestaurantPOS.Helpers;
using RestaurantPOS.Repository;
using RestaurantPOS.Helpers.ResponseDTO;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using RestaurantPOS.Helpers.UtilityHelper;
namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FilePathUpdateController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public FilePathUpdateController(IUserRepository userRepository, IConfiguration config, IErrorLogRepository errorLogRepository)//, ILogger<PermissionController> logger)
        {
            _config = config;
            _userRepository = userRepository;
            _errorLogRepository = errorLogRepository;
        }

        //Update base path in case of changing hosting 
        [HttpGet("UpdateFilePath")]
        [SwaggerOperation(Summary = "Update file path url", OperationId = "Update file path")]
        public async Task<IActionResult> UpdateFilePath()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                var SaveResult = await _userRepository.UpdateFilePath();
                if (SaveResult >= 0)
                {
                    response.StatusCode = 200;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
                else
                {
                    response.StatusCode = 400;
                    response.Message = ResponseMessageHelper.ActionFailed;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ResponseMessageHelper.ActionFailed;
                response.IsSuccess = false;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "FilePathUpdateController", "Save");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
    }
}