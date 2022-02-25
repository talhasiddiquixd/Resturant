using System;
using LinqKit;
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
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IunitOfWork _unitofWork;
        private readonly IMapper _mapper;
        public PermissionRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _unitofWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<Permission>> GetALLSync()
        {
            var dataList = await _unitofWork.Context.Permission.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<IEnumerable<PermissionResponseDTO>> Get()
        {
            var listData = await _unitofWork.Context.Permission.Where(x => x.IsDeleted == false).ToListAsync();
            return _mapper.Map<List<PermissionResponseDTO>>(listData);
        }
        public async Task<(IEnumerable<PermissionResponseDTO> PermissionsList, int recordsCount)> GetByFilters(SearchFilter filters)
        {
            List<Permission> listData = new List<Permission>();

            var predicate = PredicateBuilder.New<Permission>(true);
            bool isPradicate = false;

            predicate = predicate.And(i => i.IsDeleted == false);
            if (!string.IsNullOrEmpty(filters.SearchText))
            {
                predicate = predicate.And(i => i.Name.Contains(filters.SearchText));
                isPradicate = true;
            }
            if (filters.Filters != null)
                foreach (var filter in filters.Filters)
                {
                    if (filter.Logic == "and")
                        predicate.And(DBHelper.BuildPredicate<Permission>(filter.Name, filter.Operator, filter.Value));
                    else if (filter.Logic == "or")
                        predicate.Or(DBHelper.BuildPredicate<Permission>(filter.Name, filter.Operator, filter.Value));
                    else
                        predicate.And(DBHelper.BuildPredicate<Permission>(filter.Name, filter.Operator, filter.Value));
                    isPradicate = true;
                }
            if (isPradicate)
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitofWork.Context.Permission.Where(predicate).Skip(filters.Offset * filters.PageSize).Take(filters.PageSize).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
                _mapper.Map<List<PermissionResponseDTO>>(listData);
            }
            else
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitofWork.Context.Permission.Skip(filters.Offset * filters.PageSize).Take(filters.Offset).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            var recordsCount = await _unitofWork.Context.Permission.Where(x => x.IsDeleted == false).CountAsync();
            (List<PermissionResponseDTO> PermissionsList, int recordsCount) tuple = (_mapper.Map<List<PermissionResponseDTO>>(listData), recordsCount);
            return tuple;
        }
        public async Task<PermissionResponseDTO> Get(int id)
        {
            var Premission = await _unitofWork.Context.Permission.Where(x => x.Id == id && x.IsDeleted == false).SingleOrDefaultAsync();
            return _mapper.Map<PermissionResponseDTO>(Premission);
        }
        public async Task<long> Save(PermissionRequestDTO model, int id)
        {
            Permission Permission = new Permission();
            Permission = _mapper.Map<Permission>(model);
            if (Permission.Id <= 0)
            {
                Permission.CreatedBy = id;
                Permission.CreatedAt = DateTime.UtcNow;
                Permission.IsDeleted = false;
                Permission.IsSynchronized = false;
                Permission.UpdatedBy = null;
                Permission.UpdatedAt = null;
                _unitofWork.Context.Set<Permission>().Add(Permission);

            }
            else
            {
                var PriviousRecord = _unitofWork.Context.Permission.FirstOrDefault(x => x.Id == Permission.Id);
                Permission.CreatedBy = PriviousRecord.CreatedBy;
                Permission.CreatedAt = PriviousRecord.CreatedAt;
                Permission.IsDeleted = false;
                Permission.UpdatedBy = id;
                Permission.UpdatedAt = DateTime.UtcNow;
                _unitofWork.Context.Entry(Permission).State = EntityState.Modified;
                _unitofWork.Context.Permission.Update(Permission);
            }
            return await _unitofWork.Commit();
        }
        public async Task<long> Delete(int Id, int LoginPersonId)
        {
            Permission PermissionDel = _unitofWork.Context.Permission.Where(x => x.Id.Equals(Id) && x.IsDeleted == false).FirstOrDefault();
            if (PermissionDel != null)
            {
                PermissionDel.IsDeleted = true;
                PermissionDel.UpdatedBy = LoginPersonId;
                PermissionDel.UpdatedAt = DateTime.UtcNow;
                _unitofWork.Context.Entry(PermissionDel).State = EntityState.Modified;
                _unitofWork.Context.Permission.Update(PermissionDel);
                return await _unitofWork.Commit();
            }
            return 0;
        }

       
    }
    public interface IPermissionRepository
    {
        Task<IEnumerable<PermissionResponseDTO>> Get();
        Task<(IEnumerable<PermissionResponseDTO> PermissionsList, int recordsCount)> GetByFilters(SearchFilter filters);
        Task<PermissionResponseDTO> Get(int id);
        Task<long> Save(PermissionRequestDTO model, int id);
        Task<long> Delete(int Id, int LoginPerson);
        Task<List<Permission>> GetALLSync();

    }
}
