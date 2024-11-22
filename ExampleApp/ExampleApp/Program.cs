using MySqlConnector;
using System;

var connectionString = "Server=localhost;Port=3306;Database=course;Uid=root;Pwd=123;";

while (true) {
    Console.WriteLine("Write down your command:");
    var command = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);
    if (command.Length == 0)
        break; //giving empty command exits an application

    var connection = new MySqlConnection(connectionString);
    connection.Open();

    switch (command[0]) {
        case "mark_attendance":
            MarkAttendance(connection, command);
            break;
        case "set_mark":
            SetMark(connection, command);
            break;
        case "kill_guardian":
            KillGuardian(connection, command);
            break;
        case "week_plan":
            WeekPlan(connection, command);
            break;
        case "busiest_teachers":
            BusiestTeachers(connection, command);
            break;
        case "foreign_heads":
            ForeignHeads(connection, command);
            break;
        case "update_number_of_rooms":
            UpdateNumberOfRooms(connection, command);
            break;
        case "too_crowded":
            TooCrowded(connection, command);
            break;
        case "unstable_students":
            UnstableStudents(connection, command);
            break;
        case "leadership":
            Leadership(connection, command);
            break;
        default:
            IncorrectCommand(connection, command);
            break;
    }

    Console.WriteLine(String.Empty);
    connection.Close();
}

void AddDoctor(MySqlConnection connection, string[] command)
{
    if (command.Length != 2) {
        Console.WriteLine("Incorrect syntax!");
        return;
    }

    try {
        var cmd = new MySqlCommand("INSERT INTO doctors (name) VALUES (@name)", connection);
        cmd.Parameters.AddWithValue("@name", command[1]);
        cmd.Prepare();
        cmd.ExecuteNonQuery();
        Console.WriteLine("Doctor created!");
    }
    catch (Exception ex) {
        Console.WriteLine(ex.ToString());
    }
}

void ListDoctors(MySqlConnection connection, string[] command)
{
    if (command.Length != 1) {
        Console.WriteLine("Incorrect syntax!");
        return;
    }

    try {
        var cmd = new MySqlCommand("SELECT * FROM doctors", connection);
        var reader = cmd.ExecuteReader();

        while (reader.Read())
            Console.WriteLine("Id={0}, Name={1}", reader.GetInt32("id"), reader.GetString("name"));
    }
    catch (Exception ex) {
        Console.WriteLine(ex.ToString());
    }
}

void IncorrectCommand(MySqlConnection connection, string[] command)
{
    Console.WriteLine("No such command!");
}

