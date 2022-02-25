using System;
using System.Linq;
using RestaurantPOS.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Repository;
using System.Collections.Generic;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using RestaurantPOS.Models;
using Microsoft.EntityFrameworkCore;

namespace RestaurantPOS.Controllers
{
    //[EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DashboardController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IReportsRepository _reportsRepository;
        private readonly IunitOfWork _unitofWork;
        public DashboardController(IunitOfWork unitofWork, IReportsRepository reportsRepository, IOrderRepository orderRepository, IUserRepository userRepository, IConfiguration config, IErrorLogRepository errorLogRepository)
        {
            _config = config;
            _unitofWork = unitofWork;
            _errorLogRepository = errorLogRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _reportsRepository = reportsRepository;
        }


        
        
        // Get data for Admin Dashboard 
        [HttpGet("Dashboard")]
        [SwaggerOperation(Summary = "Data for Dashboard", OperationId = "Get data for dashboard")]
        public async Task<IActionResult> GetAllUpdated()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                long UsersCount = await _userRepository.GetUsersCount();
                GetAllOrdersRequestDTO getAllOrdersRequestDTO = new GetAllOrdersRequestDTO()
                {
                    From = DateTime.Now.AddDays(-30),
                    To = DateTime.Now.Date
                };
                var prevThirtyDaysOrders = await _reportsRepository.GetAllOrdersForDashboard(getAllOrdersRequestDTO);
                decimal TodaysSum = prevThirtyDaysOrders.Where(x => x.CreatedDate.Date == DateTime.Now.Date && x.OrderStatus == "Paid").Sum(x => x.TotalAmount);
                List<WeekelyIncomeReport> weekelyIncome = new List<WeekelyIncomeReport>();
                for (int i = 7; i >= 1; i--)
                {
                    weekelyIncome.Add(new WeekelyIncomeReport()
                    {
                        Date = DateTime.Now.AddDays(-i).ToString("dd/MMM/yyyy"),
                        Day = DateTime.Now.AddDays(-i).DayOfWeek.ToString(),
                        TotalIncome = prevThirtyDaysOrders.Where(x => x.CreatedDate.Date == DateTime.Now.AddDays(-i).Date && x.OrderStatus == "Paid").Sum(x => x.TotalAmount)
                    });
                }
              
                DashBoardResponseDTO dashBoardResponseDTO = new DashBoardResponseDTO()
                {
                    UserCount = (int)UsersCount,
                    OnGoingOrderCount = prevThirtyDaysOrders.Where(x => x.OrderStatus != "Paid" && x.OrderStatus != "Rejected" && x.OrderStatus != "Delivered" && x.OrderStatus != "Completed").Count(),
                    TodaysIncome = TodaysSum,
                    LastWeekIncome = weekelyIncome.Sum(x=> x.TotalIncome),
                    LastMonthIncome = prevThirtyDaysOrders.Where(x => x.OrderStatus == "Paid").Sum(x => x.TotalAmount),
                    WeeklyIncomes = weekelyIncome
                };
                if (dashBoardResponseDTO != null)
                {
                    response.Data = dashBoardResponseDTO;
                    response.TotalRecords = 1;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "DashboardController", "Getsettings");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        //[HttpGet("DashboardThisWeek")]
        //[SwaggerOperation(Summary = "Last week earning", OperationId = "Get last week income for dashboard")]
        //public async Task<IActionResult> GetDataForThisWeek()
        //{
        //    string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
        //    ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
        //    try
        //    {
        //        GetAllOrdersRequestDTO getAllOrdersRequestDTO = new GetAllOrdersRequestDTO()
        //        {
        //            From = DateTime.Now.Date,
        //            To = DateTime.Now.Date
        //        };
        //        var TodayOrder = await _reportsRepository.GetAllOrders(getAllOrdersRequestDTO);
        //        decimal sum = 0;
        //        foreach (var item in TodayOrder)
        //        {
        //            if (item.OrderStatus == "Paid")
        //                sum += item.TotalAmount;
        //        }
        //        List<WeekelyIncomeReport> weekelyIncome = new List<WeekelyIncomeReport>();
        //        for (int i = 7; i >= 1; i--)
        //        {
        //            decimal weekelysum = 0;
        //            GetAllOrdersRequestDTO getAllOrdersRequestDTOnew = new GetAllOrdersRequestDTO()
        //            {
        //                From = DateTime.Now.AddDays(-i),
        //                To = DateTime.Now.AddDays(-i)
        //            };
        //            var lastWeekOrders = await _reportsRepository.GetAllOrders(getAllOrdersRequestDTOnew);
        //            foreach (var item in lastWeekOrders)
        //            {
        //                if (item.OrderStatus == "Paid")
        //                    weekelysum += item.TotalAmount;
        //            }
        //            weekelyIncome.Add(new WeekelyIncomeReport()
        //            {
        //                Date = getAllOrdersRequestDTOnew.From.ToString("dd/MMM/yyyy"),
        //                Day = getAllOrdersRequestDTOnew.From.DayOfWeek.ToString(),
        //                TotalIncome = weekelysum,
        //            });
        //        }
        //        decimal LastWeekIncome = 0;
        //        foreach (var income in weekelyIncome)
        //        {
        //            LastWeekIncome += income.TotalIncome;
        //        }

        //        DashBoardResponseDTO dashBoardResponseDTO = new DashBoardResponseDTO()
        //        {
        //            //UserCount = TotalUsers,
        //            //Users = users.ToList(),
        //            // OnGoingOrderCount = OngoingOrderscount,
        //            TodaysIncome = sum,
        //            LastWeekIncome = LastWeekIncome,
        //            //LastMonthIncome = monthlysum,
        //            WeeklyIncomes = weekelyIncome
        //        };
        //        if (dashBoardResponseDTO != null)
        //        {
        //            response.Data = dashBoardResponseDTO;
        //            response.TotalRecords = 1;
        //            response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
        //        }
        //        else
        //        {
        //            response.Message = ResponseMessageHelper.NoRecordFound;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = 500;
        //        response.Message = "Critical Error: " + ex.Message;
        //        response.ExceptionMessage = ex.Message.ToString();
        //        new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "DashboardController", "Getsettings");
        //    }
        //    return ResponseHelper<object>.GenerateResponse(response);
        //}

        //private 
    }
}
