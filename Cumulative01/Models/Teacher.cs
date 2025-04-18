namespace Cumulative01.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string TeacherFirstName { get; set; }
        public string TeacherLastName { get; set; }

        public string EmployeeID { get; set; }

        public DateTime HireDate { get; set; }
        public double Salary { get; set; }
    }
}
