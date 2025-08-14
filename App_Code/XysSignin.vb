Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysSignin
    Inherits WebPage

    Private AppKey As AppKey = Nothing
    Sub New()
        'HtmlDoc.PreLoad = True
        'HtmlTranslator.Add("hi", "Hi, SKY Member2")
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

        Dim Title As New Label(Translator.Format("hi"))
        Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px")
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px")
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px")
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0")

        Dim text As New Texts(Translator.Format("email"), "email", TextTypes.text)
        text.Required = True
        text.Text.SetAttribute(HtmlAttributes.placeholder, Translator.Format("placeholder"))
        text.Text.SetAttribute(HtmlAttributes.value, "hkim@email.com") 'for demo
        text.Text.SetStyle(HtmlStyles.width, "330px")


        Dim btn As New Button(Translator.Format("next"), Button.ButtonTypes.Button)
        btn.SetAttribute(HtmlAttributes.class, "button")
        btn.SetAttribute(HtmlEvents.onclick, ByPassCall("NavXysPass", "email=::"))

        Dim lbl1 As New Label(Translator.Format("signup"))
        lbl1.Wrap.SetStyle(HtmlStyles.position, "absolute")
        lbl1.Wrap.SetStyle(HtmlStyles.top, "26px")
        lbl1.Wrap.SetStyle(HtmlStyles.right, "34px")
        lbl1.Wrap.SetStyle(HtmlStyles.fontSize, "14px")
        lbl1.Wrap.SetStyle(HtmlStyles.textDecoration, "underline")
        lbl1.Wrap.SetStyle(HtmlStyles.color, "#ff6600")
        lbl1.Wrap.SetStyle(HtmlStyles.cursor, "pointer")
        lbl1.Wrap.SetAttribute(HtmlEvents.onclick, ByPassCall("NavXysSignup"))

        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.position, "relative")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.width, "500px")

        elmBox.AddItem(Title, 40)
        elmBox.AddItem(text, 40)
        elmBox.AddItem(btn, 10)
        elmBox.AddItem(lbl1)

        HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText

        HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox)
        HtmlDoc.InitialScripts.RemoveCookie(References.Keys.AppKey)
    End Sub

    Public Function NavXysPass() As ApiResponse
        Dim email As String = GetDataValue("email")

        Dim _ApiResponse As New ApiResponse

        If email <> String.Empty Then
            If ExistUser(email) = False Then
                Dim dialogBox As New DialogBox(Translator.Format("msg_email"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                _ApiResponse.PopUpWindow(dialogBox.HtmlText)
            Else
                Dim SerializedString As String = SerializeObjectEnc(AppKey, GetType(AppKey))
                _ApiResponse.SetCookie(References.Keys.AppKey, SerializedString)
                _ApiResponse.Navigate(References.Pages.XysPass)
            End If
        Else
            Dim dialogBox As New DialogBox(Translator.Format("placeholder"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText)
        End If
        Return _ApiResponse
    End Function

    Public Function NavXysSignup() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.Navigate(References.Pages.XysSignup)
        Return _ApiResponse
    End Function

    Private Function ExistUser(UserEmail As String) As Boolean
        Dim rtnvlu As Boolean = False

        Dim SSQL As String = _
            " select a.UserId,UserName,UserEmail,UserPhone,UserPic,RoleId,UserRef " +
            " from XysUser a inner join XysUserInfo b on a.UserId = b.UserId " +
            " where b.UserEmail = @UserEmail "

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserEmail", .Value = UserEmail, .SqlDbType = SqlDbType.NVarChar})

        Dim emsg As String = String.Empty
        Dim dt As DataTable = SQLData.SQLDataTable(SqlWithParams(SSQL, SqlParams), emsg)
        If emsg = String.Empty AndAlso dt IsNot Nothing AndAlso dt.Rows.Count <> 0 Then
            AppKey = New AppKey
            AppKey.UserId = dt.Rows(0)(0).ToString
            AppKey.UserName = dt.Rows(0)(1).ToString
            AppKey.UserEmail = dt.Rows(0)(2).ToString
            AppKey.UserPhone = dt.Rows(0)(3).ToString
            AppKey.RoleId = dt.Rows(0)(4).ToString
            AppKey.RoleId = dt.Rows(0)(5).ToString
            AppKey.UserRef = dt.Rows(0)(6).ToString
            AppKey.DateTime = DateTime.Now

            rtnvlu = True
        End If
        Return rtnvlu
    End Function
End Class
