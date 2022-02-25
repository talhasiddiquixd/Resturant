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
    public class HallRepository: IHallRepository
    {
        private readonly IMapper _mapper;
        private readonly IunitOfWork _unitofWork;
        public HallRepository(IMapper mapper, IunitOfWork unitofwork)
        {
            _mapper = mapper;
            _unitofWork = unitofwork;
        }

        public async Task<List<Hall>> GetALLSync()
        {
            var dataList =await _unitofWork.Context.Hall.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }

        public async Task<List<HallResponseDTO>> GetALL()
        {
            var data = await _unitofWork.Context.Hall.Where(x => x.IsDeleted == false).Include(x => x.Table.Where(x=> x.IsDeleted==false)).ToListAsync();
            var model = _mapper.Map<List<HallResponseDTO>>(data);
            return model;
        } 

        public async Task<HallResponseDTO> GetById(int Id)
        {
            var data = await _unitofWork.Context.Hall.Where(x => x.Id == Id && x.IsDeleted == false).Include(x => x.Table.Where(x => x.IsDeleted == false)).FirstOrDefaultAsync();
            var model = _mapper.Map<HallResponseDTO>(data);
            return model;
        }

        public async Task<bool> IsExist(string Name)
        {
            var data = await _unitofWork.Context.Hall.Where(x => x.Name == Name && x.IsDeleted == false).FirstOrDefaultAsync();
            if (data == null)
                return true;
            return false;
        }

        public async Task<long> Save(HallRequestDTO modelDTO, int loginUserId)
        {
            var model = _mapper.Map<Hall>(modelDTO);
            if (model.Id <= 0)
            {
                model.CreatedBy = loginUserId;
                model.CreatedAt = DateTime.Now;
                model.IsSynchronized = false;
                _unitofWork.Context.Set<Hall>().Add(model);
            }
            else
            {
                var priviousRecord = await _unitofWork.Context.Hall.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                model.UpdatedBy = loginUserId;
                model.UpdatedAt = DateTime.Now;
                model.CreatedAt = priviousRecord.CreatedAt;
                model.CreatedBy = priviousRecord.CreatedBy;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.Hall.Update(model);
            }
            return await _unitofWork.Commit();
        }

        public async Task<long> Delete(int Id, int loginUserId)
        {
            Hall model = _unitofWork.Context.Hall.Where(x => x.Id.Equals(Id) && x.IsDeleted == false).FirstOrDefault();
            if (model != null)
            {
                model.IsDeleted = true;
                model.UpdatedBy = loginUserId;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.Hall.Update(model);
                return await _unitofWork.Commit();
            }
            return 0;
        }
    }

    public interface IHallRepository
    {
        Task<List<HallResponseDTO>> GetALL();
        Task<HallResponseDTO> GetById(int Id);
        Task<long> Save(HallRequestDTO modeltDTO, int loginUserId);
        Task<long> Delete(int id, int loinUserId);
        Task<bool> IsExist(string Name);
        Task<List<Hall>> GetALLSync();
    }
}
