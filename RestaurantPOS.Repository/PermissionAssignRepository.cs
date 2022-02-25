using AutoMapper;
using LinqKit;
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
    public class PermissionAssignRepository : IPermissionAssignRepository
    {
        private readonly IunitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PermissionAssignRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<PermissionAssign>> GetALLSync()
        {
            var dataList = await _unitOfWork.Context.PermissionAssign.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<long> AssignPrimission(int[] permissionIds, int roleId, int id)
        {
            var prevAssignedPremissions = await _unitOfWork.Context.PermissionAssign.Where(x => x.RoleId == roleId).ToListAsync();
            if (prevAssignedPremissions != null && prevAssignedPremissions.Count > 0)
            {
                _unitOfWork.Context.Set<PermissionAssign>().RemoveRange(prevAssignedPremissions);
                await _unitOfWork.Commit();
            }
            for (int i = 0; i < permissionIds.Length; i++)
            {
                PermissionAssign permissionAssign = new PermissionAssign();
                permissionAssign.PermissionId = permissionIds[i];
                permissionAssign.RoleId = roleId;
                permissionAssign.CreatedBy = id;
                permissionAssign.IsSynchronized = false;
                permissionAssign.CreatedAt = DateTime.Now;
                permissionAssign.IsDeleted = false;
                permissionAssign.UpdatedBy = null;
                permissionAssign.UpdatedAt = null;
                _unitOfWork.Context.Set<PermissionAssign>().Add(permissionAssign);
            }
            return await _unitOfWork.Commit();
        }
        public async Task<List<AssignPermissionResponseDTO>> GetAllRolePremission()
        {
            var AssignPremission = await _unitOfWork.Context.PermissionAssign.Where(x => x.IsDeleted == false).ToListAsync();
            var Assignpermissign = _mapper.Map<List<AssignPermissionResponseDTO>>(AssignPremission);
            foreach (var item in Assignpermissign)
            {
                if (item.RoleId > 0 && item.PermissionId > 0)
                {
                    Role Roledata = _unitOfWork.Context.Role.Where(x => x.Id == item.RoleId && x.IsDeleted == false).FirstOrDefault();
                    if (Roledata != null)
                        item.RoleName = Roledata.Name;
                    Permission Premissiondata = _unitOfWork.Context.Permission.Where(x => x.Id == item.PermissionId && x.IsDeleted == false).FirstOrDefault();
                    if (Premissiondata != null)
                        item.PermissionName = Premissiondata.Name;
                }
            }
            return Assignpermissign;
        }
        public async Task<AssignPermissionResponseDTO> GetRolePremission(int id)
        {
            var AssignPremission = await _unitOfWork.Context.PermissionAssign.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
            var Assignpermissign = _mapper.Map<AssignPermissionResponseDTO>(AssignPremission);
            if (Assignpermissign.RoleId > 0 && Assignpermissign.PermissionId > 0)
            {
                Role Roledata = _unitOfWork.Context.Role.Where(x => x.Id == Assignpermissign.RoleId && x.IsDeleted == false).FirstOrDefault();
                if (Roledata != null)
                    Assignpermissign.RoleName = Roledata.Name;
                Permission Premissiondata = _unitOfWork.Context.Permission.Where(x => x.Id == Assignpermissign.PermissionId && x.IsDeleted == false).FirstOrDefault();
                if (Premissiondata != null)
                    Assignpermissign.PermissionName = Premissiondata.Name;
            }
            return Assignpermissign;
        }
        public async Task<long> DeleteRolePremission(int Id, int LoginPersonId)
        {
            PermissionAssign PermissionAssgnDel = _unitOfWork.Context.PermissionAssign.Where(x => x.Id.Equals(Id) && x.IsDeleted == false).FirstOrDefault();
            if (PermissionAssgnDel != null)
            {
                PermissionAssgnDel.IsDeleted = true;
                PermissionAssgnDel.UpdatedBy = LoginPersonId;
                PermissionAssgnDel.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Context.Entry(PermissionAssgnDel).State = EntityState.Modified;
                _unitOfWork.Context.PermissionAssign.Update(PermissionAssgnDel);
                return await _unitOfWork.Commit();
            }
            return 0;
        }
        public async Task<bool> AlreadyAssined(int RoleId, int PremissionId)
        {
            PermissionAssign model = await _unitOfWork.Context.PermissionAssign.Where(x => x.RoleId == RoleId && x.PermissionId == PremissionId && x.IsDeleted == false).FirstOrDefaultAsync();
            if (model == null)
            {
                return false;
            }
            return true;
        }
        public async Task<List<RolePremissionReponseDTO>> GetAllPremissionOfAllRoles()
        {
            var rollsWithPermission = new List<RolePremissionReponseDTO>();
            var allRoles = await _unitOfWork.Context.Role.ToListAsync();
            foreach (var role in allRoles)
            {
                var PermissionAssignsList = await _unitOfWork.Context.PermissionAssign.Where(X => X.RoleId == role.Id).Include(x => x.Permission).ToListAsync();
                var rolePermissionss = new RolePremissionReponseDTO()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    PermissionResponseDTO = new List<PermissionResponseDTO>()
                };
                foreach (var rolePermission in PermissionAssignsList)
                {
                    PermissionResponseDTO permission = new PermissionResponseDTO();
                    permission = _mapper.Map<PermissionResponseDTO>(rolePermission.Permission);
                    rolePermissionss.PermissionResponseDTO.Add(permission);
                }
                rollsWithPermission.Add(rolePermissionss);
            }
            return rollsWithPermission;
        }
        public async Task<RolePremissionReponseDTO> GetAllPremissionOfSpecificRoles(int id)
        {
            RolePremissionReponseDTO rollsWithPermission = new RolePremissionReponseDTO();
            var allRoles = await _unitOfWork.Context.Role.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (allRoles != null)
            {

                var PermissionAssignsList = await _unitOfWork.Context.PermissionAssign.Where(X => X.RoleId == allRoles.Id).Include(x => x.Permission).ToListAsync();
                var rolePermissionss = new RolePremissionReponseDTO()
                {
                    Id = allRoles.Id,
                    Name = allRoles.Name,
                    Description = allRoles.Description,
                    PermissionResponseDTO = new List<PermissionResponseDTO>()
                };
                foreach (var rolePermission in PermissionAssignsList)
                {
                    PermissionResponseDTO permission = new PermissionResponseDTO();
                    permission = _mapper.Map<PermissionResponseDTO>(rolePermission.Permission);
                    rolePermissionss.PermissionResponseDTO.Add(permission);
                }
                rollsWithPermission = rolePermissionss;

            }
            return rollsWithPermission;
        }
    }
    public interface IPermissionAssignRepository
    {

        Task<long> AssignPrimission(int[] permissionIds, int roleId, int id);
        Task<List<AssignPermissionResponseDTO>> GetAllRolePremission();
        Task<AssignPermissionResponseDTO> GetRolePremission(int id);
        Task<List<RolePremissionReponseDTO>> GetAllPremissionOfAllRoles();
        Task<RolePremissionReponseDTO> GetAllPremissionOfSpecificRoles(int id);
        Task<bool> AlreadyAssined(int RoleId, int PremissionId);
        Task<long> DeleteRolePremission(int Id, int LoginPerson);
        Task<List<PermissionAssign>> GetALLSync();
    }
}
