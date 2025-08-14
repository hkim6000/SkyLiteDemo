Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysLevelEV
    Inherits WebBase

    Sub New()
        ViewFields.AddRange({New NameFlag With {.name = "LevelCode"},
                             New NameFlag With {.name = "LevelName"},
                             New NameFlag With {.name = "LevelDesc"},
                             New NameFlag With {.name = "LevelFlag"}})
    End Sub

    Public Overrides Function InitialModel() As ViewPart
        Dim SSQL As String = " Select LevelCode,LevelName,LevelDesc,LevelFlag  From XysLevel   " +
                             " where LevelCode = N'" + PartialData + "'"

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
        label.Wrap.InnerText = If(ViewPart.Mode = ViewMode.New, Translator.Format("newlevel"), Translator.Format("editlevel"))

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px")
        filter.Wrap.SetStyle(HtmlStyles.width, "90%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText

        Dim text As New Texts(Translator.Format("code"), ViewPart.Field("LevelCode").name, TextTypes.text)
        text.Required = True
        text.Text.SetStyle(HtmlStyles.width, "200px")
        text.Text.SetAttribute(HtmlAttributes.maxlength, "200")
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("LevelCode").value)
        If ViewPart.Mode = ViewMode.Edit Then text.Text.SetAttribute(HtmlAttributes.disabled, "disabled")
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text1 As New Texts(Translator.Format("name"), ViewPart.Field("LevelName").name, TextTypes.text)
        text1.Required = True
        text1.Text.SetStyle(HtmlStyles.width, "200px")
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "50")
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("LevelName").value)
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text2 As New Texts(Translator.Format("desc"), ViewPart.Field("LevelDesc").name, TextTypes.text)
        text2.Text.SetStyle(HtmlStyles.width, "300px")
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "200")
        text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("LevelDesc").value)
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")
         
        Dim chk1 As New CheckBox(Translator.Format("flag"))
        chk1.Checks.AddItem(ViewPart.Field("LevelFlag").name, "1", String.Empty, If(Val(ViewPart.Field("LevelFlag").value) = 1, True, False))
        chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")
         
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
        elmBox.AddItem(chk1, 50)
        elmBox.AddItem(BtnWrap, 20)

        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText
        Return ViewHtml
    End Function

    Public Function SaveView() As ApiResponse
        Dim LevelCode As String = ViewPart.Field("LevelCode").value
        Dim LevelName As String = ViewPart.Field("LevelName").value
        Dim LevelDesc As String = ViewPart.Field("LevelDesc").value
        Dim LevelFlag As String = ViewPart.Field("LevelFlag").value

        Dim _ApiResponse As New ApiResponse
        If LevelCode = String.Empty OrElse LevelName = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_required"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Else
            Dim rlt As String = PutSaveView()
            If rlt = String.Empty Then
                Dim dialogBox As New DialogBox(Translator.Format("msg_saved"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysLevel) + "&$PopOff();")
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
                SQL.Add(" Insert into XysLevel( LevelCode,LevelName,LevelDesc,LevelFlag ,SYSDTE,SYSUSR) " +
                        " values( @LevelCode, @LevelName, @LevelDesc, @LevelFlag, getdate(), @SYSUSR) ")
            Case ViewMode.Edit
                SQL.Add(" Update XysLevel set " +
                        " LevelName = @LevelName, LevelDesc = @LevelDesc, LevelFlag = @LevelFlag, " +
                        " SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " WHERE LevelCode = @LevelCode")
        End Select

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@LevelCode", .Value = ViewPart.Field("LevelCode").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@LevelName", .Value = ViewPart.Field("LevelName").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@LevelDesc", .Value = ViewPart.Field("LevelDesc").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@LevelFlag", .Value = Val(ViewPart.Field("LevelFlag").value).ToString, .SqlDbType = SqlDbType.Int})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SYSUSR", .Value = AppKey.UserId, .SqlDbType = Data.SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

    Public Function DeleteView() As ApiResponse
        'Dim keyVlus As List(Of KeyVlu) = DecryptCallAction()

        Dim _ApiResponse As New ApiResponse

        Dim dialogBox As New DialogBox(Translator.Format("deleteitem"))
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;")
        dialogBox.AddButton(Translator.Format("yes"), String.Empty, "class:button1;onclick:" + ByPassCall("XysLevelEV/DeleteViewConfirm"))
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
            dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysLevel) + "&$PopOff();")
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
            " delete from XysLevel where LevelCode = @LevelCode "
            }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@LevelCode", .Value = ViewPart.Field("LevelCode").value, .SqlDbType = SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

End Class
