----------------------------------------------------------------------------------------------------------
IF (OBJECT_ID ('dbo.vwCfdiFacturaImpuestosCabecera', 'V') IS NULL)
   exec('create view dbo.vwCfdiFacturaImpuestosCabecera as SELECT 1 as t');
go

alter view dbo.vwCfdiFacturaImpuestosCabecera 
as
--Propósito. Obtiene impuestos SOP
--Requisito. 
--13/08/19 jcf Creación cfdi Colombia ubl 2.1
--
	select sop.soptype, sop.sopnumbe, 
			rtrim(imp.cntcprsn)	codigoTotalImp, 
			imp.TXDTLPCT		porcentajeTotalImp, 
			sum(imp.staxamnt)	staxamnt, 
			sum(abs(imp.orslstax))	valorTotalImp, 
			sum(imp.tdttxsls)	tdttxsls, 
			sum(imp.ortxsls)	baseImponibleTotalImp,
			null				controlInterno,
			'WSD'				unidadMedida,
			null				unidadMedidaTributo,
			null				valorTributoUnidad
	from sop30200 sop
	cross apply dbo.fnCfdiImpuestosSop(sop.SOPNUMBE, sop.soptype , 0, '%', '%') imp
	group by sop.soptype, sop.sopnumbe, imp.cntcprsn, imp.TXDTLPCT

go

IF (@@Error = 0) PRINT 'Creación exitosa de: vwCfdiFacturaImpuestosCabecera()'
ELSE PRINT 'Error en la creación de: vwCfdiFacturaImpuestosCabecera()'
GO
----------------------------------------------------------------------------------------------------------
IF (OBJECT_ID ('dbo.vwCfdiFacturaImpuestosDetalles', 'V') IS NULL)
   exec('create view dbo.vwCfdiFacturaImpuestosDetalles as SELECT 1 as t');
go

alter view dbo.vwCfdiFacturaImpuestosDetalles 
as
--Propósito. Obtiene impuestos SOP
--Requisito. 
--13/08/19 jcf Creación cfdi Colombia ubl 2.1
--
	select sop.soptype, sop.sopnumbe, sop.LNITMSEQ, 
			sop.itemnmbr, sop.cmpntseq,
			rtrim(imp.cntcprsn)	codigoTotalImp, 
			imp.TXDTLPCT		porcentajeTotalImp, 

			sum(imp.staxamnt)	staxamnt, 
			sum(abs(imp.orslstax))	valorTotalImp, 
			sum(imp.tdttxsls)	tdttxsls, 
			sum(imp.ortxsls)	baseImponibleTotalImp,

			''					controlInterno,
			'WSD'				unidadMedida,
			''					unidadMedidaTributo,
			''					valorTributoUnidad
	from sop30300 sop
	cross apply dbo.fnCfdiImpuestosSop(sop.SOPNUMBE, sop.soptype , sop.LNITMSEQ, '%', '%') imp
	group by sop.soptype, sop.sopnumbe, sop.LNITMSEQ, sop.itemnmbr, sop.cmpntseq, imp.cntcprsn, imp.TXDTLPCT

go

IF (@@Error = 0) PRINT 'Creación exitosa de: vwCfdiFacturaImpuestosDetalles()'
ELSE PRINT 'Error en la creación de: vwCfdiFacturaImpuestosDetalles()'
GO


