Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data
Imports System.Net
Imports System.IO

Public Class XysHome
    Inherits WebBase

    Public Overrides Function InitialModel() As ViewPart

        ViewPart = New ViewPart
        ViewPart.Methods = ViewMethods()
        ViewPart.Mode = ViewMode.View

        ViewPart.UIControl = New UIControl
        ViewPart.UIControl.Set({ _
            New UIControl.Item() With {.Name = "BltnTitle", .Label = "", .Styles = "width:500px;border:none;border-bottom:1px solid #333;border-radius:0px;", .IsReadOnly = True, .LineSpacing = 1}, _
            New UIControl.Item() With {.Name = "BltnMemo", .Label = "", .Styles = "font-size:12px;width:500px; height:180px;border:none; ", .UIType = UITypes.TextArea, .IsReadOnly = True, .LineSpacing = 1}, _
            New UIControl.Item() With {.Name = "CreatedBy", .Label = "", .Styles = "width:260px;border:none;border-bottom:1px solid #333;border-radius:0px;", .IsReadOnly = True, .LineSpacing = 1}, _
            New UIControl.Item() With {.Name = "SYSDTE", .Label = "", .Styles = "border:none; font-size:12px;width:160px;", .IsReadOnly = True, .LineSpacing = 1}, _
            New UIControl.Item() With {.Name = "FileRefId", .Label = "Attach File(s)", .Styles = "margin-left:14px", .UIType = UITypes.File, .IsVisible = False} _
         })

        Return ViewPart
    End Function

    Public Overrides Sub OnBeforRender()
        If AppKey Is Nothing Then
            HtmlDoc.InitialScripts.Navigate(References.Pages.XysSignin)
        End If
    End Sub

    Public Overrides Function InitialView() As String

        Dim mnulist As MenuList = SetPageMenu({})

        Dim fLabel As New Label
        fLabel.Wrap.InnerText = Translator.Format("hi") + AppKey.UserName
        fLabel.Wrap.SetStyle(HtmlStyles.fontSize, "22px")
        fLabel.Wrap.SetStyle(HtmlStyles.fontWeight, "bold")
        fLabel.Wrap.SetStyle(HtmlStyles.margin, "14px 0px 0px 14px")

        Dim filter As New ToolKit.FilterSection()
        filter.FilterHtml = fLabel.HtmlText
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.margin, "8px 2% 8px 2%")
        filter.Menu = mnulist
         
        Dim Image As New ImageBox
        Image.Image.SetAttribute(HtmlAttributes.src, ImagePath + "img_home.gif")

        Dim mmnu As New MultiMenuSection
        mmnu.Wrap.SetStyle(HtmlStyles.margin, "16px 2% 8px 2%")
        mmnu.Wrap.SetStyle(HtmlStyles.marginBottom, "80px")

        Dim BltnDt As DataTable = SQLData.SQLDataTable( _
                  " SELECT top 15 BltnId, " +
                  "        BltnTitle, " +
                  "        ' (' + replace(substring(convert(varchar(10),SYSDTE,121),6,5),'-','/') + ')' BltnDte " +
                  " FROM XysBulletin ORDER BY SYSDTE DESC ")

        If BltnDt IsNot Nothing AndAlso BltnDt.Rows.Count > 0 Then
            If BltnDt IsNot Nothing Then
                Dim mnus As New MenuList
                mnus.Title.InnerText = Translator.Format("bulletin")
                mnus.Wrap.SetStyles(" border:1px solid #ff6600;")
                For i As Integer = 0 To BltnDt.Rows.Count - 1
                    Dim bltnId As String = BltnDt.Rows(i)(0).ToString
                    Dim bltnTitle As String = BltnDt.Rows(i)(1).ToString
                    Dim bltnDte As String = BltnDt.Rows(i)(2).ToString

                    bltnTitle = If(Len(bltnTitle) > 24, bltnTitle.Substring(0, 24) + " ....", bltnTitle)

                    Dim mnuitem As New HtmlTag
                    mnuitem.SetStyles("font-size:14px;")
                    mnuitem.InnerText = bltnTitle + bltnDte
                    mnuitem.SetAttribute(HtmlEvents.onclick, ByPassCall("ShowBulletin", "t=" + bltnId))
                    mnus.Add(mnuitem)
                Next
                mmnu.Menus.Add(mnus)
            End If
        End If

        Dim PageGroups As List(Of String) = GetPageGroups()
        If PageGroups.Count > 0 Then
            Dim MenuablePages As List(Of MenuablePage) = GetMeuablePages(PageGroups)
            If MenuablePages IsNot Nothing Then

                For i As Integer = 0 To PageGroups.Count - 1
                    Dim _pagegroup As String = PageGroups(i)
                    Dim _MenuablePages As List(Of MenuablePage) = MenuablePages.FindAll(Function(x) x.PageGroup = _pagegroup)

                    If _MenuablePages.Count > 0 Then
                        Dim mnus As New MenuList
                        mnus.Title.InnerText = _pagegroup

                        For j As Integer = 0 To _MenuablePages.Count - 1
                            Dim mnuAttr As String = HtmlAttributes.title + ":" + _MenuablePages(j).PageDesc + ";" + _
                                                 HtmlEvents.onclick + ":" + ByPassCall("NaviClick", "m=" + _MenuablePages(j).PageName)
                            mnus.Add(_MenuablePages(j).PageDesc, String.Empty, mnuAttr)
                        Next
                        mmnu.Menus.Add(mnus)
                    End If
                Next
            End If
        End If

        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript)
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript)
        HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')")

        Dim PageLayout As TitleSection2 = PageTitle()
        PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents)
        PageLayout.ContentWrap.InnerText = Image.HtmlText + filter.HtmlText + mmnu.HtmlText

        Return PageLayout.HtmlText
    End Function
   
    Private Function GetMeuablePages(PageGroups As List(Of String)) As List(Of MenuablePage)
        Dim rlt As List(Of MenuablePage) = Nothing
        Dim JoinPageGroups As String = "(N'" + String.Join("',N'", PageGroups) + "')"
        Dim SSQL As String = " select PageId,PageName,PageDesc,PageGroup from XysPage " +
                             " where PageMenu = 1 and PageUse = 1 and PageGroup in " + JoinPageGroups + " " +
                             " order by PageOrder "

        Dim emsg As String = String.Empty
        Dim dt As DataTable = SQLData.SQLDataTable(SSQL, emsg)
        If dt IsNot Nothing AndAlso dt.Rows.Count <> 0 Then
            rlt = DataTableListT(Of MenuablePage)(dt)
        End If
        Return rlt
    End Function
    Private Function GetPageGroups() As List(Of String)
        Dim rlt As New List(Of String)
        Dim SSQL As String = " declare @roleid nvarchar(50) " +
                             " set @roleid = N'" + AppKey.RoleId + "' " +
                             " select PageGroup from XysPage " +
                             " where PageUse = 1 and PageGroup <> '' and PageId in (select PageId from XysRolePage where RoleId = @roleid ) " +
                             " order by PageOrder "

        Dim emsg As String = String.Empty
        Dim dt As DataTable = SQLData.SQLDataTable(SSQL, emsg)
        If dt IsNot Nothing AndAlso dt.Rows.Count <> 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim pagegroup As String = dt.Rows(i)(0).ToString
                Dim grp As String = rlt.Find(Function(x) x = pagegroup)
                If grp Is Nothing Then
                    rlt.Add(pagegroup)
                End If
            Next
        End If
        Return rlt
    End Function

    Public Function NaviClick() As ApiResponse
        Dim m As String = GetDataValue("m")
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.Navigate(m)
        Return _ApiResponse
    End Function

    Public Function Profile() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.Navigate(References.Pages.XysProfile)
        Return _ApiResponse
    End Function

    Public Function SignOut() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.RemoveCookie(References.Keys.AppKey)
        _ApiResponse.ClearStorage()
        _ApiResponse.Navigate(References.Pages.XysSignin)
        Return _ApiResponse
    End Function

    Public Function ShowBulletin() As ApiResponse
        Dim t As String = GetDataValue("t")

        ViewPart.UIControl.Data = GetBltn(t)
        ViewPart.UIControl.UIMode = UIControl.Modes.View
        ViewPart.UIControl.Wrap.SetStyle(HtmlStyles.padding, "10px")
        Dim BltnContents As String = ViewPart.UIControl.HtmlText

        Dim dialogBox As New DialogBox(BltnContents)
        dialogBox.ContentsWrap.SetStyles("width:600px;")

        Dim _ApiResponse As New ApiResponse
        _ApiResponse.PopUpWindow(dialogBox.HtmlText)
        Return _ApiResponse
    End Function

    Public Function FileDownLoad() As ApiResponse
        Return FileDownLoadEnc()
    End Function
    Public Function GetBltn(BltnId As String) As DataTable
        Dim emsg As String = String.Empty
        Dim SSQL As String = String.Empty
        SSQL = " declare @BltnId decimal(18) " +
                " set @BltnId = " + Val(BltnId).ToString() + "  " +
                " select BltnId, BltnTitle, BltnMemo, CreatedBy,'@E #|' + dbo.XF_FileDownListEnc(FileRefId,'#') FileRefId, SYSDTE, SYSUSR " +
                " from XysBulletin " +
                " where  BltnId = @BltnId    "
        Dim dt As DataTable = SQLData.SQLDataTable(SSQL, emsg)
        Return dt
    End Function

    Private Class MenuablePage
        Public PageId As String = String.Empty
        Public PageName As String = String.Empty
        Public PageDesc As String = String.Empty
        Public PageGroup As String = String.Empty
    End Class
End Class
