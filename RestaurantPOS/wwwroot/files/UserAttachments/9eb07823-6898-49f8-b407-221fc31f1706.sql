
USE [RestaurantDBLive]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AddOns]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AddOns](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[IsDeleted] [bit] NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_AddOns] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AddOnsAssign]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AddOnsAssign](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AddOnsId] [int] NOT NULL,
	[FoodVarientId] [int] NOT NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_AddOnsAssign] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Attachment]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileToUpLoad] [nvarchar](max) NULL,
	[FileType] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Counter]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Counter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsSynchronized] [bit] NULL,
	[HallId] [int] NULL,
 CONSTRAINT [PK_Counter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CounterAssign]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CounterAssign](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[UserId] [int] NOT NULL,
	[CounterId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_CounterAssign] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ErrorLog]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LogController] [nvarchar](max) NULL,
	[LogAction] [nvarchar](max) NULL,
	[LogMessage] [nvarchar](max) NULL,
	[LogDetail] [nvarchar](max) NULL,
	[ErrorLogDate] [nvarchar](max) NULL,
	[ErrorLogTime] [nvarchar](max) NULL,
	[ErrorCode] [nvarchar](max) NULL,
	[ErrorLine] [nvarchar](max) NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FoodCategory]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FoodCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[AttachmentId] [int] NOT NULL,
	[AttachmentPath] [nvarchar](max) NULL,
	[IsDeleted] [bit] NULL,
	[IsOffer] [bit] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_FoodCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FoodCategoryOffer]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FoodCategoryOffer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FoodCategoryId] [int] NOT NULL,
	[OfferName] [nvarchar](max) NULL,
	[OfferStart] [datetime2](7) NULL,
	[OfferEnd] [datetime2](7) NULL,
	[IsActive] [bit] NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_FoodCategoryOffer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FoodItem]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FoodItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Note] [nvarchar](max) NULL,
	[VAT] [nvarchar](max) NULL,
	[FoodCategoryId] [int] NOT NULL,
	[KitchenId] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[CookingTime] [nvarchar](max) NULL,
	[Quantity] [int] NOT NULL,
	[AttachmentId] [int] NOT NULL,
	[AttachmentPath] [nvarchar](max) NULL,
	[IsOffer] [bit] NOT NULL,
	[IsSpecial] [bit] NOT NULL,
	[Status] [bit] NOT NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_FoodItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FoodItemOffer]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FoodItemOffer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FoodItemId] [int] NOT NULL,
	[OfferName] [nvarchar](max) NULL,
	[OfferPrice] [decimal](18, 2) NULL,
	[OfferStart] [datetime2](7) NULL,
	[OfferEnd] [datetime2](7) NULL,
	[IsActive] [bit] NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_FoodItemOffer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FoodVarient]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FoodVarient](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[FoodItemId] [int] NOT NULL,
	[Price] [decimal](18, 2) NULL,
	[KitchenId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [int] NOT NULL,
	[CreatedBy] [int] NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_FoodVarient] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Hall]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hall](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_Hall] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HallAssign]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HallAssign](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[UserId] [int] NOT NULL,
	[HallId] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[UpdatedBy] [int] NOT NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_HallAssign] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Kitchen]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Kitchen](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_Kitchen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KitchenAssign]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KitchenAssign](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[KitchenId] [int] NOT NULL,
	[AssignDate] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_KitchenAssign] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HallId] [int] NULL,
	[TableId] [int] NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Discount] [decimal](18, 2) NULL,
	[ServiceCharges] [decimal](18, 2) NULL,
	[DeliveryCharges] [decimal](18, 2) NULL,
	[Tax] [decimal](18, 2) NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[CookingTime] [nvarchar](max) NULL,
	[PaidStatus] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[StartDateTime] [datetime2](7) NULL,
	[OrderStatus] [nvarchar](max) NULL,
	[OrderType] [int] NULL,
	[IsSynchronized] [bit] NULL,
	[PreparedTime] [datetime] NULL,
	[ServedTime] [datetime] NULL,
	[CompleteTime] [datetime] NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItem]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[FoodItemId] [int] NOT NULL,
	[FoodVarientId] [int] NOT NULL,
	[KitchenId] [int] NOT NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[Price] [decimal](18, 2) NULL,
	[Total] [decimal](18, 2) NULL,
	[Status] [nvarchar](max) NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permission]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[CanView] [bit] NULL,
	[CanCreate] [bit] NULL,
	[CanUpdate] [bit] NULL,
	[CanDelete] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PermissionAssign]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PermissionAssign](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[PermissionId] [int] NOT NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_PermissionAssign] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Restaurant]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Restaurant](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RestaurantName] [nvarchar](max) NULL,
	[WebSite] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[ContactNo] [nvarchar](max) NULL,
	[AttachmentId] [int] NOT NULL,
	[AttachmentPath] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_Restaurant] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResturantCharges]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResturantCharges](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ServiceCharges] [decimal](18, 2) NULL,
	[DeliveryCharges] [decimal](18, 2) NULL,
	[Tax] [decimal](18, 2) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_ResturantCharges] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Table]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Table](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[HallId] [int] NOT NULL,
	[IsAssigned] [int] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_Table] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](max) NULL,
	[AssignedRole] [int] NULL,
	[AssignedType] [int] NULL,
	[Password] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[ContactNo] [nvarchar](max) NULL,
	[UserAttachmentId] [int] NOT NULL,
	[AttachmentPath] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[FcmToken] [nvarchar](max) NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedOn] [datetime2](7) NULL,
	[UpdatedOn] [datetime2](7) NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsSynchronized] [bit] NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Counter]  WITH CHECK ADD  CONSTRAINT [FK_Counter_Hall_HallId] FOREIGN KEY([HallId])
