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
    public class AddOnsRepository : IAddOnsRepository
    {
        private readonly IunitOfWork _unitofWork;
        private readonly IMapper _mapper;
        public AddOnsRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _unitofWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<AddOns>> GetALLSync()
        {
            var dataList = await _unitofWork.Context.AddOns.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<IEnumerable<AddOnsResponseDTO>> Get()
        {
            var listData = await _unitofWork.Context.AddOns.Where(x => x.IsDeleted == false).ToListAsync();
            if (listData != null && listData.Count > 0)
            {
                return _mapper.Map<List<AddOnsResponseDTO>>(listData);
            }
            return null;
        }
        public async Task<IEnumerable<AssignAddOnsResponseDTO>> GetAssignAddOns()
        {
            var listData = await _unitofWork.Context.AddOnsAssign.ToListAsync();
            var model = _mapper.Map<List<AssignAddOnsResponseDTO>>(listData);
            if (listData != null)
            {
                foreach (var item in model)
                {
                    var foodVarient = await _unitofWork.Context.FoodVarient.Where(x => x.Id == item.FoodVarientId).Select(x => x.Name).SingleOrDefaultAsync();
                    if (foodVarient != null)
                    {
                        item.FoodVarientName = foodVarient;
                    }

                    var AddOns = await _unitofWork.Context.AddOns.Where(x => x.Id == item.AddOnsId).SingleOrDefaultAsync();
                    if (AddOns != null)
                    {
                        item.Price = AddOns.Price;
                        item.AddOnsName = AddOns.Name;
                    }
                }
            }
            return model;
        }
        public async Task<(IEnumerable<AddOnsResponseDTO> PermissionsList, int recordsCount)> GetByFilters(SearchFilter filters)
        {
            List<AddOns> listData = new List<AddOns>();
            var predicate = PredicateBuilder.New<AddOns>(true);
            bool isPradicate = false;
            if (!string.IsNullOrEmpty(filters.SearchText))
            {
                predicate = predicate.And(i => i.Name.Contains(filters.SearchText));
                isPradicate = true;
            }
            if (filters.Filters != null)
                foreach (var filter in filters.Filters)
                {
                    if (filter.Logic == "and")
                        predicate.And(DBHelper.BuildPredicate<AddOns>(filter.Name, filter.Operator, filter.Value));
                    else if (filter.Logic == "or")
                        predicate.Or(DBHelper.BuildPredicate<AddOns>(filter.Name, filter.Operator, filter.Value));
                    else
                        predicate.And(DBHelper.BuildPredicate<AddOns>(filter.Name, filter.Operator, filter.Value));
                    isPradicate = true;
                }
            if (isPradicate)
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitofWork.Context.AddOns.Where(predicate).Skip(filters.Offset * filters.PageSize).Take(filters.PageSize).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
                _mapper.Map<List<AddOnsResponseDTO>>(listData);
            }
            else
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitofWork.Context.AddOns.Skip(filters.Offset * filters.PageSize).Take(filters.Offset).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            var recordsCount = await _unitofWork.Context.Permission.Where(x => x.IsDeleted == false).CountAsync();
            (List<AddOnsResponseDTO> PermissionsList, int recordsCount) tuple = (_mapper.Map<List<AddOnsResponseDTO>>(listData), recordsCount);
            return tuple;
        }
        public async Task<AddOnsResponseDTO> Get(int id)
        {
            var Premission = await _unitofWork.Context.AddOns.Where(x => x.Id == id && x.IsDeleted == false).SingleOrDefaultAsync();
            return _mapper.Map<AddOnsResponseDTO>(Premission);
        }
        public async Task<AssignAddOnsResponseDTO> GetById(int Id)
        {
            var response = await _unitofWork.Context.AddOnsAssign.Where(x => x.Id == Id).SingleOrDefaultAsync();
            var AddOnsAssign = _mapper.Map<AssignAddOnsResponseDTO>(response);
            var foodVarient = await _unitofWork.Context.FoodVarient.Where(x => x.Id == response.FoodVarientId).Select(x => x.Name).SingleOrDefaultAsync();
            AddOnsAssign.FoodVarientName = foodVarient;

            return AddOnsAssign;
        }
        public async Task<long> Save(AddOnsRequestDTO model, int id)
        {
            AddOns addOns = new AddOns();
            addOns = _mapper.Map<AddOns>(model);
            if (addOns.Id <= 0)
            {
                addOns.IsDeleted = false;
                addOns.IsSynchronized = false;
                _unitofWork.Context.Set<AddOns>().Add(addOns);
            }
            else
            {
                addOns.IsDeleted = false;
                var PriviousRecord = _unitofWork.Context.AddOns.FirstOrDefault(x => x.Id == addOns.Id && x.IsDeleted == false);
                _unitofWork.Context.Entry(addOns).State = EntityState.Modified;
                _unitofWork.Context.AddOns.Update(addOns);
            }
            return await _unitofWork.Commit();
        }
        public async Task<long> Delete(int Id, int LoginPersonId)
        {
            AddOns PermissionDel = _unitofWork.Context.AddOns.Where(x => x.Id.Equals(Id) && x.IsDeleted == false).FirstOrDefault();
            if (PermissionDel != null)
            {
                PermissionDel.IsDeleted = true;
                _unitofWork.Context.Entry(PermissionDel).State = EntityState.Modified;
                _unitofWork.Context.AddOns.Update(PermissionDel);
                return await _unitofWork.Commit();
            }
            return 0;
        }
        public async Task<long> AssignAddOns(AssignAddOnsRequestDTO model, int id)
        {
            AddOnsAssign addonsAssign = new AddOnsAssign();
            addonsAssign = _mapper.Map<AddOnsAssign>(model);
            if (addonsAssign.Id <= 0)
            {
                addonsAssign.IsSynchronized = false;
                _unitofWork.Context.Set<AddOnsAssign>().Add(addonsAssign);
            }
            else
            {
                var PriviousRecord = _unitofWork.Context.AddOnsAssign.FirstOrDefault(x => x.Id == addonsAssign.Id);
                addonsAssign.IsSynchronized = PriviousRecord.IsSynchronized;
                _unitofWork.Context.Entry(addonsAssign).State = EntityState.Modified;
                _unitofWork.Context.AddOnsAssign.Update(addonsAssign);
            }
            return await _unitofWork.Commit();
        }
    }
    public interface IAddOnsRepository
    {
        Task<IEnumerable<AddOnsResponseDTO>> Get();
        Task<(IEnumerable<AddOnsResponseDTO> PermissionsList, int recordsCount)> GetByFilters(SearchFilter filters);
        Task<AddOnsResponseDTO> Get(int id);
        Task<long> Save(AddOnsRequestDTO model, int id);
        Task<long> AssignAddOns(AssignAddOnsRequestDTO model, int id);
        Task<long> Delete(int Id, int LoginPerson);
        Task<List<AddOns>> GetALLSync();
        Task<AssignAddOnsResponseDTO> GetById(int Id);
        Task<IEnumerable<AssignAddOnsResponseDTO>> GetAssignAddOns();
    }
}
