<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="App.Web.SpaB.Index" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Home Page - Spa B</title>
    <link href="/Content/bootstrap.css" rel="stylesheet" />
    <script src="/Scripts/jquery-3.1.0.min.js"></script>
    <script src="/Scripts/bootstrap.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="navbar navbar-inverse navbar-fixed-top">
            <div class="container">
                <div class="navbar-header">
                    <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <a class="navbar-brand" href="#">Spa B</a>
                </div>
                <div class="navbar-collapse collapse">
                    <ul class="nav navbar-nav navbar-right">
                        <li>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="OnLogoutClick">Log out</asp:LinkButton>
                        </li>
                    </ul>

                </div>
            </div>
        </div>
        <div class="container body-content">

            <br />
            <br />
            <br />
            <div class="container">
                <div class="row">
                    <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6 col-xs-offset-0 col-sm-offset-0 col-md-offset-3 col-lg-offset-3 toppad">

                        <h3 style="color: cadetblue">Your Full LDAP Profile</h3>
                        <div class="panel panel-info">
                            <div class="panel-heading">
                                <h3 class="panel-title">
                                    <asp:Label ID="txtFullName" runat="server"></asp:Label>
                                </h3>
                            </div>
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-md-3 col-lg-3 " align="center">
                                        <img alt="User Pic" src="Images/photo.png" class="img-circle img-responsive">
                                    </div>
                                    <div class=" col-md-9 col-lg-9 ">
                                        <table class="table table-user-information">
                                            <tbody>
                                                <tr>
                                                    <td>Username:</td>
                                                    <td>
                                                        <asp:Label ID="txtUsername" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>UID</td>
                                                    <td>
                                                        <asp:Label ID="txtId" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Fax Number:</td>
                                                    <td>
                                                        <asp:Label ID="txtFax" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Phone Number:</td>
                                                    <td>
                                                        <asp:Label ID="txtPhone" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Room Number</td>
                                                    <td>
                                                        <asp:Label ID="txtRoom" runat="server"></asp:Label>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <tr>
                                                        <td>Email</td>
                                                        <td>
                                                            <asp:Label ID="txtEmail" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>City</td>
                                                        <td>
                                                            <asp:Label ID="txtLocation" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <td>Groups</td>
                                                    <td>
                                                        <asp:Repeater ID="rptRoles" runat="server">
                                                            <ItemTemplate>
                                                                <%# Container.DataItem %><br />
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
