Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysReport
    Inherits WebBase

    Private rpts As New OptionValues
    Sub New()
        rpts.AddItem(Translator.Format("select"), "00000")

        Dim data As List(Of NameValueGroup) = GetNameValueGrp()
        If data IsNot Nothing AndAlso data.Count > 0 Then
            For i As Integer = 0 To data.Count - 1
                rpts.AddItem(data(i).name, data(i).value, data(i).group)
            Next
        End If
    End Sub
    Private Function GetNameValueGrp() As List(Of NameValueGroup)
        Dim SSQL As String = " select MenuDesc name,MenuTag value, MenuMethod  as [group] " +
                               " from XysMenu where MenuArea = N'X' and  " +
                               "PageId = (select PageId from XysPage where PageName = N'XysReport') order by MenuTag "
        Dim data As List(Of NameValueGroup) = DataTableListT(Of NameValueGroup)(SQLData.SQLDataTable(SSQL))
        Return data
    End Function

    Public Overrides Function InitialView() As String
        Dim mnulist As MenuList = SetPageMenu(If(ViewPart.Mode = ViewMode.New, {"xysrole"}, {}))
        Dim BtnWrap As Wrap = SetPageButtons(If(ViewPart.Mode = ViewMode.New, {"save"}, {}))

        Dim label As New Label
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;")
        label.Wrap.InnerText = Translator.Format("report")

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "100px")
        filter.Wrap.SetStyle(HtmlStyles.width, "95%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText

        Dim filterDD As New Dropdown
        'filterDD.SelOptions = rpts
        filterDD.Wrap.SetStyle(HtmlStyles.margin, "2px")
        filterDD.Wrap.SetStyle(HtmlStyles.marginTop, "10px")
        filterDD.SelBox.SetStyle(HtmlStyles.fontSize, "20px")
        filterDD.SelBox.SetStyle(HtmlStyles.borderColor, "#333")
        filterDD.SelBox.SetAttribute(HtmlAttributes.id, "rpt")
        filterDD.SelBox.SetAttribute(HtmlEvents.onchange, "CallReport(this)")
        filterDD.SelBox.InnerText = rpts.HtmlText

        Dim FilterSection As New FilterSection
        FilterSection.Wrap.SetStyle(HtmlStyles.marginTop, "10px")
        FilterSection.Wrap.SetStyle(HtmlStyles.width, "100%")
        FilterSection.FilterWrap.SetStyle(HtmlStyles.padding, String.Empty)
        FilterSection.FilterHtml = filterDD.HtmlText

        Dim Wrap As New Wrap
        Wrap.SetAttribute(HtmlAttributes.id, "RptBox")
        Wrap.SetStyle(HtmlStyles.boxSizing, "border-box")
        Wrap.SetStyle(HtmlStyles.padding, "8px")
        Wrap.SetStyle(HtmlStyles.width, "100%")
        Wrap.SetStyle(HtmlStyles.border, "1px solid #aaa")
        Wrap.SetStyle(HtmlStyles.borderRadius, "6px")
        Wrap.SetStyle(HtmlStyles.boxShadow, "1px 2px 2px 1px rgba(0, 0, 0, 0.15)")

        Wrap.InnerText = Rpt_Empty()

        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.width, "95%")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.marginTop, "10px")
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px")
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "10px")

        elmBox.AddItem(FilterSection, 10)
        elmBox.AddItem(Wrap, 10)
          
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript)
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript)
        HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')")

        Dim PageLayout As TitleSection2 = PageTitle()
        PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents)
        PageLayout.ContentWrap.InnerText = filter.HtmlText + elmBox.HtmlText

        Return PageLayout.HtmlText
    End Function
    Public Function NaviClick() As ApiResponse
        Dim m As String = GetDataValue("m")
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.Navigate(m)
        Return _ApiResponse
    End Function
    Private Function Rpt_Empty() As String
        Dim Wrap As New Wrap
        Wrap.SetStyle(HtmlStyles.display, "table-cell")
        Wrap.SetStyle(HtmlStyles.verticalAlign, "bottom")
        Wrap.SetStyle(HtmlStyles.fontSize, "20px")
        Wrap.SetStyle(HtmlStyles.color, "darkolivegreen")
        Wrap.InnerText = Translator.Format("selrpt")
        Return Wrap.HtmlText
    End Function
    Public Function CallReport() As ApiResponse
        Dim rpt As String = GetDataValue("rpt")

        Dim _ApiResponse As New ApiResponse

        Dim rpthtml As String = String.Empty
        Select Case rpt
            Case "00000"
                rpthtml = Rpt_Empty()
            Case "10000"
                'rpthtml = Rpt_10000()
            Case "20000"
                'rpthtml = Rpt_20000()
            Case "30000"
                'rpthtml = Rpt_30000()
        End Select

        _ApiResponse.SetElementContents("RptBox", rpthtml)
        Return _ApiResponse
    End Function

    Private Function Rpt_10000() As String
        Dim rptHtml As String = String.Empty

        Dim bdt As String = DateTime.Now.ToString("yyyy-MM-dd")

        Dim filterText As New Texts(TextTypes.date)
        filterText.Wrap.SetStyle(HtmlStyles.margin, "2px")
        filterText.Wrap.SetStyle(HtmlStyles.marginRight, "10px")
        filterText.Wrap.SetStyle(HtmlStyles.marginTop, "10px")
        filterText.Text.SetStyle(HtmlStyles.fontSize, "20px")
        filterText.Text.SetStyle(HtmlStyles.height, "20px")
        filterText.Text.SetStyle(HtmlStyles.padding, "8px")
        filterText.Text.SetStyle(HtmlStyles.border, "none")
        filterText.Text.SetStyle(HtmlStyles.borderBottom, "1px solid #aaa")
        filterText.Text.SetAttribute(HtmlAttributes.id, "fltdte")
        filterText.Text.SetAttribute(HtmlAttributes.value, bdt)

        Dim filterBtn As New Button
        filterBtn.SetStyle(HtmlStyles.backgroundImage, "url('" + ImagePath + "search.jpg')")
        filterBtn.SetStyle(HtmlStyles.backgroundRepeat, "no-repeat")
        filterBtn.SetStyle(HtmlStyles.backgroundSize, "24px 24px")
        filterBtn.SetStyle(HtmlStyles.borderRadius, "50%")
        filterBtn.SetStyle(HtmlStyles.border, "1px solid #ddd")
        filterBtn.SetStyle(HtmlStyles.padding, "6px")
        filterBtn.SetStyle(HtmlStyles.height, "30px")
        filterBtn.SetStyle(HtmlStyles.width, "30px")
        filterBtn.SetStyle(HtmlStyles.boxShadow, "1px 2px 2px 1px rgba(0, 0, 0, 0.15)")
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("Report/RptRlt_10000", "fltdte=::"))

        Dim FilterSection As New FilterSection
        FilterSection.Wrap.SetStyle(HtmlStyles.marginTop, "20px")
        FilterSection.Wrap.SetStyle(HtmlStyles.width, "100%")
        FilterSection.Wrap.SetStyle(HtmlStyles.padding, String.Empty)
        FilterSection.FilterHtml = filterText.HtmlText + filterBtn.HtmlText

        Dim Wrap As New Wrap
        Wrap.SetAttribute(HtmlAttributes.id, "RptRlt")
        Wrap.SetStyle(HtmlStyles.marginLeft, "8px")
        Wrap.SetStyle(HtmlStyles.paddingTop, "8px")
        Wrap.SetStyle(HtmlStyles.width, "100%")

        rptHtml = FilterSection.HtmlText + Wrap.HtmlText

        Return rptHtml
    End Function
    Public Function RptRlt_10000() As ApiResponse
        Dim fltdte As String = GetDataValue("fltdte")

        Dim _ApiResponse As New ApiResponse

        If IsDate(fltdte) = False Then
            _ApiResponse.SetElementContents("RptRlt", Translator.Format("errdatatype"))
        Else
            Dim SQLGridInfo As New SQLGridSection.SQLGridInfo With { _
                    .Id = "ClockInGrid",
                    .Name = Translator.Format("clockin"),
                    .CurrentPageNo = 1,
                    .LinesPerPage = 50,
                    .DisplayCount = SQLGridSection.DisplayCounts.FilteredOnly,
                    .TitleEnabled = True,
                    .TDictionary = Me.HtmlTranslator.TDictionary,
                    .Query = New SQLGridSection.SQLQuery With {
                    .Tables = "TagLog a inner join Loc b on a.LocId = b.LocId inner join Mem c on a.MemId = c.MemId ",
                    .OrderBy = {"a.sysdte desc"}, _
                    .Columns = {"c.MemName", "c.MemEmail", "b.LocName", "left(a.CDST,5) Distance", "b.LocUnit", _
                                "dateadd(hour, convert(int,LocTime), a.sysdte) Local"}, _
                    .ColumnAlias = {Translator.Format("name"),
                                    Translator.Format("email"),
                                    Translator.Format("loc"),
                                    Translator.Format("dst"),
                                    Translator.Format("unit"),
                                    Translator.Format("time")}, _
                    .Filters = " convert(varchar(10),dateadd(hour, convert(int,LocTime), a.sysdte),121) = '" + _
                                    DateTime.Parse(fltdte).ToString("yyyy-MM-dd") + "' "
                }
              }
            Dim SQLGrid As New SQLGridSection(SQLGridInfo)
            SetGridStyle(SQLGrid)

            _ApiResponse.SetElementContents("RptRlt", SQLGrid.HtmlText)
        End If

        Return _ApiResponse
    End Function
    Private Sub SetGridStyle(SQLGrid As SQLGridSection)
        SQLGrid.Wrap.SetStyle(HtmlStyles.margin, String.Empty)
        SQLGrid.Wrap.SetStyle(HtmlStyles.display, "inline-block")

        If SQLGrid.GridData IsNot Nothing Then

            SQLGrid.Grid.TableColumns(2).SetColumnStyle(HtmlStyles.whiteSpace, "nowrap")
            SQLGrid.Grid.TableColumns(5).SetColumnStyle(HtmlStyles.whiteSpace, "nowrap")
            SQLGrid.Grid.TableColumns(4).SetColumnFormat("@R {4} | K.Km, M.Mile")
            SQLGrid.Grid.TableColumns(5).SetColumnFormat("@D {5} | MM/dd/yyyy HH:mm:ss")

            For i As Integer = 0 To SQLGrid.Grid.TableColumns.Count - 1
                Select Case i
                    Case 0, 1
                        SQLGrid.Grid.TableColumns(i).SetColumnStyle(HtmlStyles.textAlign, "left")
                    Case Else
                        SQLGrid.Grid.TableColumns(i).SetColumnStyle(HtmlStyles.textAlign, "center")
                End Select
            Next
        End If
    End Sub

    Private Function Rpt_20000() As String
        Dim rptHtml As String = String.Empty

        Dim bdt As String = DateTime.Now.ToString("yyyy-MM")

        Dim filterText As New Texts(TextTypes.month)
        filterText.Wrap.SetStyle(HtmlStyles.margin, "2px")
        filterText.Wrap.SetStyle(HtmlStyles.marginRight, "10px")
        filterText.Wrap.SetStyle(HtmlStyles.marginTop, "10px")
        filterText.Text.SetStyle(HtmlStyles.fontSize, "20px")
        filterText.Text.SetStyle(HtmlStyles.height, "20px")
        filterText.Text.SetStyle(HtmlStyles.padding, "8px")
        filterText.Text.SetStyle(HtmlStyles.border, "none")
        filterText.Text.SetStyle(HtmlStyles.borderBottom, "1px solid #aaa")
        filterText.Text.SetAttribute(HtmlAttributes.id, "fltdte")
        filterText.Text.SetAttribute(HtmlAttributes.value, bdt)

        Dim filterBtn As New Button
        filterBtn.SetStyle(HtmlStyles.backgroundImage, "url('" + ImagePath + "search.jpg')")
        filterBtn.SetStyle(HtmlStyles.backgroundRepeat, "no-repeat")
        filterBtn.SetStyle(HtmlStyles.backgroundSize, "24px 24px")
        filterBtn.SetStyle(HtmlStyles.borderRadius, "50%")
        filterBtn.SetStyle(HtmlStyles.border, "1px solid #ddd")
        filterBtn.SetStyle(HtmlStyles.padding, "6px")
        filterBtn.SetStyle(HtmlStyles.height, "30px")
        filterBtn.SetStyle(HtmlStyles.width, "30px")
        filterBtn.SetStyle(HtmlStyles.boxShadow, "1px 2px 2px 1px rgba(0, 0, 0, 0.15)")
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("Report/RptRlt_20000", "fltdte=::"))

        Dim FilterSection As New FilterSection
        FilterSection.Wrap.SetStyle(HtmlStyles.marginTop, "20px")
        FilterSection.Wrap.SetStyle(HtmlStyles.width, "100%")
        FilterSection.Wrap.SetStyle(HtmlStyles.padding, String.Empty)
        FilterSection.FilterHtml = filterText.HtmlText + filterBtn.HtmlText

        Dim Wrap As New Wrap
        Wrap.SetAttribute(HtmlAttributes.id, "RptRlt")
        Wrap.SetStyle(HtmlStyles.marginLeft, "8px")
        Wrap.SetStyle(HtmlStyles.paddingTop, "8px")
        Wrap.SetStyle(HtmlStyles.width, "100%")

        rptHtml = FilterSection.HtmlText + Wrap.HtmlText

        Return rptHtml
    End Function
    Public Function RptRlt_20000() As ApiResponse
        Dim fltdte As String = GetDataValue("fltdte")

        Dim _ApiResponse As New ApiResponse

        If IsDate(fltdte) = False Then
            _ApiResponse.SetElementContents("RptRlt", Translator.Format("errdatatype"))
        Else
            Dim SQLGridInfo As New SQLGridSection.SQLGridInfo With { _
                    .Id = "ClockInGrid",
                    .Name = Translator.Format("clockin"),
                    .CurrentPageNo = 1,
                    .LinesPerPage = 50,
                    .DisplayCount = SQLGridSection.DisplayCounts.FilteredOnly,
                    .TitleEnabled = True,
                    .TDictionary = Me.HtmlTranslator.TDictionary,
                    .Query = New SQLGridSection.SQLQuery With {
                    .Tables = "TagLog a inner join Loc b on a.LocId = b.LocId inner join Mem c on a.MemId = c.MemId ",
                    .OrderBy = {"a.sysdte desc"}, _
                    .Columns = {"c.MemName", "c.MemEmail", "b.LocName", "left(a.CDST,5) Distance", "b.LocUnit", _
                                "dateadd(hour, convert(int,LocTime), a.sysdte) Local"}, _
                    .ColumnAlias = {Translator.Format("name"),
                                    Translator.Format("email"),
                                    Translator.Format("loc"),
                                    Translator.Format("dst"),
                                    Translator.Format("unit"),
                                    Translator.Format("time")}, _
                    .Filters = " convert(varchar(7),dateadd(hour, convert(int,LocTime), a.sysdte),121) = '" + _
                                    DateTime.Parse(fltdte).ToString("yyyy-MM") + "' "
                }
              }
            Dim SQLGrid As New SQLGridSection(SQLGridInfo)
            SetGridStyle(SQLGrid)

            _ApiResponse.SetElementContents("RptRlt", SQLGrid.HtmlText)
        End If

        Return _ApiResponse
    End Function

    Private Function Rpt_30000() As String
        Dim rptHtml As String = String.Empty

        Dim bdt As String = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd")
        Dim edt As String = DateTime.Now.ToString("yyyy-MM-dd")

        Dim filterText As New Texts(TextTypes.date)
        filterText.Wrap.SetStyle(HtmlStyles.margin, "2px")
        filterText.Wrap.SetStyle(HtmlStyles.marginRight, "10px")
        filterText.Wrap.SetStyle(HtmlStyles.marginTop, "10px")
        filterText.Text.SetStyle(HtmlStyles.fontSize, "20px")
        filterText.Text.SetStyle(HtmlStyles.height, "20px")
        filterText.Text.SetStyle(HtmlStyles.padding, "8px")
        filterText.Text.SetStyle(HtmlStyles.border, "none")
        filterText.Text.SetStyle(HtmlStyles.borderBottom, "1px solid #aaa")
        filterText.Text.SetAttribute(HtmlAttributes.id, "fltdte")
        filterText.Text.SetAttribute(HtmlAttributes.value, bdt)

        Dim filterText1 As New Texts(TextTypes.date)
        filterText1.Wrap.SetStyle(HtmlStyles.margin, "2px")
        filterText1.Wrap.SetStyle(HtmlStyles.marginRight, "10px")
        filterText1.Wrap.SetStyle(HtmlStyles.marginTop, "10px")
        filterText1.Text.SetStyle(HtmlStyles.fontSize, "20px")
        filterText1.Text.SetStyle(HtmlStyles.height, "20px")
        filterText1.Text.SetStyle(HtmlStyles.padding, "8px")
        filterText1.Text.SetStyle(HtmlStyles.border, "none")
        filterText1.Text.SetStyle(HtmlStyles.borderBottom, "1px solid #aaa")
        filterText1.Text.SetAttribute(HtmlAttributes.id, "fltdte1")
        filterText1.Text.SetAttribute(HtmlAttributes.value, edt)

        Dim filterBtn As New Button
        filterBtn.SetStyle(HtmlStyles.backgroundImage, "url('" + ImagePath + "search.jpg')")
        filterBtn.SetStyle(HtmlStyles.backgroundRepeat, "no-repeat")
        filterBtn.SetStyle(HtmlStyles.backgroundSize, "24px 24px")
        filterBtn.SetStyle(HtmlStyles.borderRadius, "50%")
        filterBtn.SetStyle(HtmlStyles.border, "1px solid #ddd")
        filterBtn.SetStyle(HtmlStyles.padding, "6px")
        filterBtn.SetStyle(HtmlStyles.height, "30px")
        filterBtn.SetStyle(HtmlStyles.width, "30px")
        filterBtn.SetStyle(HtmlStyles.boxShadow, "1px 2px 2px 1px rgba(0, 0, 0, 0.15)")
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("Report/RptRlt_30000", "fltdte=::&fltdte1=::"))

        Dim FilterSection As New FilterSection
        FilterSection.Wrap.SetStyle(HtmlStyles.marginTop, "20px")
        FilterSection.Wrap.SetStyle(HtmlStyles.width, "100%")
        FilterSection.Wrap.SetStyle(HtmlStyles.padding, String.Empty)
        FilterSection.FilterHtml = filterText.HtmlText + filterText1.HtmlText + filterBtn.HtmlText

        Dim Wrap As New Wrap
        Wrap.SetAttribute(HtmlAttributes.id, "RptRlt")
        Wrap.SetStyle(HtmlStyles.marginLeft, "8px")
        Wrap.SetStyle(HtmlStyles.paddingTop, "8px")
        Wrap.SetStyle(HtmlStyles.width, "100%")

        rptHtml = FilterSection.HtmlText + Wrap.HtmlText

        Return rptHtml
    End Function
    Public Function RptRlt_30000() As ApiResponse
        Dim fltdte As String = GetDataValue("fltdte")
        Dim fltdte1 As String = GetDataValue("fltdte1")

        Dim _ApiResponse As New ApiResponse

        If IsDate(fltdte) = False Or IsDate(fltdte1) = False Then
            _ApiResponse.SetElementContents("RptRlt", Translator.Format("errdatatype"))
        Else
            Dim SQLGridInfo As New SQLGridSection.SQLGridInfo With { _
                    .Id = "ClockInGrid",
                    .Name = Translator.Format("clockin"),
                    .CurrentPageNo = 1,
                    .LinesPerPage = 50,
                    .DisplayCount = SQLGridSection.DisplayCounts.FilteredOnly,
                    .TitleEnabled = True,
                    .TDictionary = Me.HtmlTranslator.TDictionary,
                    .Query = New SQLGridSection.SQLQuery With {
                    .Tables = "TagLog a inner join Loc b on a.LocId = b.LocId inner join Mem c on a.MemId = c.MemId ",
                    .OrderBy = {"a.sysdte desc"}, _
                    .Columns = {"c.MemName", "c.MemEmail", "b.LocName", "left(a.CDST,5) Distance", "b.LocUnit", _
                                "dateadd(hour, convert(int,LocTime), a.sysdte) Local"}, _
                    .ColumnAlias = {Translator.Format("name"),
                                    Translator.Format("email"),
                                    Translator.Format("loc"),
                                    Translator.Format("dst"),
                                    Translator.Format("unit"),
                                    Translator.Format("time")}, _
                    .Filters = " convert(varchar(10),dateadd(hour, convert(int,LocTime), a.sysdte),121) between " +
                               " '" + DateTime.Parse(fltdte).ToString("yyyy-MM-dd") + "' and '" + DateTime.Parse(fltdte1).ToString("yyyy-MM-dd") + "' "
                }
              }
            Dim SQLGrid As New SQLGridSection(SQLGridInfo)
            SetGridStyle(SQLGrid)

            _ApiResponse.SetElementContents("RptRlt", SQLGrid.HtmlText)
        End If

        Return _ApiResponse
    End Function
End Class
