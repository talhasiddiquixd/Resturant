using System;
using AutoMapper;
using System.Linq;
using RestaurantPOS.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Helpers.ResponseDTO;
namespace RestaurantPOS.Repository
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly IunitOfWork _unitofWork;
        private readonly IMapper _mapper;
        public AttachmentRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _unitofWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<Attachment>> GetALLSync()
        {
            var dataList = await _unitofWork.Context.Attachment.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<List<AttachmentResponseDTO>> Get(string Path)
        {
            var data = await _unitofWork.Context.Attachment.ToListAsync();
           var Attachments= _mapper.Map<List<AttachmentResponseDTO>>(data);
            if (Attachments != null)
            {
                foreach (var item in Attachments)
                {
                    item.FileToUpLoad = Path + item.FileToUpLoad;
                }
                    return Attachments; 
            }
            else
                return null;
        }
        public async Task<AttachmentResponseDTO> GetById(int id, string Path)
        {

            try
            {
                Attachment data = await _unitofWork.Context.Attachment.Where(x => x.Id == id).FirstOrDefaultAsync();
                AttachmentResponseDTO Attachments = _mapper.Map<AttachmentResponseDTO>(data);
                if (Attachments != null)
                {
                    Attachments.FileToUpLoad = Path + Attachments.FileToUpLoad;
                    return Attachments;
                }
                else
                    return null;
            }
            catch (Exception ex )
            {
                throw ex;
            }
        }
        public async  Task<long> Save(Attachment Attachment)
        {
            try
            {
                if (Attachment.Id <= 0)
                {
                    Attachment.IsSynchronized = false;
                    _unitofWork.Context.Set<Attachment>().Add(Attachment);
                }
                else
                {
                    var priviousRecord = await _unitofWork.Context.Attachment.Where(x => x.Id == Attachment.Id).FirstOrDefaultAsync();
                    _unitofWork.Context.Entry(Attachment).State = EntityState.Modified;
                    _unitofWork.Context.Attachment.Update(Attachment);
                }
                await _unitofWork.Commit();
                return Attachment.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
    public interface IAttachmentRepository
    {
        Task<List<AttachmentResponseDTO>> Get(string Path);
        Task<AttachmentResponseDTO> GetById(int id, string Path);
        Task<long> Save(Attachment Attachment);
        Task<List<Attachment>> GetALLSync();
    }
}
