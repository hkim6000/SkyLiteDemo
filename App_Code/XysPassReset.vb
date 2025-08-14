Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data
Imports System.IO

Public Class XysPassReset
    Inherits WebPage

    Private AppKey As AppKey = Nothing
    Sub New()
        HtmlTranslator.Add(GetPageDict(Me.GetType.ToString))
    End Sub
    Protected Friend Function GetPageDict(pagename As String) As List(Of Translator.[Dictionary])
        Dim rlt As New List(Of Translator.[Dictionary])
        Dim SSQL As String = _
                        " declare @pageid nvarchar(50),@isocode nvarchar(10)  " +
                        " set @pageid = N'" + pagename + "' " +
                        " set @isocode = N'" + ClientLanguage + "' "

        Select Case ClientLanguage.Contains("-")
            Case True
                SSQL += " if exists(select * from XysDict where Isocode =  @isocode) " +
                       " begin " +
                       "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                       "  Where (Target = @pageid or Target = '*') " +
                       "          and (Isocode = '*' or Isocode = @isocode ) " +
                       "  order by KeyWord  " +
                       " end " +
                       " else " +
                       " begin " +
                       "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                       "  Where (Target = @pageid or Target = '*') " +
                       "          and (Isocode = '*' or Isocode =  'en-US' ) " +
                       "  order by KeyWord  " +
                       " end "
            Case False
                SSQL += " if exists(select * from XysDict where left(Isocode,2) =  @isocode) " +
                       " begin " +
                       "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                       "  Where (Target = @pageid or Target = '*') " +
                       "          and (Isocode = '*' or Isocode = @isocode ) " +
                       "  order by KeyWord  " +
                       " end " +
                       " else " +
                       " begin " +
                       "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                       "  Where (Target = @pageid or Target = '*') " +
                       "          and (Isocode = '*' or Isocode =  'en-US' ) " +
                       "  order by KeyWord  " +
                       " end "
        End Select

        Dim emsg As String = String.Empty
        Dim dt As DataTable = SQLData.SQLDataTable(SSQL, emsg)
        If emsg = String.Empty AndAlso dt IsNot Nothing AndAlso dt.Rows.Count <> 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim _Dict As New Translator.[Dictionary]
                _Dict.IsoCode = dt.Rows(i)(1).ToString
                _Dict.DicKey = dt.Rows(i)(2).ToString
                _Dict.DicWord = dt.Rows(i)(3).ToString
                rlt.Add(_Dict)
            Next
        End If
        Return rlt
    End Function
    Public Overrides Sub OnInitialized()
        HtmlDoc.AddJsFile("WebScript.js")
        HtmlDoc.AddCSSFile("WebStyle.css")
        HtmlDoc.SetTitle(Translator.Format("title"))


        Dim Title As New Label(Translator.Format("reset"))
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px")
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px")
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px")
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0")

        Dim lbl1 As New Label(Translator.Format("enteremail"))
        lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px")
        lbl1.Wrap.SetStyle(HtmlStyles.color, "#444")

        Dim text As New Texts(Translator.Format("email"), "email", TextTypes.text)
        text.Required = True
        text.Text.SetStyle(HtmlStyles.width, "330px")

        Dim btn As New Button(Translator.Format("next"), Button.ButtonTypes.Button)
        btn.SetAttribute(HtmlAttributes.class, "button")
        btn.SetAttribute(HtmlEvents.onclick, "NavXysSent()")

        Dim btn1 As New Button(Translator.Format("back"), Button.ButtonTypes.Button)
        btn1.SetStyle(HtmlStyles.marginLeft, "12px")
        btn1.SetAttribute(HtmlAttributes.class, "button1")
        btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignin()")


        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.position, "relative")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.width, "500px")

        elmBox.AddItem(Title, 16)
        elmBox.AddItem(lbl1, 20)
        elmBox.AddItem(text, 40)
        elmBox.AddItem(btn)
        elmBox.AddItem(btn1, 10)

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText

        HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox)
    End Sub

    Public Function NavXysSignin() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.Navigate(References.Pages.XysSignin)
        Return _ApiResponse
    End Function
    Public Function NavXysSent() As ApiResponse
        Dim email As String = GetDataValue("email")

        Dim _ApiResponse As New ApiResponse

        If email <> String.Empty Then

            Dim data As ViewData = GetViewData(email)
            If data IsNot Nothing Then
                Dim SerializedString As String = SerializeObjectEnc(data, GetType(ViewData))

                Dim UserLink As String = VirtualPath + "xyspasschange?x=" + SerializedString

                Dim MailFile As String = HtmlFolder + References.Htmls.Email_PassReset + "_" + ClientLanguage + ".html"
                If File.Exists(MailFile) = False Then
                    MailFile = References.Htmls.Email_PassReset
                Else
                    MailFile = References.Htmls.Email_PassReset + "_" + ClientLanguage
                End If

                Dim Subject As String = HtmlTranslator.Value("msg_reset")
                Dim bodyHtml As String = ReadHtmlFile(MailFile) _
                                         .Replace("{username}", data.Name) _
                                         .Replace("{useremail}", data.Email) _
                                         .Replace("{userlink}", UserLink)
                Dim ToAddr As String() = {data.Email}

                Dim rltmail As String = SendEmail(Subject, bodyHtml, ToAddr)
                If rltmail = String.Empty Then
                    Dim rlt As String = SaveViewData(data)
                    If rlt = String.Empty Then
                        _ApiResponse.Navigate(References.Pages.XysSent)
                    Else
                        Dim dialogBox As New DialogBox(rlt)
                        dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                        _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
                        _ApiResponse.ExecuteScript("ShowButtons();")
                    End If
                Else
                    Dim dialogBox As New DialogBox(rltmail)
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
                    _ApiResponse.ExecuteScript("ShowButtons();")
                End If
            Else
                _ApiResponse.Navigate(References.Pages.XysSent)
            End If
        Else
            Dim dialogBox As New DialogBox(Translator.Format("msg_email"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText)
        End If
        Return _ApiResponse
    End Function

    Private Function SaveViewData(data As ViewData) As String
        Dim SQL As New List(Of String) From _
            {
            " Insert into XysUserReset( Tid,Email,UserId,Status,Created,Expired) " +
            " values ( N'" + data.Tid + "',N'" + data.Email + "',N'" + data.UserId + "',0,getdate(),dateadd(minute,10,getdate())) "
            }

        Return PutData(SQL)
    End Function

    Private Function GetViewData(UserEmail As String) As ViewData
        Dim _data As ViewData = Nothing

        Dim SSql As String = " select a.UserId,UserEmail,UserName " +
                            " from XysUser a inner join XysUserInfo b on a.UserId = b.UserId " +
                            " where b.UserEmail = @UserEmail "

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserEmail", .Value = UserEmail, .SqlDbType = SqlDbType.NVarChar})

        Dim emsg As String = String.Empty
        Dim dt As DataTable = SQLData.SQLDataTable(SqlWithParams(SSql, SqlParams), emsg)
        If emsg = String.Empty AndAlso dt IsNot Nothing AndAlso dt.Rows.Count <> 0 Then
            _data = New ViewData
            _data.Tid = NewID()
            _data.UserId = dt.Rows(0)(0).ToString
            _data.Email = dt.Rows(0)(1).ToString
            _data.Name = dt.Rows(0)(2).ToString
        End If
        Return _data
    End Function

    Private Function PutData(SQL As List(Of String)) As String
        Dim emsg As String = String.Empty
        SQLData.SQLDataPut(SQL, emsg)
        WriteXysLog(String.Join("|", SQL), emsg)
        Return emsg
    End Function
    Private Sub WriteXysLog(logTxt As String, ByRef msg As String)
        Dim userid As String = If(AppKey IsNot Nothing, AppKey.UserId, String.Empty)

        Dim SQL As New List(Of String) From _
        {
            "insert into XysLog(LogId,ClientIp,UserId,LogTxt,JobRlt,SysDte) " +
            "values(@LogId,@ClientIp,@UserId,@LogTxt,@JobRlt,GETDATE())"
        }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@LogId", .Value = NewID(), .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@ClientIp", .Value = ClientIPAddress, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserId", .Value = userid, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@LogTxt", .Value = EscQuote(logTxt.Trim), .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@JobRlt", .Value = EscQuote(msg.Trim), .SqlDbType = SqlDbType.NVarChar})

        Dim emsg As String = String.Empty
        Dim _SqlData As New SQLData
        _SqlData.DataPut(SqlWithParams(SQL, SqlParams), emsg)
    End Sub

    Private Function SendEmail(Subject As String, bodyHtml As String, ToAddr As String()) As String
        Dim mail As New Mail
        mail.Subject = Subject
        mail.ToAddr = ToAddr
        mail.Body = bodyHtml
        Dim rlt As String = mail.SendMail()

        Return rlt
    End Function
    Public Class ViewData
        Public Property Tid As String = String.Empty
        Public Property UserId As String = String.Empty
        Public Property Email As String = String.Empty
        Public Property Name As String = String.Empty
        Public Property Pwd As String = String.Empty
    End Class

End Class
