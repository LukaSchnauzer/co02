--Factura Electr�nica
--Prop�sito. Accesos a objetos de factura electr�nica
--Requisitos. Para usuario de dominio: Crear login y accesos a bds: Dynamics, [GCOL], INTDB2
--			Ejecutar en Dynamics
--15/05/19 jcf Creaci�n

use dynamics;
GO

IF DATABASE_PRINCIPAL_ID('rol_cfdiColombia') IS NULL
	create role rol_cfdiColombia;
	
--Objetos que usa factura electr�nica
grant select on dbo.sy01500 to rol_cfdiColombia, dyngrp;
grant select on dbo.vwCfdCompannias  to rol_cfdiColombia, dyngrp;
grant select on dbo.MC40200  to rol_cfdiColombia, dyngrp;
GO
