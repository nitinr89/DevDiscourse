$(document).ready(function () {
    $(document).find('#regDropDown').change(function () {
        var date = new Date();
        date.setTime(date.getTime() + (24 * 60 * 60 * 1000));
        document.cookie = "Edition=" + $(this).val() + "; expires=" + date.toGMTString() + "; path=/";
        window.location.href = $(this).find('option:selected').attr('data-to');
    });
  
    GetNewsAlert();
    $('#opinion1').owlCarousel({
        items: 2,
        nav: true,
        margin: 0,
        startPosition: '0',
        autoplay: true,
        autoplayTimeout: 3000,
        autoplayHoverPause: true,
        loop: true,
        responsive: {
            320: {
                items: 1
            },
            425: {
                items: 1
            },
            550: {
                items: 1
            },
            768: {
                items: 2
            },
            1024: {
                items: 3
            },
            1200: {
                items: 3
            }
        }
    });
    $("#opinion1 .owl-prev").html('<span class="fa fa-angle-left"></span>');
    $("#opinion1 .owl-next").html('<span class="fa fa-angle-right"></span>');

    debugger;
    GetVideoNews();

   
    function GetNewsAlert() {
        var ApiDataUrl = "/api/Searchapi/GetNewsAlert";
        $.getJSON(ApiDataUrl, function (data) {
            var alertHtml = '';
            $.each(data, function (i, item) {
                var slugUrl = convertToSlug(item.newsId, item.title);
                var label = item.label != null ? item.label : "agency-wire";
                alertHtml += '<li><a href="/article/' + label + '/' + slugUrl + '" class="text-decoration"><span class="fg-black">●</span> ' + item.title + '</a></li>';
            });
            $('#news-alert').html(alertHtml);
            $('#example').breakingNews();
        });
    }

    function GetVideoNews() {
        var ApiDataUrl = "/api/SearchApi/GetHomeVideoNews/" + encodeURI(_edition);
        $.getJSON(ApiDataUrl, function (data) {
            var videohtml = '';
            $.each(data, function (i, item) {
                var slugUrl = convertToSlug(item.id, item.title);
                var newsImage = item.fileThumbUrl; //item.fileThumbUrl.indexOf("devdiscourse.blob.core.windows.net") != -1 ? item.fileThumbUrl : item.fileThumbUrl;
                var label = item.label != null ? item.label : "agency-wire";
                videohtml += '<div class="item"><a href="/news/videos/' + slugUrl + '" title="' + item.title.split('"').join('') + '" class="text-decoration"><div class="news-card bg-gray lazy" data-src="' + newsImage + '?width=435&height=245&format=jpeg&mode=crop&quality=60"><div style="width:100%;height:100%;background-color:rgba(0,0,0,.3);padding:15px"><div class="media" style="bottom:15px;position:absolute"><div class="media-left"><div class="text-center" style="color: #fff; width: 40px; font-size: 17px; border: 2px solid #fff; border-radius: 50%; height: 40px; line-height: 40px; padding-left: 3px;"><span class="fa fa-play"></span></div></div><div class="media-body"><h3 class="no-margin" style="color:#fff;font-size:18px;line-height:1.26">' + item.title + '</h3></div></div></div></div></a></div>';
            });
            
            $('#videoNews').html('<h2 class="section-title2"><span style="background-color:#f4f4f4">DevShots</span>' + '</h2><div id="videoCarousel" class="owl-carousel">' + videohtml + '</div>');


            $('.lazy').lazy();

            
            $(document).find('#videoCarousel').owlCarousel({
                items: 3,
                nav: true,
                dots:false,
                margin: 45,
                startPosition: '0',
                autoplay: true,
                autoplayTimeout: 3000,
                autoplayHoverPause: true,
                loop: false,
                responsive: {
                    320: {
                        items: 1
                    },
                    425: {
                        items: 1
                    },
                    550: {
                        items: 2
                    },
                    768: {
                        items: 2
                    },
                    1024: {
                        items: 3
                    },
                    1200: {
                        items: 3
                    }
                }
            });
            
            $("#videoCarousel .owl-prev").html('<span class="fa fa-angle-left"></span>');
            $("#videoCarousel .owl-next").html('<span class="fa fa-angle-right"></span>');
            $("#videoCarousel .owl-next").click(function () {
                if ($(this).hasClass('disabled')) {
                    $(this).css('width', '100px').html('<a href="/news/videos" style="color:#fff;font-size:14px">View All</a>');
                }
            });
            $("#videoCarousel .owl-prev").click(function () {
                $(".owl-next").css('width', '40px').html('<span class="fa fa-angle-right"></span>');
            });
        });
    }
    $(document).find('#interviewCarousel').owlCarousel({
        items: 3,
        nav: true,
        margin: 45,
        startPosition: '0',
        autoplay: true,
        autoplayTimeout: 3000,
        autoplayHoverPause: true,
        loop: false,
        responsive: {
            320: {
                items: 1
            },
            425: {
                items: 1
            },
            550: {
                items: 2
            },
            768: {
                items: 2
            },
            1024: {
                items: 3
            },
            1200: {
                items: 3
            }
        }
    });
    
    $("#interviewCarousel .owl-prev").html('<span class="fa fa-angle-left"></span>');
    $("#interviewCarousel .owl-next").html('<span class="fa fa-angle-right"></span>');
    $("#interviewCarousel .owl-next").click(function () {
        if ($(this).hasClass('disabled')) {
            $(this).css('width', '100px').html('<a href="/blogs/interview" style="color:#fff;font-size:14px">View All</a>');
        }
    });
    $("#interviewCarousel .owl-prev").click(function () {
        $(".owl-next").css('width', '40px').html('<span class="fa fa-angle-right"></span>');
    });
    function convertToSlug(newsId, str) {
        str = str.replace(/^\s+|\s+$/g, '');
        str = str.toLowerCase();
        var from = "àáäâèéëêìíïîòóöôùúüûñç·/_,:;";
        var to = "aaaaeeeeiiiioooouuuunc------";
        for (var i = 0, l = from.length; i < l; i++) {
            str = str.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
        }
        str = str.replace(/[^a-z0-9 -]/g, '').replace(/\s+/g, '-').replace(/-+/g, '-');
        return newsId + "-" + str;
    }
});