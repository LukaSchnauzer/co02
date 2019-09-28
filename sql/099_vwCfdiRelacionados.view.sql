IF (OBJECT_ID ('dbo.vwCfdiRelacionados', 'V') IS NULL)
   exec('create view dbo.[vwCfdiRelacionados] as SELECT 1 as t');
go

alter VIEW [dbo].[vwCfdiRelacionados]
--Propósito. Obtiene los documentos referenciados
--14/08/19 jcf Creación
--
AS
	select rel.orden, 
	sop.SOPTYPE			soptypeFrom, 
	sop.SOPNUMBE		sopnumbeFrom,
	rel.doctype			soptypeTo,
	rel.docnumbr		sopnumbeTo,
	''					codigoInterno,
	4					discrepancyResponse,
	5					billingReference,
	rel.UUID			cufeDocReferenciado,
	case when isnumeric(substring(sop.commntid, 2, 2)) = 1 then
		convert(varchar(5), convert(int, substring(sop.commntid, 2, 2)))
	else substring(sop.commntid, 2, 2)
	end					codigoEstatusDocumento, 
	sop.comment_1		cufeDescripcion,
	rel.docdate			fecha,
	upper(left(ltrim(rel.docnumbr), convert(int, pr.param1)-1)) 
	+ convert(varchar(20), convert(int, substring(ltrim(rel.docnumbr), convert(int, pr.param1), 20))) numeroDocumento,	--serie + número sin ceros a la izquierda
	''					tipoDocumento,
	case when rel.doctype = 3 then 'CUFE-SHA384' else 'CUDE-SHA384' end  tipoCufe
	from dbo.vwCfdiSopTransaccionesVenta sop
	 cross apply dbo.fnCfdiRelacionados(sop.soptype, sop.sopnumbe) rel
	 outer apply dbo.fCfdiParametros('V_ININUMEROFAC', 'NA', 'NA', 'NA', 'na', 'na', 'FECOL') pr	--posición donde inicia el número de la factura

GO

IF (@@Error = 0) PRINT 'Creación exitosa de la vista: [vwCfdiRelacionados]  '
ELSE PRINT 'Error en la creación de la vista: [vwCfdiRelacionados] '
GO

