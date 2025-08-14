Imports Microsoft.VisualBasic
Imports skylite
Imports System.Data
Imports skylite.ToolKit
Imports System.IO
Imports System.Drawing

Public Structure References
    Const ProjectTitle As String = "SkyLite"

    Public Structure Htmls
        Const Email_SignUp As String = "Email_SignUp"
        Const Email_PassReset As String = "Email_PassReset"
        Const Email_PassChanged As String = "Email_PassChanged"
        Const Email_ResetDevice As String = "Email_ResetDevice"
    End Structure
    Public Structure Keys
        Const SignUp_User As String = WebParams.GlobalPrefix + ProjectTitle + ".TempUser"
        Const AppKey As String = WebParams.GlobalPrefix + ProjectTitle + ".AppKey"
        Const PageKey As String = WebParams.GlobalPrefix + ProjectTitle + ".Page.{0}"
    End Structure
    Public Structure Elements
        Const PageContents As String = "pagecontents"
        Const ContentWrap As String = "contentwrap"
        Const ElmBox As String = "elmBox"
    End Structure
    Public Structure Pages
        Const XysSignupExpired As String = "XysSignupExpired"
        Const XysUnAuthorized As String = "XysUnAuthorized"
        Const XysAuth As String = "XysAuth"
        Const XysVerify As String = "XysVerify"
        Const XysSignup As String = "XysSignup"
        Const XysSignin As String = "XysSignin"
        Const XysSent As String = "XysSent"
        Const XysPassReset As String = "XysPassReset"
        Const XysPassChange As String = "XysPassChange"
        Const XysPermission As String = "XysPermission"
        Const XysPass As String = "XysPass"
        Const XysRole As String = "XysRole"
        Const XysRoleMV As String = "XysRoleMV"
        Const XysRoleEV As String = "XysRoleEV"
        Const XysUser As String = "XysUser"
        Const XysUserMV As String = "XysUserMV"
        Const XysUserEV As String = "XysUserEV"
        Const XysHome As String = "XysHome"
        Const XysSettings As String = "XysSettings"
        Const XysPage As String = "XysPage"
        Const XysPageEV As String = "XysPageEV"
        Const XysPageMV As String = "XysPageMV"
        Const XysLevel As String = "XysLevel"
        Const XysLevelEV As String = "XysLevelEV"
        Const XysLevelMV As String = "XysLevelMV"
        Const XysFile As String = "XysFile"
        Const XysFileMV As String = "XysFileMV"
        Const XysMenu As String = "XysMenu"
        Const XysMenuMV As String = "XysMenuMV"
        Const XysMenuEV As String = "XysMenuEV"
        Const XysProfile As String = "XysProfile"
        Const XysProPass As String = "XysProPass"
        Const XysCloseAcct As String = "XysCloseAcct"
        Const XysReport As String = "XysReport"
        Const XysBulletin As String = "XysBulletin"
        Const XysBulletinEV As String = "XysBulletinEV"
        Const XysBulletinMV As String = "XysBulletinMV"
        Const XysDict As String = "XysDict"
        Const XysDictMV As String = "XysDictMV"
        Const XysDictEV As String = "XysDictEV"
        Const XysLang As String = "XysLang"
        Const XysLangMV As String = "XysLangMV"
        Const XysLangEV As String = "XysLangEV"
        Const XysOption As String = "XysOption"
        Const XysOptionMV As String = "XysOptionMV"
        Const XysOptionEV As String = "XysOptionEV"

        Const Support As String = "Support"
        Const SupportMV As String = "SupportMV"
    End Structure
End Structure

Public Class AppKey
    Public Property UserId As String = String.Empty
    Public Property UserName As String = String.Empty
    Public Property UserEmail As String = String.Empty
    Public Property UserPhone As String = String.Empty
    Public Property UserPic As String = String.Empty
    Public Property RoleId As String = String.Empty
    Public Property UserRef As String = String.Empty
    Public Property DateTime As DateTime
End Class

