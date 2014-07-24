select e1.Student_Id as student
	, e2.Student_Id as calssmate
	, c.name as sharedClass 
from enrollment e1 
	join Enrollment e2 on e1.CourseId = e2.CourseId
		and e1.Student_Id <> e2.Student_Id
	join Course c on e2.CourseId = c.Id
where 1=1 
and exists (
	select main.Student_Id, scd.Student_Id, count(*)
	from enrollment main 
		join Enrollment scd on main.CourseId = scd.CourseId
			and main.Student_Id <> scd.Student_Id
			and scd.Student_Id = e2.Student_Id
			and main.Student_Id = e1.Student_Id
		group by main.Student_Id, scd.Student_Id
		having count(*) > 1
	)
order by e1.Student_Id, e2.Student_Id

