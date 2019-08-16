IF OBJECT_ID ('dbo.fnCfdiObtieneUUIDDeAR') IS NOT NULL
   DROP FUNCTION dbo.fnCfdiObtieneUUIDDeAR
GO

create function dbo.fnCfdiObtieneUUIDDeAR(@RMDTYPAL smallint, @DOCNUMBR varchar(21), @CUSTNMBR varchar(15))
returns table
as
--Propósito. Devuelve el UUID de una factura AR. Este dato debe estar en el campo nota.
--Requisitos. 
--16/08/19 jcf Creación 
--
return
(
	select substring(ni.txtfield, 1, 40) uuid, rmx.voidstts
		from dbo.vwRmTransaccionesTodas rmx
		inner join sy03900 ni
			on ni.noteindx = rmx.noteindx
	where rmx.RMDTYPAL = 1			-- 1 invoice
		and rmx.rmdTypAl = @RMDTYPAL
       and rmx.DOCNUMBR = @DOCNUMBR
	   and rmx.custnmbr = @CUSTNMBR
)
go


IF (@@Error = 0) PRINT 'Creación exitosa de la función: fnCfdiObtieneUUIDDeAR()'
ELSE PRINT 'Error en la creación de la función: fnCfdiObtieneUUIDDeAR()'
GO

-------------------------------------------------------------------------------------------------------------
--select *
--from dbo.fnCfdiObtieneUUIDDeAR(3, '00000002')

