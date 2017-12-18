<%@ Page Title="" Language="C#" MasterPageFile="~/user/user.Master" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="Deduplication.user.Upload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../css/Style.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
    <br /><br />
        <div class="row">
        
            <div class="box rounded col-lg-6 col-lg-offset-3 col-md-6 col-md-offset-3 col-sm-8 col-sm-offset-2 col-xs-10 col-xs-offset-1">
                <div class="row">
                    <br />
                    <div class="col-lg-4 col-lg-offset-3">
                        <asp:FileUpload ID="FileUpload1" Font-Bold="true" Font-Size="Large" runat="server" />
                    </div>
                    <div class="col-lg-4"></div>
                </div>
                <br /><br />
                <center>
                <div class="row">
                    
                    <div class="col-lg-2 col-lg-offset-5">
                        <asp:Button ID="uploadButton" Text="Upload" Font-Bold="true" Font-Size="Large" 
                            runat="server" onclick="uploadButton_Click"/>
                    </div>
                    <div class="col-lg-5"></div>
                </div>
                <br />
                <div class="row">
                    <div class="col-lg-4"></div>
                    <div class="col-lg-4">
                        <asp:Label runat="server" ID="errorLabel" Font-Size="Large" Visible="false" CssClass="label label-warning">No file</asp:Label>
                    </div>
                    <div class="col-lg-4"></div>
                </div>
                </center>
                <br />
                <br />
                
            </div><div class="col-lg-2"></div>
        </div>
    </div>

</asp:Content>
