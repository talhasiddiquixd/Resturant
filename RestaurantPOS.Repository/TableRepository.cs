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
    public class TableRepository : ITableRepository
    {
        private readonly IMapper _mapper;
        private readonly IunitOfWork _unitofWork;
        public TableRepository(IunitOfWork unitofwork, IMapper mapper)
        {
            _mapper = mapper;
            _unitofWork = unitofwork;
        }
        public async Task<List<Table>> GetALLSync()
        {
            var dataList = await _unitofWork.Context.Table.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<List<TableResponseDTO>> GetALL()
        {
            var data = await _unitofWork.Context.Table.Where(x => x.IsDeleted == false).ToListAsync();
            var model = _mapper.Map<List<TableResponseDTO>>(data);
            try
            {
                foreach (var item in model)
                {
                    if (item.HallId > 0)
                    {
                        try
                        {
                            Hall Halldata = _unitofWork.Context.Hall.Where(x => x.Id == item.HallId && x.IsDeleted == false).FirstOrDefault();
                            if (Halldata != null)
                                item.HallName = Halldata.Name;
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {

                throw ex ;
            }
        }
        public async Task<List<TableResponseDTO>> GetALLFreeTable()
        {
            var data = await _unitofWork.Context.Table.Where(x => x.IsDeleted == false && x.IsAssigned == 0).ToListAsync();
            try
            {
                var model = _mapper.Map<List<TableResponseDTO>>(data);
                foreach (var item in model)
                {
                    if (item.HallId > 0)
                    {
                        Hall Halldata = _unitofWork.Context.Hall.Where(x => x.Id == item.HallId && x.IsDeleted == false).FirstOrDefault();
                        if (Halldata != null)
                            item.HallName = Halldata.Name;
                    }
                }
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<TableResponseDTO> GetById(int Id)
        {
            var data = await _unitofWork.Context.Table.Where(x => x.Id == Id && x.IsDeleted == false).FirstOrDefaultAsync();
            var model = _mapper.Map<TableResponseDTO>(data);
            if (model.HallId > 0)
            {
                Hall Halldata = _unitofWork.Context.Hall.Where(x => x.Id == model.HallId && x.IsDeleted == false).FirstOrDefault();
                if (Halldata != null)
                    model.HallName = Halldata.Name;
            }
            return model;
        }
        public async Task<bool> IsExist(string Name)
        {
            var data = await _unitofWork.Context.Table.Where(x => x.Name == Name && x.IsDeleted == false).FirstOrDefaultAsync();
            if (data == null)
                return true;
            return false;
        }
        public async Task<long> Save(TableRequestDTO modelDTO, int loginUserId)
        {
            var model = _mapper.Map<Table>(modelDTO);
            try
            {
                if (model.Id <= 0)
                {
                    model.CreatedBy = loginUserId;
                    model.CreatedAt = DateTime.Now;
                    model.IsAssigned = 0;
                    model.IsSynchronized = false;
                    _unitofWork.Context.Set<Table>().Add(model);
                }
                else
                {
                    var priviousRecord = await _unitofWork.Context.Table.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                    model.UpdatedBy = loginUserId;
                    model.UpdatedAt = DateTime.Now;
                    model.CreatedAt = priviousRecord.CreatedAt;
                    model.CreatedBy = priviousRecord.CreatedBy;
                    _unitofWork.Context.Entry(model).State = EntityState.Modified;
                    _unitofWork.Context.Table.Update(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return await _unitofWork.Commit();
        }
        public async Task<long> Delete(int Id, int loginUserId)
        {
            Table model = _unitofWork.Context.Table.Where(x => x.Id.Equals(Id) && x.IsDeleted == false).FirstOrDefault();
            if (model != null)
            {
                model.IsDeleted = true;
                model.UpdatedBy = loginUserId;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.Table.Update(model);
                return await _unitofWork.Commit();
            }
            return 0;
        }

        public async Task<long> SaveStatus(int? tableId, int status)
        {
            Table model = _unitofWork.Context.Table.Where(x => x.Id.Equals(tableId) && x.IsDeleted == false).FirstOrDefault();
            if (model != null)
            {
                model.IsAssigned = status;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.Table.Update(model);
                return await _unitofWork.Commit();
            }
            return 0;
        }
    }
    public interface ITableRepository
    {
        Task<List<TableResponseDTO>> GetALL();
        Task<TableResponseDTO> GetById(int Id);
        Task<List<TableResponseDTO>> GetALLFreeTable();
        Task<long> Save(TableRequestDTO modeltDTO, int loginUserId);
        Task<long> SaveStatus(int? tableId, int status);
        Task<long> Delete(int id, int loinUserId);
        Task<bool> IsExist(string Name);
        Task<List<Table>> GetALLSync();
    }
}
