IF (OBJECT_ID ('dbo.vwCfdiDatosDelXml', 'V') IS NULL)
   exec('create view dbo.vwCfdiDatosDelXml as SELECT 1 as t');
go

alter view dbo.vwCfdiDatosDelXml as
--Prop�sito. Lista los datos del xml emitido 
--14/08/19 jcf Creaci�n 
--
select lf.soptype, lf.sopnumbe, lf.secuencia, lf.estado, lf.mensaje, lf.estadoActual, lf.mensajeEA, 
	--Datos del xml sellado por el PAC:
	isnull(dx.UUID, '') UUID,
	isnull(dx.schemeName, '') schemeName

from dbo.cfdlogfacturaxml lf
	outer apply dbo.fnCfdiDatosDelXml(lf.archivoXML) dx

go
IF (@@Error = 0) PRINT 'Creaci�n exitosa de la vista: vwCfdiDatosDelXml  '
ELSE PRINT 'Error en la creaci�n de la vista: vwCfdiDatosDelXml '
GO
-----------------------------------------------------------------------------------------
