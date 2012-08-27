var isAuthenticated = false;

// Display a client side message
function displayMessage(selector, msg, type) {
    // type: success, error, attention, information
    var wrapper = $(selector);
    wrapper.html("");
    var a = $("<a>").addClass("close").attr("href", "#");
    var div = $("<div>").addClass("notification png_bg " + type).attr("style", "display: none; opacity: 0").append(a).append("<div>" + msg + "</div>");
    wrapper.append(div);
    div.show(200, function () { div.fadeTo(200, 1) });
}

// Close button on notification
$(document).ready(function () {
    $(".notification .close").live("click", function () {
        $(this).parent().fadeTo(200, 0, function () { // Links with the class "close" will close parent
            $(this).slideUp(300);
        });
        return false;
    });
});

function initLogin() {
    $("img.open-id").click(function () {
        var site = $(this).attr("data-provider");
            
        if (site == "google")
            value = "https://www.google.com/accounts/o8/id";
        else if (site == "yahoo")
            value = "https://me.yahoo.com/"
        else
            return false;

        $("#loading").html("<img src='/content/images/loading.gif' style='margin-left: 50px;' />");
        $("#openid_identifier").val(value);
        setTimeout( function () { $("#open-form").submit(); }, 300);
    });
}

function initTinyMCE() {
    tinyMCE.init({
        // General options
        mode: "exact",
        theme: "advanced",
        skin: "o2k7",
        elements: "editor",
        plugins: "autolink,tabfocus,paste",
        content_css: false,
        // Theme options
        theme_advanced_buttons1: "bold,italic,underline,|,fontsizeselect,forecolor,|,link,unlink,|,removeformat",
        theme_advanced_buttons2: "",
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_resizing: false,
        tab_focus: ':prev,:next',
        relative_urls: false,
        height: '300',
        paste_text_sticky: true,
        paste_auto_cleanup_on_paste : true,
        paste_remove_styles: true,
        paste_remove_spans: true,
        setup: function (ed) {
            ed.onInit.add(function (ed) {
                ed.pasteAsPlainText = true;
            });
        }
    });
}

