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
    public class CounterRepository:ICounterRepository
    {
        private readonly IMapper _mapper;
        private readonly IunitOfWork _unitofWork;
        public CounterRepository(IMapper mapper, IunitOfWork unitofwork)
        {
            _mapper = mapper;
            _unitofWork = unitofwork;
        }
        public async Task<List<Counter>> GetALLSync()
        {
            var dataList = await _unitofWork.Context.Counter.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<List<CounterResponseDTO>> GetALL()
        {
            var Counter = await _unitofWork.Context.Counter.Where(x => x.IsActive == true).ToListAsync();
            var model = _mapper.Map<List<CounterResponseDTO>>(Counter);
            return model;
        }

        public async Task<CounterResponseDTO> GetById(int Id)
        {
            var data = await _unitofWork.Context.Counter.Where(x => x.Id == Id && x.IsActive == true).FirstOrDefaultAsync();
            var model = _mapper.Map<CounterResponseDTO>(data);
            return model;
        }
        public async Task<bool> IsExist(string Name)
        {
            var data = await _unitofWork.Context.Counter.Where(x => x.Name == Name && x.IsActive == true).FirstOrDefaultAsync();
            if (data == null)
                return true;
            return false;
        }

        public async Task<long> Save(CounterRequestDTO modelDTO, int loginUserId)
        {
            var model = _mapper.Map<Counter>(modelDTO);
            if (model.Id <= 0)
            {
                model.IsActive = true;
                model.CreatedBy = loginUserId;
                model.CreatedAt = DateTime.Now;
                model.IsSynchronized = false;
                _unitofWork.Context.Set<Counter>().Add(model);
            }
            else
            {
                var priviousRecord = await _unitofWork.Context.Counter.Where(x => x.Id == model.Id && x.IsActive==true).FirstOrDefaultAsync();
                model.IsActive = true;
                model.CreatedAt = priviousRecord.CreatedAt;
                model.CreatedBy = priviousRecord.CreatedBy;
                model.UpdatedBy = loginUserId;
                model.UpdatedAt = DateTime.Now;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.Counter.Update(model);
            }
            return await _unitofWork.Commit();
        }
        public async Task<long> Delete(int Id, int loginUserId)
        {
            Counter model = _unitofWork.Context.Counter.Where(x => x.Id.Equals(Id) && x.IsActive == true).FirstOrDefault();
            if (model != null)
            {
                model.IsActive = false;
                model.UpdatedBy = loginUserId;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.Counter.Update(model);
                return await _unitofWork.Commit();
            }
            return 0;
        }
    }
    public interface ICounterRepository
    {
        Task<List<CounterResponseDTO>> GetALL();
        Task<CounterResponseDTO> GetById(int Id);
        Task<long> Save(CounterRequestDTO modeltDTO, int loginUserId);
        Task<long> Delete(int id, int loinUserId);
        Task<bool> IsExist(string Name);
        Task<List<Counter>> GetALLSync();
    }
}
