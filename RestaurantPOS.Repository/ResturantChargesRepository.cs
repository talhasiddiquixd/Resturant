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
    public class ResturantChargesRepository : IResturantChargesRepository
    {
        private readonly IMapper _mapper;
        private readonly IunitOfWork _unitofwork;
        public ResturantChargesRepository(IMapper mapper, IunitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
        }
        public async Task<List<ResturantCharges>> GetALLSync()
        {
            var dataList = await _unitofwork.Context.ResturantCharges.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<List<ResturantChargesResponseDTO>> GetALL()
        {
            var data = await _unitofwork.Context.ResturantCharges.Where(x => x.IsDeleted == false).ToListAsync();
            var model = _mapper.Map<List<ResturantChargesResponseDTO>>(data);
            return model;
        }
        public async Task<ResturantChargesResponseDTO> GetById(int Id)
        {
            var data = await _unitofwork.Context.ResturantCharges.Where(x => x.Id == Id && x.IsDeleted == false).FirstOrDefaultAsync();
            var model = _mapper.Map<ResturantChargesResponseDTO>(data);
            return model;
        }
        public async Task<long> Save(ResturantChargesRequestDTO modelDTO, int loginUserId)
        {
            var model = _mapper.Map<ResturantCharges>(modelDTO);
            if (model.Id <= 0)
            {
                model.CreatedBy = loginUserId;
                model.CreatedAt = DateTime.Now;
                model.IsSynchronized = false;
                _unitofwork.Context.Set<ResturantCharges>().Add(model);
            }
            else
            {
                var priviousRecord = await _unitofwork.Context.ResturantCharges.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                model.UpdatedBy = loginUserId;
                model.UpdatedAt = DateTime.Now;
                model.CreatedAt = priviousRecord.CreatedAt;
                model.CreatedBy = priviousRecord.CreatedBy;
                model.IsSynchronized = false;
                _unitofwork.Context.Entry(model).State = EntityState.Modified;
                _unitofwork.Context.ResturantCharges.Update(model);
            }
            return await _unitofwork.Commit();
        }
        public async Task<long> Delete(int Id, int loginUserId)
        {
            ResturantCharges model = _unitofwork.Context.ResturantCharges.Where(x => x.Id.Equals(Id) && x.IsDeleted == false).FirstOrDefault();
            if (model != null)
            {
                model.IsDeleted = true;
                model.UpdatedBy = loginUserId;
                _unitofwork.Context.Entry(model).State = EntityState.Modified;
                _unitofwork.Context.ResturantCharges.Update(model);
                return await _unitofwork.Commit();
            }
            return 0;
        }
    }
    public interface IResturantChargesRepository
    {
        Task<List<ResturantChargesResponseDTO>> GetALL();
        Task<ResturantChargesResponseDTO> GetById(int Id);
        Task<long> Save(ResturantChargesRequestDTO modeltDTO, int loginUserId);
        Task<long> Delete(int id, int loinUserId);
        Task<List<ResturantCharges>> GetALLSync();
    }
}
