IF OBJECT_ID ('dbo.fnCfdiDatosDelXml') IS NOT NULL
   drop function dbo.fnCfdiDatosDelXml
go

create function dbo.fnCfdiDatosDelXml(@archivoXml xml)
--Propósito. Obtiene los datos de la factura electrónica
--Usado por. 
--Requisitos. -
--14/08/19 jcf Creación
--
returns table
return(
	WITH XMLNAMESPACES('http://www.dian.gov.co/contratos/facturaelectronica/v1' as "fe", 
						'urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2' as "cbc")
	select 
	@archivoXml.value('(//cbc:UUID)[1]', 'varchar(100)') UUID

	)
	go

IF (@@Error = 0) PRINT 'Creación exitosa de: [fnCfdiDatosDelXml]()'
ELSE PRINT 'Error en la creación de: [fnCfdiDatosDelXml]()'
GO

--------------------------------------------------------------------------------------
