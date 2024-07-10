USE BaseDatosSN
GO


CREATE TABLE ROL(
	ID_Rol int primary key identity,
	Descripcion varchar(50),
	FechaRegistro datetime default getdate(),
)
GO

CREATE TABLE PERMISO(
	ID_Permiso int primary key identity,
	ID_Rol int references ROL(ID_Rol),
	NombreMenu varchar(100),
	FechaRegistro datetime default getdate(),
)
GO

CREATE TABLE CLIENTE(
	ID_Cliente int primary key identity,
	Tipo varchar(50), -- 
	NombreCompleto varchar(100),
	TipoDocumento char(3),
	NDocumento varchar(15), 
	Direccion varchar(50),
	Telefono char(9),
	Estado bit,
	FechaRegistro datetime default getdate(),
)
GO

CREATE TABLE USUARIO(
	ID_Usuario int primary key identity,
	Nombre varchar(50),
	Contraseña varchar(50),
	ID_Rol int references ROL(ID_Rol),
	Estado bit,
	FechaRegistro datetime default getdate(),
)
GO

CREATE TABLE CATEGORIA(
	ID_Categoria int primary key identity,
	Descripcion varchar(100),
	Estado bit,
	FechaRegistro datetime default getdate(),
)
GO

CREATE TABLE PRODUCTO(
	ID_Producto int primary key identity,
	Codigo char(7),
	ID_Categoria int references CATEGORIA(ID_Categoria),
	Nombre varchar(50), 
	Descripcion varchar(50),
	PrecioCatalogo decimal(10,2) default 0,
	PrecioCompra decimal(10,2),
	DescuentoCompra decimal(10,2), 
	Stock int,
	Estado bit,
	FechaRegistro datetime default getdate(),
)
GO

CREATE TABLE COMPRA(
	ID_Compra int primary key identity,
	ID_Usuario int references USUARIO(ID_Usuario),
	Serie varchar(10),
	NDocumento varchar(50),
	Total decimal(10,2),
	Igv decimal(10,2),
	FechaRegistro datetime default getdate(),
) 
GO

CREATE TABLE DETALLE_COMPRA(
	ID_DetalleCompra int primary key identity,
	ID_Compra int references COMPRA(ID_Compra),
	ID_Producto int references PRODUCTO(ID_Producto),
	PrecioCompra decimal(10,2),
	Cantidad int,
	Subtotal decimal(10,2),
	FechaRegistro datetime default getdate(),
) 
GO

-----------------------------------------------------------------﻿

CREATE TABLE VENTA(
	ID_Venta int primary key identity,
	ID_Usuario int references USUARIO(ID_Usuario),
	DocumentoCliente varchar(15),
	NombreCliente varchar(100),
	Serie varchar(10),
	TipoDocumento varchar(50),
	NDocumento varchar(50),
	Igv decimal(10,2),
	Total decimal(10,2),
	FechaRegistro datetime default getdate(),
)
GO

CREATE TABLE DETALLE_VENTA(
	ID_DetalleVenta int primary key identity,
	ID_Venta int references VENTA(ID_Venta),
	ID_Producto int references PRODUCTO(ID_Producto),
	PrecioVenta decimal(10,2),
	Cantidad int,
	Descuento decimal(10,2),
	Subtotal decimal(10,2),
	FechaRegistro datetime default getdate(),
)
GO

--NEGOCIO
CREATE TABLE NEGOCIO(
	ID_Negocio int primary key,
	Nombre varchar(60),
	RUC varchar(60),
	Direccion varchar(60),
	Logo varbinary(max) NULL
)

go

--DEVOLUCION
CREATE TABLE DEVOLUCION(
	ID_Devolucion int primary key identity,
	ID_Producto int references PRODUCTO(ID_Producto),
	Descripcion varchar(50),
	Cantidad int,
	Estado bit,
	FechaRegistro datetime default getdate()
	)
Go

/*************************** CREACION DE PROCEDIMIENTOS ALMACENADOS ***************************/
/*--------------------------------------------------------------------------------------------*/

