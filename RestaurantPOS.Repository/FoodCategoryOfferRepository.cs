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
    public class FoodCategoryOfferRepository : IFoodCategoryOfferRepository
    {
        private readonly IunitOfWork _unitofWork;
        private readonly IMapper _mapper;

        public FoodCategoryOfferRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitofWork = unitOfWork;
        }
        public async Task<List<FoodCategoryOffer>> GetALLSync()
        {
            var dataList = await _unitofWork.Context.FoodCategoryOffer.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<IEnumerable<FoodCategoryOfferResponseDTO>> GetAll()
        {
            var response = await _unitofWork.Context.FoodCategoryOffer.Where(x => x.IsActive == true).ToListAsync();
            return _mapper.Map<List<FoodCategoryOfferResponseDTO>>(response);
        }
        public async Task<FoodCategoryOfferResponseDTO> GetById(int Id)
        {
            var response = await _unitofWork.Context.FoodCategoryOffer.Where(x => x.Id == Id && x.IsActive == true).SingleOrDefaultAsync();
            return _mapper.Map<FoodCategoryOfferResponseDTO>(response);
        }
        public async Task<long> Save(FoodCategoryOfferRequestDTO model)
        {
            FoodCategoryOffer foodCategoryOffer = new FoodCategoryOffer();
            foodCategoryOffer = _mapper.Map<FoodCategoryOffer>(model);
            foodCategoryOffer.IsSynchronized = false;
            if (model.Id <= 0)
            {
                _unitofWork.Context.Set<FoodCategoryOffer>().Add(foodCategoryOffer);
            }
            else
            {
                var PriviousRecord = _unitofWork.Context.User.FirstOrDefault(x => x.Id == model.Id);
                _unitofWork.Context.Entry(foodCategoryOffer).State = EntityState.Modified;
                _unitofWork.Context.FoodCategoryOffer.Update(foodCategoryOffer);
            }
            return await _unitofWork.Commit();
        }
        public async Task<long> InActive(int Id, bool Status)
        {
            FoodCategoryOffer foodOfferActive = _unitofWork.Context.FoodCategoryOffer.Where(x => x.Id.Equals(Id) && x.IsActive == true).FirstOrDefault();
            if (foodOfferActive != null)
            {
                foodOfferActive.IsActive = false;
                _unitofWork.Context.Entry(foodOfferActive).State = EntityState.Modified;
                _unitofWork.Context.FoodCategoryOffer.Update(foodOfferActive);
                return await _unitofWork.Commit();
            }
            return 0;
        }
        public async Task<(IEnumerable<FoodCategoryOfferResponseDTO> FoodCategoryOfferList, int recordsCount)> GetByFilters(SearchFilter filters)
        {
            List<FoodCategoryOffer> listData = new List<FoodCategoryOffer>();

            var predicate = PredicateBuilder.New<FoodCategoryOffer>(true);
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
                        predicate.And(DBHelper.BuildPredicate<FoodCategoryOffer>(filter.Name, filter.Operator, filter.Value));
                    else if (filter.Logic == "or")
                        predicate.Or(DBHelper.BuildPredicate<FoodCategoryOffer>(filter.Name, filter.Operator, filter.Value));
                    else
                        predicate.And(DBHelper.BuildPredicate<FoodCategoryOffer>(filter.Name, filter.Operator, filter.Value));
                    isPradicate = true;
                }
            if (isPradicate)
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitofWork.Context.FoodCategoryOffer.Where(predicate).Skip(filters.Offset * filters.PageSize).Take(filters.PageSize).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            else
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitofWork.Context.FoodCategoryOffer.Skip(filters.Offset * filters.PageSize).Take(filters.Offset).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            var recordsCount = await _unitofWork.Context.User.Where(x => x.IsDeleted == false).CountAsync();

            (List<FoodCategoryOfferResponseDTO> FoodCategoryOfferList, int recordsCount) tuple = (_mapper.Map<List<FoodCategoryOfferResponseDTO>>(listData), recordsCount);
            return tuple;
        }
    }

    public interface IFoodCategoryOfferRepository
    {
        Task<IEnumerable<FoodCategoryOfferResponseDTO>> GetAll();
        Task<FoodCategoryOfferResponseDTO> GetById(int Id);
        Task<long> Save(FoodCategoryOfferRequestDTO model);
        Task<long> InActive(int Id, bool Status);
        Task<(IEnumerable<FoodCategoryOfferResponseDTO> FoodCategoryOfferList, int recordsCount)> GetByFilters(SearchFilter filters);
        Task<List<FoodCategoryOffer>> GetALLSync();
    }
}