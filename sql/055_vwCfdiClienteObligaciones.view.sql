----------------------------------------------------------------------------------------------------------
IF (OBJECT_ID ('dbo.vwCfdiClienteObligaciones', 'V') IS NULL)
   exec('create view dbo.vwCfdiClienteObligaciones as SELECT 1 as t');
go

alter view dbo.vwCfdiClienteObligaciones 
as
--Propósito. Obtiene obligaciones impositivas del cliente
--Requisito. -
--14/08/19 jcf Creación cfdi Colombia ubl 2.1
--
SELECT cliXml.custnmbr,  isnull(cliXml.param2, '') cliente_regimen,
    case when isnull(Cliente.obligacion.value('.', 'VARCHAR(100)'), '') like 'no existe tag%' then 
		'' 
	else upper(isnull(Cliente.obligacion.value('.', 'VARCHAR(100)'), ''))
	end cliente_obligaciones  
 FROM  
 (
 	select cli.custnmbr, 
		prcliente.param2,
		CAST ('<M>' + REPLACE(prcliente.param1, ',', '</M><M>') + '</M>' AS XML) AS obligaciones  
	from dbo.rm00101 cli
	outer apply dbo.fCfdiParametrosCliente(cli.CUSTNMBR, 'OBLIGACIONESFIS', 'REGIMENFISCAL', 'NA', 'NA', 'NA', 'NA', 'PREDETERMINADO') prcliente

 ) cliXml 
 outer APPLY cliXml.obligaciones.nodes ('/M') AS Cliente(obligacion); 


go

IF (@@Error = 0) PRINT 'Creación exitosa de: vwCfdiClienteObligaciones()'
ELSE PRINT 'Error en la creación de: vwCfdiClienteObligaciones()'
GO
