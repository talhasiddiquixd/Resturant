using RestaurantPOS.Helpers.ResponseDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantPOS.Helpers.UtilityHelper
{
    public class ResponseHelper<T>
    {
        public static ObjectResult GenerateResponse(ResponseDTO<T> response)
        {
            ObjectResult objectResult = new ObjectResult(response);

            if (response.StatusCode == 100)
                objectResult.StatusCode = StatusCodes.Status100Continue;
            else if (response.StatusCode == 101)
                objectResult.StatusCode = StatusCodes.Status101SwitchingProtocols;
            else if (response.StatusCode == 102)
                objectResult.StatusCode = StatusCodes.Status102Processing;
            else if (response.StatusCode == 200)
                objectResult.StatusCode = StatusCodes.Status200OK;
            else if (response.StatusCode == 201)
                objectResult.StatusCode = StatusCodes.Status201Created;
            else if (response.StatusCode == 202)
                objectResult.StatusCode = StatusCodes.Status202Accepted;
            else if (response.StatusCode == 203)
                objectResult.StatusCode = StatusCodes.Status203NonAuthoritative;
            else if (response.StatusCode == 204)
                objectResult.StatusCode = StatusCodes.Status204NoContent;
            else if (response.StatusCode == 205)
                objectResult.StatusCode = StatusCodes.Status205ResetContent;
            else if (response.StatusCode == 206)
                objectResult.StatusCode = StatusCodes.Status206PartialContent;
            else if (response.StatusCode == 207)
                objectResult.StatusCode = StatusCodes.Status207MultiStatus;
            else if (response.StatusCode == 208)
                objectResult.StatusCode = StatusCodes.Status208AlreadyReported;
            else if (response.StatusCode == 226)
                objectResult.StatusCode = StatusCodes.Status226IMUsed;
            else if (response.StatusCode == 300)
                objectResult.StatusCode = StatusCodes.Status300MultipleChoices;
            else if (response.StatusCode == 301)
                objectResult.StatusCode = StatusCodes.Status301MovedPermanently;
            else if (response.StatusCode == 302)
                objectResult.StatusCode = StatusCodes.Status302Found;
            else if (response.StatusCode == 303)
                objectResult.StatusCode = StatusCodes.Status303SeeOther;
            else if (response.StatusCode == 304)
                objectResult.StatusCode = StatusCodes.Status304NotModified;
            else if (response.StatusCode == 305)
                objectResult.StatusCode = StatusCodes.Status305UseProxy;
            else if (response.StatusCode == 306)
                objectResult.StatusCode = StatusCodes.Status306SwitchProxy;
            else if (response.StatusCode == 307)
                objectResult.StatusCode = StatusCodes.Status307TemporaryRedirect;
            else if (response.StatusCode == 308)
                objectResult.StatusCode = StatusCodes.Status308PermanentRedirect;
            else if (response.StatusCode == 400)
                objectResult.StatusCode = StatusCodes.Status400BadRequest;
            else if (response.StatusCode == 401)
                objectResult.StatusCode = StatusCodes.Status401Unauthorized;
            else if (response.StatusCode == 402)
                objectResult.StatusCode = StatusCodes.Status402PaymentRequired;
            else if (response.StatusCode == 403)
                objectResult.StatusCode = StatusCodes.Status403Forbidden;
            else if (response.StatusCode == 404)
                objectResult.StatusCode = StatusCodes.Status404NotFound;
            else if (response.StatusCode == 405)
                objectResult.StatusCode = StatusCodes.Status405MethodNotAllowed;
            else if (response.StatusCode == 406)
                objectResult.StatusCode = StatusCodes.Status406NotAcceptable;
            else if (response.StatusCode == 407)
                objectResult.StatusCode = StatusCodes.Status407ProxyAuthenticationRequired;
            else if (response.StatusCode == 408)
                objectResult.StatusCode = StatusCodes.Status408RequestTimeout;
            else if (response.StatusCode == 409)
                objectResult.StatusCode = StatusCodes.Status409Conflict;
            else if (response.StatusCode == 410)
                objectResult.StatusCode = StatusCodes.Status410Gone;
            else if (response.StatusCode == 411)
                objectResult.StatusCode = StatusCodes.Status411LengthRequired;
            else if (response.StatusCode == 412)
                objectResult.StatusCode = StatusCodes.Status412PreconditionFailed;
            else if (response.StatusCode == 413)
                objectResult.StatusCode = StatusCodes.Status413PayloadTooLarge;
            else if (response.StatusCode == 413)
                objectResult.StatusCode = StatusCodes.Status413RequestEntityTooLarge;
            else if (response.StatusCode == 414)
                objectResult.StatusCode = StatusCodes.Status414RequestUriTooLong;
            else if (response.StatusCode == 415)
                objectResult.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            else if (response.StatusCode == 416)
                objectResult.StatusCode = StatusCodes.Status416RangeNotSatisfiable;
            else if (response.StatusCode == 416)
                objectResult.StatusCode = StatusCodes.Status416RequestedRangeNotSatisfiable;
            else if (response.StatusCode == 417)
                objectResult.StatusCode = StatusCodes.Status417ExpectationFailed;
            else if (response.StatusCode == 418)
                objectResult.StatusCode = StatusCodes.Status418ImATeapot;
            else if (response.StatusCode == 419)
                objectResult.StatusCode = StatusCodes.Status419AuthenticationTimeout;
            else if (response.StatusCode == 412)
                objectResult.StatusCode = StatusCodes.Status421MisdirectedRequest;
            else if (response.StatusCode == 422)
                objectResult.StatusCode = StatusCodes.Status422UnprocessableEntity;
            else if (response.StatusCode == 423)
                objectResult.StatusCode = StatusCodes.Status423Locked;
            else if (response.StatusCode == 424)
                objectResult.StatusCode = StatusCodes.Status424FailedDependency;
            else if (response.StatusCode == 426)
                objectResult.StatusCode = StatusCodes.Status426UpgradeRequired;
            else if (response.StatusCode == 428)
                objectResult.StatusCode = StatusCodes.Status428PreconditionRequired;
            else if (response.StatusCode == 429)
                objectResult.StatusCode = StatusCodes.Status429TooManyRequests;
            else if (response.StatusCode == 431)
                objectResult.StatusCode = StatusCodes.Status431RequestHeaderFieldsTooLarge;
            else if (response.StatusCode == 451)
                objectResult.StatusCode = StatusCodes.Status451UnavailableForLegalReasons;
            else if (response.StatusCode == 500)
                objectResult.StatusCode = StatusCodes.Status500InternalServerError;
            else if (response.StatusCode == 501)
                objectResult.StatusCode = StatusCodes.Status501NotImplemented;
            else if (response.StatusCode == 502)
                objectResult.StatusCode = StatusCodes.Status502BadGateway;
            else if (response.StatusCode == 503)
                objectResult.StatusCode = StatusCodes.Status503ServiceUnavailable;
            else if (response.StatusCode == 504)
                objectResult.StatusCode = StatusCodes.Status504GatewayTimeout;
            else if (response.StatusCode == 505)
                objectResult.StatusCode = StatusCodes.Status505HttpVersionNotsupported;
            else if (response.StatusCode == 506)
                objectResult.StatusCode = StatusCodes.Status506VariantAlsoNegotiates;
            else if (response.StatusCode == 507)
                objectResult.StatusCode = StatusCodes.Status507InsufficientStorage;
            else if (response.StatusCode == 508)
                objectResult.StatusCode = StatusCodes.Status508LoopDetected;
            else if (response.StatusCode == 510)
                objectResult.StatusCode = StatusCodes.Status510NotExtended;
            else if (response.StatusCode == 511)
                objectResult.StatusCode = StatusCodes.Status511NetworkAuthenticationRequired;
            else
                objectResult.StatusCode = StatusCodes.Status500InternalServerError;

            return objectResult;
        }
    }

    public static class ResponseMessageHelper
    {
        /// <summary>
        /// FcmTokenController
        /// </summary>
        public static string PleaseProvideUserId = "Please provide User Id";
        public static string PleaseProvideUserType = "Please provide User Type";
        /// <summary>
        /// <summary>
        /// FoodCategory Controller
        public static string PleaseProvideFoodCategoryName = "Please provide food category name";
        public static string PleaseProvideFoodCategoryImage = "Please provide food category image";
        public static string FoodCategoryNameAlreadyEsixt = "This food category name already exist";
        /// </summary>
        /// <summary>
        /// FoodCategoryOffer Controller
        public static string PleaseProvideFoodCategoryOfferName = "Please provide food category offername";
        public static string PleaseProvideFoodCategoryOfferStartDate = "Please provide food category offer start date";
        public static string PleaseProvideFoodCategoryOfferEndDate = "Please provide food category offer end date";
        public static string PleaseProvideFoodCategoryOfferStatus = "Please provide food category offer status";
        /// </summary>
        /// <summary>
        /// FoodItem Controller
        public static string PleaseProvideFoodItemName = "Please provide food item name";
        public static string PleaseProvideFoodItemImage = "Please provide food item image";
        public static string PleaseProvideKitchenName = "Please provide kitchen name";
        public static string PleaseProvideFoodItemPrice = "Please provide food item price";
        public static string PleaseProvideFoodItemCookingName = "Please provide food item cooking name";
        public static string FoodItemNameAlreadyEsixt = "This food item name already exist";
        /// </summary>
        /// <summary>
        /// FoodItemOffer Controller
        public static string PleaseProvideFoodItemOfferName = "Please provide food item offer name";
        public static string PleaseProvideFoodItemOfferPrice = "Please provide food item offer price";
        public static string PleaseProvideFoodItemOfferStartDate = "Please provide food item offer start date";
        public static string PleaseProvideFoodItemOfferEndDate = "Please provide food item offer end date";
        /// </summary>
        /// <summary>
        /// Order Controller
        public static string PleaseProvideCookingTimeInCorrectFormate = "Cookiing time is not in correct formate";
        public static string PleaseProvideCookingTime = "Please provide Cooking time";
        public static string PleaseProvideOrderAmount = "Please provide order amount";
        public static string PleaseProvideOrderTotalAmount = "Please provide order total amount";
        /// </summary>
        /// <summary>
        /// <UserController>
        public static string PleaseProvideUserName = "Please provide UserName";
        public static string PleaseProvideEmail = "Please provide email";
        public static string PleaseProvidePassword = "Please provide password";
        public static string PleaseProvideContactNo = "Please provide contact number";
        public static string PleaseProvideValidEmail = "Please provide vaild Email or ContactNo";
        public static string PasswordsAreNotSame = "Password don't match";
        public static string OldPasswordNotMatch = "Please provide correct old password";
        public static string ThisEmailAlreadyExist = "This email already exist please try with other one";
        public static string PasswordOrConfirmPasswordIsEmpty = "password or confirm password both are required";
        public static string UserNotExist = "User not exist";
        public static string PleaseProvideYourEmail = "Please provide your email";
        public static string DataHasBeenSynchronizedSuccessfully = "Data has been updated on live successfully";
        public static string YourPasswordHasBeenSentToYourGivenEmailAccount = "Your password has been sent to your email account";
        /// </summary>
        /// <summary>
        /// NotificationController
        /// </summary>
        public static string PleaseProvideNotificationTitle = "Please provide notification title ";
        public static string PleaseProvideNotificationType = "Please provide notification type ";
        public static string PleaseProvideNotificationDescriptions = "Please provide notification description ";
        /// <summary>
        /// <EmailTemplateController>
        /// </summary>
        public static string PleaseProvideEmailTemplate = "Please select Email Template";
        public static string PleaseProvideEmailSettingsName = "Please provide Email Template Name";
        public static string PleaseProvideEmailSubject = "Please provide Email Subject";
        /// <summary>
        /// Cart Items Messages
        /// </summary>
        public static string PleaseSelectFoodieeForUpdatingCart = "Please select foodiee for updating Cart Items";
        public static string PleaseEnterQuantityGreaterThenZero = "Please enter Items quantity greater then zero";
        public static string PleaseUpdateDishPrice = "Please Update Dish Price";
        /// </summary>    
        /// Gernal messages
        /// </summary>
        public static string ActivatedSuccessfully = "Activated successfully";
        public static string ActionPerformSuccessfully = "Action perform successfully";
        public static string PleaseProvideRquiredFields = "Please provide required fields";
        public static string ActionFailed = "Some error occured";
        public static string UplodedFileIsNotCorrect = "please upload the releavent file`";
        public static string ImageSizeToLargeToUpload = "Image size to large to upload";
        public static string EmailOrPasswordIsEmpty = "Email or password is empty";
        public static string LoginSuccessfully = "Login Successfully";
        public static string FailedToValidateRecord = "Failed to validate record";
        public static string EmailAlreadyExist = "Email already exist, please signup with another email";
        public static string NoRecordFound = "Record not found.";
        public static string DataLoadedSuccessfully = "Data loaded successfully";
        public static string PostedDataIsNotValid = "Posted dats is not valid";
        /// </summary>
        /// Premission Controller
        /// <summary>
        public static string PleaseProvidePremissionName = "Please provide Premission name";
        /// </summary>
        /// Role Controller
        /// </summary>
        public static string PleaseProvideCookName = "Please Select Cook to Follow or Unfollow.";
        public static string PleaseProvideRoleId = "Please Provide Role Id.";
        public static string PleaseProvideRoleName = "Please Provide Role Name.";
        /// </summary>    
        /// Kitchen messages
        /// </summary>
        public static string PleaseProvideKitchenId = "Please Provide Kitchen Id";
        public static string PleaseProvideKitchensName = "Kitchen's Name Already Exist";
        public static string PleaseProvideKitchenDescription = "Please Provide Kitchen Description";
        public static string AKitchenHasbeenAlreadyAssignToThisUser = " A kitchen has been already assigned to this user";
        /// </summary>    
        /// Hall messages
        /// </summary>
        public static string PleaseProvideHallId = "Please Provide Hall Id";
        public static string PleaseProvideHallName = "Please Provide Hall Name";
        public static string PleaseProvideHallsName = " Hall Name already exsist";
        public static string AHallHasbeenAlreadyAssignToThisUser = "This hall has been already assign to this user";
        public static string PleaseProvideHallDescription = "Please Provide HallDescription";
        /// </summary>    
        /// Hall Assign messages
        /// </summary>
        public static string PleaseProvideHallAssignId = "Please Provide Assigned Hall Id";
        public static string PleaseProvideHallAssignName = "Please Provide Assigned Hall Name";
        public static string PleaseProvideAssignHallsName = "Assigned Hall Name already exsist";
        /// </summary>    
        /// Table messages
        /// </summary>
        public static string PleaseProvideTableId = "Please Provide Table Id";
        public static string PleaseProvideTableName = "Please Provide Table Name";
        public static string PleaseProvideTablesName = " Table Name already exsist";
        public static string PleaseProvideTableDescription = "Please Provide Table Description";
        /// </summary>    
        /// Addons messages
        /// </summary>
        public static string PleaseProvideAddOnsName = "Please Provide AddOns Name";
        public static string PleaseProvideFoodVarient = "Please Provide Food Varient ";
        /// </summary>    
        /// Addons messages
        /// </summary>
        public static string PleaseProvideCounterName = "Please Provide Counter Name";
        public static string PleaseProvideCountersName = "Counters Name Already Added";
        public static string PleaseProvideCounterId = "Please Provide Counter Id ";
        /// </summary>    
        /// Counter Assign messages
        /// </summary>
        public static string PleaseProvideCounterAssignName = "Please Provide Assigned Counter Name";
        public static string PleaseProvideCounterAssignId = "Please Provide Assigned Counter Id";
        public static string PleaseProvideCounterAssignsName = "Assigned Name Already exsist";
        /// </summary>    
        /// Food Variant messages
        /// </summary>
        public static string PleaseProvideFoodItemId = "Please Provide FoodItem Id";
        public static string PleaseProvideFoodVariantName = "Please Provide Food Variant Name";
        public static string PleaseProvideFoodVariantId = "Please Provide Food Variant Id";
        public static string PleaseProvideFoodVariantsName = "Food Variant Name Already Exist";
        public static string PleaseProvidePrice = "Please Provide Food Variant Price";
        public static string ThisCounterHasbeenAlreadyAssignToThisUser = "This counter has been already assigned to this user";
        /// <summary>
        /// restaurant Messages
        /// </summary>
        public static string PleaseProvideReastaurantEmail = "Please Provide restaurant email";
        public static string PleaseProvideReastaurantAddress = "Please Provide restaurant adderss";
        public static string PleaseProvideReastaurantContactNo = "Please Provide contact number";
        public static string PleaseProvideReastaurantName = "Please Provide restaurant name";
    }
}