USE [PROJECT.FC2J]
GO

/****** Object:  Table [sale.order.detail].[FC2J]    Script Date: 03/10/2020 11:07:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [sale.order.detail].[Payment_FC2J](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderHeaderId] [bigint] NOT NULL,
	[ProductId] [bigint] NOT NULL, -- can have multiple entry based on the total quantity that fits the total order
	[Quantity] [float] NOT NULL, 
	-- below columns shall be fixed data based from order details
	[Name] [varchar](50) NULL,
	[Category] [varchar](200) NULL,
	[UnitOfMeasure] [varchar](50) NULL,
	[SalePrice] [money] NULL,
	[UnitDiscount] [money] NULL,
	[SFAUnitOfMeasure] [varchar](50) NULL,
	[SFAReferenceNo] [varchar](50) NULL,
	[SubTotal] [money] NULL,
	[IsTaxable] [bit] NULL,
	[TaxRate] [money] NULL,
	[TaxPrice] [money] NULL,
	[InvoiceNo] [varchar](25) NULL CONSTRAINT [DF_sale_order_detail_Payment_FC2J_InvoiceNo]  DEFAULT (''),
 CONSTRAINT [PK_sale.order.detail_Payment_FC2J] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


