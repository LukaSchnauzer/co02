IF OBJECT_ID ('dbo.fnCfdiDatosDelXml') IS NOT NULL
   drop function dbo.fnCfdiDatosDelXml
go

create function dbo.fnCfdiDatosDelXml(@archivoXml xml)
--Prop�sito. Obtiene los datos de la factura electr�nica
--Usado por. 
--Requisitos. -
--14/08/19 jcf Creaci�n
--23/12/19 jcf El cufe est� dentro del elemento Description
--
returns table

return(
	-- @archivoXml.value('(//cbc:UUID)[1]', 'varchar(120)') UUID,
	-- @archivoXml.value('(//cbc:UUID/@schemeName)[1]', 'varchar(15)') schemeName
	
	WITH XMLNAMESPACES('http://www.dian.gov.co/contratos/facturaelectronica/v1' as "fe", 
				'urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2' as "cbc")
	select 
		invoice.descri.value('(//cbc:UUID)[1]', 'varchar(120)') UUID,
		invoice.descri.value('(//cbc:UUID/@schemeName)[1]', 'varchar(15)') schemeName
	from (
		select cast(@archivoXml.value('(//cbc:Description)[1]', 'varchar(max)') as xml) descri
		) invoice

	)
	go

IF (@@Error = 0) PRINT 'Creaci�n exitosa de: [fnCfdiDatosDelXml]()'
ELSE PRINT 'Error en la creaci�n de: [fnCfdiDatosDelXml]()'
GO

--------------------------------------------------------------------------------------

