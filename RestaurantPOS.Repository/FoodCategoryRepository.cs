using System;
using LinqKit;
using AutoMapper;
using System.Linq;
using RestaurantPOS.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
namespace RestaurantPOS.Repository
{       
    public class FoodCategoryRepository : IFoodCategoryRepository
    {
        private readonly IunitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FoodCategoryRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<FoodCategory>> GetALLSync()
        {
            var dataList = await _unitOfWork.Context.FoodCategory.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<IEnumerable<FoodCategoryReponseDTO>> GetAll()
        {
            var FoodCategoryList = await _unitOfWork.Context.FoodCategory.Where(x => x.IsDeleted == false).Include(x => x.FoodCategoryOffer).Include(x => x.FoodItem).ThenInclude(x => x.FoodVarient).ToListAsync();
            if (FoodCategoryList != null && FoodCategoryList.Count > 0)
            {
                var FoodCategoryResponseList = _mapper.Map<List<FoodCategoryReponseDTO>>(FoodCategoryList);
                if (FoodCategoryResponseList!=null)
                {
                    foreach (var item in FoodCategoryResponseList)
                    {
                        foreach (var food in item.FoodItem)
                        {
                            if (food.FoodVarient != null)
                            {
                                foreach (var varient in food.FoodVarient)
                                {
                                    if (varient.FoodItemId > 0)
                                    {
                                        var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == varient.FoodItemId).SingleOrDefaultAsync();
                                        if (FoodItem != null)
                                        {
                                            varient.FoodItemName = FoodItem.Name;
                                            varient.Quantity = 1;
                                            varient.CookingTime = FoodItem.CookingTime;
                                            varient.Total = varient.Price;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return FoodCategoryResponseList; 
                }
            }
            return null;
        }
        public async Task<FoodCategoryReponseDTO> GetById(int Id, string Path)
        {
            var FoodCategory = await _unitOfWork.Context.FoodCategory.Where(x => x.Id == Id && x.IsDeleted == false).Include(x => x.FoodCategoryOffer).Include(x => x.FoodItem).ThenInclude(x => x.FoodVarient).SingleOrDefaultAsync();
            if (FoodCategory != null)
            {
                var FoodCategoryResponse = _mapper.Map<FoodCategoryReponseDTO>(FoodCategory);
                if (FoodCategoryResponse != null)
                {
                    foreach (var item in FoodCategoryResponse.FoodItem)
                    {
                        if (item.FoodVarient != null)
                        {
                            foreach (var varient in item.FoodVarient)
                            {
                                List<SearchFoodItemResponseDTO> searchresponse = new List<SearchFoodItemResponseDTO>();
                                if (varient.FoodItemId > 0)
                                {
                                    var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == varient.FoodItemId).SingleOrDefaultAsync();
                                    if (FoodItem != null)
                                    {
                                        varient.FoodItemName = FoodItem.Name;
                                        varient.Quantity = 1;
                                        varient.CookingTime = FoodItem.CookingTime;
                                        varient.Total = varient.Price;
                                    }
                                }
                            }
                        }
                    }
                }
                return FoodCategoryResponse;
            }
            return null;
        }
        //public async Task<FoodCategoryReponseDTO> GetById(int Id, string Path)
        //{
        //    var FoodCategory = await _unitOfWork.Context.FoodCategory.Where(x => x.Id == Id && x.IsDeleted == false).SingleOrDefaultAsync();
        //    if (FoodCategory != null)
        //    {
        //        var FoodCategoryResponse = _mapper.Map<FoodCategoryReponseDTO>(FoodCategory);
        //        if (FoodCategoryResponse != null)
        //        {
        //            List<FoodItem> searchItem = new List<FoodItem>();
        //            List<SearchFoodItemResponseDTO> searchresponse = new List<SearchFoodItemResponseDTO>();
        //            searchItem = await _unitOfWork.Context.FoodItem.Where(x=> x.FoodCategoryId==Id).Include(x=>x.FoodItemOffer).ToListAsync();
        //            var FoodItemResponseList = _mapper.Map<List<SearchFoodItemResponseDTO>>(searchItem);
        //            if (FoodItemResponseList!=null)
        //            {
        //                foreach (var item in FoodItemResponseList)
        //                {
        //                    var foodvarient = await _unitOfWork.Context.FoodVarient.Where(x => x.FoodItemId == item.Id).ToListAsync();
        //                    foreach (var varient in foodvarient)
        //                    {
        //                        SearchFoodItemResponseDTO searchFoodItemResponseDTO = new SearchFoodItemResponseDTO()
        //                        {
        //                            Name = item.Name,
        //                            AttachmentId = item.AttachmentId,
        //                            CookingTime = item.CookingTime,
        //                            FoodCategoryId = item.FoodCategoryId,
        //                            Id = item.Id,
        //                            IsOffer = item.IsOffer,
        //                            KitchenId = item.KitchenId,
        //                            ItemPrice = item.ItemPrice,
        //                            VAT = item.VAT,
        //                            Total = item.Total,
        //                            Description = item.Description,
        //                            IsSpecial = item.IsSpecial,
        //                            Quantity = item.Quantity,
        //                            Status = item.Status,
        //                            Note = item.Note,
        //                            FoodCategoryName = item.FoodCategoryName
        //                        };
        //                        searchFoodItemResponseDTO.Name = searchFoodItemResponseDTO.Name + " " + varient.Name;
        //                        //var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == varient.FoodItemId).SingleOrDefaultAsync();
        //                        searchFoodItemResponseDTO.FoodVarient = _mapper.Map<FoodVarientResponseDTO>(varient);
        //                        var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == searchFoodItemResponseDTO.FoodVarient.FoodItemId).SingleOrDefaultAsync();
        //                        if (FoodItem != null)
        //                        {
        //                            searchFoodItemResponseDTO.FoodVarient.FoodItemName = FoodItem.Name;
        //                            searchFoodItemResponseDTO.FoodVarient.Quantity = 1;
        //                            searchFoodItemResponseDTO.FoodVarient.Total = varient.Price;
        //                        }
        //                        searchresponse.Add(searchFoodItemResponseDTO);
        //                    }
        //                } 
        //            }
        //            FoodCategoryResponse.searchresponse = searchresponse;
        //            return FoodCategoryResponse;
        //        }
        //    }
        //    return null;
        //}
        public async Task<long> IsExist(string CategoryName)
        {
            var ExistCategoryName = await _unitOfWork.Context.FoodCategory.Where(x => x.Name==CategoryName && x.IsDeleted == false).FirstOrDefaultAsync();
            if (ExistCategoryName == null)
                return 0;
            return 1;
        }
        public async Task<long> Save(FoodCategoryRequestDTO model, int Id)
        {
            FoodCategory foodCategory = new FoodCategory();
            foodCategory = _mapper.Map<FoodCategory>(model);
            foodCategory.IsSynchronized = false;
            if (model.Id <= 0)
            {
                var data = _unitOfWork.Context.FoodCategory.SingleOrDefault(x => x.Name.Equals(model.Name)&& x.IsDeleted==false);
                if (data == null)
                {
                    foodCategory.IsDeleted = false;
                    foodCategory.CreatedBy = Id;
                    foodCategory.CreatedAt = DateTime.UtcNow;
                    foodCategory.UpdatedBy = null;
                    foodCategory.UpdatedAt = null;
                    _unitOfWork.Context.Set<FoodCategory>().Add(foodCategory);
                    await _unitOfWork.Commit();
                }
            }
            else
            {
                var PriviousRecord = _unitOfWork.Context.FoodCategory.FirstOrDefault(x => x.Id == model.Id);
                foodCategory.IsOffer = model.IsOffer;
                foodCategory.UpdatedBy = Id;
                foodCategory.UpdatedAt = DateTime.UtcNow;
                foodCategory.CreatedBy = PriviousRecord.CreatedBy;
                foodCategory.CreatedAt = PriviousRecord.CreatedAt;
                _unitOfWork.Context.Entry(foodCategory).State = EntityState.Modified;
                _unitOfWork.Context.FoodCategory.Update(foodCategory);
                var response = await _unitOfWork.Commit();
            }
            return foodCategory.Id;
        }
        public async Task<long> Delete(int Id, int LoginPersonId)
        {
            FoodCategory foodDelete = _unitOfWork.Context.FoodCategory.Where(x => x.Id.Equals(Id) && x.IsDeleted == false).FirstOrDefault();
            if (foodDelete != null)
            {
                foodDelete.IsDeleted = true;
                foodDelete.UpdatedBy = LoginPersonId;
                foodDelete.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Context.Entry(foodDelete).State = EntityState.Modified;
                _unitOfWork.Context.FoodCategory.Update(foodDelete);
                return await _unitOfWork.Commit();
            }
            return 0;
        }
        public async Task<(IEnumerable<FoodCategoryReponseDTO> FoodCategoryList, int recordsCount)> GetByFilters(SearchFilter filters)
        {
            var predicate = PredicateBuilder.New<FoodCategory>(true);
            bool isPradicate = false;
            List<FoodCategory> listData = new List<FoodCategory>();
            predicate = predicate.And(i => i.IsDeleted == false);
            if (!string.IsNullOrEmpty(filters.SearchText))
            {
                predicate = predicate.Or(i => i.Name.Contains(filters.SearchText));
                predicate = predicate.Or(i => i.Description.Contains(filters.SearchText));
                isPradicate = true;
            }
            if (filters.Filters != null)
                foreach (var filter in filters.Filters)
                {
                    if (filter.Logic == "and")
                        predicate.And(DBHelper.BuildPredicate<FoodCategory>(filter.Name, filter.Operator, filter.Value));
                    else if (filter.Logic == "or")
                        predicate.Or(DBHelper.BuildPredicate<FoodCategory>(filter.Name, filter.Operator, filter.Value));
                    else
                        predicate.And(DBHelper.BuildPredicate<FoodCategory>(filter.Name, filter.Operator, filter.Value));
                    isPradicate = true;
                }
            if (isPradicate)
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitOfWork.Context.FoodCategory.Where(predicate).Skip(filters.Offset * filters.PageSize).Take(filters.PageSize).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            else
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitOfWork.Context.FoodCategory.Skip(filters.Offset * filters.PageSize).Take(filters.Offset).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            var recordsCount = await _unitOfWork.Context.User.Where(x => x.IsDeleted == false).CountAsync();

            (List<FoodCategoryReponseDTO> FoodCategoryList, int recordsCount) tuple = (_mapper.Map<List<FoodCategoryReponseDTO>>(listData), recordsCount);
            return tuple;
        }
    }

    public interface IFoodCategoryRepository
    {
        Task<IEnumerable<FoodCategoryReponseDTO>> GetAll();
        Task<FoodCategoryReponseDTO> GetById(int Id, string Path);
        Task<long> IsExist(string CategoryName);
        Task<long> Save(FoodCategoryRequestDTO model, int Id);
        Task<long> Delete(int Id, int LoginPerson);
        Task<(IEnumerable<FoodCategoryReponseDTO> FoodCategoryList, int recordsCount)> GetByFilters(SearchFilter filters);
        Task<List<FoodCategory>> GetALLSync();
    }
}