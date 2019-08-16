IF not EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.nfmcp20100') AND OBJECTPROPERTY(id,N'IsTable') = 1)
begin
	create table dbo.nfmcp20100
	(
	[MCPTYPID] [char](21) NOT NULL,
	[NUMBERIE] [char](21) NOT NULL,
	[LNSEQNBR] [numeric](19, 5) NOT NULL,
	[MEDIOID] [char](21) NOT NULL,
	[BANKID] [char](15) NOT NULL,
	[DOCNUMBR] [char](21) NOT NULL,
	[LOCATNNM] [char](31) NOT NULL,
	[TITACCT] [char](65) NOT NULL,
	[EMIDATE] [datetime] NOT NULL,
	[DUEDATE] [datetime] NOT NULL,
	[LINEAMNT] [numeric](19, 5) NOT NULL,
	[AMOUNTO] [numeric](19, 5) NOT NULL,
	[CURNCYID] [char](15) NOT NULL,
	[STSDESCR] [char](31) NOT NULL,
	[CURRNIDX] [smallint] NOT NULL,
	[BANACTID] [char](21) NOT NULL,
	[TII_MCP_Clearing] [smallint] NOT NULL,
	[TII_MCP_Checkbook_Integ] [tinyint] NOT NULL,
	[TII_MCP_Integrated_Date] [datetime] NOT NULL,
	[TII_CHEKBKID] [char](15) NOT NULL,
	[TXRGNNUM] [char](25) NOT NULL,
	[DEX_ROW_ID] [int] IDENTITY(1,1) NOT NULL
	) on [PRIMARY];
end

IF not EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.nfMCP_PM20100') AND OBJECTPROPERTY(id,N'IsTable') = 1)
begin
	CREATE TABLE [dbo].[nfMCP_PM20100](
		[MCPTYPID] [char](21) NOT NULL,
		[NUMBERIE] [char](21) NOT NULL,
		[MEDIOID] [char](21) NOT NULL,
		[BANKID] [char](15) NOT NULL,
		[DOCNUMBR] [char](21) NOT NULL,
		[TITACCT] [char](65) NOT NULL,
		[EMIDATE] [datetime] NOT NULL,
		[DUEDATE] [datetime] NOT NULL,
		[LINEAMNT] [numeric](19, 5) NOT NULL,
		[AMOUNTO] [numeric](19, 5) NOT NULL,
		[CURNCYID] [char](15) NOT NULL,
		[STSDESCR] [char](31) NOT NULL,
		[LNSEQNBR] [numeric](19, 5) NOT NULL,
		[CHEKBKID] [char](15) NOT NULL,
		[BANACTID] [char](21) NOT NULL,
		[CURRNIDX] [smallint] NOT NULL,
		[TII_MCP_Realized_Gain_Lo] [numeric](19, 5) NOT NULL,
		[TII_MCP_Clearing] [smallint] NOT NULL,
		[TII_MCP_Checkbook_Integ] [tinyint] NOT NULL,
		[TII_MCP_Integrated_Date] [datetime] NOT NULL,
		[TII_CHEKBKID] [char](15) NOT NULL,
		[VOIDED] [tinyint] NOT NULL,
		[VOIDDATE] [datetime] NOT NULL,
		[TXRGNNUM] [char](25) NOT NULL,
		[DEX_ROW_ID] [int] IDENTITY(1,1) NOT NULL,
	 CONSTRAINT [PKnfMCP_PM20100] PRIMARY KEY NONCLUSTERED 
	(
		[MCPTYPID] ASC,
		[NUMBERIE] ASC,
		[MEDIOID] ASC,
		[LNSEQNBR] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
end

-----------------------------------------------------------------------------------------------------------------------------
IF not EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.nfmcp30100') AND OBJECTPROPERTY(id,N'IsTable') = 1)
begin
	create table dbo.nfmcp30100
	(
	[NUMBERIE] [char](21) NOT NULL,
	[MEDIOID] [char](21) NOT NULL,
	[LINEAMNT] [numeric](19, 5) NOT NULL,
	[TII_CHEKBKID] [char](15) NOT NULL,
	[DEX_ROW_ID] [int] IDENTITY(1,1) NOT NULL
	) on [PRIMARY];
end

-----------------------------------------------------------------------------------------------------------------------------
IF not EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.nfmcp00700') AND OBJECTPROPERTY(id,N'IsTable') = 1)
begin
	create table dbo.nfmcp00700
	(
	[GRUPID] [char](21) NOT NULL,
	[MEDIOID] [char](21) NOT NULL,
	[CHEKBKID] [char](15) NOT NULL
	) on [PRIMARY];
end

-----------------------------------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.fnCfdiMcpFormaPago') IS NOT NULL
   DROP FUNCTION dbo.fnCfdiMcpFormaPago
GO

create function dbo.fnCfdiMcpFormaPago(@DOCNUMBR varchar(21))
returns table
--Propósito. Obtiene la forma de pago de MCP
--14/08/19 jcf Creación
--
as
return(
	select top (1) mcpd.grupid, mcpfp.tii_chekbkid
	from
		( select tii_chekbkid, medioid, numberie, lineamnt  
		from nfmcp20100  
		union all
		select tii_chekbkid, medioid, numberie, lineamnt  
		from nfmcp30100  
		) mcpfp
 	left join nfmcp00700 mcpd 
		on mcpd.medioid=mcpfp.medioid
	where mcpfp.numberie = @DOCNUMBR
	order by mcpfp.lineamnt desc
)

go
IF (@@Error = 0) PRINT 'Creación exitosa de: fnCfdiMcpFormaPago()'
ELSE PRINT 'Error en la creación de: fnCfdiMcpFormaPago()'
GO
--------------------------------------------------------------------------------------------------------