go

create PROC SP_REGISTRARUSUARIO(
@Nombre varchar(50),
@Contraseña varchar(50),
@ID_Rol int,
@Estado bit,
@IdUsuarioResultado int output,
@Mensaje varchar(500) output
)
as
begin
	set @IdUsuarioResultado = 0
	set @Mensaje = ''


	if not exists(select * from USUARIO where Nombre = @Nombre)
	begin
		insert into usuario(Nombre,Contraseña,ID_Rol,Estado) values
		(@Nombre,@Contraseña,@ID_Rol,@Estado)

		set @IdUsuarioResultado = SCOPE_IDENTITY()
		
	end
	else
		set @Mensaje = 'No se puede repetir el nombre para más de un usuario'


end

go

create PROC SP_EDITARUSUARIO(
@ID_Usuario int,
@Nombre varchar(50),
@Contraseña varchar(50),
@ID_Rol int,
@Estado bit,
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
	set @Respuesta = 0
	set @Mensaje = ''


	if not exists(select * from USUARIO where Nombre = @Nombre and ID_Usuario != @ID_Usuario)
	begin
		update  usuario set
		Nombre = @Nombre,
		Contraseña = @Contraseña,
		ID_Rol = @ID_Rol,
		Estado = @Estado
		where ID_Usuario = @ID_Usuario

		set @Respuesta = 1
		
	end
	else
		set @Mensaje = 'No se puede repetir el nombre para más de un usuario'

end
go

create PROC SP_ELIMINARUSUARIO(
@ID_Usuario int,
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
	set @Respuesta = 0
	set @Mensaje = ''
	declare @pasoreglas bit = 1

	IF EXISTS (SELECT * FROM COMPRA C 
	INNER JOIN USUARIO U ON U.ID_Usuario = C.ID_Usuario
	WHERE U.ID_USUARIO = @ID_Usuario
	)
	BEGIN
		set @pasoreglas = 0
		set @Respuesta = 0
		set @Mensaje = @Mensaje + 'No se puede eliminar porque el usuario se encuentra relacionado a una COMPRA\n' 
	END

	IF EXISTS (SELECT * FROM VENTA V
	INNER JOIN USUARIO U ON U.ID_Usuario = V.ID_Usuario
	WHERE U.ID_USUARIO = @ID_Usuario
	)
	BEGIN
		set @pasoreglas = 0
		set @Respuesta = 0
		set @Mensaje = @Mensaje + 'No se puede eliminar porque el usuario se encuentra relacionado a una VENTA\n' 
	END

	if(@pasoreglas = 1)
	begin
		delete from USUARIO where ID_Usuario = @ID_Usuario
		set @Respuesta = 1 
	end

end

go

/* ---------- PROCEDIMIENTOS PARA CATEGORIA -----------------*/


create PROC SP_RegistrarCategoria(
@Descripcion varchar(100),
@Estado bit,
@Resultado int output,
@Mensaje varchar(500) output
)as
begin
	SET @Resultado = 0
	IF NOT EXISTS (SELECT * FROM CATEGORIA WHERE Descripcion = @Descripcion)
	begin
		insert into CATEGORIA(Descripcion,Estado) values (@Descripcion,@Estado)
		set @Resultado = SCOPE_IDENTITY()
	end
	ELSE
		set @Mensaje = 'No se puede repetir la descripcion de una categoria'
	
end


go

Create procedure sp_EditarCategoria(
@ID_Categoria int,
@Descripcion varchar(100),
@Estado bit,
@Resultado bit output,
@Mensaje varchar(500) output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM CATEGORIA WHERE Descripcion =@Descripcion and ID_Categoria != @ID_Categoria)
		update CATEGORIA set
		Descripcion = @Descripcion,
		Estado = @Estado
		where ID_Categoria = @ID_Categoria
	ELSE
	begin
		SET @Resultado = 0
		set @Mensaje = 'No se puede repetir la descripcion de una categoria'
	end

end

go

create procedure sp_EliminarCategoria(
@ID_Categoria int,
@Resultado bit output,
@Mensaje varchar(500) output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (
	 select *  from CATEGORIA c
	 inner join PRODUCTO p on p.ID_Categoria = c.ID_Categoria
	 where c.ID_Categoria = @ID_Categoria
	)
	begin
	 delete top(1) from CATEGORIA where ID_Categoria = @ID_Categoria
	end
	ELSE
	begin
		SET @Resultado = 0
		set @Mensaje = 'La categoria se encuentara relacionada a un producto'
	end

end

GO
  

/* ---------- PROCEDIMIENTOS PARA PRODUCTO -----------------*/

create PROC sp_RegistrarProducto(
@Codigo char(7),
@Nombre varchar(50),
@Descripcion varchar(50),
@ID_Categoria int,
@PrecioCatalogo decimal(10,2),
@PrecioCompra decimal(10,2),
@DescuentoCompra decimal(10,2),
@Stock int,
@Estado bit,
@Resultado int output,
@Mensaje varchar(500) output
)as
begin
	SET @Resultado = 0
	IF NOT EXISTS (SELECT * FROM producto WHERE Nombre = @Nombre)
	begin
		insert into producto(Codigo,Nombre,Descripcion,ID_Categoria,PrecioCatalogo,PrecioCompra,DescuentoCompra,Stock,Estado) 
				values (@Codigo,@Nombre,@Descripcion,@ID_Categoria,@PrecioCatalogo,@PrecioCompra,@DescuentoCompra,@Stock,@Estado)
		set @Resultado = SCOPE_IDENTITY()
	end
	ELSE
	 SET @Mensaje = 'Ya existe un producto con el mismo nombre' 
	
end

GO

create procedure sp_ModificarProducto(
@ID_Producto int,
@Codigo char(7),
@Nombre varchar(50),
@Descripcion varchar(50),
@ID_Categoria int,
@PrecioCatalogo decimal(10,2),
@PrecioCompra decimal(10,2),
@DescuentoCompra decimal(10,2),
@Stock int,
@Estado bit,
@Resultado bit output,
@Mensaje varchar(500) output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM producto WHERE Nombre = @Nombre and ID_Producto != @ID_Producto)
		
		update PRODUCTO set
		codigo = @Codigo,
		Nombre = @Nombre,
		Descripcion = @Descripcion,
		ID_Categoria = @ID_Categoria,
		PrecioCatalogo = @PrecioCatalogo,
		PrecioCompra = @PrecioCompra,
		DescuentoCompra = @DescuentoCompra,
		Stock = @Stock,
		Estado = @Estado
		where ID_Producto = @ID_Producto
	ELSE
	begin
		SET @Resultado = 0
		SET @Mensaje = 'Ya existe un producto con el mismo nombre' 
	end
end

go

create PROC SP_EliminarProducto(
@ID_Producto int,
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
	set @Respuesta = 0
	set @Mensaje = ''
	declare @pasoreglas bit = 1

	IF EXISTS (SELECT * FROM DETALLE_COMPRA dc 
	INNER JOIN PRODUCTO p ON p.ID_Producto = dc.ID_Producto
	WHERE p.ID_Producto = @ID_Producto
	)
	BEGIN
		set @pasoreglas = 0
		set @Respuesta = 0
		set @Mensaje = @Mensaje + 'No se puede eliminar porque se encuentra relacionado a una COMPRA\n' 
	END

	IF EXISTS (SELECT * FROM DETALLE_VENTA dv
	INNER JOIN PRODUCTO p ON p.ID_Producto = dv.ID_Producto
	WHERE p.ID_Producto = @ID_Producto
	)
	BEGIN
		set @pasoreglas = 0
		set @Respuesta = 0
		set @Mensaje = @Mensaje + 'No se puede eliminar porque se encuentra relacionado a una VENTA\n' 
	END

	if(@pasoreglas = 1)
	begin
		delete from PRODUCTO where ID_Producto = @ID_Producto
		set @Respuesta = 1 
	end

end
go

/* ---------- PROCEDIMIENTOS PARA CLIENTE -----------------*/
create PROC sp_RegistrarCliente(
@Tipo varchar(50),
@NombreCompleto varchar(100),
@TipoDocumento char(3),
@NDocumento varchar(15),
@Direccion varchar(50),
@Telefono char(9),
@Estado bit,
@Resultado int output,
@Mensaje varchar(500) output
)as
begin
	SET @Resultado = 0
	DECLARE @IDPERSONA INT 
	IF NOT EXISTS (SELECT * FROM CLIENTE WHERE NDocumento = @NDocumento)
	begin
		insert into CLIENTE(Tipo,NombreCompleto,TipoDocumento,NDocumento,Direccion,Telefono,Estado) values (
		@Tipo,@NombreCompleto,@TipoDocumento,@NDocumento,@Direccion,@Telefono,@Estado)

		set @Resultado = SCOPE_IDENTITY()
	end
	else
		set @Mensaje = 'El numero de documento ya existe'
end

go

create PROC sp_ModificarCliente(
@ID_Cliente int,
@Tipo varchar(50),
@NombreCompleto varchar(100),
@TipoDocumento char(3),
@NDocumento varchar(15),
@Direccion varchar(50),
@Telefono char(9),
@Estado bit,
@Resultado bit output,
@Mensaje varchar(500) output
)as
begin
	SET @Resultado = 1
	DECLARE @IDPERSONA INT 
	IF NOT EXISTS (SELECT * FROM CLIENTE WHERE NDocumento = @NDocumento and ID_Cliente != @ID_Cliente)
	begin
		update CLIENTE set
		Tipo = @Tipo,
		NombreCompleto = @NombreCompleto,
		TipoDocumento = @TipoDocumento,
		NDocumento = @NDocumento,
		Direccion = @Direccion,
		Telefono = @Telefono,
		Estado = @Estado
		where ID_Cliente = @ID_Cliente
	end
	else
	begin
		SET @Resultado = 0
		set @Mensaje = 'El numero de documento ya existe'
	end
end

GO

/* PROCESOS PARA REGISTRAR UNA VENTA */
CREATE TYPE [dbo].[EDetalle_Venta] AS TABLE(
	[ID_Producto] int NULL,
	[PrecioVenta] decimal(10,2) NULL,
	[Cantidad] int NULL,
	[Descuento] decimal(10,2) NULL,
	[Subtotal] decimal(10,2) NULL
	
)


GO

create procedure usp_RegistrarVenta(
@ID_Usuario int,
@TipoDocumento varchar(50),
@Serie varchar(10),
@NDocumento varchar(50),
@DocumentoCliente varchar(15),
@NombreCliente varchar(100),
@Igv decimal(10,2),
@Total decimal(10,2),
@DetalleVenta [EDetalle_Venta] READONLY,                                      
@Resultado bit output,
@Mensaje varchar(500) output
)
as
begin
	
	begin try

		declare @ID_Venta int = 0
		set @Resultado = 1
		set @Mensaje = ''

		begin  transaction registro

		insert into VENTA(ID_Usuario,TipoDocumento,Serie,NDocumento,DocumentoCliente,NombreCliente,Igv,Total)
		values(@ID_Usuario,@TipoDocumento,@Serie,@NDocumento,@DocumentoCliente,@NombreCliente,@Igv,@Total)

		set @ID_Venta = SCOPE_IDENTITY()

		insert into DETALLE_VENTA(ID_Venta,ID_Producto,PrecioVenta,Cantidad,Descuento,Subtotal)
		select @ID_Venta,ID_Producto,PrecioVenta,Cantidad,Descuento,Subtotal from @DetalleVenta

		commit transaction registro

	end try
	begin catch
		set @Resultado = 0
		set @Mensaje = ERROR_MESSAGE()
		rollback transaction registro
	end catch

end

go

--REPORTES
 CREATE PROC sp_ReporteVentas(
 @fechainicio varchar(10),
 @fechafin varchar(10)
 )
 as
 begin
 SET DATEFORMAT dmy;  
 select 
 convert(char(10),v.FechaRegistro,103)[FechaRegistro],v.TipoDocumento,v.Serie,v.NDocumento,v.Total,
 u.Nombre[UsuarioRegistro],
 v.DocumentoCliente,v.NombreCliente,
 p.Codigo[CodigoProducto],p.Nombre[NombreProducto],ca.Descripcion[Categoria],dv.PrecioVenta,dv.Cantidad,dv.Descuento,dv.Subtotal
 from VENTA v
 inner join USUARIO u on u.ID_Usuario = v.ID_Usuario
 inner join DETALLE_VENTA dv on dv.ID_Venta = v.ID_Venta
 inner join PRODUCTO p on p.ID_Producto = dv.ID_Producto
 inner join CATEGORIA ca on ca.ID_Categoria = p.ID_Categoria
 where CONVERT(date,v.FechaRegistro) between @fechainicio and @fechafin
end
go

--PROCEDIMIENTO COMPRAS
CREATE TYPE [dbo].[EDetalle_Compra] as table(
[ID_Producto] int null,
[PrecioCompra] decimal(10,2) null,
[Cantidad] int null,
[Subtotal] decimal(10,2) null
)
Go

Create procedure sp_RegistrarCompra(
@ID_Usuario int,
@Serie varchar,
@NDocumento varchar(50),
@Total decimal(10,2),
@Igv decimal(10,2),
@DetalleCompra [EDetalle_Compra] READONLY,
@Resultado bit output,
@Mensaje varchar(500) output
)
as
begin

	begin try
		declare @idcompra int = 0
		set @Resultado = 1
		set @Mensaje = ''

		begin transaction registro

		insert into  COMPRA(ID_Usuario,Serie,NDocumento,Total,Igv)
		values(@ID_Usuario,@Serie,@NDocumento,@Total,@Igv)

		set @idcompra = SCOPE_IDENTITY()

		insert into DETALLE_COMPRA(ID_Compra,ID_Producto,PrecioCompra,Cantidad,Subtotal)
		select @idcompra, ID_Producto, PrecioCompra,Cantidad,Subtotal from @DetalleCompra

		update p set p.Stock = p.Stock + dc.Cantidad
		from PRODUCTO p
		inner join @DetalleCompra dc on dc.ID_Producto = p.ID_Producto

		commit transaction registro

	end try
	begin catch
		rollback transaction registro
	end catch

end
Go

Create proc sp_ReporteCompras(
@fechainicio varchar(10),
@fechafin varchar(10)
)
as
begin

SET DATEFORMAT dmy;
Select
convert(char(10),c.FechaRegistro,103)[Fecha],c.NDocumento,c.Serie,c.Total,
u.Nombre[Usuario],
p.Codigo[CodigoProducto],p.Nombre[Producto],ca.Descripcion[Categoria],dc.PrecioCompra,dc.Cantidad,dc.Subtotal
from COMPRA c
inner join USUARIO u on u.ID_Usuario = c.ID_Usuario
inner join DETALLE_COMPRA dc on dc.ID_Compra = c.ID_Compra
inner join PRODUCTO p on p.ID_Producto = dc.ID_Producto
inner join CATEGORIA ca on ca.ID_Categoria = p.ID_Categoria
where CONVERT(date,c.FechaRegistro) between @fechainicio and @fechafin
end
go

--PROCEDIMIENTO DE GANANCIAS
create proc sp_Ganancias(
@fechainicio varchar(10),
@fechafin varchar(10)
)
as
begin
SET DATEFORMAT dmy;
select 
  PRODUCTO.Nombre,
  PRODUCTO.PrecioCompra,
  PRODUCTO.PrecioCatalogo,
  (SELECT SUM(Descuento) FROM DETALLE_VENTA WHERE DETALLE_VENTA.ID_Producto = PRODUCTO.ID_Producto) AS Descuento,
  (SELECT SUM(cantidad) FROM DETALLE_COMPRA WHERE DETALLE_COMPRA.ID_Producto = PRODUCTO.ID_Producto) AS CantidadComprada, 
  (SELECT SUM(cantidad) FROM DETALLE_VENTA WHERE DETALLE_VENTA.ID_Producto = PRODUCTO.ID_Producto) AS CantidadVendida,
  (SELECT SUM(PrecioCompra * Cantidad) FROM DETALLE_COMPRA WHERE DETALLE_COMPRA.ID_Producto = PRODUCTO.ID_Producto) AS Inversion,
  (SELECT SUM((PrecioVenta * cantidad) - Descuento) FROM DETALLE_VENTA WHERE DETALLE_VENTA.ID_Producto = PRODUCTO.ID_Producto) AS GBruta,
  (PrecioCatalogo* Stock) as GEsperada,
  (SELECT SUM((PrecioVenta * cantidad) - Descuento) FROM DETALLE_VENTA WHERE DETALLE_VENTA.ID_Producto = PRODUCTO.ID_Producto) - (SELECT SUM(PrecioCompra * Cantidad) FROM DETALLE_COMPRA WHERE DETALLE_COMPRA.ID_Producto = PRODUCTO.ID_Producto) AS GReal
FROM PRODUCTO
inner join DETALLE_VENTA d on d.ID_Producto = PRODUCTO.ID_Producto
where convert(date, d.FechaRegistro) between @fechainicio and @fechafin
GROUP BY PRODUCTO.Nombre, PRODUCTO.PrecioCompra,PRODUCTO.PrecioCatalogo,Stock,PRODUCTO.ID_Producto
end
go

--PROCEDIMIENTOS PARA DEVOLUCION
create PROC SP_REGISTRARDEVOLUCION(
@Descripcion varchar(50),
@Cantidad int,
@ID_Producto int,
@Estado bit,
@Resultado int output,
@Mensaje varchar(500) output
)
as
begin

	Declare @ward int
	SET @Resultado = 0
	SET @Mensaje = ''
	SET @ward = (Select stock from PRODUCTO where ID_Producto = @ID_Producto) 

	if(@ward > @Cantidad)
	begin
		insert into DEVOLUCION(Descripcion,ID_Producto,Cantidad,Estado)values
		(@Descripcion,@ID_Producto,@Cantidad,@Estado)

		set @Resultado = SCOPE_IDENTITY()
	end
	else
		set @Mensaje = 'La cantidad no puede ser mayor al stock del producto'
end
go

create PROC SP_EDITARDEVOLUCION(
@ID_Devolucion int,
@Descripcion varchar(50),
@Cantidad int,
@ID_Producto int,
@Estado bit,
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
	set @Respuesta = 0
	set @Mensaje = ''


	if exists(select * from DEVOLUCION where ID_Devolucion = @ID_Devolucion)
	begin
		update  DEVOLUCION set
		Descripcion = @Descripcion,
		Cantidad = @Cantidad,
		ID_Producto = @ID_Producto,
		Estado = @Estado
		where ID_Devolucion = @ID_Devolucion

		set @Respuesta = 1
		
	end
	else
		set @Mensaje = 'ERROR¿?'

end
go

create PROC SP_ELIMINARDEVOLUCION(
@ID_Devolucion int,
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
	set @Mensaje = ''

		delete from DEVOLUCION where ID_Devolucion = @ID_Devolucion
		set @Respuesta = 1 

end

go


create proc sp_LIMPIARDEVOLUCION(
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
	set @Respuesta = 0
	set @Mensaje = ''

	if EXISTS (SELECT Estado from DEVOLUCION where Estado = 1)
	begin	
		delete from DEVOLUCION where Estado = 1
	set @Respuesta = 1
	end 

	else 
		set	@Mensaje = 'No hay registros para limpiar'
end
go

-- DASHBOARD
create proc spc_ItemsNumeros
@nclient int output,
@nprod int output,
@ncateg int output
as
Set @nclient=(select count (ID_Cliente)  as Clientes from CLIENTE)
Set @nprod=(select count (ID_Producto) as Productos  from PRODUCTO)
set @ncateg  = (select count (ID_Categoria) AS categorias from CATEGORIA)
go

create proc spc_NumVentasPorFecha
@fromDate varchar(10),
@toDate varchar(10),
@nventas int output
as
begin
SET DATEFORMAT dmy;
set @nventas =(select count(ID_Venta) from Venta where CONVERT(date, FechaRegistro) between @fromDate and @toDate)
end
go

create proc spc_TopProductosPorFecha
@fromDate varchar(10),
@toDate varchar(10)
as
begin
SET DATEFORMAT dmy;  
select top 5 P.Nombre as Producto, sum(D.Cantidad) as Cantidad
from DETALLE_VENTA AS D
inner join Producto P on P.ID_Producto = D.ID_Producto
inner join VENTA V on V.ID_Venta = D.ID_Venta
where CONVERT(date,V.FechaRegistro) between @fromDate and @toDate
group by P.Nombre
order by Cantidad desc
end
go

create proc ProdBajoStock
as
select Nombre, Stock 
from PRODUCTO
where Stock <= 6
order by Nombre
go

create proc spc_TotalVentasPorFecha
@fromDate varchar(10),
@toDate varchar(10)
as
begin
SET DATEFORMAT dmy;
select FechaRegistro, sum(Total) as Total
from Venta
where CONVERT(date, FechaRegistro) between @fromDate and @toDate
group by FechaRegistro
end
go

create proc spc_TotalComprasPorFecha
@fromDate varchar(10),
@toDate varchar(10),
@totcompras int output
as
begin
SET DATEFORMAT dmy;
set @totcompras =(select sum(Total) from COMPRA where CONVERT(date, FechaRegistro) between @fromDate and @toDate)
end
go

--------------------------------------------------------

 insert into rol (Descripcion)
 values('ADMINISTRADOR')
 GO

 insert into rol (Descripcion)
 values('EMPLEADO')
 GO

 insert into USUARIO(Nombre,Contraseña,ID_Rol,Estado)
 values 
 ('ADMIN','123',1,1)
 GO


 insert into USUARIO(Nombre,Contraseña,ID_Rol,Estado)
 values 
 ('EMPLEADO','456',2,1)
 GO

   insert into PERMISO(ID_Rol,NombreMenu) values
  (1,'menuusuarios'),
  (1,'menumantenedor'),
  (1,'menuventas'),
  (1,'menucompras'),
  (1,'menuclientes'),
  (1,'menuproveedores'),
  (1,'menureportes'),
  (1,'menuacercade')
  GO

  insert into PERMISO(ID_Rol, NombreMenu) values
  (1,'menudashboard')
  go
  
  insert into PERMISO(ID_Rol, NombreMenu) values
  (1,'menuganancias')
  go

  insert into PERMISO(ID_Rol,NombreMenu) values
  (2,'menuventas'),
  (2,'menucompras'),
  (2,'menuclientes'),
  (2,'menuproveedores'),
  (2,'menuacercade')

  GO

  insert into PERMISO(ID_Rol, NombreMenu) values
  (2,'menudashboard')
  go

  insert into NEGOCIO(ID_Negocio,Nombre,RUC,Direccion,Logo) values
  (1,'Dtr. Santa Natura','20602114121','V. JORGE BASADRE GROHMANN NRO. 990 URB. ORRANTIA',null)
  go

  insert into CATEGORIA(Descripcion, Estado) valueS ('ejemplocategoria', 1)
  go
  
  insert into CLIENTE(Tipo, NombreCompleto, TipoDocumento, NDocumento, Direccion, Telefono, Estado) 
  values ('Publico', 'cliente general', 'DNI', '88888888', '', '', 1)
  
  insert into PRODUCTO(Codigo,ID_Categoria,Nombre,Descripcion,PrecioCatalogo,PrecioCompra,DescuentoCompra,Stock,Estado)
  values ('',1,'Concentrado de Alcachofa','Concentrado de Alcachofa',68,34,50,5,1)