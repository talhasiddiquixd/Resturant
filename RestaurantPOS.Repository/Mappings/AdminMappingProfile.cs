using AutoMapper;
using RestaurantPOS.Models;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using RestaurantPOS.Helpers.ViewModels;

namespace RestaurantPOS.Repository.Mappings
{
    public class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            //CreateMap<Administrator, AdministratorRequestDTO>()RoleResponseDTO
            //       .ForMember(dest => dest.Id, source => source.MapFrom(src => src.AdminId))
            //       .ReverseMap();
            //// User Api Models mappings 
            CreateMap<User, UserRequestDTO>().ReverseMap();
            CreateMap<User, UserResponseDTO>().ReverseMap();
            ////// Premission...
            CreateMap<Permission, PermissionRequestDTO>().ReverseMap();
            CreateMap<Permission, PermissionResponseDTO>().ReverseMap();
            //// Attachment...    
            CreateMap<Attachment, AttachmentResponseDTO>().ReverseMap();
            //// umar siddique
            CreateMap<Role, RoleResponseDTO>().ReverseMap();
            CreateMap<Role, RoleRequestDTO>().ReverseMap();
            ////// FoodCategory Mapping...
            CreateMap<FoodCategory, FoodCategoryRequestDTO>().ReverseMap();
            CreateMap<FoodCategory, FoodCategoryReponseDTO>().ReverseMap();
            ////// FoodCategoryOffer Mapping...
            CreateMap<FoodCategoryOffer, FoodCategoryOfferRequestDTO>().ReverseMap();
            CreateMap<FoodCategoryOffer, FoodCategoryOfferResponseDTO>().ReverseMap();
            ////// FoodItem Mapping...FoodItemResponseDTO
            CreateMap<FoodItem, FoodItemRequestDTO>().ReverseMap();
            CreateMap<FoodItem, FoodItemResponseDTO>().ReverseMap();
            CreateMap<FoodItem, SearchFoodItemResponseDTO>().ReverseMap();
            CreateMap<FoodItemResponseDTO, SearchFoodItemResponseDTO>().ReverseMap();
            ////// FoodItemOffer Mapping...
            CreateMap<FoodItemOffer, FoodItemOfferResponseDTO>().ReverseMap();
            CreateMap<FoodItemOffer, FoodItemOfferRequestDTO>().ReverseMap();
            ////// Order Mapping...OrderChargesReportResponseDTO
            CreateMap<Order, OrderResponseDTO>().ReverseMap();
            CreateMap<Order, OrderRequestDTO>().ReverseMap();
            CreateMap<OrderItem, OrderRequestDTO>().ReverseMap();
            ////// OrderItem Mapping...
            CreateMap<OrderItem, OrderItemResponseDTO>().ReverseMap();
            CreateMap<OrderItem, OrderItemRequestDTO>().ReverseMap();
            CreateMap<OrderItem, OrderItemsViewModel>().ReverseMap();
            ///// Kitchen Api models mappings
            CreateMap<Kitchen, KitchenRequestDTO>().ReverseMap();
            CreateMap<Kitchen, KitchenResponseDTO>().ReverseMap();
            ///// Error logs
            CreateMap<ErrorLog, ErrorLogRequestDTO>().ReverseMap();
            //CreateMap<ErrorLog, ErrorLogResponseDTO>().ReverseMap();
            ///// KitchenAssign Api models mappings    
            CreateMap<KitchenAssign, KitchenAssignRequestDTO>().ReverseMap();
            CreateMap<KitchenAssign, KitchenAssignResponseDTO>().ReverseMap();
            /// Hall Api models mappings
            CreateMap<Hall, HallRequestDTO>().ReverseMap();
            CreateMap<Hall, HallResponseDTO>().ReverseMap();
            //// Table Api models mappings
            /// Hall Assign Api models mappings
            CreateMap<HallAssign, HallAssignRequestDTO>().ReverseMap();
            CreateMap<HallAssign, HallAssignResponseDTO>().ReverseMap();
            //// Table Api models mappings
            CreateMap<Table, TableRequestDTO>().ReverseMap();
            CreateMap<Table, TableResponseDTO>().ReverseMap();
            ///// AddOns  umar siddique 
            CreateMap<AddOns, AddOnsResponseDTO>().ReverseMap();
            CreateMap<AddOns, AddOnsRequestDTO>().ReverseMap();
            CreateMap<AddOnsAssign, AssignAddOnsRequestDTO>().ReverseMap();
            ///// UserRole umar siddique 
            CreateMap<UserRole, UserRoleRequestDTO>().ReverseMap();
            CreateMap<UserRole, UserRoleResponseDTO>().ReverseMap();
            ////// PermissionAssign
            CreateMap<PermissionAssign, AssignPermissionRequestDTO>().ReverseMap();
            CreateMap<PermissionAssign, AssignPermissionResponseDTO>().ReverseMap();
            //// Counter Talha Siddiqui
            CreateMap<Counter, CounterRequestDTO>().ReverseMap();
            CreateMap<Counter, CounterResponseDTO>().ReverseMap();
            //// Counter Talha Siddiqui
            CreateMap<CounterAssign, CounterAssignRequestDTO>().ReverseMap();
            CreateMap<CounterAssign, CounterAssignResponseDTO>().ReverseMap();
            //// Food Varient Talha Siddiqui
            CreateMap<FoodVarient, FoodVarientRequestDTO>().ReverseMap();
            CreateMap<FoodVarient, FoodVarientResponseDTO>().ReverseMap();
            /////////Restaurant db
            CreateMap<Restaurant, RestaurantResposeDTO>().ReverseMap();
            CreateMap<Restaurant, RestaurantRequestDTO>().ReverseMap();
            //// Resturant Charges Talha Siddiqui
            CreateMap<ResturantCharges, ResturantChargesRequestDTO>().ReverseMap();
            CreateMap<ResturantCharges, ResturantChargesResponseDTO>().ReverseMap();
            //// UnassignedRequestDTO Changes Talha Siddiqui
            CreateMap<Table, UnassignedTableRequestDTO>().ReverseMap();
            //// AssignAddOns Response DTO Talha Siddiqui
            CreateMap<AddOnsAssign,AssignAddOnsResponseDTO>().ReverseMap();
            
        }
    }
}