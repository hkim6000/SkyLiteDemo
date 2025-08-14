Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class Support
    Inherits WebBase

    Public Overrides Function InitialView() As String
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript)
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript)
        HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')")

        Dim PageLayout As TitleSection2 = PageTitle()
        PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents)
        PageLayout.ContentWrap.InnerText = PartialPage(References.Pages.SupportMV)

        Return PageLayout.HtmlText
    End Function

    Public Function MenuClick() As ApiResponse
        Dim m As String = GetDataValue("m")
        Dim t As String = GetDataValue("t")

        Dim _ApiResponse As New ApiResponse
        _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(m, t))
        _ApiResponse.ExecuteScript("$ScrollToTop()")
        Return _ApiResponse
    End Function

    Public Function NaviClick() As ApiResponse
        Dim m As String = GetDataValue("m")
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.Navigate(m)
        Return _ApiResponse
    End Function

End Class
