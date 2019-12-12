--COLOMBIA
--Factura Electr�nica
--Prop�sito. Rol que da accesos a objetos de factura electr�nica
--Requisitos. Ejecutar en la compa��a.
--15/05/19 JCF Creaci�n
--
-----------------------------------------------------------------------------------

IF DATABASE_PRINCIPAL_ID('rol_cfdiColombia') IS NULL
	create role rol_cfdiColombia;

--Objetos que usa factura electr�nica
grant select, insert, update, delete on cfdLogFacturaXML to rol_cfdiColombia, dyngrp;
grant execute on proc_cfdLogFacturaXMLLoadByPrimaryKey to rol_cfdiColombia, dyngrp;
grant execute on proc_cfdLogFacturaXMLLoadAll to rol_cfdiColombia, dyngrp;
grant execute on proc_cfdLogFacturaXMLUpdate to rol_cfdiColombia, dyngrp;
grant execute on proc_cfdLogFacturaXMLInsert to rol_cfdiColombia, dyngrp;
grant execute on proc_cfdLogFacturaXMLDelete to rol_cfdiColombia, dyngrp;

grant select on dbo.vwCfdiTransaccionesDeVenta to rol_cfdiColombia, dyngrp;
--grant select on dbo.vwCfdiDocumentosAImprimir to rol_cfdiColombia, dyngrp;
grant select on dbo.vwCfdIdDocumentos  to rol_cfdiColombia, dyngrp;
grant select on dbo.vwCfdClienteDireccionesCorreo to rol_cfdiColombia, dyngrp;
grant select on dbo.vwCfdCartasReclamacionDeuda to rol_cfdiColombia, dyngrp;
--grant select on dbo.vwCfdiListaResumenDiario to rol_cfdiColombia, dyngrp;
grant select on dbo.fCfdiParametros to rol_cfdiColombia;
grant select on dbo.fCfdiParametrosTipoLeyenda to rol_cfdiColombia;

grant select on dbo.vwCfdiConceptos to rol_cfdiColombia;
grant select on dbo.vwCfdiGeneraDocumentoDeVenta to rol_cfdiColombia;
grant select on dbo.vwCfdiRelacionados to rol_cfdiColombia;
--grant select on dbo.vwCfdiGeneraResumenDiario to rol_cfdiColombia;
