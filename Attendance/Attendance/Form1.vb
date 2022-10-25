Imports System
Imports System.IO.Ports
Imports MySql.Data.MySqlClient


Public Class Form1
    Dim comPORT As String
    Dim receivedData As String = ""
    Dim SQL As New SQLControl
    Dim SQL2 As New SQLControl
    Dim SQLFill As New FillSQLControl
    Dim SQLFill2 As New FillSQLControl

    Private Sub clear()
        txtrfidno.Clear()
        txtname.Clear()
        txttagno.Clear()
        txtsection.Clear()
        txtyear.Clear()
        lblSubject.Text = ""
        lblInstructor.Text = ""
        lblRoom.Text = ""
        lblsubjcode.Text = ""
    End Sub
    
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.Show()
        txtrfidno.Focus()

        Timer2.Enabled = False
        comPORT = ""
        For Each sp As String In My.Computer.Ports.SerialPortNames
            comPort_ComboBox.Items.Add(sp)
        Next

    End Sub

    Private Sub Door()
        With SerialPort1
            .PortName = comPort_ComboBox.Text
            .BaudRate = 9600
            .DataBits = 8
            .Parity = IO.Ports.Parity.None
            .StopBits = IO.Ports.StopBits.One
            .Handshake = IO.Ports.Handshake.None

        End With

        Try
            SerialPort1.Open()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub LoadInformation()
        SQLFill.ExecQuery("Select * From stud_info Where ic_no = '" & txtrfidno.Text & "'")

        If SQLFill.RecordCount = 1 Then
            txttagno.Text = SQLFill.SQLDS.Tables(0).Rows(0).Item("stud_no")
            txtname.Text = SQLFill.SQLDS.Tables(0).Rows(0).Item("stud_name")
            txtyear.Text = SQLFill.SQLDS.Tables(0).Rows(0).Item("year")
            txtsection.Text = SQLFill.SQLDS.Tables(0).Rows(0).Item("stud_section")

            SQLFill2.ExecQuery("Select * From stud_subject Where ic_no = '" & txtrfidno.Text & "' and " & _
                               "time_start <= '" & lbl24hrstime.Text & "' and time_end >= '" & lbl24hrstime.Text & "'")


            If SQLFill2.RecordCount = 1 Then
                lblsubjcode.Text = SQLFill2.SQLDS.Tables(0).Rows(0).Item("subject_code")
                lblSubject.Text = SQLFill2.SQLDS.Tables(0).Rows(0).Item("subject_name")
                lblInstructor.Text = SQLFill2.SQLDS.Tables(0).Rows(0).Item("professor_name")
                lblRoom.Text = SQLFill2.SQLDS.Tables(0).Rows(0).Item("classroom")
                SaveAttendance()
                SerialPort1.Write("A")
                lblWarning.Visible = True
                Timer3.Interval = 2000
                Timer3.Start()
            Else
                lblWarning.Text = "No Schedule"
                SerialPort1.Write("B")
                lblWarning.Visible = True
                Timer3.Interval = 2000
                Timer3.Start()
            End If
        Else
            lblWarning.Text = "No Record Found"
            SerialPort1.Write("B")
            lblWarning.Visible = True
            Timer3.Interval = 2000
            Timer3.Start()
        End If

        If SQLFill.HasException(True) Then Exit Sub

    End Sub

    Private Sub SaveAttendance()
        SQL2.ExecQuery("Select * From attendance_info Where att_date = '" & lbldate.Text & "'")
        If SQL2.RecordCount >= 1 Then
            SQLFill.ExecQuery("Select * From attendance_info Where att_date = '" & lbldate.Text & "' and ic_no ='" & txtrfidno.Text & "' Order by status Desc")
            If SQLFill.RecordCount <= 0 Then
                lblstatus.Text = 0
            Else
                lblstatus.Text = SQLFill.SQLDS.Tables(0).Rows(0).Item("status")
            End If

            Dim Ta As Integer
            Ta = Val(lblstatus.Text) + 1
            If Ta Mod 2 = 0 Then
                lblstatus.Text = Val(lblstatus.Text) + 1
                SQL.AddParam("@icno", txtrfidno.Text)
                SQL.AddParam("@studno", txtrfidno.Text)
                SQL.AddParam("@studname", txtname.Text)
                SQL.AddParam("@attstat", "Out")
                SQL.AddParam("@timeinout", lbltime.Text)
                SQL.AddParam("@attdate", lbldate.Text)
                SQL.AddParam("@subjcode", lblsubjcode.Text)
                SQL.AddParam("@subjname", lblSubject.Text)
                SQL.AddParam("@instructor", lblInstructor.Text)
                SQL.AddParam("@room", lblRoom.Text)
                SQL.AddParam("@stat", lblstatus.Text)

                SQL.ExecQuery("Insert into attendance_info (ic_no, stud_no, stud_name, att_stat, att_date, " & _
                              "subject_code, subject_name, instructor_name, classroom, status, time_in_out) Values (@icno, @studno, " & _
                              "@studname, @attstat, @attdate, @subjcode, @subjname, @instructor, @room, @stat, @timeinout);")

                If SQL.HasException(True) Then Exit Sub

                lblWarning.Text = "Successfully Logged Out"
            Else
                lblstatus.Text = Val(lblstatus.Text) + 1
                SQL.AddParam("@icno", txtrfidno.Text)
                SQL.AddParam("@studno", txtrfidno.Text)
                SQL.AddParam("@studname", txtname.Text)
                SQL.AddParam("@attstat", "In")
                SQL.AddParam("@timeinout", lbltime.Text)
                SQL.AddParam("@attdate", lbldate.Text)
                SQL.AddParam("@subjcode", lblsubjcode.Text)
                SQL.AddParam("@subjname", lblSubject.Text)
                SQL.AddParam("@instructor", lblInstructor.Text)
                SQL.AddParam("@room", lblRoom.Text)
                SQL.AddParam("@stat", lblstatus.Text)

                SQL.ExecQuery("Insert into attendance_info (ic_no, stud_no, stud_name, att_stat, att_date, " & _
                              "subject_code, subject_name, instructor_name, classroom, status, time_in_out) Values (@icno, @studno, " & _
                              "@studname, @attstat, @attdate, @subjcode, @subjname, @instructor, @room, @stat, @timeinout);")

                If SQL.HasException(True) Then Exit Sub

                lblWarning.Text = "Successfully Logged In"
            End If
        Else
            SQL.AddParam("@icno", txtrfidno.Text)
            SQL.AddParam("@studno", txtrfidno.Text)
            SQL.AddParam("@studname", txtname.Text)
            SQL.AddParam("@attstat", "In")
            SQL.AddParam("@timeinout", lbltime.Text)
            SQL.AddParam("@attdate", lbldate.Text)
            SQL.AddParam("@subjcode", lblsubjcode.Text)
            SQL.AddParam("@subjname", lblSubject.Text)
            SQL.AddParam("@instructor", lblInstructor.Text)
            SQL.AddParam("@room", lblRoom.Text)
            SQL.AddParam("@stat", "1")

            SQL.ExecQuery("Insert into attendance_info (ic_no, stud_no, stud_name, att_stat, att_date, " & _
                          "subject_code, subject_name, instructor_name, classroom, status, time_in_out) Values (@icno, @studno, " & _
                          "@studname, @attstat, @attdate, @subjcode, @subjname, @instructor, @room, @stat, @timeinout);")

            If SQL.HasException(True) Then Exit Sub

            lblWarning.Text = "Successfully Logged In"
        End If

    End Sub

    Private Sub offset_Click(sender As Object, e As EventArgs) Handles offset.Click
        If Panel2.Width = 43 Then

            Panel2.Hide()
            Panel2.Width = 212
            Me.Width = 799
            BunifuTransition1.ShowSync(Panel2)

            Panel2.Show()
            Me.StartPosition = FormStartPosition.CenterScreen
   

            comPort_ComboBox.Visible = True
            connect_BTN.Visible = True
            Timer_LBL.Visible = True
        ElseIf Panel2.Width = 212 Then

            'Panel2.BackColor = Color.FromArgb(200, 34, 40, 49)
            Panel2.Hide()
            Panel2.Width = 43
            Me.Width = 637
            BunifuTransition1.ShowSync(Panel2)
            Me.StartPosition = FormStartPosition.CenterScreen
            Panel2.Show()
            comPort_ComboBox.Visible = False
            connect_BTN.Visible = False
            Timer_LBL.Visible = False
        End If
    End Sub

    Private Sub txtrfidno_TextChanged(sender As Object, e As EventArgs) Handles txtrfidno.TextChanged
        LoadInformation()
        'Door()
    End Sub

    Private Sub BunifuFlatButton1_Click(sender As Object, e As EventArgs) Handles BunifuFlatButton1.Click
        Application.Exit()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        lbltime.Text = DateTime.Now.ToString("hh:mm:ss tt")
        lbl24hrstime.Text = DateTime.Now.ToString("HH:mm:ss")
        lbldate.Text = DateTime.Now.ToString("MMMM dd, yyyy")
    End Sub

    Private Sub comPort_ComboBox_SelectedIndexChanged_SelectedIndexChanged(sender As Object, e As EventArgs) Handles comPort_ComboBox.SelectedIndexChanged
        If (comPort_ComboBox.SelectedItem <> "") Then
            comPORT = comPort_ComboBox.SelectedItem
        End If
    End Sub

    Private Sub connect_BTN_Click_Click(sender As Object, e As EventArgs) Handles connect_BTN.Click
        If (connect_BTN.Text = "Connect") Then
            If (comPORT <> "") Then
                SerialPort1.Close()
                SerialPort1.PortName = comPORT
                SerialPort1.BaudRate = 9600
                SerialPort1.DataBits = 8
                SerialPort1.Parity = Parity.None
                SerialPort1.StopBits = StopBits.One
                SerialPort1.Handshake = Handshake.None
                SerialPort1.Encoding = System.Text.Encoding.Default
                SerialPort1.ReadTimeout = 10000


                SerialPort1.Open()
                connect_BTN.Text = "Dis-connect"
                Timer2.Enabled = True
                Timer_LBL.Text = "Connection : ON"
            Else
                MsgBox("Select a COM port first")
            End If
        Else
            SerialPort1.Close()
            connect_BTN.Text = "Connect"
            Timer2.Enabled = False
            Timer_LBL.Text = "Connection : OFF"
        End If
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        receivedData = ReceiveSerialData()
        txtrfidno.Text &= receivedData
     

    End Sub

    Function ReceiveSerialData() As String
        Dim Incoming As String
        Try
            Incoming = SerialPort1.ReadExisting()
            If Incoming Is Nothing Then
                Return "nothing" & vbCrLf
            Else
                Return Incoming
            End If
        Catch ex As TimeoutException
            Return "Error: Serial Port read timed out."
        End Try

    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        txtrfidno.Clear()
        txtrfidno.Focus()
    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        Timer3.Stop()

        clear()
        txtrfidno.Clear()
        lblWarning.Visible = False
    End Sub

End Class
