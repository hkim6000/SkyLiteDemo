Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data
Imports System.Net
Imports System.IO

Public Class XysPassChange
    Inherits WebPage

    Private data As ViewData
    Private AppKey As AppKey = Nothing

    Sub New()
        Try
            HtmlTranslator.Add(GetPageDict(Me.GetType.ToString))
            Dim QryVlu As String = QueryValue("x")
            data = DeserializeObjectEnc(QryVlu, GetType(ViewData))
        Catch ex As Exception
            data = Nothing
        End Try
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
    Public Overrides Sub OnBeforRender()
        If data Is Nothing Then
            HtmlDoc.InitialScripts.Navigate(References.Pages.XysSignin)
        Else
            HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox)
        End If
    End Sub

    Public Overrides Sub OnInitialized()
        HtmlDoc.AddJsFile("WebScript.js")
        HtmlDoc.AddCSSFile("WebStyle.css")
        HtmlDoc.SetTitle(Translator.Format("title"))

        Dim Title As New Label(Translator.Format("changepwd"))
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px")
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px")
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px")
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0")

        Dim lbl1 As New Label(Translator.Format("enterpwd"))
        lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px")
        lbl1.Wrap.SetStyle(HtmlStyles.color, "#444")

        Dim text As New Texts(Translator.Format("pwd"), "pwd", TextTypes.password)
        text.Required = True
        text.Text.SetStyle(HtmlStyles.width, "180px")
        text.Text.SetAttribute(HtmlAttributes.maxlength, "15")

        Dim text1 As New Texts(Translator.Format("pwd1"), "pwd1", TextTypes.password)
        text1.Required = True
        text1.Text.SetStyle(HtmlStyles.width, "180px")
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "15")

        Dim btn As New Button(Translator.Format("submit"), Button.ButtonTypes.Button)
        btn.SetAttribute(HtmlAttributes.class, "button")
        btn.SetAttribute(HtmlEvents.onclick, "NavXysSignin()")


        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.position, "relative")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.width, "500px")

        elmBox.AddItem(Title, 16)
        elmBox.AddItem(lbl1, 24)
        elmBox.AddItem(text, 4)
        elmBox.AddItem(text1, 40)
        elmBox.AddItem(btn, 10)

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText
    End Sub

    Public Function NavXysSignin() As ApiResponse
        Dim pwd As String = GetDataValue("pwd")
        Dim pwd1 As String = GetDataValue("pwd1")

        Dim _ApiResponse As New ApiResponse

        Dim Valid As Boolean = True
        Dim DialogMsg As String = String.Empty

        If pwd = String.Empty OrElse pwd1 = String.Empty Then
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
            If ExistsReset(data.Tid) = False Then
                Dim dialogBox As New DialogBox(Translator.Format("msg_exist"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                dialogBox.AddButton(Translator.Format("passreset"), String.Empty, "class:button;onclick:$NavigateTo('" + References.Pages.XysPassReset + "');")
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
                _ApiResponse.ExecuteScript("ShowButtons();")
            Else
                data.Pwd = pwd

                Dim MailFile As String = HtmlFolder + References.Htmls.Email_PassChanged + "_" + ClientLanguage + ".html"
                If File.Exists(MailFile) = False Then
                    MailFile = References.Htmls.Email_PassChanged
                Else
                    MailFile = References.Htmls.Email_PassChanged + "_" + ClientLanguage
                End If

                Dim rlt As String = SaveViewData(data)
                If rlt = String.Empty Then
                    Dim Subject As String = HtmlTranslator.Value("msg_email")
                    Dim bodyHtml As String = ReadHtmlFile(MailFile) _
                                             .Replace("{username}", data.Name) _
                                             .Replace("{useremail}", data.Email)
                    Dim ToAddr As String() = {data.Email}

                    Dim rltmail As String = SendEmail(Subject, bodyHtml, ToAddr)
                    If rltmail = String.Empty Then
                        _ApiResponse.Navigate(References.Pages.XysSignin)
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

    Private Function SaveViewData(_data As ViewData) As String
        Dim SQL As New List(Of String) From _
            {
            " update XysUserReset set Status = 9, Expired=getdate() where Tid = N'" + _data.Tid + "' ",
            " update XysUser set " +
            " UserPwd = N'" + Encryptor.EncryptData(_data.Pwd) + "', SYSDTE = GETDATE(), SYSUSR = N'" + _data.UserId + "' " +
            " where UserId = N'" + _data.UserId + "' "
            }

        Return PutData(SQL)
    End Function

    Private Function ExistsReset(Tid As String) As Boolean
        Dim rtnvlu As Boolean = False

        Dim SQLText As New SQLText
        SQLText.Sql = " select * from XysUserReset where Status = 0 and getdate() between Created and Expired and Tid = @Tid "
        SQLText.Params.Add(New SqlClient.SqlParameter With {.ParameterName = "@Tid", .Value = Tid, .SqlDbType = SqlDbType.NVarChar})

        Dim emsg As String = String.Empty
        Dim dt As DataTable = SQLData.SQLDataTable(SQLText.ToString, emsg)
        If emsg = String.Empty AndAlso dt IsNot Nothing AndAlso dt.Rows.Count <> 0 Then
            rtnvlu = True
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
        Public Property UserId As String = String.Empty
        Public Property Email As String = String.Empty
        Public Property Name As String = String.Empty
        Public Property Pwd As String = String.Empty
    End Class

End Class
