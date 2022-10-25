Imports MySql.Data.MySqlClient

Public Class FillSQLControl
    Private SQLCon As New MySqlConnection("Server=localhost; Database=std_db; User=root;")
    Private SQLCmd As MySqlCommand

    Public SQLDA As MySqlDataAdapter
    Public SQLDS As DataSet

    Public Params As New List(Of MySqlParameter)

    Public RecordCount As Integer
    Public Exception As String

    Public Sub ExecQuery(Query As String)
        Try
            SQLCon.Open()

            SQLCmd = New MySqlCommand(Query, SQLCon)

            Params.ForEach(Sub(x) SQLCmd.Parameters.Add(x))

            Params.Clear()

            ' Execute command and fill the Dataset
            SQLDS = New DataSet
            SQLDA = New MySqlDataAdapter(SQLCmd)
            RecordCount = SQLDA.Fill(SQLDS)

            SQLCon.Close()
        Catch ex As Exception
            Exception = ex.Message
        Finally
            If SQLCon.State = ConnectionState.Open Then
                SQLCon.Close()
            End If
        End Try
    End Sub

    Public Sub AddParam(Name As String, Value As Object)
        Dim NewParam As New MySqlParameter(Name, Value)
        Params.Add(NewParam)
    End Sub

    Public Function HasException(Optional Report As Boolean = False) As Boolean
        If String.IsNullOrEmpty(Exception) Then
            Return False
        End If
        If Report = True Then
            MsgBox(Exception, MsgBoxStyle.Critical, "Exception")
        End If
        Return True
    End Function
End Class
