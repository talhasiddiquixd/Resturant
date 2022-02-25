using RestaurantPOS.Models;
using RestaurantPOS.Repository;
using Microsoft.Extensions.DependencyInjection;
using RestaurantPOS.SignalR;

namespace RestaurantPOS.Helpers
{
    public class DependencyInjectionScopesHelper
    {
        public static void Update(IServiceCollection services)
        {
            #region Repositories Scopes
            ///// General Scope will be there
            {
                // Added By Umar Siddique...ISynchronizationRepository
                services.AddScoped<IunitOfWork, UnitOfWork>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IJwtHelper, JwtHelper>();
                services.AddScoped<IunitOfWork, UnitOfWork>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IPermissionRepository, PermissionRepository>();
                services.AddScoped<IAttachmentRepository, AttachmentRepository>();
                services.AddScoped<IAddOnsRepository, AddOnsRepository>();
                services.AddScoped<IRoleRepository, RoleRepository>();
                services.AddScoped<IRestaurantRepository, RestaurantRepository>();
                services.AddScoped<IPermissionAssignRepository, PermissionAssignRepository>();
                // Added By Anas...
                services.AddScoped<IFoodCategoryRepository, FoodCategoryRepository>();
                services.AddScoped<IFoodCategoryOfferRepository, FoodCategoryOfferRepository>();
                services.AddScoped<IFoodItemRepository, FoodItemRepository>();
                services.AddScoped<IFoodItemOfferRepository, FoodItemOfferRepository>();
                services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
                services.AddScoped<IOrderRepository, OrderRepository>();
                services.AddScoped<IReportsRepository, ReportsRepository>();
                //services.AddScoped<IJwtHelper, JwtHelper>();
                services.AddScoped<IunitOfWork, UnitOfWork>();
                services.AddScoped<IUserRepository, UserRepository>();
                // Added By Talha Siddiqui
                services.AddScoped<IKitchenRepository, KitchenRepository>();
                services.AddScoped<IKitchenAssignRepository, KitchenAssignRepository>();
                services.AddScoped<IHallRepository, HallRepository>();
                services.AddScoped<IHallAssignRepository, HallAssignRepository>();
                services.AddScoped<ITableRepository, TableRepository>();
                services.AddScoped<ICounterRepository, CounterRepository>();
                services.AddScoped<ICounterAssignRepository, CounterAssignRepository>();
                services.AddScoped<IFoodVarientRepository, FoodVarientRepository>();
                services.AddScoped<IResturantChargesRepository, ResturantChargesRepository>();
                services.AddScoped<IOrderNotificationsManager, OrderNotificationsManager>();
                #endregion
            }
        }
    }
}