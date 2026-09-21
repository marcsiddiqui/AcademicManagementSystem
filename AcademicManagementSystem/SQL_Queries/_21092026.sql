create procedure EnrollmentInfo
as
begin
	select e.Id, e.EnrollmentDate, e.Status, e.IsActive, s.FullName StudentName, c.Name CourseName, c.Fee from Enrollment e
	inner join Courses c on e.CourseId = c.Id
	inner join Student s on e.StudentId = s.Id
end


exec EnrollmentInfo