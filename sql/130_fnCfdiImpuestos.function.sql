----------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.fnCfdiImpuestosSop') IS NOT NULL
   DROP FUNCTION dbo.fnCfdiImpuestosSop
GO

create function dbo.fnCfdiImpuestosSop(@SOPNUMBE char(21), @DOCTYPE smallint, @LNITMSEQ int, @prefijo varchar(15), @tipoTributo varchar(10))
returns table
as
--Propósito. Detalle de impuestos en trabajo e históricos de SOP. Filtra los impuestos requeridos por @prefijo
--Requisitos. Los impuestos iva deben ser configurados con un prefijo constante
--27/11/17 jcf Creación 
--13/08/18 jcf Agrega txdtlbse
--14/11/18 jcf Ajustes para ubl2.1
--
return
(
	select imp.soptype, imp.sopnumbe, imp.taxdtlid, imp.staxamnt, imp.orslstax, imp.tdttxsls, imp.ortxsls,
			tx.NAME, tx.cntcprsn, tx.TXDTLPCT, tx.txdtlbse, tx.address1
	from sop10105 imp
		inner join tx00201 tx
		on tx.taxdtlid = imp.taxdtlid
		and tx.cntcprsn like @tipoTributo
	where imp.sopnumbe = @SOPNUMBE
	and imp.soptype = @DOCTYPE
	and imp.LNITMSEQ = @LNITMSEQ
	and imp.taxdtlid like @prefijo + '%'
)

go


IF (@@Error = 0) PRINT 'Creación exitosa de la función: fnCfdiImpuestosSop()'
ELSE PRINT 'Error en la creación de la función: fnCfdiImpuestosSop()'
GO

----------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.fnCfdiSumaImpuestosSop') IS NOT NULL
   DROP FUNCTION dbo.fnCfdiSumaImpuestosSop
GO

create function dbo.fnCfdiSumaImpuestosSop(@SOPNUMBE char(21), @DOCTYPE smallint, @LNITMSEQ int, @prefijo varchar(15), @tipoTributo varchar(10), @tipoAfectacion varchar(2))
returns table
as
--Propósito. Agrupa los impuestos en trabajo e históricos de SOP. Filtra los impuestos requeridos por @prefijo
--Requisitos. -
--13/08/19 jcf Creación ubl2.1
--
return
(
	select sum(imp.staxamnt) staxamnt, sum(imp.orslstax) orslstax, sum(abs(imp.staxamnt)) staxamntAbs, sum(abs(imp.orslstax)) orslstaxAbs, 
			sum(imp.tdttxsls) tdttxsls, sum(imp.ortxsls) ortxsls,
			sum(case when imp.staxamnt < 0 then imp.staxamnt else 0 end) staxamntNegativo,
			sum(case when imp.staxamnt < 0 then imp.orslstax else 0 end) orslstaxNegativo,
			sum(case when imp.staxamnt < 0 then imp.tdttxsls else 0 end) tdttxslsNegativo,
			sum(case when imp.staxamnt < 0 then imp.ortxsls else 0 end) ortxslsNegativo
	from dbo.fnCfdiImpuestosSop(@SOPNUMBE, @DOCTYPE, @LNITMSEQ,  @prefijo , @tipoTributo) imp
	where imp.name like @tipoAfectacion
	and imp.staxamnt != 0
)

go


IF (@@Error = 0) PRINT 'Creación exitosa de la función: fnCfdiSumaImpuestosSop()'
ELSE PRINT 'Error en la creación de la función: fnCfdiSumaImpuestosSop()'
GO

----------------------------------------------------------------------------------------------------
--IF OBJECT_ID ('dbo.fnCfdiSumaImpuestosNegativosSop') IS NOT NULL
--   DROP FUNCTION dbo.fnCfdiSumaImpuestosNegativosSop
--GO

--create function dbo.fnCfdiSumaImpuestosNegativosSop(@SOPNUMBE char(21), @DOCTYPE smallint, @LNITMSEQ int, @prefijo varchar(15), @tipoTributo varchar(10), @tipoAfectacion varchar(2))
--returns table
--as
----Propósito. Agrupa los impuestos en trabajo e históricos de SOP. Filtra los impuestos negativos por @prefijo
----Requisitos. -
----13/09/19 jcf Creación ubl2.1
----
--return
--(
--	select sum(imp.staxamnt) staxamnt, sum(imp.orslstax) orslstax, sum(imp.tdttxsls) tdttxsls, sum(imp.ortxsls) ortxsls
--	from dbo.fnCfdiImpuestosSop(@SOPNUMBE, @DOCTYPE, @LNITMSEQ,  @prefijo , @tipoTributo) imp
--	where imp.name like @tipoAfectacion
--	and imp.staxamnt < 0
--)

--go


--IF (@@Error = 0) PRINT 'Creación exitosa de la función: fnCfdiSumaImpuestosNegativosSop()'
--ELSE PRINT 'Error en la creación de la función: fnCfdiSumaImpuestosNegativosSop()'
--GO
