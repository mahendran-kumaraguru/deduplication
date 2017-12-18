<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="Deduplication.About" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">

        <div class="row">
            <div class="box">
                <div class="col-lg-12">
                    <hr>
                    <h2 class="intro-text text-center">About
                        <strong>Data Deduplication</strong>
                    </h2>
                    <hr>
                </div>
                <div class="col-md-6">
                    <img class="img-responsive img-border-left" src="img/arch.png" alt="">
                </div>
                <div class="col-md-6">
                    <p>CLOUD computing provides seemingly unlimited “virtualized” resources to users as services across the whole Internet.</p>
                    <p>Deduplication has been a well-known technique to make data management scalable in cloud computing.</p>
                    <p>Convergent encryption has been proposed to enforce data confidentiality while making deduplication feasible.</p>
                    <p>This site will not allow you to save the same file again in the cloud.</p>
                    <p>This site will also check the integrity and correctness of your saved files.</p>
                </div>
                <div class="clearfix"></div>
            </div>
        </div>

        <div class="row">
            <div class="box">
                <div class="col-lg-12">
                    <hr>
                    <h2 class="intro-text text-center">Contact
                        <strong>form</strong>
                    </h2>
                    <hr>
                    <p>If you have any queries, please contact us. Also post your valuable comments about our work. Thank you for visiting our website.</p>
                    
                        <div class="row">
                            <div class="form-group col-lg-4">
                                <label>Name</label>
                                <asp:TextBox ID="Vname" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group col-lg-4">
                                <label>Email Address</label>
                                <asp:TextBox ID="Vmail" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group col-lg-4">
                                <label>Mobile Number</label>
                                <asp:TextBox ID="Vmobile" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="clearfix"></div>
                            <div class="form-group col-lg-12">
                                <label>Message</label>
                                <asp:TextBox ID="Vmessage" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="6"></asp:TextBox>
                            </div>
                            <div class="form-group col-lg-12">
                                <asp:button runat="server" Text="Submit" ID="contactButton" class="btn btn-default" 
                                    onclick="contactButton_Click" /><center>
                                <asp:Label ID="erroLabel" runat="server" Text="" Font-Size="Medium" CssClass="label-danger label"></asp:Label></center>
                            </div>
                        </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="box">
                <div class="col-lg-12">
                    <hr>
                    <h2 class="intro-text text-center">Our
                        <strong>Team</strong>
                    </h2>
                    <hr>
                </div>
                <div class="col-sm-4 text-center">
                    <img class="img-responsive"  src="img/kar.jpg"  alt=" ">
                    <h3><a href="http://www.facebook.com/karthick.raja.106902">Karthick Raja C</a>
                        <small></small>
                    </h3>
                </div>
                <div class="col-sm-4 text-center">
                    <img class="img-responsive" src="img/kis.jpg" alt=" ">
                    <h3><a href="http://www.facebook.com/kishore.kumar.79">Kishore Kumar S</a>
                        <small></small>
                    </h3>
                </div>
                <div class="col-sm-4 text-center">
                    <img class="img-responsive" src="img/mah.jpg" alt=" ">
                    <h3><a href="http://www.facebook.com/yoursmahii">Mahendran K</a>
                        <small></small>
                    </h3>
                </div>
                <div class="clearfix"></div>
            </div>
        </div>
    </div>
</asp:Content>
