using System;
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
    public class FoodVarientRepository : IFoodVarientRepository
    {
        private readonly IMapper _mapper;
        private readonly IunitOfWork _unitofwork;
        public FoodVarientRepository(IunitOfWork unitofwork, IMapper mapper)
        {
            _mapper = mapper;
            _unitofwork = unitofwork;
        }
        public async Task<List<FoodVarient>> GetALLSync()
        {
            var dataList = await _unitofwork.Context.FoodVarient.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<List<FoodVarientResponseDTO>> GetALL()
        {
            var data = await _unitofwork.Context.FoodVarient.Where(x => x.IsDeleted == false && x.IsActive == true).ToListAsync();
            var model = _mapper.Map<List<FoodVarientResponseDTO>>(data);
            if (model != null)
            {
                foreach (var item in model)
                {
                    if (item.FoodItemId > 0)
                    {
                        FoodItem FoodItem = _unitofwork.Context.FoodItem.Where(x => x.Id == item.FoodItemId).FirstOrDefault();
                        if (FoodItem != null)
                        {
                            item.FoodItemName = FoodItem.Name;
                            item.CookingTime = FoodItem.CookingTime;
                        }

                    }
                }
            }
            return model;
        }

        public async Task<FoodVarientResponseDTO> GetById(int Id)
        {
            var data = await _unitofwork.Context.FoodVarient.Where(x => x.Id == Id && x.IsDeleted == false && x.IsActive == true).FirstOrDefaultAsync();
            var model = _mapper.Map<FoodVarientResponseDTO>(data);
            if (model != null)
            {
                model.IsActive = true;
                if (model.FoodItemId > 0)
                {
                    FoodItem FoodItem = _unitofwork.Context.FoodItem.Where(x => x.Id == model.FoodItemId).FirstOrDefault();
                    if (FoodItem != null)
                    {
                        model.FoodItemName = FoodItem.Name;
                        model.CookingTime = FoodItem.CookingTime;
                    }
                }
            }
            return model;
        }
        public async Task<bool> IsExist(string Name, int foodItemId)
        {
            var data = await _unitofwork.Context.FoodVarient.Where(x => x.Name == Name && x.FoodItemId == foodItemId && x.IsDeleted == false).FirstOrDefaultAsync();
            if (data == null)
                return true;
            return false;
        }

        public async Task<long> Save(FoodVarientRequestDTO modelDTO, int loginUserId)
        {
            var model = _mapper.Map<FoodVarient>(modelDTO);
            if (model.Id <= 0)
            {
                model.CreatedBy = loginUserId;
                model.CreatedAt = DateTime.Now;
                model.IsSynchronized = false;
                model.IsActive = true;
                _unitofwork.Context.Set<FoodVarient>().Add(model);
            }
            else
            {
                var priviousRecord = await _unitofwork.Context.FoodVarient.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                model.UpdatedBy = loginUserId;
                model.UpdatedAt = DateTime.Now;
                model.CreatedAt = priviousRecord.CreatedAt;
                model.CreatedBy = priviousRecord.CreatedBy;
                model.KitchenId = priviousRecord.KitchenId;
                model.IsActive = priviousRecord.IsActive;
                model.IsDeleted = priviousRecord.IsDeleted;
                _unitofwork.Context.Entry(model).State = EntityState.Modified;
                _unitofwork.Context.FoodVarient.Update(model);
            }
            return await _unitofwork.Commit();
        }
        public async Task<long> Delete(int Id, int loginUserId)
        {
            FoodVarient model = _unitofwork.Context.FoodVarient.Where(x => x.Id.Equals(Id) && x.IsDeleted == false).FirstOrDefault();
            if (model != null)
            {
                model.IsDeleted = true;
                model.UpdatedBy = loginUserId;
                _unitofwork.Context.Entry(model).State = EntityState.Modified;
                _unitofwork.Context.FoodVarient.Update(model);
                return await _unitofwork.Commit();
            }
            return 0;
        }
    }



    public interface IFoodVarientRepository
    {
        Task<List<FoodVarientResponseDTO>> GetALL();
        Task<FoodVarientResponseDTO> GetById(int Id);
        Task<long> Save(FoodVarientRequestDTO modeltDTO, int loginUserId);
        Task<long> Delete(int id, int loinUserId);
        Task<bool> IsExist(string Name, int itemId);
        Task<List<FoodVarient>> GetALLSync();
    }
}