Public Class WebBase
    Inherits WebPage

    Public ViewPart As ViewPart = Nothing
    Public ViewFields As New List(Of NameFlag)

    Public AppKey As AppKey = Nothing

    Public DateTimeFormat As String = ClientCulture.DateTimeFormat.ShortDatePattern + " " + ClientCulture.DateTimeFormat.ShortTimePattern
    Public DateFormat As String = ClientCulture.DateTimeFormat.ShortDatePattern

    Sub New()
        MyBase.New()

        Try
            Dim paramVlu As String = ParamValue(References.Keys.AppKey, True)
            AppKey = DeserializeObjectEnc(paramVlu, GetType(AppKey))
        Catch ex As Exception
            AppKey = Nothing
        End Try

        HtmlTranslator.Add(GetPageDict(Me.GetType.ToString))
    End Sub

    Public Overrides Sub OnResponse(ByRef _ApiResponse As ApiResponse)
        MyBase.OnResponse(_ApiResponse)

    End Sub
    Public Overrides Function OnRequest(Optional Method As String = "") As ApiResponse
        Dim SerializedViewPart As String = ParamValue(References.Keys.PageKey.Replace("{0}", Me.GetType.ToString))
        If SerializedViewPart <> String.Empty Then ViewPart = DeserializeObjectEnc(SerializedViewPart, GetType(ViewPart))
        If ViewPart IsNot Nothing Then
            For i As Integer = 0 To ViewFields.Count - 1
                If ViewFields(i).flag = False Then
                    Dim fld As NameValue = ViewPart.Field(ViewFields(i).name)
                    If fld IsNot Nothing Then fld.value = ParamValue(ViewFields(i).name)
                End If
            Next
        End If

        Dim _ApiResponse As ApiResponse = Nothing
        If IsMethodName(Method) = False Then
            _ApiResponse = New ApiResponse
            Dim dialogBox As New DialogBox(Translator.Format("accessdenied"))
            dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        End If
        Return _ApiResponse
    End Function
    Public Overrides Sub OnInitialized()
        ViewPart = InitialModel()

        Dim SerializedViewPart As String = SerializeObjectEnc(ViewPart, GetType(ViewPart))
        HtmlDoc.InitialScripts.StoreLocalValue(References.Keys.PageKey.Replace("{0}", Me.GetType.ToString), SerializedViewPart)
        If AppKey IsNot Nothing Then
            HtmlDoc.InitialScripts.SetCookie(References.Keys.AppKey, SerializeObjectEnc(AppKey, GetType(AppKey)))
        End If

        HtmlDoc.AddJsFile("WebScript.js")
        HtmlDoc.AddCSSFile("WebStyle.css")
        HtmlDoc.SetTitle(Translator.Format("title"))

        Dim _ViewAccess As Boolean = ViewAccess()
        HtmlDoc.HtmlBodyAddOn = If(_ViewAccess = True, InitialView(), PartialPage(References.Pages.XysUnAuthorized))
    End Sub

    Protected Friend Function IsMethodName(MethodName As String) As Boolean
        Dim rtnvlu As Boolean = False
        If ViewPart Is Nothing OrElse ViewPart.Methods Is Nothing Then Return True
        If ViewPart.Methods.Find(Function(x) x.Method.ToLower.EndsWith(MethodName.ToLower) = True) Is Nothing Then Return True

        For i As Integer = 0 To ViewPart.Methods.Count - 1
            Dim compMethod As String = If(ViewPart.Methods(i).Method.Contains("/") = False, MethodName, Me.GetType.ToString + "/" + MethodName)
            If ViewPart.Methods(i).Method.ToLower.Trim = compMethod.ToLower.Trim Then
                rtnvlu = True
            End If
        Next
        Return rtnvlu
    End Function

    Private Function ViewAccess() As Boolean
        If AppKey Is Nothing Then Return False
        Dim SSQL As String = " declare @roleid nvarchar(50),@pagename nvarchar(100) " +
                             " set @roleid = N'" + AppKey.RoleId + "' " +
                             " set @pagename = N'" + Me.GetType.ToString + "' " +
                             " select count(*) from XysPage where PageUse=1 and dbo.XF_RolePage(@roleid,PageId) = 1 and PageName = @pagename "

        Dim tcnt As String = SQLData.SQLFieldValue(SSQL)
        Return If(Val(tcnt) = 0, False, True)
    End Function
    Protected Friend Function ViewMethods(Optional PageType As String = "") As List(Of ViewMethod)
        PageType = If(PageType = String.Empty, Me.GetType.ToString, PageType)
        If AppKey Is Nothing Then Return Nothing
        Dim SSQL As String = " declare @roleid nvarchar(50),@pagename nvarchar(100) " +
                             " set @roleid = N'" + AppKey.RoleId + "' " +
                             " set @pagename = N'" + PageType + "' " +
                             " select MenuArea Area,MenuTag Tag,MenuMethod Method,MenuParams Params,MenuCtl Ctl,MenuType CtlType,dbo.XF_RoleMenu(@roleid,MenuId) Allowed" +
                             " from XysMenu where MenuUse=1 and PageId = dbo.XF_PageIdByName(@pagename) " +
                             "  order by area,MenuOrder "

        Dim data As List(Of ViewMethod) = DataTableListT(Of ViewMethod)(SQLData.SQLDataTable(SSQL))
        Return data
    End Function
    Public Overridable Function InitialModel() As ViewPart
        Dim _ViewPart = New ViewPart
        _ViewPart.Methods = ViewMethods()
        Return _ViewPart
    End Function
    Public Overridable Function InitialView() As String
        Return String.Empty
    End Function
    Public Overrides Sub OnAfterRender()
        MyBase.OnAfterRender()
    End Sub
    Protected Friend Function GetPageDict(pagename As String) As List(Of Translator.[Dictionary])
        Dim rlt As New List(Of Translator.[Dictionary])
        Dim SSQL As String = _
                        " declare @pageid nvarchar(50),@isocode nvarchar(10)  " +
                        " set @pageid = N'" + pagename + "' " +
                        " set @isocode = N'" + ClientLanguage + "' "

        Select Case ClientLanguage.Contains("-")
            Case True
                SSQL += " if exists(select * from XysDict where Isocode =  @isocode) " +
                       " begin " +
                       "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                       "  Where (Target = @pageid or Target = '*') " +
                       "          and (Isocode = '*' or Isocode = @isocode ) " +
                       "  order by KeyWord  " +
                       " end " +
                       " else " +
                       " begin " +
                       "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                       "  Where (Target = @pageid or Target = '*') " +
                       "          and (Isocode = '*' or Isocode =  'en-US' ) " +
                       "  order by KeyWord  " +
                       " end "
            Case False
                SSQL += " if exists(select * from XysDict where left(Isocode,2) =  @isocode) " +
                       " begin " +
                       "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                       "  Where (Target = @pageid or Target = '*') " +
                       "          and (Isocode = '*' or Isocode = @isocode ) " +
                       "  order by KeyWord  " +
                       " end " +
                       " else " +
                       " begin " +
                       "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                       "  Where (Target = @pageid or Target = '*') " +
                       "          and (Isocode = '*' or Isocode =  'en-US' ) " +
                       "  order by KeyWord  " +
                       " end "
        End Select

        Dim emsg As String = String.Empty
        Dim dt As DataTable = SQLData.SQLDataTable(SSQL, emsg)
        If emsg = String.Empty AndAlso dt IsNot Nothing AndAlso dt.Rows.Count <> 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim _Dict As New Translator.[Dictionary]
                _Dict.IsoCode = dt.Rows(i)(1).ToString
                _Dict.DicKey = dt.Rows(i)(2).ToString
                _Dict.DicWord = dt.Rows(i)(3).ToString
                rlt.Add(_Dict)
            Next
        End If
        Return rlt
    End Function

    Public Function PutData(SQL As List(Of String)) As String
        Dim emsg As String = String.Empty
        SQLData.SQLDataPut(SQL, emsg)
        WriteXysLog(String.Join("|", SQL), emsg)
        Return emsg
    End Function
    Public Sub WriteXysLog(logTxt As String, ByRef msg As String)
        Dim userid As String = If(AppKey IsNot Nothing, AppKey.UserId, String.Empty)

        Dim SQL As New List(Of String) From _
        {
            "insert into XysLog(LogId,ClientIp,UserId,LogTxt,JobRlt,SysDte) " +
            "values(@LogId,@ClientIp,@UserId,@LogTxt,@JobRlt,GETDATE())"
        }

        Dim SqlParams As New List(Of SqlClient.SqlParameter)
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@LogId", .Value = NewID(), .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@ClientIp", .Value = ClientIPAddress, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@UserId", .Value = userid, .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@LogTxt", .Value = EscQuote(logTxt.Trim), .SqlDbType = SqlDbType.NVarChar})
        SqlParams.Add(New SqlClient.SqlParameter With {.ParameterName = "@JobRlt", .Value = EscQuote(msg.Trim), .SqlDbType = SqlDbType.NVarChar})

        Dim emsg As String = String.Empty
        Dim _SqlData As New SQLData
        _SqlData.DataPut(SqlWithParams(SQL, SqlParams), emsg)
    End Sub

    Public Function SendEmail(Subject As String, bodyHtml As String, ToAddr As String()) As String
        Dim mail As New Mail
        mail.Subject = Subject
        mail.ToAddr = ToAddr
        mail.Body = bodyHtml
        Dim rlt As String = mail.SendMail()

        Return rlt
    End Function

    Protected Friend Function PageTitle(Optional showTimer As Boolean = True) As TitleSection2
        Dim tmr As New Timer With {.InnerText = "00:00"}
        tmr.SetAttribute(HtmlAttributes.id, "tmr")
        tmr.SetStyle(HtmlStyles.top, "38px")
        tmr.SetStyle(HtmlStyles.right, "90px")

        Dim imgfile As String = PhysicalFolder + "photos\" + AppKey.UserId + ".jpg"
        If File.Exists(imgfile) = False Then
            imgfile = ImagePath + "img_fakeuser.jpg"
        Else
            imgfile = GetPhotoData(imgfile)
        End If

        Dim tSector As New TitleSection2
        tSector.Title.Caption.InnerText = WebAppName
        tSector.Title.Page.InnerText = Translator.Format("title")
        If showTimer = True Then tSector.ContentExtra.InnerText = tmr.HtmlText
        tSector.Title.LogoImage.SetAttribute(HtmlEvents.onclick, "$navi('" + References.Pages.XysHome + "')")
        tSector.UserIcon.Icon.SetAttribute(HtmlAttributes.src, imgfile)
        tSector.UserIcon.Menu.AddItem(Translator.Format("profile"), String.Empty, "onclick:Profile();")
        tSector.UserIcon.Menu.AddItem(Translator.Format("signout"), String.Empty, "onclick:SignOut();")
        tSector.FooterSection.AddMenu(Translator.Format("support"), String.Empty, "onclick:" + ByPassCall("NaviClick", "m=support"))
        Return tSector
    End Function

    Protected Function SetPageMenu(selectedTags As String()) As MenuList
        Dim menulist As New MenuList
        menulist.Wrap.SetStyle(HtmlStyles.float, "right")

        Dim Methods As List(Of ViewMethod) = ViewPart.Methods.FindAll(Function(x) x.Area = "M" And x.Allowed = 1).OrderBy(Function(x) x.Sort).ToList
        If Methods IsNot Nothing Then
            For i As Integer = 0 To Methods.Count - 1
                If selectedTags.Length = 0 Then
                    Dim mnuctl As String = RollbackQuote(Methods(i).Ctl).Replace("{params}", String.Empty)
                    Dim mnutyp As String = Methods(i).CtlType
                    Dim mnuobj As Object = DeserializeObject(mnuctl, AssemblyType(mnutyp))
                    For j As Integer = 0 To mnuobj.Attributes.Count - 1
                        mnuobj.Attributes(j).value = mnuobj.Attributes(j).value.Replace("%partialdata%", PartialData)
                    Next
                    menulist.Add(mnuobj)
                Else
                    Dim Tags As String = String.Join(",", selectedTags)
                    If Tags.ToLower.Contains(Methods(i).Tag.ToLower) = True Then
                        Dim mnuctl As String = RollbackQuote(Methods(i).Ctl).Replace("{params}", String.Empty)
                        Dim mnutyp As String = Methods(i).CtlType
                        Dim mnuobj As Object = DeserializeObject(mnuctl, AssemblyType(mnutyp))
                        For j As Integer = 0 To mnuobj.Attributes.Count - 1
                            mnuobj.Attributes(j).value = mnuobj.Attributes(j).value.Replace("%partialdata%", PartialData)
                        Next
                        menulist.Add(mnuobj)
                    End If
                End If
            Next
        End If
        Return menulist
    End Function

    Protected Function SetPageButtons(selectedTags As String()) As Wrap
        Dim BtnWrap As New Wrap
        BtnWrap.SetStyle(HtmlStyles.display, "flex")
        BtnWrap.SetStyle(HtmlStyles.justifyContent, "flex-start")

        Dim BtnHtml As String = String.Empty
        Dim Methods As List(Of ViewMethod) = ViewPart.Methods.FindAll(Function(x) x.Area = "B" And x.Allowed = 1).OrderBy(Function(x) x.Sort).ToList
        If Methods IsNot Nothing Then
            For i As Integer = 0 To Methods.Count - 1
                If selectedTags.Length = 0 Then
                    Dim mnuctl As String = RollbackQuote(Methods(i).Ctl).Replace("{params}", String.Empty)
                    Dim mnutyp As String = Methods(i).CtlType
                    Dim mnuobj As Object = DeserializeObject(mnuctl, AssemblyType(mnutyp))
                    BtnHtml += mnuobj.HtmlText
                Else
                    Dim Tags As String = String.Join(",", selectedTags)
                    If Tags.ToLower.Contains(Methods(i).Tag.ToLower) = True Then
                        Dim mnuctl As String = RollbackQuote(Methods(i).Ctl).Replace("{params}", String.Empty)
                        Dim mnutyp As String = Methods(i).CtlType
                        Dim mnuobj As Object = DeserializeObject(mnuctl, AssemblyType(mnutyp))
                        BtnHtml += mnuobj.HtmlText
                    End If
                End If
            Next
        End If
        BtnWrap.InnerText = BtnHtml
        Return BtnWrap
    End Function

