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
		/// Returns 400 if Course data is not valid or empty.
		/// Dynamically constructs an ID for the new course from the _course list
		/// </summary>
		[HttpPost]
		public IActionResult CreateCourse([FromBody] Course course)
		{
			if (!ModelState.IsValid)
            {
				Context.Response.Headers["Warning"] = "Course data not valid";
				return new HttpStatusCodeResult(400);
            }
            else
            {
				course.ID = 1 + _courses.Max(x => (int?)x.ID) ?? 0;
                _courses.Add(course);
				
                Context.Response.StatusCode = 201;
                Context.Response.Headers["Location"] = Url.Link("GetCourseByID", new {id = course.ID});
				return new ObjectResult(course);
			}
		}
		
		/// <summary>
		/// Updates the data in the Course with the given ID.
		/// Returns warnings if data not valid or course ID is not found.
		/// </summary>
		[HttpPut("{id:int}")]
		public IActionResult UpdateCourse(int id, [FromBody] Course course)
		{
			int index = _courses.FindIndex(c => c.ID == id);
			if(index < 0)
			{
				Context.Response.Headers["Warning"] = string.Format("No Course with id: {0}", id);
				return new HttpStatusCodeResult(404);
			}
			
			if(!ModelState.IsValid) {
				Context.Response.Headers["Warning"] = "Course data not valid";
				return new HttpStatusCodeResult(400);
			}
			
			// Update the Course data with the new data.
			course.ID = id;
			_courses[index] = course;	
			
			Context.Response.StatusCode = 200;
			Context.Response.Headers["Location"] = Url.Link("GetCourseByID", new {id = course.ID});
			return new ObjectResult(course);
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
			{
				Context.Response.Headers["Warning"] = string.Format("No Course with id: {0}", id);
				return HttpNotFound();
			}
			
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
			{
				Context.Response.Headers["Warning"] = string.Format("No Course with id: {0}", id);
				return HttpNotFound();
			}
			return new ObjectResult(course.Students);
		}
		
		[HttpPost("{id:int}/students")]
		public IActionResult AddStudentToCourse(int id, [FromBody] Student student)
		{
			if (!ModelState.IsValid)
            {
				Context.Response.Headers["Warning"] = "Student data not valid";
				return new HttpStatusCodeResult(400);
            }
            else
            {
				int index = _courses.FindIndex(c => c.ID == id);
				if(index < 0)
				{
					Context.Response.Headers["Warning"] = string.Format("No Course with id: {0}", id);
					return new HttpStatusCodeResult(404);
				}
				
                _courses[index].Students.Add(student);
				
                Context.Response.StatusCode = 201;
                Context.Response.Headers["Location"] = Url.Link("GetAllStudentsInCourseByID", new {id = id});
				return new ObjectResult(_courses[index].Students);
			}
		}
	}
}