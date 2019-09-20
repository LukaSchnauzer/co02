--FACTURA ELECTRONICA GP - COLOMBIA
--Proyectos:		GETTY
--Propósito:		Genera funciones y vistas de FACTURAS para la facturación electrónica en GP - COLOMBIA
--Referencia:		
--		12/08/19 Versión CFDI UBL 2.1
--Utilizado por:	Aplicación C# de generación de factura electrónica COLOMBIA
-------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.fCfdiCertificadoVigente') IS NOT NULL
   DROP FUNCTION dbo.fCfdiCertificadoVigente
GO

create function dbo.fCfdiCertificadoVigente(@fecha datetime)
returns table
as
--Propósito. Verifica que la fecha corresponde a un certificado vigente y activo
--			Si existe más de uno o ninguno, devuelve el estado: inconsistente
--			También devuelve datos del folio y certificado asociado.
--Requisitos. Los estados posibles para generar o no archivos xml son: no emitido, inconsistente
--06/11/17 jcf Creación cfdi Perú
--
return
(  
	--declare @fecha datetime
	--select @fecha = '1/4/12'
	select top 1 --fyc.noAprobacion, fyc.anoAprobacion, 
			fyc.ID_Certificado, fyc.ruta_certificado, fyc.ruta_clave, fyc.contrasenia_clave, fyc.fila, 
			case when fyc.fila > 1 then 'inconsistente' else 'no emitido' end estado
	from (
		SELECT top 2 rtrim(B.ID_Certificado) ID_Certificado, rtrim(B.ruta_certificado) ruta_certificado, rtrim(B.ruta_clave) ruta_clave, 
				rtrim(B.contrasenia_clave) contrasenia_clave, row_number() over (order by B.ID_Certificado) fila
		FROM cfd_CER00100 B
		WHERE B.estado = '1'
			and B.id_certificado <> 'PAC'	--El id PAC está reservado para el PAC
			and datediff(day, B.fecha_vig_desde, @fecha) >= 0
			and datediff(day, B.fecha_vig_hasta, @fecha) <= 0
		) fyc
	order by fyc.fila desc
)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: fCfdiCertificadoVigente()'
ELSE PRINT 'Error en la creación de la función: fCfdiCertificadoVigente()'
GO

--------------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.fCfdiCertificadoPAC') IS NOT NULL
   DROP FUNCTION dbo.fCfdiCertificadoPAC
GO

create function dbo.fCfdiCertificadoPAC(@fecha datetime)
returns table
as
--Propósito. Obtiene el certificado del PAC. 
--			Verifica que la fecha corresponde a un certificado vigente y activo
--Requisitos. El id PAC está reservado para registrar el certificado del PAC. 
--06/11/17 jcf Creación 
--
return
(  
	--declare @fecha datetime
	--select @fecha = '5/4/12'
	SELECT rtrim(B.ID_Certificado) ID_Certificado, rtrim(B.ruta_certificado) ruta_certificado, rtrim(B.ruta_clave) ruta_clave, 
			rtrim(B.contrasenia_clave) contrasenia_clave
	FROM cfd_CER00100 B
	WHERE B.estado = '1'
		and B.id_certificado = 'PAC'	--El id PAC está reservado para el PAC
		and datediff(day, B.fecha_vig_desde, @fecha) >= 0
		and datediff(day, B.fecha_vig_hasta, @fecha) <= 0
)
go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: fCfdiCertificadoPAC()'
ELSE PRINT 'Error en la creación de la función: fCfdiCertificadoPAC()'
GO

--------------------------------------------------------------------------------------------------------

IF (OBJECT_ID ('dbo.vwCfdiSopLineasTrxVentas', 'V') IS NULL)
   exec('create view dbo.vwCfdiSopLineasTrxVentas as SELECT 1 as t');
go

alter view dbo.vwCfdiSopLineasTrxVentas as
--Propósito. Obtiene todas las líneas de facturas de venta SOP
--			Incluye descuentos
--Requisito. Atención ! DEBE usar unidades de medida listadas en el SERVICIO DE IMPUESTOS. 
--30/11/17 JCF Creación cfdi 3.3
--16/01/19 jcf Reemplaza caracteres no imprimibles en itemdesc
--
select dt.soptype, dt.sopnumbe, dt.LNITMSEQ, dt.ITEMNMBR, rtrim(ma.itmshnam) itmshnam, dt.ShipToName,
	dt.QUANTITY, dt.UOFM,
	um.UOFMLONGDESC UOFMsat,
	udmfa.descripcion UOFMsat_descripcion,
	um.UOFMLONGDESC, 
	dbo.fCfdReemplazaCaracteresNI(dt.ITEMDESC) ITEMDESC,
	dt.ORUNTPRC, dt.OXTNDPRC, dt.CMPNTSEQ, 
	dt.QUANTITY * dt.ORUNTPRC cantidadPorPrecioOri, 
	isnull(ma.ITMTRKOP, 1) ITMTRKOP,		--3 lote, 2 serie, 1 nada
	ma.uscatvls_5, 
	ma.uscatvls_6, 
	dt.ormrkdam,
	dt.QUANTITY * dt.ormrkdam descuento
