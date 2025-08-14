Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysDictEV
    Inherits WebBase

    Sub New()
        ViewFields.AddRange({New NameFlag With {.name = "Target", .flag = True},
                             New NameFlag With {.name = "IsoCode", .flag = True},
                             New NameFlag With {.name = "KeyWord", .flag = True},
                             New NameFlag With {.name = "Translated"}})
    End Sub

    Public Overrides Function InitialModel() As ViewPart
        Dim SSQL As String = " Select Target,IsoCode,KeyWord,Translated   " +
                             " from XysDict where Target+IsoCode+KeyWord = N'" + PartialData + "'"

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
        label.Wrap.InnerText = If(ViewPart.Mode = ViewMode.New, Translator.Format("newdict"), Translator.Format("editdict"))

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px")
        filter.Wrap.SetStyle(HtmlStyles.width, "90%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText

        Dim lbl As New Label(Translator.Format("target"))
        lbl.Wrap.SetStyles("padding-left:8px;color:#777;")
        Dim Vlbl As New Label(ViewPart.Field("Target").value)
        Vlbl.Wrap.SetStyles("padding-left:8px;font-weight:bold;font-size:20px;")

        Dim lbl1 As New Label(Translator.Format("isocode"))
        lbl1.Wrap.SetStyles("padding-left:8px;color:#777;")
        Dim Vlbl1 As New Label(ViewPart.Field("IsoCode").value)
        Vlbl1.Wrap.SetStyles("padding-left:8px;font-weight:bold;font-size:20px;")

        Dim lbl2 As New Label(Translator.Format("keyword"))
        lbl2.Wrap.SetStyles("padding-left:8px;color:#777;")
        Dim Vlbl2 As New Label(ViewPart.Field("KeyWord").value)
        Vlbl2.Wrap.SetStyles("padding-left:8px;font-weight:bold;font-size:20px;")


        Dim text As New Texts(Translator.Format("translated"), ViewPart.Field("Translated").name, TextTypes.text)
        text.Required = True
        text.Text.SetStyle(HtmlStyles.width, "200px")
        text.Text.SetAttribute(HtmlAttributes.maxlength, "200")
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("Translated").value)
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")


        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.width, "90%")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.marginTop, "8px")
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px")
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px")

        elmBox.AddItem(lbl, 10)
        elmBox.AddItem(Vlbl, 20)
        elmBox.AddItem(lbl1, 10)
        elmBox.AddItem(Vlbl1, 20)
        elmBox.AddItem(lbl2, 10)
        elmBox.AddItem(Vlbl2, 20)
        elmBox.AddItem(text, 40)
        elmBox.AddItem(BtnWrap, 20)

        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText
        Return ViewHtml
    End Function

    Public Function SaveView() As ApiResponse
        Dim Translated As String = ViewPart.Field("Translated").value

        Dim _ApiResponse As New ApiResponse
        If Translated = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_required"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Else
            Dim rlt As String = PutSaveView()
            If rlt = String.Empty Then
                Dim dialogBox As New DialogBox(Translator.Format("msg_saved"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysDict) + "&$PopOff();")
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
                SQL.Add(" Insert into XysDict(Target,IsoCode,KeyWord,Translated) " +
                        " values( @Target,@IsoCode,@KeyWord,@Translated) ")
            Case ViewMode.Edit
                SQL.Add(" Update XysDict set " +
                        " Translated = @Translated " +
                        " WHERE Target+IsoCode+KeyWord = @KeyData")
        End Select

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@KeyData", .Value = ViewPart.Params, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@IsoCode", .Value = ViewPart.Field("IsoCode").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@KeyWord", .Value = ViewPart.Field("KeyWord").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@Translated", .Value = ViewPart.Field("Translated").value, .SqlDbType = SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

    Public Function DeleteView() As ApiResponse
        'Dim keyVlus As List(Of KeyVlu) = DecryptCallAction()

        Dim _ApiResponse As New ApiResponse

        Dim dialogBox As New DialogBox(Translator.Format("deletepage"))
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;")
        dialogBox.AddButton(Translator.Format("yes"), String.Empty, "class:button1;onclick:" + ByPassCall("XysDictEV/DeleteViewConfirm"))
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
            dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysDict) + "&$PopOff();")
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
            " delete from XysDict where Target+IsoCode+KeyWord = @KeyData "
            }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@KeyData", .Value = ViewPart.Params, .SqlDbType = SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function
End Class
