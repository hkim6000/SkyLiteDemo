Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysBulletinEV
    Inherits WebBase

    Sub New()
        ViewFields.AddRange({New NameFlag With {.name = "BltnId", .flag = True},
                             New NameFlag With {.name = "BltnTitle"},
                             New NameFlag With {.name = "BltnMemo"},
                             New NameFlag With {.name = "CreatedBy"},
                             New NameFlag With {.name = "Files", .flag = True},
                             New NameFlag With {.name = "FileRefId", .flag = True}})
    End Sub

    Public Overrides Function InitialModel() As ViewPart
        Dim SSQL As String = " Select  BltnId, BltnTitle, BltnMemo, CreatedBy, dbo.XF_FileList(FileRefId) Files, FileRefId, SYSDTE, SYSUSR  " +
                             " from XysBulletin where BltnId = " + PartialData

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
        label.Wrap.InnerText = If(ViewPart.Mode = ViewMode.New, Translator.Format("newnotice"), Translator.Format("editnotice"))

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px")
        filter.Wrap.SetStyle(HtmlStyles.width, "90%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText

        Dim text As New Texts(Translator.Format("title"), ViewPart.Field("BltnTitle").name, TextTypes.text)
        text.Required = True
        text.Text.SetStyle(HtmlStyles.width, "60%")
        text.Text.SetAttribute(HtmlAttributes.maxlength, "200")
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("BltnTitle").value)
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")
        text.Wrap.SetStyle(HtmlStyles.display, "block")

        Dim text1 As New TextArea(Translator.Format("memo"))
        text1.Required = True
        text1.Text.SetStyle(HtmlStyles.width, "90%")
        text1.Text.SetStyle(HtmlStyles.height, "300px")
        text1.Text.SetAttribute(HtmlAttributes.id, ViewPart.Field("BltnMemo").name)
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "500")
        text1.Text.InnerText = ViewPart.Field("BltnMemo").value
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text2 As New Texts(Translator.Format("createdby"), ViewPart.Field("CreatedBy").name, TextTypes.text)
        text2.Required = True
        text2.Text.SetStyle(HtmlStyles.width, "50%")
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "200")
        text2.Text.SetAttribute(HtmlAttributes.value, If(ViewPart.Mode = ViewMode.New, AppKey.UserName, ViewPart.Field("CreatedBy").value))
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")
        text2.Wrap.SetStyle(HtmlStyles.display, "block")

        Dim fileRef As New FileUpload()
        fileRef.File.SetAttribute(HtmlAttributes.id, ViewPart.Field("Files").name)
        fileRef.Wrap.SetAttribute(HtmlAttributes.class, "__filebtn")
        fileRef.Button.SetStyles("padding: 8px; border-radius: 4px; border: 1px solid rgb(68, 68, 68); border-image: none; color: rgb(68, 68, 68); font-size: 12px; cursor: pointer; background-color: rgb(255, 255, 255);")
        fileRef.Label.SetStyle(HtmlStyles.paddingLeft, "10px")
        fileRef.Label.InnerText = ViewPart.Field("Files").value

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
        elmBox.AddItem(fileRef, 20)
        elmBox.AddItem(BtnWrap, 20)

        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText
        Return ViewHtml
    End Function

    Public Function SaveView() As ApiResponse
        Dim BltnTitle As String = ViewPart.Field("BltnTitle").value
        Dim BltnMemo As String = ViewPart.Field("BltnMemo").value
        Dim CreatedBy As String = ViewPart.Field("CreatedBy").value

        If ViewPart.Mode = ViewMode.New Then ViewPart.Field("FileRefId").value = NewID(1)

        Dim _ApiResponse As New ApiResponse
        If BltnTitle = String.Empty OrElse BltnMemo = String.Empty OrElse createdby = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_required"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Else
            Dim rlt As String = PutSaveView()
            If rlt = String.Empty Then
                rlt = UploadFile(ViewPart.Field("Files").name, ViewPart.Field("FileRefId").value)
                If rlt = String.Empty Then
                    Dim dialogBox As New DialogBox(Translator.Format("msg_saved"))
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                    dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysBulletinMV) + "&$PopOff();")
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
                Else
                    Dim dialogBox As New DialogBox(rlt)
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
                End If
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
                SQL.Add(" declare @seq int " +
                        " exec dbo.XP_NextSeq N'XysBulletin',N'BltnId',@seq out " +
                        " insert into XysBulletin(BltnId, BltnTitle, BltnMemo, CreatedBy, FileRefId, SYSDTE, SYSUSR) " +
                        " values(@seq,@BltnTitle,@BltnMemo,@CreatedBy,@FileRefId,getdate(),@SYSUSR)    ")
            Case ViewMode.Edit
                SQL.Add(" Update XysBulletin set " +
                        "   BltnTitle = @BltnTitle, BltnMemo = @BltnMemo, CreatedBy = @CreatedBy, " +
                        "   FileRefId = (case when @FileRefId='' then FileRefId else @FileRefId end), " +
                        "   SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " Where  BltnId = @BltnId  ")
        End Select

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@BltnId", .Value = Val(ViewPart.Field("BltnId").value).ToString, .SqlDbType = Data.SqlDbType.Int})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@BltnTitle", .Value = ViewPart.Field("BltnTitle").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@BltnMemo", .Value = ViewPart.Field("BltnMemo").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@CreatedBy", .Value = ViewPart.Field("CreatedBy").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@FileRefId", .Value = ViewPart.Field("FileRefId").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SYSUSR", .Value = AppKey.UserId, .SqlDbType = Data.SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

    Public Function DeleteView() As ApiResponse
        'Dim keyVlus As List(Of KeyVlu) = DecryptCallAction()

        Dim _ApiResponse As New ApiResponse

        Dim dialogBox As New DialogBox(Translator.Format("deletepage"))
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;")
        dialogBox.AddButton(Translator.Format("yes"), String.Empty, "class:button1;onclick:" + ByPassCall("XysBulletinEV/DeleteViewConfirm"))
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
            dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysBulletin) + "&$PopOff();")
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
            " delete from XysBulletin where BltnId = @BltnId "
            }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@BltnId", .Value = Val(ViewPart.Field("BltnId").value).ToString, .SqlDbType = Data.SqlDbType.Int})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

End Class
