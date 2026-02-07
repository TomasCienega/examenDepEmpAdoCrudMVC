create database examenDepEmpAdoCrudMVC
use examenDepEmpAdoCrudMVC

create table Departamento
(
	idDepartamento int identity(1,1) not null,
	nombreDepartamento varchar(50) not null,
	constraint PK_Departamento primary key (idDepartamento)
);

create table Empleado
(
	idEmpleado int identity(1,1) not null,
	nombreEmpleado varchar(100) not null,
	idDepartamento int,
	activo bit default 1,
	constraint PK_Empleado primary key (idEmpleado),
	constraint FK_Empleado_Departamento foreign key (idDepartamento)
										references Departamento(idDepartamento)
);

ALTER TABLE Empleado
ADD CONSTRAINT DF_Empleado_Activo 
DEFAULT 1 FOR activo;

insert into Departamento(nombreDepartamento)values('TI'),('RH'),('Ventas');
insert into Empleado(nombreEmpleado,idDepartamento) values ('Tomas',1)

--=================== CREAR PROCEDIMIENTOS ALMACENADOS PARA DEPARTAMENTOS =========================

create procedure sp_ListarDepartamentos
as
begin
	select idDepartamento,nombreDepartamento from Departamento;
end

--=================== CREAR PROCEDIMIENTOS ALMACENADOS PARA EMPLEADOS =============================

create procedure sp_ListarEmpleados
as
begin
	select e.idEmpleado,e.nombreEmpleado, d.idDepartamento, d.nombreDepartamento, e.activo 
	from Empleado e inner join 
	Departamento d on e.idDepartamento = d.idDepartamento
	order by e.activo DESC;
end

create procedure sp_ListarEmpleadoPorIdEmpleado
(
	@idEmpleado int
)
as
begin
	select e.idEmpleado,e.nombreEmpleado, d.idDepartamento, d.nombreDepartamento, e.activo 
	from Empleado e inner join
	Departamento d on e.idDepartamento = d.idDepartamento
	where e.idEmpleado = @idEmpleado
end

create procedure sp_ListarEmpleadoPorIdDepartamento
(
	@idDepartamento int
)
as
begin
	select e.idEmpleado,e.nombreEmpleado, d.idDepartamento,d.nombreDepartamento, e.activo 
	from Empleado e 
	inner join Departamento d on e.idDepartamento = d.idDepartamento
	where d.idDepartamento =@idDepartamento
end

create procedure sp_ListarEmpleadoPornomEmpl
(
	@nomEmpl varchar(100)
)
as
begin
	select e.idEmpleado,e.nombreEmpleado, d.idDepartamento,d.nombreDepartamento, e.activo 
	from Empleado e 
	inner join Departamento d on e.idDepartamento = d.idDepartamento
	where e.nombreEmpleado like '%' + @nomEmpl + '%'
end

create procedure sp_GuardarEmpleado
(
	@nombreEmpleado varchar(100),
	@idDepartamento int
)
as
begin
	insert into Empleado(nombreEmpleado,idDepartamento)
	values(@nombreEmpleado,@idDepartamento)
end

create procedure sp_EditarEmpleado
(
	@idEmpleado int,
	@nombreEmpleado varchar(100),
	@idDepartamento int,
	@activo bit
)
as
begin 
	update Empleado set
	nombreEmpleado = @nombreEmpleado,
	idDepartamento = @idDepartamento,
	activo = @activo
	where idEmpleado = @idEmpleado 
end

create procedure sp_EliminarEmpleadoFisico
(
	@idEmpleado int
)
as
begin
	delete from Empleado
	where idEmpleado = @idEmpleado
end

-- este sp solo pone inactivo al empleado pero nunca se podra activar a menos de hacer algo en el codigo
--create procedure sp_EliminarEmpleadoLogico
--(
--	@idEmpleado int
--)
--as
--begin
--	update Empleado set 
--	activo = 0
--	where idEmpleado = @idEmpleado
--end

-- DROP PROCEDURE sp_EliminarEmpleadoLogico;

create procedure sp_CambiarEstadoEmpl
(
	@idEmpleado int
)
as
begin
	update Empleado
	set activo = case WHEN activo = 1 THEN 0 ELSE 1 END
	where idEmpleado = @idEmpleado
end

--select * from [dbo].[Departamento]
select * from [dbo].[Empleado]