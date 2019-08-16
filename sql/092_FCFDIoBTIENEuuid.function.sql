IF OBJECT_ID ('dbo.fnCfdiObtieneUUID') IS NOT NULL
   DROP FUNCTION dbo.fnCfdiObtieneUUID
GO

create function dbo.fnCfdiObtieneUUID(@soptype smallint, @sopnumbe varchar(21))
returns table
as
--Prop�sito. Devuelve el UUID de un cfdi
--Requisitos. 
--14/08/19 jcf Creaci�n 
--
return
(
	select tv.docid, tv.docdate, dx.uuid, tv.voidstts
	from dbo.vwCfdiSopTransaccionesVenta tv
		left join dbo.vwCfdiDatosDelXml dx
		on dx.soptype = tv.SOPTYPE
		and dx.sopnumbe = tv.sopnumbe
		and dx.estado = 'emitido'
	where tv.soptype = @soptype
	and tv.sopnumbe = @sopnumbe
)
go


IF (@@Error = 0) PRINT 'Creaci�n exitosa de la funci�n: fnCfdiObtieneUUID()'
ELSE PRINT 'Error en la creaci�n de la funci�n: fnCfdiObtieneUUID()'
GO

-------------------------------------------------------------------------------------------------------------
--select *
--from dbo.fnCfdiObtieneUUID(3, '00000002')

