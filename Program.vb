Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports System.Threading

Module Program
    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)
        ThreadPool.SetMinThreads(200, 200)

        builder.Configuration.AddEnvironmentVariables()

        Dim database As String = If(Environment.GetEnvironmentVariable("DATABASE"), "DefaultDatabase")
        Dim dbUser As String = If(Environment.GetEnvironmentVariable("DB_USER"), "DefaultUser")
        Dim dbPassword As String = If(Environment.GetEnvironmentVariable("DB_PWD"), "DefaultPassword")
        Dim dbHost As String = "localhost"

        Dim connectionString As String = $"Server={dbHost};Database={database};User Id={dbUser};Password={dbPassword};Pooling=true;Maximum Pool Size=1000;Timeout=60;"
        builder.Configuration("ConnectionStrings:DefaultConnection") = connectionString

        builder.Services.AddControllers()
        builder.Services.AddResponseCompression()

        Dim app = builder.Build()

        If app.Environment.IsDevelopment() Then
            app.UseDeveloperExceptionPage()
        End If

        app.UseResponseCompression()
        app.UseAuthorization()
        app.MapControllers()

        app.RunAsync("http://0.0.0.0:8001").Wait()
    End Sub
End Module