REFERENCES [dbo].[Hall] ([Id])
GO
ALTER TABLE [dbo].[Counter] CHECK CONSTRAINT [FK_Counter_Hall_HallId]
GO
ALTER TABLE [dbo].[FoodCategoryOffer]  WITH CHECK ADD  CONSTRAINT [FK_FoodCategoryOffer_FoodCategory_FoodCategoryId] FOREIGN KEY([FoodCategoryId])
REFERENCES [dbo].[FoodCategory] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FoodCategoryOffer] CHECK CONSTRAINT [FK_FoodCategoryOffer_FoodCategory_FoodCategoryId]
GO
ALTER TABLE [dbo].[FoodItem]  WITH CHECK ADD  CONSTRAINT [FK_FoodItem_FoodCategory_FoodCategoryId] FOREIGN KEY([FoodCategoryId])
REFERENCES [dbo].[FoodCategory] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FoodItem] CHECK CONSTRAINT [FK_FoodItem_FoodCategory_FoodCategoryId]
GO
ALTER TABLE [dbo].[FoodItemOffer]  WITH CHECK ADD  CONSTRAINT [FK_FoodItemOffer_FoodItem_FoodItemId] FOREIGN KEY([FoodItemId])
REFERENCES [dbo].[FoodItem] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FoodItemOffer] CHECK CONSTRAINT [FK_FoodItemOffer_FoodItem_FoodItemId]
GO
ALTER TABLE [dbo].[FoodVarient]  WITH CHECK ADD  CONSTRAINT [FK_FoodVarient_FoodItem_FoodItemId] FOREIGN KEY([FoodItemId])
REFERENCES [dbo].[FoodItem] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FoodVarient] CHECK CONSTRAINT [FK_FoodVarient_FoodItem_FoodItemId]
GO
ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_Order_OrderId] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [FK_OrderItem_Order_OrderId]
GO
ALTER TABLE [dbo].[PermissionAssign]  WITH CHECK ADD  CONSTRAINT [FK_PermissionAssign_Permission_PermissionId] FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permission] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PermissionAssign] CHECK CONSTRAINT [FK_PermissionAssign_Permission_PermissionId]
GO
ALTER TABLE [dbo].[PermissionAssign]  WITH CHECK ADD  CONSTRAINT [FK_PermissionAssign_Role_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PermissionAssign] CHECK CONSTRAINT [FK_PermissionAssign_Role_RoleId]
GO
ALTER TABLE [dbo].[Table]  WITH CHECK ADD  CONSTRAINT [FK_Table_Hall_HallId] FOREIGN KEY([HallId])
REFERENCES [dbo].[Hall] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Table] CHECK CONSTRAINT [FK_Table_Hall_HallId]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_Role_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Role_RoleId]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_User_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_User_UserId]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllOrderItems]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetAllOrderItems]
	-- Add the parameters for the stored procedure here
	@orderStatus nvarchar(MAX),
	@pageNo int,
	@pageSize int,
	@dateFrom nvarchar(100),
	@dateTo nvarchar(100)
