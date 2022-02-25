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
using Microsoft.Extensions.Configuration;
using RestaurantPOS.Helpers.UtilityHelper;
using System.Diagnostics.CodeAnalysis;

namespace RestaurantPOS.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IunitOfWork _unitofWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public UserRepository(IunitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitofWork = unitOfWork;
            _mapper = mapper;
            _config = config;
        }
        public async Task<IEnumerable<UserResponseDTO>> Get()
        {
            var UserList = await _unitofWork.Context.User.Where(x => x.IsDeleted == false).Include(x => x.UserRoles).ToListAsync();
            var UserResponseList = _mapper.Map<List<UserResponseDTO>>(UserList);
            foreach (var item in UserResponseList)
            {
                if (item.UserAttachmentId > 0)
                {
                    if (item.AssignedType == 0)
                    {
                        item.AssignesTypeName = "Admin";
                    }
                    else if (item.AssignedType == 1)
                    {
                        var Kitchen = await _unitofWork.Context.Kitchen.Where(x => x.Id == item.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (Kitchen != null)
                            item.AssignesTypeName = Kitchen.Name;
                    }
                    else if (item.AssignedType == 2)
                    {
                        var Hall = await _unitofWork.Context.Hall.Where(x => x.Id == item.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (Hall != null)
                            item.AssignesTypeName = Hall.Name;
                    }
                    else if (item.AssignedType == 3)
                    {
                        var counter = await _unitofWork.Context.Counter.Where(x => x.Id == item.AssignedRole && x.IsActive == true).SingleOrDefaultAsync();
                        if (counter != null)
                            item.AssignesTypeName = counter.Name;
                    }
                    else
                    {
                        item.AssignesTypeName = "No Role";
                    }

                    foreach (var role in item.UserRoles)
                    {
                        var Role = await _unitofWork.Context.Role.Where(x => x.Id == role.Id && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (Role != null)
                            role.RoleName = Role.Name;
                        role.UserName = item.Username;
                    }
                }
            }
            return UserResponseList;
        }
        public async Task<IEnumerable<UserResponseDTO>> GetAll()
        {
            var UserList = await _unitofWork.Context.User.Where(x => x.IsDeleted == false).ToListAsync();
            var UserResponseList = _mapper.Map<List<UserResponseDTO>>(UserList);
            foreach (var item in UserResponseList)
            {
                if (item.UserAttachmentId > 0)
                {
                    if (item.AssignedType == 0)
                    {
                        item.AssignesTypeName = "Admin";

                    }
                    else if (item.AssignedType == 1)
                    {
                        var Kitchen = await _unitofWork.Context.Kitchen.Where(x => x.Id == item.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (Kitchen != null)
                            item.AssignesTypeName = Kitchen.Name;
                    }
                    else if (item.AssignedType == 2)
                    {
                        var Hall = await _unitofWork.Context.Hall.Where(x => x.Id == item.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (Hall != null)
                            item.AssignesTypeName = Hall.Name;
                    }
                    else if (item.AssignedType == 3)
                    {
                        var counter = await _unitofWork.Context.Counter.Where(x => x.Id == item.AssignedRole && x.IsActive == true).SingleOrDefaultAsync();
                        if (counter != null)
                            item.AssignesTypeName = counter.Name;
                    }
                    else
                    {
                        item.AssignesTypeName = "No Role";
                    }
                }
            }
            return UserResponseList;
        }

        public async Task<long> GetUsersCount()
        {
            return await _unitofWork.Context.User.Where(x => x.IsDeleted == false && x.AssignedType != 0 && x.AssignedRole != 0).CountAsync();
        }

        public async Task<(IEnumerable<UserResponseDTO> UsersList, int recordsCount)> GetByFilters(SearchFilter filters)
        {
            List<User> listData = new List<User>();

            var predicate = PredicateBuilder.New<User>(true);
            bool isPradicate = false;

            predicate = predicate.And(i => i.IsDeleted == false);
            if (!string.IsNullOrEmpty(filters.SearchText))
            {
                predicate = predicate.And(i => i.Username.Contains(filters.SearchText));
                isPradicate = true;
            }
            if (filters.Filters != null)
                foreach (var filter in filters.Filters)
                {
                    if (filter.Logic == "and")
                        predicate.And(DBHelper.BuildPredicate<User>(filter.Name, filter.Operator, filter.Value));
                    else if (filter.Logic == "or")
                        predicate.Or(DBHelper.BuildPredicate<User>(filter.Name, filter.Operator, filter.Value));
                    else
                        predicate.And(DBHelper.BuildPredicate<User>(filter.Name, filter.Operator, filter.Value));
                    isPradicate = true;
                }
            if (isPradicate)
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitofWork.Context.User.Where(predicate).Skip(filters.Offset * filters.PageSize).Take(filters.PageSize).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            else
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitofWork.Context.User.Skip(filters.Offset * filters.PageSize).Take(filters.Offset).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            var recordsCount = await _unitofWork.Context.User.Where(x => x.IsDeleted == false).CountAsync();
            (List<UserResponseDTO> UseresList, int recordsCount) tuple = (_mapper.Map<List<UserResponseDTO>>(listData), recordsCount);
            foreach (var item in tuple.UseresList)
            {
                if (item.UserAttachmentId > 0)
                {
                    if (item.AssignedType == 0)
                    {
                        item.AssignesTypeName = "Admin";

                    }
                    else if (item.AssignedType == 1)
                    {
                        var Kitchen = await _unitofWork.Context.Kitchen.Where(x => x.Id == item.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (Kitchen != null)
                            item.AssignesTypeName = Kitchen.Name;
                    }
                    else if (item.AssignedType == 2)
                    {
                        var Hall = await _unitofWork.Context.Hall.Where(x => x.Id == item.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (Hall != null)
                            item.AssignesTypeName = Hall.Name;
                    }
                    else if (item.AssignedType == 3)
                    {
                        var counter = await _unitofWork.Context.Counter.Where(x => x.Id == item.AssignedRole && x.IsActive == true).SingleOrDefaultAsync();
                        if (counter != null)
                            item.AssignesTypeName = counter.Name;
                    }
                    else
                    {
                        item.AssignesTypeName = "No Role";
                    }
                }
            }
            return tuple;
        }
        public async Task<UserResponseDTO> Get(int id)
        {
            var SpecifcUser = await _unitofWork.Context.User.Where(x => x.Id == id && x.IsDeleted == false).SingleOrDefaultAsync();
            var SpecificUserResponse = _mapper.Map<UserResponseDTO>(SpecifcUser);

            if (SpecificUserResponse.AssignedType != 0)
            {
                if (SpecificUserResponse.AssignedType == 1)
                {
                    var Kitchen = await _unitofWork.Context.Kitchen.Where(x => x.Id == SpecificUserResponse.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
                    if (Kitchen != null)
                        SpecificUserResponse.AssignesTypeName = Kitchen.Name;
                }
                else if (SpecificUserResponse.AssignedType == 2)
                {
                    var Hall = await _unitofWork.Context.Hall.Where(x => x.Id == SpecificUserResponse.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
                    if (Hall != null)
                        SpecificUserResponse.AssignesTypeName = Hall.Name;
                }
                else if (SpecificUserResponse.AssignedType == 3)
                {
                    var counter = await _unitofWork.Context.Counter.Where(x => x.Id == SpecificUserResponse.AssignedRole && x.IsActive == true).SingleOrDefaultAsync();
                    if (counter != null)
                        SpecificUserResponse.AssignesTypeName = counter.Name;
                }
                else
                {
                    SpecificUserResponse.AssignesTypeName = "No Role";
                }
            }
            else
            {
                SpecificUserResponse.AssignesTypeName = "Admin";

            }
            return SpecificUserResponse;
        }
        public async Task<bool> IsExist(string Email)
        {
            var UserExist = await _unitofWork.Context.User.FirstOrDefaultAsync(x => x.Email.Equals(Email));
            if (UserExist == null)
                return false;
            return true;
        }
        public async Task<long> Save(UserRequestDTO model, int id)
        {
            User User = new User();
            User = _mapper.Map<User>(model);
            if (User.Id <= 0)
            {
                User.Password = GeneralHelper.Encrypt(User.Password);
                var data = _unitofWork.Context.User.FirstOrDefault(x => x.Email.Equals(User.Email));
                if (data == null)
                {
                    User.CreatedBy = id;
                    User.CreatedOn = DateTime.UtcNow;
                    User.IsDeleted = false;
                    User.UpdatedBy = null;
                    User.UpdatedOn = null;
                    User.AssignedRole = null;
                    User.AssignedType = null;
                    User.IsSynchronized = false;//IsSynchronize
                    _unitofWork.Context.Set<User>().Add(User);
                }
            }
            else
            {
                var PriviousRecord = _unitofWork.Context.User.FirstOrDefault(x => x.Id == User.Id);
                User.CreatedBy = PriviousRecord.CreatedBy;
                User.CreatedOn = PriviousRecord.CreatedOn;
                User.Password = PriviousRecord.Password;
                User.IsDeleted = false;
                User.UpdatedBy = id;
                User.UpdatedOn = DateTime.UtcNow;
                _unitofWork.Context.Entry(User).State = EntityState.Modified;
                _unitofWork.Context.User.Update(User);
            }
            return await _unitofWork.Commit();
        }
        public async Task<long> Delete(int Id, int LoginPersonId)
        {
            User UserDelete = _unitofWork.Context.User.Where(x => x.Id.Equals(Id) && x.IsDeleted == false).FirstOrDefault();
            if (UserDelete != null)
            {
                UserDelete.IsDeleted = true;
                UserDelete.UpdatedBy = LoginPersonId;
                UserDelete.UpdatedOn = DateTime.UtcNow;
                _unitofWork.Context.Entry(UserDelete).State = EntityState.Modified;
                _unitofWork.Context.User.Update(UserDelete);
                return await _unitofWork.Commit();
            }
            return 0;
        }
        public async Task<User> GetByUserId(int Id)
        {
            return await _unitofWork.Context.User.Where(x => x.Id == Id).FirstOrDefaultAsync();
        }
        public async Task<UserResponseDTO> ValidateAndLogin(UserLoginRequestDTO model)
        {
            var password = GeneralHelper.Encrypt(model.Password);
            var ValidUsers = await _unitofWork.Context.User.Where(x => x.Email == model.Email && x.Password == password).FirstOrDefaultAsync();
            var ValidUser = _mapper.Map<UserResponseDTO>(ValidUsers);
            if (ValidUser != null)
            {
                if (ValidUser.AssignedType != 0)
                {
                    if (ValidUser.AssignedType == 1)
                    {
                        var Kitchen = await _unitofWork.Context.Kitchen.Where(x => x.Id == ValidUser.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (Kitchen != null)
                            ValidUser.AssignesTypeName = Kitchen.Name;
                    }
                    else if (ValidUser.AssignedType == 2)
                    {
                        var Hall = await _unitofWork.Context.Hall.Where(x => x.Id == ValidUser.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (Hall != null)
                            ValidUser.AssignesTypeName = Hall.Name;
                    }
                    else if (ValidUser.AssignedType == 3)
                    {
                        var counter = await _unitofWork.Context.Counter.Where(x => x.Id == ValidUser.AssignedRole && x.IsActive == true).SingleOrDefaultAsync();
                        if (counter != null)
                            ValidUser.AssignesTypeName = counter.Name;
                    }
                    else
                    {
                        ValidUser.AssignesTypeName = "No Role";
                    }
                }
                else
                {
                    ValidUser.AssignesTypeName = "Admin";

                }
                return ValidUser;
            }
            else
                return null;
        }

        public async Task<User> GetByEmail(string Email)
        {
            var ValidUser = await _unitofWork.Context.User.Where(x => x.Email == Email).FirstOrDefaultAsync();
            if (ValidUser != null)
                return ValidUser;
            else
                return null;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUnassignUsers()
        {
            var UserList = await _unitofWork.Context.User.Where(x => x.IsDeleted == false && x.AssignedRole == null && x.AssignedType == null).ToListAsync();
            var UserResponseList = _mapper.Map<List<UserResponseDTO>>(UserList);
            //foreach (var item in UserResponseList)
            //{
            //    if (item.UserAttachmentId > 0)
            //    {
            //        if (item.AssignedType == 0)
            //        {
            //            item.AssignesTypeName = "Admin";

            //        }
            //        else if (item.AssignedType == 1)
            //        {
            //            var Kitchen = await _unitofWork.Context.Kitchen.Where(x => x.Id == item.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
            //            if (Kitchen != null)
            //                item.AssignesTypeName = Kitchen.Name;
            //        }
            //        else if (item.AssignedType == 2)
            //        {
            //            var Hall = await _unitofWork.Context.Hall.Where(x => x.Id == item.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
            //            if (Hall != null)
            //                item.AssignesTypeName = Hall.Name;
            //        }
            //        else if (item.AssignedType == 3)
            //        {
            //            var counter = await _unitofWork.Context.Counter.Where(x => x.Id == item.AssignedRole && x.IsActive == true).SingleOrDefaultAsync();
            //            if (counter != null)
            //                item.AssignesTypeName = counter.Name;
            //        }
            //        else
            //        {
            //            item.AssignesTypeName = "No Role";
            //        }

            //        foreach (var role in item.UserRoles)
            //        {
            //            var Role = await _unitofWork.Context.Role.Where(x => x.Id == role.Id && x.IsDeleted == false).SingleOrDefaultAsync();
            //            if (Role != null)
            //                role.RoleName = Role.Name;
            //            role.UserName = item.Username;
            //        }
            //    }
            //}
            return UserResponseList;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllByRoleType(int AssignedTypes)
        {
            var UserList = await _unitofWork.Context.User.Where(x => x.IsDeleted == false && x.AssignedType == AssignedTypes).Include(x => x.UserRoles).ToListAsync();
            var UserResponseList = _mapper.Map<List<UserResponseDTO>>(UserList);
            foreach (var item in UserResponseList)
            {
                if (item.UserAttachmentId > 0)
                {
                    if (item.AssignedType == 0)
                    {
                        item.AssignesTypeName = "Admin";

                    }
                    else if (item.AssignedType == 1)
                    {
                        var Kitchen = await _unitofWork.Context.Kitchen.Where(x => x.Id == item.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (Kitchen != null)
                            item.AssignesTypeName = Kitchen.Name;
                    }
                    else if (item.AssignedType == 2)
                    {
                        var Hall = await _unitofWork.Context.Hall.Where(x => x.Id == item.AssignedRole && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (Hall != null)
                            item.AssignesTypeName = Hall.Name;
                    }
                    else if (item.AssignedType == 3)
                    {
                        var counter = await _unitofWork.Context.Counter.Where(x => x.Id == item.AssignedRole && x.IsActive == true).SingleOrDefaultAsync();
                        if (counter != null)
                            item.AssignesTypeName = counter.Name;
                    }
                    else
                    {
                        item.AssignesTypeName = "No Role";
                    }

                    foreach (var role in item.UserRoles)
                    {
                        var Role = await _unitofWork.Context.Role.Where(x => x.Id == role.Id && x.IsDeleted == false).SingleOrDefaultAsync();
                        if (Role != null)
                            role.RoleName = Role.Name;
                        role.UserName = item.Username;
                    }
                }
            }
            return UserResponseList;
        }

        public async Task<long> UpdateFilePath()
        {
            try
            {
                //// User FilePath Update...
                List<User> FileList = await _unitofWork.Context.User.Where(x => x.IsDeleted == false).ToListAsync();
                foreach (var item in FileList)
                {
                    var filePath = item.AttachmentPath;
                    if (filePath != null)
                    {
                        var path = filePath.Split("/");
                        string BaseUrl = _config["Utility:APIBaseURL"];
                        var UpdatedPath = path[3] + "/" + path[4] + "/" + path[5];
                        item.AttachmentPath = BaseUrl + UpdatedPath;
                        _unitofWork.Context.Entry(item).State = EntityState.Modified;
                        _unitofWork.Context.User.Update(item);
                        await _unitofWork.Commit();
                    }
                }

                //// User FilePath Update...
                List<FoodCategory> FoodCategoryFileList = await _unitofWork.Context.FoodCategory.Where(x => x.IsDeleted == false).ToListAsync();
                foreach (var item in FoodCategoryFileList)
                {
                    var filePath = item.AttachmentPath;
                    if (filePath != null)
                    {
                        var path = filePath.Split("/");
                        string BaseUrl = _config["Utility:APIBaseURL"];
                        var UpdatedPath = path[3] + "/" + path[4] + "/" + path[5];
                        item.AttachmentPath = BaseUrl + UpdatedPath;
                        _unitofWork.Context.Entry(item).State = EntityState.Modified;
                        _unitofWork.Context.FoodCategory.Update(item);
                        await _unitofWork.Commit();
                    }
                }

                //// FoodItem FilePath Update...
                List<FoodItem> FoodItemFileList = await _unitofWork.Context.FoodItem.ToListAsync();
                foreach (var item in FoodItemFileList)
                {
                    var filePath = item.AttachmentPath;
                    if (filePath != null)
                    {
                        var path = filePath.Split("/");
                        string BaseUrl = _config["Utility:APIBaseURL"];
                        var UpdatedPath = path[3] + "/" + path[4] + "/" + path[5];
                        item.AttachmentPath = BaseUrl + UpdatedPath;
                        _unitofWork.Context.Entry(item).State = EntityState.Modified;
                        _unitofWork.Context.FoodItem.Update(item);
                        await _unitofWork.Commit();
                    }
                }

                //// Restaurant FilePath Update...
                List<Restaurant> RestaurantFileList = await _unitofWork.Context.Restaurant.ToListAsync();
                foreach (var item in RestaurantFileList)
                {
                    var filePath = item.AttachmentPath;
                    if (filePath != null)
                    {
                        var path = filePath.Split("/");
                        string BaseUrl = _config["Utility:APIBaseURL"];
                        var UpdatedPath = path[3] + "/" + path[4] + "/" + path[5];
                        item.AttachmentPath = BaseUrl + UpdatedPath;
                        _unitofWork.Context.Entry(item).State = EntityState.Modified;
                        _unitofWork.Context.Restaurant.Update(item);
                        await _unitofWork.Commit();
                    }
                }
                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<long> SyncData()
        {
            #region Add-on Table
            try
            {
                var addons = _unitofWork.Context.AddOns.Where(x => x.IsSynchronized == false).ToList();
                addons.ForEach(x =>
                {
                    x.IsSynchronized = true;
                });
                _unitofWork.Context.AddOns.UpdateRange(addons);
                await _unitofWork.Commit();
                var tempAddons = addons;
                tempAddons.ForEach(x =>
                {
                    x.IsSynchronized = true;
                    x.Id = 0;
                });
                var entityType = _unitofWork.LiveContext.Model.FindEntityType(typeof(AddOns));
                await _unitofWork.LiveContext.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} ON");
                _unitofWork.LiveContext.AddOns.AddRange(tempAddons);
                await _unitofWork.CommitLive();
                await _unitofWork.LiveContext.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} OFF");
                await _unitofWork.CommitLive();
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region Attachments Table
            try
            {
                var dataList = _unitofWork.Context.Attachment.Where(x => x.IsSynchronized == false).ToList();
                dataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                });
                _unitofWork.Context.Attachment.UpdateRange(dataList);
                await _unitofWork.Commit();
                var tempDataList = dataList;
                tempDataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                    x.Id = 0;
                });
                _unitofWork.LiveContext.Attachment.AddRange(tempDataList);
                await _unitofWork.CommitLive();
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region Counter Assign Table
            try
            {
                var dataList = _unitofWork.Context.CounterAssign.Where(x => x.IsSynchronized == false).ToList();
                dataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                });
                _unitofWork.Context.CounterAssign.UpdateRange(dataList);
                await _unitofWork.Commit();
                var tempDataList = dataList;
                tempDataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                    x.Id = 0;
                });
                _unitofWork.LiveContext.CounterAssign.AddRange(tempDataList);
                await _unitofWork.CommitLive();
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion

            #region Counter Table
            try
            {
                var dataList = _unitofWork.Context.Counter.Where(x => x.IsSynchronized == false).ToList();
                dataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                });
                _unitofWork.Context.Counter.UpdateRange(dataList);
                await _unitofWork.Commit();
                var tempDataList = dataList;
                tempDataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                    x.Id = 0;
                });
                _unitofWork.LiveContext.Counter.AddRange(tempDataList);
                await _unitofWork.CommitLive();
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region Counter Assign Table
            try
            {
                var CounterAssignList = _unitofWork.Context.CounterAssign.Where(x => x.IsSynchronized == false).ToList();
                CounterAssignList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                });
                _unitofWork.Context.CounterAssign.UpdateRange(CounterAssignList);
                await _unitofWork.Commit();
                var tempDataList = CounterAssignList;
                tempDataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                    x.Id = 0;
                });
                _unitofWork.LiveContext.CounterAssign.AddRange(tempDataList);
                await _unitofWork.CommitLive();
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region errorLog Table
            try
            {
                var ErrorLogList = _unitofWork.Context.ErrorLog.Where(x => x.IsSynchronized == false).ToList();
                ErrorLogList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                });
                _unitofWork.Context.ErrorLog.UpdateRange(ErrorLogList);
                await _unitofWork.Commit();
                var tempDataList = ErrorLogList;
                tempDataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                    x.Id = 0;
                });
                _unitofWork.LiveContext.ErrorLog.AddRange(tempDataList);
                await _unitofWork.CommitLive();
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion



            #region Food category Table
            try
            {
                var FoodCategoryList = _unitofWork.Context.FoodCategory.Where(x => x.IsSynchronized == false).ToList();
                FoodCategoryList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                });
                _unitofWork.Context.FoodCategory.UpdateRange(FoodCategoryList);
                await _unitofWork.Commit();
                var tempDataList = FoodCategoryList;
                tempDataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                    x.Id = 0;
                });
                _unitofWork.LiveContext.FoodCategory.AddRange(tempDataList);
                await _unitofWork.CommitLive();
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion




            #region Food category offer Table
            try
            {
                var FoodCategoryofferList = _unitofWork.Context.FoodCategoryOffer.Where(x => x.IsSynchronized == false).ToList();
                FoodCategoryofferList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                });
                _unitofWork.Context.FoodCategoryOffer.UpdateRange(FoodCategoryofferList);
                await _unitofWork.Commit();
                var tempDataList = FoodCategoryofferList;
                tempDataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                    x.Id = 0;
                });
                _unitofWork.LiveContext.FoodCategoryOffer.AddRange(tempDataList);
                await _unitofWork.CommitLive();
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region Food Item Table
            try
            {
                var FoodItemList = _unitofWork.Context.FoodItem.Where(x => x.IsSynchronized == false).ToList();
                FoodItemList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                });
                _unitofWork.Context.FoodItem.UpdateRange(FoodItemList);
                await _unitofWork.Commit();
                var tempDataList = FoodItemList;
                tempDataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                    x.Id = 0;
                });
                _unitofWork.LiveContext.FoodItem.AddRange(tempDataList);
                await _unitofWork.CommitLive();
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region Food Item offer Table
            try
            {
                var FoodItemofferList = _unitofWork.Context.FoodItemOffer.Where(x => x.IsSynchronized == false).ToList();
                FoodItemofferList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                });
                _unitofWork.Context.FoodItemOffer.UpdateRange(FoodItemofferList);
                await _unitofWork.Commit();
                var tempDataList = FoodItemofferList;
                tempDataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                    x.Id = 0;
                });
                _unitofWork.LiveContext.FoodItemOffer.AddRange(tempDataList);
                await _unitofWork.CommitLive();
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region Food variant Table
            try
            {
                var FoodvariantList = _unitofWork.Context.FoodVarient.Where(x => x.IsSynchronized == false).ToList();
                FoodvariantList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                });
                _unitofWork.Context.FoodVarient.UpdateRange(FoodvariantList);
                await _unitofWork.Commit();
                var tempDataList = FoodvariantList;
                tempDataList.ForEach(x =>
                {
                    x.IsSynchronized = true;
                    x.Id = 0;
                });
                _unitofWork.LiveContext.FoodVarient.AddRange(tempDataList);
                await _unitofWork.CommitLive();
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region Hall Table
            try
            {
                var HallList = _unitofWork.Context.Hall.Where(x => x.IsSynchronized == false).ToList();
                if (HallList != null)
                {
                    HallList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                    });
                    _unitofWork.Context.Hall.UpdateRange(HallList);
                    await _unitofWork.Commit();
                    var tempDataList = HallList;
                    tempDataList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                        x.Id = 0;
                    });
                    _unitofWork.LiveContext.Hall.AddRange(tempDataList);
                    await _unitofWork.CommitLive();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion



            #region Hall Assign Table
            try
            {
                var halAssignList = _unitofWork.Context.HallAssign.Where(x => x.IsSynchronized == false).ToList();
                if (halAssignList != null)
                {
                    halAssignList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                    });
                    _unitofWork.Context.HallAssign.UpdateRange(halAssignList);
                    await _unitofWork.Commit();
                    var tempDataList = halAssignList;
                    tempDataList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                        x.Id = 0;
                    });
                    _unitofWork.LiveContext.HallAssign.AddRange(tempDataList);
                    await _unitofWork.CommitLive();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region Kitchen Table
            try
            {
                var kitchenList = _unitofWork.Context.Kitchen.Where(x => x.IsSynchronized == false).ToList();
                if (kitchenList != null)
                {
                    kitchenList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                    });
                    _unitofWork.Context.Kitchen.UpdateRange(kitchenList);
                    await _unitofWork.Commit();
                    var tempDataList = kitchenList;
                    tempDataList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                        x.Id = 0;
                    });
                    _unitofWork.LiveContext.Kitchen.AddRange(tempDataList);
                    await _unitofWork.CommitLive();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region Kitchen Assign Table
            try
            {
                var kitchenAssignList = _unitofWork.Context.KitchenAssign.Where(x => x.IsSynchronized == false).ToList();
                if (kitchenAssignList != null)
                {
                    kitchenAssignList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                    });
                    _unitofWork.Context.KitchenAssign.UpdateRange(kitchenAssignList);
                    await _unitofWork.Commit();
                    var tempDataList = kitchenAssignList;
                    tempDataList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                        x.Id = 0;
                    });
                    _unitofWork.LiveContext.KitchenAssign.AddRange(tempDataList);
                    await _unitofWork.CommitLive();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion




            #region Order Table
            try
            {
                var OrderList = _unitofWork.Context.Order.Where(x => x.IsSynchronized == false).ToList();
                if (OrderList != null)
                {
                    OrderList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                    });
                    _unitofWork.Context.Order.UpdateRange(OrderList);
                    await _unitofWork.Commit();
                    var tempDataList = OrderList;
                    tempDataList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                        x.Id = 0;
                    });
                    _unitofWork.LiveContext.Order.AddRange(tempDataList);
                    await _unitofWork.CommitLive();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region Order Item Table
            try
            {
                var OrderitemList = _unitofWork.Context.OrderItem.Where(x => x.IsSynchronized == false).ToList();
                if (OrderitemList != null)
                {
                    OrderitemList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                    });
                    _unitofWork.Context.OrderItem.UpdateRange(OrderitemList);
                    await _unitofWork.Commit();
                    var tempDataList = OrderitemList;
                    tempDataList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                        x.Id = 0;
                    });
                    _unitofWork.LiveContext.OrderItem.AddRange(tempDataList);
                    await _unitofWork.CommitLive();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region Permission Table
            try
            {
                var PrimissionList = _unitofWork.Context.Permission.Where(x => x.IsSynchronized == false).ToList();
                if (PrimissionList != null)
                {
                    PrimissionList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                    });
                    _unitofWork.Context.Permission.UpdateRange(PrimissionList);
                    await _unitofWork.Commit();
                    var tempDataList = PrimissionList;
                    tempDataList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                        x.Id = 0;
                    });
                    _unitofWork.LiveContext.Permission.AddRange(tempDataList);
                    await _unitofWork.CommitLive();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion



            #region Permission Assign Table
            try
            {
                var PrimissionassignList = _unitofWork.Context.PermissionAssign.Where(x => x.IsSynchronized == false).ToList();
                if (PrimissionassignList != null)
                {
                    PrimissionassignList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                    });
                    _unitofWork.Context.PermissionAssign.UpdateRange(PrimissionassignList);
                    await _unitofWork.Commit();
                    var tempDataList = PrimissionassignList;
                    tempDataList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                        x.Id = 0;
                    });
                    _unitofWork.LiveContext.PermissionAssign.AddRange(tempDataList);
                    await _unitofWork.CommitLive();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion



            #region Restaurant Table
            try
            {
                var RestaurantList = _unitofWork.Context.Restaurant.Where(x => x.IsSynchronized == false).ToList();
                if (RestaurantList != null)
                {
                    RestaurantList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                    });
                    _unitofWork.Context.Restaurant.UpdateRange(RestaurantList);
                    await _unitofWork.Commit();
                    var tempDataList = RestaurantList;
                    tempDataList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                        x.Id = 0;
                    });
                    _unitofWork.LiveContext.Restaurant.AddRange(tempDataList);
                    await _unitofWork.CommitLive();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion


            #region Restaurant Charges Table
            try
            {
                var RestaurantChargesList = _unitofWork.Context.ResturantCharges.Where(x => x.IsSynchronized == false).ToList();
                if (RestaurantChargesList != null)
                {
                    RestaurantChargesList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                    });
                    _unitofWork.Context.ResturantCharges.UpdateRange(RestaurantChargesList);
                    await _unitofWork.Commit();
                    var tempDataList = RestaurantChargesList;
                    tempDataList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                        x.Id = 0;
                    });
                    _unitofWork.LiveContext.ResturantCharges.AddRange(tempDataList);
                    await _unitofWork.CommitLive();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion



            #region Role Table
            try
            {
                var RoleList = _unitofWork.Context.Role.Where(x => x.IsSynchronized == false).ToList();
                if (RoleList != null)
                {
                    RoleList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                    });
                    _unitofWork.Context.Role.UpdateRange(RoleList);
                    await _unitofWork.Commit();
                    var tempDataList = RoleList;
                    tempDataList.ForEach(x =>
                    {
                        x.IsSynchronized = true;
                        x.Id = 0;
                    });
                    _unitofWork.LiveContext.Role.AddRange(tempDataList);
                    await _unitofWork.CommitLive();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            #endregion

            return 1;
        }
    }

    public interface IUserRepository
    {
        Task<IEnumerable<UserResponseDTO>> Get();
        Task<IEnumerable<UserResponseDTO>> GetAll();
        Task<long> GetUsersCount();
        Task<(IEnumerable<UserResponseDTO> UsersList, int recordsCount)> GetByFilters(SearchFilter filters);
        Task<UserResponseDTO> Get(int id);
        Task<IEnumerable<UserResponseDTO>> GetAllUnassignUsers();
        Task<UserResponseDTO> ValidateAndLogin(UserLoginRequestDTO model);
        Task<long> Save(UserRequestDTO model, int id);
        Task<IEnumerable<UserResponseDTO>> GetAllByRoleType(int AssignedTypes);
        Task<User> GetByUserId(int Id);
        Task<long> Delete(int Id, int LoginPerson);
        Task<User> GetByEmail(string Email);
        Task<bool> IsExist(string Email);
        Task<long> UpdateFilePath();
        Task<long> SyncData();
    }
}