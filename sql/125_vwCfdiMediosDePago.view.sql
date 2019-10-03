IF (OBJECT_ID ('dbo.vwCfdiMediosDePago', 'V') IS NULL)
   exec('create view dbo.[vwCfdiMediosDePago] as SELECT 1 as t');
go

alter VIEW [dbo].[vwCfdiMediosDePago]
--Propósito. Obtiene los medios de pago de facturas
--14/08/19 Creación
--
AS
SELECT  apl.APTODCTY, apl.APTODCTY +2 soptype, apl.APTODCNM sopnumbe,
		apl.APFRDCTY, apl.APFRDCNM,
		case when ISNUMERIC(pago.medioDePago) <> 1 then
			pago.medioDePago
		else convert(varchar(10), convert(int, pago.medioDePago))
		end mediopago, 
		'1' metodopago, --contado
		case when rtrim(isnull(rmx.cheknmbr, '')) = '' then 'na' else rtrim(rmx.cheknmbr) end numeroreferencia
FROM    dbo.vwCfdiRmTrxAplicadas apl
		cross apply [dbo].fnCfdiDocumentoDePago (apl.APFRDCTY, apl.APFRDCNM) pago
		left outer join vwRmTransaccionesTodas rmx
             ON rmx.RMDTYPAL = apl.apfrdcty
			 and rmx.docnumbr = apl.apfrdcnm

GO

IF (@@Error = 0) PRINT 'Creación exitosa de la vista: [vwCfdiMediosDePago]  '
ELSE PRINT 'Error en la creación de la vista: [vwCfdiMediosDePago] '
GO
