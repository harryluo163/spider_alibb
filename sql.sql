USE [CSHous_harry_test]
GO

/****** Object:  Table [dbo].[LR_Demo_CPA2]    Script Date: 2018/6/29 13:17:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[LR_Demo_CPA2](
	[F_CPAId] [VARCHAR](50) NOT NULL,
	[F_ProductName] [NVARCHAR](30) NULL,
	[F_QueryUrl] [VARCHAR](150) NULL,
	[F_QueryName] [VARCHAR](50) NULL,
	[F_QueryPwd] [VARCHAR](50) NULL,
	[F_QueryPwd2] [VARCHAR](500) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LR_Demo_CPA2', @level2type=N'COLUMN',@level2name=N'F_CPAId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LR_Demo_CPA2', @level2type=N'COLUMN',@level2name=N'F_ProductName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'后台URL' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LR_Demo_CPA2', @level2type=N'COLUMN',@level2name=N'F_QueryUrl'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LR_Demo_CPA2', @level2type=N'COLUMN',@level2name=N'F_QueryName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LR_Demo_CPA2', @level2type=N'COLUMN',@level2name=N'F_QueryPwd'
GO


USE [CSHous_harry_test]
GO

/****** Object:  Table [dbo].[LR_Demo_CPA2]    Script Date: 2018/6/29 13:17:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[LR_Demo_CPA2](
	[F_CPAId] [varchar](50) NOT NULL,
	[F_ProductName] [nvarchar](30) NULL,
	[F_QueryUrl] [varchar](150) NULL,
	[F_QueryName] [varchar](50) NULL,
	[F_QueryPwd] [varchar](50) NULL,
	[F_QueryPwd2] [varchar](500) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LR_Demo_CPA2', @level2type=N'COLUMN',@level2name=N'F_CPAId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'产品名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LR_Demo_CPA2', @level2type=N'COLUMN',@level2name=N'F_ProductName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'后台URL' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LR_Demo_CPA2', @level2type=N'COLUMN',@level2name=N'F_QueryUrl'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LR_Demo_CPA2', @level2type=N'COLUMN',@level2name=N'F_QueryName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LR_Demo_CPA2', @level2type=N'COLUMN',@level2name=N'F_QueryPwd'
GO

