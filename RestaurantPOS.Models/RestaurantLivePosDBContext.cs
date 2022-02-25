using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Helpers.UtilityHelper;
using RestaurantPOS.Helpers.ViewModels;
using System;

namespace RestaurantPOS.Models
{
    public class RestaurantLivePosDBContext : DbContext
    {
        public RestaurantLivePosDBContext(DbContextOptions<RestaurantLivePosDBContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<Company>().HasOne(bc => new { bc.Country }).WithMany(x => x.Country.Companies).HasForeignKey(p => p.CountryId);
            //builder.Entity<BookCategory>().HasKey(bc => new { bc.BookId, bc.CategoryId });
            builder.Entity<OrdersViewModel>().HasNoKey().ToView(null);
            builder.Entity<OrderItemsViewModel>().HasNoKey().ToView(null);
            builder.Entity<TablesViewModel>().HasNoKey().ToView(null);
            builder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@gmail.com",
                IsDeleted = false,
                Password = GeneralHelper.Encrypt("123"), //// 123
                ContactNo = "03003444332",
                AssignedRole = 0,
                AssignedType = 0,
                CreatedBy = -1,
                CreatedOn = DateTime.Now,
                FcmToken = "",
                IsSynchronized = false,
                UpdatedBy = -1,
                UpdatedOn = null,
                UserAttachmentId = 0
            });
        }

        #region Custom Quries View Models

        public DbSet<OrdersViewModel> OrdersViewModel { get; set; }
        public DbSet<OrderItemsViewModel> OrderItemsViewModel { get; set; }
        public DbSet<TablesViewModel> TablesViewModel { get; set; }
        #endregion

        public DbSet<User> User { get; set; }
        public DbSet<ResturantCharges> ResturantCharges { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<PermissionAssign> PermissionAssign { get; set; }
        public DbSet<AddOns> AddOns { get; set; }
        public DbSet<AddOnsAssign> AddOnsAssign { get; set; }
        public DbSet<Counter> Counter { get; set; }
        public DbSet<CounterAssign> CounterAssign { get; set; }
        public DbSet<FoodCategory> FoodCategory { get; set; }
        public DbSet<FoodCategoryOffer> FoodCategoryOffer { get; set; }
        public DbSet<FoodVarient> FoodVarient { get; set; }
        public DbSet<FoodItem> FoodItem { get; set; }
        public DbSet<FoodItemOffer> FoodItemOffer { get; set; }
        public DbSet<Hall> Hall { get; set; }
        public DbSet<HallAssign> HallAssign { get; set; }
        public DbSet<Kitchen> Kitchen { get; set; }
        public DbSet<KitchenAssign> KitchenAssign { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<Table> Table { get; set; }
        public DbSet<Attachment> Attachment { get; set; }
        public DbSet<Restaurant> Restaurant { get; set; }
        public DbSet<ErrorLog> ErrorLog { get; set; }
    }

}