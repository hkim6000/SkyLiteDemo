Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data

Public Class XysPermission
    Inherits WebBase

    Sub New()
        ViewFields.AddRange({New NameFlag With {.name = "RoleName", .flag = True}})
    End Sub

    Public Overrides Function InitialModel() As ViewPart
        Dim SSQL As String = " Select RoleName From XysRole Where  RoleId = '" + PartialData + "'"

        ViewPart = New ViewPart
        ViewPart.Methods = ViewMethods()
        ViewPart.Mode = ViewMode.Edit
        ViewPart.Params = PartialData
        ViewPart.Data = SQLData.SQLDataTable(SSQL)

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
        label.Wrap.InnerText = Translator.Format("rolepermission")

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px")
        filter.Wrap.SetStyle(HtmlStyles.width, "90%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText


        Dim Switch As Wrap = SwitchUI()
        Switch.SetStyle(HtmlStyles.marginLeft, "8px")

        Dim elmBox As New HtmlElementBox
        elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox)
        elmBox.SetStyle(HtmlStyles.width, "90%")
        elmBox.SetStyle(HtmlStyles.margin, "auto")
        elmBox.SetStyle(HtmlStyles.marginTop, "8px")
        elmBox.SetStyle(HtmlStyles.marginBottom, "80px")
        elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px")
        elmBox.Wrap.SetStyle(HtmlStyles.overflow, "auto")

        elmBox.AddItem(Switch, 1)

        Dim ViewHtml As String = filter.HtmlText + elmBox.HtmlText
        Return ViewHtml
    End Function
    Private Function SwitchUI() As Wrap
        Dim roleid As String = ViewPart.Params
        Dim rolename As String = SQLData.SQLFieldValue("SELECT dbo.XF_RoleName(N'" + roleid + "')")

        Dim _Wrap As New Wrap
        _Wrap.SetStyle(HtmlStyles.margin, "auto")
        _Wrap.SetStyle(HtmlStyles.marginTop, "30px")
        _Wrap.SetStyle(HtmlStyles.marginBottom, "30px")

        Dim rlt As String = String.Empty
        Dim sSql As String = " declare @RoleId NVARCHAR(50); set @RoleId = N'" + roleid + "'; " +
                             " select t1, case when t2='' and t3 <> '' then 'Common' else t2 end as t2,t3,t4,tck,tint from dbo.XF_RolePermission(@RoleId) order by tord;"
        Dim dt As DataTable = SQLData.SQLDataTable(sSql, rlt)
        If rlt = String.Empty Then
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim HtmlTranslator As New Translator

                Dim _tablex As New Grid
                _tablex.Table.SetAttribute(HtmlAttributes.class, "table")
                _tablex.Table.SetStyle(HtmlStyles.fontSize, "14px")
                _tablex.Table.SetStyle(HtmlStyles.width, String.Empty)
                _tablex.DataSource(dt)

                For i As Integer = 0 To _tablex.Headers.Count - 1
                    Select Case i
                        Case 0, 5
                            _tablex.Headers(i).SetStyle(HtmlStyles.display, "none")
                            _tablex.SetColumnStyle(i, HtmlStyles.display, "none")
                        Case 1
                            _tablex.Headers(i).InnerText = Translator.Format("area")
                            _tablex.SetColumnStyle(i, HtmlStyles.textAlign, "center")
                            _tablex.SetColumnStyle(i, HtmlStyles.fontWeight, "bold")
                        Case 2
                            _tablex.Headers(i).InnerText = Translator.Format("page")
                            _tablex.SetColumnStyle(i, HtmlStyles.textAlign, "left")
                            _tablex.SetColumnStyle(i, HtmlStyles.fontWeight, "bold")
                        Case 3
                            _tablex.Headers(i).InnerText = Translator.Format("function")
                            _tablex.SetColumnStyle(i, HtmlStyles.textAlign, "left")
                        Case 4
                            _tablex.Headers(i).InnerText = Translator.Format("status")
                            _tablex.SetColumnStyle(i, HtmlStyles.textAlign, "center")
                    End Select
                    _tablex.SetColumnStyle(i, HtmlStyles.whiteSpace, "nowrap")

                Next

                For i As Integer = 0 To dt.Rows.Count - 1
                    Dim rowid As String = dt.Rows(i)(0).ToString
                    Dim rowstatus As String = dt.Rows(i)(4).ToString
                    Dim rowtype As String = dt.Rows(i)(5).ToString

                    For j As Integer = 0 To dt.Columns.Count - 1
                        If j = 4 Then
                            Dim jsEvent As String = "SetRoleRange('XysPermission/SetRoleRange','" + roleid + "','" + rowid + "','" + rowtype + "',this.checked)"

                            Dim Switch As New Switch
                            Switch.Id = rowid
                            Switch.Name = "switch"
                            Switch.Attributes = "onclick:" + jsEvent
                            Switch.Size = 50
                            If Val(rowstatus) = 1 Then Switch.Checked = True

                            _tablex.Rows(i).Columns(j).InnerText = Switch.HtmlText
                        End If

                    Next
                Next
                _Wrap.InnerText += _tablex.HtmlText
            End If
        End If

        Return _Wrap
    End Function

    Public Function SetRoleRange() As ApiResponse
        Dim roleid As String = GetDataValue("d")
        Dim key As String = GetDataValue("c")
        Dim keytyp As String = GetDataValue("t")
        Dim keyvlu As String = GetDataValue("s")

        Dim SQL As New List(Of String)
        SQL.Add(" DECLARE @RoleId NVARCHAR(50),@PageId NVARCHAR(50),@Key NVARCHAR(50),@KeyTyp INT,@KeyVlu INT,@SYSDTE datetime,@SYSUSR nvarchar(100) " +
                 " SET @RoleId = N'" + roleid + "' " +
                 " SET @PageId = N'' " +
                 " SET @Key = N'" + key + "' " +
                 " SET @KeyTyp = " + Val(keytyp).ToString + " " +
                 " SET @KeyVlu = " + Val(keyvlu).ToString + " " +
                 " SET @SYSDTE = getdate() " +
                 " SET @SYSUSR = N'" + AppKey.UserId + "' " +
                 " IF @KeyTyp = 0 " +
                 " BEGIN " +
                 "      DELETE FROM XysRolePage WHERE RoleId=@RoleId AND PageId =@Key " +
                 "      if @KeyVlu = 1 " +
                 "      begin " +
                 "          INSERT INTO XysRolePage(RoleId,PageId,SYSDTE,SYSUSR) values(@RoleId,@Key,@SYSDTE,@SYSUSR) " +
                 "      end " +
                 "      else " +
                 "      begin " +
                 "          DELETE FROM XysRoleMenu WHERE RoleId=@RoleId AND MenuId in (select MenuId from XysMenu where PageId =@Key)  " +
                 "      end " +
                 " END " +
                 " ELSE " +
                 " BEGIN " +
                 "      SET @PageId = isnull((select PageId from XysMenu where MenuId = @Key),'') " +
                 "      DELETE FROM XysRoleMenu WHERE RoleId=@RoleId AND MenuId =@Key " +
                 "      if @KeyVlu = 1 " +
                 "      begin " +
                 "          INSERT INTO XysRoleMenu(RoleId,MenuId,SYSDTE,SYSUSR) values(@RoleId,@Key,@SYSDTE,@SYSUSR) " +
                 "          if not exists(select * from XysRolePage where RoleId=@RoleId and PageId = (select PageId from XysMenu where MenuId = @Key))  " +
                 "          begin " +
                 "              if @PageId <> '' " +
                 "              begin " +
                 "                  INSERT INTO XysRolePage(RoleId,PageId,SYSDTE,SYSUSR) values(@RoleId,@PageId,@SYSDTE,@SYSUSR) " +
                 "              end " +
                 "          end " +
                 "      end " +
                 "      else " +
                 "      begin " +
                 "          if @PageId <> '' " +
                 "          begin " +
                 "              if not exists(select * from XysRoleMenu where RoleId=@RoleId and MenuId in (select MenuId from XysMenu where PageId = @PageId))  " +
                 "              begin " +
                 "                  delete from XysRolePage where RoleId=@RoleId and PageId=@PageId " +
                 "              end " +
                 "          end " +
                 "      end " +
                 " END ")

        Dim rlt As String = PutData(SQL)

        Dim _ApiResponse As New ApiResponse
        If rlt <> String.Empty Then
            Dim dialogBox As New DialogBox(rlt)
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        End If

        Dim SSQL As String = String.Empty
        If Val(keytyp) = 0 Then
            SSQL = _
                " DECLARE @RoleId NVARCHAR(50) " +
                " SET @RoleId = N'" + roleid + "' " +
                " select t1,tck from dbo.XF_RolePermission(@RoleId) where t1 in " +
                " ( select PageId from XysPage where PageId = N'" + key + "' " +
                "   union all " +
                "   select MenuId from XysMenu where PageId = N'" + key + "' ) "
        Else
            SSQL = _
                " DECLARE @RoleId NVARCHAR(50) " +
                " SET @RoleId = N'" + roleid + "' " +
                " select t1,tck from dbo.XF_RolePermission(@RoleId) where t1 in " +
                " ( select PageId from XysPage where PageId = (select PageId from XysMenu where MenuId = N'" + key + "') " +
                "   union all " +
                "   select MenuId from XysMenu where MenuId = N'" + key + "' ) "
        End If
        Dim dt As DataTable = SQLData.SQLDataTable(SSQL, rlt)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Dim script As String = " var elem = document.getElementsByName('switch'); "
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim tid As String = dt.Rows(i)(0).ToString
                Dim tbool As String = If(Val(dt.Rows(i)(1).ToString) = False, "false", "true")
                script += _
                    " for (var i = 0, j = elem.length; i < j; i++) { " +
                    "    if (elem[i].id == '" + tid + "'){ " +
                    "       elem[i].checked = " + tbool + "; " +
                    "    } " +
                    " } "
            Next

            _ApiResponse.ExecuteScript(script)
        End If

        Return _ApiResponse
    End Function
    
    Public Function ItemSelected() As ApiResponse
        Dim t As String = GetDataValue("t")

        Dim _ApiResponse As New ApiResponse
        Dim Html As HtmlDocument = PartialDocument(References.Pages.XysRoleEV, t)
        _ApiResponse.SetElementContents(References.Elements.PageContents, Html)
        Return _ApiResponse
    End Function

End Class
