create table Student
(
	Id int primary key identity(1,1),
	FullName nvarchar(200) not null,
	Email nvarchar(200) not null,
	Phone nvarchar(200) not null,
	Gender int not null default 0,
	DateOfBirth datetime not null,
	[Address] nvarchar(max) null,
	AdmissionDate datetime not null,
	IsActive bit not null default 1
)