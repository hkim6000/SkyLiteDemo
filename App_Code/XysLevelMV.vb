Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysLevelMV
    Inherits WebBase

    Private SQLGridInfo As New SQLGridSection.SQLGridInfo With { _
        .Id = "DataGrid",
        .Name = "DataGrid",
        .CurrentPageNo = If(Val(ParamValue("DataGrid_PageNo")) = 0, 1, Val(ParamValue("DataGrid_PageNo"))),
        .LinesPerPage = 50,
        .ExcludeDownloadColumns = {0}, _
        .TDictionary = Me.HtmlTranslator.TDictionary,
        .Query = New SQLGridSection.SQLQuery With {
            .Tables = "XysLevel",
            .OrderBy = {"LevelCode"}, _
            .Columns = {"LevelCode", "LevelName", "LevelDesc", "LevelFlag", _
                        "dbo.XF_OffetTime(SYSDTE," + CSTimeOffset.ToString + ") SYSDTE", _
                        "dbo.XF_UserName(SYSUSR) SYSUSR"}, _
            .ColumnAlias = {Translator.Format("code"), _
                            Translator.Format("name"), _
                            Translator.Format("desc"), _
                            Translator.Format("inuse"), _
                            Translator.Format("modified"), _
                            Translator.Format("by")}, _
            .Filters = If(ParamValue("DataGrid_Filter") = String.Empty, String.Empty, ParamValue("DataGrid_Filter"))
        }
    }
    Public Overrides Function InitialModel() As ViewPart
        Return New ViewPart With {.Methods = ViewMethods("XysLevel")}
    End Function
    Public Overrides Function InitialView() As String
        Dim mnulist As MenuList = SetPageMenu({})
        Dim BtnWrap As Wrap = SetPageButtons({})

        Dim filterText As New Texts(TextTypes.text)
        filterText.Wrap.SetStyle(HtmlStyles.margin, "2px")
        filterText.Wrap.SetStyle(HtmlStyles.marginTop, "4px")
        filterText.Wrap.SetStyle(HtmlStyles.marginLeft, "8px")
        filterText.Text.SetStyle(HtmlStyles.fontSize, "16px")
        filterText.Text.SetStyle(HtmlStyles.height, "24px")
        filterText.Text.SetAttribute(HtmlAttributes.placeholder, Translator.Format("searchterm"))
        filterText.Text.SetAttribute(HtmlAttributes.id, "sbox")
        filterText.Text.SetAttribute(HtmlAttributes.value, ParamValue("sboxtxt"))

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
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysLevelMV/SearchClicked"))

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px")
        filter.Wrap.SetStyle(HtmlStyles.width, "90%")
        filter.Menu = mnulist
        filter.FilterHtml = filterText.HtmlText + filterBtn.HtmlText

        Dim SQLGrid As New SQLGridSection(SQLGridInfo)
        SetGridStyle(SQLGrid)

        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.width, "90%")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.marginTop, "8px")
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px")
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "20px 10px 20px 10px")

        elmBox.AddItem(SQLGrid)

        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText

        Return ViewHtml
    End Function
    Private Sub SetGridStyle(SQLGrid As SQLGridSection)
        SQLGrid.Wrap.SetStyle(HtmlStyles.margin, String.Empty)
        SQLGrid.Wrap.SetStyle(HtmlStyles.marginLeft, "8px")
        SQLGrid.Wrap.SetStyle(HtmlStyles.display, "inline-block")

        If SQLGrid.GridData IsNot Nothing Then

            SQLGrid.Grid.TableColumns(0).SetColumnAttribute(HtmlEvents.onclick, ByPassCall("MenuClick", "m=" + References.Pages.XysLevelEV + "&t={0}"))
            SQLGrid.Grid.TableColumns(0).SetColumnStyle(HtmlStyles.textDecoration, "underline")
            SQLGrid.Grid.TableColumns(0).SetColumnStyle(HtmlStyles.cursor, "pointer")
            SQLGrid.Grid.TableColumns(0).SetColumnStyle(HtmlStyles.whiteSpace, "nowrap")

            SQLGrid.Grid.TableColumns(3).SetColumnFormat("@R {3} | 0. , 1.✓")
            SQLGrid.Grid.TableColumns(4).SetColumnFormat("@D {4} |" + DateFormat)


            For i As Integer = 0 To SQLGrid.Grid.TableColumns.Count - 1
                Select Case i
                    Case 2
                        SQLGrid.Grid.TableColumns(i).SetColumnStyle(HtmlStyles.textAlign, "left")
                    Case Else
                        SQLGrid.Grid.TableColumns(i).SetColumnStyle(HtmlStyles.textAlign, "center")
                End Select
            Next
        End If
    End Sub

    Public Function SearchClicked() As ApiResponse
        Dim sbox As String = ParamValue("sbox")

        SQLGridInfo.Query.Filters = "LevelCode+LevelName+LevelDesc like N'%" + sbox + "%' "
        Dim SQLGrid As New SQLGridSection(SQLGridInfo)
        SetGridStyle(SQLGrid)


        Dim _ApiResponse As New ApiResponse
        _ApiResponse.ReplaceSQLGridSection("DataGrid", SQLGrid)
        _ApiResponse.StoreLocalValue("sboxtxt", sbox)
        Return _ApiResponse
    End Function
End Class
