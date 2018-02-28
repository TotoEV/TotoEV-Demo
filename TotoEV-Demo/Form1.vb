Imports System.Net
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Form1
    Public AccessToken As String = "174303fb4870ed9400655d3f086ee990"
    Public ID As String
    Public InUse As String
    Public State As String = Nothing
    Public Sub program_load() Handles MyBase.Load
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12
    End Sub

    Private Function API_Send(ByRef State As Integer)
        Dim BodyText As String = Nothing

        BodyText = "{""in_use"":" & State & ",""assigned_to"": 1}"
        'MsgBox(BodyText)
        'convert text into ascii
        Dim encoding As New ASCIIEncoding()
        Dim Bytes As Byte() = encoding.GetBytes(BodyText)
        Dim request = TryCast(System.Net.WebRequest.Create("https://server.totoev.com.au/api/cards/" & ID), System.Net.HttpWebRequest)

        request.Method = "PUT"
        request.Headers.Add("X-Access-Token", AccessToken)
        request.ContentType = "application/json"
        request.ContentLength = Bytes.Length

        'write the info into the json body
        Dim Stream = request.GetRequestStream()
        Stream.Write(Bytes, 0, Bytes.Length)
        Stream.Close()


        Dim responseContent As String
        Using response = TryCast(request.GetResponse(), System.Net.HttpWebResponse)
            Using reader = New System.IO.StreamReader(response.GetResponseStream())
                responseContent = reader.ReadToEnd()
                'MessageBox.Show(responseContent)
            End Using
        End Using

        Dim json = JsonConvert.DeserializeObject(Of Rootobject)(responseContent)

        If json.data.in_use = 1 Then
            Button1.Text = "Tap Off"
        ElseIf json.data.in_use = 0 Then
            Button1.Text = "Tap On"
        End If
        Return True
    End Function

    Private Function API_Recieve(ByRef CardIDNumber As String)
        Dim request = TryCast(System.Net.WebRequest.Create("https://server.totoev.com.au/api/cards/" & CardIDNumber), System.Net.HttpWebRequest)

        request.Method = "GET"
        request.Headers.Add("X-Access-Token", AccessToken)

        Dim responseContent As String
        Using response = TryCast(request.GetResponse(), System.Net.HttpWebResponse)
            Using reader = New System.IO.StreamReader(response.GetResponseStream())
                responseContent = reader.ReadToEnd()
                'MsgBox(responseContent)
                'My.Computer.Clipboard.SetText(responseContent)
            End Using
        End Using

        Dim json = JsonConvert.DeserializeObject(Of Rootobject)(responseContent)
        ID = json.data.id
        InUse = json.data.in_use

        Return True
    End Function

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Button1.Text = "Getting card status..."
        Dim ID As String = ComboBox1.SelectedItem.ToString()
        Dim CardIDNumber As String = ID.Substring(ID.Length - 1)
        API_Recieve(CardIDNumber)

        If InUse = 0 Then
            Button1.Text = "Tap On"
        Else
            Button1.Text = "Tap Off"
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Button1.Text = "Tap On" Then
            State = 1
            API_Send(State)
            State = Nothing
        ElseIf Button1.Text = "Tap Off" Then
            State = 0
            API_Send(State)
            State = Nothing
        End If
    End Sub
End Class



Public Class Rootobject
    Public Property status As Integer
    Public Property data As Data
End Class

Public Class Data
    Public Property id As Integer
    Public Property card_number As String
    Public Property network As String
    Public Property in_use As String
    Public Property assigned_to As String
    Public Property created_at As String
    Public Property updated_at As String
End Class


