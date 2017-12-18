<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Deduplication.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
    <div class="row">
        <div class="box">
            <div class="col-lg-12">
                <hr>
                <h2 class="intro-text text-center">Login
                    <strong>form</strong>
                </h2>
                <hr>
                    <div class="row">
                    <div class="form-group col-lg-4 col-md-3 col-sm-3 col-xs-1"></div>
                        <div class="form-group col-lg-4 col-md-6 col-sm-6 col-xs-10">
                            <label>Username</label>
                            <asp:TextBox runat="server" ID="username" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group col-lg-4 col-md-3 col-sm-3 col-xs-1"></div>
                        </div>
                        <div class="row">
                        <div class="form-group col-lg-4 col-md-3 col-sm-3 col-xs-1"></div>
                        <div class="form-group col-lg-4 col-md-6 col-sm-6 col-xs-10">
                            <label>Password</label>
                            <asp:TextBox runat="server" ID="password" TextMode="Password" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group col-lg-4 col-md-3 col-sm-3 col-xs-1"></div>
                        </div>
                        <div class="row">
                        <center>
                        <div class="form-group col-lg-4 col-md-3 col-sm-3 col-xs-1"></div>
                        <div class="form-group col-lg-3 col-xs-10">
                        <asp:Label runat="server" id="errorLabel" CssClass="label-danger label"></asp:Label>
                        </div>
                        <div class="form-group col-lg-1 col-sm-3 col-xs-5">
                            <asp:Button runat="server" ID="loginButton" CssClass="btn btn-success" 
                                Text="Login" onclick="loginButton_Click" />
                        </div>
                        <div class="form-group col-lg-4 col-md-3 col-sm-3 col-xs-1"></div>
                        </center>
                        </div>
                 </div>
            </div>
        </div>
    </div>
</asp:Content>
