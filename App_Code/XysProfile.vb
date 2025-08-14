Imports Microsoft.VisualBasic
Imports skylite
Imports skylite.ToolKit
Imports System.Data
Imports System.IO

Public Class XysProfile
    Inherits WebBase

    Sub New()
        ViewFields.AddRange({New NameFlag With {.name = "UserEmail", .flag = True},
                             New NameFlag With {.name = "UserName"},
                             New NameFlag With {.name = "UserPhone"},
                             New NameFlag With {.name = "UserRole", .flag = True}})
    End Sub

    Public Overrides Function InitialModel() As ViewPart
        Dim SSQL As String = " select UserEmail,UserName,UserPhone,dbo.XF_RoleName(RoleId) UserRole from XysUserInfo where UserId = '" + AppKey.UserId + "'"

        ViewPart = New ViewPart
        ViewPart.Methods = ViewMethods()
        ViewPart.Mode = ViewMode.Edit
        ViewPart.Data = SQLData.SQLDataTable(SSQL)

        For i As Integer = 0 To ViewFields.Count - 1
            ViewPart.Fields.Add(New NameValue With {.name = ViewFields(i).name, .value = ViewPart.ColunmValue(.name)})
        Next

        Return ViewPart
    End Function

    Public Overrides Function InitialView() As String
        Dim mnulist As MenuList = SetPageMenu({})
        Dim BtnWrap As Wrap = SetPageButtons({})

        Dim label As New Label
        label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;")
        label.Wrap.InnerText = Translator.Format("profile")

        Dim filter As New ToolKit.FilterSection()
        filter.ModalWrap = True
        filter.Wrap.SetStyle(HtmlStyles.marginTop, "100px")
        filter.Wrap.SetStyle(HtmlStyles.width, "95%")
        filter.Menu = mnulist
        filter.FilterHtml = label.HtmlText


        Dim txtid As New Label
        txtid.Wrap.InnerText = Translator.Format("email") + "&nbsp;" + ViewPart.Field("UserEmail").value
        txtid.Wrap.SetStyle(HtmlStyles.fontSize, "18px")
        txtid.Wrap.SetStyle(HtmlStyles.fontWeight, "bold")
        txtid.Wrap.SetStyle(HtmlStyles.color, "#444")
        txtid.Wrap.SetStyle(HtmlStyles.marginLeft, "8px")

        Dim txtrole As New Label
        txtrole.Wrap.InnerText = Translator.Format("userrole") + "&nbsp;" + ViewPart.Field("UserRole").value
        txtrole.Wrap.SetStyle(HtmlStyles.fontSize, "18px")
        txtrole.Wrap.SetStyle(HtmlStyles.fontWeight, "bold")
        txtrole.Wrap.SetStyle(HtmlStyles.color, "#444")
        txtrole.Wrap.SetStyle(HtmlStyles.marginLeft, "8px")

        Dim text As New Texts(Translator.Format("name"), ViewPart.Field("UserName").name, TextTypes.text)
        text.Required = True
        text.Text.SetStyle(HtmlStyles.width, "200px")
        text.Text.SetAttribute(HtmlAttributes.maxlength, "100")
        text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserName").value)
        text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim text1 As New Texts(Translator.Format("phone"), ViewPart.Field("UserPhone").name, TextTypes.text)
        text1.Text.SetStyle(HtmlStyles.width, "200px")
        text1.Text.SetAttribute(HtmlAttributes.maxlength, "15")
        text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserPhone").value)
        text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px")

        Dim elmWrap As New HtmlWrapper
        elmWrap.AddContents(txtrole, 10)
        elmWrap.AddContents(txtid, 30)
        elmWrap.AddContents(text, 1)
        elmWrap.AddContents(text1, 46)
        elmWrap.AddContents(BtnWrap)

        Dim col As New Wrap
        col.SetStyle(HtmlStyles.marginLeft, "40px")
        col.InnerText = elmWrap.HtmlText

        Dim imgfile As String = PhysicalFolder + "photos\" + AppKey.UserId + ".jpg"
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
        imginput.SetAttribute(HtmlEvents.onchange, "UpdatePhoto('UserPic','UserFile','" + EncryptString(AppKey.UserId) + "')")
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
        colWrap.SetStyle(HtmlStyles.width, "95%")
        colWrap.SetStyle(HtmlStyles.margin, "auto")
        colWrap.SetStyle(HtmlStyles.marginTop, "10px")
        colWrap.SetStyle(HtmlStyles.marginBottom, "80px")
        colWrap.SetStyle(HtmlStyles.borderRadius, "2px")
        colWrap.SetStyle(HtmlStyles.border, "1px solid #ddd")
        colWrap.SetStyle(HtmlStyles.boxShadow, "3px 4px 6px 1px rgba(0, 0, 0, 0.15)")
        colWrap.SetStyle(HtmlStyles.boxSizing, "border-box")
        colWrap.SetStyle(HtmlStyles.padding, "30px")
        colWrap.SetStyle(HtmlStyles.display, "flex")
        colWrap.InnerText = col1.HtmlText + col.HtmlText

        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript)
        HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript)
        HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')")

        Dim PageLayout As TitleSection2 = PageTitle()
        PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents)
        PageLayout.ContentWrap.InnerText = filter.HtmlText + colWrap.HtmlText

        Return PageLayout.HtmlText
    End Function

    Public Function MenuClick() As ApiResponse
        Dim m As String = GetDataValue("m")
        Dim t As String = GetDataValue("t")

        Dim _ApiResponse As New ApiResponse
        _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(m, t))
        _ApiResponse.ExecuteScript("$ScrollToTop()")
        Return _ApiResponse
    End Function
    Public Function NaviClick() As ApiResponse
        Dim m As String = GetDataValue("m")
        Dim _ApiResponse As New ApiResponse
        _ApiResponse.Navigate(m)
        Return _ApiResponse
    End Function
    Public Function SaveView() As ApiResponse
        Dim UserName As String = ViewPart.Field("UserName").value
        Dim UserPhone As String = ViewPart.Field("UserPhone").value

        Dim _ApiResponse As New ApiResponse
        If UserName = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("msg_required"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        Else
            Dim rlt As String = PutSaveView()
            If rlt = String.Empty Then
                AppKey.UserName = UserName
                AppKey.UserPhone = UserPhone
                Dim SerializedString As String = SerializeObjectEnc(AppKey, GetType(AppKey))
                _ApiResponse.SetCookie(References.Keys.AppKey, SerializedString)

                Dim dialogBox As New DialogBox(Translator.Format("msg_saved"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
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
        Dim SQL As New List(Of String) From _
            {
            " update XysUserInfo set UserName = @UserName,UserPhone=@UserPhone where UserId = @UserId " +
            " update XysUser set SYSDTE = getdate(),SYSUSR=@UserId where UserId = @UserId "
            }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserId", .Value = AppKey.UserId, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserName", .Value = ViewPart.Field("UserName").value, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserPhone", .Value = ViewPart.Field("UserPhone").value, .SqlDbType = SqlDbType.NVarChar})

        Return PutData(SqlWithParams(SQL, SqlParams))
    End Function

End Class