$(document).ready(function () {
    // Vote up
    $('.upvote-link').live("click", function (e) {
        e.preventDefault();
        if (isAuthenticated) {
            var idStr = $(this).attr('data-id').split("_");
            var entity = idStr[1];
            var id = idStr[2];
            var url = "";

            if (entity == "Answer") {
                url = "/Questions/VoteAnswer";
            }
            else if (entity == "Question") {
                url = "/Questions/VoteQuestion";
            }
            else if (entity == "NewsComment") {
                url = "/NewsComments/VoteComment";
            }
            else if (entity == "TipComment") {
                url = "/TipComments/VoteComment";
            }
            else if (entity == "RecipeComment") {
                url = "/RecipeComments/VoteComment";
            }
            else if (entity == "Tip") {
                url = "/Tips/VoteTip";
            }
            if (url == "")
                return false;

            $.ajax({
                type: "POST",
                url: url,
                dataType: "html",
                data: {
                    id: id,
                    isUpVote: true
                },
                success: function (e) {
                    var data = JSON.parse(e);
                    if (data.Success) {
                        $('a[data-id|="up_' + entity + '_' + id + '"]').attr("class", "selectedupvote-link");
                        $('a[data-id|="down_' + entity + '_' + id + '"]').attr("class", "downvote-link");
                        if (data.Data[0] != -1)
                            $('li[data-id|="upcount_' + entity + '_' + id + '"]').html(data.Data[0]);
                        if (data.Data[1] != -1)
                            $('li[data-id|="downcount_' + entity + '_' + id + '"]').html(data.Data[1]);
                    } else {
                        if (data.Message == "NotAuthorized")
                            showUnauthorizedMessage();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.statusText);
                    alert(thrownError);
                },
                complete: function () {
                }
            });
        }
        else {
            showUnauthorizedMessage();
        }
    });

    //Vote down
    $('.downvote-link').live("click", function (e) {
        e.preventDefault();
        if (isAuthenticated) {
            var idStr = $(this).attr('data-id').split("_");
            var entity = idStr[1];
            var id = idStr[2];
            var url = "";

            if (entity == "Answer") {
                url = "/Questions/VoteAnswer";
            }
            else if (entity == "Question") {
                url = "/Questions/VoteQuestion";
            }
            else if (entity == "NewsComment") {
                url = "/NewsComments/VoteComment";
            }
            else if (entity == "TipComment") {
                url = "/TipComments/VoteComment";
            }
            else if (entity == "RecipeComment") {
                url = "/RecipeComments/VoteComment";
            }
            else if (entity == "Tip") {
                url = "/Tips/VoteTip";
            }
            if (url == "")
                return false;

            $.ajax({
                type: "POST",
                url: url,
                dataType: "html",
                data: {
                    id: id,
                    isUpVote: false
                },
                success: function (e) {
                    var data = JSON.parse(e);
                    if (data.Success) {
                        $('a[data-id|="down_' + entity + '_' + id + '"]').attr("class", "selecteddownvote-link");
                        $('a[data-id|="up_' + entity + '_' + id + '"]').attr("class", "upvote-link");
                        if (data.Data[1] != -1)
                            $('li[data-id|="downcount_' + entity + '_' + id + '"]').html(data.Data[1]);
                        if (data.Data[0] != -1)
                            $('li[data-id|="upcount_' + entity + '_' + id + '"]').html(data.Data[0]);
                    } else {
                        if (data.Message == "NotAuthorized")
                            showUnauthorizedMessage();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.statusText);
                    alert(thrownError);
                },
                complete: function () {
                }
            });
        }
        else {
            showUnauthorizedMessage();
        }
    });

    //Report spam
    $('.report-spam').live("click", function (e) {
        e.preventDefault();
        if (isAuthenticated) {
            var done = $(this).attr('data-done');
            if (done == "true") {
                alert("Báo cáo vi phạm hoàn tất! Ban quan trị sẽ kiểm tra lại trong thời gian sớm nhất.");
                return false;
            }
            var idStr = $(this).attr('data-id').split("_");
            var entity = idStr[1];
            var reportId = idStr[2];
            var url = "";
            var title = "";

            if (entity == "Answer") {
                url = "/Questions/ReportAnswer";
                title = "câu trả lời";
            }
            else if (entity == "Question") {
                url = "/Questions/ReportQuestion";
                title = "câu hỏi";
            }
            else if (entity == "NewsComment") {
                url = "/NewsComments/ReportComment";
                title = "bình luận";
            }
            else if (entity == "TipComment") {
                url = "/TipComments/ReportComment";
                title = "bình luận";
            }
            else if (entity == "RecipeComment") {
                url = "/RecipeComments/ReportComment";
                title = "bình luận";
            }
            else if (entity == "Place") {
                url = "/Places/ReportPlace";
                title = "địa điểm";
            }
            else if (entity == "News") {
                url = "/News/ReportNews";
                title = "tin tức";
            }
            else if (entity == "Tip") {
                url = "/Tips/ReportTip";
                title = "mẹo vặt";
            }
            else if (entity == "Recipe") {
                url = "/Recipes/ReportRecipe";
                title = "công thức nấu ăn";
            }
            if (url == "")
                return false;

            $("#reporttextarea").val("");

            $("#reportdialog").dialog("option", "title", "Báo cáo " + title + " vi phạm");
            $("#reportdialog").dialog("option", "buttons", {
                "Báo cáo": function () {
                    if (isAuthenticated) {
                        var reason = $("#reporttextarea").val();
                        if (reason.length > 100) {
                            alert("Lý do không dài quá 100 ký tự");
                            return false;
                        }
                        var button = $(this);

                        $.ajax({
                            type: "POST",
                            url: url,
                            dataType: "html",
                            data: {
                                reportId: reportId,
                                Reason: reason
                            },
                            success: function (e) {
                                var data = JSON.parse(e);
                                if (data.Success) {

                                    $('a[data-id|="report_' + entity + '_' + reportId + '"]').attr("data-done", "true");

                                    button.dialog("close");
                                    alert("Báo cáo vi phạm hoàn tất! Ban quan trị sẽ kiểm tra lại trong thời gian sớm nhất.");
                                } else {
                                    if (data.Message == "NotAuthorized")
                                        showUnauthorizedMessage();
                                }
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert(xhr.statusText);
                                alert(thrownError);
                            },
                            complete: function () {
                            }
                        });
                    }
                    else {
                        showUnauthorizedMessage();
                    }
                },
                "Bỏ qua": function () {
                    $(this).dialog("close");
                }
            });

            $("#reportdialog").dialog('open');
        }
        else {
            showUnauthorizedMessage();
        }
    });

    $(".removequestion").live('click', function (e) {
        e.preventDefault();
        if (isAuthenticated) {

            var idStr = $(this).attr('data-id').split("_");
            var entity = idStr[1];
            var id = idStr[2];
            var createdById = idStr[3];
            var url = "";

            if (entity == "Answer") {
                url = "/Questions/DeleteAnswer";
            }
            else if (entity == "NewsComment") {
                url = "/NewsComments/DeleteComment";
            }
            else if (entity == "TipComment") {
                url = "/TipComments/DeleteComment";
            }
            else if (entity == "RecipeComment") {
                url = "/RecipeComments/DeleteComment";
            }
            if (url == "")
                return false;

            $.ajax({
                type: "POST",
                url: url,
                dataType: "html",
                data: {
                    id: id,
                    createdById: createdById
                },
                success: function (e) {
                    var data = JSON.parse(e);
                    if (data.Success) {
                        $("#li" + entity + "_" + id).fadeTo(300, 0, function () { $(this).hide(300, function () { $(this).remove(); }); });

                        var total = parseInt($("#total" + entity).html());
                        if (total > 0)
                            $("#total" + entity).html(total - 1);
                    } else {
                        if (data.Message == "NotAuthorized")
                            showUnauthorizedMessage();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.statusText);
                    alert(thrownError);
                },
                complete: function () {
                }
            });
        }
        else {
            showUnauthorizedMessage();
        }
    });
});

