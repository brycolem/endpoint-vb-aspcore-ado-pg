Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting

Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        builder.Configuration.AddEnvironmentVariables()

        Dim database As String = If(Environment.GetEnvironmentVariable("DATABASE"), "DefaultDatabase")
        Dim dbUser As String = If(Environment.GetEnvironmentVariable("DB_USER"), "DefaultUser")
        Dim dbPassword As String = If(Environment.GetEnvironmentVariable("DB_PWD"), "DefaultPassword")
        Dim dbHost As String = "postgres_bench"

        Dim connectionString As String = $"Server={dbHost};Database={database};User Id={dbUser};Password={dbPassword};"
        builder.Configuration("ConnectionStrings:DefaultConnection") = connectionString

        builder.Services.AddControllers()

        Dim app = builder.Build()

        If app.Environment.IsDevelopment() Then
            app.UseDeveloperExceptionPage()
        End If

        app.UseAuthorization()
        app.MapControllers()

        app.Run("http://0.0.0.0:8001")
    End Sub
End Module
