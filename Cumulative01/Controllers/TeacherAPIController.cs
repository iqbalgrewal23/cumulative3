using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative01.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Cumulative01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }

       /// <summary>
/// Returns a list of Teachers in the system. If a search key is included, search for teachers with a first or last name matching.
/// </summary>
/// <example>
/// GET api/TeacherAPI/Teacher?SearchKey=John -> [{"TeacherId":1,"TeacherFirstName":"John", "TeacherLastName":"Smith"}, ...]
/// </example>
/// <returns>A list of teacher objects</returns>
[HttpGet("Teacher")]
public List<Teacher> ListTeacherNames(string SearchKey = null)
{
    List<Teacher> Teachers = new List<Teacher>();

    using (MySqlConnection Connection = _context.GetConnection())
    {
        Connection.Open();
        MySqlCommand Command = Connection.CreateCommand();

        string query = "SELECT * FROM teachers";

        if (!string.IsNullOrEmpty(SearchKey))
        {
            query += " WHERE LOWER(teacherfname) LIKE @key OR LOWER(teacherlname) LIKE @key OR LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE @key";
            Command.Parameters.AddWithValue("@key", "%" + SearchKey.ToLower() + "%");
        }

        Command.CommandText = query;
        Command.Prepare();

        using (MySqlDataReader ResultSet = Command.ExecuteReader())
        {
            while (ResultSet.Read())
            {
                Teacher NewTeacher = new Teacher()
                {
                    TeacherId = Convert.ToInt32(ResultSet["teacherid"]),
                    TeacherFirstName = ResultSet["teacherfname"].ToString(),
                    TeacherLastName = ResultSet["teacherlname"].ToString(),
                    EmployeeID = ResultSet["employeenumber"].ToString(),
                    HireDate = Convert.ToDateTime(ResultSet["hiredate"]),
                    Salary = Convert.ToDouble(ResultSet["salary"])
                };

                Teachers.Add(NewTeacher);
            }
        }
    }

    return Teachers;
}

        [HttpGet]
        [Route(template: "FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            Teacher teacher = new Teacher();

            MySqlConnection Connection = _context.GetConnection();
            Connection.Open();

            string SQL = "Select * FROM teachers Where Teacherid = "+id.ToString();

            MySqlCommand Command = Connection.CreateCommand();

            Command.CommandText = SQL;

            MySqlDataReader DataReader = Command.ExecuteReader();


            while (DataReader.Read())
            {
                int TeacherId = Convert.ToInt32(DataReader["teacherid"]);
                string TeacherFName = DataReader["teacherfname"].ToString();
                string TeacherLName = DataReader["teacherlname"].ToString();
                string EmployeeID = DataReader["employeenumber"].ToString();
                DateTime HireDate = Convert.ToDateTime(DataReader["hiredate"]);
                double Salary = Convert.ToDouble(DataReader["salary"]);

                teacher.TeacherId = TeacherId;
                teacher.TeacherFirstName = TeacherFName;
                teacher.TeacherLastName = TeacherLName;
                teacher.EmployeeID = EmployeeID;
                teacher.HireDate = HireDate;
                teacher.Salary = Salary;
            }

            Connection.Close(); 


            return teacher;
        }
        /// <summary>
/// Adds a teacher to the database
/// </summary>
/// <param name="TeacherData">Teacher Object</param>
/// <example>
/// POST: api/TeacherAPI/AddTeacher
/// Headers: Content-Type: application/json
/// Request Body:
/// {
///     "TeacherFirstName":"John",
///     "TeacherLastName":"Doe",
///     "EmployeeID":"T12345",
///     "HireDate":"2023-09-01",
///     "Salary":75000
/// }
/// </example>
/// <returns>
/// The inserted Teacher Id from the database if successful. 0 if Unsuccessful
/// </returns>
[HttpPost("AddTeacher")]
public int AddTeacher([FromBody] Teacher TeacherData)
{
    using (MySqlConnection Connection = _context.GetConnection())
    {
        Connection.Open();
        MySqlCommand Command = Connection.CreateCommand();

        Command.CommandText = @"INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) 
                                VALUES (@teacherfname, @teacherlname, @employeenumber, @hiredate, @salary)";
        Command.Parameters.AddWithValue("@teacherfname", TeacherData.TeacherFirstName);
        Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLastName);
        Command.Parameters.AddWithValue("@employeenumber", TeacherData.EmployeeID);
        Command.Parameters.AddWithValue("@hiredate", TeacherData.HireDate);
        Command.Parameters.AddWithValue("@salary", TeacherData.Salary);

        Command.ExecuteNonQuery();

        return Convert.ToInt32(Command.LastInsertedId);
    }

    // Failure
    return 0;
}

/// <summary>
/// Deletes a Teacher from the database
/// </summary>
/// <param name="id">Primary key of the teacher to delete</param>
/// <example>
/// DELETE: api/TeacherAPI/DeleteTeacher/3
/// </example>
/// <returns>
/// Number of rows affected by delete operation.
/// </returns>
[HttpDelete("DeleteTeacher/{id}")]
public int DeleteTeacher(int id)
{
    using (MySqlConnection Connection = _context.GetConnection())
    {
        Connection.Open();
        MySqlCommand Command = Connection.CreateCommand();

        Command.CommandText = "DELETE FROM teachers WHERE teacherid=@id";
        Command.Parameters.AddWithValue("@id", id);

        return Command.ExecuteNonQuery();
    }

    // Failure
    return 0;
}
/// <summary>
/// Updates an existing Teacher in the database
/// </summary>
/// <param name="TeacherId">Primary key of the teacher to update</param>
/// <param name="TeacherData">Teacher object with updated information</param>
/// <example>
/// PUT: api/TeacherAPI/UpdateTeacher/3
/// BODY: { "TeacherFirstName": "John", "TeacherLastName": "Doe", "EmployeeID": "T123", "HireDate": "2021-09-01", "Salary": 60000 }
/// </example>
/// <returns>
/// Nothing (void). Updates the teacher record in the database.
/// </returns>

[HttpPut("UpdateTeacher/{TeacherId}")]
public void UpdateTeacher(int TeacherId, [FromBody] Teacher TeacherData)
{
    using (MySqlConnection Connection = _context.GetConnection())
    {
        Connection.Open();
        MySqlCommand Command = Connection.CreateCommand();

        Command.CommandText = @"UPDATE teachers 
                                SET teacherfname = @teacherfname, 
                                    teacherlname = @teacherlname, 
                                    employeenumber = @employeenumber, 
                                    hiredate = @hiredate, 
                                    salary = @salary 
                                WHERE teacherid = @id";

        Command.Parameters.AddWithValue("@teacherfname", TeacherData.TeacherFirstName);
        Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLastName);
        Command.Parameters.AddWithValue("@employeenumber", TeacherData.EmployeeID);
        Command.Parameters.AddWithValue("@hiredate", TeacherData.HireDate);
        Command.Parameters.AddWithValue("@salary", TeacherData.Salary);
        Command.Parameters.AddWithValue("@id", TeacherId);

        Command.ExecuteNonQuery();
    }
}



        

    }
    
}
