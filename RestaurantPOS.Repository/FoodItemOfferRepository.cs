using AutoMapper;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using RestaurantPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantPOS.Repository
{
    public class FoodItemOfferRepository : IFoodItemOfferRepository
    {
        private readonly IunitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FoodItemOfferRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<FoodItemOffer>> GetALLSync()
        {
            var dataList = await _unitOfWork.Context.FoodItemOffer.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<IEnumerable<FoodItemOfferResponseDTO>> GetAll()
        {
            var response = await _unitOfWork.Context.FoodItemOffer.Where(x => x.IsActive == true).ToListAsync();
            return _mapper.Map<List<FoodItemOfferResponseDTO>>(response);
        }
        public async Task<FoodItemOfferResponseDTO> GetById(int Id)
        {
            var response = await _unitOfWork.Context.FoodItemOffer.Where(x => x.Id == Id && x.IsActive == true).SingleOrDefaultAsync();
            return _mapper.Map<FoodItemOfferResponseDTO>(response);
        }
        public async Task<long> Save(FoodItemOfferRequestDTO model)
        {
            FoodItemOffer FoodItemOffer = new FoodItemOffer();
            FoodItemOffer = _mapper.Map<FoodItemOffer>(model);
            FoodItemOffer.IsSynchronized = false;
            if (model.Id <= 0)
            {
                _unitOfWork.Context.Set<FoodItemOffer>().Add(FoodItemOffer);
            }
            else
            {
                var PriviousRecord = _unitOfWork.Context.User.FirstOrDefault(x => x.Id == model.Id);
                _unitOfWork.Context.Entry(FoodItemOffer).State = EntityState.Modified;
                _unitOfWork.Context.FoodItemOffer.Update(FoodItemOffer);
            }
            return await _unitOfWork.Commit();
        }
        public async Task<long> InActive(int Id, bool Status)
        {
            FoodItemOffer foodOfferActive = _unitOfWork.Context.FoodItemOffer.Where(x => x.Id.Equals(Id) && x.IsActive == true).FirstOrDefault();
            if (foodOfferActive != null)
            {
                foodOfferActive.IsActive = false;
                _unitOfWork.Context.Entry(foodOfferActive).State = EntityState.Modified;
                _unitOfWork.Context.FoodItemOffer.Update(foodOfferActive);
                return await _unitOfWork.Commit();
            }
            return 0;
        }
        public async Task<(IEnumerable<FoodItemOfferResponseDTO> FoodItemOfferList, int recordsCount)> GetByFilters(SearchFilter filters)
        {
            List<FoodItemOffer> listData = new List<FoodItemOffer>();

            var predicate = PredicateBuilder.New<FoodItemOffer>(true);
            bool isPradicate = false;

            predicate = predicate.And(i => i.IsActive == false);
            if (!string.IsNullOrEmpty(filters.SearchText))
            {
                predicate = predicate.Or(i => i.OfferName.Contains(filters.SearchText));
                isPradicate = true;
            }
            if (filters.Filters != null)
                foreach (var filter in filters.Filters)
                {
                    if (filter.Logic == "and")
                        predicate.And(DBHelper.BuildPredicate<FoodItemOffer>(filter.Name, filter.Operator, filter.Value));
                    else if (filter.Logic == "or")
                        predicate.Or(DBHelper.BuildPredicate<FoodItemOffer>(filter.Name, filter.Operator, filter.Value));
                    else
                        predicate.And(DBHelper.BuildPredicate<FoodItemOffer>(filter.Name, filter.Operator, filter.Value));
                    isPradicate = true;
                }
            if (isPradicate)
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitOfWork.Context.FoodItemOffer.Where(predicate).Skip(filters.Offset * filters.PageSize).Take(filters.PageSize).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            else
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitOfWork.Context.FoodItemOffer.Skip(filters.Offset * filters.PageSize).Take(filters.Offset).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            var recordsCount = await _unitOfWork.Context.User.Where(x => x.IsDeleted == false).CountAsync();

            (List<FoodItemOfferResponseDTO> FoodItemOfferList, int recordsCount) tuple = (_mapper.Map<List<FoodItemOfferResponseDTO>>(listData), recordsCount);
            return tuple;
        }
    }

    public interface IFoodItemOfferRepository
    {
        Task<IEnumerable<FoodItemOfferResponseDTO>> GetAll();
        Task<FoodItemOfferResponseDTO> GetById(int Id);
        Task<long> Save(FoodItemOfferRequestDTO model);
        Task<long> InActive(int Id, bool Status);
        Task<(IEnumerable<FoodItemOfferResponseDTO> FoodItemOfferList, int recordsCount)> GetByFilters(SearchFilter filters);
        Task<List<FoodItemOffer>> GetALLSync();
    }
}