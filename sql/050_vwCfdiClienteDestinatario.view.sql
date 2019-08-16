----------------------------------------------------------------------------------------------------------
IF (OBJECT_ID ('dbo.vwCfdiClienteDestinatario', 'V') IS NULL)
   exec('create view dbo.vwCfdiClienteDestinatario as SELECT 1 as t');
go

alter view dbo.vwCfdiClienteDestinatario 
as
--Propósito. Obtiene direcciones de correo del cliente
--Requisito. -
--14/08/19 jcf Creación cfdi Colombia ubl 2.1
--
	select dir.custnmbr, 
		case when isnull(prcliente.param1, '-') like 'no existe tag%' then '0' else isnull(prcliente.param1, '0') end cliente_canalEntrega, 
		dir.Email_Recipient cliente_email
	from dbo.rm00106 dir
	outer apply dbo.fCfdiParametrosCliente(dir.CUSTNMBR, 'CANALDENTREGA', 'NA', 'NA', 'NA', 'NA', 'NA', 'PREDETERMINADO') prcliente

go

IF (@@Error = 0) PRINT 'Creación exitosa de: vwCfdiClienteDestinatario()'
ELSE PRINT 'Error en la creación de: vwCfdiClienteDestinatario()'
GO
