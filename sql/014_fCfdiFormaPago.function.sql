IF OBJECT_ID ('dbo.fnCfdiFormaPagoManual') IS NOT NULL
   DROP FUNCTION dbo.fnCfdiFormaPagoManual
GO

create function dbo.fnCfdiFormaPagoManual(@chekbkid varchar(15), @CSHRCTYP smallint, @FRTSCHID varchar(15), @longCodigoFormaPago int)
returns table
--Propósito. Obtiene la forma de pago de un recibo de cobro
--14/08/19 jcf Creación
--
as
return(
	select cm.chekbkid, 
			case when left(UPPER(cm.locatnid), 2) = 'CB' then	--ch representa una cuenta bancaria
 				case @CSHRCTYP  
 					when 0 then '23'	--cheque
 					when 1 then '10'	--efectivo
 					when 2 then left(@FRTSCHID, @longCodigoFormaPago)
					else null 
				end
			else												--representa un medio de pago
				left(Rtrim(cm.locatnid), @longCodigoFormaPago)
			end	FormaPago	
	from CM00100 cm
	where cm.chekbkid = @chekbkid
	union all
	select top(1) @chekbkid,  
			case @CSHRCTYP 
 				when 2 then left(@FRTSCHID, @longCodigoFormaPago)	--tarjeta
				else null 
			end
	from CM00100 cm
	where @chekbkid = ''
)
go
IF (@@Error = 0) PRINT 'Creación exitosa de: fnCfdiFormaPagoManual()'
ELSE PRINT 'Error en la creación de: fnCfdiFormaPagoManual()'
GO
--------------------------------------------------------------------------------------------------------

