Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysOptionEV
    Inherits WebBase

    Sub New()
        ViewFields.AddRange({New NameFlag With {.name = "CODE", .flag = False},
                             New NameFlag With {.name = "SNO"},
                             New NameFlag With {.name = "SD01"},
                             New NameFlag With {.name = "SD02"},
                             New NameFlag With {.name = "SD03"},
                             New NameFlag With {.name = "SD04"},
                             New NameFlag With {.name = "SD05"},
                             New NameFlag With {.name = "SD06"},
                             New NameFlag With {.name = "SD07"}})
    End Sub

    Public Overrides Function InitialModel() As ViewPart
        Dim SSQL As String = " Select CODE, SNO, SD01, SD02, SD03, SD04, SD05, SD06, SD07  From XysOption   " +
                             " where CODE+convert(varchar(18), SNO) = N'" + PartialData + "'"

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
        label.Wrap.InnerText = If(ViewPart.Mode = ViewMode.New, Translator.Format("newopt"), Translator.Format("editopt"))

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px")
        filter.Wrap.SetStyle(HtmlStyles.width, "90%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText

        Dim text As New Texts(Translator.Format("code"), ViewPart.Field("CODE").name, TextTypes.text)
        text.Wrap.SetStyle(HtmlStyles.display, "none")
        text.Text.SetAttribute(HtmlAttributes.value, "OPTION")
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text1 As New Texts(Translator.Format("no."), ViewPart.Field("SNO").name, TextTypes.text)
        text1.Required = True
        text1.Text.SetStyle(HtmlStyles.width, "100px")
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "10")
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SNO").value)
        If ViewPart.Mode = ViewMode.Edit Then text1.Text.SetAttribute(HtmlAttributes.disabled, "disabled")
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text2 As New Texts(Translator.Format("sd01"), ViewPart.Field("SD01").name, TextTypes.text)
        text2.Required = True
        text2.Text.SetStyle(HtmlStyles.width, "300px")
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD01").value)
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text3 As New Texts(Translator.Format("sd02"), ViewPart.Field("SD02").name, TextTypes.text)
        text3.Text.SetStyle(HtmlStyles.width, "300px")
        text3.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text3.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD02").value)
        text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text4 As New Texts(Translator.Format("sd03"), ViewPart.Field("SD03").name, TextTypes.text)
        text4.Text.SetStyle(HtmlStyles.width, "300px")
        text4.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text4.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD03").value)
        text4.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text5 As New Texts(Translator.Format("sd04"), ViewPart.Field("SD04").name, TextTypes.text)
        text5.Text.SetStyle(HtmlStyles.width, "300px")
        text5.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text5.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD04").value)
        text5.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text6 As New Texts(Translator.Format("sd05"), ViewPart.Field("SD05").name, TextTypes.text)
        text6.Text.SetStyle(HtmlStyles.width, "300px")
        text6.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text6.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD05").value)
        text6.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text7 As New Texts(Translator.Format("sd06"), ViewPart.Field("SD06").name, TextTypes.text)
        text7.Text.SetStyle(HtmlStyles.width, "300px")
        text7.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text7.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD06").value)
        text7.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text8 As New Texts(Translator.Format("sd07"), ViewPart.Field("SD07").name, TextTypes.text)
        text8.Text.SetStyle(HtmlStyles.width, "300px")
        text8.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text8.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD07").value)
        text8.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.width, "90%")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.marginTop, "8px")
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px")
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px")

        elmBox.AddItem(text)
        elmBox.AddItem(text1, 20)
        elmBox.AddItem(text2, 1)
        elmBox.AddItem(text3, 1)
        elmBox.AddItem(text4, 1)
        elmBox.AddItem(text5, 1)
        elmBox.AddItem(text6, 1)
        elmBox.AddItem(text7, 1)
        elmBox.AddItem(text8, 50)
        elmBox.AddItem(BtnWrap, 20)

        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText
        Return ViewHtml
    End Function

    Public Function SaveView() As ApiResponse
        Dim CODE As String = ViewPart.Field("CODE").value
        Dim SNO As String = ViewPart.Field("SNO").value
        Dim SD01 As String = ViewPart.Field("SD01").value
        Dim SD02 As String = ViewPart.Field("SD02").value
        Dim SD03 As String = ViewPart.Field("SD03").value

        Dim _ApiResponse As New ApiResponse
        If CODE = String.Empty OrElse SNO = String.Empty OrElse SD01 = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_required"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Else
            Dim rlt As String = PutSaveView()
            If rlt = String.Empty Then
                Dim dialogBox As New DialogBox(Translator.Format("msg_saved"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysOption) + "&$PopOff();")
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
                SQL.Add(" insert into XysOption(CODE, SNO, SD01, SD02, SD03, SD04, SD05, SD06, SD07) " +
                        " values(@CODE,@SNO,@SD01,@SD02,@SD03,@SD04,@SD05,@SD06,@SD07) ")
            Case ViewMode.Edit
                SQL.Add(" Update XysOption set " +
                        "   SD01 = @SD01, " +
                        "   SD02 = @SD02, " +
                        "   SD03 = @SD03, " +
                        "   SD04 = @SD04, " +
                        "   SD05 = @SD05, " +
                        "   SD06 = @SD06, " +
                        "   SD07 = @SD07 " +
                        " Where  CODE = @CODE and SNO = @SNO  ")
        End Select

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@CODE", .Value = ViewPart.Field("CODE").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SNO", .Value = ViewPart.Field("SNO").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SD01", .Value = ViewPart.Field("SD01").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SD02", .Value = ViewPart.Field("SD02").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SD03", .Value = ViewPart.Field("SD03").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SD04", .Value = ViewPart.Field("SD04").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SD05", .Value = ViewPart.Field("SD05").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SD06", .Value = ViewPart.Field("SD06").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SD07", .Value = ViewPart.Field("SD07").value, .SqlDbType = Data.SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function
     
End Class
