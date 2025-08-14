Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysPageEV
    Inherits WebBase
     
    Sub New()
        ViewFields.AddRange({New NameFlag With {.name = "PageId", .flag = True},
                             New NameFlag With {.name = "PageName"},
                             New NameFlag With {.name = "PageGroup"},
                             New NameFlag With {.name = "PageDesc"},
                             New NameFlag With {.name = "PageOrder"},
                             New NameFlag With {.name = "PageMenu"},
                             New NameFlag With {.name = "PageUse"}})
    End Sub

    Public Overrides Function InitialModel() As ViewPart
        Dim SSQL As String = " Select PageId,PageName,PageGroup,PageDesc,PageOrder,PageMenu,PageUse From XysPage   " +
                             " where PageId = N'" + PartialData + "'"

        ViewPart = New ViewPart
        ViewPart.Methods = ViewMethods()
        If PartialData <> String.Empty Then
            ViewPart.Mode = ViewMode.Edit
            ViewPart.Data = SQLData.SQLDataTable(SSQL)
            ViewPart.Params = PartialData
        Else
            ViewPart.Mode = ViewMode.New
        End If

        For i As Integer = 0 To ViewFields.Count - 1
            ViewPart.Fields.Add(New NameValue With {.name = ViewFields(i).name, .value = ViewPart.ColunmValue(.name)})
        Next
         
        Return ViewPart
    End Function

    Public Overrides Function InitialView() As String

        Dim mnulist As MenuList = SetPageMenu({})
        Dim BtnWrap As Wrap = SetPageButtons(If(ViewPart.Mode = ViewMode.New, {"save"}, {}))

        Dim label As New Label
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;")
        label.Wrap.InnerText = If(ViewPart.Mode = ViewMode.New, Translator.Format("newpage"), Translator.Format("editpage"))

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px")
        filter.Wrap.SetStyle(HtmlStyles.width, "90%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText

        Dim text As New Texts(Translator.Format("name"), ViewPart.Field("PageName").name, TextTypes.text)
        text.Required = True
        text.Text.SetStyle(HtmlStyles.width, "200px")
        text.Text.SetAttribute(HtmlAttributes.maxlength, "200")
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("PageName").value)
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text1 As New Texts(Translator.Format("group"), ViewPart.Field("PageGroup").name, TextTypes.text)
        text1.Text.SetStyle(HtmlStyles.width, "200px")
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "50")
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("PageGroup").value)
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text2 As New Texts(Translator.Format("desc"), ViewPart.Field("PageDesc").name, TextTypes.text)
        text2.Required = True
        text2.Text.SetStyle(HtmlStyles.width, "300px")
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "200")
        text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("PageDesc").value)
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text3 As New Texts(Translator.Format("order"), ViewPart.Field("PageOrder").name, TextTypes.text)
        text3.Text.SetStyle(HtmlStyles.width, "50px")
        text3.Text.SetAttribute(HtmlAttributes.maxlength, "4")
        text3.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("PageOrder").value)
        text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim chk1 As New CheckBox(Translator.Format("menu"))
        chk1.Checks.AddItem(ViewPart.Field("PageMenu").name, "1", String.Empty, If(Val(ViewPart.Field("PageMenu").value) = 1, True, False))
        chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim chk2 As New CheckBox(Translator.Format("use"))
        chk2.Checks.AddItem(ViewPart.Field("PageUse").name, "1", String.Empty, If(Val(ViewPart.Field("PageUse").value) = 1, True, False))
        chk2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")
          
        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.width, "90%")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.marginTop, "8px")
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px")
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px")

        elmBox.AddItem(text, 1)
        elmBox.AddItem(text1, 1)
        elmBox.AddItem(text2, 20)
        elmBox.AddItem(text3, 20)
        elmBox.AddItem(chk1, 1)
        elmBox.AddItem(chk2, 50)
        elmBox.AddItem(BtnWrap, 20)
         
        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText
        Return ViewHtml
    End Function
  
    Public Function SaveView() As ApiResponse
        Dim PageName As String = ViewPart.Field("PageName").value
        Dim PageGroup As String = ViewPart.Field("PageGroup").value
        Dim PageDesc As String = ViewPart.Field("PageDesc").value
        Dim PageOrder As String = ViewPart.Field("PageOrder").value
        Dim PageMenu As String = ViewPart.Field("PageMenu").value
        Dim PageUse As String = ViewPart.Field("PageUse").value

        Dim _ApiResponse As New ApiResponse
        If PageName = String.Empty OrElse PageDesc = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_required"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Else
            Dim rlt As String = PutSaveView()
            If rlt = String.Empty Then
                Dim dialogBox As New DialogBox(Translator.Format("msg_saved"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysPage) + "&$PopOff();")
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
            Else
                Dim dialogBox As New DialogBox(rlt)
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
            End If
        End If

        Return _ApiResponse
    End Function

    Private Function PutSaveView() As String
        Dim SQL As New List(Of String)

        Select Case ViewPart.Mode
            Case ViewMode.New
                ViewPart.Field("PageId").value = NewID()
                SQL.Add(" Insert into XysPage( PageId,PageName,PageGroup,PageDesc,PageOrder,PageMenu,PageUse,SYSDTE,SYSUSR) " +
                        " values( @PageId, @PageName, @PageGroup, @PageDesc, @PageOrder, @PageMenu,@PageUse, getdate(), @SYSUSR) ")
            Case ViewMode.Edit
                SQL.Add(" Update XysPage set " +
                        " PageName = @PageName, PageGroup = @PageGroup, PageDesc = @PageDesc, " +
                        " PageOrder = @PageOrder, PageMenu = @PageMenu, PageUse= @PageUse, SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " WHERE PageId = @PageId")
        End Select

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@PageId", .Value = ViewPart.Field("PageId").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@PageName", .Value = ViewPart.Field("PageName").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@PageGroup", .Value = ViewPart.Field("PageGroup").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@PageDesc", .Value = ViewPart.Field("PageDesc").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@PageOrder", .Value = Val(ViewPart.Field("PageOrder").value).ToString, .SqlDbType = SqlDbType.Int})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@PageMenu", .Value = Val(ViewPart.Field("PageMenu").value).ToString, .SqlDbType = SqlDbType.Int})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@PageUse", .Value = Val(ViewPart.Field("PageUse").value).ToString, .SqlDbType = SqlDbType.Int})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SYSUSR", .Value = AppKey.UserId, .SqlDbType = Data.SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

    Public Function DeleteView() As ApiResponse
        'Dim keyVlus As List(Of KeyVlu) = DecryptCallAction()

        Dim _ApiResponse As New ApiResponse

        Dim dialogBox As New DialogBox(Translator.Format("deletepage"))
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;")
        dialogBox.AddButton(Translator.Format("yes"), String.Empty, "class:button1;onclick:" + ByPassCall("XysPageEV/DeleteViewConfirm"))
        dialogBox.AddButton(Translator.Format("no"), String.Empty, "onclick:$PopOff();class:button;")
        _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)

        Return _ApiResponse
    End Function

    Public Function DeleteViewConfirm() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        Dim rlt As String = PutDeleteViewData()
        If rlt = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_deleted"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysPage) + "&$PopOff();")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Else
            Dim dialogBox As New DialogBox(rlt)
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        End If
        Return _ApiResponse
    End Function

    Private Function PutDeleteViewData() As String
        Dim SQL As New List(Of String) From _
            {
            " delete from XysRoleMenu where MenuId in (select MenuId from XysMenu where Pageid = @PageId) ",
            " delete from XysMenu where Pageid = @PageId ",
            " delete from XysRolePage where Pageid = @PageId ",
            " delete from XysPage where Pageid = @PageId "
            }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@PageId", .Value = ViewPart.Field("PageId").value, .SqlDbType = SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

End Class
