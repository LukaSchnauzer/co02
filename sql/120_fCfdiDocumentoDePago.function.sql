IF OBJECT_ID ('dbo.fnCfdiDocumentoDePago') IS NOT NULL
   DROP FUNCTION dbo.fnCfdiDocumentoDePago
GO

create function [dbo].fnCfdiDocumentoDePago (@RMDTYPAL smallint, @DOCNUMBR varchar(21))
returns table 
as
--Propósito. Devuelve datos de un cobro
--Requisitos. -
--14/08/19 jcf Creación cfdi
--
return
		(
		SELECT 
			hdr.RMDTYPAL, hdr.DOCNUMBR, 
			hdr.docdate, hdr.bchsourc, hdr.mscschid, hdr.CSHRCTYP, hdr.FRTSCHID, 
			CASE WHEN hdr.bchsourc like '%MCP%' 
				then Rtrim(mcp.grupid) 
				else ch.FormaPago
			end											medioDePago,
			hdr.ororgtrx								Monto

		FROM dbo.vwRmTransaccionesTodas hdr
			outer apply dbo.fnCfdiMcpFormaPago(hdr.DOCNUMBR) mcp
			outer apply dbo.fnCfdiFormaPagoManual(hdr.mscschid, hdr.CSHRCTYP, hdr.FRTSCHID, 3) ch
		where hdr.docnumbr = @DOCNUMBR	
		and hdr.RMDTYPAL = @RMDTYPAL
		)

go

IF (@@Error = 0) PRINT 'Creación exitosa de: [fnCfdiDocumentoDePago]()'
ELSE PRINT 'Error en la creación de: [fnCfdiDocumentoDePago]()'
GO
