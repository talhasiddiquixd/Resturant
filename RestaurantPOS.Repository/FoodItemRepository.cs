using LinqKit;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using RestaurantPOS.Models;
using System.Collections.Generic;
using RestaurantPOS.Helpers.ResponseDTO;
using RestaurantPOS.Helpers.RequestDTO;
using Microsoft.EntityFrameworkCore;
namespace RestaurantPOS.Repository
{
    public class FoodItemRepository : IFoodItemRepository
    {
        private readonly IunitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FoodItemRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<FoodItem>> GetALLSync()
        {
            var dataList = await _unitOfWork.Context.FoodItem.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<IEnumerable<FoodItemResponseDTO>> GetAll(string Path)
        {
            var response = await _unitOfWork.Context.FoodItem.Include(x => x.FoodItemOffer).Where(x => x.Status == false || x.Status == true).Include(x => x.FoodVarient).ToListAsync();
            var FoodItemResponseList = _mapper.Map<List<FoodItemResponseDTO>>(response);
            if (FoodItemResponseList != null)
            {
                foreach (var item in FoodItemResponseList)
                {
                    var Kitchen = await _unitOfWork.Context.Kitchen.Where(x => x.Id == item.KitchenId).SingleOrDefaultAsync();
                    item.KitchenName = Kitchen.Name;
                    item.Total = item.ItemPrice;
                    if (item.FoodCategoryId > 0)
                    {
                        var AttachmentPath = await _unitOfWork.Context.FoodCategory.Where(x => x.Id == item.FoodCategoryId && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (AttachmentPath != null)
                            item.FoodCategoryName = AttachmentPath.Name;
                    }
                    if (item.FoodVarient != null)
                    {
                        foreach (var varient in item.FoodVarient)
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
            return FoodItemResponseList;
        }
        public async Task<FoodItemResponseDTO> GetById(int Id, string Path)
        {
            var response = await _unitOfWork.Context.FoodItem.Include(x => x.FoodItemOffer).Where(x => x.Id == Id).Include(x => x.FoodVarient).SingleOrDefaultAsync();
            var FoodItemResponseList = _mapper.Map<FoodItemResponseDTO>(response);
            if (FoodItemResponseList != null)
            {
                var Kitchen = await _unitOfWork.Context.Kitchen.Where(x => x.Id == FoodItemResponseList.KitchenId).SingleOrDefaultAsync();
                FoodItemResponseList.KitchenName = Kitchen.Name;
                if (FoodItemResponseList.FoodCategoryId > 0)
                {
                    var AttachmentPath = await _unitOfWork.Context.FoodCategory.Where(x => x.Id == FoodItemResponseList.FoodCategoryId && x.IsDeleted == false).SingleOrDefaultAsync();
                    if (AttachmentPath != null)
                        FoodItemResponseList.FoodCategoryName = AttachmentPath.Name;
                }
                if (FoodItemResponseList.FoodVarient != null)
                {
                    foreach (var varient in FoodItemResponseList.FoodVarient)
                    {
                        if (varient.FoodItemId > 0)
                        {
                            var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == varient.FoodItemId).SingleOrDefaultAsync();
                            if (FoodItem != null)
                            {
                                varient.FoodItemName = FoodItem.Name;
                                varient.Quantity = 1;
                                varient.Total = varient.Price;
                            }
                        }
                    }
                }
            }
            return FoodItemResponseList;
        }
        public async Task<FoodItemResponseDTO> GetByFoodItemId(int Id)
        {
            var response = await _unitOfWork.Context.FoodItem.Include(x => x.FoodItemOffer).Where(x => x.Id == Id && x.Status == false).Include(x => x.FoodVarient).SingleOrDefaultAsync();
            var FoodItemResponseList = _mapper.Map<FoodItemResponseDTO>(response);
            //if (FoodItemResponseList != null)
            //{
            //    if (FoodItemResponseList.AttachmentId >= 0)
            //    {
            //        var Attachment = await _unitOfWork.Context.Attachment.Where(x => x.Id == FoodItemResponseList.AttachmentId && x.IsDeleted == false).SingleOrDefaultAsync();
            //        if (Attachment != null)
            //            FoodItemResponseList.Attachment = Path + Attachment.FileToUpLoad;
            //    }
            //    if (FoodItemResponseList.FoodCategoryId > 0)
            //    {
            //        var AttachmentPath = await _unitOfWork.Context.FoodCategory.Where(x => x.Id == FoodItemResponseList.FoodCategoryId && x.IsDeleted == false).SingleOrDefaultAsync();
            //        if (AttachmentPath != null)
            //            FoodItemResponseList.FoodCategoryName = AttachmentPath.Name;
            //    }
            //    if (FoodItemResponseList.FoodVarient != null)
            //    {
            //        foreach (var varient in FoodItemResponseList.FoodVarient)
            //        {
            //            if (varient.FoodItemId > 0)
            //            {
            //                var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == varient.FoodItemId).SingleOrDefaultAsync();
            //                if (FoodItem != null)
            //                {
            //                    varient.FoodItemName = FoodItem.Name;
            //                    varient.Quantity = 1;
            //                    varient.Total = varient.Price;
            //                }
            //            }
            //        }
            //    }
            //}
            return FoodItemResponseList;
        }
        public async Task<List<FoodItemResponseDTO>> SearchFoodItem(SearchFoodItemRequestDTO model)
        {
            var Id = model.Id.ToString();
            List<FoodItem> searchItem = new List<FoodItem>();
            if (model.Id > 0)
            {
                searchItem = await _unitOfWork.Context.FoodItem.Where(s => s.Id == model.Id && s.Status==true).Include(x => x.FoodItemOffer).Where(x => x.Status == false).Include(x => x.FoodVarient).ToListAsync();
            }
            else if (model.Name != null)
            {
                searchItem = await _unitOfWork.Context.FoodItem.Where(s => s.Name.Contains(model.Name) && s.Status==true).Include(x => x.FoodVarient).ToListAsync();
            }
            var FoodItemResponseList = _mapper.Map<List<FoodItemResponseDTO>>(searchItem);
            foreach (var item in FoodItemResponseList)
            {
                foreach (var varient in item.FoodVarient)
                {
                    var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == varient.FoodItemId).SingleOrDefaultAsync();
                    if (FoodItem != null)
                    {
                        varient.FoodItemName = FoodItem.Name;
                        varient.Quantity = 1;
                        varient.Total = varient.Price;
                    }
                }
            }
            return FoodItemResponseList;
        }
        public async Task<List<SearchFoodItemResponseDTO>> SearchFoodItemUpd(SearchFoodItemRequestDTO model)
        {
            var Id = model.Id.ToString();
            List<FoodItem> searchItem = new List<FoodItem>();
            List<SearchFoodItemResponseDTO> searchresponse = new List<SearchFoodItemResponseDTO>();
            if (model.Id > 0)
            {
                searchItem = await _unitOfWork.Context.FoodItem.Where(s => s.Id == model.Id && s.Status==true).Include(x => x.FoodItemOffer).ToListAsync();
            }
            else if (model.Name != null)
            {
                searchItem = await _unitOfWork.Context.FoodItem.Where(s => s.Name.Contains(model.Name) && s.Status==true).ToListAsync();
            }
            var FoodItemResponseList = _mapper.Map<List<SearchFoodItemResponseDTO>>(searchItem);
            foreach (var item in FoodItemResponseList)
            {

                var foodvarient = await _unitOfWork.Context.FoodVarient.Where(x => x.FoodItemId == item.Id).ToListAsync();
                foreach (var varient in foodvarient)
                {
                    SearchFoodItemResponseDTO searchFoodItemResponseDTO = new SearchFoodItemResponseDTO()
                    {
                        Name = item.Name,
                        AttachmentId = item.AttachmentId,
                        CookingTime = item.CookingTime,
                        FoodCategoryId = item.FoodCategoryId,
                        Id = item.Id,
                        IsOffer = item.IsOffer,
                        KitchenId = item.KitchenId,
                        ItemPrice = item.ItemPrice,
                        VAT = item.VAT,
                        Total = item.Total,
                        Description = item.Description,
                        IsSpecial = item.IsSpecial,
                        Quantity = item.Quantity,
                        Status = item.Status,
                        Note = item.Note,
                        FoodCategoryName = item.FoodCategoryName
                    };
                    searchFoodItemResponseDTO.Name = searchFoodItemResponseDTO.Name + " " + varient.Name;
                    //var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == varient.FoodItemId).SingleOrDefaultAsync();
                    searchFoodItemResponseDTO.FoodVarient = _mapper.Map<FoodVarientResponseDTO>(varient);
                    var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == searchFoodItemResponseDTO.FoodVarient.FoodItemId).SingleOrDefaultAsync();
                    if (FoodItem != null)
                    {
                        searchFoodItemResponseDTO.FoodVarient.FoodItemName = FoodItem.Name;
                        searchFoodItemResponseDTO.FoodVarient.Quantity = 1;
                        //searchFoodItemResponseDTO.FoodVarient.Total = varient.Price;
                    }
                    searchresponse.Add(searchFoodItemResponseDTO);
                }
            }
            return searchresponse;
        }
        public async Task<long> IsExist(string CategoryName)
        {
            var ExistCategoryName = await _unitOfWork.Context.FoodItem.Where(x => x.Status == true).FirstOrDefaultAsync(x => x.Name.Equals(CategoryName));
            if (ExistCategoryName == null)
                return 0;
            return 1;
        }
        public async Task<long> Save(FoodItemRequestDTO model, int Id)
        {
            FoodItem foodItem = new FoodItem();
            foodItem = _mapper.Map<FoodItem>(model);
            foodItem.IsSynchronized = false;
            if (model.Id <= 0)
            {

                var data = _unitOfWork.Context.FoodItem.SingleOrDefault(x => x.Name.Equals(model.Name));
                if (data == null)
                {
                    foodItem.Quantity = 1;
                    _unitOfWork.Context.Set<FoodItem>().Add(foodItem);
                }
            }
            else
            {
                // var PriviousRecord = _unitOfWork.Context.FoodItem.FirstOrDefault(x => x.Id == model.Id);
                _unitOfWork.Context.Entry(foodItem).State = EntityState.Modified;
                _unitOfWork.Context.FoodItem.Update(foodItem);
            }
            await _unitOfWork.Commit();
            return foodItem.Id;
        }
        public async Task<long> Delete(int Id)
        {
            FoodItem foodItemDelete = _unitOfWork.Context.FoodItem.Where(x => x.Id.Equals(Id) && x.Status == true).FirstOrDefault();
            if (foodItemDelete != null)
            {
                foodItemDelete.Status = false;
                _unitOfWork.Context.Entry(foodItemDelete).State = EntityState.Modified;
                _unitOfWork.Context.FoodItem.Update(foodItemDelete);
                return await _unitOfWork.Commit();
            }
            return 0;
        }
        public async Task<long> ActiveFoodItem(int Id)
        {
            FoodItem foodItemDelete = _unitOfWork.Context.FoodItem.Where(x => x.Id.Equals(Id) && x.Status == false).FirstOrDefault();
            if (foodItemDelete != null)
            {
                foodItemDelete.Status = true;
                _unitOfWork.Context.Entry(foodItemDelete).State = EntityState.Modified;
                _unitOfWork.Context.FoodItem.Update(foodItemDelete);
                return await _unitOfWork.Commit();
            }
            return 0;
        }
        public async Task<(IEnumerable<FoodItemResponseDTO> FoodItemList, int recordsCount)> GetByFilters(SearchFilter filters)
        {
            List<FoodItem> listData = new List<FoodItem>();
            var predicate = PredicateBuilder.New<FoodItem>(true);
            bool isPradicate = false;
            predicate = predicate.And(i => i.Status == false);
            if (!string.IsNullOrEmpty(filters.SearchText))
            {
                predicate = predicate.Or(i => i.Name.Contains(filters.SearchText));
                predicate = predicate.Or(i => i.Description.Contains(filters.SearchText));
                predicate = predicate.Or(i => i.VAT.Contains(filters.SearchText));
                predicate = predicate.Or(i => i.Note.Contains(filters.SearchText));
                predicate = predicate.Or(i => i.CookingTime.Contains(filters.SearchText));
                isPradicate = true;
            }
            if (filters.Filters != null)
                foreach (var filter in filters.Filters)
                {
                    if (filter.Logic == "and")
                        predicate.And(DBHelper.BuildPredicate<FoodItem>(filter.Name, filter.Operator, filter.Value));
                    else if (filter.Logic == "or")
                        predicate.Or(DBHelper.BuildPredicate<FoodItem>(filter.Name, filter.Operator, filter.Value));
                    else
                        predicate.And(DBHelper.BuildPredicate<FoodItem>(filter.Name, filter.Operator, filter.Value));
                    isPradicate = true;
                }
            if (isPradicate)
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitOfWork.Context.FoodItem.Where(predicate).Skip(filters.Offset * filters.PageSize).Take(filters.PageSize).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            else
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitOfWork.Context.FoodItem.Skip(filters.Offset * filters.PageSize).Take(filters.Offset).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            var recordsCount = await _unitOfWork.Context.User.Where(x => x.IsDeleted == false).CountAsync();
            (List<FoodItemResponseDTO> FoodCategoryList, int recordsCount) tuple = (_mapper.Map<List<FoodItemResponseDTO>>(listData), recordsCount);
            return tuple;
        }

        public async Task<List<SearchFoodItemResponseDTO>> GetAllFooditemWithvariant()
        {

            List<FoodItem> searchItem = new List<FoodItem>();
            List<SearchFoodItemResponseDTO> searchresponse = new List<SearchFoodItemResponseDTO>();
            searchItem = await _unitOfWork.Context.FoodItem.Include(x => x.FoodItemOffer).ToListAsync();
            var FoodItemResponseList = _mapper.Map<List<SearchFoodItemResponseDTO>>(searchItem);
            foreach (var item in FoodItemResponseList)
            {

                var foodvarient = await _unitOfWork.Context.FoodVarient.Where(x => x.FoodItemId == item.Id).ToListAsync();
                foreach (var varient in foodvarient)
                {
                    SearchFoodItemResponseDTO searchFoodItemResponseDTO = new SearchFoodItemResponseDTO()
                    {
                        Name = item.Name,
                        AttachmentId = item.AttachmentId,
                        CookingTime = item.CookingTime,
                        FoodCategoryId = item.FoodCategoryId,
                        Id = item.Id,
                        IsOffer = item.IsOffer,
                        KitchenId = item.KitchenId,
                        ItemPrice = item.ItemPrice,
                        VAT = item.VAT,
                        Total = item.Total,
                        Description = item.Description,
                        IsSpecial = item.IsSpecial,
                        Quantity = item.Quantity,
                        Status = item.Status,
                        Note = item.Note,
                        FoodCategoryName = item.FoodCategoryName
                    };
                    searchFoodItemResponseDTO.Name = searchFoodItemResponseDTO.Name + " " + varient.Name;
                    //var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == varient.FoodItemId).SingleOrDefaultAsync();
                    searchFoodItemResponseDTO.FoodVarient = _mapper.Map<FoodVarientResponseDTO>(varient);
                    var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == searchFoodItemResponseDTO.FoodVarient.FoodItemId).SingleOrDefaultAsync();
                    if (FoodItem != null)
                    {
                        searchFoodItemResponseDTO.FoodVarient.FoodItemName = FoodItem.Name;
                        searchFoodItemResponseDTO.FoodVarient.Quantity = 1;
                        //searchFoodItemResponseDTO.FoodVarient.Total = varient.Price;
                    }
                    searchresponse.Add(searchFoodItemResponseDTO);
                }
            }
            return searchresponse;
        }
    }
    public interface IFoodItemRepository
    {
        Task<IEnumerable<FoodItemResponseDTO>> GetAll(string Path);
        Task<FoodItemResponseDTO> GetById(int Id, string Path);
        Task<long> ActiveFoodItem(int Id);
        Task<FoodItemResponseDTO> GetByFoodItemId(int Id);
        Task<List<SearchFoodItemResponseDTO>> GetAllFooditemWithvariant();
        Task<List<SearchFoodItemResponseDTO>> SearchFoodItemUpd(SearchFoodItemRequestDTO model);
        Task<List<FoodItemResponseDTO>> SearchFoodItem(SearchFoodItemRequestDTO model);
        Task<long> IsExist(string CategoryName);
        Task<long> Save(FoodItemRequestDTO model, int Id);
        Task<long> Delete(int Id);
        Task<(IEnumerable<FoodItemResponseDTO> FoodItemList, int recordsCount)> GetByFilters(SearchFilter filters);
        Task<List<FoodItem>> GetALLSync();
    }
}