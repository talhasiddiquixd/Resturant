using System.Collections.Generic;
namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class DashBoardResponseDTO
    {
        public int UserCount { get; set; }
        public int OnGoingOrderCount { get; set; }
        public decimal TodaysIncome { get; set; }
        public decimal LastWeekIncome { get; set; }
        public decimal LastMonthIncome { get; set; }
        public List<WeekelyIncomeReport> WeeklyIncomes { get; set; }
    }
    public class WeekelyIncomeReport
    {
        public string Date { get; set; }
        public string Day { get; set; }
        public decimal TotalIncome { get; set; }
    }
}