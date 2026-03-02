jQuery.event.special.touchstart = { setup: function (_, ns, handle) { if (ns.indexOf("noPreventDefault") != -1) { this.addEventListener("touchstart", handle, { passive: false }); } else { this.addEventListener("touchstart", handle, { passive: true }); } } };
$(".login-toggle").click(function () {
    $(".login-dropdown").toggleClass('open');
});

//function setTimezoneCookie() {
//    var e = "timezoneoffset";
//    if ($.cookie(e)) {
//        parseInt($.cookie(e)) !== (new Date).getTimezoneOffset() && ($.cookie(e, (new Date).getTimezoneOffset()), location.reload());
//    } else {
//        var t = "test cookie";
//        $.cookie(t, !0), $.cookie(t) && ($.cookie(t, null), $.cookie(e, (new Date).getTimezoneOffset()), location.reload());
//    }
//}
$(function () {
    $(".lazy").lazy();
    $("#regDropDown").val(__webEdition);
    $("#subscribeBtn").click(function () {
            var e = $("#subscribeEmail").val();
        /^\w+([\.-]?\w+)*@@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(e) ? $.ajax({
            url: "/Home/SubscribeNews",
            type: "Post",
            data: {
                email: e
            },
            dataType: "Json",
            success: function (e) {
                n("Subscribe successfully!", "success");
                $("#subscribeEmail").val("");
            },
            error: function () { }
        }) : n("Please enter your valid email", "danger");
        });
    var e = !0;
    $("#navBtn").click(function () {
        $(this).toggleClass("active"), e ? $("#NewsSector").animate({
            left: "0"
        }) : $("#NewsSector").animate({
            left: "-250px"
        }), e = !e
    });
    $("#myBtn").click(function () {
        $("body,html").animate({
            scrollTop: 0
        }, 500);
    });
    window.onscroll = function () {
        100 < document.body.scrollTop || 100 < document.documentElement.scrollTop ? (document.getElementById("myBtn").style.display = "block", $("#NewsSector").css({
            left: "-250px"
        }), $("#navBtn").removeClass("active"), e = !0) : document.getElementById("myBtn").style.display = "none"
    };
    var t = "",
        o = "",
        a = "";

    function n(e, t) {
        $.notify({
            message: e
        }, {
                type: t,
                delay: 1e3,
                placement: {
                    from: "top",
                    align: "right"
                }
            });
    }
    $('#logoContainer').click(function () {
        var date = new Date();
        date.setTime(date.getTime() + (60 * 60 * 1000));
        document.cookie = "Edition=Global Edition;expires=" + date.toGMTString() + "; path=/";
        window.location.href = "/";
    });
    $('.globeBtn').click(function () {
        var date = new Date();
        date.setTime(date.getTime() + (60 * 60 * 1000));
        document.cookie = "Edition=Global Edition;expires=" + date.toGMTString() + "; path=/";
        window.location.href = "/";
    });
    function getCookie(cname) {
        var name = cname + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }
    var cookieEdition = getCookie("Edition");
    if (cookieEdition == "Global Edition") {
        $('.globeBtn').hide();
        $('#homeBtn').html('Home');
        $('#homeBtn').attr('Title', 'Home');
        $('.sortupIcon').css({
            'margin-left': '30px'
        });
    } else {
        $('.globeBtn').show();
        $('#homeBtn').html('Edition Home');
        $('#homeBtn').attr('Title', cookieEdition);
        $('.sortupIcon').css({
            'margin-left': '140px'
        });
    }
    $("#cancelPopup").click(function () {
        $(".feedbackPopup").fadeOut()
    });
    $("#feedbackBtn").click(function () {
        $(".feedbackPopup").fadeToggle()
    });
    $("#submitFeedBtn").click(function () {
        t = $("#fuser").val(), o = $("#femail").val(), a = $("#fmessage").val(), "" == t ? n("Please enter your name", "danger") : /^\w+([\.-]?\w+)*@@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(o) ? "" == a ? n("Please enter your message", "danger") : $.ajax({
            url: "/Home/SubmitFeedback",
            type: "Post",
            data: {
                name: t,
                email: o,
                message: a
            },
            dataType: "Json",
            success: function (e) {
                n("Submit feedback successfully!", "success"), $("#fuser").val(""), $("#femail").val(""), $("#fmessage").val(""), a = o = t = "", $(".feedbackPopup").fadeOut()
            },
            error: function () { }
        }) : n("Please enter your valid email", "danger")
    });
    $('.carousel').carousel({
        interval: 2000
    })
    //setTimeout(function () {
    //    null == localStorage.getItem("SocialPopup") && $("#PageLikeModel").modal()
    //    }, 1e4);
    //$("#socialCloseBtn").click(function () {
    //    null == localStorage.getItem("SocialPopup") && localStorage.setItem("SocialPopup", !0)
    //});
    //setTimezoneCookie();
});