IF OBJECT_ID ('dbo.fnCfdiObtieneUUIDDeSOP') IS NOT NULL
   DROP FUNCTION dbo.fnCfdiObtieneUUIDDeSOP
GO

create function dbo.fnCfdiObtieneUUIDDeSOP(@SOPTYPE smallint, @SOPNUMBE varchar(21))
returns table
as
--Propósito. Devuelve el UUID de una factura sop. Este dato debe estar en el campo nota.
--Requisitos. Se debería usar en caso que la factura fue emitida en gestiones anteriores por otro PAC
--16/08/19/19 jcf Creación 
--
return
(
	select substring(ni.txtfield, 1, 40) uuid, rmx.voidstts
		from dbo.sop30200 rmx
		inner join sy03900 ni
			on ni.noteindx = rmx.noteindx
	where rmx.soptype = 3
		and rmx.sopnumbe = @SOPNUMBE
)
go


IF (@@Error = 0) PRINT 'Creación exitosa de la función: fnCfdiObtieneUUIDDeSOP()'
ELSE PRINT 'Error en la creación de la función: fnCfdiObtieneUUIDDeSOP()'
GO

-------------------------------------------------------------------------------------------------------------
--select *
--from dbo.fnCfdiObtieneUUIDDeSOP(3, '00000002')