AS
BEGIN
CREATE TABLE #orderItemsViewModel
(
	Id int default 0,
	[Name] nvarchar(200) null,
	Quantity decimal null default 0.00,
	Price decimal null default 0.00,
	Total decimal null default 0.00,
	[Status] nvarchar(200) null,
	OrderId int default 0,
	KitchenId int null default 0,
	KitchenName nvarchar(200) null,
	FoodItemId  int null default 0,
	AttachmentId int null default 0,
	AttachmentPath nvarchar(MAX) null,
	FoodItemName nvarchar(MAX) null,
	CookingTime nvarchar(MAX) null,
	[Hours] nvarchar(MAX) null,
	[Min] nvarchar(MAX) null,
	FoodVarientId int null default 0
);
DECLARE @vId int = 0;
DECLARE @vName nvarchar(200);
DECLARE @vQuantity decimal ;
DECLARE @vPrice decimal ;
DECLARE @vTotal decimal ;
DECLARE @vStatus nvarchar(200);
DECLARE @vOrderId int ;
DECLARE @vKitchenId int ;
DECLARE @vKitchenName nvarchar(200) ;
DECLARE @vFoodItemId  int;
DECLARE @vAttachmentId int ;
DECLARE @vAttachmentPath nvarchar(MAX) ;
DECLARE @vFoodItemName nvarchar(MAX) ;
DECLARE @vCookingTime nvarchar(MAX) ;
DECLARE @vHours nvarchar(MAX) ;
DECLARE @vMin nvarchar(MAX) ;
DECLARE @vFoodVarientId int ;

