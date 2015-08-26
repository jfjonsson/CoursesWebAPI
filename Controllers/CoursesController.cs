using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using CoursesWebAPI.Models;
using Microsoft.AspNet.Cors.Core;

namespace CoursesWebAPI.Controllers
{
	[EnableCors("mypolicy")]
	[Route("api/[controller]")]
	public class CoursesController : Controller
    {
		/// <summary>
		/// List of all courses currently stored in memory.
		/// </summary>
		private static List<Course> _courses;
		
		public CoursesController()
		{
			if(_courses == null)
			{
				// Simple test data.
				_courses = new List<Course>()
				{
					new Course() 
					{
						ID = 1,
						Name = "Web Services",
						TemplateID = "T-514-VEFT",
						StartDate = new DateTime(2015, 8, 17),
						EndDate = new DateTime(2016, 1, 1),
						Students = new List<Student>()
						{
							new Student () 
							{
								SSN = "1234567890",
								Name = "Jón Freysteinn Jónsson"
							}
						}
					},
					new Course() 
					{
						ID = 2,
						Name = "Compilers",
						TemplateID = "T-603-THYD",
						StartDate = new DateTime(2015, 8, 17),
						EndDate = new DateTime(2016, 1, 1),
						Students = new List<Student>()
						{
							new Student () 
							{
								SSN = "0987654321",
								Name = "Sverrir Páll Sverrisson"
							}
						}
					}
				};
			}
		}
		
		/// <summary>
		/// Returns all the courses contained in the _courses list.
		/// </summary>
		[HttpGet(Name = "GetAllCourses")]
		public IActionResult GetCourses()
		{
			return new ObjectResult(_courses);
		}
		
		/// <summary>
		/// Returns a single course with given ID if the course exists.
		/// </summary>
		[HttpGet("{id:int}", Name = "GetCourseByID")]
		public IActionResult GetCourse(int id)
		{
			var course = _courses.FirstOrDefault(c => c.ID == id);
			if (course == null)
			{
				return HttpNotFound();
			}
			return new ObjectResult(course);
		}
		
		/// <summary>
		/// Create a new Course in the _course list with provided information.
		/// Returns 412 if Course data is not valid or empty.
		/// Dynamically constructs an ID for the new course from the _course list
		/// </summary>
		[HttpPost]
		public IActionResult CreateCourse([FromBody] Course course)
		{
			if (course == null || !ModelState.IsValid)
            {
				return HttpBadRequest(new { message = "Course data not valid" });
            }
            else
            {
				// Get the highest ID from the course list, is list is empty default to 0.
				course.ID = 1 + _courses.Max(x => (int?)x.ID) ?? 0;
                _courses.Add(course);
				
				return new CreatedAtRouteResult("GetCourseByID", new {id = course.ID} ,course);
			}
		}
		
		/// <summary>
		/// Updates the data in the Course with the given ID.
		/// Returns error message if data not valid or course ID is not found.
		/// </summary>
		[HttpPut("{id:int}")]
		public IActionResult UpdateCourse(int id, [FromBody] Course course)
		{
			int index = _courses.FindIndex(c => c.ID == id);
			
			if(index < 0)
				return HttpNotFound(new { message = string.Format("No Course with id: {0}", id)});
			
			if(course == null || !ModelState.IsValid)
				return HttpBadRequest(new { message = "Course data not valid"});
			
			// Update the Course data with the new data.
			course.ID = id;
			if(course.Students == null)
				course.Students = _courses[index].Students;
			_courses[index] = course;	
			
			Context.Response.Headers["Location"] = Url.Link("GetCourseByID", new {id = course.ID});

			return new ObjectResult(course) { StatusCode = 200 };
		}
		
		/// <summary>
		/// Delete the Course with the given ID.
		/// Returns 404 if Course not found.
		/// </summary>
		[HttpDelete("{id:int}")]
		public IActionResult RemoveCourse(int id)
		{
			var course = _courses.FirstOrDefault(c => c.ID == id);
			if(course == null) 
				return HttpNotFound(new { message = string.Format("No Course with id: {0}", id) });

			_courses.Remove(course);
			return new HttpStatusCodeResult(204);
		}
		
		/// <summary>
		/// Returns a list of students in the Course with the given ID.
		/// Returns 404 if Course not found.
		/// </summary>
		[HttpGet("{id:int}/students", Name = "GetAllStudentsInCourseByID")]
		public IActionResult GetStudents(int id)
		{
			var course = _courses.FirstOrDefault(c => c.ID == id);
			if (course == null)
				return HttpNotFound(new { message = string.Format("No Course with id: {0}", id) });
				
			return new ObjectResult(course.Students);
		}
		
		/// <summary>
		/// Add a student to the Course with the given ID.
		/// Returns 404 if Course is not found.
		/// Returns 412 if Student data is not valid or empty.
		/// </summary>
		[HttpPost("{id:int}/students")]
		public IActionResult AddStudentToCourse(int id, [FromBody] Student student)
		{
			if (student == null || !ModelState.IsValid)
            {
				return new ObjectResult(new { message  =  "Student data not valid" }){StatusCode = 412};
            }
            else
            {
				int index = _courses.FindIndex(c => c.ID == id);
				if(index < 0)
					return HttpNotFound(new { message = string.Format("No Course with id: {0}", id) });
				
                _courses[index].Students.Add(student);
				
				return new CreatedAtRouteResult("GetAllStudentsInCourseByID", new {id = id}, _courses[index].Students);
			}
		}
	}
}