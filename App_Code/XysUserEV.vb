Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data
Imports System.Runtime.Serialization.Json
Imports System.Net
Imports System.IO

Public Class XysUserEV
    Inherits WebBase

    Sub New()
        ViewFields.AddRange({New NameFlag With {.name = "UserId", .flag = True},
                             New NameFlag With {.name = "UserPwd"},
                             New NameFlag With {.name = "UserOTP"},
                             New NameFlag With {.name = "UserStatus"},
                             New NameFlag With {.name = "UserEmail"},
                             New NameFlag With {.name = "UserName"},
                             New NameFlag With {.name = "UserPhone"},
                             New NameFlag With {.name = "RoleId"}})
    End Sub

    Public Overrides Function InitialModel() As ViewPart
        Dim SSQL As String = " select a.UserId,a.UserPwd,a.UserOTP, a.UserStatus, b.UserEmail,b.UserName,b.UserPhone,b.RoleId " +
                             " from XysUser a inner join XysUserInfo b on a.UserId = b.UserId " +
                             " where a.UserId = N'" + PartialData + "'"

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

        ViewPart.Field("UserPwd").value = Encryptor.DecryptData(ViewPart.Field("UserPwd").value)

        Return ViewPart
    End Function

    Public Overrides Function InitialView() As String
        Dim mnulist As MenuList = SetPageMenu({})
        Dim BtnWrap As Wrap = SetPageButtons(If(ViewPart.Mode = ViewMode.New, {"save"}, {}))

        Dim label As New Label
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;")
        label.Wrap.InnerText = If(ViewPart.Mode = ViewMode.New, Translator.Format("newuser"), Translator.Format("edituser"))

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px")
        filter.Wrap.SetStyle(HtmlStyles.width, "90%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText


        Dim text As New Texts(Translator.Format("email"), ViewPart.Field("UserEmail").name, TextTypes.text)
        text.Required = True
        text.Text.SetStyle(HtmlStyles.width, "300px")
        text.Text.SetAttribute(HtmlAttributes.maxlength, "200")
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserEmail").value)
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text1 As New Texts(Translator.Format("name"), ViewPart.Field("UserName").name, TextTypes.text)
        text1.Required = True
        text1.Text.SetStyle(HtmlStyles.width, "200px")
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "10")
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserName").value)
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text2 As New Texts(Translator.Format("phone"), ViewPart.Field("UserPhone").name, TextTypes.text)
        text2.Text.SetStyle(HtmlStyles.width, "200px")
        text2.Text.SetAttribute(HtmlAttributes.maxlength, "16")
        text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserPhone").value)
        text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim sel1 As New Dropdown(Translator.Format("role"), ViewPart.Field("RoleId").name)
        sel1.Required = True
        sel1.SelBox.SetStyle(HtmlStyles.width, "216px")
        sel1.SelOptions = New OptionValues("sql@select RoleId,RoleName from XysRole order by RoleOrder", ViewPart.Field("RoleId").value)
        sel1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text3 As New Texts(Translator.Format("pwd"), ViewPart.Field("UserPwd").name, TextTypes.text)
        text3.Required = True
        text3.Text.SetStyle(HtmlStyles.width, "200px")
        text3.Text.SetAttribute(HtmlAttributes.maxlength, "32")
        text3.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserPwd").value)
        text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim chk1 As New CheckBox(Translator.Format("mfa"))
        chk1.Checks.AddItem(ViewPart.Field("UserOTP").name, "1", String.Empty, If(Val(ViewPart.Field("UserOTP").value) = 1, True, False))
        chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim sel2 As New Dropdown(Translator.Format("status"), ViewPart.Field("UserStatus").name)
        sel2.SelBox.SetStyle(HtmlStyles.width, "216px")
        sel2.SelOptions = New OptionValues("{0|Normal}{8|Suspended}{9|Terminated}", ViewPart.Field("UserStatus").value)
        sel2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim elmWrap As New HtmlWrapper
        elmWrap.AddContents(text, 1)
        elmWrap.AddContents(text1, 1)
        elmWrap.AddContents(text2, 10)
        elmWrap.AddContents(sel1, 1)
        elmWrap.AddContents(text3, 10)
        elmWrap.AddContents(chk1, 10)
        elmWrap.AddContents(sel2, 46)
        elmWrap.AddContents(BtnWrap, 20)


        Dim col As New Wrap
        col.SetStyle(HtmlStyles.marginLeft, "40px")
        col.InnerText = elmWrap.HtmlText

        Dim imgfile As String = PhysicalFolder + "photos\" + ViewPart.Field("UserId").value + ".jpg"
        If File.Exists(imgfile) = False Then
            imgfile = ImagePath + "img_fakeuser.jpg"
        Else
            imgfile = GetPhotoData(imgfile)
        End If
        Dim img As New HtmlTag(HtmlTags.img, HtmlTag.Types.Empty)
        img.SetAttribute(HtmlAttributes.id, "UserPic")
        img.SetAttribute(HtmlAttributes.src, imgfile)
        img.SetStyle(HtmlStyles.borderRadius, "6px")
        img.SetStyle(HtmlStyles.width, "220px")
        img.SetStyle(HtmlStyles.height, "250px")
        img.SetStyle(HtmlStyles.objectFit, "cover")

        Dim imginput As New HtmlTag(HtmlTags.input, HtmlTag.Types.Empty)
        imginput.SetAttribute(HtmlAttributes.id, "UserFile")
        imginput.SetAttribute(HtmlAttributes.type, "file")
        imginput.SetAttribute(HtmlEvents.onchange, "UpdatePhoto('UserPic','UserFile','" + EncryptString(ViewPart.Field("UserId").value) + "')")
        imginput.SetStyles("left: 0px; top: 0px; width: 100%; height: 100%; color: transparent; position: absolute; cursor: pointer; opacity: 0;")
        Dim imgbtnwrap As New HtmlTag()
        imgbtnwrap.SetStyles("overflow: hidden; display: inline-block; border-radius:4px; width:36px; height:36px; position:absolute; bottom:0px;right:-2px;background-repeat: no-repeat;background-size: contain;")
        imgbtnwrap.SetStyle(HtmlStyles.backgroundImage, "url('" + ImagePath + "changephoto.jpg')")
        imgbtnwrap.InnerText = imginput.HtmlText

        Dim imgWrap As New Wrap
        imgWrap.SetStyle(HtmlStyles.position, "relative")
        imgWrap.SetStyle(HtmlStyles.width, "220px")
        imgWrap.SetStyle(HtmlStyles.height, "250px")
        imgWrap.InnerText = img.HtmlText + imgbtnwrap.HtmlText

        Dim col1 As New Wrap
        col1.SetStyle(HtmlStyles.padding, "20px")
        col1.InnerText = imgWrap.HtmlText

        Dim colWrap As New Wrap
        colWrap.SetStyle(HtmlStyles.width, "90%")
        colWrap.SetStyle(HtmlStyles.margin, "auto")
        colWrap.SetStyle(HtmlStyles.marginTop, "10px")
        colWrap.SetStyle(HtmlStyles.marginBottom, "80px")
        colWrap.SetStyle(HtmlStyles.borderRadius, "2px")
        colWrap.SetStyle(HtmlStyles.border, "1px solid #ddd")
        colWrap.SetStyle(HtmlStyles.boxShadow, "3px 4px 6px 1px rgba(0, 0, 0, 0.15)")
        colWrap.SetStyle(HtmlStyles.boxSizing, "border-box")
        colWrap.SetStyle(HtmlStyles.padding, "30px")
        colWrap.SetStyle(HtmlStyles.display, "flex")
        colWrap.InnerText = If(ViewPart.Mode = ViewMode.New, String.Empty, col1.HtmlText) + col.HtmlText

        Dim ViewHtml As String = filter.HtmlText + colWrap.HtmlText
        Return ViewHtml
    End Function

    Public Function SaveView() As ApiResponse
        Dim UserOTP As String = ViewPart.Field("UserOTP").value
        Dim UserPwd As String = ViewPart.Field("UserPwd").value
        Dim UserEmail As String = ViewPart.Field("UserEmail").value
        Dim UserName As String = ViewPart.Field("UserName").value
        Dim UserPhone As String = ViewPart.Field("UserPhone").value
        Dim RoleId As String = ViewPart.Field("RoleId").value

        Dim _ApiResponse As New ApiResponse
        If UserPwd = String.Empty OrElse UserEmail = String.Empty OrElse UserName = String.Empty OrElse RoleId = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_required"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Else
            Dim rlt As String = PutSaveView()
            If rlt = String.Empty Then
                Dim dialogBox As New DialogBox(Translator.Format("msg_saved"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysUser) + "&$PopOff();")
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
                ViewPart.Field("UserId").value = NewID()
                SQL.Add(" Insert into XysUser( UserId,UserPwd,UserOTP,UserStatus,PassChanged,Created,Closed,SYSDTE,SYSUSR) " +
                        " values ( @UserId, @UserPwd, @UserOTP, @UserStatus, getdate(), getdate(), getdate(), getdate(), @SYSUSR) ")
                SQL.Add(" Insert into XysUserInfo( UserId,UserName,UserDesc,UserEmail,UserPhone,UserPic,RoleId,UserRef) " +
                        " values ( @UserId, @UserName, N'', @UserEmail, @UserPhone, N'', @RoleId, N'') ")
            Case ViewMode.Edit
                SQL.Add(" update XysUserInfo set UserEmail = @UserEmail,UserName = @UserName,UserPhone=@UserPhone,RoleId=@RoleId where UserId = @UserId ")
                SQL.Add(" update XysUser set UserPwd=@UserPwd,UserOTP=@UserOTP,UserStatus=@UserStatus, SYSDTE = getdate(),SYSUSR=@SYSUSR where UserId = @UserId ")
        End Select


        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserId", .Value = ViewPart.Field("UserId").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserPwd", .Value = Encryptor.EncryptData(ViewPart.Field("UserPwd").value), .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserOTP", .Value = Val(ViewPart.Field("UserOTP").value).ToString, .SqlDbType = SqlDbType.Int})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserStatus", .Value = Val(ViewPart.Field("UserStatus").value).ToString, .SqlDbType = SqlDbType.Int})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserEmail", .Value = ViewPart.Field("UserEmail").value, .SqlDbType = Data.SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserName", .Value = ViewPart.Field("UserName").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserPhone", .Value = ViewPart.Field("UserPhone").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@RoleId", .Value = ViewPart.Field("RoleId").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@SYSUSR", .Value = AppKey.UserId, .SqlDbType = Data.SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

    Public Function DeleteView() As ApiResponse
        Dim _ApiResponse As New ApiResponse

        Dim dialogBox As New DialogBox(Translator.Format("deleteuser"))
        dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;")
        dialogBox.AddButton(Translator.Format("yes"), String.Empty, "class:button1;onclick:" + ByPassCall("XysUserEV/DeleteViewConfirm"))
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
            dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button;onclick:" + ByPassCall("MenuClick", "m=" + References.Pages.XysUser) + "&$PopOff();")
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
            " delete from XysUserReset where UserId = @UserId ",
            " delete from XysUserInfo where UserId = @UserId ",
            " delete from XysUser where UserId = @UserId "
            }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserId", .Value = ViewPart.Field("UserId").value, .SqlDbType = SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

End Class
