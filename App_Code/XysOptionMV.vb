Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysOptionMV
    Inherits WebBase

    Private SQLGridInfo As New SQLGridSection.SQLGridInfo With { _
        .Id = "DataGrid",
        .Name = "DataGrid",
        .CurrentPageNo = If(Val(ParamValue("DataGrid_PageNo")) = 0, 1, Val(ParamValue("DataGrid_PageNo"))),
        .LinesPerPage = 50,
        .ExcludeDownloadColumns = {0}, _
        .TDictionary = Me.HtmlTranslator.TDictionary,
        .Query = New SQLGridSection.SQLQuery With {
            .Tables = "XysOption",
            .OrderBy = {"CODE", "SNO"}, _
            .Columns = {"CODE", "SNO", "SD01", "SD02", "SD03", "SD04", "SD05", "SD06", "SD07"}, _
            .ColumnAlias = {Translator.Format("code"), _
                            Translator.Format("no."), _
                            Translator.Format("sd1"), _
                            Translator.Format("sd2"), _
                            Translator.Format("sd3"), _
                            Translator.Format("sd4"), _
                            Translator.Format("sd5"), _
                            Translator.Format("sd6"), _
                            Translator.Format("sd7")}, _
            .Filters = If(ParamValue("DataGrid_Filter") = String.Empty, "CODE=N'OPTION' and CODE+SD01+SD02+SD03+SD04+SD05+SD06+SD07 like '%%' ", ParamValue("DataGrid_Filter"))
        }
    }
    Public Overrides Function InitialModel() As ViewPart
        Return New ViewPart With {.Methods = ViewMethods("XysOption")}
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
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysOptionMV/SearchClicked"))

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
            SQLGrid.Grid.TableColumns(0).SetHeaderStyle(HtmlStyles.display, "none")
            SQLGrid.Grid.TableColumns(0).SetColumnStyle(HtmlStyles.display, "none")

            SQLGrid.Grid.TableColumns(1).SetColumnAttribute(HtmlEvents.onclick, ByPassCall("MenuClick", "m=" + References.Pages.XysOptionEV + "&t={0}{1}"))
            SQLGrid.Grid.TableColumns(1).SetColumnStyle(HtmlStyles.textDecoration, "underline")
            SQLGrid.Grid.TableColumns(1).SetColumnStyle(HtmlStyles.cursor, "pointer")
            SQLGrid.Grid.TableColumns(1).SetColumnStyle(HtmlStyles.whiteSpace, "nowrap")
            SQLGrid.Grid.TableColumns(2).SetColumnStyle(HtmlStyles.whiteSpace, "nowrap")

            SQLGrid.Grid.TableColumns(4).SetColumnFormat("@R {4} | 0. , 1.✓")


            For i As Integer = 0 To SQLGrid.Grid.TableColumns.Count - 1
                SQLGrid.Grid.TableColumns(i).SetHeaderStyle(HtmlStyles.whiteSpace, "nowrap")
                Select Case i
                    Case 2, 3, 4, 5, 6, 7, 8
                        SQLGrid.Grid.TableColumns(i).SetColumnStyle(HtmlStyles.textAlign, "left")
                    Case Else
                        SQLGrid.Grid.TableColumns(i).SetColumnStyle(HtmlStyles.textAlign, "center")
                End Select
            Next
        End If
    End Sub

    Public Function SearchClicked() As ApiResponse
        Dim sbox As String = ParamValue("sbox")

        SQLGridInfo.Query.Filters = "CODE=N'OPTION' and CODE+SD01+SD02+SD03+SD04+SD05+SD06+SD07  like N'%" + sbox + "%' "
        Dim SQLGrid As New SQLGridSection(SQLGridInfo)
        SetGridStyle(SQLGrid)


        Dim _ApiResponse As New ApiResponse
        _ApiResponse.ReplaceSQLGridSection("DataGrid", SQLGrid)
        _ApiResponse.StoreLocalValue("sboxtxt", sbox)
        Return _ApiResponse
    End Function
End Class