#Region "Attached File Handler"

    Protected Function UploadFile(fileKey As String, refId As String) As String
        Dim rlt As String = String.Empty
        If HttpContext.Current.Request.Files.Count = 0 Then Return rlt
        If ViewPart.Mode = ViewMode.Edit Then DeleteFiles(refId)

        Dim emsg As String = String.Empty
        Dim SQL As New List(Of String)

        Dim hfc As HttpFileCollection = HttpContext.Current.Request.Files
        SQL.Add(" delete from XysFile where FileRefId = N'" + refId + "' ")
        For i As Integer = 0 To hfc.Count - 1
            If hfc.Keys(i) = fileKey Then
                Dim flId As String = NewID(1)
                Dim flRef As String = Me.GetType.ToString
                Dim flRefid As String = refId
                Dim fltype As String = hfc(i).ContentType
                Dim flname As String = hfc(i).FileName.Split("\").Last
                Dim flname2 As String = NewID(0, False) + If(hfc(i).FileName.Split(".").Last = String.Empty, String.Empty, "." + hfc(i).FileName.Split(".").Last)
                Dim flFolder As String = DataFolder + flname2
                Dim flPath As String = DataPath + flname2
                hfc(i).SaveAs(flFolder)

                Dim ssql As String = " Insert XysFile(FileId,FileRef,FileRefId,FileType,FileName,FileLink,FilePath,SYSDTE,SYSUSR) " +
                                     " values(N'" + flId + "',N'" + flRef + "',N'" + flRefid + "',N'" + fltype + "',N'" + flname + "', " +
                                     "        N'" + flPath + "',N'" + flFolder + "'," +
                                     "          Getdate(),N'" + AppKey.UserId + "')"
                SQL.Add(ssql)
            End If
        Next

        Return PutData(SQL)
    End Function
    Private Sub DeleteFiles(refId As String)
        Dim ssql As String = " select FilePath from XysFile where FileRefId = N'" + refId + "' "
        Dim dt As DataTable = SQLData.SQLDataTable(ssql)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim filepath As String = dt.Rows(i)(0).ToString
                If File.Exists(filepath) = True Then
                    Try
                        File.Delete(filepath)
                    Catch ex As Exception

                    End Try
                End If
            Next
        End If
    End Sub

    Protected Function DeleteFile() As ApiResponse
        Dim fileid As String = GetDataValue("t")

        Dim _ApiResponse As New ApiResponse
        If fileid <> String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("wannadeletefile"))
            dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;")
            dialogBox.AddButton(Translator.Format("yes"), String.Empty, "class:button1;onclick:" + ByPassCall("RemoveFileProcess", "t=" + fileid))
            dialogBox.AddButton(Translator.Format("no"), String.Empty, "onclick:$PopOff();class:button;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents)
        End If
        Return _ApiResponse
    End Function
    Protected Function DeleteFileProcess() As ApiResponse
        Dim fileid As String = GetDataValue("t")

        Dim _ApiResponse As New ApiResponse
        Dim rlt As String = PutDeleteFile(fileid)
        If rlt = String.Empty Then
            Dim dialogBox As New DialogBox(Translator.Format("successdeletefile"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            dialogBox.AddButton(Translator.Format("ok"), String.Empty, "class:button1;onclick:" + ByPassCall("Refresh"))
            _ApiResponse.PopUpWindow(dialogBox.HtmlText)
        Else
            Dim dialogBox As New DialogBox(rlt)
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.PopUpWindow(dialogBox.HtmlText)
        End If
        Return _ApiResponse
    End Function
    Private Function PutDeleteFile(FileId As String) As String
        Dim rlt As String = String.Empty
        Dim filepath As String = SQLData.SQLFieldValue(" declare @FileId nvarchar(50) " +
                                                   " set @FileId = N'" + FileId + "' " +
                                                   " select FilePath from XysFile Where  FileId = @FileId ")
        If filepath <> String.Empty AndAlso File.Exists(filepath) = True Then
            Dim SQL As New List(Of String)
            SQL.Add("delete from XysFile where FileId = N'" + FileId + "'")
            File.Delete(filepath)
            rlt = PutData(SQL)
        Else
            If filepath.ToLower.Contains("sqlerror") = False AndAlso File.Exists(filepath) = False Then
                Dim SQL As New List(Of String)
                SQL.Add("delete from XysFile where FileId = N'" + FileId + "'")
                PutData(SQL)
            End If
            rlt = Translator.Format("filenotfound")
        End If

        Return rlt
    End Function

    Protected Function FileDownLoadEnc() As ApiResponse
        Dim encFileId As String = GetDataValue("FileId")
        Dim decFileId As String = Encryptor.DecryptData(encFileId)

        Dim _ApiResponse As New ApiResponse
        If decFileId <> String.Empty And decFileId.Split("|").Length >= 2 Then
            Dim p1 As String = decFileId.Split("|")(0)
            Dim p2 As String = decFileId.Split("|")(1)

            If DateDiff(DateInterval.Minute, CDate(p1), Now) > 30 Then
                Dim dialogBox As New DialogBox(Translator.Format("filenotfound"))
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                _ApiResponse.PopUpWindow(dialogBox.HtmlText)
            Else

                Dim dt As DataTable = SQLData.SQLDataTable("select FileName,FilePath from XysFile where FileId = N'" + p2 + "' ")
                If dt IsNot Nothing Then
                    Dim filename As String = dt.Rows(0)(0).ToString
                    Dim filepath As String = dt.Rows(0)(1).ToString
                    Dim FileLink As String = DownLoadFileLink(filename, filepath)
                    _ApiResponse.DownloadFile(FileLink)
                Else
                    Dim dialogBox As New DialogBox(Translator.Format("filenotfound"))
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText)
                End If
            End If

        End If
        Return _ApiResponse
    End Function


    Public Function UpdatePhoto() As ApiResponse
        Dim imgid As String = GetDataValue("f")
        Dim inputid As String = GetDataValue("s")
        Dim userid As String = DecryptString(GetDataValue("p"))

        Dim _ApiResponse As New ApiResponse
        Dim hfc As HttpFileCollection = HttpContext.Current.Request.Files

        If hfc.Count = 0 OrElse hfc(0).FileName.Split(".").Last <> "jpg" Then
            Dim dialogBox As New DialogBox(Translator.Format("nouploadfilenojpg"))
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
            _ApiResponse.SetElementValue(inputid, String.Empty)
            _ApiResponse.PopUpWindow(dialogBox.HtmlText)
        Else
            Dim FileExt As String = hfc(0).FileName.Split(".").Last
            Dim FileName As String = PhysicalFolder + "photos\" + userid + ".jpg"

            Try
                hfc(0).SaveAs(FileName)
                Dim imgdata As String = GetPhotoData(FileName)
                _ApiResponse.SetElementAttribute(imgid, HtmlAttributes.src, imgdata)
            Catch ex As Exception
                Dim dialogBox As New DialogBox(ex.Message)
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;")
                _ApiResponse.SetElementValue(inputid, String.Empty)
                _ApiResponse.PopUpWindow(dialogBox.HtmlText)
            End Try
        End If
        Return _ApiResponse
    End Function
    Public Function GetPhotoData(FileName As String) As String
        Dim _imageData As String = String.Empty
        Dim webClient As New System.Net.WebClient
        Try
            Dim stream As Stream = webClient.OpenRead(FileName)
            Dim bitmap As New Bitmap(stream)
            _imageData = ImageHandler.ImageBase64(bitmap, 300, 380)
        Catch ex As Exception
            Dim stream As Stream = webClient.OpenRead(ImagePath + "noimage.jpg")
            Dim bitmap As New Bitmap(stream)
            _imageData = ImageHandler.ImageBase64(bitmap, 300, 380)
        End Try
        Return _imageData
    End Function

#End Region
End Class


Public Enum ViewMode
    View = 0
    [New] = 1
    Edit = 2
End Enum
Public Class ViewPart
    Public Property Fields As New List(Of NameValue)
    Public Property Methods As List(Of ViewMethod) = Nothing
    Public Property Data As DataTable = Nothing
    Public Property Params As String = String.Empty
    Public Property Mode As ViewMode = ViewMode.View
    Public Property UIControl As UIControl = Nothing
    Public Function ColunmValue(ColumnName As String) As String
        Dim rtnVlu As String = String.Empty
        If Data IsNot Nothing AndAlso Data.Columns(ColumnName) IsNot Nothing AndAlso Data.Rows.Count > 0 Then
            rtnVlu = Data.Rows(0)(ColumnName).ToString
        End If
        Return rtnVlu
    End Function
    Public Function Field(Name As String) As NameValue
        Return Fields.Find(Function(x) x.name = Name)
    End Function
End Class
Public Class ViewMethod
    Public Property Area As String = String.Empty
    Public Property Tag As String = String.Empty
    Public Property Method As String = String.Empty
    Public Property Params As String = String.Empty
    Public Property Ctl As String = String.Empty
    Public Property CtlType As String = String.Empty
    Public Property Sort As Integer = 0
    Public Property Allowed As Integer = 0
End Class