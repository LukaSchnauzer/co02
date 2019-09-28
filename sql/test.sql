select * 
from SY04200

insert into SY04200(cmtsries, commntid, CMMTTEXT) 
values (3, '03-ERR DESCRIPC', '')

SELECT TOP 100 *
FROM dbo.vwCfdiGeneraDocumentoDeVenta tv
where sopnumbe = 'TISC00000011'

select *
from vwCfdiTransaccionesDeVenta
where sopnumbe IN (
'TISC00000011'
)

select *
from vwCfdiRelacionados
where sopnumbeFrom = 'TISC00000011'

SELECT *
from dbo.fCfdiCertificadoVigente('7/30/18') fv

select *
--update c set estadoActual='000101', mensajeEA = 'Xml emitido. Pdf impreso.'	--estado='impreso'	-- 	-- noAprobacion = '1513019407979'
--delete c
from cfdlogfacturaxml c
where 
--c.sopnumbe like '%TISC00000011'
--estado = 'emitido'
 c.sopnumbe in (
 'F001-00000060'
-- ,
-- 'F001-00000011',
--,'F001-00000008'
)
--like 'TISC00000011%'
--and secuencia = 1187
--and c.soptype = 3
--and estado = 'emitido'

select sopnumbe, count(*)
from sop30300
where sopnumbe like 'F00%'
group by sopnumbe
having count(*) > 5

order by sopnumbe


select *
from dbo.fLcLvParametros('V_PREFEXONERADO', 'V_PREFEXENTO', 'V_PREFIVA', 'V_GRATIS', 'na', 'na') pr	--Parámetros. prefijo inafectos, prefijo exento, prefijo iva

select *
from dbo.fCfdiImpuestosSop( 'TISC00000011        ', 3, 0, 'V-GRATIS', '02') gra	--gratuito

select *
from dbo.fnCfdiSumaImpuestosSop('TISF00000003', 3, 0, '%', '%', '%') sumaImpuestos


select *
from vwCfdiConceptos
where sopnumbe = 'TISC00000011        '


select *
from vwCfdiSopTransaccionesVenta
where sopnumbe like 'F001%'	--'TISC00000011'
ORDER BY SOPNUMBE

select *
from vwCfdiGeneraResumenDiario


select *
from dbo.fnCfdGetDireccionesCorreo('HL901436')

	select *
	from rm00106	--rmStmtEmailAddrs 1:To, 2:CC, 3:CCO
	where custnmbr = ''	--'003098841'



select *
from dbo.fCfdiPagoSimultaneoMayor(3, 'FV 00000247', 1) pg


select *
from dbo.sop10106
where sopnumbe LIKE 'TISF00000016'

select docncorr, *
--update s set docncorr = '10:23:10:060'
from sop10100 s
where s.sopnumbe like 'B%'

--ingresar la causa de la nc
select docncorr, commntid,  *
update s set commntid = '009'	--'01ANULA OPER', refrence = 'x'	-- docncorr = '10:23:10:060'
from sop30200 s
where s.soptype = 3
and s.sopnumbe like 'TISF00000016'

--ingresar la causa de la nc
insert into sop10106 (SOPTYPE,SOPNUMBE,USRDAT01,USRDAT02,USRTAB01,USRTAB09,USRTAB03,USERDEF1,USERDEF2,USRDEF03,USRDEF04,USRDEF05,COMMENT_1,COMMENT_2,COMMENT_3,COMMENT_4,CMMTTEXT)
values (3, 'TISF00000016', 0, 0, '', '', '', '', '', '', '', '', 'Descuento General', '', '', '', 'Descuento General')

--------------------------------------------------------------------------------
--Para eliminar un número que se pueda reusar seguir el procedimiento: C:\jcTii\SW\SWDynamicsGP\KnowledgeBase\Completely Removing a Posted SOP Invoice From Dynamics GP.html
SELECT * --into _181026_tx30000
--DELETE TX
FROM TX30000 TX
WHERE 
DOCTYPE = 3 AND SERIES = 1 
AND DOCNUMBR in --= 'BNC1-0000005'
(
'TISC00000011'
--'BNC1-00000007',
--'BNC1-00000008',
--'TISC00000011'
)

select *
from sop30200
where sopnumbe in
(
'TISC00000011',
'BNC1-00000007',
'BNC1-00000008',
'TISC00000011'
)


select *
from rm20101
where docnumbr in
(
'TISC00000011',
'BNC1-00000007',
'BNC1-00000008',
'TISC00000011'
)
----------------------------------------------------------------------------------------------
--agregar parámetros de compañía
select inetinfo, *
--UPDATE ia set inetinfo = convert(varchar(100), inetinfo) + 'CTABNACION=335544112233'+char(13)
from SY01200 ia
where master_id = 'PER10'
and adrscode = 'PRINCIPAL'