/********************* Category Suggesting *********************/
var currentCatKeyword = null;
var selectedCat = 0;
var totalCat = 0;
var categoryInput;
var catSuggest;
var catTimeout = null;

function setCurrentCat(current) {
    if (totalCat != 0) {
        $("#cat-sl li").removeClass("current");
        $("#cat-sl li").eq(current).addClass("current");
        selectedCat = current;
        currentCatKeyword = $("#cat-sl li").eq(current).html();
        categoryInput.val(currentCatKeyword);
    }
}

function highlight(current) {
    if (totalCat != 0) {
        $("#cat-sl li").removeClass("highlight");
        $("#cat-sl li").eq(current).addClass("highlight");
    }
}

function getCatSuggestions() {
    var newKeyword = categoryInput.val();
    if (newKeyword != currentCatKeyword) {
        currentCatKeyword = newKeyword;
        if (currentCatKeyword != "") {
            $.post('/search/GetMatchedCategories', { query: currentCatKeyword }, function (data) {
                // replace list by new ones
                var cul = $("#cat-sl");
                $(cul).html("");
                $.each(data, function (i, o) {
                    $(cul).append("<li>" + o.Name + "</li>");
                });
                totalCat = data.length;
                selectedCat = -1;
                if (totalCat > 0)
                    catSuggest.css("display", "block");
                else
                    catSuggest.css("display", "none");
            });
        } else {
            $("#cat-sl").html("");
            totalCat = 0;
            catSuggest.css("display", "none");
        }
    }
}

function initCategorySuggestion() {
    // Init position of search suggestions
    categoryInput = $("#cat-q");
    catSuggest = $("#category-suggestion");
    catSuggest.offset({ top: categoryInput.offset().top + 27, left: categoryInput.offset().left });
    catSuggest.css("min-width", categoryInput.outerWidth() - 2);

    // highlight category selection when moving up/down
    categoryInput.keydown(function (e) {
        switch (e.keyCode) {
            case 27: // escape
                e.preventDefault();
                catSuggest.css("display", "none");
                break;
            case 13: // enter
                if (catSuggest.css("display") != "none" && selectedCat != -1) {
                    e.preventDefault();
                    catSuggest.css("display", "none");
                }
                break;
            case 33: // page up
                e.preventDefault();
                setCurrentCat(0);
                break;
            case 34: // page down
                e.preventDefault();
                setCurrentCat(totalCat - 1);
                break;
            case 38: // up
                e.preventDefault();
                setCurrentCat(selectedCat <= 0 ? totalCat - 1 : selectedCat - 1);
                break;
            case 40: // down
                e.preventDefault();
                setCurrentCat(selectedCat >= totalCat - 1 ? 0 : selectedCat + 1);
                break;
        }
    });

    // Highlight when mouse enter
    $("#cat-sl li").live("mouseenter", function () {
        highlight($("#cat-sl li").index(this));
    });

    // Populate back to search input when click from suggestions
    $("#cat-sl li").live("mousedown", function () {
        selectedCat = $("#cat-sl li").index(this);
        currentCatKeyword = $(this).html();
        categoryInput.val(currentCatKeyword);
    });

    // Disable suggestions when lost focus
    categoryInput.blur(function () {
        catSuggest.css("display", "none");
    });

    categoryInput.keyup(function (e) {
        if (catTimeout) {
            clearTimeout(catTimeout);
            catTimeout = null;
        }
        catTimeout = setTimeout(getCatSuggestions, 100);
    });
}

