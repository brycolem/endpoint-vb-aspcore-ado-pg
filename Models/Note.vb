Imports System.Text.Json.Serialization

Namespace VBAspCoreAdoPg.Models
    Public Class Note
        Public Property Id As Integer

        <JsonPropertyName("noteText")>
        Public Property NoteText As String = String.Empty

        <JsonPropertyName("applicationId")>
        Public Property ApplicationId As Integer
    End Class
End Namespace