void MarkAttendance(MySqlConnection connection, string[] command)
{
    if (command.Length != 3)
    {
        Console.WriteLine("Incorrect syntax!");
        return;
    }

    try
    {
        var cmd = new MySqlCommand("INSERT INTO attendance (student_id, assignment_id) VALUES (@studentId, @assignmentId)", connection);
        cmd.Parameters.AddWithValue("@studentId", command[1]);
        cmd.Parameters.AddWithValue("@assignmentId", command[2]);
        cmd.ExecuteNonQuery();
        Console.WriteLine("Attendance marked!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

void SetMark(MySqlConnection connection, string[] command)
{
    if (command.Length != 4)
    {
        Console.WriteLine("Incorrect syntax!");
        return;
    }

    try
    {
        var cmd = new MySqlCommand("INSERT INTO attendance (student_id, assignment_id, mark) VALUES (@studentId, @assignmentId, @mark) " +
            "ON DUPLICATE KEY UPDATE mark = @mark", connection);
        cmd.Parameters.AddWithValue("@studentId", command[1]);
        cmd.Parameters.AddWithValue("@assignmentId", command[2]);
        cmd.Parameters.AddWithValue("@mark", command[3]);
        cmd.ExecuteNonQuery();
        Console.WriteLine("Mark set!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

void KillGuardian(MySqlConnection connection, string[] command)
{
    if (command.Length != 4)
    {
        Console.WriteLine("Incorrect syntax!");
        return;
    }

    try
    {
        var cmd = new MySqlCommand("DELETE FROM guardian WHERE first_name = @firstName AND surname = @surname AND email = @Email", connection);
        cmd.Parameters.AddWithValue("@firstName", command[1]);
        cmd.Parameters.AddWithValue("@surname", command[2]);
        cmd.Parameters.AddWithValue("@Email", command[3]);
        int rowsAffected = cmd.ExecuteNonQuery();
        if (rowsAffected > 0)
            Console.WriteLine("Guardian deleted!");
        else
            Console.WriteLine("Guardian not found.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

void WeekPlan(MySqlConnection connection, string[] command)
{
    if (command.Length != 2)
    {
        Console.WriteLine("Incorrect syntax!");
        return;
    }

    try
    {
        var cmd = new MySqlCommand("SELECT * FROM assignment WHERE teacher_id = @teacherId AND date BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 7 DAY)", connection);
        cmd.Parameters.AddWithValue("@teacherId", command[1]);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine("Assignment ID: {0}, Date: {1}", reader.GetInt32("id"), reader.GetDateTime("date"));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

void BusiestTeachers(MySqlConnection connection, string[] command)
{
    if (command.Length != 3)
    {
        Console.WriteLine("Incorrect syntax!");
        return;
    }
    try
    {
        var cmd = new MySqlCommand("SELECT teacher_id, COUNT(*) as assignment_count FROM assignment " +
            "WHERE date BETWEEN @startDate AND @endDate GROUP BY teacher_id " +
            "ORDER BY assignment_count DESC LIMIT 10", connection);
        cmd.Parameters.AddWithValue("@startDate", command[1]);
        cmd.Parameters.AddWithValue("@endDate", command[2]);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine("Teacher ID: {0}, Assignments: {1}", reader.GetInt32("teacher_id"), reader.GetInt32("assignment_count"));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

void ForeignHeads(MySqlConnection connection, string[] command)
{
    if (command.Length != 1)
    {
        Console.WriteLine("Incorrect syntax!");
        return;
    }

    try
    {
        var cmd = new MySqlCommand("SELECT d.name, t.first_name, t.surname FROM departments d " +
            "INNER JOIN teachers t ON d.id != t.departmentid " +
            "WHERE t.id IN (SELECT head_of_the_department FROM departments)", connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine("Department: {0}, Head: {1} {2}", reader.GetString("name"), reader.GetString("first_name"), reader.GetString("surname"));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

void UpdateNumberOfRooms(MySqlConnection connection, string[] command)
{
    if (command.Length != 1)
    {
        Console.WriteLine("Incorrect syntax!");
        return;
    }

    try
    {
        var cmd = new MySqlCommand("UPDATE buildings SET number_of_rooms = (SELECT COUNT(*) FROM rooms WHERE building_id = buildings.id)", connection);
        cmd.ExecuteNonQuery();
        Console.WriteLine("Number of rooms updated!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

void TooCrowded(MySqlConnection connection, string[] command)
{
    if (command.Length != 1)
    {
        Console.WriteLine("Incorrect syntax!");
        return;
    }
    try
    {
        var cmd = new MySqlCommand("SELECT a.id, COUNT(sa.student_id) / r.number_of_desks AS students_per_desk FROM assignment a " +
            "INNER JOIN attendance sa ON a.id = sa.assignment_id " +
            "INNER JOIN rooms r ON a.room_id = r.id " +
            "GROUP BY a.id HAVING students_per_desk > 2", connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine("Assignment ID: {0}, Students per Desk: {1}", reader.GetInt32("id"), reader.GetDecimal("students_per_desk"));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

void UnstableStudents(MySqlConnection connection, string[] command)
{
    if (command.Length != 1)
    {
        Console.WriteLine("Incorrect syntax!");
        return;
    }
    try
    {
        var cmd = new MySqlCommand("SELECT student_id, STDDEV(mark) as stddev FROM attendance GROUP BY student_id ORDER BY stddev DESC LIMIT 10", connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine("Student ID: {0}, Standard Deviation of Marks: {1}", reader.GetInt32("student_id"), reader.GetDecimal("stddev"));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

void Leadership(MySqlConnection connection, string[] command)
{
    if (command.Length != 1)
    {
        Console.WriteLine("Incorrect syntax!");
        return;
    }
    try
    {
        var cmd = new MySqlCommand("SELECT c.id, AVG(a.mark) as avg_mark FROM classes c " +
            "INNER JOIN students s ON c.class_leader = s.id " +
            "INNER JOIN attendance a ON s.id = a.student_id " +
            "WHERE YEAR(a.date) = YEAR(CURDATE()) GROUP BY c.id " +
            "HAVING avg_mark = (SELECT MIN(avg_mark) FROM " +
            "(SELECT AVG(mark) as avg_mark FROM attendance a " +
            "INNER JOIN students s ON a.student_id = s.id AND c.id = s.class_id))", connection); 
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine("Class ID: {0}, Average Mark: {1}", reader.GetInt32("id"), reader.GetDecimal("avg_mark"));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}
