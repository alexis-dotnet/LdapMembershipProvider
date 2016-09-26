<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="App.Web.SpaA.Index" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Home Page - Spa A</title>
    <link href="/Content/bootstrap.min.css" rel="stylesheet" />
    <link href="/Content/DataTables/css/dataTables.bootstrap.min.css" rel="stylesheet" />
    <script src="/Scripts/jquery-3.1.0.min.js"></script>
    <script src="/Scripts/bootstrap.js"></script>
    <script src="/Scripts/DataTables/jquery.dataTables.min.js"></script>
    <script src="/Scripts/DataTables/dataTables.bootstrap.min.js"></script>
    <script src="/Scripts/DataTables/dataTables.colReorder.min.js"></script>
    <script src="/Scripts/DataTables/dataTables.autoFill.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#example').DataTable();
        });
    </script>
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
                    <a class="navbar-brand" href="#">Spa A</a>
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
            <table id="example" class="table table-striped table-bordered" cellspacing="0" width="100%">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Username</th>
                        <th>Fax</th>
                        <th>Phone</th>
                        <th>Room</th>
                        <th>Email</th>
                        <th>Location</th>
                        <th>Group 1</th>
                        <th>Group 2</th>
                    </tr>
                </thead>
                <tfoot>
                    <tr>
                        <th>Id</th>
                        <th>Username</th>
                        <th>Fax</th>
                        <th>Phone</th>
                        <th>Room</th>
                        <th>Email</th>
                        <th>Location</th>
                        <th>Group 1</th>
                        <th>Group 2</th>
                    </tr>
                </tfoot>
                <tbody>
                    <asp:Repeater ID="rptData" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Id") %></td>
                                <td><%# Eval("Username") %></td>
                                <td><%# Eval("Fax") %></td>
                                <td><%# Eval("Phone") %></td>
                                <td><%# Eval("RoomNumber") %></td>
                                <td><%# Eval("Email") %></td>
                                <td><%# Eval("Location") %></td>
                                <td><%# Eval("Role1") %></td>
                                <td><%# Eval("Role2") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </form>
</body>
</html>
