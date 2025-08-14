Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysPass
    Inherits WebPage

    Private AppKey As AppKey = Nothing

    Sub New()
        Dim paramVlu As String = ParamValue(References.Keys.AppKey, True)
        AppKey = DeserializeObjectEnc(paramVlu, GetType(AppKey))

        If AppKey Is Nothing Then
            HtmlDoc.InitialScripts.Navigate(References.Pages.XysSignin)
        Else
            HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox)
        End If
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

        Dim Title As New Label(Translator.Format("userpwd"))
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px")
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px")
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px")
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0")

        Dim text As New Texts(Translator.Format("pass"), "pass", TextTypes.password)
        text.Required = True
        text.Text.SetAttribute(HtmlAttributes.placeholder, Translator.Format("placeholder"))
        text.Text.SetStyle(HtmlStyles.width, "330px")
        text.Text.SetAttribute(HtmlAttributes.value, "12345") 'for demo

        Dim btn As New Button(Translator.Format("next"), Button.ButtonTypes.Button)
        btn.SetAttribute(HtmlAttributes.class, "button")
        btn.SetAttribute(HtmlEvents.onclick, "NavXysHome()")
        btn.IDTag = "C101"

        Dim btn1 As New Button(Translator.Format("back"), Button.ButtonTypes.Button)
        btn1.SetStyle(HtmlStyles.marginLeft, "12px")
        btn1.SetAttribute(HtmlAttributes.class, "button1")
        btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignIn()")
        btn1.IDTag = "C102"

        Dim lbl2 As New Label(Translator.Format("forgotpwd"))
        lbl2.Wrap.SetStyle(HtmlStyles.textAlign, "right")
        lbl2.Wrap.SetStyle(HtmlStyles.fontSize, "12px")
        lbl2.Wrap.SetStyle(HtmlStyles.paddingRight, "40px")
        lbl2.Wrap.SetStyle(HtmlStyles.color, "#ff6600")
        lbl2.Wrap.SetStyle(HtmlStyles.cursor, "pointer")
        lbl2.Wrap.SetStyle(HtmlStyles.fontStyle, "italic")
        lbl2.Wrap.SetStyle(HtmlStyles.textDecoration, "underline")
        lbl2.Wrap.SetAttribute(HtmlEvents.onclick, "NavXysPassReset()")
        lbl2.IDTag = "T101"

        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.position, "relative")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.width, "500px")

        elmBox.AddItem(Title, 40)
        elmBox.AddItem(text)
        elmBox.AddItem(lbl2, 30)
        elmBox.AddItem(btn)
        elmBox.AddItem(btn1, 10)

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText
    End Sub

    Public Function NavXysPassReset() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.Navigate(References.Pages.XysPassReset)
        Return _ApiResponse
    End Function
    Public Function NavXysSignIn() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.Navigate(References.Pages.XysSignin)
        Return _ApiResponse
    End Function
    Public Function NavXysHome() As ApiResponse
        Dim pass As String = GetDataValue("pass")

        Dim _ApiResponse As New ApiResponse

        If pass <> String.Empty Then
            If ExistUser(AppKey.UserEmail, pass) = True Then
                _ApiResponse.Navigate(References.Pages.XysHome)
            Else
                Dim dialogBox As New DialogBox(Translator.Format("msg_wrongcred"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
            End If
        Else
            Dim dialogBox As New DialogBox(Translator.Format("placeholder"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox)
        End If
        Return _ApiResponse
    End Function

    Private Function ExistUser(UserEmail As String, UserPwd As String) As Boolean
        Dim rtnvlu As Boolean = False

        Dim SSQL As String = " select count(*) from XysUser a inner join XysUserInfo b on a.UserId = b.UserId " +
                             " where b.UserEmail = @UserEmail and a.UserPwd = @UserPwd "

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserEmail", .Value = UserEmail, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserPwd", .Value = Encryptor.EncryptData(UserPwd), .SqlDbType = SqlDbType.NVarChar})

        Dim emsg As String = String.Empty
        Dim dt As DataTable = SQLData.SQLDataTable(SqlWithParams(SSQL, SqlParams), emsg)
        If emsg = String.Empty AndAlso dt IsNot Nothing AndAlso dt.Rows.Count <> 0 Then
            If Val(dt.Rows(0)(0).ToString) <> 0 Then
                rtnvlu = True
            End If
        End If
        Return rtnvlu
    End Function
End Class
