Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data
Imports System.Runtime.Serialization.Json
Imports System.Net


Public Class XysRoleEV
    Inherits WebBase

    Sub New()
        ViewFields.AddRange({New NameFlag With {.name = "RoleId", .flag = True},
                             New NameFlag With {.name = "RoleName"},
                             New NameFlag With {.name = "RoleAlias"},
                             New NameFlag With {.name = "RoleOrder"}})
    End Sub
    Public Overrides Function InitialModel() As ViewPart

        ViewPart = New ViewPart
        ViewPart.Methods = ViewMethods()
        If PartialData <> String.Empty Then
            ViewPart.Mode = ViewMode.Edit
            ViewPart.Data = SQLData.SQLDataTable(" Select RoleId,RoleName,RoleAlias,RoleOrder,SYSDTE,SYSUSR From XysRole Where  RoleId = '" + PartialData + "'")
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
        Dim mnulist As MenuList = SetPageMenu(If(ViewPart.Mode = ViewMode.New, {"xysrole"}, {}))
        Dim BtnWrap As Wrap = SetPageButtons(If(ViewPart.Mode = ViewMode.New, {"save"}, {}))
 
        Dim label As New Label
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;")
        label.Wrap.InnerText = If(ViewPart.Mode = ViewMode.New, Translator.Format("newrole"), Translator.Format("editrole"))

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px")
        filter.Wrap.SetStyle(HtmlStyles.width, "90%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText
         

        Dim text As New Texts(Translator.Format("name"), ViewPart.Field("RoleName").name, TextTypes.text)
        text.Required = True
        text.Text.SetStyle(HtmlStyles.width, "200px")
        text.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("RoleName").value)
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text1 As New Texts(Translator.Format("alias"), ViewPart.Field("RoleAlias").name, TextTypes.text)
        text1.Required = True
        text1.Text.SetStyle(HtmlStyles.width, "200px")
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "10")
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("RoleAlias").value)
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim sel1 As New Dropdown(Translator.Format("order"), ViewPart.Field("RoleOrder").name)
        sel1.Required = True
        sel1.SelBox.SetStyle(HtmlStyles.width, "216px")
        sel1.SelOptions = New OptionValues("{0|0}{1|1}{2|2}{3|3}{4|4}{5|5}{6|6}{7|7}{8|8}{9|9}{10|10}", ViewPart.Field("RoleOrder").value)
        sel1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")
         

        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.width, "90%")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.marginTop, "8px")
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px")
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px")
         
        elmBox.AddItem(text, 1)
        elmBox.AddItem(text1, 20)
        elmBox.AddItem(sel1, 40)
        elmBox.AddItem(BtnWrap, 20)

        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText
        Return ViewHtml
    End Function

    Public Function SaveView() As ApiResponse
        Dim rolename As String = ViewPart.Field("RoleName").value
        Dim rolealias As String = ViewPart.Field("RoleAlias").value
        Dim roleorder As String = ViewPart.Field("RoleOrder").value

        Dim _ApiResponse As New ApiResponse
        If rolename = String.Empty OrElse rolealias = String.Empty OrElse roleorder = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_required"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Else
            Dim rlt As String = PutSaveView()
            If rlt = String.Empty Then
                Dim dialogBox As New DialogBox(Translator.Format("msg_saved"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysRole) + "&$PopOff();")
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
                ViewPart.Field("RoleId").value = NewID()
                SQL.Add(" Insert into XysRole( RoleId,RoleName,RoleAlias,RoleOrder,SYSDTE,SYSUSR) " +
                        " values ( @RoleId, @RoleName, @RoleAlias, @RoleOrder, getdate(), @SYSUSR) ")
            Case ViewMode.Edit
                SQL.Add(" Update XysRole set " +
                        " RoleName = @RoleName, RoleAlias = @RoleAlias, RoleOrder = @RoleOrder, " +
                        " SYSDTE = getdate(), SYSUSR = @SYSUSR Where RoleId = @RoleId ")
        End Select

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@RoleId", .Value = ViewPart.Field("RoleId").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@RoleName", .Value = ViewPart.Field("RoleName").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@RoleAlias", .Value = ViewPart.Field("RoleAlias").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@RoleOrder", .Value = ViewPart.Field("RoleOrder").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SYSUSR", .Value = AppKey.UserId, .SqlDbType = SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

    Public Function DeleteView() As ApiResponse
        Dim _ApiResponse As New ApiResponse

        If IfExists() = True Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_exists"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Else
            Dim dialogBox As New DialogBox(Translator.Format("deleterole"))
            dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;")
            dialogBox.AddButton(Translator.Format("yes"), String.Empty, "class:button1;onclick:" + ByPassCall("XysRoleEV/DeleteViewConfirm"))
            dialogBox.AddButton(Translator.Format("no"), String.Empty, "onclick:$PopOff();class:button;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        End If
        Return _ApiResponse
    End Function

    Private Function IfExists() As Boolean
        Dim rtnvlu As Boolean = False
        Dim dt As DataTable = SQLData.SQLDataTable(" select count(*) from XysUserInfo where RoleId = '" + ViewPart.Field("RoleId").value + "'")
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            If Val(dt.Rows(0)(0).ToString) > 0 Then
                rtnvlu = True
            End If
        End If
        Return rtnvlu
    End Function
    Public Function DeleteViewConfirm() As ApiResponse
        Dim _ApiResponse As New ApiResponse
        Dim rlt As String = PutDeleteView()
        If rlt = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_deleted"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysRole) + "&$PopOff();")
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
            " delete from XysRoleMenu where RoleId = @RoleId ",
            " delete from XysRolePage where RoleId = @RoleId ",
            " delete from XysRole where RoleId = @RoleId "
            }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@RoleId", .Value = ViewPart.Field("RoleId").value, .SqlDbType = SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function


End Class
