Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysMenuEV
    Inherits WebBase

    Sub New()
        ViewFields.AddRange({New NameFlag With {.name = "MenuId", .flag = True},
                             New NameFlag With {.name = "MenuDesc"},
                             New NameFlag With {.name = "MenuArea"},
                             New NameFlag With {.name = "MenuTag"},
                             New NameFlag With {.name = "MenuMethod"},
                             New NameFlag With {.name = "MenuParams"},
                             New NameFlag With {.name = "MenuCtl"},
                             New NameFlag With {.name = "MenuType"},
                             New NameFlag With {.name = "MenuClass"},
                             New NameFlag With {.name = "MenuOrder"},
                             New NameFlag With {.name = "MenuUse"},
                             New NameFlag With {.name = "PageId"}})
    End Sub

    Public Overrides Function InitialModel() As ViewPart
        Dim SSQL As String = " Select MenuId,PageId,MenuDesc,MenuArea,MenuTag,MenuMethod,MenuParams,MenuCtl,MenuType,MenuClass,MenuOrder,MenuUse From XysMenu   " +
                             " where MenuId = N'" + PartialData + "'"

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

        'Dim mnu As New HtmlTag
        'mnu.InnerText = Translator.Format("xysmenu")
        'mnu.SetAttribute(HtmlEvents.onclick, ByPassCall("MenuClick", "m=" + References.Pages.XysMenu))
        'mnulist.Add(mnu)
         
        Dim BtnWrap As Wrap = SetPageButtons(If(ViewPart.Mode = ViewMode.New, {"save"}, {}))

        'Dim BtnWrap As New Wrap
        'Dim btn As New Button(Translator.Format("save"), Button.ButtonTypes.Button)
        'btn.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")
        'btn.SetAttribute(HtmlAttributes.class, "button")
        'btn.SetAttribute(HtmlEvents.onclick, ByPassCall("XysMenuEV/SaveView", "PageId=::&MenuDesc=::&MenuArea=::&MenuTag=::&MenuMethod=::&MenuParams=::&MenuUse=::&MenuClass=::"))
        'Dim btn1 As New Button(Translator.Format("delete"), Button.ButtonTypes.Button)
        'btn1.SetAttribute(HtmlAttributes.class, "button1")
        'btn1.SetAttribute(HtmlEvents.onclick, ByPassCall("XysMenuEV/DeleteView"))
        'BtnWrap.InnerText = btn.HtmlText + btn1.HtmlText


        Dim label As New Label
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;")
        label.Wrap.InnerText = If(ViewPart.Mode = ViewMode.New, Translator.Format("newmenu"), Translator.Format("editmenu"))

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px")
        filter.Wrap.SetStyle(HtmlStyles.width, "90%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText
         

        Dim sel1 As New Dropdown(Translator.Format("page"), ViewPart.Field("PageId").name)
        sel1.Required = True
        sel1.SelBox.SetStyle(HtmlStyles.width, "316px")
        sel1.SelOptions = New OptionValues("sql@select PageId,PageName + case when PageName <> PageDesc then ' (' + PageDesc + ')' else '' end as PageName " +
                                           " from XysPage order by PageGroup,PageOrder,PageName", ViewPart.Field("PageId").value)
        sel1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim sel2 As New Dropdown(Translator.Format("area"), ViewPart.Field("MenuArea").name)
        sel2.Required = True
        sel2.SelBox.SetStyle(HtmlStyles.width, "120px")
        sel2.SelOptions = New OptionValues("{X|Method}{M|Menu}{B|Button}", ViewPart.Field("MenuArea").value)
        sel2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text2 As New Texts(Translator.Format("desc"), ViewPart.Field("MenuDesc").name, TextTypes.text)
        text2.Required = True
        text2.Text.SetStyle(HtmlStyles.width, "400px")
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuDesc").value)
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text As New Texts(Translator.Format("tag"), ViewPart.Field("MenuTag").name, TextTypes.text)
        text.Text.SetStyle(HtmlStyles.width, "268px")
        text.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuTag").value)
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text1 As New Texts(Translator.Format("method"), ViewPart.Field("MenuMethod").name, TextTypes.text)
        text1.Required = True
        text1.Text.SetStyle(HtmlStyles.width, "400px")
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "400")
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuMethod").value)
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text3 As New Texts(Translator.Format("params"), ViewPart.Field("MenuParams").name, TextTypes.text)
        text3.Text.SetStyle(HtmlStyles.width, "400px")
        text3.Text.SetStyle(HtmlStyles.fontSize, "14px")
        text3.Text.SetAttribute(HtmlAttributes.maxlength, "500")
        text3.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuParams").value)
        text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text4 As New Texts(Translator.Format("class"), ViewPart.Field("MenuClass").name, TextTypes.text)
        text4.Text.SetStyle(HtmlStyles.width, "200px")
        text4.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text4.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuClass").value)
        text4.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")


        Dim sel3 As New Dropdown(Translator.Format("order"), ViewPart.Field("MenuOrder").name)
        sel3.Required = True
        sel3.SelBox.SetStyle(HtmlStyles.width, "80px")
        sel3.SelOptions = New OptionValues("{0|0}{1|1}{2|2}{3|3}{4|4}{5|5}{6|6}{7|7}{8|8}{9|9}{10|10}{11|11}{12|12}{13|13}{14|14}{15|15}", ViewPart.Field("MenuOrder").value)
        sel3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim chk1 As New CheckBox(Translator.Format("use"))
        If ViewPart.Mode = ViewMode.Edit Then
            chk1.Checks.AddItem(ViewPart.Field("MenuUse").name, "1", String.Empty, If(Val(ViewPart.Field("MenuUse").value) = 1, True, False))
        Else
            chk1.Checks.AddItem(ViewPart.Field("MenuUse").name, "1", String.Empty, True)
        End If
        chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")


        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.width, "90%")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.marginTop, "8px")
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px")
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px")

        elmBox.AddItem(sel1, 1)
        elmBox.AddItem(text2, 1)
        elmBox.AddItem(sel2)
        elmBox.AddItem(text, 20)
        elmBox.AddItem(text1, 1)
        elmBox.AddItem(text3, 1)
        elmBox.AddItem(text4, 20)
        elmBox.AddItem(sel3, 20)
        elmBox.AddItem(chk1, 50)
        elmBox.AddItem(BtnWrap, 20)

        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText
        Return ViewHtml
    End Function

    Public Function SaveView() As ApiResponse
        Dim MenuDesc As String = ViewPart.Field("MenuDesc").value
        Dim MenuArea As String = ViewPart.Field("MenuArea").value
        Dim MenuTag As String = ViewPart.Field("MenuTag").value
        Dim MenuMethod As String = ViewPart.Field("MenuMethod").value
        Dim MenuParams As String = ViewPart.Field("MenuParams").value
        Dim MenuClass As String = ViewPart.Field("MenuClass").value
        Dim MenuOrder As String = ViewPart.Field("MenuOrder").value
        Dim MenuUse As String = ViewPart.Field("MenuUse").value
        Dim PageId As String = ViewPart.Field("PageId").value

        Dim _ApiResponse As New ApiResponse
        If PageId = String.Empty OrElse MenuMethod = String.Empty OrElse MenuDesc = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_required"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Else
            Dim rlt As String = PutSaveView()
            If rlt = String.Empty Then
                Dim dialogBox As New DialogBox(Translator.Format("msg_saved"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysMenu) + ";onclick:$PopOff();")
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

        Dim tag As String = ViewPart.Field("MenuTag").value
        Dim method As String = ViewPart.Field("MenuMethod").value
        Dim params As String = ViewPart.Field("MenuParams").value
        Dim styleclass As String = ViewPart.Field("MenuClass").value

        Select Case ViewPart.Field("MenuArea").value.ToLower
            Case "x"
                ViewPart.Field("MenuCtl").value = String.Empty
                ViewPart.Field("MenuType").value = String.Empty
            Case "m"
                Dim ctlObj As String = SerializeObject(MenuElement(tag, method, params, styleclass), GetType(HtmlTag))
                ViewPart.Field("MenuCtl").value = EscQuote(ctlObj)
                ViewPart.Field("MenuType").value = GetType(HtmlTag).FullName
            Case "b"
                Dim ctlObj As String = SerializeObject(BtnElement(tag, method, params, styleclass), GetType(Button))
                ViewPart.Field("MenuCtl").value = EscQuote(ctlObj)
                ViewPart.Field("MenuType").value = GetType(Button).FullName
        End Select

        Select Case ViewPart.Mode
            Case ViewMode.New
                ViewPart.Field("MenuId").value = NewID()
                SQL.Add(" Insert into XysMenu( MenuId,PageId,MenuDesc,MenuArea,MenuTag,MenuMethod,MenuParams,MenuCtl,MenuType,MenuClass,MenuOrder,MenuUse,SYSDTE,SYSUSR) " +
                        " values ( @MenuId, @PageId, @MenuDesc, @MenuArea, @MenuTag, @MenuMethod, @MenuParams, @MenuCtl, @MenuType, @MenuClass,@MenuOrder,@MenuUse, getdate(), @SYSUSR)")
            Case ViewMode.Edit
                SQL.Add(" Update XysMenu set " +
                        " PageId = @PageId, MenuDesc = @MenuDesc, MenuArea = @MenuArea, MenuTag = @MenuTag, MenuMethod = @MenuMethod, " +
                        " MenuParams = @MenuParams, MenuCtl = @MenuCtl,MenuType = @MenuType, MenuClass = @MenuClass, MenuOrder = @MenuOrder, MenuUse = @MenuUse, " +
                        " SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " WHERE MenuId = @MenuId")
        End Select

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@MenuId", .Value = ViewPart.Field("MenuId").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@PageId", .Value = ViewPart.Field("PageId").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@MenuDesc", .Value = ViewPart.Field("MenuDesc").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@MenuArea", .Value = ViewPart.Field("MenuArea").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@MenuTag", .Value = ViewPart.Field("MenuTag").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@MenuMethod", .Value = ViewPart.Field("MenuMethod").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@MenuParams", .Value = ViewPart.Field("MenuParams").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@MenuCtl", .Value = ViewPart.Field("MenuCtl").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@MenuType", .Value = ViewPart.Field("MenuType").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@MenuClass", .Value = ViewPart.Field("MenuClass").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@MenuOrder", .Value = Val(ViewPart.Field("MenuOrder").value).ToString, .SqlDbType = SqlDbType.Int})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@MenuUse", .Value = Val(ViewPart.Field("MenuUse").value).ToString, .SqlDbType = SqlDbType.Int})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SYSUSR", .Value = AppKey.UserId, .SqlDbType = Data.SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

    Public Function DeleteView() As ApiResponse
        Dim _ApiResponse As New ApiResponse

        Dim dialogBox As New DialogBox(Translator.Format("deletemenu"))
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;")
        dialogBox.AddButton(Translator.Format("yes"), String.Empty, "class:button1;onclick:" + ByPassCall("XysMenuEV/DeleteViewConfirm"))
        dialogBox.AddButton(Translator.Format("no"), String.Empty, "onclick:$PopOff();class:button;")
        _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)

        Return _ApiResponse
    End Function

    Public Function DeleteViewConfirm() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        Dim rlt As String = PutDeleteView()
        If rlt = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_deleted"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysMenu) + "&$PopOff();")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Else
            Dim dialogBox As New DialogBox(rlt)
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        End If
        Return _ApiResponse
    End Function

    Private Function PutDeleteView() As String
        Dim SQL As New List(Of String) From _
            {
            " delete from XysRoleMenu where MenuId = @MenuId ",
            " delete from XysMenu where Menuid = @MenuId "
            }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@MenuId", .Value = ViewPart.Field("MenuId").value, .SqlDbType = SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function
    Private Function MenuElement(tag As String, method As String, params As String, styleclass As String) As HtmlTag
        params = If(params.Trim <> String.Empty, params.Trim, "{params}")
        Dim mnu As New HtmlTag
        mnu.InnerText = Translator.Format(tag)
        mnu.SetAttribute(HtmlAttributes.class, styleclass)
        mnu.SetAttribute(HtmlEvents.onclick, ByPassCall(method, params, False))
        Return mnu
    End Function
    Private Function BtnElement(tag As String, method As String, params As String, styleclass As String) As Button
        params = If(params.Trim <> String.Empty, params.Trim, "{params}")
        Dim btn As New Button(Translator.Format(tag), Button.ButtonTypes.Button)
        btn.SetAttribute(HtmlAttributes.class, styleclass)
        btn.SetAttribute(HtmlEvents.onclick, ByPassCall(method, params, False))
        btn.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")
        Return btn
    End Function
End Class
