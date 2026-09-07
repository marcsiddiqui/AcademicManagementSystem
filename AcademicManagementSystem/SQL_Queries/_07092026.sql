create table Department
(
	Id int primary key identity(1,1),
	Name nvarchar(100) not null unique,
	IsActive bit not null default 1
)

create table Course
(
	Id int primary key identity(1,1),
	Name nvarchar(100) not null,
	Fee decimal (18,2) not null default 0,
	DepartmentId int foreign key references Department(Id)
)