IF (OBJECT_ID ('dbo.vwCfdiRelacionados', 'V') IS NULL)
   exec('create view dbo.[vwCfdiRelacionados] as SELECT 1 as t');
go

alter VIEW [dbo].[vwCfdiRelacionados]
--Prop�sito. Obtiene los documentos referenciados
--14/08/19 jcf Creaci�n
--
AS
	select rel.orden, 
	sop.commntid		tipoDocumento, 
	sop.SOPTYPE			soptypeFrom, 
	sop.SOPNUMBE		sopnumbeFrom,
	rel.doctype			soptypeTo,
	rel.docnumbr		sopnumbeTo,
	null				codigoInterno,
	4					discrepancyResponse,
	5					billingReference,
	rel.UUID			cufeDocReferenciado,
	sop.comment_1		cufeDescripcion,
	rel.docdate			fecha,
	rel.docnumbr		numeroDocumento,
	null				tipoCufe  
	from dbo.vwCfdiSopTransaccionesVenta sop
	 cross apply dbo.fnCfdiRelacionados(sop.soptype, sop.sopnumbe) rel

GO

IF (@@Error = 0) PRINT 'Creaci�n exitosa de la vista: [vwCfdiRelacionados]  '
ELSE PRINT 'Error en la creaci�n de la vista: [vwCfdiRelacionados] '
GO

