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
	null				codigoInterno,
	4					discrepancyResponse,
	5					billingReference,
	rel.UUID			cufeDocReferenciado,
	substring(sop.commntid, 2, 2)	codigoEstatusDocumento, 
	sop.comment_1		cufeDescripcion,
	rel.docdate			fecha,
	rel.docnumbr		numeroDocumento,
	null				tipoDocumento,
	case when rel.doctype = 3 then 'CUFE-SHA384' else 'CUDE-SHA384' end  tipoCufe  
	from dbo.vwCfdiSopTransaccionesVenta sop
	 cross apply dbo.fnCfdiRelacionados(sop.soptype, sop.sopnumbe) rel

GO

IF (@@Error = 0) PRINT 'Creación exitosa de la vista: [vwCfdiRelacionados]  '
ELSE PRINT 'Error en la creación de la vista: [vwCfdiRelacionados] '
GO

