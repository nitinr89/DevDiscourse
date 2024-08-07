﻿function convertToSlug(newsId, str) {
    str = str.replace(/^\s+|\s+$/g, ''); str = str.toLowerCase(); var from = "àáäâèéëêìíïîòóöôùúüûñç·/_,:;"; var to = "aaaaeeeeiiiioooouuuunc------"; for (var i = 0, l = from.length; i < l; i++) { str = str.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i)) } str = str.replace(/[^a-z0-9 -]/g, '').replace(/\s+/g, '-').replace(/-+/g, '-'); return newsId + "-" + str
}
(function () {
    var prevNewsUrl = "";
    var imageCopyright = "";
    var adminSection = "";
    var addBannerUrl = '';
    var whatsappIcon = '';
    $(document).find('#regDropDown').change(function () { var date = new Date(); date.setTime(date.getTime() + (24 * 60 * 60 * 1000)); document.cookie = "Edition=" + $(this).val() + "; expires=" + date.toGMTString() + "; path=/"; window.location.href = $(this).find('option:selected').attr('data-to') });
    $(window).scroll(function () {
        if (skip < 0 && !inCallback) {
            GetPreviousNews();
        }
    });
    function GetPreviousNews() {

        if (skip > -2 && !inCallback) {
            inCallback = !0; skip++; $("#loading").show();
            if (_sector != "0") {
                prevNewsUrl = '/api/DevNewsApi/GetPreviousSectorNews/' + newsId + '/' + _sector.split(',')[0] + '/' + _region + '/' + skip;
            } else {
                prevNewsUrl = '/api/DevNewsApi/GetPreviousNews/' + newsId + '/' + label + '/' + _region + '/' + skip;
            }
            $.ajax({
                url: prevNewsUrl,
                type: 'Get',
                dataType: 'Json',
                success: function (result) {
                    $.each(result, function (i, data) {
                        var subtitle = data.subtitle == null ? "" : '<h2 class="sub-title">' + data.subtitle + '</h2>';
                        var label = data.label == null ? "agency-wire" : data.label;
                        var slug = data.slug == null ? "" : data.slug;
                        var encodedTitle = encodeURI(data.title);
                        if (_isAdmin == true) {
                            adminSection = data.type == "News" ? '<section class="admin-section"><a href="/DevNews/Edit?id=' + data.id + '"  target="_blank" rel="nofollow" class="btn btn-default btn-sm"><span class="fa fa-edit"></span> Edit</a></section>' : '<section class="admin-section"><a href="/DevNews/EditBlog?id=' + data.id + '"  target="_blank" rel="nofollow" class="btn btn-default btn-sm"><span class="fa fa-edit"></span> Edit</a></section>';
                        }
                        var firstPublishedIn = '';
                        if (data.source != null && data.source != "") {
                            firstPublishedIn = data.type == 'Blog' ? '<section class="m-t-30"><ul class="list-inline"><li class="f-18">FIRST PUBLISHED IN: </li><li><span class="badge tag">' + data.source + '</span></li></ul></section>' : '';
                        }
                        var cityHtml = "";
                        if (data.sourceUrl !== null && data.sourceUrl !== "" && data.sourceUrl.toLowerCase().indexOf('washington') != -1) {
                            cityHtml = '<li>Washington DC</li><li class="list-divider">|</li>';
                        } else if (data.sourceUrl !== null && data.sourceUrl !== "" && data.sourceUrl.toLowerCase().indexOf('losangeles') != -1) {
                            cityHtml = '<li>Los Angeles</li><li class="list-divider">|</li>';
                        } else if (data.sourceUrl !== null && data.sourceUrl !== "") {
                            cityHtml = '<li class="text-capitalize">' + data.sourceUrl.toLowerCase() + '</li><li class="list-divider">|</li>';
                        }
                        imageCopyright = data.imageCopyright == null ? "" : data.imageCopyright;
                        var imageHtml = '';
                        var BannerIndex = i % 7;
                        var addBannerUrl = bannerArr[BannerIndex];
                        var adBanners = '';
                        var amazonAd = '';
                        //var amazonAd = '<div class="advertisement-area m-t-10"><div class="advertisement-title">Advertisement</div><div class="text-center p-5"><a href="https://tracking.vcommission.com/aff_c?offer_id=10014&aff_id=106900&file_id=194481" target="_blank"><img src="https://media.vcommission.com/brand/files/vcm/10014/Sweepstakes_CPL_Travel_Gift_Card_300x250.jpg" width="300" height="250" border="0" class="img-responsive center-block" /></a><img src="https://tracking.vcommission.com/aff_i?offer_id=10014&file_id=194481&aff_id=106900" width="0" height="0" style="position:absolute;visibility:hidden;" border="0" /></div></div>';
                        //var amazonAd = '<div class="advertisement-area m-t-10"> <div class="advertisement-title" style="text-align: center;">Advertisement</div> <div class="text-center"><a href="https://www.accuwebhosting.com/ref/2638-12-1-80.html" target="_blank" rel="nofollow noopener"><img style="border: 0px;" src="https://www.accuwebhosting.com/web-images/banners/brand_website_slow_250x250.jpg" alt="Your website is still loading? Fix it Now with AccuWebHosting" width="250" height="250" /></a></div></div>';
                        var adOnLeft = '';
                        //var adOnMobile = '';
                        if (_ismobile == true) {
                            addBannerUrl = mobileBannerArr[BannerIndex];
                            var slides = '';

                            for (var j = 0; j < 5; j++) {
                                var activeSlide = '';
                                if (j == 0) {
                                    activeSlide = 'active';
                                }
                                slides += '<div class="item ' + activeSlide + '">' +
                                    '<a href="' + urlLinkArr[j] + '" target="_blank" rel="noreferrer">' +
                                    '<img src="' + mobileBannerArr[j] + '" class="img-responsive" />' +
                                    '</a>' +
                                    '</div>';
                            }
                            adBanners = '<div class="carousel slide custom-carousel" data-ride="carousel"><div class="carousel-inner" role="listbox">' + slides + '</div></div>';
                            whatsappIcon = '<li><a class="social-btn wa" href="whatsapp://send?text=https://www.devdiscourse.com/article/' + label + '/' + data.slug + '" rel="nofollow" data-action="share/whatsapp/share" aria-label="Share on whatsapp"><span class="fa fa-whatsapp wa"></span></a><li>';
                            //adOnMobile = amazonAd;
                        }
                        else {
                            var linkUrl = urlLinkArr[BannerIndex];
                            addBannerUrl = bannerArr[BannerIndex];
                            adBanners = '<a href="' + linkUrl + '" target="_blank" rel="noopener"><img src="' + addBannerUrl + '" class="img-responsive center-block m-t-10 m-b-10" alt="add-banner" /></a>';
                            whatsappIcon = '';
                            //adOnLeft = amazonAd;
                        }
                        //if (i > 0) {
                        //    adOnMobile = '';
                        //}

                        var imageUrl = data.imageUrl.indexOf("devdiscourse.blob.core.windows.net") != -1 ? "/Experiment/Img?imageUrl=" + data.imageUrl : "/Experiment/Img?imageUrl=" + "https://www.devdiscourse.com" + data.imageUrl;
                        if (data.imageUrl != null && data.imageUrl != "/images/defaultImage.jpg" && data.imageUrl != "/images/newstheme.jpg" && data.imageUrl != "/images/sector/all_sectors.jpg") {
                            imageHtml = '<figure class="figure"><picture> <source srcset="' + imageUrl + '&width=920&format=webp" type="image/webp"> <source srcset="' + imageUrl + '&width=920&format=jpeg" type="image/jpeg"><img src="' + imageUrl + '&width=920&format=jpeg" class="img-responsive" alt="' + data.title + '"></picture><figcaption class="fig-caption">' + imageCopyright + '</figcaption></figure>';
                        }
                        var source = "";
                        if (data.type == 'News') {
                            switch (data.source) {
                                case "PTI": source = '<li><a href="/pti-stories/">' + data.source + '</a></li><li class="list-divider">|</li>'; break;
                                case "Reuters": source = '<li><a href="/reuters-stories/">' + data.source + '</a></li><li class="list-divider">|</li>'; break;
                                case "IANS": source = '<li><a href="/ians-stories/">' + data.source + '</a></li><li class="list-divider">|</li>'; break;
                                case "Devdiscourse News Desk": source = '<li><a href="/devdiscourse-stories/">' + data.source + '</a></li><li class="list-divider">|</li>'; break;
                                case "ANI": source = '<li><a href="/ani-stories/">' + data.source + '</a></li><li class="list-divider">|</li>'; break;
                                case "PR Newswire": source = '<li><a href="/pr-newswire/">' + data.source + '</a></li><li class="list-divider">|</li>'; break;
                                default: source = '<li><a href="/news-source/' + data.source + '">' + data.source + '</a></li><li class="list-divider">|</li>'; break;
                            }
                        } else if (data.type == "Blog" && data.author != null && data.author != "") {
                            var authorImage = '';
                            if (data.themes != null) {
                                authorImage = data.themes.IndexOf("devdiscourse.blob.core.windows.net") != -1 ? "/Experiment/Img?imageUrl=" + data.themes : "/Experiment/Img?imageUrl=" + 'https://www.devdiscourse.com' + data.themes;
                            } else {
                                authorImage = data.avatar;
                            }
                            source = '<li><a href = "/Home/AuthorArticles?fl=' + data.author.Trim() + '"> <img class="img-circle author-avatar" src="' + authorImage + '&width=30&height=30&mode=crop" onerror="this.src=&#39;/Experiment/Img?imageUrl=https://www.devdiscourse.com/AdminFiles/Logo/img_avatar.png&width=30&height=30&mode=crop&#39;" alt="' + data.author + '" />' + data.author.Trim() + '</a></li>';
                        }
                        var shareHtml = '<ul class="list-inline no-margin share-button-list"><li class="l-h-28">SHARE</li><li><a onclick="window.open(&#39;https://www.facebook.com/sharer/sharer.php?u=https://www.devdiscourse.com/article/' + label + '/' + slug + '&#39;, &#39;facebook_share&#39;, &#39;height=320, width=640, toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, directories=no, status=no&#39;);" href="javascript:void(0);" title="Share on Facebook" class="social-btn"><span class="fa fa-facebook fb"></span></a></li>' +
                            '<li><a onclick="window.open(&#39;http://twitter.com/share?url=https://www.devdiscourse.com/article/' + label + '/' + slug + '&amp;text=' + encodedTitle + '&#39;, &#39;facebook_share&#39;, &#39;height=320, width=640, toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, directories=no, status=no&#39;);" href="javascript:void(0);" title="share on Twitter" class="social-btn"><span class="fa fa-twitter tw"></span></a> </li>' +
                            '<li><a onclick="window.open(&#39;https://www.linkedin.com/shareArticle?mini=true&amp;url=https://www.devdiscourse.com/article/' + label + '/' + slug + '&amp;title=' + encodedTitle + '&amp;summary=&amp;source=devdiscourse.com&#39;, &#39;linkedIn_share&#39;, &#39;height=620, width=400, toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, directories=no, status=no&#39;);" href="javascript:void(0);" target="_blank" title="Share on LinkedIn" class="social-btn"><span class="fa fa-linkedin ln"></span></a></li>' +
                            '<li><a class="social-btn" href="https://www.youtube.com/channel/UC28dlbVXA88OyB83dM8BEYg?sub_confirmation=1" target="_blank" title="Subscribe on Youtube"> <span class="fa fa-youtube yt"></span></a> </li>' +
                            whatsappIcon + '</ul>';

                        var modifiedOn = new Date(data.modifiedOn); modifiedOn.setMinutes(modifiedOn.getMinutes() - (new Date).getTimezoneOffset());
                        var publishedOn = new Date(data.publishedOn);
                        publishedOn.setMinutes(publishedOn.getMinutes() - (new Date).getTimezoneOffset());
                        var tag = data.tags != null ? data.tags.split(',').splice(0, 30) : [];
                        var country = data.country != null ? data.country.split(',') : [];
                        var tagHtml = '';
                        var countryHtml = '';
                        if (tag.length > 0) {
                            var combineTag = '';
                            tag = $.grep(tag, function (n, i) {
                                return n.length > 5;
                            });
                            $.each(tag, function (i, item) {
                                combineTag += '<li><a href="/news?tag=' + item.trim() + '" class="tag">' + item.trim() + '</a></li>';
                            });
                            tagHtml = '<section><ul class="list-inline"><li class="f-18">READ MORE ON: </li>' + combineTag + '</ul></section>';
                        }
                        if (country.length > 0) {
                            var combineCountry = '';
                            $.each(country, function (i, item) { combineCountry += '<li class="fg-orange">' + item.trim() + '</li>'; });
                            countryHtml = '<ul class="list-inline no-margin pull-left l-h-28"><li>Country:</li>' + combineCountry + '</ul>';
                        }
                        var addBannerUrlHtml = '';
                        if (i == 0) {
                            addBannerUrlHtml = "";
                        } else {
                            addBannerUrlHtml = '<div class="advertisement-area m-b-10"> <div class="advertisement-title">Advertisement</div>' + adBanners + '</div>';
                        }
                        var disclaimer = "";
                        if ((data.source == "Reuters" || data.source == "PTI" || data.source == "IANS" || data.source == "ANI") && !data.description.Contains("(This story has not been edited by Devdiscourse staff and is auto-generated from a syndicated feed.)")) {
                            disclaimer = "<p>(This story has not been edited by Devdiscourse staff and is auto-generated from a syndicated feed.)</p>";
                        }
                        else if (data.source == "Devdiscourse News Desk" && (data.originalSource == "Reuters" || data.originalSource == "PTI" || data.originalSource == "IANS" || data.originalSource == "ANI")) {
                            disclaimer = "<p>(With inputs from agencies.)</p>";
                        }
                        function formating(input) { return input < 10 ? '0' + input : input; }
                        var html = addBannerUrlHtml + '<div class="article-divider clearfix" data-articleurl="' + data.slug + '" data-articletitle="' + data.title + '"><span>Next Article</span></div><article>' +
                            '<h2 class="title">' + data.title + '</h2>' + subtitle + '<hr class="seperator" />' +
                            '<div class="m-b-10 m-t-10">' +
                            '<ul class="list-inline metadata">' + source + cityHtml + '<li>Updated: ' + formating(modifiedOn.getDate()) + '-' + formating(modifiedOn.getMonth() + 1) + '-' + modifiedOn.getFullYear() + ' ' + formating(modifiedOn.getHours()) + ':' + formating(modifiedOn.getMinutes()) + ' IST</li>' + '<li class="list-divider">|</li>' + '<li>Created: ' + formating(publishedOn.getDate()) + '-' + formating(publishedOn.getMonth() + 1) + '-' + publishedOn.getFullYear() + ' ' + formating(publishedOn.getHours()) + ':' + formating(publishedOn.getMinutes()) + ' IST</li></ul>' +
                            '</div>' + adminSection + imageHtml +
                            '<section class="left-section clearfix">' + countryHtml + shareHtml + '</section>' +
                            '<hr class="seperator m-b-20" />' +
                            '<section class="article-cont">' + data.description + disclaimer + '</section>' + tagHtml + firstPublishedIn +
                            '<button onclick="reset(\'https://www.devdiscourse.com/Article/' + data.slug + '\',' + data.newsId + ');" class="btn btn-default btn-lg m-b-20 m-t-20 btn-block"><img src="/images/icons/comment_bubble.svg" alt="comments" /> POST / READ COMMENTS</button> <section id="art_' + data.newsId + '" class="commentbox"> <div id="disqus_thread" style="display:none;"><img src="/images/icons/disqus_loader.svg" class="center-block" /></div> </section>' +
                            '</article>';

                        $('#NewsNews').append(html);
                        ga('send', 'pageview', location.pathname);
                        inCallback = !1; $("#loading").hide();
                        if (imageHtml == '') {
                            var totalHeight = 0; $('#art-' + data.newsId + ' p').each(function () {
                                totalHeight = totalHeight + $(this).outerHeight(!0);
                            });
                            if (totalHeight > 350) {
                                $('#art-' + data.newsId).height(300);
                            } else {
                                $('#art-' + data.newsId).css({ height: 'auto' });
                                $('#art-' + data.newsId + '+.full-read-overlay').remove();
                            }
                        }
                    });
                    $('.carousel').carousel({
                        interval: 2000
                    })
                    if (twttr != null) { twttr.widgets.load(); }
                    if (window.instgrm != null) {
                        window.instgrm.Embeds.process();
                    }
                }, error: function (req, status, error) { /*console.log(req.responseText);*/ }
            });
        }
    }

    GetVideos();
    function GetVideos() {
        $.get('/api/Search/GetHomeVideoNews/' + _region, function (data) {
            var videoHtml = '';
            var videoFirstHtml = '';
            //console.log(data);
            $.each(data.splice(0, 4), function (i, item) {
                var slugUrl = convertToSlug(item.id, item.title);
                // var label = item.Label != null ? item.Label : "agency-wire";
                var newsImage = '';
                if (item.fileThumbUrl != null && item.fileThumbUrl != "/images/defaultImage.jpg" && item.fileThumbUrl != "/images/newstheme.jpg" && item.fileThumbUrl != "/images/sector/all_sectors.jpg") {
                    newsImage = item.fileThumbUrl.indexOf("devdiscourse.blob.core.windows.net") != -1 ? "/Experiment/Img?imageUrl=" + item.fileThumbUrl : "/Experiment/Img?imageUrl=" + "https://www.devdiscourse.com" + item.fileThumbUrl;
                }
                var country = item.country != null ? item.country.split(',') : [];
                var countryText = country.length > 0 ? country[0] : "Global";

                videoHtml += '<div class="col-md-12 col-sm-12">' +
                    '<a href="/news/videos/' + slugUrl + '">' +
                    '<div class="video-cover m-b-20 lazy lazyloaded" title="' + item.title + '" style="background-image: url(&quot;' + newsImage + '&width=555&amp;height=300&amp;mode=crop&quot;);">' +
                    '<div class="cover-overlay">' +
                    '<div class="video-btn"><span class="fa fa-play"></span></div>' +
                    '<h3 class="video-title">' + item.title + '</h3>' +
                    '</div>' +
                    '</div>' +
                    '</a>' +
                    '</div>';
            });
            if (videoHtml != '') {
                $('#video_section').append('<div class="row">' + videoHtml + '</div>');
            } else {
                $('#video_section').hide();
            }

            $('.lazy').lazy();
        });
    }
    $(window).scroll(function () {
        $('.article-divider').each(function () {
            if (isScrolledIntoView($(this))) {
                url = $(this).attr('data-articleurl');
                title = $(this).attr('data-articletitle');
                ChangeUrl(title, url);
            }
        });
    });
    function isScrolledIntoView(elem) {
        var jQueryelem = elem;
        var jQuerywindow = $(window);
        var docViewTop = jQuerywindow.scrollTop();
        var docViewBottom = docViewTop + jQuerywindow.height();
        var elemTop = jQueryelem.offset().top; var elemBottom = elemTop + jQueryelem.height();
        return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop - 200));
    }
    function ChangeUrl(title, url) {
        if (typeof (history.pushState) != "undefined") {
            var obj = { Title: title, Url: url }; history.replaceState(obj, title, url); $('html head').find('title').text(title)
        } else { alert("Browser does not support HTML5."); }
    }
    $(document).on('click', '.full-read-btn', function () {
        var alsoId = $(this).attr('data-id'); $('#' + alsoId).css({ height: "auto" }); $(this).parent().remove();
    });
    var secArr = _sector.split(","), sectorArray = [];
    function SaveData() {
        localStorage.removeItem("CacheData");
        var e = localStorage.getItem("CurrentArticle");
        if (null == localStorage.getItem("CacheSector")) {
            for (var t = 1; t <= 17; t++) {
                sectorArray.push(new SectorWithCount(t, 0));
            }
            for (var r = 0; r < sectorArray.length; r++) {
                for (var o = 0; o < secArr.length; o++) {
                    sectorArray[r].sector == secArr[o];
                    sectorArray[r].count = parseInt(sectorArray[r].count) + 1;
                }
            } sectorArray.sort(SortByCount); var a = { Sectors: sectorArray };
            localStorage.setItem("CacheSector", JSON.stringify(a)), localStorage.setItem("CurrentArticle", _id);
        } else {
            var c = JSON.parse(localStorage.getItem("CacheSector"));
            if (_id != e) {
                localStorage.setItem("CurrentArticle", _id);
                var n = c.Sectors; $.each(n, function (e, t) {
                    for (var r = 0; r < secArr.length; r++)t.sector == secArr[r] && (t.count = parseInt(t.count) + 1);
                });
                n.sort(SortByCount); a = { Sectors: n }; localStorage.setItem("CacheSector", JSON.stringify(a));
            }
        }
    } function SortByCount(e, t) { return t.count - e.count; }
    function SectorWithCount(e, t) { this.sector = e; this.count = t; }
    SaveData();
})();