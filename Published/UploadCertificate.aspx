<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadCertificate.aspx.cs" Inherits="ZipNachWebAPI.UploadCertificate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
    function UploadFile(fileUpload) {
             debugger;
             if (fileUpload.value != '') {
                 
                 document.getElementById("<%=btnUpload.ClientID %>").click();
             }
         }

    </script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:Label runat="server" Text="Enter AppId"></asp:Label>
        <asp:TextBox runat="server" ID="txtAppId"></asp:TextBox>
        <div>
            <asp:FileUpload ID="FileUpload1" runat="server" onchange="UploadFile(this);" accept=".cer" />
        </div>
        <asp:LinkButton runat="server" ID="btnUpload" Visible="true" Text="<i class='fa fa-upload'></i> Upload"
            Style="font-size: 11.5px!important; font-weight: 600; color: #393939;"
            OnClick="btnUpload_Click"></asp:LinkButton>
    </form>
</body>
</html>
