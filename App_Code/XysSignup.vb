Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data
Imports System.IO

Public Class XysSignup
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

        Dim Title As New Label(Translator.Format("signup"))
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px")
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px")
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px")
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0")

        Dim text As New Texts(Translator.Format("name"), "name", TextTypes.text)
        text.Required = True
        text.Text.SetStyle(HtmlStyles.width, "200px")
        text.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")
        text.Text.SetAttribute(HtmlEvents.onblur, "txtblur(event, this)")

        Dim text1 As New Texts(Translator.Format("email"), "email", TextTypes.text)
        text1.Required = True
        text1.Text.SetStyle(HtmlStyles.width, "350px")
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "150")
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")
        text1.Text.SetAttribute(HtmlEvents.onblur, "txtblur(event, this)")

        Dim text2 As New Texts(Translator.Format("pwd"), "pwd", TextTypes.password)
        text2.Required = True
        text2.Text.SetStyle(HtmlStyles.width, "200px")
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "20")
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")
        text2.Text.SetAttribute(HtmlEvents.onblur, "txtblur(event, this)")

        Dim text3 As New Texts(Translator.Format("pwd1"), "pwd1", TextTypes.password)
        text3.Required = True
        text3.Text.SetStyle(HtmlStyles.width, "200px")
        text3.Text.SetAttribute(HtmlAttributes.maxlength, "20")
        text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")
        text3.Text.SetAttribute(HtmlEvents.onblur, "txtblur(event, this)")

        Dim btn As New Button(Translator.Format("next"), Button.ButtonTypes.Button)
        btn.SetAttribute(HtmlAttributes.class, "button")
        btn.SetAttribute(HtmlEvents.onclick, "NavXysVerify()")

        Dim btn1 As New Button(Translator.Format("back"), Button.ButtonTypes.Button)
        btn1.SetAttribute(HtmlAttributes.class, "button")
        btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignin()")

        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.position, "relative")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.width, "500px")

        elmBox.AddItem(Title, 34)
        elmBox.AddItem(text, 1)
        elmBox.AddItem(text1, 14)
        elmBox.AddItem(text2, 1)
        elmBox.AddItem(text3, 40)
        elmBox.AddItem(btn1)
        elmBox.AddItem(btn, 10)

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText

        HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox)
    End Sub

    Public Function NavXysSignin() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.Navigate(References.Pages.XysSignin)
        Return _ApiResponse
    End Function
    Public Function NavXysVerify() As ApiResponse
        Dim name As String = GetDataValue("name")
        Dim email As String = GetDataValue("email")
        Dim pwd As String = GetDataValue("pwd")
        Dim pwd1 As String = GetDataValue("pwd1")

        Dim _ApiResponse As New ApiResponse

        Dim Valid As Boolean = True
        Dim DialogMsg As String = String.Empty

        If name = String.Empty OrElse _
            email = String.Empty OrElse _
            pwd = String.Empty OrElse _
            pwd1 = String.Empty Then
            Valid = False
        End If

        If Valid = False Then
            DialogMsg = Translator.Format("msg_required")
        Else
            If pwd <> pwd1 Then
                DialogMsg = Translator.Format("msg_pwdconfirm")
            Else
                Dim rlt As String = ValidatePassword(pwd)
                'If rlt <> String.Empty Then
                '    DialogMsg = Translator.Format("msg_pwdvalid") + rlt
                'End If
            End If
        End If

        If DialogMsg <> String.Empty Then
            Dim dialogBox As New DialogBox(DialogMsg)
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
            _ApiResponse.ExecuteScript("ShowButtons();")
        Else
            If ExistsEmail(email) = True Then
                Dim dialogBox As New DialogBox(Translator.Format("msg_exist"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
                _ApiResponse.ExecuteScript("ShowButtons();")
            Else
                Dim data As New ViewData
                data.Tid = NewID()
                data.Name = name
                data.Email = email
                data.Pass = Encryptor.EncryptData(pwd)
                data.OTP = RandNUM()
                data.IpAddr = ClientIPAddress
                data.Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                data.Expired = DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss")

                Dim MailFile As String = HtmlFolder + References.Htmls.Email_SignUp + "_" + ClientLanguage + ".html"
                If File.Exists(MailFile) = False Then
                    MailFile = References.Htmls.Email_SignUp
                Else
                    MailFile = References.Htmls.Email_SignUp + "_" + ClientLanguage
                End If

                Dim rlt As String = SaveViewData(data)
                If rlt = String.Empty Then
                    Dim Subject As String = HtmlTranslator.Value("msg_email")
                    Dim bodyHtml As String = ReadHtmlFile(MailFile) _
                                             .Replace("{username}", data.Name) _
                                             .Replace("{useremail}", data.Email) _
                                             .Replace("{userpin}", data.OTP)
                    Dim ToAddr As String() = {data.Email}

                    Dim rltmail As String = SendEmail(Subject, bodyHtml, ToAddr)
                    If rltmail = String.Empty Then
                        Dim SerializedString As String = SerializeObjectEnc(data, GetType(ViewData))
                        _ApiResponse.SetCookie(References.Keys.SignUp_User, SerializedString)
                        _ApiResponse.Navigate(References.Pages.XysVerify)
                    Else
                        Dim dialogBox As New DialogBox(rltmail)
                        dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                        _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
                        _ApiResponse.ExecuteScript("ShowButtons();")
                    End If

                Else
                    Dim dialogBox As New DialogBox(rlt)
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
                    _ApiResponse.ExecuteScript("ShowButtons();")
                End If
            End If

        End If

        Return _ApiResponse
    End Function

    Private Function SaveViewData(data As ViewData) As String
        '" Insert into XysSignUp( Tid,Name,Email,Pass,OTP,IpAddr,Created,Expired) " +
        '" values ( N'" + SignUp.Tid + "',N'" + SignUp.Name + "',N'" + SignUp.Email + "', " +
        '"          N'" + SignUp.Pass + "',N'" + SignUp.OTP + "',N'" + SignUp.IpAddr + "', " +
        '"          N'" + SignUp.Created + "',N'" + SignUp.Expired + "') ",

        Dim SQL As New List(Of String) From _
        {
            " Insert into XysSignUp( Tid,Name,Email,Pass,OTP,IpAddr,Created,Expired) " +
            " values ( @Tid,@Name,@Email,@Pass,@OTP,@IpAddr,@Created,@Expired) "
        }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@Tid", .Value = data.Tid, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@Name", .Value = data.Name, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@Email", .Value = data.Email, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@Pass", .Value = data.Pass, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@OTP", .Value = data.OTP, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@IpAddr", .Value = data.IpAddr, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@Created", .Value = data.Created, .SqlDbType = SqlDbType.DateTime})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@Expired", .Value = data.Expired, .SqlDbType = SqlDbType.DateTime})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function


    Private Function ExistsEmail(UserEmail As String) As Boolean
        Dim rtnvlu As Boolean = False

        Dim SQLText As New SQLText
        SQLText.Sql = " if exists(select * from XysUserInfo  where UserEmail = @UserEmail) " +
                      " begin select 1 end else begin select 0 end "
        SQLText.Params.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserEmail", .Value = UserEmail, .SqlDbType = SqlDbType.NVarChar})

        Dim emsg As String = String.Empty
        Dim dt As DataTable = SQLData.SQLDataTable(SQLText.ToString, emsg)
        If emsg = String.Empty AndAlso dt IsNot Nothing AndAlso dt.Rows.Count <> 0 Then
            If Val(dt.Rows(0)(0).ToString) = 1 Then
                rtnvlu = True
            End If
        End If
        Return rtnvlu
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
        Public Property Name As String = String.Empty
        Public Property Email As String = String.Empty
        Public Property Pass As String = String.Empty
        Public Property OTP As String = String.Empty
        Public Property IpAddr As String = String.Empty
        Public Property Created As String = String.Empty
        Public Property Expired As String = String.Empty
    End Class
End Class

