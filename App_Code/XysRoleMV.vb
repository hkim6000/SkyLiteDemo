Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data
Imports System.Runtime.Serialization.Json
Imports System.Net

Public Class XysRoleMV
    Inherits WebBase
     
    Public Overrides Function InitialModel() As ViewPart
        ViewPart = New ViewPart
        ViewPart.Methods = ViewMethods("XysRole")
        ViewPart.Mode = ViewMode.View
        ViewPart.Data = SQLData.SQLDataTable(" Select RoleId,RoleName,RoleAlias,RoleOrder From XysRole order by RoleOrder,RoleName")
        Return ViewPart
    End Function

    Public Overrides Function InitialView() As String
        Dim mnulist As MenuList = SetPageMenu({})
        Dim BtnWrap As Wrap = SetPageButtons(If(ViewPart.Mode = ViewMode.New, {"save"}, {}))

        Dim label As New Label
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;")
        label.Wrap.InnerText = If(ViewPart.Mode = ViewMode.New, Translator.Format("role"), Translator.Format("role"))

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px")
        filter.Wrap.SetStyle(HtmlStyles.width, "90%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText
         

        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.width, "90%")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.marginTop, "8px")
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "30px 30px 30px 30px")

        Dim data As List(Of ViewModel) = DataTableListT(Of ViewModel)(ViewPart.Data)
        If ViewPart.Data IsNot Nothing Then
            For i As Integer = 0 To data.Count - 1
                Dim elm As New HtmlTag(HtmlTags.img, HtmlTag.Types.Empty)
                elm.SetAttribute(HtmlAttributes.title, data(i).RoleName)
                elm.SetAttribute(HtmlAttributes.src, ImagePath + "role.jpg")
                elm.SetStyle(HtmlStyles.width, "60px")

                Dim elm1 As New HtmlTag()
                elm1.SetStyle(HtmlStyles.padding, "6px")
                elm1.InnerText = data(i).RoleName + "<br>(" + data(i).RoleAlias + ")"

                Dim itmPnl As New ItemPanel
                itmPnl.Wrap.SetAttribute(HtmlEvents.onclick, ByPassCall("MenuClick", "m=" + References.Pages.XysRoleEV + "&t=" + data(i).RoleId))
                itmPnl.Wrap.SetAttribute(HtmlAttributes.id, data(i).RoleId)
                itmPnl.Wrap.SetAttribute(HtmlAttributes.class, "itmPnl")
                itmPnl.Wrap.SetStyle(HtmlStyles.boxShadow, "3px 4px 6px 1px rgba(0, 0, 0, 0.15)")
                itmPnl.Wrap.SetStyle(HtmlStyles.minWidth, "100px")
                itmPnl.AddElement(elm, HorizontalAligns.Center)
                itmPnl.AddElement(elm1, HorizontalAligns.Center)
                elmBox.AddItem(itmPnl)
            Next
        End If

        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText
        
        Return ViewHtml
    End Function

    Private Class ViewModel
        Public Property RoleId As String = String.Empty
        Public Property RoleName As String = String.Empty
        Public Property RoleAlias As String = String.Empty
        Public Property RoleOrder As String = String.Empty
    End Class
End Class
