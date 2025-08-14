Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit

Public Class XysAuth
    Inherits WebBase

    Public Overrides Sub OnBeforRender()
        HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox)
    End Sub

    Public Overrides Function InitialView() As String

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
        btn.SetAttribute(HtmlEvents.onclick, "NavXysHome()")

        Dim btn1 As New Button(Translator.Format("back"), Button.ButtonTypes.Button)
        btn1.SetStyle(HtmlStyles.marginLeft, "12px")
        btn1.SetAttribute(HtmlAttributes.class, "button1")
        btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignIn()")


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

        Return elmBox.HtmlText
    End Function

    Public Function NavXysSignIn() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.Navigate(References.Pages.XysSignin)
        Return _ApiResponse
    End Function
    Public Function NavXysHome() As ApiResponse
        Dim pin As String = GetDataValue("pin")

        Dim _ApiResponse As New ApiResponse

        If pin <> String.Empty Then
            _ApiResponse.Navigate(References.Pages.XysHome)
        Else
            Dim dialogBox As New DialogBox(Translator.Format("enterpin"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText)
        End If
        Return _ApiResponse
    End Function
End Class
