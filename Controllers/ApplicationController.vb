Imports Microsoft.AspNetCore.Mvc
Imports Npgsql
Imports System.Data
Imports System.Text.Json
Imports Microsoft.Extensions.Configuration

Namespace VBAspCoreAdoPg.Controllers
    <ApiController>
    <Route("application")>
    Public Class ApplicationsController
        Inherits ControllerBase

        Private ReadOnly _configuration As IConfiguration

        Public Sub New(configuration As IConfiguration)
            _configuration = configuration
        End Sub

        ' GET: /application
        <HttpGet>
        Public Async Function GetApplications() As Task(Of IActionResult)
            Dim connectionString As String = _configuration.GetConnectionString("DefaultConnection")
            Dim applications As New List(Of VBAspCoreAdoPg.Models.Application)()

            Using connection As New NpgsqlConnection(connectionString)
                Await connection.OpenAsync()

                Dim query As String = "
                    SELECT a.id, a.employer, a.title, a.link, a.company_id,
                        COALESCE(json_agg(json_build_object('id', n.id, 'noteText', n.note_text, 'applicationId', n.application_id)) 
                        FILTER (WHERE n.id IS NOT NULL), '[]') as notes
                    FROM applications a
                    LEFT JOIN notes n ON a.id = n.application_id
                    GROUP BY a.id, a.employer, a.title, a.link, a.company_id
                    LIMIT 100 OFFSET 0"

                Using command As New NpgsqlCommand(query, connection)
                    Using reader As NpgsqlDataReader = Await command.ExecuteReaderAsync()
                        While Await reader.ReadAsync()
                            Dim app As New VBAspCoreAdoPg.Models.Application With {
                                .Id = reader.GetInt32(0),
                                .Employer = reader.GetString(1),
                                .Title = reader.GetString(2),
                                .Link = reader.GetString(3),
                                .CompanyId = reader.GetInt32(4),
                                .Notes = New List(Of VBAspCoreAdoPg.Models.Note)()
                            }

                            If Not reader.IsDBNull(5) Then
                                Dim notesJson As String = reader.GetString(5)
                                Try
                                    Dim notesList As List(Of VBAspCoreAdoPg.Models.Note) = JsonSerializer.Deserialize(Of List(Of VBAspCoreAdoPg.Models.Note))(notesJson)
                                    If notesList IsNot Nothing Then
                                        app.Notes = notesList
                                    End If
                                Catch jsonEx As JsonException
                                    Console.WriteLine($"Error deserializing notes JSON: {jsonEx.Message}")
                                End Try
                            End If

                            applications.Add(app)
                        End While
                    End Using
                End Using
            End Using

            Return Ok(applications)
        End Function

        ' POST: /application
        <HttpPost>
        Public Async Function AddApplication(<FromBody> application As VBAspCoreAdoPg.Models.Application) As Task(Of IActionResult)
            Dim connectionString As String = _configuration.GetConnectionString("DefaultConnection")

            Using connection As New NpgsqlConnection(connectionString)
                Await connection.OpenAsync()
                Using command As New NpgsqlCommand("INSERT INTO applications (employer, title, link, company_id) VALUES (@employer, @title, @link, @companyId) RETURNING id", connection)
                    command.Parameters.AddWithValue("employer", application.Employer)
                    command.Parameters.AddWithValue("title", application.Title)
                    command.Parameters.AddWithValue("link", application.Link)
                    command.Parameters.AddWithValue("companyId", application.CompanyId)

                    Dim newApplicationId As Integer = Convert.ToInt32(Await command.ExecuteScalarAsync())
                    application.Id = newApplicationId
                End Using
            End Using

            Return Ok($"Application '{application.Title}' added successfully.")
        End Function
    End Class
End Namespace
