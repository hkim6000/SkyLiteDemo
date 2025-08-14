Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysSettings
    Inherits WebBase


    Public Overrides Function InitialView() As String
        Dim ViewHtml As String = String.Empty

        Dim Title As New Label
        Title.Wrap.InnerText = Translator.Format("settings")
        Title.Wrap.SetStyle(HtmlStyles.fontSize, "28px")
        Title.Wrap.SetStyle(HtmlStyles.fontWeight, "bold")
        Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0")
        Title.Wrap.SetStyle(HtmlStyles.marginLeft, "10px")

        Dim mnu As New Label
        mnu.Wrap.InnerText = "&#149;&nbsp;" + Translator.Format("setroles")
        mnu.Wrap.SetAttribute(HtmlEvents.onclick, CallActionEnc("MenuClick", "m=" + References.Pages.XysRole))
        mnu.Wrap.SetAttribute(HtmlAttributes.class, "mnulabel")
        mnu.IDTag = "T100"

        Dim mnu1 As New Label
        mnu1.Wrap.InnerText = "&#149;&nbsp;" + Translator.Format("setaccounts")
        mnu1.Wrap.SetAttribute(HtmlEvents.onclick, CallActionEnc("MenuClick", "m=" + References.Pages.XysUser))
        mnu1.Wrap.SetAttribute(HtmlAttributes.class, "mnulabel")
        mnu1.IDTag = "T110"

        Dim mnu2 As New Label
        mnu2.Wrap.InnerText = "&#149;&nbsp;" + Translator.Format("setpages")
        mnu2.Wrap.SetAttribute(HtmlEvents.onclick, CallActionEnc("MenuClick", "m=" + References.Pages.XysPage))
        mnu2.Wrap.SetAttribute(HtmlAttributes.class, "mnulabel")
        mnu2.IDTag = "T120"

        Dim mnu3 As New Label
        mnu3.Wrap.InnerText = "&#149;&nbsp;" + Translator.Format("setmenu")
        mnu3.Wrap.SetAttribute(HtmlEvents.onclick, CallActionEnc("MenuClick", "m=" + References.Pages.XysMenu))
        mnu3.Wrap.SetAttribute(HtmlAttributes.class, "mnulabel")
        mnu3.IDTag = "T130"

        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.ClearStyles()
        elmBox.Wrap.SetStyle(HtmlStyles.marginLeft, "50px")
        elmBox.Wrap.SetStyle(HtmlStyles.marginTop, "60px")

        elmBox.AddItem(Title, 50)

        'If IsMethodTag(mnu.IDTag) = True Then elmBox.AddItem(mnu, 28)
        'If IsMethodTag(mnu1.IDTag) = True Then elmBox.AddItem(mnu1, 28)
        'If IsMethodTag(mnu2.IDTag) = True Then elmBox.AddItem(mnu2, 28)
        'If IsMethodTag(mnu3.IDTag) = True Then elmBox.AddItem(mnu3, 28)

        ViewHtml = elmBox.HtmlText
        Return ViewHtml
    End Function

End Class
