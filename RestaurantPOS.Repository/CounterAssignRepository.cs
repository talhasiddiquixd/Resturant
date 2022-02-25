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
    public class CounterAssignRepository : ICounterAssignRepository
    {
        private readonly IMapper _mapper;
        private readonly IunitOfWork _unitofWork;
        public CounterAssignRepository(IMapper mapper, IunitOfWork unitofwork)
        {
            _mapper = mapper;
            _unitofWork = unitofwork;
        }
        public async Task<List<CounterAssign>> GetALLSync()
        {
            var dataList = await _unitofWork.Context.CounterAssign.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<List<UserResponseDTO>> GetALL()
        {
            //var data = await _unitofWork.Context.CounterAssign.Where(x => x.IsActive == true).ToListAsync();
            //var model = _mapper.Map<List<CounterAssignResponseDTO>>(data);
            ////if (model != null)
            ////{
            ////    foreach (var item in model)
            ////    {
            ////        if (item.UserId > 0 && item.CounterId > 0)
            ////        {
            ////            User UserData = _unitofWork.Context.User.Where(x => x.Id == item.UserId && x.IsDeleted == false).FirstOrDefault();
            ////            if (UserData != null)
            ////                item.UserName = UserData.Username;
            ////            Counter CounterData = _unitofWork.Context.Counter.Where(x => x.Id == item.CounterId && x.IsActive == true).FirstOrDefault();
            ////            if (CounterData != null)
            ////                item.CounterName = CounterData.Name;
            ////        }
            ////    }
            ////}
            //return model;
            var data = await _unitofWork.Context.User.Where(x => x.AssignedType == 3 && x.IsDeleted == false).ToListAsync();
            var response = _mapper.Map<List<UserResponseDTO>>(data);
            foreach (var item in response)
            {
                item.AssignesTypeName = _unitofWork.Context.Counter.Where(x => x.Id == item.AssignedRole).FirstOrDefault().Name;
            }
            return response;
        }
        public async Task<CounterAssignResponseDTO> GetById(int Id)
        {
            var data = await _unitofWork.Context.CounterAssign.Where(x => x.Id == Id && x.IsActive == true).FirstOrDefaultAsync();
            var model = _mapper.Map<CounterAssignResponseDTO>(data);
            if (model != null)
            {
                if (model.UserId > 0 && model.CounterId > 0)
                {
                    User UserData = _unitofWork.Context.User.Where(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                    if (UserData != null)
                        model.UserName = UserData.Username;
                    Counter CounterData = _unitofWork.Context.Counter.Where(x => x.Id == model.CounterId && x.IsActive == true).FirstOrDefault();
                    if (CounterData != null)
                        model.CounterName = CounterData.Name;
                }
                return model;
            }
            return null;
        }
        public async Task<bool> IsExist(string Name)
        {
            var data = await _unitofWork.Context.CounterAssign.Where(x => x.Name == Name && x.IsActive == true).FirstOrDefaultAsync();
            if (data == null)
                return true;
            return false;
        }

        public async Task<long> Save(CounterAssignRequestDTO modelDTO, int loginUserId)
        {
            var model = _mapper.Map<CounterAssign>(modelDTO);
            if (model.Id <= 0)
            {
                var data = await _unitofWork.Context.CounterAssign.Where(x => x.UserId == modelDTO.UserId && x.IsActive == true).FirstOrDefaultAsync();
                model.IsActive = true;
                model.CreatedBy = loginUserId;
                model.IsSynchronized = false;
                model.CreatedAt = DateTime.Now;
                _unitofWork.Context.Set<CounterAssign>().Add(model);
            }
            else
            {
                var priviousRecord = await _unitofWork.Context.CounterAssign.Where(x => x.Id == model.Id && x.IsActive==true).FirstOrDefaultAsync();
                model.IsActive = true;
                model.CreatedAt = priviousRecord.CreatedAt;
                model.CreatedBy = priviousRecord.CreatedBy;
                model.UpdatedBy = loginUserId;
                model.UpdatedAt = DateTime.Now;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.CounterAssign.Update(model);
            }
            return await _unitofWork.Commit();
        }
        public async Task<long> Delete(int Id, int loginUserId)
        {
            CounterAssign model = _unitofWork.Context.CounterAssign.Where(x => x.Id.Equals(Id) && x.IsActive == true).FirstOrDefault();
            if (model != null)
            {
                model.IsActive = false;
                model.UpdatedBy = loginUserId;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.CounterAssign.Update(model);
                return await _unitofWork.Commit();
            }
            return 0;
        }

        public async Task<bool> AlreadyAssined(int UserId, int CounterId)
        {
            CounterAssign model = await _unitofWork.Context.CounterAssign.Where(x => x.UserId == UserId && x.CounterId == CounterId).FirstOrDefaultAsync();
            if (model == null)
            {
                return false;
            }
            return true;
        }
    }
    public interface ICounterAssignRepository
    {
        Task<List<UserResponseDTO>> GetALL();
        Task<CounterAssignResponseDTO> GetById(int Id);
        Task<bool> AlreadyAssined(int UserId, int CounterId);
        Task<long> Save(CounterAssignRequestDTO modeltDTO, int loginUserId);
        Task<long> Delete(int id, int loinUserId);
        Task<bool> IsExist(string Name);
        Task<List<CounterAssign>> GetALLSync();
    }
}
