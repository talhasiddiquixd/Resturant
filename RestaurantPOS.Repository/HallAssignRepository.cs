using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using RestaurantPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Repository
{
    public class HallAssignRepository:IHallAssignRepository
    {
        private readonly IMapper _mapper;
        private readonly IunitOfWork _unitofWork;
        public HallAssignRepository(IunitOfWork unitofwork, IMapper mapper)
        {
            _mapper = mapper;
            _unitofWork = unitofwork;
        }
        public async Task<List<HallAssign>> GetALLSync()
        {
            var dataList = await _unitofWork.Context.HallAssign.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<List<UserResponseDTO>> GetALL()
        {
            //var data = await _unitofWork.Context.HallAssign.Where(x => x.IsDeleted == false).ToListAsync();
            //var model = _mapper.Map<List<HallAssignResponseDTO>>(data);
            //if (model!=null)
            //{
            //    foreach (var item in model)
            //    {
            //        if (item.UserId > 0 && item.HallId > 0)
            //        {
            //            Hall HallData = _unitofWork.Context.Hall.Where(x => x.Id == item.HallId && x.IsDeleted == false).FirstOrDefault();
            //            if (HallData != null)
            //                item.HallName = HallData.Name;
            //            User User = _unitofWork.Context.User.Where(x => x.Id == item.UserId && x.IsDeleted == false).FirstOrDefault();
            //            if (User != null)
            //                item.UserName = User.Username;
            //        }
            //    } 
            //}
            //return model;

            var data = await _unitofWork.Context.User.Where(x => x.AssignedType== 2 && x.IsDeleted ==false).ToListAsync();
            var response = _mapper.Map<List<UserResponseDTO>>(data);
            foreach (var item in response)
            {
                item.AssignesTypeName = _unitofWork.Context.Hall.Where(x => x.Id == item.AssignedRole).FirstOrDefault().Name;
            }
            return response;
        }
        public async Task<HallAssignResponseDTO> GetById(int Id)
        {
            var data = await _unitofWork.Context.HallAssign.Where(x => x.Id == Id && x.IsDeleted == false).FirstOrDefaultAsync();
            var model = _mapper.Map<HallAssignResponseDTO>(data);
            if ( model!=null && model.UserId > 0 && model.HallId > 0 )
            {
                Hall HallData = _unitofWork.Context.Hall.Where(x => x.Id == model.HallId && x.IsDeleted == false).FirstOrDefault();
                if (HallData != null)
                    model.HallName = HallData.Name;
                User User = _unitofWork.Context.User.Where(x => x.Id == model.UserId && x.IsDeleted == false).FirstOrDefault();
                if (User != null)
                    model.UserName = User.Username;
            }
            return model;
        }
        public async Task<bool> IsExist(string Name)
        {
            var data = await _unitofWork.Context.HallAssign.Where(x => x.Name == Name && x.IsDeleted == false).FirstOrDefaultAsync();
            if (data == null)
                return true;
            return false;
        }

        public async Task<long> Save(HallAssignRequestDTO modelDTO, int loginUserId)
        {
            var model = _mapper.Map<HallAssign>(modelDTO);
            if (model.Id <= 0)
            {
                model.CreatedBy = loginUserId;
                model.CreatedAt = DateTime.Now;
                model.IsSynchronized = false;
                _unitofWork.Context.Set<HallAssign>().Add(model);
            }
            else
            {
                var priviousRecord = await _unitofWork.Context.HallAssign.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                model.UpdatedBy = loginUserId;
                model.UpdatedAt = DateTime.Now;
                model.CreatedBy = priviousRecord.CreatedBy;
                model.CreatedAt = priviousRecord.CreatedAt;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.HallAssign.Update(model);
            }
            return await _unitofWork.Commit();
        }
        public async Task<long> Delete(int Id, int loginUserId)
        {
            HallAssign model = _unitofWork.Context.HallAssign.Where(x => x.Id.Equals(Id) && x.IsDeleted == false).FirstOrDefault();
            if (model != null)
            {
                model.IsDeleted = true;
                model.UpdatedBy = loginUserId;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.HallAssign.Update(model);
                return await _unitofWork.Commit();
            }
            return 0;
        }

        public async  Task<bool> AlreadyAssined(int UserId, int HallId)
        {
            HallAssign model = await _unitofWork.Context.HallAssign.Where(x => x.UserId == UserId && x.HallId == HallId && x.IsDeleted == false).FirstOrDefaultAsync();
            if (model == null)
            {
                return false;
            }
            return true;
        }
    }
    public interface IHallAssignRepository
    {
        //Task<List<HallAssignResponseDTO>> GetALL();
        Task<List<UserResponseDTO>> GetALL();
        Task<HallAssignResponseDTO> GetById(int Id);
        Task<bool> AlreadyAssined(int UserId, int HallId);
        Task<long> Save(HallAssignRequestDTO modeltDTO, int loginUserId);
        Task<long> Delete(int id, int loinUserId);
        Task<bool> IsExist(string Name);
        Task<List<HallAssign>> GetALLSync();
    }
}
