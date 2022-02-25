using System;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using RestaurantPOS.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Repository;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using static RestaurantPOS.Helpers.Enums;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsRepository _reportsRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;


        //private readonly IHubContext<OrderNotificationHub> _orderNotificationHubContext;
        //private readonly IOrderNotificationsManager _orderNotificationsManager;
        public ReportsController(IConfiguration config, IReportsRepository reportsRepository, ILogger<OrderController> logger, IErrorLogRepository errorLogRepository)
        {
            _reportsRepository = reportsRepository;
            _errorLogRepository = errorLogRepository;
            _config = config;
        }



        // GetAllOrders...
        [HttpPost("GetAllOrders")]
        [SwaggerOperation(Summary = "Get all orders", OperationId = "Get all")]
        public async Task<IActionResult> GetAllOrders(GetAllOrdersRequestDTO model)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<OrderResponseDTO>> response = new ResponseDTO<IEnumerable<OrderResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var responseData = await _reportsRepository.GetAllOrders(model);
                var count = responseData.Count();
                if (responseData != null && count > 0)
                {
                    response.Data = responseData;
                    response.TotalRecords = count;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "ReportsController", "GetAllOrders");
            }
            return ResponseHelper<IEnumerable<OrderResponseDTO>>.GenerateResponse(response);
        }
        // ExportAllOrdersReportToExcel...
        [HttpPost("ExportAllOrdersReportToExcel")]
        [SwaggerOperation(Summary = "Export all orders to excel", OperationId = "All order export to excel")]
        public async Task<IActionResult> ExportAllOrdersReportToExcel(GetAllOrdersRequestDTO model)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<OrderResponseDTO>> response = new ResponseDTO<IEnumerable<OrderResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var responseData = await _reportsRepository.GetAllOrders(model);
                var count = responseData.Count();
                if (responseData != null && count > 0)
                {
                    response.Data = responseData;
                    response.TotalRecords = count;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"SpecificOrderTypesReportsheet-{DateTime.Now:d/MMMM/yyyy/h/m/s}.xlsx";
                    try
                    {
                        string month = DateTime.Now.ToString("MMMM-yyyy");
                        using var workbook = new XLWorkbook();
                        int ind = 1;
                        int index = 1;
                        decimal? sum = 0;
                        IXLWorksheet worksheet =
                        workbook.Worksheets.Add(month.ToString());
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 12)).Merge().Value = "Specific Order Types Report ( " + model.From.ToString("dd-MM-yyyy") + " - " + model.To.ToString("dd-MM-yyyy") + ")";
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 12)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 12)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                        worksheet.Cell(2, 1).Value = "SrNo.";
                        worksheet.Cell(2, 1).Style.Font.Bold = true;
                        worksheet.Cell(2, 2).Value = "HallName";
                        worksheet.Cell(2, 2).Style.Font.Bold = true;
                        worksheet.Cell(2, 3).Value = "TableName";
                        worksheet.Cell(2, 3).Style.Font.Bold = true;
                        worksheet.Cell(2, 4).Value = "Amount";
                        worksheet.Cell(2, 4).Style.Font.Bold = true;
                        worksheet.Cell(2, 5).Value = "Discount";
                        worksheet.Cell(2, 5).Style.Font.Bold = true;
                        worksheet.Cell(2, 6).Value = "ServiceCharges";
                        worksheet.Cell(2, 6).Style.Font.Bold = true;
                        worksheet.Cell(2, 7).Value = "DeliveryCharges";
                        worksheet.Cell(2, 7).Style.Font.Bold = true;
                        worksheet.Cell(2, 8).Value = "Tax";
                        worksheet.Cell(2, 8).Style.Font.Bold = true;
                        worksheet.Cell(2, 9).Value = "TotalAmount";
                        worksheet.Cell(2, 9).Style.Font.Bold = true;
                        worksheet.Cell(2, 10).Value = "OrderDate";
                        worksheet.Cell(2, 10).Style.Font.Bold = true;
                        worksheet.Cell(2, 11).Value = "OrderStatus";
                        worksheet.Cell(2, 11).Style.Font.Bold = true;
                        worksheet.Cell(2, 12).Value = "OrderType";
                        worksheet.Cell(2, 12).Style.Font.Bold = true;
                        for (index = 2; index < (responseData.Count + 2); index++, ind++)
                        {
                            worksheet.Cell(index + 1, 1).Value = index - 1;
                            worksheet.Cell(index + 1, 1).Style.Font.Bold = true;
                            worksheet.Cell(index + 1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                            worksheet.Cell(index + 1, 2).Value = responseData[index - 2].HallName;
                            worksheet.Cell(index + 1, 3).Value = responseData[index - 2].TableName;
                            worksheet.Cell(index + 1, 4).Value = responseData[index - 2].Amount;
                            worksheet.Cell(index + 1, 5).Value = responseData[index - 2].Discount;
                            worksheet.Cell(index + 1, 6).Value = responseData[index - 2].ServiceCharges;
                            worksheet.Cell(index + 1, 7).Value = responseData[index - 2].DeliveryCharges;
                            worksheet.Cell(index + 1, 8).Value = responseData[index - 2].Tax;
                            worksheet.Cell(index + 1, 9).Value = responseData[index - 2].TotalAmount;
                            worksheet.Cell(index + 1, 10).Value = responseData[index - 2].CreatedDate;
                            worksheet.Cell(index + 1, 11).Value = responseData[index - 2].OrderStatus;
                            if (responseData[index - 2].OrderType == 1)
                                worksheet.Cell(index + 1, 12).Value = "Online";
                            if (responseData[index - 2].OrderType == 2)
                                worksheet.Cell(index + 1, 12).Value = "TakeAway";
                            if (responseData[index - 2].OrderType == 3)
                                worksheet.Cell(index + 1, 12).Value = "Dinning";
                            if (responseData[index - 2].OrderType == 4)
                                worksheet.Cell(index + 1, 12).Value = "DriveThrough";
                            if (responseData[index - 2].OrderType == 5)
                                worksheet.Cell(index + 1, 12).Value = "Delivery";
                            sum += responseData[index - 2].TotalAmount;
                            worksheet.Column(1).AdjustToContents();
                            worksheet.Column(2).AdjustToContents();
                            worksheet.Column(3).AdjustToContents();
                            worksheet.Column(4).AdjustToContents();
                            worksheet.Column(5).AdjustToContents();
                            worksheet.Column(6).AdjustToContents();
                            worksheet.Column(7).AdjustToContents();
                            worksheet.Column(8).AdjustToContents();
                            worksheet.Column(9).AdjustToContents();
                            worksheet.Column(10).AdjustToContents();
                            worksheet.Column(12).AdjustToContents();
                            worksheet.Column(13).AdjustToContents();
                        }
                        worksheet.Cell(index + 1, 9).Value = sum;
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 8)).Merge().Value = "Total Sum";
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 8)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 8)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(index + 1, 1).Style.Fill.BackgroundColor = XLColor.Green;
                        // worksheet.Cell(index + 1, 2).Style.Fill.BackgroundColor = XLColor.Green;
                        // worksheet.Cell(index + 1, 1).Style.Font.FontColor = XLColor.White;
                        //worksheet.Cell(index + 1, 2).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 9).Style.Fill.BackgroundColor = XLColor.LightPink;
                        worksheet.Cell(index + 1, 9).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 9).Style.Font.Bold = true;
                        //using var stream = new MemoryStream();
                        //workbook.SaveAs(stream);
                        //var content = stream.ToArray();
                        //return File(content, contentType, fileName);
                        using var stream = new MemoryStream();
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                    catch (Exception ex)
                    {
                        return Ok(new string("Error" + ex.ToString()));
                    }
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                //response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "ReportsController", "ExportAllOrdersReportToExcel");
            }
            return ResponseHelper<IEnumerable<OrderResponseDTO>>.GenerateResponse(response);
        }


        //// GetByOrderId....
        [HttpGet("GetByOrderId/{OrderId}")]
        [SwaggerOperation(Summary = "Get specific order", OperationId = "Get by id")]
        public async Task<IActionResult> Get(int OrderId)
        {
            ResponseDTO<OrderResponseDTO> response = new ResponseDTO<OrderResponseDTO>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.GetByOrderId(OrderId);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "ReportsController", "GetByOrderId");
            }
            return ResponseHelper<OrderResponseDTO>.GenerateResponse(response);
        }


        //// GetOrdersByKitchenId
        [HttpPost("GetOrdersByKitchenId")]
        [SwaggerOperation(Summary = "Get All Orders of specific kitchen", OperationId = "Get Orders by kitchen id")]
        public async Task<IActionResult> GetOrdersByKitchenId(KitshensOrderRequestDTO model)
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.GetOrdersByKitchenId(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "ReportsController", "GetOrdersByKitchenId");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }


        //// KitchenSalesReport...
        [HttpPost("KitchenSalesReport")]
        [SwaggerOperation(Summary = "Get all kitchen sales report", OperationId = "All kitchen report")]
        public async Task<IActionResult> KitchenReport(GetAllOrdersRequestDTO model)
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.KitchenReport(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "ReportsController", "KitchenSalesReport");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }
        // ExportKitchenSalesReportToExcel...
        [HttpPost("ExportKitchenSalesReportToExcel")]
        [SwaggerOperation(Summary = "Get all kitchen sales report to excel", OperationId = "kitchen sale export to excel")]
        public async Task<IActionResult> KitchenSalesReportExportInExcel(GetAllOrdersRequestDTO model)
        {
            var responseData = await _reportsRepository.KitchenReport(model);
            if (responseData != null)
            {
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string v = $"KitchenReportsheet-{DateTime.Now:d/MMMM/yyyy/h/m/s}.xlsx";
                string fileName = v;
                try
                {
                    string month = DateTime.Now.ToString("MMMM-yyyy");
                    using var workbook = new XLWorkbook();
                    int ind = 1;
                    int index = 1;
                    decimal? sum = 0;
                    IXLWorksheet worksheet =
                    workbook.Worksheets.Add(month.ToString());
                    worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Value = "Kitchens Sale Report (" + model.From.Date + " - " + model.To.Date + ")";
                    worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Style.Font.Bold = true;
                    worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                    worksheet.Cell(2, 1).Value = "SrNo.";
                    worksheet.Cell(2, 1).Style.Font.Bold = true;
                    worksheet.Cell(2, 2).Value = "KitchenName";
                    worksheet.Cell(2, 2).Style.Font.Bold = true;
                    worksheet.Cell(2, 3).Value = "Total";
                    worksheet.Cell(2, 3).Style.Font.Bold = true;
                    for (index = 2; index < (responseData.Count + 2); index++, ind++)
                    {
                        worksheet.Cell(index + 1, 1).Value = index - 1;
                        worksheet.Cell(index + 1, 1).Style.Font.Bold = true;
                        worksheet.Cell(index + 1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                        worksheet.Cell(index + 1, 2).Value = responseData[index - 2].KitchenName;
                        worksheet.Cell(index + 1, 3).Value = responseData[index - 2].Total;
                        sum += responseData[index - 2].Total;
                        worksheet.Column(1).AdjustToContents();
                        worksheet.Column(2).AdjustToContents();
                        worksheet.Column(3).AdjustToContents();
                    }
                    worksheet.Cell(index + 1, 3).Value = sum;
                    worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 2)).Merge().Value = "Total Sum";
                    worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 2)).Merge().Style.Font.Bold = true;
                    worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 2)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(index + 1, 1).Style.Fill.BackgroundColor = XLColor.Green;
                    worksheet.Cell(index + 1, 2).Style.Fill.BackgroundColor = XLColor.Green;
                    worksheet.Cell(index + 1, 1).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(index + 1, 2).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(index + 1, 3).Style.Fill.BackgroundColor = XLColor.Red;
                    worksheet.Cell(index + 1, 3).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(index + 1, 3).Style.Font.Bold = true;
                    using var stream = new MemoryStream();
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
                catch (Exception ex)
                {
                    return Ok(new string("Error" + ex.ToString()));
                }
            }
            else
            {
                return null;
            }
        }
        // ExportLastMonthKitchenSalesReportToExcel...
        [HttpGet("ExportLastMonthKitchenSalesReportToExcel")]
        [SwaggerOperation(Summary = "Get last month kitchen sales report to excel", OperationId = " Last month kitchen sale export to excel")]
        public async Task<IActionResult> KitchenSalesReportExportInExcelLastMonth()
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                GetAllOrdersRequestDTO model = new GetAllOrdersRequestDTO()
                {
                    From = DateTime.Now.AddDays(-30).Date,
                    To = DateTime.Now.Date
                };
                var responseData = await _reportsRepository.KitchenReport(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"KitchenReportsheet-{DateTime.Now:d/MMMM/yyyy/h/m/s}.xlsx";
                    try
                    {
                        string month = DateTime.Now.ToString("MMMM-yyyy");
                        using var workbook = new XLWorkbook();
                        int ind = 1;
                        int index = 1;
                        decimal? sum = 0;
                        IXLWorksheet worksheet =
                        workbook.Worksheets.Add(month.ToString());
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Value = "Kitchens Sale Report (" + model.From.Date + " - " + model.To.Date + ")";
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                        worksheet.Cell(2, 1).Value = "SrNo.";
                        worksheet.Cell(2, 1).Style.Font.Bold = true;
                        worksheet.Cell(2, 2).Value = "KitchenName";
                        worksheet.Cell(2, 2).Style.Font.Bold = true;
                        worksheet.Cell(2, 3).Value = "Total";
                        worksheet.Cell(2, 3).Style.Font.Bold = true;
                        for (index = 2; index < (responseData.Count + 2); index++, ind++)
                        {
                            worksheet.Cell(index + 1, 1).Value = index - 1;
                            worksheet.Cell(index + 1, 1).Style.Font.Bold = true;
                            worksheet.Cell(index + 1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                            worksheet.Cell(index + 1, 2).Value = responseData[index - 2].KitchenName;
                            worksheet.Cell(index + 1, 3).Value = responseData[index - 2].Total;
                            sum += responseData[index - 2].Total;
                            worksheet.Column(1).AdjustToContents();
                            worksheet.Column(2).AdjustToContents();
                            worksheet.Column(3).AdjustToContents();
                        }
                        worksheet.Cell(index + 1, 3).Value = sum;
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 2)).Merge().Value = "Total Sum";
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 2)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 2)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(index + 1, 1).Style.Fill.BackgroundColor = XLColor.Green;
                        worksheet.Cell(index + 1, 2).Style.Fill.BackgroundColor = XLColor.Green;
                        worksheet.Cell(index + 1, 1).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 2).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 3).Style.Fill.BackgroundColor = XLColor.Red;
                        worksheet.Cell(index + 1, 3).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 3).Style.Font.Bold = true;
                        using var stream = new MemoryStream();
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                    catch (Exception ex)
                    {
                        return Ok(new string("Error" + ex.ToString()));
                    }
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        /// Hall Sale Report
        [HttpPost("HallSalesReport")]
        [SwaggerOperation(Summary = "Get all halls sale report", OperationId = "Halls sale export ")]
        public async Task<IActionResult> HallSalesReportExportToExcel(GetAllOrdersRequestDTO model)
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.HallReport(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }
        //// ExportHallSalesReportToExcel...
        [HttpPost("ExportHallSalesReportToExcel")]
        [SwaggerOperation(Summary = "Get all halls sale report export to excel", OperationId = "All hall report export to excel")]
        public async Task<IActionResult> HallReport(GetAllOrdersRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.HallReport(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"HallReportsheet-{DateTime.Now:d/MMMM/yyyy/h/m/s}.xlsx";
                    try
                    {
                        string month = DateTime.Now.ToString("MMMM-yyyy");
                        using var workbook = new XLWorkbook();
                        int ind = 1;
                        int index = 1;
                        decimal? sum = 0;
                        IXLWorksheet worksheet =
                        workbook.Worksheets.Add(month.ToString());
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Value = "Halls Sale Report (" + model.From.Date + " - " + model.To.Date + ")";
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                        worksheet.Cell(2, 1).Value = "SrNo.";
                        worksheet.Cell(2, 1).Style.Font.Bold = true;
                        worksheet.Cell(2, 2).Value = "HallName";
                        worksheet.Cell(2, 2).Style.Font.Bold = true;
                        worksheet.Cell(2, 3).Value = "Total";
                        worksheet.Cell(2, 3).Style.Font.Bold = true;
                        for (index = 2; index < (responseData.Count + 2); index++, ind++)
                        {
                            worksheet.Cell(index + 1, 1).Value = index - 1;
                            worksheet.Cell(index + 1, 1).Style.Font.Bold = true;
                            worksheet.Cell(index + 1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                            worksheet.Cell(index + 1, 2).Value = responseData[index - 2].HallName;
                            worksheet.Cell(index + 1, 3).Value = responseData[index - 2].Total;
                            sum += responseData[index - 2].Total;
                            worksheet.Column(1).AdjustToContents();
                            worksheet.Column(2).AdjustToContents();
                            worksheet.Column(3).AdjustToContents();
                        }
                        worksheet.Cell(index + 1, 3).Value = sum;
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 2)).Merge().Value = "Total Sum";
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 2)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 2)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(index + 1, 1).Style.Fill.BackgroundColor = XLColor.Green;
                        worksheet.Cell(index + 1, 2).Style.Fill.BackgroundColor = XLColor.Green;
                        worksheet.Cell(index + 1, 1).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 2).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 3).Style.Fill.BackgroundColor = XLColor.Red;
                        worksheet.Cell(index + 1, 3).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 3).Style.Font.Bold = true;
                        //using var stream = new MemoryStream();
                        //workbook.SaveAs(stream);
                        //var content = stream.ToArray();
                        //return File(content, contentType, fileName);
                        using var stream = new MemoryStream();
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                    catch (Exception ex)
                    {
                        return Ok(new string("Error" + ex.ToString()));
                    }
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        //// All OrderTypesReport...
        [HttpPost("AllOrderTypesReport")]
        [SwaggerOperation(Summary = "Get all report by order tyes", OperationId = "All order report")]
        public async Task<IActionResult> OrderTypesReport(GetAllOrdersRequestDTO model)
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.OrderTypesReport(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "PermissionAssignController", "AllOrderTypesReport");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }
        //// ExportAllOrderTypesReportToexcel...
        [HttpPost("ExportAllOrderTypesReportToexcel")]
        [SwaggerOperation(Summary = "export all report by order tyes to excel", OperationId = "export order report to excel")]
        public async Task<IActionResult> ExportAllOrderTypesReportToexcel(GetAllOrdersRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.OrderTypesReport(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"SummaryOfOrerTypeReportsheet-{DateTime.Now:d/MMMM/yyyy/h/m/s}.xlsx";
                    try
                    {
                        string month = DateTime.Now.ToString("MMMM-yyyy");
                        using var workbook = new XLWorkbook();
                        int ind = 1;
                        int index = 1;
                        decimal? sum = 0;
                        IXLWorksheet worksheet =
                        workbook.Worksheets.Add(month.ToString());
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Value = "summary of order type Sale Report (" + model.From.Date + " - " + model.To.Date + ")";
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                        worksheet.Cell(2, 1).Value = "SrNo.";
                        worksheet.Cell(2, 1).Style.Font.Bold = true;
                        worksheet.Cell(2, 2).Value = "OrderType";
                        worksheet.Cell(2, 2).Style.Font.Bold = true;
                        worksheet.Cell(2, 3).Value = "Total";
                        worksheet.Cell(2, 3).Style.Font.Bold = true;
                        for (index = 2; index < (responseData.Count + 2); index++, ind++)
                        {
                            worksheet.Cell(index + 1, 1).Value = index - 1;
                            worksheet.Cell(index + 1, 1).Style.Font.Bold = true;
                            worksheet.Cell(index + 1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                            if (responseData[index - 2].OrderType == (int)OrderType.Online)
                                worksheet.Cell(index + 1, 2).Value = Enum.GetName(typeof(OrderType), 1); /// Online
                            if (responseData[index - 2].OrderType == 2)
                                worksheet.Cell(index + 1, 2).Value = Enum.GetName(typeof(OrderType), 2); // TakeAway
                            if (responseData[index - 2].OrderType == 3)
                                worksheet.Cell(index + 1, 2).Value = Enum.GetName(typeof(OrderType), 3);  // Dinning
                            if (responseData[index - 2].OrderType == 4)
                                worksheet.Cell(index + 1, 2).Value = Enum.GetName(typeof(OrderType), 4);  //DriveThrough
                            if (responseData[index - 2].OrderType == 5)
                                worksheet.Cell(index + 1, 2).Value = Enum.GetName(typeof(OrderType), 5);  /// Delivery
                            worksheet.Cell(index + 1, 3).Value = responseData[index - 2].Total; 
                            sum += responseData[index - 2].Total;
                            worksheet.Column(1).AdjustToContents();
                            worksheet.Column(2).AdjustToContents();
                            worksheet.Column(3).AdjustToContents();
                        }
                        worksheet.Cell(index + 1, 3).Value = sum;
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 2)).Merge().Value = "Total Sum";
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 2)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 2)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(index + 1, 1).Style.Fill.BackgroundColor = XLColor.Green;
                        worksheet.Cell(index + 1, 2).Style.Fill.BackgroundColor = XLColor.Green;
                        worksheet.Cell(index + 1, 1).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 2).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 3).Style.Fill.BackgroundColor = XLColor.Red;
                        worksheet.Cell(index + 1, 3).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 3).Style.Font.Bold = true;
                        using var stream = new MemoryStream();
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                    catch (Exception ex)
                    {
                        return Ok(new string("Error" + ex.ToString()));
                    }
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "ReportsController", "ExportKitchenSalesReportToExcel");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
        //// SpecificOrderTypesReport...
        [HttpPost("SpecificOrderTypesReport")]
        [SwaggerOperation(Summary = "Get all report by specific order tyes", OperationId = "specific order report")]
        public async Task<IActionResult> SpecificOrderTypesReport(GetSpecificOrdersRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.SpecificOrderTypesReport(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        [HttpPost("ExportSpecificOrderTypesReportToExcel")]
        [SwaggerOperation(Summary = "Get all report by specific  order tyes to excel", OperationId = "expotr specific order report to excel")]
        public async Task<IActionResult> ExportSpecificOrderTypesReportToExcel(GetSpecificOrdersRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.SpecificOrderTypesReport(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"SpecificOrderTypesReportsheet-{DateTime.Now:d/MMMM/yyyy/h/m/s}.xlsx";
                    try
                    {
                        string month = DateTime.Now.ToString("MMMM-yyyy");
                        using var workbook = new XLWorkbook();
                        int ind = 1;
                        int index = 1;
                        decimal? sum = 0;
                        IXLWorksheet worksheet =
                        workbook.Worksheets.Add(month.ToString());
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 12)).Merge().Value = "Specific Order Types Report (" + model.From.Date + " - " + model.To.Date + ")";
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 12)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 12)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                        worksheet.Cell(2, 1).Value = "SrNo.";
                        worksheet.Cell(2, 1).Style.Font.Bold = true;
                        worksheet.Cell(2, 2).Value = "HallName";
                        worksheet.Cell(2, 2).Style.Font.Bold = true;
                        worksheet.Cell(2, 3).Value = "TableName";
                        worksheet.Cell(2, 3).Style.Font.Bold = true;
                        worksheet.Cell(2, 4).Value = "Amount";
                        worksheet.Cell(2, 4).Style.Font.Bold = true;
                        worksheet.Cell(2, 5).Value = "Discount";
                        worksheet.Cell(2, 5).Style.Font.Bold = true;
                        worksheet.Cell(2, 6).Value = "ServiceCharges";
                        worksheet.Cell(2, 6).Style.Font.Bold = true;
                        worksheet.Cell(2, 7).Value = "DeliveryCharges";
                        worksheet.Cell(2, 7).Style.Font.Bold = true;
                        worksheet.Cell(2, 8).Value = "Tax";
                        worksheet.Cell(2, 8).Style.Font.Bold = true;
                        worksheet.Cell(2, 9).Value = "TotalAmount";
                        worksheet.Cell(2, 9).Style.Font.Bold = true;
                        worksheet.Cell(2, 10).Value = "OrderDate";
                        worksheet.Cell(2, 10).Style.Font.Bold = true;
                        worksheet.Cell(2, 11).Value = "OrderStatus";
                        worksheet.Cell(2, 11).Style.Font.Bold = true;
                        worksheet.Cell(2, 12).Value = "OrderType";
                        worksheet.Cell(2, 12).Style.Font.Bold = true;
                        for (index = 2; index < (responseData.Count + 2); index++, ind++)
                        {
                            worksheet.Cell(index + 1, 1).Value = index - 1;
                            worksheet.Cell(index + 1, 1).Style.Font.Bold = true;
                            worksheet.Cell(index + 1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                            worksheet.Cell(index + 1, 2).Value = responseData[index - 2].HallName;
                            worksheet.Cell(index + 1, 3).Value = responseData[index - 2].TableName;
                            worksheet.Cell(index + 1, 4).Value = responseData[index - 2].Amount;
                            worksheet.Cell(index + 1, 5).Value = responseData[index - 2].Discount;
                            worksheet.Cell(index + 1, 6).Value = responseData[index - 2].ServiceCharges;
                            worksheet.Cell(index + 1, 7).Value = responseData[index - 2].DeliveryCharges;
                            worksheet.Cell(index + 1, 8).Value = responseData[index - 2].Tax;
                            worksheet.Cell(index + 1, 9).Value = responseData[index - 2].TotalAmount;
                            worksheet.Cell(index + 1, 10).Value = responseData[index - 2].CreatedDate;
                            worksheet.Cell(index + 1, 11).Value = responseData[index - 2].OrderStatus;
                            if (responseData[index - 2].OrderType == 1)
                                worksheet.Cell(index + 1, 12).Value = "Online";
                            if (responseData[index - 2].OrderType == 2)
                                worksheet.Cell(index + 1, 12).Value = "TakeAway";
                            if (responseData[index - 2].OrderType == 3)
                                worksheet.Cell(index + 1, 12).Value = "Dinning";
                            if (responseData[index - 2].OrderType == 4)
                                worksheet.Cell(index + 1, 12).Value = "DriveThrough";
                            if (responseData[index - 2].OrderType == 5)
                                worksheet.Cell(index + 1, 12).Value = "Delivery";
                            sum += responseData[index - 2].TotalAmount;
                            worksheet.Column(1).AdjustToContents();
                            worksheet.Column(2).AdjustToContents();
                            worksheet.Column(3).AdjustToContents();
                            worksheet.Column(4).AdjustToContents();
                            worksheet.Column(5).AdjustToContents();
                            worksheet.Column(6).AdjustToContents();
                            worksheet.Column(7).AdjustToContents();
                            worksheet.Column(8).AdjustToContents();
                            worksheet.Column(9).AdjustToContents();
                            worksheet.Column(10).AdjustToContents();
                            worksheet.Column(12).AdjustToContents();
                            worksheet.Column(13).AdjustToContents();
                        }
                        worksheet.Cell(index + 1, 9).Value = sum;
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 8)).Merge().Value = "Total Sum";
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 8)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 8)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        using var stream = new MemoryStream();
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                    catch (Exception ex)
                    {
                        return Ok(new string("Error" + ex.ToString()));
                    }
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        //Order Item Report 
        [HttpPost("OrderItemReport")]
        [SwaggerOperation(Summary = "Get all report of all order item", OperationId = "order item report")]



        public async Task<IActionResult> OrderitemReport(GetAllOrdersRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.OrderitemReport(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "ReportsController", "OrderItemReports");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        //Order Item Report to excel 
        [HttpPost("ExportOrderItemReportToExcel")]
        [SwaggerOperation(Summary = "Get all report of all order item to excel", OperationId = "export order item report to excel")]
        public async Task<IActionResult> ExportOrderItemReportToExcel(GetAllOrdersRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.OrderitemReport(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"FoodItemReportsheet-{DateTime.Now:d/MMMM/yyyy/h/m/s}.xlsx";
                    try
                    {
                        string month = DateTime.Now.ToString("MMMM-yyyy");
                        using var workbook = new XLWorkbook();
                        int ind = 1;
                        int index = 1;
                        decimal? sum = 0;
                        IXLWorksheet worksheet =
                        workbook.Worksheets.Add(month.ToString());
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Value = "Food Item Sale Report (" + model.From.Date + " - " + model.To.Date + ")";
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 1)).Merge().Value = "SrNo.";
                        worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(2, 1)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(2, 4)).Merge().Value = "ItemName";
                        worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(2, 4)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(2, 5), worksheet.Cell(2, 6)).Merge().Value = "VarientName";
                        worksheet.Range(worksheet.Cell(2, 5), worksheet.Cell(2, 6)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(2, 7), worksheet.Cell(2, 8)).Merge().Value = "Total";
                        worksheet.Range(worksheet.Cell(2, 7), worksheet.Cell(2, 8)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(2, 7), worksheet.Cell(2, 8)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        for (index = 2; index < (responseData.Count + 2); index++, ind++)
                        {
                            worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 1)).Merge().Value = index - 1;
                            worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 1)).Merge().Style.Font.Bold = true;
                            worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 1)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Range(worksheet.Cell(index + 1, 2), worksheet.Cell(index + 1, 4)).Merge().Value = responseData[index - 2].ItemName;
                            worksheet.Range(worksheet.Cell(index + 1, 5), worksheet.Cell(index + 1, 6)).Merge().Value = responseData[index - 2].VariantName;
                            worksheet.Range(worksheet.Cell(index + 1, 7), worksheet.Cell(index + 1, 8)).Merge().Value = responseData[index - 2].Total;
                            worksheet.Range(worksheet.Cell(index + 1, 7), worksheet.Cell(index + 1, 8)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            sum += responseData[index - 2].Total;
                            worksheet.Column(1).AdjustToContents();
                            worksheet.Column(2).AdjustToContents();
                            worksheet.Column(3).AdjustToContents();
                            worksheet.Column(4).AdjustToContents();
                            worksheet.Column(5).AdjustToContents();
                            worksheet.Column(6).AdjustToContents();
                            worksheet.Column(7).AdjustToContents();
                            worksheet.Column(8).AdjustToContents();
                        }
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 6)).Merge().Value = "Total Sum";
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 6)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(index + 1, 1), worksheet.Cell(index + 1, 6)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Range(worksheet.Cell(index + 1, 7), worksheet.Cell(index + 1, 8)).Merge().Value = sum;
                        worksheet.Range(worksheet.Cell(index + 1, 7), worksheet.Cell(index + 1, 8)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(index + 1, 7), worksheet.Cell(index + 1, 8)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(index + 1, 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        worksheet.Cell(index + 1, 2).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        worksheet.Cell(index + 1, 1).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 2).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(index + 1, 7).Style.Fill.BackgroundColor = XLColor.LightPink;
                        worksheet.Cell(index + 1, 8).Style.Font.FontColor = XLColor.White;
                        using var stream = new MemoryStream();
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                    catch (Exception ex)
                    {
                        return Ok(new string("Error" + ex.ToString()));
                    }
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "ReportsController", "ExportOrderItemReportToExcel");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
        //// ChargesOnOrdersReport...
        [HttpPost("ChargesOnOrdersReport")]
        [SwaggerOperation(Summary = "Get all extra charges on all orders", OperationId = "extra charges on order")]
        public async Task<IActionResult> ChargesOnOrdersReport(GetAllOrdersRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.ChargesOnOrdersReport(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "ReportsController", "ChargesOnOrdersReport");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
        ////ExportChargesOnOrdersReportToExcel...
        [HttpPost("ExportChargesOnOrdersReportToExcel")]
        [SwaggerOperation(Summary = "Export all extra charges on all orders to excel", OperationId = " export all extra charges on orders to excel")]
        public async Task<IActionResult> ExportChargesOnOrdersReportToExcel(GetAllOrdersRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _reportsRepository.ChargesOnOrdersReport(model);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"EtrtachargesReportsheet-{DateTime.Now:d/MMMM/yyyy/h/m/s}.xlsx";
                    try
                    {
                        string month = DateTime.Now.ToString("MMMM-yyyy");
                        using var workbook = new XLWorkbook();
                        IXLWorksheet worksheet =
                        workbook.Worksheets.Add(month.ToString());
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Value = "Halls Sale Report (" + model.From.Date + " - " + model.To.Date + ")";
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Style.Font.Bold = true;
                        worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 8)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center); ;
                        worksheet.Cell(2, 1).Value = "SrNo.";
                        worksheet.Cell(2, 1).Style.Font.Bold = true;
                        worksheet.Cell(2, 2).Value = "TotalServiceCharges";
                        worksheet.Cell(2, 2).Style.Font.Bold = true;
                        worksheet.Cell(2, 3).Value = "TotalDeliveryCharges";
                        worksheet.Cell(2, 3).Style.Font.Bold = true;
                        worksheet.Cell(2, 4).Value = "TotalTax";
                        worksheet.Cell(2, 4).Style.Font.Bold = true;
                        worksheet.Cell(3, 1).Value = 1;
                        worksheet.Cell(3, 1).Style.Font.Bold = true;
                        worksheet.Cell(3, 2).Value = responseData.TotalDeliveryCharges;
                        worksheet.Cell(3, 3).Value = responseData.TotalServiceCharges;
                        worksheet.Cell(3, 4).Value = responseData.TotalTax;
                        worksheet.Column(1).AdjustToContents();
                        worksheet.Column(2).AdjustToContents();
                        worksheet.Column(3).AdjustToContents();
                        using var stream = new MemoryStream();
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                    catch (Exception ex)
                    {
                        return Ok(new string("Error" + ex.ToString()));
                    }
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "ReportsController", "ExportChargesOnOrdersReportToExcel");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
    }
}