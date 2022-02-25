using System;
using System.Linq;
using RestaurantPOS.Models;
using RestaurantPOS.Helpers;
using System.Threading.Tasks;
using RestaurantPOS.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        //private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        private readonly IJwtHelper _jwtHelpers;
        public UserController(IJwtHelper jwtHelpers, IConfiguration config, IErrorLogRepository errorLogRepository, IUserRepository userRepository)//, ILogger<UserController> logger)
        {
            // _logger = logger;
            _config = config;
            _jwtHelpers = jwtHelpers;
            _userRepository = userRepository;
            _errorLogRepository = errorLogRepository;
        }



        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "List of all Users", OperationId = "User List")]
        public async Task<IActionResult> Get()
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {

                var UserData = await _userRepository.Get();
                var CountCook = UserData?.Count() ?? 0;
                if (UserData != null)
                {
                    response.Data = UserData;
                    response.TotalRecords = CountCook;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.StatusCode = 404;
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "userController", "GetAll");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }









        [HttpGet("GetbyId/{Id}")]
        [SwaggerOperation(Summary = "User by Id", OperationId = "Specific user")]
        public async Task<IActionResult> GetbyId(int Id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var UserResult = await _userRepository.Get(Id);
                if (UserResult != null)
                {
                    response.Data = UserResult;
                    response.TotalRecords = 1;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.StatusCode = 404;
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserControler", "GetbyId");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }







        [HttpGet("GetAllByRoleType/{AssignedTypes}")]
        [SwaggerOperation(Summary = "List of all Users by Role Type", OperationId = "User by role")]
        public async Task<IActionResult> GetAllByRoleType(int AssignedTypes)
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var CookData = await _userRepository.GetAllByRoleType(AssignedTypes);
                var CountCook = CookData?.Count() ?? 0;
                if (CookData != null)
                {
                    response.Data = CookData;
                    response.TotalRecords = CountCook;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.StatusCode = 404;
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserControler", "GetAllByRoleType");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }







        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save/Update User", OperationId = "Save User")]
        public async Task<IActionResult> Post([FromBody] UserRequestDTO model)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                long SaveResult = 0;
                bool IsExist = await _userRepository.IsExist(model.Email);
                if (IsExist)
                {
                    response.Message = ResponseMessageHelper.EmailAlreadyExist;
                    response.IsSuccess = false;
                }
                if (model != null && response.IsSuccess)
                {

                    SaveResult = await _userRepository.Save(model, Convert.ToInt32(loginUserId));
                }
                if (SaveResult >= 0)
                {
                    response.StatusCode = 200;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ResponseMessageHelper.ActionFailed;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserController", "Save");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }







        [HttpGet("GetAllUnassignUsers")]
        [SwaggerOperation(Summary = "List of all Unroled Users ", OperationId = " Unroled User List")]
        public async Task<IActionResult> GetAllUnassignUasers()
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var CookData = await _userRepository.GetAllUnassignUsers();
                var CountCook = CookData?.Count() ?? 0;
                if (CookData != null)
                {
                    response.Data = CookData;
                    response.TotalRecords = CountCook;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.StatusCode = 404;
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserControler", "GetAllUnassignUsers");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }







        [HttpPost("AssignRoleToUser")]
        [SwaggerOperation(Summary = "Assign Role to User", OperationId = "Assign User")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserRequestDTO AssignRole)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                long SaveResult = 0;
                if (AssignRole?.AssignedRole != null && AssignRole?.AssignedType != null)
                {
                    User User = await _userRepository.GetByUserId(AssignRole.UserId);
                    UserRequestDTO model = new UserRequestDTO()
                    {
                        Id = User.Id,
                        Username = User.Username,
                        AssignedRole = AssignRole.AssignedRole,
                        AssignedType = AssignRole.AssignedType,
                        UserAttachmentId = User.UserAttachmentId,
                        ContactNo = User.ContactNo,
                        Email = User.Email,
                    };
                    if (model != null && response.IsSuccess)
                    {
                        SaveResult = await _userRepository.Save(model, Convert.ToInt32(loginUserId));
                    }
                    if (SaveResult >= 0)
                    {
                        response.StatusCode = 200;
                        response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PostedDataIsNotValid;
                        response.StatusCode = 400;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ResponseMessageHelper.ActionFailed;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserController", "AssignRoleToUser");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }







        [HttpDelete("DeleteRoleOFUser/{UserId}")]
        [SwaggerOperation(Summary = "Delete Role of User", OperationId = "Delete User role")]
        public async Task<IActionResult> DeleteRoleOFUser(int UserId)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                long SaveResult = 0;
                User User = await _userRepository.GetByUserId(UserId);
                UserRequestDTO model = new UserRequestDTO()
                {
                    Id = User.Id,
                    Username = User.Username,
                    AssignedRole = null,
                    AssignedType = null,
                    UserAttachmentId = User.UserAttachmentId,
                    ContactNo = User.ContactNo,
                    Email = User.Email,
                };
                if (model != null && response.IsSuccess)
                {
                    SaveResult = await _userRepository.Save(model, Convert.ToInt32(loginUserId));
                }
                if (SaveResult >= 0)
                {
                    response.StatusCode = 200;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ResponseMessageHelper.ActionFailed;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserController", "DeleteRoleOFUser");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }






        [HttpPost("Login")]
        [SwaggerOperation(Summary = "Login User", OperationId = "Login User")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.EmailOrPasswordIsEmpty;
                }
                var validatedData = await _userRepository.ValidateAndLogin(model);
                if (validatedData == null)
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.FailedToValidateRecord;
                }
                else
                {
                    TokenGenrationRequestDTO tokenGenrationRequestDTO = new TokenGenrationRequestDTO()
                    {
                        Id = validatedData.Id,
                        Email = validatedData.Email,
                        Name = validatedData.Username,
                        AssignedRole = validatedData.AssignedRole,
                        AssignedType = validatedData.AssignedType,
                        Role = validatedData.AssignesTypeName
                    };
                    response.Data = _jwtHelpers.GenerateJSONWebTokenForUser(tokenGenrationRequestDTO);
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                    response.StatusCode = 200;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ResponseMessageHelper.ActionFailed;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserController", "Login");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }








        [HttpPost("ChangePassword")]
        [SwaggerOperation(Summary = "Api to change password for User", OperationId = "Change Password")]
        public async Task<object> ChangePassword(ChangePasswordRequestDTO changePasswordRequestDTO)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var Useristrator = await _userRepository.GetByUserId(Convert.ToInt32(loginUserId));
                if (string.IsNullOrEmpty(changePasswordRequestDTO.NewPassword) || string.IsNullOrEmpty(changePasswordRequestDTO.ConfirmPassword))
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PasswordOrConfirmPasswordIsEmpty;
                    return ResponseHelper<object>.GenerateResponse(response);
                }
                else if (!changePasswordRequestDTO.NewPassword.Equals(changePasswordRequestDTO.ConfirmPassword))
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PasswordsAreNotSame;
                    return ResponseHelper<object>.GenerateResponse(response);
                }
                if (!changePasswordRequestDTO.OldPassword.Equals(Useristrator.Password))
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.OldPasswordNotMatch;
                    return ResponseHelper<object>.GenerateResponse(response);
                }
                UserRequestDTO model = new UserRequestDTO
                {
                    Id = Useristrator.Id,
                    Username = Useristrator.Username,
                    Email = Useristrator.Email,
                    Password = changePasswordRequestDTO.NewPassword,
                    ContactNo = Useristrator.ContactNo
                };

                var result = await _userRepository.Save(model, Useristrator.Id);
                if (result != 0)
                {
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                    return ResponseHelper<object>.GenerateResponse(response);
                }
                return ResponseHelper<object>.GenerateResponse(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "UserController", "ChangePassword");
                return ResponseHelper<object>.GenerateResponse(response);
            }
        }




        [HttpPost("ForgotPassword/{Email}")]
        public async Task<IActionResult> Post(string Email)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                User loginUsers = new User();
                if (response.IsSuccess == true)
                {
                    // Validation
                    if (string.IsNullOrEmpty(Email))
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideYourEmail;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    else if (!GeneralHelper.IsValidEmail(Email))
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideValidEmail;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                }
                if (response.IsSuccess)
                {
                    /// check for isExist
                    loginUsers = await _userRepository.GetByEmail(Email);
                    if (loginUsers == null)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.UserNotExist;
                    }
                }
                // Email Code Goes Here
                ///// Code for Sending Email or SMS
                var emailRequest = new EmailRequestModel { EmailToEmail = Email };
                //EmailTemplateResponseDTO emailSettings = (await _emailTemplateServices.GetByEmailType((int)Enums.EmailTypes.ReSendOTP));
                emailRequest.EmailTemplate = "<style type='text/css'>.ReadMsgBody { width: 100% !important;} .ExternalClass {width: 100% !important;} #like { color:#000000; } #follow { color:#000000; }</style><table width='100%' height='100%' border='0' cellspacing='0' cellpadding='0' bgcolor='#FFFFFF' style='height:100% !important; margin:0; padding:0; width:100% !important; background-color:#FFFFFF'><tr><td align='center' valign='top'><table width='600' border='0' align='left' cellpadding='0' cellspacing='0'><tr><td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> <td width='580' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td><td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr><tr> <td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td><td width='580' align='left' valign='top' style='border-collapse:collapse;'><font face='Verdana, Arial, Helvetica, sans-serif' size='2'><font size='5'>D</font><strong>ear " + loginUsers.Username + ",</strong></font></td> <td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr><tr> <td height='12' width='10'></td> <td height='12' width='580'></td> <td height='12' width='10'></td> </tr><tr> <td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> <td width='580' align='left' valign='top' style='border-collapse:collapse; font-family:Arial, Helvetica, sans-serif; font-size:12px; font-weight:normal; color:#000000;'>your password is.</td> <td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr> <tr> <td width='10' height='12'></td> <td width='580' height='12'></td> <td width='10' height='12'></td> </tr> <tr> <td height='12'></td> <td height='12' style='border-collapse:collapse; font-family:Arial, Helvetica, sans-serif; font-size:12px; font-weight:normal; color:#237DAF;'>" + loginUsers.Password + "</td><td height='12'></td> </tr> <tr> <td height='12'></td> <td height='12'></td> <td height='12'></td> </tr> <tr> <td height='12'></td> <td height='12' style='border-collapse:collapse; font-family:Arial, Helvetica, sans-serif; font-size:12px; font-weight:normal; color:#000000;'>Pleaes don not share this password with any other<br><hr> </td> <td height='12'></td> </tr> <tr> <td width='10' height='12'></td><td width='580' height='12'></td> <td width='10' height='12'></td> </tr> <tr> <td width='10' align='left' valign='top'></td> <td width='580' align='left' valign='top' style='border-collapse:collapse; font-family:Arial, Helvetica, sans-serif; font-size:12px; font-weight:normal; color:#000000;'>We assure you our best services.&nbsp;</td> <td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr><tr> <td width='10' height='12'></td> <td width='580' height='12'></td> <td width='10' height='12'></td> </tr> <tr> <td width='10' align='left' valign='top'></td> <td width='580' align='left' valign='top' style='border-collapse:collapse; font-family:Verdana, Arial, Helvetica, sans-serif; font-size:22px; font-weight:normal; color:#000000;'><font face='Verdana, Arial, Helvetica, sans-serif' size='2'><font size='5'>T</font><strong>hanks</strong></font></td>";//<td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr> <tr> <td width='10' align='left' valign='top'></td> ";//<td width='580' align='left' valign='top' style='border-collapse:collapse; font-family:Arial, Helvetica, sans-serif; font-size:12px; font-weight:normal; color:#000000;'><a href='[%HostUrl%]' target='_blank' alias='[%HostUrl%]' style='color:#237DAF; font-family:Arial, Helvetica, sans-serif; font-size:12px; text-decoration:none'><span style='color:#237DAF;' title='[%HostUrl%]'>[%HostUrl%]</span></a></td><td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr> <tr> <td width='10' height='12'></td> <td width='580' height='12'></td> <td width='10' height='12'></td> </tr> <tr> <td width='10' height='12'></td> <td width='580' height='12'><hr ></td> <td width='10' height='12'></td> </tr> <tr> <td width='10' align='left' valign='top'></td> <td width='580' align='center' valign='top' style='border-collapse:collapse; font-family:Verdana, Arial, Helvetica, sans-serif; font-size:10px; font-weight:normal; color:#000000;'>This mail is sent to you because you registered at [%HostUrl%]. Its not a part of spam mails.</td><td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr> <tr> <td width='10' align='left' valign='top'></td> <td width='580' align='left' valign='top' style='border-collapse:collapse; font-family:Arial, Helvetica, sans-serif; font-size:12px; font-weight:normal; color:#000000;'>&nbsp;</td> <td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr> </table></td> </tr> </table>";//emailSettings.EmailTemplate.Replace("[%OTP%]", foodieeRequestDTO.OTP);
                emailRequest.EmailSubject = "Restuarent POS Recovery Password"; //emailSettings.EmailSubject;
                emailRequest.SMSTemplate = "<style type='text/css'>.ReadMsgBody { width: 100% !important;} .ExternalClass {width: 100% !important;} #like { color:#000000; } #follow { color:#000000; }</style><table width='100%' height='100%' border='0' cellspacing='0' cellpadding='0' bgcolor='#FFFFFF' style='height:100% !important; margin:0; padding:0; width:100% !important; background-color:#FFFFFF'><tr><td align='center' valign='top'><table width='600' border='0' align='left' cellpadding='0' cellspacing='0'><tr><td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> <td width='580' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td><td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr><tr> <td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td><td width='580' align='left' valign='top' style='border-collapse:collapse;'><font face='Verdana, Arial, Helvetica, sans-serif' size='2'><font size='5'>D</font><strong>ear [%EmailToName%],</strong></font></td> <td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr><tr> <td height='12' width='10'></td> <td height='12' width='580'></td> <td height='12' width='10'></td> </tr><tr> <td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> <td width='580' align='left' valign='top' style='border-collapse:collapse; font-family:Arial, Helvetica, sans-serif; font-size:12px; font-weight:normal; color:#000000;'>Please use this key to change pasword.</td> <td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr> <tr> <td width='10' height='12'></td> <td width='580' height='12'></td> <td width='10' height='12'></td> </tr> <tr> <td height='12'></td> <td height='12' style='border-collapse:collapse; font-family:Arial, Helvetica, sans-serif; font-size:12px; font-weight:normal; color:#237DAF;'>[%ForgotPasswordToken%]</td><td height='12'></td> </tr> <tr> <td height='12'></td> <td height='12'></td> <td height='12'></td> </tr> <tr> <td height='12'></td> <td height='12' style='border-collapse:collapse; font-family:Arial, Helvetica, sans-serif; font-size:12px; font-weight:normal; color:#000000;'>Pleaes don not share this key with any other<br><hr> For single use only </td> <td height='12'></td> </tr> <tr> <td width='10' height='12'></td><td width='580' height='12'></td> <td width='10' height='12'></td> </tr> <tr> <td width='10' align='left' valign='top'></td> <td width='580' align='left' valign='top' style='border-collapse:collapse; font-family:Arial, Helvetica, sans-serif; font-size:12px; font-weight:normal; color:#000000;'>We assure you our best services.&nbsp;</td> <td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr><tr> <td width='10' height='12'></td> <td width='580' height='12'></td> <td width='10' height='12'></td> </tr> <tr> <td width='10' align='left' valign='top'></td> <td width='580' align='left' valign='top' style='border-collapse:collapse; font-family:Verdana, Arial, Helvetica, sans-serif; font-size:22px; font-weight:normal; color:#000000;'><font face='Verdana, Arial, Helvetica, sans-serif' size='2'><font size='5'>T</font><strong>hanks</strong></font></td><td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr> <tr> <td width='10' align='left' valign='top'></td> <td width='580' align='left' valign='top' style='border-collapse:collapse; font-family:Arial, Helvetica, sans-serif; font-size:12px; font-weight:normal; color:#000000;'><a href='[%HostUrl%]' target='_blank' alias='[%HostUrl%]' style='color:#237DAF; font-family:Arial, Helvetica, sans-serif; font-size:12px; text-decoration:none'><span style='color:#237DAF;' title='[%HostUrl%]'>[%HostUrl%]</span></a></td><td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr> <tr> <td width='10' height='12'></td> <td width='580' height='12'></td> <td width='10' height='12'></td> </tr> <tr> <td width='10' height='12'></td> <td width='580' height='12'><hr ></td> <td width='10' height='12'></td> </tr> <tr> <td width='10' align='left' valign='top'></td> <td width='580' align='center' valign='top' style='border-collapse:collapse; font-family:Verdana, Arial, Helvetica, sans-serif; font-size:10px; font-weight:normal; color:#000000;'>This mail is sent to you because you registered at [%HostUrl%]. Its not a part of spam mails.</td><td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr> <tr> <td width='10' align='left' valign='top'></td> <td width='580' align='left' valign='top' style='border-collapse:collapse; font-family:Arial, Helvetica, sans-serif; font-size:12px; font-weight:normal; color:#000000;'>&nbsp;</td> <td width='10' align='left' valign='top' style='border-collapse:collapse;'>&nbsp;</td> </tr> </table></td> </tr> </table>";//emailSettings.SMSTemplate.Replace("[%OTP%]", foodieeRequestDTO.OTP);
                if (!string.IsNullOrEmpty(Email))
                {
                    await Task.Run(() => CommunicationHelpers.SendEmail(emailRequest, _config));
                }
                response.StatusCode = 200;
                response.Message = ResponseMessageHelper.YourPasswordHasBeenSentToYourGivenEmailAccount;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = 500;
                response.Message = ResponseMessageHelper.ActionFailed;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserController", "ForgotPassword");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }

        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "delete User", OperationId = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
                var deleteCategory = await _userRepository.Delete(id, Convert.ToInt32(loginUserId));
                if (deleteCategory != 0)
                {
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = ResponseMessageHelper.ActionFailed;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserController", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }




        [AllowAnonymous]
        [HttpGet("SyncData")]
        [SwaggerOperation(Summary = "All data synchronized on live database  ", OperationId = "Sync data on live")]
        public async Task<IActionResult> SyncData()
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
                var SyncData = await _userRepository.SyncData();
                if (SyncData != 0)
                {
                    response.Message = ResponseMessageHelper.DataHasBeenSynchronizedSuccessfully;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = ResponseMessageHelper.ActionFailed;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserController", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


    }
}
