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
    GetVideoNews();
    //GetInterview();
    //GetOpinion();
    //sectorNews("sector10", "10", "/news/agro-forestry", "AGRO-FORESTRY", "news1");
    //sectorNews("sector6", "6", "/news/technology", "TECHNOLOGY", "news2");
    //sectorNews("sector1", "1", "/news/economy-business", "ECONOMY &amp; BUSINESS", "news3");
    //sectorNews("sector4", "4", "/news/education", "EDUCATION", "news4");
    //sectorNews("sector7", "7", "/news/energy-extractives", "ENERGY &amp; EXTRACTIVES", "news5");
    //sectorNews("sector2", "2", "/news/health", "HEALTH", "news6");
    //sectorNews("sector11", "11", "/news/law-governance", "LAW &amp; GOVERNANCE", "news7");
    //sectorNews("sector15", "15", "/news/science-environment", "SCIENCE &amp; ENVIRONMENT", "news8");
    //sectorNews("sector9", "9", "/news/socialgender", "SOCIAL &amp; GENDER", "news9");
    //sectorNews("sector3", "3", "/news/transport", "TRANSPORT", "news10");
    //sectorNews("sector17", "17", "/news/urban-development", "URBAN DEVELOPMENT", "news11");
    //sectorNews("sector12", "12", "/news/wash", "WASH", "news12");
    //sectorNews("sector14", "14", "/news/art-culture", "ART &amp; CULTURE", "news13");
    //sectorNews("sector18", "18", "/news/sports", "SPORTS", "news15");
    //sectorNews("sector19", "19", "/news/politics", "POLITICS", "news19");    
    //function GetOpinion() {
    //    var ApiDataUrl = "/api/Search/GetOpinion/" + encodeURI(_edition);
    //    $.getJSON(ApiDataUrl, function (data) {
    //        var alertHtml = '';
    //        $.each(data, function (i, item) {
    //            var text = $('<div>').html(item.Description).children('p')[0].innerText;
    //            text = text.length > 150 ? text.substring(0, 150) + '...' : text;
    //            var slugUrl = convertToSlug(item.NewsId, item.Title);
    //            var label = item.Label != null ? item.Label : "agency-wire";
    //            var name = item.Name != null ? '<span class="pull-left">' + item.Name + '</span>' : "";
    //            var ImageUrl = "/AdminFiles/Logo/img_avatar.png";
    //            if (item.AuthorImage != null) {
    //                ImageUrl = item.AuthorImage;
    //            } else if (item.ImageUrl != null) {
    //                ImageUrl = item.ImageUrl;
    //            }
    //            ImageUrl = ImageUrl.indexOf("devdiscourse.blob.core.windows.net") != -1 ? "/remote.axd?" + ImageUrl : ImageUrl;
    //            alertHtml += '<div class="item"><div class="p-t-20 p-b-20 m-t-10 opinion-card">' +
    //                '<div class="media">' +
    //                '<div class="media-left">' +
    //                '<img src="https://www.devdiscourse.com' + ImageUrl + '?width=60&height=60&mode=crop" onerror="this.src=' + "'/AdminFiles/Logo/img_avatar.png?width=60&height=60&mode=crop'" + '" width="60" height="60" alt="' + (item.Name != null ? item.name : "") + '" class="img-circle" />' +
    //                '</div>' +
    //                '<div class="media-body">' +
    //                '<a href="/article/' + label + '/' + slugUrl + '" class="text-decoration"><h3 class="media-heading f-20 font-normal">' + item.Title + '</h3></a>' +
    //                '<p class="f-16 text-muted">' + text + '</p>' +
    //                '<div class="clearfix m-t-5">' + name +
    //                '<span class="pull-right"><a href="/article/' + label + '/' + slugUrl + '" class="primary-link">Read More</a></span>' +
    //                '</div>' +
    //                '</div>' +
    //                '</div>' +
    //                '</div></div>';
    //        });
    //        $('#opinion1').html(alertHtml);
    //        $('#opinion1').owlCarousel({
    //            items: 3,
    //            nav: true,
    //            margin: 0,
    //            startPosition: '0',
    //            autoplay: true,
    //            autoplayTimeout: 3000,
    //            autoplayHoverPause: true,
    //            loop: true,
    //            responsive: {
    //                320: {
    //                    items: 1
    //                },
    //                425: {
    //                    items: 1
    //                },
    //                550: {
    //                    items: 2
    //                },
    //                768: {
    //                    items: 2
    //                },
    //                1024: {
    //                    items: 3
    //                },
    //                1200: {
    //                    items: 3
    //                }
    //            }
    //        });
    //    });
    //}
    //GetLiveDiscourse();
    //function GetLiveDiscourse() {
    //    var ApiDataUrl = "/api/Discourse/GetInfocusLiveDiscourse/" + _edition;
    //    $.getJSON(ApiDataUrl, function (data) {
    //        var liveLeftHtml = '';
    //        $.each(data, function (i, item) {
    //            var slugUrl = convertToSlug(item.Id, item.Title);
    //            var newsImage = item.ImageUrl.indexOf("devdiscourse.blob.core.windows.net") != -1 ? "/remote.axd?" + item.ImageUrl : item.ImageUrl;
    //            if (i < 1) {
    //                var updateHtml = '';
    //                $.each(item.children, function (index, child) {
    //                    updateHtml += '<li><a href="/live-discourse/' + slugUrl + '%23post_' + child.Id + '" class="text-decoration">' + child.Title + '</a></li>';
    //                });
    //                liveLeftHtml += '<a href="/live-discourse/' + slugUrl + '"><div style="padding-bottom:54%;background-image:url(' + newsImage + '?width=640);background-size:cover;background-repeat:no-repeat"></div></a>' +
    //                    '<div class="p-10" style="border-width:0 1px 1px 1px;border-style:solid;border-color:#e1e1e1;">' +
    //                    '<h4 class="header-line-height" style="font-weight: 400"><a href="/live-discourse/' + slugUrl + '" class="text-decoration">' + item.Title + '</a></h4>' +
    //                    '<ul class="no-margin" style="padding-left:15px;">' + updateHtml + '</ul>' +
    //                    '</div>';
    //            } else {
    //                liveLeftHtml += '<a href="/live-discourse/' + slugUrl + '" class="text-decoration"><div class="m-t-10 p-10" style="border:1px solid #e1e1e1;"><h4 class="header-line-height no-margin" style="font-weight: 400">' + item.Title + '</h4></div></a>'
    //            }

    //        });
    //        $('#live-left').html(liveLeftHtml);
    //    });
    //}
   
    function GetNewsAlert() {
        var ApiDataUrl = "/api/Search/GetNewsAlert";
        $.getJSON(ApiDataUrl, function (data) {
            var alertHtml = '';
            $.each(data, function (i, item) {
                var slugUrl = convertToSlug(item.NewsId, item.Title);
                var label = item.Label != null ? item.Label : "agency-wire";
                alertHtml += '<li><a href="/article/' + label + '/' + slugUrl + '" class="text-decoration"><span class="fg-black">●</span> ' + item.Title + '</a></li>';
            });
            $('#news-alert').html(alertHtml);
            $('#example').breakingNews();
        });
    }
    //function GetInterview() {
    //    var ApiDataUrl = '/api/Search/GetInterview/' + encodeURI(_edition);
    //    $.getJSON(ApiDataUrl, function (data) {
    //        var interviewHtml = '';
    //        $.each(data.slice(0, 4), function (i, item) {
    //            var slugUrl = convertToSlug(item.NewsId, item.Title);
    //            var label = item.Label != null ? item.Label : "agency-wire";
    //            var country = item.Country != null ? item.Country : "Global";
    //            var newsImage = item.Image.indexOf("devdiscourse.blob.core.windows.net") != -1 ? "/remote.axd?" + item.Image : item.Image;
    //            if (i == 0) {
    //                interviewHtml = '<div class="col-md-6 no-padding">' +
    //                    '<a href="/article/' + label + '/' + slugUrl + '" class="infocus-item">' +
    //                    '<div class="interview-news bg-highlight lazy" data-src="' + newsImage + '?width=360&amp;height=240&amp;format=jpg&amp;mode=crop">' +
    //                    '<div class="item-caption">' +
    //                    '<div class="news-item-overlay">' +
    //                    '<span class="label label-primary">' + country + '</span>' +
    //                    '<h3 class="no-margin title" title="' + item.Title + '">' + item.Title + '</h3>' +
    //                    '</div>' +
    //                    '</div>' +
    //                    '</div>' +
    //                    '</a>' +
    //                    '</div>';
    //            } else {
    //                interviewHtml += '<div class="col-md-6 no-padding">' +
    //                    '<div class="media news-media-card interview-media-card ">' +
    //                    '<div class="media-left">' +
    //                    '<a href="/article/' + label + '/' + slugUrl + '" aria-label="' + item.Title + '">' +
    //                    '<div class="image-card-2 bg-gray lazy" data-src="' + newsImage + '?width=90&amp;height=80&amp;format=jpg&amp;mode=crop"></div>' +
    //                    '</a>' +
    //                    '</div>' +
    //                    '<div class="media-body">' +
    //                    '<a href="/article/' + label + '/' + slugUrl + '" title="' + item.Title + '" class="text-decoration">' +
    //                    '<h3 class="media-title fg-white">' + item.Title + '</h3>' +
    //                    '</a>' +
    //                    '<p class="fg-white media-meta">' + country + '</p>' +
    //                    '</div>' +
    //                    '</div>' +
    //                    '</div>';
    //            }
    //        });
    //        $('#interview').html(interviewHtml);
    //        $('#interview').after('<div class="clearfix"><a href="/blogs/interview" class="read-more-btn m-t-10 m-b-30">Read More</a></div>');
    //        $('.lazy').lazy();
    //    });
    //}
    //function sectorNews(sector_id, newsSector, newsUrl, newsTitle, appendTo) {
    //    var dataUrl = "/api/DevNews/GetSectorArticle/" + encodeURI(_edition) + "/" + newsSector;
    //    //$('#' + appendTo).html('<div class="h2 text-center text-muted" style="font-weight:100;"><i class="fa fa-circle-o-notch fa-spin fa-fw"></i> Loading...</div>');
    //    $.getJSON(dataUrl, function (data) {
    //       // $('#' + appendTo).html('<h2 class="section-title" title="' + newsTitle + ' NEWS"><span>' + newsTitle + '</span></h2>');
    //        var sectorNewsHtml = '';
    //        $.each(data.slice(0, 5), function (i, item) {
    //            var slugUrl = convertToSlug(item.NewsId, item.Title);
    //            var newsType = "";
    //            if (item.Sector == "Event") {
    //                newsType = item.Sector;
    //            } else if (item.sector == "Blog") {
    //                if (item.SubType != "") {
    //                    newsType = item.SubType;
    //                } else {
    //                    newsType = "Blog";
    //                }
    //            }
    //            var country = "";
    //            if (item.Country != null && item.Country != "") {
    //                var countryArr = item.Country.split(',');
    //                country = countryArr[0];
    //            } else {
    //                country = "Global";
    //            }
    //            var imageContent = "";
    //            var badge = "";
    //            var label = item.Label != null ? item.Label : "agency-wire";
    //            if (item.ImageUrl != null && item.ImageUrl != "/images/defaultImage.jpg" && item.ImageUrl != "/images/newstheme.jpg" && item.ImageUrl != "/images/sector/all_sectors.jpg") {
    //                var newsImage = item.ImageUrl.indexOf("devdiscourse.blob.core.windows.net") != -1 ? "/remote.axd?" + item.ImageUrl : item.ImageUrl;
    //                imageContent = '<div class="media-left"><a href="/article/' + label + '/' + slugUrl + '" aria-label="' + item.Title + '"><div class="image-card bg-gray lazy" data-src="' + newsImage + '?width=90&height=90&format=jpeg&mode=crop"></div></a></div>';
    //            }
    //            if (newsType != "") {
    //                badge = '<span class="pull-right infocus-badge">' + newsType + '</span>';
    //            }
    //            sectorNewsHtml += '<div class="media news-media-card">' + badge + imageContent + '<div class="media-body">' + '<a href="/article/' + label + '/' + slugUrl + '" title="' + item.Title + '" class="text-decoration"><h3 class="media-title">' + item.Title + '</h3>' + '</a>' + '<p class="media-meta">' + country + '</p></div></div>';
    //        });
    //        $('#' + appendTo).append(sectorNewsHtml+'<div class="clearfix"><a href="' + newsUrl + '" class="read-more-btn m-t-10 m-b-30">Read More</a></div>');
    //        $(".lazy").lazy();
    //    });
    //}
    function GetVideoNews() {
        //var ApiDataUrl = "/api/DevNews/GetVideoNews/" + encodeURI(_edition);
        var ApiDataUrl = "/api/Search/GetHomeVideoNews/" + encodeURI(_edition);
        $.getJSON(ApiDataUrl, function (data) {
            var videohtml = '';
            $.each(data, function (i, item) {
                //var slugUrl = convertToSlug(item.NewId, item.Title);
                var slugUrl = convertToSlug(item.Id, item.Title);
                var newsImage = item.FileThumbUrl.indexOf("devdiscourse.blob.core.windows.net") != -1 ? "/remote.axd?" + item.FileThumbUrl : item.FileThumbUrl;
                var label = item.Label != null ? item.Label : "agency-wire";
                videohtml+='<div class="item"><a href="/news/videos/' + slugUrl + '" title="' + item.Title.split('"').join('') + '" class="text-decoration"><div class="news-card bg-gray lazy" data-src="' + newsImage + '?width=435&height=245&format=jpeg&mode=crop&quality=60"><div style="width:100%;height:100%;background-color:rgba(0,0,0,.3);padding:15px"><div class="media" style="bottom:15px;position:absolute"><div class="media-left"><div class="text-center" style="color: #fff; width: 40px; font-size: 17px; border: 2px solid #fff; border-radius: 50%; height: 40px; line-height: 40px; padding-left: 3px;"><span class="fa fa-play"></span></div></div><div class="media-body"><h3 class="no-margin" style="color:#fff;font-size:18px;line-height:1.26">' + item.Title + '</h3></div></div></div></div></a></div>';
            });
            $('#videoNews').html('<h2 class="section-title2"><span style="background-color:#f4f4f4">DevShots</span>' + '</h2><div id="videoCarousel" class="owl-carousel">' + videohtml+'</div>');
            $('.lazy').lazy();
            $(document).find('#videoCarousel').owlCarousel({
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
    //var cachedData = JSON.parse(localStorage.getItem("CacheSector"));
    //if (cachedData != null) {
    //    var cachedSectors = cachedData.Sectors;
    //    $.each(cachedSectors, function (i, item) {
    //        if (item.count > 0 && _saveSector.length <= 3) {
    //            _saveSector.push(item.sector);
    //        }
    //    });
    //    if (_saveSector.length > 0) {
    //        showInterest();
    //    }
    //}
    //function showInterest() {
    //    $.ajax({
    //        url: '/Home/GetYourInterest',
    //        type: 'Get',
    //        data: {
    //            sector: _saveSector.join(","),
    //            reg: _edition
    //        },
    //        dataType: 'html',
    //        success: function (data) {
    //            $('#showYourInterest').html(data);
    //            $('.lazy').lazy();
    //        },
    //        error: function () { }
    //    });
    //}
});