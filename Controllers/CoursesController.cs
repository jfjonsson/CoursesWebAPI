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
						EndDate = new DateTime(2016, 1, 1)
					},
					new Course() 
					{
						ID = 2,
						Name = "Compilers",
						TemplateID = "T-603-THYD",
						StartDate = new DateTime(2015, 8, 17),
						EndDate = new DateTime(2016, 1, 1)
					}
				};
			}
		}
		
		/// <summary>
		/// Returns all the courses contained in the _courses list.
		/// </summary>
		[HttpGet(Name = "GetAllCourses")]
		public IEnumerable<Course> GetCourses()
		{
			return _courses;
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
				Context.Response.Headers["Warning"] = "Course not valid";
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
		/// 
		/// </summary>
		[HttpPut("{id:int}")]
		public void UpdateCourse(int id, [FromBody] Course course)
		{
			int index = _courses.FindIndex(c => c.ID == id);
			if(index < 0) 
				Context.Response.StatusCode = 400;
				
			_courses[index].Name = course.Name;
			_courses[index].TemplateID = course.TemplateID;
			_courses[index].StartDate = course.StartDate;
			_courses[index].EndDate = course.EndDate;
			
			string url = Url.RouteUrl("GetCourseByID", new { id = course.ID }, 
                    Request.Scheme, Request.Host.ToUriComponent());
				
			Context.Response.StatusCode = 303;
			Context.Response.Headers["Location"] = url;
			
		}
		
		/// <summary>
		/// 
		/// </summary>
		[HttpDelete("{id:int}")]
		public IActionResult RemoveCourse(int id)
		{
			var course = _courses.FirstOrDefault(c => c.ID == id);
			if(course == null) 
				return HttpNotFound();
			_courses.Remove(course);
			return new HttpStatusCodeResult(204);
		}
	}
}