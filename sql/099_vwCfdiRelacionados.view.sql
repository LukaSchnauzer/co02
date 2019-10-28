IF (OBJECT_ID ('dbo.vwCfdiRelacionados', 'V') IS NULL)
   exec('create view dbo.[vwCfdiRelacionados] as SELECT 1 as t');
go

alter VIEW [dbo].[vwCfdiRelacionados]
--Prop�sito. Obtiene los documentos referenciados
--14/08/19 jcf Creaci�n
--
AS
	select isnull(rel.orden, 1)		orden, 
	sop.SOPTYPE						soptypeFrom, 
	sop.SOPNUMBE					sopnumbeFrom,
	isnull(rel.doctype, 0)			soptypeTo,
	isnull(rtrim(rel.docnumbr), '') sopnumbeTo,
	''								codigoInterno,
	4								discrepancyResponse,
	5								billingReference,
	isnull(rel.UUID, '')			cufeDocReferenciado,
	case when isnumeric(substring(sop.commntid, 2, 2)) = 1 then
		convert(varchar(5), convert(int, substring(sop.commntid, 2, 2)))
	else substring(sop.commntid, 2, 2)
	end								codigoEstatusDocumento, 
	sop.comment_1					cufeDescripcion,
	sop.commntid,
	isnull(rel.docdate, '1/1/1900')	fecha,
	isnull(
		upper(left(ltrim(rel.docnumbr), convert(int, pr.param1)-1)) 
			+ convert(varchar(20), convert(int, substring(ltrim(rel.docnumbr), convert(int, pr.param1), 20))) 	
		, '')						numeroDocumento,	--serie + n�mero sin ceros a la izquierda
	''								tipoDocumento,
	isnull(rel.schemeName, '')		tipoCufe
	from dbo.vwCfdiSopTransaccionesVenta sop
	 outer apply dbo.fnCfdiRelacionados(sop.soptype, sop.sopnumbe) rel
	 outer apply dbo.fCfdiParametros('V_ININUMEROFAC', 'NA', 'NA', 'NA', 'na', 'na', 'FECOL') pr	--posici�n donde inicia el n�mero de la factura

GO

IF (@@Error = 0) PRINT 'Creaci�n exitosa de la vista: [vwCfdiRelacionados]  '
ELSE PRINT 'Error en la creaci�n de la vista: [vwCfdiRelacionados] '
GO