/********************* Address Part Suggesting *********************/
var currentAddrKeyword = null;
var selectedAddr = 0;
var totalAddr = 0;
var addrInput;
var addrSuggest;
var addrTimeout = null;

function setCurrentAddr(current) {
    if (totalAddr != 0) {
        $("#addr-sl li").removeClass("current");
        $("#addr-sl li").eq(current).addClass("current");
        selectedAddr = current;
        currentAddrKeyword = $("#addr-sl li").eq(current).html();
        addrInput.val(currentAddrKeyword);
    }
}

function highlightAddr(current) {
    if (totalAddr != 0) {
        $("#addr-sl li").removeClass("highlight");
        $("#addr-sl li").eq(current).addClass("highlight");
    }
}

function getAddrSuggestions() {
    var newKeyword = addrInput.val();
    if (newKeyword != currentAddrKeyword) {
        currentAddrKeyword = newKeyword;
        if (currentAddrKeyword != "") {
            $.post('/search/GetMatchedAddressParts', { query: currentAddrKeyword }, function (data) {
                // replace list by new ones
                var cul = $("#addr-sl");
                $(cul).html("");
                $.each(data, function (i, o) {
                    $(cul).append("<li>" + o.NameIncludeParent + "</li>");
                });
                totalAddr = data.length;
                selectedAddr = -1;
                if (totalAddr > 0)
                    addrSuggest.css("display", "block");
                else
                    addrSuggest.css("display", "none");
            });
        } else {
            $("#cat-sl").html("");
            totalAddr = 0;
            addrSuggest.css("display", "none");
        }
    }
}

function initAddressPartSuggestion() {
    // Init position of search suggestions
    addrInput = $("#addr-q");
    addrSuggest = $("#address-suggestion");
    addrSuggest.offset({ top: addrInput.offset().top + 27, left: addrInput.offset().left });
    addrSuggest.css("min-width", addrInput.outerWidth() - 2);

    // highlight address selection when moving up/down
    addrInput.keydown(function (e) {
        switch (e.keyCode) {
            case 27: // escape
                e.preventDefault();
                addrSuggest.css("display", "none");
                break;
            case 13: // enter
                if (addrSuggest.css("display") != "none" && selectedAddr != -1) {
                    e.preventDefault();
                    addrSuggest.css("display", "none");
                }
                break;
            case 33: // page up
                e.preventDefault();
                setCurrentAddr(0);
                break;
            case 34: // page down
                e.preventDefault();
                setCurrentAddr(totalAddr - 1);
                break;
            case 38: // up
                e.preventDefault();
                setCurrentAddr(selectedAddr <= 0 ? totalAddr - 1 : selectedAddr - 1);
                break;
            case 40: // down
                e.preventDefault();
                setCurrentAddr(selectedAddr >= totalAddr - 1 ? 0 : selectedAddr + 1);
                break;
        }
    });

    // Highlight when mouse enter
    $("#addr-sl li").live("mouseenter", function () {
        highlightAddr($("#addr-sl li").index(this));
    });

    // Populate back to search input when click from suggestions
    $("#addr-sl li").live("mousedown", function () {
        selectedAddr = $("#addr-sl li").index(this);
        currentAddrKeyword = $(this).html();
        addrInput.val(currentAddrKeyword);
    });

    // Disable suggestions when lost focus
    addrInput.blur(function () {
        addrSuggest.css("display", "none");
    });

    addrInput.keyup(function (e) {
        if (catTimeout) {
            clearTimeout(catTimeout);
            catTimeout = null;
        }
        catTimeout = setTimeout(getAddrSuggestions, 100);
    });
}