SELECT * INTO #OrderItemsData FROM (select * from dbo.[OrderItem] WHERE OrderId in (select Id from [Order] where OrderStatus in (select value from string_split(@orderStatus, '_')) AND (CreatedDate >= @dateFrom AND CreatedDate <= @dateTo) ORDER BY (SELECT NULL) OFFSET 0*1000 ROWS FETCH NEXT 1000 ROWS ONLY)) AS orderItems;
while((Select COUNT(*) from #OrderItemsData) > 0)
Begin
	select top 1 @vId=Id, @vKitchenId=orderItemData.KitchenId, @vOrderId=orderItemData.OrderId, @vFoodItemId=orderItemData.FoodItemId,
	@vFoodVarientId=orderItemData.FoodVarientId, @vQuantity=orderItemData.Quantity, @vPrice=orderItemData.Price, @vStatus=orderItemData.[Status],
	@vTotal=orderItemData.Total From #OrderItemsData as orderItemData;
	
	select top 1 @vCookingTime=foodItem.CookingTime, @vFoodItemName=foodItem.[Name],
	@vAttachmentId=foodItem.[AttachmentId],@vAttachmentPath=foodItem.AttachmentPath,
	@vHours =(SELECT top 1 value FROM STRING_SPLIT(foodItem.CookingTime, ':')),
	@vMin =(SELECT top 1 value FROM STRING_SPLIT(foodItem.CookingTime, ':') order by value desc)
	From FoodItem as foodItem where Id=@vFoodItemId;

	set @vKitchenName = (select [Name] from [Kitchen] where Id=COALESCE(@vKitchenId, -1));
	set @vName = (select [Name] from [FoodVarient] where Id=COALESCE(@vFoodVarientId, -1));
	insert into #orderItemsViewModel VALUES(@vId,@vName,@vQuantity,@vPrice,@vTotal,@vStatus,@vOrderId,@vKitchenId,@vKitchenName,@vFoodItemId,@vAttachmentId,@vAttachmentPath,@vFoodItemName,@vCookingTime,@vHours,@vMin,@vFoodVarientId);
	------
    Delete From #OrderItemsData Where Id=@vId;
END;
Drop table #OrderItemsData;
select * from #orderItemsViewModel;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllOrders]    Script Date: 18/04/2021 12:31:13 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAllOrders]
	-- Add the parameters for the stored procedure here
	@orderStatus nvarchar(MAX),
	@pageNo int,
	@pageSize int,
	@dateFrom nvarchar(100),
	@dateTo nvarchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
CREATE TABLE #ordersViewModel
(
	Id int default 0,
	HallId int null default 0,
	HallName nvarchar(200) null,
	TableId  int null default 0,
	TableName nvarchar(200) null,
	TotalAmount decimal null default 0.00,
	Discount decimal null default 0.00,
	CookingTime nvarchar(MAX) null,
	[Hours] nvarchar(MAX) null,
	[Min] nvarchar(MAX) null,
	ServiceCharges decimal null default 0.00,
	DeliveryCharges decimal null default 0.00,
	Tax decimal null default 0.00,
	CreatedDate datetime null,
	ReserveTimeHours int null default 0,
	ReserveTimeMin int null default 0,
	ReserveTimeSeconds int null default 0,
	StartDateTime datetime null,
	RemainingTime int null default 0,
	RemainingSeconds int null default 0,
	PaidStatus bit null default 0,
	OrderType int null default 0,
	OrderStatus nvarchar(50) null default '',
	Amount decimal null default 0.00,
	PreparedTime datetime null,
	ServedTime datetime null,
	CompleteTime datetime null,
);
DECLARE @vId int;
DECLARE @vHallId int;
DECLARE @vHallName nvarchar(200);
DECLARE @vTableId int;
DECLARE @vTableName nvarchar(200);
DECLARE @vDiscount decimal;
DECLARE @vCookingTime nvarchar(MAX);
DECLARE @vHours nvarchar(MAX);
DECLARE @vMin nvarchar(MAX);
DECLARE @vServiceCharges decimal;
DECLARE @vDeliveryCharges decimal;
DECLARE @vTax decimal;
DECLARE @vTotalAmount decimal;
DECLARE @vCreatedDate datetime;
DECLARE @vReserveTimeHours int;
DECLARE @vReserveTimeMin int;
DECLARE @vReserveTimeSeconds int;
DECLARE @vStartDateTime datetime;
DECLARE @vRemainingTime int;
DECLARE @vRemainingSeconds int;
DECLARE @vOrderType int;
DECLARE @vOrderStatus nvarchar(30);
DECLARE @vPaidStatus  nvarchar(30);
DECLARE @vUpdatedBy nvarchar(200);
DECLARE @vAmount decimal;
DECLARE @vPreparedTime datetime ;
DECLARE @vServedTime datetime ;
DECLARE @vCompleteTime datetime;
SET @vId = 0;
SET @vHallId = 0;
SET @vHallName = '';
SET @vTableId = 0;
SET @vTableName = '';
SET @vAmount = 0.00;
SET @vDiscount = 0.00;
SET @vCookingTime = '';
SET @vHours = '00';
SET @vMin = '00';
SET @vServiceCharges = 0.00;
SET @vDeliveryCharges = 0.00;
SET @vTax = 0.00;
SET @vTotalAmount= 0.00;
SET @vCreatedDate=null ;
SET @vReserveTimeHours = 0;
SET @vReserveTimeMin = 0;
SET @vReserveTimeSeconds= 0;
SET @vStartDateTime = null ;
SET @vRemainingTime = 0;
SET @vRemainingSeconds = 0;
SET @vOrderType = 0;
SET @vOrderStatus = '';
SET @vPaidStatus = '';
SET @vPreparedTime = null ;
SET @vServedTime = null ;
SET @vCompleteTime  = null ;
SELECT * INTO #OrdersData FROM (select * from dbo.[Order] where OrderStatus in (select value from string_split(@orderStatus, '_')) AND (CreatedDate >= @dateFrom AND CreatedDate <= @dateTo) ORDER BY (SELECT NULL) OFFSET @pageNo*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY) as OrderDetails;
while((Select COUNT(*) from #OrdersData) > 0)
Begin
	select top 1 @vId=Id, @vHallId=orderData.HallId, @vTableId=orderData.TableId, @vTotalAmount=orderData.TotalAmount,
	@vHours = (SELECT top 1 value FROM string_split(orderData.CookingTime, ':')), @vDiscount=orderData.Discount,
	@vMin = (SELECT top 1 value FROM string_split(orderData.CookingTime, ':') order by value DESC),
	@vPaidStatus=orderData.PaidStatus, @vCreatedDate=orderData.CreatedDate, @vUpdatedBy=orderData.UpdatedBy,
	@vPreparedTime=orderData.PreparedTime, @vServedTime=orderData.ServedTime,@vCompleteTime=orderData.CompleteTime,
	@vCreatedDate=orderData.CreatedDate, @vStartDateTime=orderData.StartDateTime, @vOrderStatus=orderData.OrderStatus, @vOrderType=orderData.OrderType From #OrdersData as orderData;
	set @vTableName = (select [Name] from [Table] where Id=COALESCE(@vTableId, -1));
	set @vHallName = (select [Name] from [Hall] where Id=COALESCE(@vHallId, -1));
	insert into #ordersViewModel VALUES(@vId,@vHallId,@vHallName,@vTableId,@vTableName,@vTotalAmount,@vDiscount,@vCookingTime,@vHours,@vMin,@vServiceCharges,@vDeliveryCharges,@vTax,@vCreatedDate,@vReserveTimeHours,@vReserveTimeMin,@vReserveTimeSeconds,@vStartDateTime,@vRemainingTime,@vRemainingSeconds,@vPaidStatus,@vOrderType,@vOrderStatus,@vAmount,@vPreparedTime,@vServedTime,@vCompleteTime);
	---
    Delete From #OrdersData Where Id=@vId;
END;
Drop table #OrdersData;
select * from #ordersViewModel;
END
GO
USE [master]
GO
ALTER DATABASE [RestaurantDB] SET  READ_WRITE 
GO
