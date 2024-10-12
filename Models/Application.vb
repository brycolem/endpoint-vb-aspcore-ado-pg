Imports System.Text.Json.Serialization

Namespace VBAspCoreAdoPg.Models
    Public Class Application
        Public Property Id As Integer
        Public Property Employer As String = String.Empty
        Public Property Title As String = String.Empty
        Public Property Link As String = String.Empty

        <JsonPropertyName("companyId")>
        Public Property CompanyId As Integer

        Public Property Notes As List(Of Note)

        Public Sub New()
            Notes = New List(Of Note)()
        End Sub
    End Class
End Namespace
