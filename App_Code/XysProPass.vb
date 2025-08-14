Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data
Imports System.Net
Imports System.IO

Public Class XysProPass
    Inherits WebBase


    Public Overrides Function InitialView() As String
        Dim mnulist As MenuList = SetPageMenu({})
        Dim BtnWrap As Wrap = SetPageButtons({})

        Dim label As New Label
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;")
        label.Wrap.InnerText = Translator.Format("changepwd")

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "100px")
        filter.Wrap.SetStyle(HtmlStyles.width, "95%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText


        Dim lbl1 As New Label(Translator.Format("enterpwd"))
        lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px")
        lbl1.Wrap.SetStyle(HtmlStyles.color, "#444")

        Dim text As New Texts(Translator.Format("pwd"), "pwd", TextTypes.password)
        text.Required = True
        text.Text.SetStyle(HtmlStyles.width, "180px")
        text.Text.SetAttribute(HtmlAttributes.maxlength, "15")
        text.Text.SetAttribute(HtmlAttributes.autocomplete, "off")

        Dim text1 As New Texts(Translator.Format("pwd1"), "pwd1", TextTypes.password)
        text1.Required = True
        text1.Text.SetStyle(HtmlStyles.width, "180px")
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "15")
        text1.Text.SetAttribute(HtmlAttributes.autocomplete, "off")
         
        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.width, "95%")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.marginTop, "10px")
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px")
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "40px 10px 40px 40px")

        elmBox.AddItem(lbl1, 34)
        elmBox.AddItem(text, 4)
        elmBox.AddItem(text1, 50)
        elmBox.AddItem(BtnWrap)

        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText
        Return ViewHtml
    End Function

    Public Function PassSave() As ApiResponse
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
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
            _ApiResponse.ExecuteScript("XysProPassShowButtons();")
        Else
            Dim MailFile As String = HtmlFolder + References.Htmls.Email_PassChanged + "_" + ClientLanguage + ".html"
            If File.Exists(MailFile) = False Then
                MailFile = References.Htmls.Email_PassChanged
            Else
                MailFile = References.Htmls.Email_PassChanged + "_" + ClientLanguage
            End If

            Dim rlt As String = SaveData(pwd)
            If rlt = String.Empty Then
                Dim Subject As String = HtmlTranslator.Value("msg_email")
                Dim bodyHtml As String = ReadHtmlFile(MailFile) _
                                         .Replace("{username}", AppKey.UserName) _
                                         .Replace("{useremail}", AppKey.UserEmail)
                Dim ToAddr As String() = {AppKey.UserEmail}

                Dim rltmail As String = SendEmail(Subject, bodyHtml, ToAddr)
                If rltmail = String.Empty Then
                    _ApiResponse.Navigate(References.Pages.XysSignin)
                Else
                    Dim dialogBox As New DialogBox(rltmail)
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
                    _ApiResponse.ExecuteScript("XysProPassShowButtons();")
                End If

            Else
                Dim dialogBox As New DialogBox(rlt)
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
                _ApiResponse.ExecuteScript("XysProPassShowButtons();")
            End If
        End If

        Return _ApiResponse
    End Function

    Private Function SaveData(UserPwd As String) As String
        Dim SQL As New List(Of String) From _
            {
            " update XysUser set UserPwd=@UserPwd, SYSDTE = getdate(),SYSUSR=@UserId where UserId = @UserId "
            }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserId", .Value = AppKey.UserId, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserPwd", .Value = Encryptor.EncryptData(UserPwd), .SqlDbType = SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

End Class
