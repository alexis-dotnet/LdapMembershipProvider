<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="App.Web.SignOn.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous" />
    <link href="css/main.css" rel="stylesheet" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.0/jquery.min.js"></script>
    <script src="scripts/js-cookie/js.cookie-2.1.3.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js" integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa" crossorigin="anonymous"></script>
    <script type="text/javascript">
        window.alert = function () { };
        var defaultCSS = document.getElementById('bootstrap-css');
        function changeCSS(css) {
            if (css) $('head > link').filter(':first').replaceWith('<link rel="stylesheet" href="' + css + '" type="text/css" />');
            else $('head > link').filter(':first').replaceWith(defaultCSS);
        }
        $(document).ready(function () {
            var iframe_height = parseInt($('html').height());
            window.parent.postMessage(iframe_height, 'http://www.designsave.com');
        });
    </script>
</head>
<body>
    <div class="container">
        <div class="row">
            <div class="container" id="formContainer">

                <form id="form1" class="form-signin" role="form">
                    <h3 class="form-signin-heading">Please sign in</h3>
                    <input type="text" class="form-control" name="loginUsername" id="loginUsername" placeholder="Username" required autofocus>
                    <input type="password" class="form-control" name="loginPass" id="loginPass" placeholder="Password" required>
                    <button class="btn btn-lg btn-primary btn-block" type="submit">Sign in</button>
                    <br/>
                    <div id="errorDiv" style="display: none">
                        <span id="errorTxt" style="color: red"></span>
                    </div>
                </form>

            </div>
            <!-- /container -->
        </div>
    </div>
    <script src="scripts/main.js"></script>
</body>
</html>
