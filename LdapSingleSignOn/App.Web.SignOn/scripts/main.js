var reqUrl = getParameterByName("ReturnUrl");
var parts = reqUrl === null ? null : reqUrl.split("_");

// A helper function that checks for the 
// support of the 3D CSS3 transformations.
function supportsCSS3D() {
    var props = [
        'perspectiveProperty', 'WebkitPerspective', 'MozPerspective'
    ], testDom = document.createElement('a');

    for (var i = 0; i < props.length; i++) {
        if (props[i] in testDom.style) {
            return true;
        }
    }

    return false;
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

$(function () {

    // Checking for CSS 3D transformation support
    $.support.css3d = supportsCSS3D();

    var formContainer = $('#formContainer');

    formContainer.find('form').submit(function (e) {
        var request = {
            Username: $("#loginUsername").val(),
            Password: $("#loginPass").val()
        };

        $.ajax({
            url: "LoginProcess.aspx",
            type: "POST",
            dataType: 'jsonp',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(request),
            complete: function (data, b) {
                switch (data.status) {
                    case 401:
                        $("#errorDiv").show();
                        $("#errorTxt").html(data.statusText);
                        console.log(data);
                        break;
                    case 200:
                        $("#errorDiv").hide();
                        $("#errorTxt").html();

                        var jsonQ = data.responseText.replace("Value", "\"Value\"");
                        var json = JSON.parse(jsonQ);
                        Cookies.set('.ASPXAUTH', json.Value, { expires: 1 });
                        var retPath = reqUrl === null ? "Logged.html" : parts[0] + "://" + parts[1] + (parts[2][0] === "/" ? parts[2] : ":" + parts[2]);
                        window.location.replace(retPath);
                        break;
                }
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Basic " + btoa(request.Username + ":" + request.Password));
            }
        });

        e.preventDefault();
    });
});
