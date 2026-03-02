var surveyUrl = 'https://ideame.azurewebsites.net/Survey/GetSingleQuestion';
var surveySubmitUrl = 'https://ideame.azurewebsites.net/Survey/SubmitResearchSurvey';
var currentLat = 0;
var currentLong = 0;
function getCountries(appendTo, id) {
    if (localStorage.SurveyResponseData != null) {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(successLocationGeo, errorLocationGeo);
        }
    } else {
        getQuestions(appendTo, id)
    }
    function successLocationGeo(position) {
        currentLat = position.coords.latitude;
        currentLong = position.coords.longitude
        getQuestions(appendTo, id);
    }
    function errorLocationGeo(error) {
        switch (error.code) {
            case error.PERMISSION_DENIED:
                console.log("User denied the request for Geolocation.");
                break;
            case error.POSITION_UNAVAILABLE:
                console.log("Location information is unavailable.");
                break;
            case error.TIMEOUT:
                console.log("The request to get user location timed out.");
                break;
            case error.UNKNOWN_ERROR:
                console.log("An unknown error occurred.");
                break;
        }
    }

}
function getQuestions(appendTo, id) {
    $.ajax({
        url: surveyUrl,
        type: 'GET',
        data: { id: id },
        dataType: 'Json',
        success: function (data) {
            var respId = "";
            $.each(data, function (i, item) {
                $('#' + appendTo).html('<div class="survey-title-research">Please provide feedback for Participatory monitoring of SDGs</div><div id="cont_' + appendTo+'" class="survey-cont"> <h4 class="survey-title">' + item.Title + '</h4></div>');
                item.opt.forEach(function (option, index) {
                    $('#cont_' + appendTo).append('<div class="option-item"><input type="radio" id="' + (index + 1) + '" name="' + item.Id + '" data-survey="' + id + '" title="' + option + '" class="surveyRadio"><label for="' + (index + 1) + '"  class="option-label">' + option + '</label></div>');
                    respId = item.Id;
                });
            });
            var ResponseArray = [];
            var alreadyAnswered = false;
            if (localStorage.SurveyResponseData != null) {
                ResponseArray = JSON.parse(localStorage.SurveyResponseData);
                var filterResponseArray = ResponseArray.filter(function (resp) { return resp.id == respId; });
                
                if (filterResponseArray.length > 0) {
                    $.each(filterResponseArray, function (i, item) {
                        if (item.lat >= (currentLat - .05) && item.lat <= (currentLat + .05) && item.lon >= (currentLong - .05) && item.lon <= (currentLong + .05)) {
                            alreadyAnswered = true;
                        }
                    });
                    if (alreadyAnswered === true) {
                        getQuestions(appendTo, id);
                    }
                }
            }
            if (alreadyAnswered === false) {
                var Att_html = '<input type="hidden" id="imageUrl"/><div class="progress" progress_id="img_' + id + '" style="height:3px;margin-bottom:5px;display:none;"><div class="progress-bar progress-bar-success" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width:0%;"></div></div>' + '<label class="res-label" for="image-attachment">Image Attachment</label><input type="file" id="image-attachment" att_id="img_' + id + '" class="response_attachment" data-ext="imageUrl" accept="image/*">' +
                    '<input type="hidden" id="audioUrl"/><div class="progress" progress_id="aud_' + id + '" style="height:3px;margin-bottom:5px;display:none;"><div class="progress-bar progress-bar-success" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width:0%;"></div></div>' + '<label class="res-label" for="audio-attachment">Audio Attachment</label><input type="file" id="audio-attachment" att_id="aud_' + id + '" class="response_attachment" data-ext="audioUrl" accept="audio/*">' +
                    '<input type="hidden" id="videoUrl"/><div class="progress" progress_id="vid_' + id + '" style="height:3px;margin-bottom:5px;display:none;"><div class="progress-bar progress-bar-success" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width:0%;"></div></div>' + '<label class="res-label" for="video-attachment">Video Attachment</label><input type="file" id="video-attachment" att_id="vid_' + id + '" class="response_attachment" data-ext="videoUrl" accept="video/*">' +
                    '<button type="button" class="btn btn-primary m-t-10" id="SubmitBtn" style="display:none;">Submit</button>'
                $('#cont_' + appendTo).append(Att_html);
                $('.survey-main').show();
                setTimeout(function () {
                    $('.survey-main').addClass('in-active');
                }, 1500);
            }
        },
        error: function () {
        }
    });
}
$(document).on('change', '.response_attachment', function () {
    if (window.FormData !== undefined) {
        var fileUpload = $(this).get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
        var UploadType = $(this).data("ext");
        var progressId = $(this).attr("att_id");
        fileData.append("att_id", $(this).attr("att_id"));
        $.ajax({
            url: 'https://ideame.azurewebsites.net/SurveyBuilder/UploadAttachmentForWeb',
            type: "POST",
            xhr: function () {
                var myXhr = $.ajaxSettings.xhr();
                $('div[progress_id="' + progressId + '"] .progress-bar').css({ "width": "0" });
                if (myXhr.upload) {
                    myXhr.upload.addEventListener('progress', progress, false);
                    $('div[progress_id="' + progressId + '"]').show();
                }
                return myXhr;
            },
            contentType: false,
            processData: false,
            data: fileData,
            success: function (result) {
                if (result == "Error") {
                    alert('Something went wrong!');
                } else {
                    $("#" + UploadType).val(result.value);
                }
            },
            error: function (req, status, err) {
                //$("body").html(req.responseText)
            }
        });
        function progress(e) {
            if (e.lengthComputable) {
                var max = e.total;
                var current = e.loaded;
                var Percentage = (current * 100) / max;
                Percentage = parseInt(Percentage);
                $('div[progress_id="' + progressId + '"] .progress-bar').css({ "width": Percentage + "%" });
                if (Percentage >= 100) {
                    $('div[progress_id="' + progressId + '"]').hide();
                    $('label[for="' + progressId + '_id"] .status').html('<span class="fa fa-check"></span>');
                }
            }

        }
    }
});
$(document).on("change", ".surveyRadio", function () {
    $("#SubmitBtn").show();
});
$(document).on("click", "#SubmitBtn", getLocation);
function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(successLocation, errorLocation);
    } else {
        console.log("Geolocation is not supported by this browser.");
    }
   
    function successLocation(position) {
        var GEOCODING = 'https://maps.googleapis.com/maps/api/geocode/json?latlng=' + position.coords.latitude + '%2C' + position.coords.longitude + '&language=en&key=AIzaSyCeQCMZAeIW-9-dX1wVbNbSOWZpwn31HNs';
        $.getJSON(GEOCODING).done(function (location) {
            var surveyId = $('.surveyRadio:checked').attr('data-survey');
            var responseArray = [];
            if (localStorage.SurveyResponseData != null) {
                responseArray = JSON.parse(localStorage.SurveyResponseData)
            } 
            var newResLoc = new surveyResponseLocation(surveyId, position.coords.latitude, position.coords.longitude);
            responseArray.push(newResLoc);
            var locAddress = "";
            if (location.results.length != 0) {
                locAddress = location.results[0].formatted_address;
            }
            console.log("locAddress", locAddress)
            localStorage.SurveyResponseData = JSON.stringify(responseArray);
            SubmitSurvey(surveyId, position.coords.latitude + ',' + position.coords.longitude, locAddress , $('.surveyRadio:checked').attr('name'), $('.surveyRadio:checked').attr('title'), $('#imageUrl').val(), $('#audioUrl').val(), $('#videoUrl').val());
        });
    }
    function errorLocation(error) {
        switch (error.code) {
            case error.PERMISSION_DENIED:
                console.log("User denied the request for Geolocation.");
                break;
            case error.POSITION_UNAVAILABLE:
                console.log("Location information is unavailable.");
                break;
            case error.TIMEOUT:
                console.log("The request to get user location timed out.");
                break;
            case error.UNKNOWN_ERROR:
                console.log("An unknown error occurred.");
                break;
        }
    }
}
var surveyResponseLocation = function (id, lat,lon) {
    this.id = id;
    this.lat = lat;
    this.lon = lon;
}
function SubmitSurvey(surveyId, location, locationTitle, scId, value,imgUrl,audUrl,vidUrl) {
    $.ajax({
        url: surveySubmitUrl,
        type: 'Post',
        data: { SurveyId: surveyId, location: location, locationTitle: locationTitle, scId: scId, value: value, imgUrl: imgUrl, audUrl: audUrl, vidUrl: vidUrl },
        dataType: 'Json',
        success: function (data) {
            console.log("here",data)
            $('#survey').html('<h3 class="text-center no-margin" style="color:#ff6a00;"><span style="font-size: 2em;line-height: 1em;">☺</span> Thanks</h3><p class="text-center">To View response on map <a href="/Research#lms" target="_blank">click here</a></p>');
        },
        error: function (req, status, err) {
            console.log("error", res.responseText)
           // $('#survey').html(res.responseText);
        },
    });
}