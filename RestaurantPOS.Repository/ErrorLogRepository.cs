using AutoMapper;
using LinqKit;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Repository
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly IunitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ErrorLogRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<ErrorLog>> GetALLSync()
        {
            var dataList = await _unitOfWork.Context.ErrorLog.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<IEnumerable<ErrorLog>> Get()
        {
            var list = await _unitOfWork.Context.ErrorLog.OrderByDynamic("LogId", DBHelper.Order.Desc).ToListAsync();
            return list.ToList();
        }
        public async Task<(IEnumerable<ErrorLog> ErrorLogesList, int recordsCount)> GetByFilters(SearchFilter filters)
        {
            IEnumerable<ErrorLog> listData = null;

            var predicate = PredicateBuilder.New<ErrorLog>(true);
            bool isPradicate = false;

            if (!string.IsNullOrEmpty(filters.SearchText))
            {
                predicate = predicate.And(i => i.LogMessage.Contains(filters.SearchText));
                isPradicate = true;
            }
            if (filters.Filters != null)
                foreach (var filter in filters?.Filters)
                {
                    if (filter.Logic == "and")
                        predicate.And(DBHelper.BuildPredicate<ErrorLog>(filter.Name, filter.Operator, filter.Value));
                    else if (filter.Logic == "or")
                        predicate.Or(DBHelper.BuildPredicate<ErrorLog>(filter.Name, filter.Operator, filter.Value));
                    else
                        predicate.And(DBHelper.BuildPredicate<ErrorLog>(filter.Name, filter.Operator, filter.Value));
                    isPradicate = true;
                }
            if (isPradicate)
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                var sortBy = (filters.sortFilter == null || String.IsNullOrEmpty(filters.sortFilter.SortBy)) ? "LogId" : filters.sortFilter.SortBy;
                if (filters.Offset == 0 && filters.PageSize == 0)
                {
                    listData = await _unitOfWork.Context.ErrorLog.OrderByDynamic(sortBy, direction).ToListAsync();
                }
                else
                    listData = await _unitOfWork.Context.ErrorLog.Where(predicate).OrderByDynamic(sortBy, direction).Skip(filters.Offset * filters.PageSize).Take(filters.PageSize).ToListAsync();
            }
            else
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                var sortBy = (filters.sortFilter == null || String.IsNullOrEmpty(filters.sortFilter.SortBy)) ? "LogId" : filters.sortFilter.SortBy;
                if (filters.Offset == 0 && filters.PageSize == 0)
                {
                    listData = await _unitOfWork.Context.ErrorLog.OrderByDynamic(sortBy, direction).ToListAsync();
                }
                else
                    listData = await _unitOfWork.Context.ErrorLog.OrderByDynamic(sortBy, direction).Skip(filters.Offset * filters.PageSize).Take(filters.PageSize).ToListAsync();
            }
            var recordsCount = await _unitOfWork.Context.ErrorLog.CountAsync();

            (IEnumerable<ErrorLog> ErrorLogesList, int recordsCount) tuple = (listData, recordsCount);
            return tuple;
        }
        public async Task<long> Save(ErrorLogRequestDTO model)
        {
            ErrorLog errorLog = _mapper.Map<ErrorLog>(model);
            errorLog.IsSynchronized = false;
            await _unitOfWork.Context.Set<ErrorLog>().AddAsync(errorLog);
            _unitOfWork.Context.SaveChanges();
            return model.Id;
        }
    }
    public interface IErrorLogRepository
    {
        Task<IEnumerable<ErrorLog>> Get();
        Task<(IEnumerable<ErrorLog> ErrorLogesList, int recordsCount)> GetByFilters(SearchFilter filters);
        Task<long> Save(ErrorLogRequestDTO errorLog);

        Task<List<ErrorLog>> GetALLSync();
    }
}
