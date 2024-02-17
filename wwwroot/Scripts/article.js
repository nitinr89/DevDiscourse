function imageLoad(img) {
    var imageContainerHeight = $('#art-' + img + ' .article-image-container').height(); var totalHeight = 0; $('#art-' + img + ' p').each(function () { totalHeight = totalHeight + $(this).outerHeight(!0) }); if (totalHeight > imageContainerHeight + 180 + initHeight) { $('#art-' + img).height(imageContainerHeight + 180 + initHeight) } else { $('#art-' + img).css({ height: 'auto' }); var searchTag = $('#art-' + img + '+.full-read-overlay button').attr('data-tag'); $('#art-' + img + '+.full-read-overlay').remove(); fetchReadAlso(img, searchTag.split('.').join('').split('/').join(' ').split(',').reverse().splice(3, 3).join(',')) }
}
function fetchReadAlso(id, tag) {
    if (tag.trim() == '') { return; }
    $.ajax({ url: '/api/GetTagNews/' + id + '/' + tag, type: "GET", success: function (result) { var alsoArticle = ''; if (result.length > 0) { var content = ''; $.each(result, function (i, item) { content += '<li class="also-title"><h3 class="f-16 no-margin font-normal"> <a href="/article/' + (item.Label == null ? 'agency-wire' : item.Label + '/' + convertToSlug(item.NewId, item.Title)) + '" class="text-decoration"> ' + item.Title + '</a></h3></li>' }); alsoArticle = '<div class="also-container"><div><h2 class="ar-title">ALSO READ</h2>' + '<ul class="ar-ul">' + content + '</ul></div></div>'; $(alsoArticle).insertBefore("#tag_" + id) } }, error: function (req, status, err) { console.log(req.responseText) } })
}
function convertToSlug(newsId, str) {
    str = str.replace(/^\s+|\s+$/g, ''); str = str.toLowerCase(); var from = "àáäâèéëêìíïîòóöôùúüûñç·/_,:;"; var to = "aaaaeeeeiiiioooouuuunc------"; for (var i = 0, l = from.length; i < l; i++) { str = str.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i)) } str = str.replace(/[^a-z0-9 -]/g, '').replace(/\s+/g, '-').replace(/-+/g, '-'); return newsId + "-" + str
}
(function () {
    var prevNewsUrl = "";
    var imageCopyright = "";
    var addBannerUrl = '';
    var whatsappIcon = '';
    $(document).find('#regDropDown').change(function () { var date = new Date(); date.setTime(date.getTime() + (24 * 60 * 60 * 1000)); document.cookie = "Edition=" + $(this).val() + "; expires=" + date.toGMTString() + "; path=/"; window.location.href = $(this).find('option:selected').attr('data-to') });
    $(window).scroll(function () { if (skip < 8 && !inCallback) {  GetPreviousNews()  } });      
    function GetPreviousNews() {
        debugger;
        if (skip > -2 && !inCallback) {
            inCallback = !0; skip++; $("#loading").show();
            if (_sector != "0") {
                debugger
                prevNewsUrl = '/api/DevNewsApi/GetPreviousSectorNews/' + newsId + '/' + _sector.split(',')[0] + '/' + _region + '/' + skip;
            } else { prevNewsUrl = '/api/DevNewsApi/GetPreviousNews/' + newsId + '/' + label + '/' + _region + '/' + skip; }
            $.ajax({
                url: prevNewsUrl, type: 'Get', dataType: 'Json', success: function (data) {
                    debugger
                    if (data == 'done') { $("#loading").hide(); return }
                    var subtitle = data.subtitle == null ? "" : '<h2 class="subtitle">' + data.subtitle + '</h2>';
                    var label = data.label == null ? "agency-wire" : data.label;
                    var slug = data.slug == null ? "" : data.slug;
                    var encodedTitle = encodeURI(data.title);
                    if (_isAdmin == true) { imageCopyright = data.imageCopyright == null ? '<p class="meta" style="margin-bottom:5px;"><a href="/DevNews/Edit?id=' + data.Id + '" target="_blank" rel="nofollow" class="btn btn-primary btn-sm"><span class="fa fa-pencil"></span> Edit</a></p>' : '<p class="meta" style="margin-bottom:5px;">' + data.imageCopyright + ' <a href="/DevNews/Edit?id=' + data.id + '" target="_blank" rel="nofollow" class="btn btn-primary btn-sm"><span class="fa fa-pencil"></span> Edit</a></p>'; }
                    else {
                        imageCopyright = data.imageCopyright == null ? "" : '<p class="meta">' + data.imageCopyright + '</p>';
                    }
                    var imageHtml = ''; var addBannerUrl = bannerArr[skip];
                    if (_ismobile == true) {
                        addBannerUrl = mobileBannerArr[skip];
                        whatsappIcon = '<a class="btn social-btn" href="whatsapp://send?text=https://www.devdiscourse.com/article/' + label + '/' + data.slug + '" rel="nofollow" data-action="share/whatsapp/share" aria-label="Share on whatsapp"><span class="fa fa-whatsapp"></span></a>';
                    }
                    else {
                        addBannerUrl = bannerArr[skip]; whatsappIcon = '';
                    }
                    //.toLowerCase()
                    var linkUrl = urlLinkArr[skip]; var imageUrl = data.imageUrl.indexOf("devdiscourse.blob.core.windows.net") != -1 ? "https://www.devdiscourse.com/remote.axd?" + data.imageUrl : "https://www.devdiscourse.com" + data.imageUrl; if (data.imageUrl != null && data.imageUrl != "/images/defaultImage.jpg" && data.imageUrl != "/images/newstheme.jpg" && data.imageUrl != "/images/sector/all_sectors.jpg") { imageHtml = '<div class="article-image-container"><img src="' + imageUrl + '?width=920" class="img-responsive m-t-10 m-b-10 center-block" onload="imageLoad(' + data.newsId + ')" title="' + data.title + '" alt="' + data.title + '">' + imageCopyright + '</div>' } var source = ""; switch (data.source) { case "PTI": source = '<a href="/pti-stories/">' + data.source + '</a>'; break; case "Reuters": source = '<a href="/reuters-stories/">' + data.source + '</a>'; break; case "IANS": source = '<a href="/ians-stories/">' + data.source + '</a>'; break; case "Devdiscourse News Desk": source = '<a href="/devdiscourse-stories/">' + data.source + '</a>'; break; case "ANI": source = '<a href="/ani-stories/">' + data.source + '</a>'; break; case "PR Newswire": source = '<a href="/pr-newswire/">' + data.source + '</a>'; break; default: source = '<a href="/news-source/' + data.source + '">' + data.source + '</a>'; break }var modifiedOn = new Date(data.modifiedOn); modifiedOn.setMinutes(modifiedOn.getMinutes() - (new Date).getTimezoneOffset()); var publishedOn = new Date(data.publishedOn); publishedOn.setMinutes(publishedOn.getMinutes() - (new Date).getTimezoneOffset()); var tag = data.tags != null ? data.tags.split(',').splice(0, 30) : []; var country = data.country != null ? data.country.split(',') : []; var countryText = country.length > 0 ? '<span class="meta-divider"></span><span>' + country[0] + '</span>' : ""; var tagHtml = ''; var countryHtml = ''; if (tag.length > 0) { var combineTag = ''; tag = $.grep(tag, function (n, i) { return (n.length > 5) }); $.each(tag, function (i, item) { combineTag += '<h2 class="inline-tag"><a href="/news?tag=' + item.trim() + '" class="badge tag">' + item.trim() + '</a></h2>' }); tagHtml = '<div><span class="fg-black f-18">READ MORE ON :</span>' + combineTag + '</div>' } if (country.length > 0) { var combineCountry = ''; $.each(country, function (i, item) { combineCountry += '<span class="badge tag">' + item.trim() + '</span>' }); countryHtml = '<div class="clearfix">' + '<span class="fg-black f-18">COUNTRY :</span>' + '<span>' + combineCountry + '</span>' + '</div>' } function formating(input) { return input < 10 ? '0' + input : input } var html = '<a href="' + linkUrl + '" target="_blank" rel="noopener"><img src="' + addBannerUrl + '" class="img-responsive center-block m-t-10 m-b-20" alt="add-banner" /></a><div class="article-divider clearfix" data-articleurl="' + data.slug + '" data-articletitle="' + data.title + '">Next Article</div><article>' + '<h2 class="title">' + data.title + '</h2>' + subtitle + '<hr class="hrcss" />' + '<div class="news-meta clearfix">' + source + ' ' + (data.sourceUrl == null ? "" : '<span class="meta-divider"></span><span class="text-capitalize">' + data.sourceUrl + '</span>') + countryText + '<p class="news-date" style="margin:0;"><span class="meta-divider date-divider"></span><span>Updated: ' + formating(modifiedOn.getDate()) + '-' + formating(modifiedOn.getMonth() + 1) + '-' + modifiedOn.getFullYear() + ' ' + formating(modifiedOn.getHours()) + ':' + formating(modifiedOn.getMinutes()) + ' IST</span>' + '<span class="meta-divider"></span>' + '<span>Created: ' + formating(publishedOn.getDate()) + '-' + formating(publishedOn.getMonth() + 1) + '-' + publishedOn.getFullYear() + ' ' + formating(publishedOn.getHours()) + ':' + formating(publishedOn.getMinutes()) + ' IST</span></p>' + '<div class="share-links">' + '<a onclick="window.open(&#39;https://www.facebook.com/sharer/sharer.php?u=https://www.devdiscourse.com/article/' + label + '/' + slug + '&#39;, &#39;facebook_share&#39;, &#39;height=320, width=640, toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, directories=no, status=no&#39;);" href="javascript:void(0);" title="Share on Facebook" class="btn social-btn"><span class="fa fa-facebook"></span></a> ' + '<a onclick="window.open(&#39;http://twitter.com/share?url=https://www.devdiscourse.com/article/' + label + '/' + slug + '&amp;text=' + encodedTitle + '&#39;, &#39;facebook_share&#39;, &#39;height=320, width=640, toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, directories=no, status=no&#39;);" href="javascript:void(0);" title="share on Twitter" class="btn social-btn"><span class="fa fa-twitter"></span></a> ' + '<a onclick="window.open(&#39;https://www.linkedin.com/shareArticle?mini=true&amp;url=https://www.devdiscourse.com/article/' + label + '/' + slug + '&amp;title=' + encodedTitle + '&amp;summary=&amp;source=devdiscourse.com&#39;, &#39;linkedIn_share&#39;, &#39;height=620, width=400, toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, directories=no, status=no&#39;);" href="javascript:void(0);" target="_blank" title="Share on LinkedIn" class="btn social-btn"><span class="fa fa-linkedin"></span></a> ' + whatsappIcon + '</div>' + '</div><div class="full-read newsDescArticle" id="art-' + data.newsId + '">' + imageHtml + data.description + tagHtml + countryHtml + '<div class="clearfix" id="tag_' + data.newsId + '"></div><button onclick="reset(\'https://www.devdiscourse.com/Article/' + data.slug + '\',' + data.newsId + ');" class="btn btn-primary m-b-20 m-t-20 btn-block"><span class="fa fa-comments-o"></span> POST / READ COMMENTS</button> <div id="art_' + data.newsId + '" class="commentbox"> <div id="disqus_thread"></div> </div></div><div class="full-read-overlay"><button class="btn btn-primary center-block full-read-btn" data-id="art-' + data.newsId + '" data-tag="' + data.tags + '">READ ARTICLE</button></div></article>';
                    $('#NewsNews').append(html); ga('send', 'pageview', location.pathname); inCallback = !1; $("#loading").hide(); if (imageHtml == '') { var totalHeight = 0; $('#art-' + data.NewsId + ' p').each(function () { totalHeight = totalHeight + $(this).outerHeight(!0) }); if (totalHeight > 350) { $('#art-' + data.NewsId).height(300) } else { $('#art-' + data.newsId).css({ height: 'auto' }); $('#art-' + data.newsId + '+.full-read-overlay').remove(); fetchReadAlso(data.NewsId, data.Tags.split('.').join('').split('/').join(' ').split(',').reverse().splice(3, 3).join(',')) } }
                    console.log(twttr);
                    if (twttr != null) { twttr.widgets.load(); }
                    if (window.instgrm != null) { window.instgrm.Embeds.process(); }
                }, error: function (req, status, error) { console.log(req.responseText); }
            });
        }
    } $.ajax('/Home/GetTrends?id=' + _id + '&filter=Trends').done(function (data) { $("#sideDiv").html('<h2 class="section-title2"><span>TRENDING</span></h2>' + data); $('.lazy').lazy() });
        GetEditionNews("Africa", "AFRICAN EDITIONS", "/");
        if (_region != "South Asia") { GetEditionNews("South Asia", "SOUTH ASIA EDITION", "/south-asia"); }
        if (_region != "Pacific") { GetEditionNews("Pacific", "PACIFIC EDITION", "/pacific"); }
        if (_region != "East and South East Asia") { GetEditionNews("East and South East Asia", "EAST AND SOUTH EAST ASIA EDITION", "/south-east-asia"); }
        if (_region != "Europe and Central Asia") { GetEditionNews("Europe and Central Asia", "EUROPE AND CENTRAL ASIA EDITION", "/europe-central-asia"); }
        if (_region != "North America") { GetEditionNews("North America", "NORTH AMERICA EDITION", "/north-america"); }
        if (_region != "Latin America and Caribbean") { GetEditionNews("Latin America and Caribbean", "LATIN AMERICA AND CARIBBEAN EDITION", "latin-america"); }
    $.ajax('/Home/GetVideoNews?id=' + newsId + '&reg=' + _region + '&filter=article').done(function (e) {
        $("#videoNews").html(e), $(".lazy").lazy()
    }); function GetEditionNews(region, title, editionSlug) {
        $.ajax({
            url: '/api/SearchApi/GetEditionNews/' + _region + '/' + title,
            type: "GET",
            success: function (result) {
                var latestItems = '';
                if (result.news.length > 0) {
                    var content = '';
                    $.each(result.news, function (i, item) {
                        var slugUrl = convertToSlug(item.newId, item.title);
                        var label = item.label != null ? item.label : "agency-wire";
                        var newsType = ''; if (item.type == "Event") {
                            newsType = '<span class="pull-right infocus-badge">' + item.type + '</span>'
                        } else if (item.type == "Blog") {
                            if (item.subType != "" && item.subType != null) {
                                newsType = '<span class="pull-right infocus-badge">' + item.subType + '</span>'
                            } else { newsType = '<span class="pull-right infocus-badge">Blog</span>' }
                        } var imageContent = "";
                        if (item.imageUrl != null && item.imageUrl != "/images/defaultImage.jpg" && item.imageUrl != "/images/newstheme.jpg" && item.imageUrl != "/images/sector/all_sectors.jpg") {
                            var newsImage = item.imageUrl.indexOf("devdiscourse.blob.core.windows.net") != -1 ? "/remote.axd?" + item.imageUrl : item.imageUrl;
                            imageContent = '<div class="media-left"><div class="image-card bg-gray lazy" data-src="' + newsImage + '?width=90&height=90&mode=crop"></div></div>'
                        }
                        content += '<a href="/article/' + label + '/' + slugUrl + '" class="text-decoration"><div class="media news-media-card">' + newsType + imageContent + '<div class="media-body"><h3 class="media-title">' + item.title + '</h3></div></div></a>'
                    }); var urlhtml = ''; if (result.edition == "AFRICA EDITION") {
                        urlhtml = result.edition
                    } else {
                        urlhtml = '<a href="' + editionSlug + '" class="text-decoration">' + result.edition + '</a>'
                    } latestItems = '<div class="clearfix m-t-40"><h2 class="section-title"><span>' + urlhtml + '</span></h2>' + content + '</div>'; if (result.edition == "AFRICAN EDITIONS") { $('#AfricaEditionNews').append(latestItems) } else { $('#EditionNews').append(latestItems) }
                }
            }, error: function (req, status, err) { console.log(req.responseText) }
        })
    } GetLatestNews(); function GetLatestNews() { $.ajax({ url: '/api/SearchApi/GetLatestNews/' + _region + '/', type: "GET", success: function (result) { var latestItems = ''; if (result.length > 0) { var content = ''; $.each(result, function (i, item) { var newsType = ''; if (item.type == "Event") { newsType = '<span class="pull-right infocus-badge">' + item.type + '</span>' } else if (item.type == "Blog") { if (item.subType != "" || item.subType != null) { newsType = '<span class="pull-right infocus-badge">' + item.subType + '</span>' } else { newsType = '<span class="pull-right infocus-badge">Blog</span>' } } content += '<a href="/article/' + (item.label == null ? 'agency-wire' : item.label + '/' + convertToSlug(item.NewId, item.title)) + '" class="text-decoration"><div class="news-media-card m-t-10">' + newsType + '<h3 class="media-title">' + item.title + '</h3></div></a>' }); latestItems = '<h2 class="section-title"><span>LATEST STORIES</span></h2>' + content + '<a href="/news" class="read-more-btn clearfix" style="margin-top:5px">Read More</a>'; $('#latestNews').html(latestItems) } }, error: function (req, status, err) { console.log(req.responseText) } }) } GetOpinion(); function GetOpinion() { var ApiDataUrl = '/api/Search/GetAnalysis/' + _region + '/' + newsId; $.getJSON(ApiDataUrl, function (data) { var alertHtml = ''; $.each(data, function (i, item) { var slugUrl = convertToSlug(item.newsId, item.title); var label = item.label != null ? item.label : "agency-wire"; var Image = item.image.indexOf("devdiscourse.blob.core.windows.net") != -1 ? "/remote.axd?" + item.image : item.image; alertHtml += '<a href="/article/' + label + '/' + slugUrl + '" class="text-decoration"><div class="news-media-card p-t-10 media"><div class="media-body"><h3 class="media-title no-margin">' + item.title + '</h3></div><div class="media-right"><img src="https://www.devdiscourse.com' + Image + '?width=60&height=60&mode=crop" width="60" height="60" class="img-rounded pull-right" alt="' + item.title + '"/></div></div></a>' }); $('#opinion').html('<h2 class="section-title"><span><a href="/blogs" class="text-decoration">OPINION / BLOG / INTERVIEW</a></span></h2>' + alertHtml) }) } $(window).scroll(function () { $('.article-divider').each(function () { if (isScrolledIntoView($(this))) { url = $(this).attr('data-articleurl'); title = $(this).attr('data-articletitle'); ChangeUrl(title, url) } }) }); function isScrolledIntoView(elem) { var jQueryelem = elem; var jQuerywindow = $(window); var docViewTop = jQuerywindow.scrollTop(); var docViewBottom = docViewTop + jQuerywindow.height(); var elemTop = jQueryelem.offset().top; var elemBottom = elemTop + jQueryelem.height(); return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop - 200)) } function ChangeUrl(title, url) { if (typeof (history.pushState) != "undefined") { var obj = { Title: title, Url: url }; history.replaceState(obj, title, url); $('html head').find('title').text(title) } else { alert("Browser does not support HTML5.") } } $(document).on('click', '.full-read-btn', function () {
        var alsoTag = $(this).attr('data-tag'); var alsoId = $(this).attr('data-id'); $('#' + alsoId).css({ height: "auto" }); $(this).parent().remove();
        fetchReadAlso(alsoId.replace('art-', ''), alsoTag.split('.').join('').split('/').join(' ').split(',').reverse().splice(3, 3).join(','))
    }); var secArr = _sector.split(","), sectorArray = []; function SaveData() { localStorage.removeItem("CacheData"); var e = localStorage.getItem("CurrentArticle"); if (null == localStorage.getItem("CacheSector")) { for (var t = 1; t <= 17; t++) { sectorArray.push(new SectorWithCount(t, 0)) }; for (var r = 0; r < sectorArray.length; r++) { for (var o = 0; o < secArr.length; o++) { sectorArray[r].sector == secArr[o]; sectorArray[r].count = parseInt(sectorArray[r].count) + 1 } } sectorArray.sort(SortByCount); var a = { Sectors: sectorArray }; localStorage.setItem("CacheSector", JSON.stringify(a)), localStorage.setItem("CurrentArticle", _id) } else { var c = JSON.parse(localStorage.getItem("CacheSector")); if (_id != e) { localStorage.setItem("CurrentArticle", _id); var n = c.Sectors; $.each(n, function (e, t) { for (var r = 0; r < secArr.length; r++)t.sector == secArr[r] && (t.count = parseInt(t.count) + 1) }); n.sort(SortByCount); a = { Sectors: n }; localStorage.setItem("CacheSector", JSON.stringify(a)) } } } function SortByCount(e, t) { return t.count - e.count } function SectorWithCount(e, t) { this.sector = e; this.count = t } SaveData()
   
})();