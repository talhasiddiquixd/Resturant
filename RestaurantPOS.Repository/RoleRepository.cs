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
    public class RoleRepository : IRoleRepository
    {
        private readonly IunitOfWork _unitofWork;
        private readonly IMapper _mapper;
        public RoleRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitofWork = unitOfWork;
        }
        public async Task<List<Role>> GetALLSync()
        {
            var dataList = await _unitofWork.Context.Role.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<long> AssignRoleToUser(UserRoleRequestDTO model, int loginUserId)
        {
            UserRole userRole = new UserRole();
            userRole = _mapper.Map<UserRole>(model);
            if (model.Id <= 0)
            {
                userRole.CreatedBy = loginUserId;
                userRole.CreatedAt = DateTime.UtcNow;
                userRole.IsActive = true;
                userRole.UpdatedBy = null;
                userRole.UpdatedAt = null;
                userRole.IsSynchronized = false;
                _unitofWork.Context.Set<UserRole>().Add(userRole);
            }
            else
            {
                var PriviousRecord = _unitofWork.Context.UserRole.FirstOrDefault(x => x.Id == model.Id);
                userRole.UpdatedBy = loginUserId;
                userRole.UpdatedAt = DateTime.UtcNow;
                userRole.CreatedAt = PriviousRecord.CreatedAt;
                userRole.CreatedBy = PriviousRecord.CreatedBy;
                _unitofWork.Context.Entry(userRole).State = EntityState.Modified;
                _unitofWork.Context.UserRole.Update(userRole);
            }
            return await _unitofWork.Commit();
        }
        public async Task<long> Delete(int Id, int LoginPersonId)
        {
            Role role = _unitofWork.Context.Role.Where(x => x.Id.Equals(Id) && x.IsDeleted == false).FirstOrDefault();
            if (role != null)
            {
                role.IsDeleted = true;
                role.UpdatedBy = LoginPersonId;
                role.UpdatedAt = DateTime.UtcNow;
                _unitofWork.Context.Entry(role).State = EntityState.Modified;
                _unitofWork.Context.Role.Update(role);
                return await _unitofWork.Commit();
            }
            return 0;
        }

        public async Task<long> DeleteRoleAssign(int Id, int LoginPersonId)
        {
            UserRole userRole = _unitofWork.Context.UserRole.Where(x => x.Id.Equals(Id) && x.IsActive == true).FirstOrDefault();
            if (userRole != null)
            {
                userRole.IsActive = false;
                userRole.UpdatedBy = LoginPersonId;
                userRole.UpdatedAt = DateTime.UtcNow;
                _unitofWork.Context.Entry(userRole).State = EntityState.Modified;
                _unitofWork.Context.UserRole.Update(userRole);
                return await _unitofWork.Commit();
            }
            return 0;
        }
        public async Task<List<RoleResponseDTO>> Get()
        {
            var Roles = await _unitofWork.Context.Role.Where(x => x.IsDeleted == false).ToListAsync();
            return _mapper.Map<List<RoleResponseDTO>>(Roles);
        }
        public async Task<RoleResponseDTO> Get(int id)
        {
            var Roles = await _unitofWork.Context.Role.Where(x => x.Id == id && x.IsDeleted == false).SingleOrDefaultAsync();
            return _mapper.Map<RoleResponseDTO>(Roles);
        }
        public async Task<List<UserRoleResponseDTO>> GetAllUserRole()
        {
            var UserRolesList = await _unitofWork.Context.UserRole.Where(x=> x.IsActive==true).ToListAsync();
            var UserRoleResponseList = _mapper.Map<List<UserRoleResponseDTO>>(UserRolesList);
            foreach (var item in UserRoleResponseList)
            {
                if (item.UserId > 0 && item.RoleId > 0)
                {
                    User UserData = _unitofWork.Context.User.Where(x => x.Id == item.UserId && x.IsDeleted == false).FirstOrDefault();
                    if (UserData != null)
                        item.UserName = UserData.Username;
                    Role role = _unitofWork.Context.Role.Where(x => x.Id == item.RoleId && x.IsDeleted == false).FirstOrDefault();
                    if (role != null)
                        item.RoleName = role.Name;
                }
            }
            return UserRoleResponseList;
        }
        public async Task<List<AssignPermissionResponseDTO>> GetPremisionOfRole(int id)
        {
            var Roles = await _unitofWork.Context.PermissionAssign.Where(x => x.RoleId == id && x.IsDeleted == false).ToListAsync();
            var RoleResponse = _mapper.Map<List<AssignPermissionResponseDTO>>(Roles);
            foreach (var item in RoleResponse)
            {
                item.Premission = _mapper.Map<PermissionResponseDTO>(await _unitofWork.Context.Permission.Where(x => x.Id == item.PermissionId && x.IsDeleted == false).FirstOrDefaultAsync());
                var role = await _unitofWork.Context.Role.Where(x => x.Id == item.RoleId && x.IsDeleted == false).FirstOrDefaultAsync();
                item.RoleName = role.Name;
            }
            return RoleResponse;
        }
        public async Task<long> Save(RoleRequestDTO model, int id)
        {
            Role role = new Role();
            role = _mapper.Map<Role>(model);
            if (model.Id <= 0)
            {
                var data = _unitofWork.Context.Role.SingleOrDefault(x => x.Name.Equals(model.Name));
                if (data == null)
                {
                    role.IsDeleted = false;
                    role.CreatedBy = id;
                    role.CreatedAt = DateTime.Now;
                    role.IsSynchronized = false;
                    role.UpdatedBy = null;
                    role.UpdatedAt = null;
                    _unitofWork.Context.Set<Role>().Add(role);
                }
            }
            else
            {
                var PriviousRecord = _unitofWork.Context.Role.FirstOrDefault(x => x.Id == model.Id);
                role.UpdatedBy = id;
                role.UpdatedAt = DateTime.UtcNow;
                _unitofWork.Context.Entry(role).State = EntityState.Modified;
                _unitofWork.Context.Role.Update(role);
            }
            return await _unitofWork.Commit();
        }
    }
    public interface IRoleRepository
    {
        Task<List<RoleResponseDTO>> Get();
        Task<RoleResponseDTO> Get(int id);
        Task<List<AssignPermissionResponseDTO>> GetPremisionOfRole(int id);
        Task<List<UserRoleResponseDTO>> GetAllUserRole();
        Task<long> Save(RoleRequestDTO model, int id);
        Task<long> AssignRoleToUser(UserRoleRequestDTO model, int loginUserId);
        Task<long> Delete(int Id, int LoginPerson);
        Task<long> DeleteRoleAssign(int Id, int LoginPerson);
        Task<List<Role>> GetALLSync();
    }
}