from SOP30300 dt
left join iv00101 ma				--iv_itm_mstr
	on ma.ITEMNMBR = dt.ITEMNMBR
outer apply dbo.fCfdiUofM(ma.UOMSCHDL, dt.UOFM ) um
outer apply dbo.fCfdiCatalogoGetDescripcion('UDM', um.UOFMLONGDESC) udmfa

go	

IF (@@Error = 0) PRINT 'Creación exitosa de: vwCfdiSopLineasTrxVentas'
ELSE PRINT 'Error en la creación de: vwCfdiSopLineasTrxVentas'
GO
----------------------------------------------------------------------------------------------------------
IF (OBJECT_ID ('dbo.vwCfdiConceptos', 'V') IS NULL)
   exec('create view dbo.vwCfdiConceptos as SELECT 1 as t');
go

alter view dbo.vwCfdiConceptos 
as
--Propósito. Obtiene las líneas de una factura 
--			Elimina carriage returns, line feeds, tabs, secuencias de espacios y caracteres especiales.
--Requisito. 
--12/08/19 jcf Creación cfdi Colombia ubl 2.1
--
		select  ROW_NUMBER() OVER(partition by Concepto.soptype, Concepto.sopnumbe ORDER BY Concepto.LNITMSEQ asc) facturadetalle_secuencia,
			Concepto.soptype, Concepto.sopnumbe, Concepto.LNITMSEQ, rtrim(Concepto.ITEMNMBR) ITEMNMBR, '' SERLTNUM, 
			Concepto.CMPNTSEQ, 
			Concepto.QUANTITY		facturadetalle_cantidadreal, 
			rtrim(Concepto.UOFMsat) facturadetalle_cantidadrealunidadmedida,
			Concepto.QUANTITY		facturadetalle_cantidadunidades,
			0						facturadetalle_cantidadporempaque,

			''						cargosdescuentos_codigo,
			'-'						cargosdescuentos_descripcion,
			0						cargosdescuentos_indicador,
			Concepto.descuento		cargosdescuentos_monto,
			Concepto.OXTNDPRC		cargosdescuentos_montobase,
			Concepto.descuento/Concepto.OXTNDPRC	cargodescuentos_porcentaje,
			'1'						cargosdescuentos_secuencia,

			rtrim(Concepto.ITEMNMBR) facturadetalle_codigoproducto,
			dbo.fCfdReemplazaSecuenciaDeEspacios(ltrim(rtrim(dbo.fCfdReemplazaCaracteresNI(Concepto.ITEMDESC))), 10) facturadetalle_descripcion,
			
			pr.param1					facturadetalle_estandarcodigo,
			case when pr.param1 = '999' then 
				rtrim(Concepto.ITEMNMBR)
			else Concepto.itmshnam
			end						facturadetalle_estandarcodigoproducto,

			Concepto.uscatvls_5		facturadetalle_marca,
			Concepto.uscatvls_6		facturadetalle_modelo,
			Concepto.OXTNDPRC + isnull(sumaImpuestos.orslstax, 0) facturadetalle_precioTotal,
			Concepto.OXTNDPRC		facturadetalle_precioTotalSinImpuestos,
			Concepto.ORUNTPRC		facturadetalle_precioVentaUnitario,
			rtrim(Concepto.UOFMsat)	facturadetalle_unidadMedida

		from vwCfdiSopLineasTrxVentas Concepto
			outer apply dbo.fCfdiParametros('ESTANDARCODIV', 'na', 'na', 'na', 'na', 'na', 'FECOL') pr	--Parámetros. estándar de código IV
			outer apply dbo.fnCfdiSumaImpuestosSop(Concepto.sopnumbe, Concepto.soptype, Concepto.LNITMSEQ, '%', '%', '%') sumaImpuestos
		where Concepto.CMPNTSEQ = 0					--a nivel kit
		and Concepto.QUANTITY != 0

go

IF (@@Error = 0) PRINT 'Creación exitosa de: vwCfdiConceptos()'
ELSE PRINT 'Error en la creación de: vwCfdiConceptos()'
GO
-----------------------------------------------------------------------------------------
IF (OBJECT_ID ('dbo.vwCfdiGeneraDocumentoDeVenta', 'V') IS NULL)
   exec('create view dbo.vwCfdiGeneraDocumentoDeVenta as SELECT 1 as t');
