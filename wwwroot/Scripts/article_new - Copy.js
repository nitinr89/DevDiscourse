function convertToSlug(newsId, str) {
    str = str.replace(/^\s+|\s+$/g, ''); str = str.toLowerCase(); var from = "àáäâèéëêìíïîòóöôùúüûñç·/_,:;"; var to = "aaaaeeeeiiiioooouuuunc------"; for (var i = 0, l = from.length; i < l; i++) { str = str.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i)) } str = str.replace(/[^a-z0-9 -]/g, '').replace(/\s+/g, '-').replace(/-+/g, '-'); return newsId + "-" + str
}
(function () {
    var prevNewsUrl = "";
    var imageCopyright = "";
    var adminSection = "";
    var addBannerUrl = '';
    var whatsappIcon = '';
    $(document).find('#regDropDown').change(function () { var date = new Date(); date.setTime(date.getTime() + (24 * 60 * 60 * 1000)); document.cookie = "Edition=" + $(this).val() + "; expires=" + date.toGMTString() + "; path=/"; window.location.href = $(this).find('option:selected').attr('data-to') });
    $(window).scroll(function () { if (skip < 0 && !inCallback) { GetPreviousNews(); } });
    function GetPreviousNews() {
        if (skip > -2 && !inCallback) {
            inCallback = !0; skip++; $("#loading").show();
            if (_sector != "0") {
                prevNewsUrl = '/api/GetPreviousSectorNews/' + newsId + '/' + _sector.split(',')[0] + '/' + _region + '/' + skip;
            } else {
                prevNewsUrl = '/api/GetPreviousNews/' + newsId + '/' + label + '/' + _region + '/' + skip;
            }
            $.ajax({
                url: prevNewsUrl,
                type: 'Get',
                dataType: 'Json',
                success: function (result) {
                    $.each(result, function (i, data) {
                        var subtitle = data.Subtitle == null ? "" : '<h2 class="sub-title">' + data.Subtitle + '</h2>';
                        var label = data.label == null ? "agency-wire" : data.label;
                        var slug = data.Slug == null ? "" : data.Slug;
                        var encodedTitle = encodeURI(data.Title);
                        if (_isAdmin == true) {
                            adminSection = data.Type == "News" ? '<section class="admin-section"><a href="/DevNews/Edit?id=' + data.Id + '"  target="_blank" rel="nofollow" class="btn btn-default btn-sm"><span class="fa fa-edit"></span> Edit</a></section>' : '<section class="admin-section"><a href="/DevNews/EditBlog?id=' + data.Id + '"  target="_blank" rel="nofollow" class="btn btn-default btn-sm"><span class="fa fa-edit"></span> Edit</a></section>';
                        }
                        var firstPublishedIn = '';
                        if (data.Source != null && data.Source != "") {
                            firstPublishedIn = data.Type == 'Blog' ? '<section class="m-t-30"><ul class="list-inline"><li class="f-18">FIRST PUBLISHED IN: </li><li><span class="badge tag">' + data.Source + '</span></li></ul></section>' : '';
                        }
                        var cityHtml = "";
                        if (data.SourceUrl !== null && data.SourceUrl !== "" && data.SourceUrl.toLowerCase().indexOf('washington') != -1) {
                            cityHtml = '<li>Washington DC</li><li class="list-divider">|</li>';
                        } else if (data.SourceUrl !== null && data.SourceUrl !== "" && data.SourceUrl.toLowerCase().indexOf('losangeles') != -1) {
                            cityHtml = '<li>Los Angeles</li><li class="list-divider">|</li>';
                        } else if (data.SourceUrl !== null && data.SourceUrl !== "") {
                            cityHtml = '<li class="text-capitalize">' + data.SourceUrl.toLowerCase() + '</li><li class="list-divider">|</li>';
                        }
                        imageCopyright = data.ImageCopyright == null ? "" : data.ImageCopyright;
                        var imageHtml = '';
                        var BannerIndex = i % 7;
                        var addBannerUrl = bannerArr[BannerIndex];
                        var whatsappIcon2 = '';
                        var amazonAd = '';
                        //var amazonAd = '<div class="advertisement-area m-t-10"><div class="advertisement-title">Advertisement</div><div class="text-center p-5"><a href="https://tracking.vcommission.com/aff_c?offer_id=10014&aff_id=106900&file_id=194481" target="_blank"><img src="https://media.vcommission.com/brand/files/vcm/10014/Sweepstakes_CPL_Travel_Gift_Card_300x250.jpg" width="300" height="250" border="0" class="img-responsive center-block" /></a><img src="https://tracking.vcommission.com/aff_i?offer_id=10014&file_id=194481&aff_id=106900" width="0" height="0" style="position:absolute;visibility:hidden;" border="0" /></div></div>';
                        //var amazonAd = '<div class="advertisement-area m-t-10"> <div class="advertisement-title" style="text-align: center;">Advertisement</div> <div class="text-center"><a href="https://www.accuwebhosting.com/ref/2638-12-1-80.html" target="_blank" rel="nofollow noopener"><img style="border: 0px;" src="https://www.accuwebhosting.com/web-images/banners/brand_website_slow_250x250.jpg" alt="Your website is still loading? Fix it Now with AccuWebHosting" width="250" height="250" /></a></div></div>';
                        var adOnLeft = '';
                        var adOnMobile = '';
                        if (_ismobile == true) {
                            addBannerUrl = mobileBannerArr[BannerIndex];
                            whatsappIcon = '<li><a class="social-btn wa" href="whatsapp://send?text=https://www.devdiscourse.com/article/' + label + '/' + data.Slug + '" rel="nofollow" data-action="share/whatsapp/share" aria-label="Share on whatsapp"><span class="fa fa-whatsapp wa"></span> Whatsapp</a><li>';
                            whatsappIcon2 = '<li><a class="social-btn wa" href="whatsapp://send?text=https://www.devdiscourse.com/article/' + label + '/' + data.Slug + '" rel="nofollow" data-action="share/whatsapp/share" aria-label="Share on whatsapp"><span class="fa fa-whatsapp wa"></span></a><li>';
                            adOnMobile = amazonAd;
                        }
                        else {
                            addBannerUrl = bannerArr[BannerIndex]; whatsappIcon = '';
                            adOnLeft = amazonAd;
                        }
                        if (i > 0) {
                            adOnMobile = '';
                        }
                        var linkUrl = urlLinkArr[BannerIndex];
                        var imageUrl = data.ImageUrl.indexOf("devdiscourse.blob.core.windows.net") != -1 ? "https://www.devdiscourse.com/remote.axd?" + data.ImageUrl : "https://www.devdiscourse.com" + data.ImageUrl;
                        if (data.ImageUrl != null && data.ImageUrl != "/images/defaultImage.jpg" && data.ImageUrl != "/images/newstheme.jpg" && data.ImageUrl != "/images/sector/all_sectors.jpg") {
                            imageHtml = '<figure class="figure"><picture> <source srcset="' + imageUrl + '?width=920&format=webp" type="image/webp"> <source srcset="' + imageUrl + '?width=920&format=jpeg" type="image/jpeg"><img src="' + imageUrl + '?width=920&format=jpeg" class="img-responsive" alt="' + data.Title + '"></picture><figcaption class="fig-caption">' + imageCopyright + '</figcaption></figure>';
                        }
                        var source = "";
                        if (data.Type == 'News') {
                            switch (data.Source) {
                                case "PTI": source = '<li><a href="/pti-stories/">' + data.Source + '</a></li><li class="list-divider">|</li>'; break;
                                case "Reuters": source = '<li><a href="/reuters-stories/">' + data.Source + '</a></li><li class="list-divider">|</li>'; break;
                                case "IANS": source = '<li><a href="/ians-stories/">' + data.Source + '</a></li><li class="list-divider">|</li>'; break;
                                case "Devdiscourse News Desk": source = '<li><a href="/devdiscourse-stories/">' + data.Source + '</a></li><li class="list-divider">|</li>'; break;
                                case "ANI": source = '<li><a href="/ani-stories/">' + data.Source + '</a></li><li class="list-divider">|</li>'; break;
                                case "PR Newswire": source = '<li><a href="/pr-newswire/">' + data.Source + '</a></li><li class="list-divider">|</li>'; break;
                                default: source = '<li><a href="/news-source/' + data.Source + '">' + data.Source + '</a></li><li class="list-divider">|</li>'; break;
                            }
                        } else if (data.Type == "Blog" && data.Author != null && data.Author != "") {
                            var authorImage = '';
                            if (data.Themes != null) {
                                authorImage = data.Themes.IndexOf("devdiscourse.blob.core.windows.net") != -1 ? domainUrl + "/remote.axd?" + data.Themes : 'https://www.devdiscourse.com' + data.Themes;
                            } else {
                                authorImage = data.Avatar;
                            }
                            source = '<li><a href = "/Home/AuthorArticles?fl=' + data.Author.Trim() + '"> <img class="img-circle author-avatar" src="' + authorImage + '?width=30&height=30&mode=crop" onerror="this.src=&#39;/AdminFiles/Logo/img_avatar.png?width=30&height=30&mode=crop&#39;" alt="' + data.Author + '" />' + data.Author.Trim() + '</a></li>';
                        }
                        var shareHtml = '<section class="left-section m-b-20"><div class="fg-black f-18 m-b-10"> Share </div><ul class="list-unstyled no-margin"><li><a onclick="window.open(&#39;https://www.facebook.com/sharer/sharer.php?u=https://www.devdiscourse.com/article/' + label + '/' + slug + '&#39;, &#39;facebook_share&#39;, &#39;height=320, width=640, toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, directories=no, status=no&#39;);" href="javascript:void(0);" title="Share on Facebook" class="social-btn"><span class="fa fa-facebook fb"></span> Facebook</a></li>' +
                            '<li><a onclick="window.open(&#39;http://twitter.com/share?url=https://www.devdiscourse.com/article/' + label + '/' + slug + '&amp;text=' + encodedTitle + '&#39;, &#39;facebook_share&#39;, &#39;height=320, width=640, toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, directories=no, status=no&#39;);" href="javascript:void(0);" title="share on Twitter" class="social-btn"><span class="fa fa-twitter tw"></span> Twitter</a> </li>' +
                            '<li><a onclick="window.open(&#39;https://www.linkedin.com/shareArticle?mini=true&amp;url=https://www.devdiscourse.com/article/' + label + '/' + slug + '&amp;title=' + encodedTitle + '&amp;summary=&amp;source=devdiscourse.com&#39;, &#39;linkedIn_share&#39;, &#39;height=620, width=400, toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, directories=no, status=no&#39;);" href="javascript:void(0);" target="_blank" title="Share on LinkedIn" class="social-btn"><span class="fa fa-linkedin ln"></span> LinkedIn</a></li>' +
                            '<li><a class="social-btn" href="https://www.youtube.com/channel/UC28dlbVXA88OyB83dM8BEYg?sub_confirmation=1" target="_blank" title="Subscribe on Youtube"> <span class="fa fa-youtube yt"></span> Youtube</a> </li>' +
                            whatsappIcon + '</ul></section>';

                        var modifiedOn = new Date(data.ModifiedOn); modifiedOn.setMinutes(modifiedOn.getMinutes() - (new Date).getTimezoneOffset());
                        var publishedOn = new Date(data.PublishedOn);
                        publishedOn.setMinutes(publishedOn.getMinutes() - (new Date).getTimezoneOffset());
                        var tag = data.Tags != null ? data.Tags.split(',').splice(0, 30) : [];
                        var country = data.Country != null ? data.Country.split(',') : [];
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
                            $.each(country, function (i, item) { combineCountry += '<li>' + item.trim() + '</li>'; });
                            countryHtml = '<section class="left-section m-b-20"><div class="fg-black f-18 m-b-10"> Country</div><ul class="list-unstyled no-margin">' + combineCountry + '</ul></section>';
                        }
                        var addBannerUrlHtml = '';
                        if (i == 0) {
                            addBannerUrlHtml = "";
                        } else {
                            addBannerUrlHtml = '<div class="advertisement-area m-b-10"> <div class="advertisement-title">Advertisement</div><a href="' + linkUrl + '" target="_blank" rel="noopener"><img src="' + addBannerUrl + '" class="img-responsive center-block m-t-10 m-b-10" alt="add-banner" /></a></div>';
                        }
                        function formating(input) { return input < 10 ? '0' + input : input; }
                        var html = addBannerUrlHtml + '<div class="article-divider clearfix" data-articleurl="' + data.Slug + '" data-articletitle="' + data.Title + '"><span>Next Article</span></div><article>' +
                            '<h2 class="title">' + data.Title + '</h2>' + subtitle + '<ul class="list-inline metadata">' + source + cityHtml + '<li>Updated: ' + formating(modifiedOn.getDate()) + '-' + formating(modifiedOn.getMonth() + 1) + '-' + modifiedOn.getFullYear() + ' ' + formating(modifiedOn.getHours()) + ':' + formating(modifiedOn.getMinutes()) + ' IST</li>' + '<li class="list-divider">|</li>' + '<li>Created: ' + formating(publishedOn.getDate()) + '-' + formating(publishedOn.getMonth() + 1) + '-' + publishedOn.getFullYear() + ' ' + formating(publishedOn.getHours()) + ':' + formating(publishedOn.getMinutes()) + ' IST</li></ul><hr class="seperator" /><div class="row"><div class="col-md-9 col-md-push-3 col-sm-9 col-sm-push-3" >' + adminSection + imageHtml + '<section class="article-cont">' + data.Description + '</section><section class="m-b-20 visible-xs"><ul class="list-inline no-margin"><li class="f-18 l-h">Share</li><li><a onclick="window.open(&#39;https://www.facebook.com/sharer/sharer.php?u=https://www.devdiscourse.com/article/' + label + '/' + slug + '&#39;, &#39;facebook_share&#39;, &#39;height=320, width=640, toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, directories=no, status=no&#39;);" href="javascript:void(0);" title="Share on Facebook" class="social-btn"><span class="fa fa-facebook fb"></span></a></li>' +
                            '<li><a onclick="window.open(&#39;http://twitter.com/share?url=https://www.devdiscourse.com/article/' + label + '/' + slug + '&amp;text=' + encodedTitle + '&#39;, &#39;facebook_share&#39;, &#39;height=320, width=640, toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, directories=no, status=no&#39;);" href="javascript:void(0);" title="share on Twitter" class="social-btn"><span class="fa fa-twitter tw"></span></a> </li>' +
                            '<li><a onclick="window.open(&#39;https://www.linkedin.com/shareArticle?mini=true&amp;url=https://www.devdiscourse.com/article/' + label + '/' + slug + '&amp;title=' + encodedTitle + '&amp;summary=&amp;source=devdiscourse.com&#39;, &#39;linkedIn_share&#39;, &#39;height=620, width=400, toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, directories=no, status=no&#39;);" href="javascript:void(0);" target="_blank" title="Share on LinkedIn" class="social-btn"><span class="fa fa-linkedin ln"></span></a></li>' +
                            '<li><a class="social-btn" href="https://www.youtube.com/channel/UC28dlbVXA88OyB83dM8BEYg?sub_confirmation=1" target="_blank" title="Subscribe on Youtube"> <span class="fa fa-youtube yt"></span></a> </li>' +
                        whatsappIcon2 + '</ul></section>' + adOnMobile + '<hr />' + tagHtml + firstPublishedIn + '<button onclick="reset(\'https://www.devdiscourse.com/Article/' + data.Slug + '\',' + data.NewsId + ');" class="btn btn-default btn-lg m-b-20 m-t-20 btn-block"><img src="/images/icons/comment_bubble.svg" alt="comments" /> POST / READ COMMENTS</button> <section id="art_' + data.NewsId + '" class="commentbox"> <div id="disqus_thread" style="display:none;"><img src="/images/icons/disqus_loader.svg" class="center-block" /></div> </section> </div> <div class="col-md-3 col-md-pull-9 col-sm-3 col-sm-pull-9 make-me-sticky hidden-xs">' + countryHtml + shareHtml + adOnLeft + ' </div></div></article>';
                        $('#NewsNews').append(html);
                        ga('send', 'pageview', location.pathname);
                        inCallback = !1; $("#loading").hide();
                        if (imageHtml == '') {
                            var totalHeight = 0; $('#art-' + data.NewsId + ' p').each(function () {
                                totalHeight = totalHeight + $(this).outerHeight(!0);
                            });
                            if (totalHeight > 350) {
                                $('#art-' + data.NewsId).height(300);
                            } else {
                                $('#art-' + data.NewsId).css({ height: 'auto' });
                                $('#art-' + data.NewsId + '+.full-read-overlay').remove();
                            }
                        }
                    });
                    if (twttr != null) { twttr.widgets.load(); }
                    if (window.instgrm != null) {
                        window.instgrm.Embeds.process();
                    }
                }, error: function (req, status, error) { console.log(req.responseText); }
            });
        }
    }
    GetVideos();
    function GetVideos() {
        $.get('/api/DevNews/GetVideoNews/' + _region, function (data) {
            var videoHtml = '';
            var videoFirstHtml = '';
            $.each(data.splice(0, 4), function (i, item) {
                var slugUrl = convertToSlug(item.NewId, item.Title);
                var label = item.Label != null ? item.Label : "agency-wire";
                var newsImage = '';
                if (item.ImageUrl != null && item.ImageUrl != "/images/defaultImage.jpg" && item.ImageUrl != "/images/newstheme.jpg" && item.ImageUrl != "/images/sector/all_sectors.jpg") {
                    newsImage = item.ImageUrl.indexOf("devdiscourse.blob.core.windows.net") != -1 ? "/remote.axd?" + item.ImageUrl : item.ImageUrl;
                }
                var country = item.Country != null ? item.Country.split(',') : [];
                var countryText = country.length > 0 ? country[0] : "Global";
                if (i == 0) {
                    videoFirstHtml += '<a href="/article/' + label + '/' + slugUrl + '">' +
                        '<div class="video-cover m-b-20 lazy lazyload" title="' + item.Title + '" data-src="https://www.devdiscourse.com' + newsImage + '?width=555&height=300&mode=crop">' +
                        '<div class="cover-overlay">' +
                        '<h3 class="video-title">' + item.Title + '</h3>' +
                        '<small class="fg-white no-margin">' +
                        countryText +
                        '</small>' +
                        '</div>' +
                        '<div class="video-btn"><span class="fa fa-play"></span></div>' +
                        '</div></a>';
                } else {
                    videoHtml += '<a href="/article/' + label + '/' + slugUrl + '">' +
                        '<div class="media m-b-10" title="' + item.Title + '">' +
                        '<div class="media-left">' +
                        '<div class="video-cover small lazy" data-src="https://www.devdiscourse.com' + newsImage + '?width=100&height=90&mode=crop">' +
                        '<div class="video-btn small"><span class="fa fa-play"></span></div>' +
                        '</div>' +
                        '</div>' +
                        '<div class="media-body">' +
                        '<h4 class="fg-black f-16 font-normal no-margin l-h">' + item.Title + '</h4>' +
                        '<small class="text-muted">' +
                        countryText +
                        '</small>' +
                        '</div>' +
                        '</div></a>';
                }
            });
            if (videoFirstHtml != '') {
                $('#video_section').append('<div class="row"><div class="col-md-6">' + videoFirstHtml + '</div><div class="col-md-6">' + videoHtml + '</div></div>');
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