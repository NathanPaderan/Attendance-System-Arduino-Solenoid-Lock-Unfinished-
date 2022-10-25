Imports MySql.Data.MySqlClient

Public Class SQLControl
    Private DBCon As New MySqlConnection("Server=localhost; Database=std_db; User=root;")
    Private DBCmd As MySqlCommand

    'DB Data
    Public DBDA As MySqlDataAdapter
    Public DBDT As DataTable

    'Query Parameters
    Public Params As New List(Of MySqlParameter)

    'Query Statistics
    Public RecordCount As Integer
    Public Exception As String

    Public Sub New()
    End Sub

    ' allow connection string override
    Public Sub New(ConnectionString As String)
        DBCon = New MySqlConnection(ConnectionString)
    End Sub

    ' execute query sub
    Public Sub ExecQuery(Query As String)
        ' reset query stats
        RecordCount = 0
        Exception = ""

        Try
            DBCon.Open()

            'Create DB Command
            DBCmd = New MySqlCommand(Query, DBCon)

            'Load params into DB command
            Params.ForEach(Sub(p) DBCmd.Parameters.Add(p))

            Params.Clear()

            'Execute command and fill dataset 
            DBDT = New DataTable
            DBDA = New MySqlDataAdapter(DBCmd)
            RecordCount = DBDA.Fill(DBDT)
        Catch ex As Exception
            Exception = "ExecQuery Error: " & vbNewLine & ex.Message
        Finally
            If DBCon.State = ConnectionState.Open Then
                DBCon.Close()
            End If
        End Try
    End Sub

    'Add param sub  
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
