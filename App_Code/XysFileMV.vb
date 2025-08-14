Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysFileMV
    Inherits WebBase

    Private SQLGridInfo As New SQLGridSection.SQLGridInfo With { _
            .Id = "FileGrid",
            .Name = Translator.Format("Files"),
            .CurrentPageNo = 1,
            .LinesPerPage = 50,
            .ExcludeDownloadColumns = {0}, _
            .DisplayCount = SQLGridSection.DisplayCounts.FilteredNAll,
            .TitleEnabled = True,
            .TDictionary = Me.HtmlTranslator.TDictionary,
            .Query = New SQLGridSection.SQLQuery With {
                .Tables = "XysFile",
                .OrderBy = {"FileRef", "FileName"}, _
                .Columns = {"FileId", "FileRef", "FileRefId", "FileName", "FilePath", _
                        "dbo.XF_OffetTime(SYSDTE," + CSTimeOffset.ToString + ") SYSDTE", _
                        "dbo.XF_UserName(SYSUSR) SYSUSR", "'Delete' [Delete]"}, _
                .ColumnAlias = { _
                        Translator.Format("FileId"), _
                        Translator.Format("Reference Area"), _
                        Translator.Format("FileRefId"), _
                        Translator.Format("File Name"), _
                        Translator.Format("File GUID"), _
                        Translator.Format("Uploaded"), _
                        Translator.Format("By"), _
                        " "}, _
                .Filters = ""
        }
    }
    Public Overrides Function InitialModel() As ViewPart
        Return New ViewPart With {.Methods = ViewMethods("XysFile")}
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
        filterBtn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysFileMV/SearchClicked"))

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
        elmBox.Wrap.SetStyle(HtmlStyles.overflow, "auto")

        elmBox.AddItem(SQLGrid)

        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText

        Return ViewHtml
    End Function
    Private Sub SetGridStyle(SQLGrid As SQLGridSection)
        SQLGrid.Wrap.SetStyle(HtmlStyles.margin, String.Empty)
        SQLGrid.Wrap.SetStyle(HtmlStyles.marginLeft, "8px")
        SQLGrid.Wrap.SetStyle(HtmlStyles.display, "inline-block")
        SQLGrid.Wrap.SetStyle(HtmlStyles.width, "Calc(100% - 14px)")
        SQLGrid.Wrap.SetStyle(HtmlStyles.boxSizing, "border-box")

        If SQLGrid.GridData IsNot Nothing Then
            For i As Integer = 0 To SQLGrid.GridData.Rows.Count - 1
                SQLGrid.GridData.Rows(i)(4) = SQLGrid.GridData.Rows(i)(4).ToString.Split("\").Last.Split(".").FirstOrDefault
            Next

            SQLGrid.Grid.TableColumns(0).SetHeaderStyle(HtmlStyles.display, "none")
            SQLGrid.Grid.TableColumns(0).SetColumnStyle(HtmlStyles.display, "none")
            SQLGrid.Grid.TableColumns(2).SetHeaderStyle(HtmlStyles.display, "none")
            SQLGrid.Grid.TableColumns(2).SetColumnStyle(HtmlStyles.display, "none")

            SQLGrid.Grid.TableColumns(7).SetColumnAttribute(HtmlEvents.onclick, ByPassCall("XysFile/RemoveFile", "t={0}"))
            SQLGrid.Grid.TableColumns(7).SetColumnStyle(HtmlStyles.textDecoration, "underline")
            SQLGrid.Grid.TableColumns(7).SetColumnStyle(HtmlStyles.cursor, "pointer")
            SQLGrid.Grid.TableColumns(7).SetColumnStyle(HtmlStyles.color, "#ff6600")

            SQLGrid.Grid.TableColumns(5).SetColumnFormat("@D {5} |" + DateFormat)

            SQLGrid.Grid.TableColumns(3).SetColumnStyle(HtmlStyles.fontSize, "12px")
            SQLGrid.Grid.TableColumns(4).SetColumnStyle(HtmlStyles.fontSize, "12px")
            SQLGrid.Grid.TableColumns(5).SetColumnStyle(HtmlStyles.fontSize, "12px")
            SQLGrid.Grid.TableColumns(6).SetColumnStyle(HtmlStyles.fontSize, "12px")

            For i As Integer = 0 To SQLGrid.Grid.TableColumns.Count - 1
                Select Case i
                    Case 0, 1, 2, 3, 4
                        SQLGrid.Grid.TableColumns(i).SetColumnStyle(HtmlStyles.textAlign, "left")
                    Case Else
                        SQLGrid.Grid.TableColumns(i).SetColumnStyle(HtmlStyles.textAlign, "center")
                End Select
            Next
        End If
    End Sub

    Public Function SearchClicked() As ApiResponse
        Dim sbox As String = ParamValue("sbox")

        SQLGridInfo.Query.Filters = "FileId+FileRef+FileName like N'%" + sbox + "%' "
        Dim SQLGrid As New SQLGridSection(SQLGridInfo)
        SetGridStyle(SQLGrid)


        Dim _ApiResponse As New ApiResponse
        _ApiResponse.ReplaceElement("FileGrid", SQLGrid.HtmlText)
        _ApiResponse.PopOff()
        Return _ApiResponse
    End Function
End Class
