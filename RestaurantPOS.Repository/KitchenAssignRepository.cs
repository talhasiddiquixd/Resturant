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
    public class KitchenAssignRepository : IKitchenAssignRepository
    {
        private readonly IMapper _mapper;
        private readonly IunitOfWork _unitofWork;
        public KitchenAssignRepository(IMapper mapper, IunitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitofWork = unitOfWork;
        }
        public async Task<List<KitchenAssign>> GetALLSync()
        {
            var dataList = await _unitofWork.Context.KitchenAssign.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        //public async Task<List<KitchenAssignResponseDTO>> GetALL()
        //{
        //    var KitchenAssignList = await _unitofWork.Context.KitchenAssign.Where(x => x.IsDeleted == false).ToListAsync();
        //    var KitchenAssignResponseList = _mapper.Map<List<KitchenAssignResponseDTO>>(KitchenAssignList);
        //    foreach (var item in KitchenAssignResponseList)
        //    {
        //        if (item.UserId > 0 && item.KitchenId > 0)
        //        {
        //            var user = await _unitofWork.Context.User.Where(x => x.Id == item.UserId && x.IsDeleted == false).FirstOrDefaultAsync();
        //            item.UserName = user.Username;
        //            var Kitchen = await _unitofWork.Context.Kitchen.Where(x => x.Id == item.KitchenId && x.IsDeleted == false).FirstOrDefaultAsync();
        //            item.KitchenName = Kitchen.Name;
        //        }
        //    }
        //    return KitchenAssignResponseList;
        //}
        public async Task<KitchenAssignResponseDTO> GetById(int Id)
        {
            var KitchenAssign = await _unitofWork.Context.KitchenAssign.Where(x => x.Id == Id && x.IsDeleted == false).FirstOrDefaultAsync();
            var KitchenAssignResponse = _mapper.Map<KitchenAssignResponseDTO>(KitchenAssign);
            if (KitchenAssignResponse.UserId > 0 && KitchenAssignResponse.KitchenId > 0)
            {
                var user = await _unitofWork.Context.User.Where(x => x.Id == KitchenAssignResponse.UserId && x.IsDeleted == false).FirstOrDefaultAsync();
                KitchenAssignResponse.UserName = user.Username;
                var Kitchen = await _unitofWork.Context.Kitchen.Where(x => x.Id == KitchenAssignResponse.UserId && x.IsDeleted == false).FirstOrDefaultAsync();
                KitchenAssignResponse.KitchenName = Kitchen.Name;
            }
            return KitchenAssignResponse;
        }
        public async Task<long> Save(KitchenAssignRequestDTO modelDTO)
        {
            var model = _mapper.Map<KitchenAssign>(modelDTO);
            if (model.Id <= 0)
            {
                model.AssignDate = DateTime.Now;
                model.IsSynchronized = false;
                _unitofWork.Context.Set<KitchenAssign>().Add(model);
            }
            else
            {
                var priviousRecord = await _unitofWork.Context.KitchenAssign.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.KitchenAssign.Update(model);
            }
            return await _unitofWork.Commit();
        }
        public async Task<long> Delete(int Id)
        {
            KitchenAssign model = _unitofWork.Context.KitchenAssign.Where(x => x.Id == Id && x.IsDeleted == false).FirstOrDefault();
            if (model != null)
            {
                model.IsDeleted = true;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.KitchenAssign.Update(model);
                return await _unitofWork.Commit();
            }
            return 0;
        }
        public async Task<bool> AlreadyAssined(int UserId, int kitchenId)
        {
            KitchenAssign model = await _unitofWork.Context.KitchenAssign.Where(x => x.UserId == UserId && x.KitchenId == kitchenId && x.IsDeleted == false).FirstOrDefaultAsync();
            if (model == null)
            {
                return false;
            }
            return true;
        }
        public async Task<List<UserResponseDTO>> GetAll()
        {
            var data =  await _unitofWork.Context.User.Where(x => x.AssignedType == 1 && x.IsDeleted ==false).ToListAsync();
            var response = _mapper.Map<List<UserResponseDTO>>(data);
            foreach (var item in response)
            {
                item.AssignesTypeName = _unitofWork.Context.Kitchen.Where(x => x.Id == item.AssignedRole).FirstOrDefault().Name;
            }
            return response;
        }

    }
    public interface IKitchenAssignRepository
    {
        //Task<List<KitchenAssignResponseDTO>> GetALL();
        Task<List<UserResponseDTO>> GetAll();
        Task<KitchenAssignResponseDTO> GetById(int Id);
        Task<bool> AlreadyAssined(int UserId, int kitchenId);
        Task<long> Save(KitchenAssignRequestDTO modelDTO);
        Task<long> Delete(int Id);
        Task<List<KitchenAssign>> GetALLSync();
    }
}
