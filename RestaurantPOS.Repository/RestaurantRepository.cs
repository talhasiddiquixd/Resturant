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
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly IunitOfWork _unitofWork;
        private readonly IMapper _mapper;
        public RestaurantRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _unitofWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<Restaurant>> GetALLSync()
        {
            var dataList = await _unitofWork.Context.Restaurant.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<long> Delete(int Id)
        {
            Restaurant model = await _unitofWork.Context.Restaurant.Where(x => x.Id==Id && x.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                model.IsDeleted = true;
                model.IsSynchronized = false;
                _unitofWork.Context.Entry(model).State = EntityState.Modified;
                _unitofWork.Context.Restaurant.Update(model);
                return await _unitofWork.Commit();
            }
            return 0;
        }
        public async Task<List<RestaurantResposeDTO>> Get(string Path)
        {
            var listData = await _unitofWork.Context.Restaurant.ToListAsync();
            if (listData != null && listData.Count > 0)
            {
                var listResponseDTO= _mapper.Map<List<RestaurantResposeDTO>>(listData);
                return listResponseDTO;
            }
            return null;
        }
        public async Task<RestaurantResposeDTO> Get(int id, string Path)
        {
            var Premission = await _unitofWork.Context.Restaurant.Where(x => x.Id == id).SingleOrDefaultAsync();
            var RestaurantResposeDTO= _mapper.Map<RestaurantResposeDTO>(Premission);
            return RestaurantResposeDTO;
        }
        public async Task<long> Save(RestaurantRequestDTO model, int id)
        {
            Restaurant Restaurant = new Restaurant();
            try
            {
                Restaurant = _mapper.Map<Restaurant>(model);
                if (Restaurant.Id <= 0)
                {
                    Restaurant.IsSynchronized = false;
                    _unitofWork.Context.Set<Restaurant>().Add(Restaurant);
                }
                else
                {
                    var previousRecord = _unitofWork.Context.Restaurant.FirstOrDefault(x => x.Id == Restaurant.Id);
                    previousRecord.Email = !string.IsNullOrEmpty(model.Email) ? model.Email : previousRecord.Email;
                    previousRecord.WebSite = !string.IsNullOrEmpty(model.WebSite) ? model.WebSite : previousRecord.WebSite;
                    previousRecord.Address = !string.IsNullOrEmpty(model.Address) ? model.Address : previousRecord.Address;
                    previousRecord.ContactNo = !string.IsNullOrEmpty(model.ContactNo) ? model.ContactNo : previousRecord.ContactNo;
                    previousRecord.AttachmentId = model.AttachmentId > 0 ? model.AttachmentId : previousRecord.AttachmentId;
                    previousRecord.RestaurantName = !string.IsNullOrEmpty(model.RestaurantName) ? model.RestaurantName : previousRecord.RestaurantName;
                    previousRecord.AttachmentPath = !string.IsNullOrEmpty(model.AttachmentPath) ? model.AttachmentPath : previousRecord.AttachmentPath;
                    _unitofWork.Context.Entry(previousRecord).State = EntityState.Modified;
                    _unitofWork.Context.Restaurant.Update(previousRecord);
                }
                return await _unitofWork.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public interface IRestaurantRepository
    {
        Task<List<RestaurantResposeDTO>> Get(string Path);
        Task<RestaurantResposeDTO> Get(int id, string Path);
        Task<long> Save(RestaurantRequestDTO model, int id);
        Task<long> Delete(int Id);
        Task<List<Restaurant>> GetALLSync();
    }
}
