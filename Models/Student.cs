using System.ComponentModel.DataAnnotations;
namespace CoursesWebAPI.Models
{
	public class Student
	{
		/// <summary>
		/// Social security number of the student, Example "1234567890" -- REQUIRED
		/// </summary>
		[Required(ErrorMessage = "You forgot {0}")]
		public string SSN { get; set; }
		/// <summary>
		/// Full name of the student, Example "Jón Freysteinn Jónsson" -- REQUIRED
		/// </summary>
		[Required(ErrorMessage = "You forgot {0}")]
		public string Name { get; set; }
	}
}