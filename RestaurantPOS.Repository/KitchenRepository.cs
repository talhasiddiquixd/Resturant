using AutoMapper;
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
    public class KitchenRepository : IKitchenRepository
    {
        private readonly IMapper _mapper;
        private readonly IunitOfWork _unitofWork;
        public KitchenRepository(IMapper mapper, IunitOfWork unitofwork)
        {
            _mapper = mapper;
            _unitofWork = unitofwork;
        }
        public async Task<List<Kitchen>> GetALLSync()
        {
            var dataList = await _unitofWork.Context.Kitchen.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<List<KitchenResponseDTO>> GetALL()
        {
            var data = await _unitofWork.Context.Kitchen.Where(x => x.IsDeleted == false).ToListAsync();
            var response = _mapper.Map<List<KitchenResponseDTO>>(data);
            foreach (var item in response)
            {
                item.Collapse = false;
            }
            return response;
        }
        public async Task<KitchenResponseDTO> GetById(int Id)
        {
            var data = await _unitofWork.Context.Kitchen.Where(x => x.Id == Id && x.IsDeleted == false).FirstOrDefaultAsync();
            var response = _mapper.Map<KitchenResponseDTO>(data);
            response.Collapse = false;
            return response;
        }
        public async Task<bool> IsExist(string Name)
        {
            var data = await _unitofWork.Context.Kitchen.SingleOrDefaultAsync(x => x.Name == Name && x.IsDeleted == false);
            if (data == null)
             return true; 
            return false;
        }
    
        public async Task<long> Save(KitchenRequestDTO modelDTO, int loginUserId)
        {
            var model = _mapper.Map<Kitchen>(modelDTO);
            if (model.Id <= 0)
            {
                model.CreatedBy = loginUserId;
                model.CreatedAt = DateTime.Now;
                model.IsSynchronized = false;
                _unitofWork.Context.Set<Kitchen>().Add(model);
            }
            else
            {
                var priviousRecord = await _unitofWork.Context.Kitchen.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                model.UpdatedBy = loginUserId;
                model.UpdatedAt = DateTime.Now;
                model.CreatedAt = priviousRecord.CreatedAt;
                model.CreatedBy = priviousRecord.CreatedBy;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.Kitchen.Update(model);
            }
            return await _unitofWork.Commit();
        }
        public async Task<long> Delete(int Id, int loginUserId)
        {
            Kitchen model = _unitofWork.Context.Kitchen.Where(x => x.Id.Equals(Id) && x.IsDeleted == false).FirstOrDefault();
            if (model != null)
            {
                model.IsDeleted = true;
                model.UpdatedBy = loginUserId;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.Kitchen.Update(model);
                return await _unitofWork.Commit();
            }
            return 0;
        }
    }

    public interface IKitchenRepository
    {
        Task<List<KitchenResponseDTO>> GetALL();
        Task<KitchenResponseDTO> GetById(int Id);
        Task<long> Save(KitchenRequestDTO modelDTO, int loginUserId);
        Task<long> Delete(int id, int loinUserId);
        Task<bool> IsExist(string Name);
        Task<List<Kitchen>> GetALLSync();
    }
}