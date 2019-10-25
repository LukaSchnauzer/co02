IF (OBJECT_ID ('dbo.vwCfdiDatosDelXml', 'V') IS NULL)
   exec('create view dbo.vwCfdiDatosDelXml as SELECT 1 as t');
go

alter view dbo.vwCfdiDatosDelXml as
--Propósito. Lista los datos del xml emitido 
--14/08/19 jcf Creación 
--
select lf.soptype, lf.sopnumbe, lf.secuencia, lf.estado, lf.mensaje, lf.estadoActual, lf.mensajeEA, 
	--Datos del xml sellado por el PAC:
	isnull(dx.UUID, '') UUID,
	isnull(dx.schemeName, '') schemeName

from dbo.cfdlogfacturaxml lf
	outer apply dbo.fnCfdiDatosDelXml(lf.archivoXML) dx

go
IF (@@Error = 0) PRINT 'Creación exitosa de la vista: vwCfdiDatosDelXml  '
ELSE PRINT 'Error en la creación de la vista: vwCfdiDatosDelXml '
GO
-----------------------------------------------------------------------------------------