go

alter view dbo.vwCfdiGeneraDocumentoDeVenta
as
--Propósito. Obtiene los datos para la clase FacturaGeneral del web service de The Factory Colombia
--Requisitos.  
--08/08/19 jcf Creación cfdi Colombia
--
	select 
		tv.soptype,
		tv.sopnumbe,
		tv.custnmbr,
		convert(int, parametros.param1)	cantidadDecimales, 
		convert(varchar(20), convert(int, substring(tv.sopnumbe, convert(int, parametros.param5), 20))) consecutivoDocumento,
		upper(left(tv.sopnumbe, convert(int, parametros.param5)-1)) prefijo,
		--Clase CargosDescuentos:
		substring(tv.commntid, 2, 2)	cargosdescuentos_codigo, 
		tv.comment_1					cargosdescuentos_descripcion,
		left(tv.commntid, 1)			cargosdescuentos_indicador,
		tv.ORTDISAM						cargosdescuentos_monto,
		tv.ORSUBTOT						cargosdescuentos_montobase,
		tv.ORTDISAM / 
			case when tv.ORSUBTOT=0
			then 1 
			else tv.ORSUBTOT 
			end							cargodescuentos_porcentaje,
		'1' 							cargosdescuentos_secuencia,
		--Clase Cliente
		tv.idImpuestoCliente			cliente_nitProveedorReceptor,
		tv.phone1						cliente_telefono,
		--Clase Dirección fiscal del cliente
		catCiudad.descripcion			cliente_difCiudad,
		tv.stateCode					cliente_difcodigoDepartamento,
		catDepartamento.descripcion		cliente_difdepartamento,
		left(tv.address1 +' '+ tv.address2, 100)	cliente_difdireccion,
		'es'							cliente_diflenguaje,
		tv.cityCode						cliente_difmunicipio,
		tv.countryCode					cliente_difpais, 
		tv.zipcode						cliente_difzonapostal, 
		rtrim(unEmail.Email_Recipient)	cliente_email,

		 --Información legal cliente
		tv.nombreCliente				cliente_nombreRegistroRUT,

		case when tv.TXRGNNUM = '' then
			nitTercero.numeroIdentificacion
		else reverse(substring(reverse(tv.idImpuestoCliente), 2, 30))	
		end								cliente_numeroIdentificacion,
		reverse(
			substring(
				reverse(tv.idImpuestoCliente)
				, 2, 30))				cliente_numeroDocumento,

		case when tv.TXRGNNUM = '' then
			nitTercero.numeroIdentificacionDV
		else left(reverse(tv.idImpuestoCliente), 1) 
		end								cliente_numeroIdentificacionDV,

		nitTercero.nsaif_type_nit		cliente_tipoIdentificacion,
		''								cliente_nombreComercial,
		tv.nombreCliente				cliente_nombreRazonSocial,
		case when tv.send_email_statements=1 then 'SI' else 'NO' end cliente_notificar,
		left(tv.userdef2, 1)			cliente_tipoPersona,
		''								cliente_actividadEconomicaCIIU,

		convert(varchar(10), tv.docdate, 111) + ' ' + convert(varchar(10), tv.fechaHora, 108) fechaEmision,
		tv.duedate						fechaVencimiento,
		tv.curncyid						moneda,
		upper(left(tv.sopnumbe, convert(int, parametros.param5)-1)) +'-'+ parametros.param2	rangonumeracion,
		0								redondeoaplicado,	--calcular an la app
		tv.xchgrate						tasaDeCambio,
		parametros.param3				tipoDocumento,
		parametros.param4				tipoOperacion,

		--isnull(sumaImpuestos.ortxsls, 0)	totalBaseImponible,
		tv.ORSUBTOT - tv.ORTDISAM		totalBaseImponible,
		tv.ORSUBTOT - tv.ORTDISAM		totalSinImpuestos,
		tv.total + abs(isnull(sumaImpuestosNeg.staxamnt, 0))	totalBrutoconImpuestos,
		tv.total + abs(isnull(sumaImpuestosNeg.staxamnt, 0))	totalMonto,

		0								totalProductos		--calcular en la app

	from dbo.vwCfdiSopTransaccionesVenta tv
		outer apply (select top 1 rtrim(nsaIF_Type_Nit) nsaIF_Type_Nit, nsaIFNit, 
					substring(reverse(rtrim(replace(nsaIFNit, '-', ''))), 2, 20) numeroIdentificacion, left(reverse(rtrim(nsaIFNit)), 1) numeroIdentificacionDV
					from NSAIF02666
					where CUSTNMBR = tv.CUSTNMBR
					) nitTercero
		outer apply dbo.fCfdiCatalogoGetDescripcion('CITY', tv.cityCode) catCiudad
		outer apply dbo.fCfdiCatalogoGetDescripcion('DPTO', tv.stateCode) catDepartamento
		outer apply dbo.fCfdiParametros('V_CANTDECIMALES', 'I_'+tv.docid, 'D_'+tv.docid, 'O_'+tv.docid, 'V_ININUMEROFAC', 'na', 'FECOL') parametros	--Parámetros. Cantidad decimales
		outer apply dbo.fnCfdiSumaImpuestosNegativosSop(tv.sopnumbe, tv.soptype, 0, '%', '%', '%') sumaImpuestosNeg
		outer apply (select top 1 Email_Recipient from dbo.rm00106 where CUSTNMBR = tv.custnmbr and Email_Type = 1) unEmail 
		--outer apply dbo.fnCfdGetDireccionesCorreo(tv.custnmbr) mail
		--outer apply dbo.fCfdiGetLeyendaDeFactura(tv.SOPNUMBE, tv.soptype, '01') lfa

