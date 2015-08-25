using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoursesWebAPI.Models
{
	/*
		This class represents a single course in out school.
	*/
    public class Course
	{
		/// <summary>
		/// ID of the course, Example: 1
		/// </summary>
		public int ID { get; set; }
		
		// All the following fields are required
		
		/// <summary>
		/// Name of the course, Example "Web Design" -- REQUIRED
		/// </summary>
		[Required]
		public string Name { get; set; }
		/// <summary>
		/// TemplateID of the course, Example "T-514-VEFT" -- REQUIRED
		/// </summary>
		[Required]
		public string TemplateID { get; set; }
		/// <summary>
		/// Starting date of the course, Example "2015-08-17" -- REQUIRED
		/// </summary>
		[Required]
		public DateTime StartDate { get; set; }
		/// <summary>
		/// End date of the course, Example "2015-11-08" -- REQUIRED
		/// </summary>
		[Required]
		public DateTime EndDate { get; set; }
		/// <summary>
		/// List containing all the students in the Course.
		/// </summary>
		public List<Student> Students { get; set; }
	}
}