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
			--NC o devolución que relaciona a factura o nd
			SELECT 1 orden,
				ap.aptodcty+2 doctype, ap.aptodcnm docnumbr,
				ap.APTODCDT docdate,
				isnull(isnull(isnull(u.UUID, rtrim(nt.uuid)), rtrim(usop.uuid)), 'No existe uuid') UUID, 
				isnull(u.schemeName, 'CUFE-SHA384') schemeName,
				isnull(u.voidstts, nt.voidstts) voidstts,
				crds.cargosdescuentos_codigo, crds.cargosdescuentos_descripcion, crds.cargosdescuentos_indicador
			from dbo.vwRmTrxAplicadas  ap
				outer apply dbo.fnCfdiObtieneUUID(ap.aptodcty+2, ap.aptodcnm) u	
				outer apply dbo.fnCfdiObtieneUUIDDeAR(ap.aptodcty, ap.aptodcnm, ap.custnmbr) nt	--tipo factura es 1 en AR
				outer apply dbo.fnCfdiObtieneUUIDDeSOP(ap.aptodcty+2, ap.aptodcnm) usop			--tipo factura es 1 en AR
				outer apply (select substring(sop.commntid, 2, 2)	cargosdescuentos_codigo, 
									rtrim(da.comment_1)				cargosdescuentos_descripcion,
									left(sop.commntid, 1)			cargosdescuentos_indicador
							from sop30200 sop
								outer apply dbo.fCfdiDatosAdicionales(sop.soptype, sop.sopnumbe) da
							where soptype = ap.aptodcty+2 
							and sopnumbe = ap.aptodcnm) crds
			where ap.APFRDCTY = @soptype+4										--tipo nc es 8 en AR
			AND ap.apfrdcnm = @p_sopnumbe
			and @soptype = 4

			union all

			--relaciona a su mismo tipo de documento. 
			select top(1) 2 orden,
				da.soptype doctype, da.tracking_number docnumbr, 
				u.docdate,
				isnull(u.UUID, 'no existe uuid') UUID, 
				isnull(u.schemeName, 'CUDE-SHA384' ) schemeName,
				u.voidstts,
				'', '', ''
			from sop10107 da	--
				outer apply dbo.fnCfdiObtieneUUID(da.soptype, da.tracking_number) u
			where da.sopnumbe = @p_sopnumbe
			and da.soptype = @soptype
			and da.soptype = 3	--facturas
			
)	
go

IF (@@Error = 0) PRINT 'Creación exitosa de: fnCfdiRelacionados()'
ELSE PRINT 'Error en la creación de: fnCfdiRelacionados()'
GO

--------------------------------------------------------------------------------------------------------
