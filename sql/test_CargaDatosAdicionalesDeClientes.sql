--Carga tipo de persona: 1-jurídia o 2-natural
select userdef2, *
--update rm set USERDEF2 = da.Tipo_de_persona
from rm00101 rm
inner join [dbo].[_tmpDatosAdicionalesClientesFE191004] da
	on da.Id_de_CLiente = rm.custnmbr

--Carga el régimen fiscal en la info de internet del cliente
insert into SY01200(
Master_Type,
Master_ID,
ADRSCODE,
INET1,
INET2,
INET3,
INET4,
INET5,
INET6,
INET7,
INET8,
Messenger_Address,
INETINFO,
EmailToAddress,
EmailCcAddress,
EmailBccAddress)
select 'CUS', da.Id_de_cliente, da.Id_de_dirección, '', '', '', '', '', '', '', '', '', 'OBLIGACIONESFIS=R-99-PN'+char(13)+char(10)+'REGIMENFISCAL=05'+char(13)+char(10), '','',''
from [dbo].[_tmpDatosAdicionalesClientesFE191004] da
left join SY01200 rm
	on rm.Master_ID = da.Id_de_cliente
		and rm.Master_Type = 'CUS'
		and rm.ADRSCODE =  da.[Id_de_dirección]
where rm.Master_ID is null
--and da.Id_de_cliente = '000000024      '


--Actualiza código dpto y código municipio
select userdef2, city, [state], zip, da.*
--update rm set [state]= da.[Código_Departamento], city = da.[Código_de_municipio] 
from rm00101 rm
inner join [dbo].[_tmpDatosAdicionalesClientesFE191004] da
	on da.Id_de_CLiente = rm.custnmbr

select * 
--update rm set [state]= da.[Código_Departamento], city = da.[Código_de_municipio] 
from RM00102 rm
inner join [dbo].[_tmpDatosAdicionalesClientesFE191004] da
	on da.Id_de_CLiente = rm.custnmbr
	and rm.ADRSCODE = da.[id_de_dirección]

-------------------------------------------------------------------
--verifica la carga de info adicional
select ia.*
--delete ia
	from SY01200 ia			--coInetAddress
	inner join rm00101 ci	
		on ci.custnmbr = ia.Master_ID
		--and ci.custnmbr = @CUSTNMBR
		and ia.Master_Type = 'CUS'
		and ia.ADRSCODE =  ci.ADRSCODE


--select *
--from rm00101
--where CUSTNMBR = '000024077'		

--select distinct [id_de_dirección]
--from [_tmpDatosAdicionalesClientesFE191004]   
