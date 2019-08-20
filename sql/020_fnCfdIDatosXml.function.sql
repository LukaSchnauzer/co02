IF OBJECT_ID ('dbo.fnCfdiDatosDelXml') IS NOT NULL
   drop function dbo.fnCfdiDatosDelXml
go

create function dbo.fnCfdiDatosDelXml(@archivoXml xml)
--Prop�sito. Obtiene los datos de la factura electr�nica
--Usado por. 
--Requisitos. -
--14/08/19 jcf Creaci�n
--
returns table
return(
	WITH XMLNAMESPACES('http://www.dian.gov.co/contratos/facturaelectronica/v1' as "fe", 
						'urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2' as "cbc")
	select 
	@archivoXml.value('(//cbc:UUID)[1]', 'varchar(100)') UUID

	)
	go

IF (@@Error = 0) PRINT 'Creaci�n exitosa de: [fnCfdiDatosDelXml]()'
ELSE PRINT 'Error en la creaci�n de: [fnCfdiDatosDelXml]()'
GO

--------------------------------------------------------------------------------------
