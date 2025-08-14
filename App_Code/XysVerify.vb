Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysVerify
    Inherits WebPage

    Private _SignUp As ViewData = Nothing
    Private _SignUpAlive As Boolean = False
    Private AppKey As AppKey = Nothing

    Sub New()
        HtmlTranslator.Add(GetPageDict(Me.GetType.ToString))

        Try
            Dim paramVlu As String = ParamValue(References.Keys.SignUp_User, True)
            _SignUp = DeserializeObjectEnc(paramVlu, GetType(ViewData))
            _SignUpAlive = ValidData(_SignUp.Tid)
        Catch ex As Exception
            _SignUp = Nothing
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
    Public Overrides Sub OnInitialized()
        HtmlDoc.AddJsFile("WebScript.js")
        HtmlDoc.AddCSSFile("WebStyle.css")
        HtmlDoc.SetTitle(Translator.Format("title"))

        Dim Title As New Label(Translator.Format("verify"))
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px")
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px")
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px")
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0")

        Dim text As New Texts(Translator.Format("pinno"), "pin", TextTypes.text)
        text.Text.SetStyle(HtmlStyles.width, "330px")
        text.Text.SetAttribute(HtmlAttributes.maxlength, "5")

        Dim lbl1 As New Label(Translator.Format("sentemail"))
        lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px")
        lbl1.Wrap.SetStyle(HtmlStyles.color, "#444")

        Dim btn As New Button(Translator.Format("next"), Button.ButtonTypes.Button)
        btn.SetAttribute(HtmlAttributes.class, "button")
        btn.SetAttribute(HtmlEvents.onclick, "Submit()")

        Dim btn1 As New Button(Translator.Format("back"), Button.ButtonTypes.Button)
        btn1.SetStyle(HtmlStyles.marginLeft, "12px")
        btn1.SetAttribute(HtmlAttributes.class, "button1")
        btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignin()")


        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.position, "relative")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.width, "500px")

        elmBox.AddItem(Title, 14)
        elmBox.AddItem(lbl1, 30)
        elmBox.AddItem(text, 40)
        elmBox.AddItem(btn)
        elmBox.AddItem(btn1, 10)

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText
    End Sub
    Public Overrides Sub OnBeforRender()

        If _SignUp Is Nothing Then
            HtmlDoc.InitialScripts.Navigate(References.Pages.XysSignin)
        Else
            If _SignUpAlive = True Then
                HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox)
            Else
                HtmlDoc.InitialScripts.Navigate(References.Pages.XysSignupExpired)
            End If
        End If

    End Sub
     
    Public Function NavXysSignin() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.Navigate(References.Pages.XysSignin)
        Return _ApiResponse
    End Function
    Public Function Submit() As ApiResponse
        Dim pin As String = GetDataValue("pin")

        Dim _ApiResponse As New ApiResponse

        If pin <> String.Empty Then
            If pin <> _SignUp.OTP Then
                Dim dialogBox As New DialogBox(Translator.Format("msg_diff"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
            Else
                Dim rlt As String = SaveViewData(_SignUp)
                If rlt = String.Empty Then
                    Dim dialogBox As New DialogBox(Translator.Format("msg_signin"))
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                    dialogBox.AddButton(Translator.Format("signin"), String.Empty, "class:button;onclick:$NavigateTo('" + References.Pages.XysSignin + "');")
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
                Else
                    Dim dialogBox As New DialogBox(Translator.Format(rlt))
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
                End If
            End If
        Else
            Dim dialogBox As New DialogBox(Translator.Format("msg_required"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
        End If
        Return _ApiResponse
    End Function

    Private Function SaveViewData(_data As ViewData) As String

        Dim SQL As New List(Of String) From _
            {
            " If not exists(select * from XysUserInfo where UserEmail = @UserEmail) " +
            " begin " +
            " Insert into XysUser( UserId,UserPwd,UserOTP,UserStatus,PassChanged,Created,Closed,SYSDTE,SYSUSR) " +
            " values ( @UserId, @UserPwd, @UserOTP, @UserStatus, @PassChanged, @Created, @Closed, @SYSDTE, @SYSUSR) " +
            " Insert into XysUserInfo( UserId,UserName,UserDesc,UserEmail,UserPhone,UserPic,RoleId,UserRef) " +
            " values ( @UserId, @UserName, @UserDesc, @UserEmail, @UserPhone, @UserPic, @RoleId, @UserRef) " +
            " end " +
            " else " +
            " begin " +
            " RAISERROR('msg_exist',16,1) " +
            " end"
            }

        Dim UserId As String = NewID()

        Dim SQLParams As New List(Of SqlClient.SqlParameter)
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserId", .Value = UserId, .SqlDbType = data.SqlDbType.NVarChar})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserPwd", .Value = _data.Pass, .SqlDbType = Data.SqlDbType.NVarChar})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserOTP", .Value = "0", .SqlDbType = data.SqlDbType.Int})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserStatus", .Value = "0", .SqlDbType = data.SqlDbType.Int})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@PassChanged", .Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), .SqlDbType = data.SqlDbType.DateTime})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@Created", .Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), .SqlDbType = data.SqlDbType.DateTime})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@Closed", .Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), .SqlDbType = data.SqlDbType.DateTime})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SYSDTE", .Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), .SqlDbType = data.SqlDbType.DateTime})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SYSUSR", .Value = UserId, .SqlDbType = data.SqlDbType.NVarChar})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserName", .Value = _data.Name, .SqlDbType = Data.SqlDbType.NVarChar})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserDesc", .Value = String.Empty, .SqlDbType = data.SqlDbType.NVarChar})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserEmail", .Value = _data.Email, .SqlDbType = Data.SqlDbType.NVarChar})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserPhone", .Value = String.Empty, .SqlDbType = data.SqlDbType.NVarChar})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserPic", .Value = String.Empty, .SqlDbType = data.SqlDbType.NVarChar})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@RoleId", .Value = String.Empty, .SqlDbType = data.SqlDbType.NVarChar})
        SQLParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserRef", .Value = String.Empty, .SqlDbType = data.SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SQLParams))
    End Function
    Private Function ValidData(Tid As String) As Boolean
        Dim rtnvlu As Boolean = False

        Dim SSQL As String = "select Tid from XysSignUp  where getdate() between Created and Expired and Tid = @Tid"

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@Tid", .Value = Tid, .SqlDbType = SqlDbType.NVarChar})
         
        Dim emsg As String = String.Empty
        Dim dt As DataTable = SQLData.SQLDataTable(SqlWithParams(SSQL, SqlParams), emsg)
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
