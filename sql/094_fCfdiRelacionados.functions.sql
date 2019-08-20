--------------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.fnCfdiRelacionados') IS NOT NULL
   DROP FUNCTION dbo.fnCfdiRelacionados
GO

create function dbo.fnCfdiRelacionados(@soptype smallint, @p_sopnumbe varchar(21))
returns table
as
--Propósito. Obtiene la relación con otros documentos. 
--		Si la factura relaciona a otra factura o nd, consultar el tracking number
--Requisito. 
--15/08/19 jcf Creación
--
return(
			--relaciona a su mismo tipo de documento. 
			select top(1) 1 orden,
				da.soptype doctype, da.tracking_number docnumbr, 
				u.docdate,
				isnull(u.UUID, 'no existe uuid') UUID, 
				u.voidstts
			from sop10107 da	--
				outer apply dbo.fnCfdiObtieneUUID(da.soptype, da.tracking_number) u
			where da.sopnumbe = @p_sopnumbe
			and da.soptype = @soptype
			and da.soptype = 3	--facturas
			
			union all

			--NC o devolución que relaciona a factura o nd
			SELECT 2 orden,
				ap.aptodcty+2, ap.aptodcnm,
				ap.APTODCDT,
				isnull(isnull(isnull(u.UUID, rtrim(nt.uuid)), rtrim(usop.uuid)), 'No existe uuid') UUID, 
				isnull(u.voidstts, nt.voidstts) voidstts
			from dbo.vwRmTrxAplicadas  ap
				outer apply dbo.fnCfdiObtieneUUID(ap.aptodcty+2, ap.aptodcnm) u	
				outer apply dbo.fnCfdiObtieneUUIDDeAR(ap.aptodcty, ap.aptodcnm, ap.custnmbr) nt	--tipo factura es 1 en AR
				outer apply dbo.fnCfdiObtieneUUIDDeSOP(ap.aptodcty+2, ap.aptodcnm) usop			--tipo factura es 1 en AR
			where ap.APFRDCTY = @soptype+4										--tipo nc es 8 en AR
			AND ap.apfrdcnm = @p_sopnumbe
			and @soptype = 4
)	
go

IF (@@Error = 0) PRINT 'Creación exitosa de: fnCfdiRelacionados()'
ELSE PRINT 'Error en la creación de: fnCfdiRelacionados()'
GO

--------------------------------------------------------------------------------------------------------
