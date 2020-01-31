ALTER PROCEDURE [dbo].[CreateCustomerPricelistTemplateTable] 
@Id bigint

AS BEGIN
	
	if ( @Id > 0 )
	BEGIN
		Declare @CustomerId as varchar(20);
		Set @CustomerId = CONVERT(varchar, @Id)

		DECLARE @SQL AS NVARCHAR(MAX)= '
			CREATE TABLE [pricelist.template].['+@CustomerId+'](
				[Id] [bigint] NOT NULL,
				[SFAReferenceNo] [varchar](50) NULL,
				[Name] [varchar](50) NULL,
				[Description] [varchar](200) NULL,
				[Category] [varchar](200) NULL,
				[UnitOfMeasure] [varchar](50) NULL,
				[CostPrice] [money] NULL,
				[UnitDiscount] [money] NULL,
				[SFAUnitOfMeasure] [varchar](50) NULL,
				[IsTaxable] [bit] NULL CONSTRAINT [DF_pricelist_template_'+@CustomerId+'_IsTaxable]  DEFAULT ((1)),
				[Deleted] [bit] NULL,
				[ProductType] [varchar](50) NULL CONSTRAINT [DF_pricelist_template_'+@CustomerId+'_ProductType]  DEFAULT (''Stockable Product''),
				[CanBeSold] [bit] NULL CONSTRAINT [DF_pricelist_template_'+@CustomerId+'_CanBeSold]  DEFAULT ((1)),
				[CanBePurchased] [bit] NULL CONSTRAINT [DF_pricelist_template_'+@CustomerId+'_CanBePurchased]  DEFAULT ((1)),
				[Barcode] [varchar](50) NULL,
				[InternalCategory] [varchar](100) NULL,
				[PurchaseUnitOfMeasure] [varchar](50) NULL,
				[ControlPurchaseBills] [varchar](100) NULL,
				[DeductionFixPrice] [money] NULL CONSTRAINT [DF_pricelist_template_'+@CustomerId+'_DeductionFixPrice]  DEFAULT ((0)),
				[DeductionOutright] [money] NULL CONSTRAINT [DF_pricelist_template_'+@CustomerId+'_DeductionOutright]  DEFAULT ((0)),
				[Discount] [float] NULL CONSTRAINT [DF_pricelist_template_'+@CustomerId+'_Discount]  DEFAULT ((0)),
				[DeductionCashDiscount] [money] NULL CONSTRAINT [DF_pricelist_template_'+@CustomerId+'_DeductionCashDiscount]  DEFAULT ((0)),
				[DeductionPromoDiscount] [money] NULL CONSTRAINT [DF_pricelist_template_'+@CustomerId+'_DeductionPromoDiscount]  DEFAULT ((0)),
				[Image] [varchar](max) NULL,
				[UpdatedDate] [datetime] NULL CONSTRAINT [DF_pricelist_template_'+@CustomerId+'_UpdatedDate]  DEFAULT ((getdate())),
				[SalePrice_CORON] [money] NULL,
				[SalePrice_LUBANG] [money] NULL,
				[SalePrice_SANILDEFONSO] [money] NULL,

			 CONSTRAINT [PK_pricelist_template_'+@CustomerId+'] PRIMARY KEY CLUSTERED 
			(
				[Id] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
			) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
			';

	
			IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES
			WHERE TABLE_NAME='[pricelist.template].['+@CustomerId+']')
			BEGIN
				EXEC(@SQL)
			END
	
		END

END