go

IF (@@Error = 0) PRINT 'Creación exitosa de la función: vwCfdiGeneraDocumentoDeVenta ()'
ELSE PRINT 'Error en la creación de la función: vwCfdiGeneraDocumentoDeVenta ()'
GO
-----------------------------------------------------------------------------------------
IF (OBJECT_ID ('dbo.vwCfdiTransaccionesDeVenta', 'V') IS NULL)
   exec('create view dbo.vwCfdiTransaccionesDeVenta as SELECT 1 as t');
go

alter view dbo.vwCfdiTransaccionesDeVenta as
--Propósito. Todos los documentos de venta: facturas y notas de crédito. 
--Usado por. App Factura digital (doodads)
--Requisitos. El estado "no emitido" indica que no se ha emitido el archivo xml pero que está listo para ser generado.
--			El estado "inconsistente" indica que existe un problema en el folio o certificado, por tanto no puede ser generado.
--			El estado "emitido" indica que el archivo xml ha sido generado y sellado por el PAC y está listo para ser impreso.
--01/09/19 jcf Creación cfdi Colombia
--13/09/19 jcf Corrige fechahora
--

select tv.estadoContabilizado, tv.soptype, tv.docid, tv.sopnumbe, 
	cast(cast(tv.docdate as date) as datetime) + cast(cast(tv.fechahora as time) as datetime) fechahora, 
	tv.CUSTNMBR, tv.nombreCliente, tv.idImpuestoCliente, cast(tv.total as numeric(19,2)) total, tv.montoActualOriginal, tv.voidstts, 

	isnull(lf.estado, isnull(fv.estado, 'inconsistente')) estado,
	case when isnull(lf.estado, isnull(fv.estado, 'inconsistente')) = 'inconsistente' 
		then 'folio o certificado inconsistente'
		else ISNULL(lf.mensaje, tv.estadoContabilizado)
	end mensaje,
	case when isnull(lf.estado, isnull(fv.estado, 'inconsistente')) = 'no emitido' 
		then null	--dbo.fCfdiGeneraDocumentoDeVentaXML (tv.soptype, tv.sopnumbe) 
		else cast('' as xml) 
	end comprobanteXml,
	
	fv.ID_Certificado, fv.ruta_certificado, fv.ruta_clave, fv.contrasenia_clave, 
	isnull(pa.ruta_certificado, '_noexiste') ruta_certificadoPac, isnull(pa.ruta_clave, '_noexiste') ruta_clavePac, isnull(pa.contrasenia_clave, '') contrasenia_clavePac, 
	emi.TAXREGTN rfc, 
	isnull(lf.noAprobacion, '7') regimen, 
	emi.INET7 rutaXml, 
	emi.ZIPCODE codigoPostal,
	isnull(lf.estadoActual, '000000010') estadoActual, 
	isnull(lf.mensajeEA, tv.estadoContabilizado) mensajeEA,
	tv.curncyid isocurrc,
	null addenda
from dbo.vwCfdiSopTransaccionesVenta tv
	cross join dbo.fCfdiEmisor() emi
	outer apply dbo.fCfdiCertificadoVigente(tv.fechahora) fv
	outer apply dbo.fCfdiCertificadoPAC(tv.fechahora) pa
	left join cfdlogfacturaxml lf
		on lf.soptype = tv.SOPTYPE
		and lf.sopnumbe = tv.sopnumbe
		and lf.estado = 'emitido'

go

IF (@@Error = 0) PRINT 'Creación exitosa de la vista: vwCfdiTransaccionesDeVenta'
ELSE PRINT 'Error en la creación de la vista: vwCfdiTransaccionesDeVenta'
GO

-- FIN DE SCRIPT ***********************************************

