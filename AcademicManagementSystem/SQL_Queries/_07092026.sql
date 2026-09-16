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

create table Enrollment
(
	Id int primary key identity(1,1),
	StudentId int not null foreign key references Student(Id),
	CourseId int not null foreign key references Courses(Id),
	EnrollmentDate datetime not null,
	[Status] nvarchar(10) not null default 'InProgress',
	IsActive bit not null default 1
)