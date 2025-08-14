Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data
Imports System.Net

Public Class XysCloseAcct
    Inherits WebBase

    Public Overrides Function InitialView() As String
        Dim mnulist As MenuList = SetPageMenu({})
        Dim BtnWrap As Wrap = SetPageButtons({})

        Dim label As New Label
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;")
        label.Wrap.InnerText = Translator.Format("cancel")

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "100px")
        filter.Wrap.SetStyle(HtmlStyles.width, "95%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText
         
        Dim label1 As New Label
        label1.Wrap.SetStyles("font-size:18px;")
        label1.Wrap.InnerText = Translator.Format("losedata")

        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.width, "95%")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.marginTop, "10px")
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px")
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "40px 10px 40px 40px")

        elmBox.AddItem(label1, 50)
        elmBox.AddItem(BtnWrap)

        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText
        Return ViewHtml
    End Function

    Public Function CancelView() As ApiResponse
        Dim t As String = GetDataValue("t")

        Dim _ApiResponse As New ApiResponse
        Dim dialogBox As New DialogBox(Translator.Format("closeaccount"))
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;")
        dialogBox.AddButton(Translator.Format("yes"), String.Empty, "class:button1;onclick:" + ByPassCall("XysCloseAcct/CancelViewConfirm"))
        dialogBox.AddButton(Translator.Format("no"), String.Empty, "onclick:$PopOff();class:button;")
        _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Return _ApiResponse
    End Function
    Public Function CancelViewConfirm() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        Dim rlt As String = PutCancelViewData()
        If rlt = String.Empty Then
            _ApiResponse.ExecuteScript("SignOut()")
        Else
            Dim dialogBox As New DialogBox(rlt)
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        End If
        Return _ApiResponse
    End Function

    Private Function PutCancelViewData() As String
        Dim SQL As New List(Of String) From _
            {
            " delete from Membership where UserId = @UserId ",
            " delete from XysUser where UserId = @UserId ",
            " delete from XysUserInfo where UserId = @UserId ",
            " delete from XysUserReset where UserId = @UserId "
            }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserId", .Value = AppKey.UserId, .SqlDbType = SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function


End Class
