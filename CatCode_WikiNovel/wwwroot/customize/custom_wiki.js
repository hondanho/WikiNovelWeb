var btn = $('#button-back-to-top');
var loadingShowing = true;
var parentNoiDungTruyen = $('#item-noidung-chuong');
var select_Chuong, select_TheLoaiTruyen, select_TrangThaiTruyen, btnSearch = null;

function setupDanhMucTruyen(obj) {
    btnSearch = $('#btnSearch');
    select_TheLoaiTruyen = $('#cbb_TheLoaiTruyen').select2({
        placeholder: "Thể loại",
        allowClear: true,
        closeOnSelect: false,
        value: obj.theLoai
    });
    select_TrangThaiTruyen = $('#cbb_TrangThaiTruyen').select2({
        placeholder: "Trạng thái",
        value: obj.trangThai
    });
    btn_TimTruyen_Click(obj.tuKhoa);

    select_TheLoaiTruyen.on('change', function (e) {
        updateURLParameter(window.location.href, 'theLoai', select_TheLoaiTruyen.val());
        updateURLParameter(window.location.href, 'trang', '');

    });
    select_TrangThaiTruyen.on('change', function (e) {
        updateURLParameter(window.location.href, 'trangThai', select_TrangThaiTruyen.val());
        updateURLParameter(window.location.href, 'trang', '');

    });
}


$(window).scroll(function () {
    if ($(window).scrollTop() > 300) {
        btn.addClass('show');
    } else {
        btn.removeClass('show');
    }
});
window.addEventListener("keydown", onKeyDown_Handle, true);

function onKeyDown_Handle(e) {
    if (parentNoiDungTruyen == undefined || loadingShowing) return;
    var offsetTop = parentNoiDungTruyen.scrollTop();
    var jumbScroll = 100;
    switch (e.keyCode) {
        case 40://down
        case 32://space
            parentNoiDungTruyen.stop().animate({ scrollTop: offsetTop + jumbScroll }, 200, 'swing', function () { });
            break;
        case 38://up
            parentNoiDungTruyen.stop().animate({ scrollTop: offsetTop - jumbScroll }, 200, 'swing', function () { });
            break;
        case 37://left
            btnChuongPhiaTruocClick();
            break;
        case 39://right
            btnChuongPhiaSauClick();
            break;
    }
}

btn.on('click', function (e) {
    e.preventDefault();
    $('html, body').animate({ scrollTop: 0 }, '300');
});

 
function btnSearch_Click(ele_txt) {
    var key = ele_txt != undefined ? $('#' + ele_txt).val() : $('#txt-search-box').val();
    if (key == undefined || key == '') {
        return;
    }
    showLoading();
    window.location = '/Truyen/TimKiem?truyen=' + key;
}

function searchKeyPress(e) {
    // look for window.event in case event isn't passed in
    e = e || window.event;
    if (e.keyCode == 13) {
        btnSearch_Click();
        return false;
    }
    return true;
}

function searchTruyenKeyPress(e) {
    // look for window.event in case event isn't passed in
    e = e || window.event;
    if (e.keyCode == 13) {
        btn_TimTruyen_Click();
        return false;
    }
    return true;
}

function stringToSlug(str) {
    // remove accents
    var from = "àáãảạăằắẳẵặâầấẩẫậèéẻẽẹêềếểễệđùúủũụưừứửữựòóỏõọôồốổỗộơờớởỡợìíỉĩịäëïîöüûñçýỳỹỵỷ",
        to = "aaaaaaaaaaaaaaaaaeeeeeeeeeeeduuuuuuuuuuuoooooooooooooooooiiiiiaeiiouuncyyyyy";
    for (var i = 0, l = from.length; i < l; i++) {
        str = str.replace(RegExp(from[i], "gi"), to[i]);
    }

    str = str.toLowerCase()
        .trim()
        .replace(/[^a-z0-9\-]/g, '-')
        .replace(/-+/g, '-');

    return str;
}

function btn_TimTruyen_Click(init_key) {
    var key = '';
    if (init_key != undefined && init_key != '') {
        key = init_key;
    } else {
        key = $('#txt-truyen-search-box').val();
    }

    var theLoai = '';
    $.each(select_TheLoaiTruyen.val(), function (key, value) {
        theLoai += value + ',';
    });
    if (theLoai != '') {
        theLoai = theLoai.substring(0, theLoai.length - 1);
    }
    var trangThai = select_TrangThaiTruyen.val();
    showLoading();
    updateURLParameter(window.location.href, 'theLoai', theLoai);
    updateURLParameter(window.location.href, 'trangThai', trangThai);
    updateURLParameter(window.location.href, 'truyen', key);
    var trang = getcurrentPageFromURL();
    updateURLParameter(window.location.href, 'trang', trang);
    document.title = 'Tìm kiếm truyện';
    $.ajax({
        url: '/Truyen/Finds',
        type: "POST",
        data: {
            truyen: key,
            theLoai: theLoai,
            trang: trang,
            trangThai: trangThai,
        },
        dataType: 'json',
        success: function (response) {
            hideLoading();
            loadPager({ total: response.total, visible: (key != '' || theLoai != '' || trangThai != 'tat-ca') });
            if (response.new_url != undefined) {
                window.history.pushState({}, '', new_url);
            }
            var parent = $('#lst-ket-qua-tim-kiem');
            parent.empty();
            if (response == null || response.data == undefined || response.data.length == 0) {
                var rowHtmlEmpty = '<div class="col-12 mb-4"><h3><center>Không có kết quả phù hợp với bộ lọc của bạn</center></h3></div>';
                parent.append(rowHtmlEmpty);
            }
            else {
                for (var i = 0; i < response.data.length; i++) {
                    var rowData = response.data[i];
                    var rowHtml = getHtmlItemCard(rowData);
                    parent.append(rowHtml);
                }
            } 
        }, error: function (err) {
            alert('Exception: ' + err);
            hideLoading();

        }
    });
}

function updateURLParameter(url, param, paramVal) {
    var TheAnchor = null;
    var newAdditionalURL = "";
    var tempArray = url.split("?");
    var baseURL = tempArray[0];
    var additionalURL = tempArray[1];
    var temp = "";

    if (additionalURL) {
        var tmpAnchor = additionalURL.split("#");
        var TheParams = tmpAnchor[0];
        TheAnchor = tmpAnchor[1];
        if (TheAnchor)
            additionalURL = TheParams;

        tempArray = additionalURL.split("&");

        for (var i = 0; i < tempArray.length; i++) {
            if (tempArray[i].split('=')[0] != param) {
                newAdditionalURL += temp + tempArray[i];
                temp = "&";
            }
        }
    }
    else {
        var tmpAnchor = baseURL.split("#");
        var TheParams = tmpAnchor[0];
        TheAnchor = tmpAnchor[1];

        if (TheParams)
            baseURL = TheParams;
    }

    if (TheAnchor)
        paramVal += "#" + TheAnchor;

    var rows_txt = paramVal != undefined && paramVal != '' && paramVal != 'tat-ca' ? temp + "" + param + "=" + paramVal : "";
    var newURL = baseURL + "?" + newAdditionalURL + rows_txt;
    window.history.pushState({}, '', newURL);
    return;
}


function loadPager(config) {
    var parent = $('#pagination-ket-qua-tim-kiem');
    parent.empty();
    if (!config.visible || config.total == 0) return;
    $.ajax({
        url: '/Truyen/Pagers',
        type: "POST",
        data: {
            total: config.total,
            current: getcurrentPageFromURL(),
            uriPage: window.location.toString(),
        },
        dataType: 'html',
        success: function (html) {
            parent.html(html);
        }, error: function (err) {
            alert('Exception: ' + err);
        }
    });
}
function getcurrentPageFromURL() {
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    var page = urlParams.get('trang');
    if (isNaN(page) || page == undefined || page < 1) return 1;
    return page;
}

var data_chuong = [];
var index_chuong = 0;
var qTruyen = '';
var cChuong = '';

function loadThongTinChuong(chuong, uriChuong) {
    if (uriChuong != undefined && uriChuong != '') {
        select_Chuong.val(uriChuong).trigger("change");
        return;
    }
    if (chuong != undefined) {
        index_chuong = chuong;
    }
    uriChuong = data_chuong[index_chuong].uriChuong;
    showLoading();
    parentNoiDungTruyen.stop().animate({ scrollTop: 0 }, 1000, 'swing', function () { });
    $.ajax({
        url: '/Truyen/Contents',
        type: "POST",
        data: {
            truyen: qTruyen
            , chuong: uriChuong
        },
        dataType: 'json',
        success: function (response) {
            hideLoading();
            if (response.type == 'images') {
                parentNoiDungTruyen.empty();
                var pic = 0;
                response.noiDungTruyen.forEach(function (src) {
                    // do something with `item`
                    parentNoiDungTruyen.append('<div><img src="' + src + '" alt="pic-' + (pic++) + '" loading="lazy" /></div>');
                });
            }
            else {
                parentNoiDungTruyen.html('<h4>' + data_chuong[index_chuong].title + '</h4><br/>');
                parentNoiDungTruyen.append(response.noiDungTruyen);
            }

            updateURLParameter(window.location.href, 'chuong', uriChuong);
            if (index_chuong == 0) {
                $('#item-button-chuongPhiaTruoc').addClass('disabled');
            } else {
                $('#item-button-chuongPhiaTruoc').removeClass('disabled');
            }

            if (index_chuong == data_chuong.length - 1) {
                $('#item-button-chuongTiepTheo').addClass('disabled');
            } else {
                $('#item-button-chuongTiepTheo').removeClass('disabled');
            }
        }, error: function (err) {
            alert('Exception: ' + err.statusText);
            hideLoading();
        }
    });
}

function selectChuong_OnChange(obj) {
    var uriChuong = obj.val();
    if (uriChuong == undefined || uriChuong == '') return;
    for (var i = 0; i < data_chuong.length; i++) {
        if (uriChuong == data_chuong[i].uriChuong) {
            loadThongTinChuong(i);
            return;
        }
    }
}

function btnChuongPhiaTruocClick() {
    index_chuong = index_chuong - 1;
    select_Chuong.val(data_chuong[index_chuong].uriChuong).trigger("change");
}

function btnChuongPhiaSauClick() {
    index_chuong = index_chuong + 1;
    select_Chuong.val(data_chuong[index_chuong].uriChuong).trigger("change");
}

function hienThiGioiThieuDayDu() {
    var gioiThieuDayDu = $('#gioiThieuDayDu');
    gioiThieuDayDu.addClass('mb-2');
    gioiThieuDayDu.removeClass('max-line-4');
    $('#btn-gioiThieuDayDu').empty();
}


function showLoading() {
    if (loadingShowing) return;
    loadingShowing = true;
    jQuery("#load").fadeIn();
    jQuery("#loading").delay().fadeIn("");
}
function hideLoading() {
    if (!loadingShowing) return;
    loadingShowing = false;
    jQuery("#load").fadeOut();
    jQuery("#loading").delay().fadeOut("");
}
/*---------------------------------------------------------------------
Page Loader
-----------------------------------------------------------------------*/
hideLoading();
function getTitleURL(uri) {
    var base_url = 'https://truyenfree.net';
    //var base_url = 'https://localhost:44308';
    return base_url + '?tenTruyen=' + uri;
}
function getHtmlItemCard(data) {
    var htmlUpload = ''
    if (data.isAdmin) {
        htmlUpload = '<form name="Upload-' + data.title_url + '" action="/Admin/UploadImage" enctype="multipart/form-data" method="post">'
            + '<input type="file" name="file"/>'
            + '<input type="text" name="tenTruyen" hidden value="' + data.title_url + '" />'
            + '<input type="submit" value="Save"/>';
    }
    if (data.urlHinhAnh_image_path == null || data.urlHinhAnh_image_path == '') {
        data.urlHinhAnh_image_path = 'https://truyenfree.net/img2/tu-thu-tay-ha.png';
    }
    var html = '<div class="col-6 col-sm-3 col-md-3 col-lg-2 col-xl-2">'
        + htmlUpload
        +'</form> '
        + ' <div class="iq-card iq-card-block iq-card-stretch iq-card-height browse-bookcontent">'
        + '       <div class="iq-card-body p-0">'
        + '           <div class="row">'
        + '               <div class="col-12 position-relative">'
        + '                   <div class="mb-3 card">'
        + '                       <div class="imgBox">'
        + '                           <a href="' + getTitleURL(data.title_url) + '" itemprop="url"><img class="img" loading="lazy" src="' + data.urlHinhAnh_image_path + '" alt="' + data.title + '" itemprop="image" title="' + data.title + '"></a>'
        + '                           <div class="details-button">'
        + '                               <div class="view-book"><a href="' + getTitleURL(data.title_url) + '" itemprop="url" class="btn btn-sm btn-white">Đọc ngay</a></div>'
        + '                           </div>';

    if (data.trangThaiTruyen != '' && data.trangThaiTruyen != undefined) {
        html += ' <div class="imgBox-trangThai"><span>' + data.trangThaiTruyen + '</span></div>';
    }

    if (data.idLoaiTruyen == 2) {
        html += ' <div class="imgBox-truyenTranh"><span>Truyện tranh</span></div>';
    }

    html += '           </div>'
        + '                       <div class="details">'
        + '                               <p class="font-size-13 line-height mb-1 p-author text-center"><strong> ' + data.soLuongChuong + ' chương</strong></p>'
        + '                           <a href="' + getTitleURL(data.title_url) + '" itemprop="url">'
        + '                           <p class="font-size-13 line-height mb-1 p-description"><strong></strong><span itemprop="description">' + data.gioiThieu + '</span></p>'
        + '                           </a>'
        + '                       </div>'
        + '                   </div>'
        + '                   <div class="mb-2 a-name">'
        + '                       <a href="' + getTitleURL(data.title_url) + '"><h6 class="mb-0 item-title-truyen-list" itemprop="name" title="' + data.title + '">' + data.title + '</h6></a>';

    if (data.soLuotDanhGia > 0) {
        html += '    <div class="d-block line-height text-center" title="' + data.diemDanhGia + ' + "/10 từ "' + data.soLuotDanhGia + ' + " lượt"><span class="font-size-11 text-warning">';
        for (var i = 1; i <= 10; i++) {
            if (i > data.diemDanhGia) {
                break;
            }

            if (data.diemDanhGia - i >= 0.5 && data.diemDanhGia - i < 1) {
                html += ' <i class="fa fa-star-half"></i>';
            }
            else {
                html += ' <i class="fa fa-star"></i>';
            }
        }

        html += '   </span></div>'
            + ' <div class="small text-center" itemprop="aggregateRating" itemtype="https://schema.org/AggregateRating"><em><strong><span itemprop="ratingValue">' + data.diemDanhGia + '</span></strong>/<span class="text-muted" itemprop="bestRating">10</span><meta itemprop="worstRating" content="1"> từ <strong><span itemprop="ratingCount">' + data.soLuotDanhGia + '</span> lượt</strong></em></div>';

    }
    html += '                   </div>'
        + '               </div>'
        + '           </div>'
        + '       </div>'
        + '   </div>'
        + '</div>';
    return html;
}
 
function loadTableChapter(id, cp2, text, uriChapter, textChapter) {
    $.ajax({
        url: "/Truyen/ListChapter/",
        data: { id: id.toString() },
        method: "POST",
        dataType: 'json',
    }).done(function (data) {
        for (var i = 0; i < data.length; i++) {
            var dr = data[i];
            var title = dr.title;
            var chuong = dr.chuong;
            if (!title.includes(chuong) && !title.startsWith('Chương')) {
                data[i].title = 'Chương ' + chuong + ': ' + title
            } else if (title.includes(chuong) && !title.startsWith('Chương')) {
                data[i].title = 'Chương ' + title
            }
        }
        data_chuong = data;
        if (select_Chuong.length == 0) return;
        for (var i = 0; i < data_chuong.length; i++) {
            var col1 = data_chuong[i];
            select_Chuong.append(new Option(col1.title, col1.uriChuong, true, true));
        }

        if (cp2 != undefined && cp2 != '') {
            $('#modal-chuong').modal('show');
            select_Chuong.val(cp2).trigger('change');
        }
        else if (uriChapter != undefined && uriChapter != '') {
            swal.fire({
                html: "Bạn đang đọc dỡ tại <strong>" + textChapter + '</strong><br/>Bạn có muốn tiếp tục?',
                title: '<br/>' + text,
                showCancelButton: true,
                //imageUrl: imageUrl,
                cancelButtonText: "Bỏ qua",
                confirmButtonText: "Đọc tiếp",
            }).then((result) => {
                if (result.value == true) {
                    $('#modal-chuong').modal('show');
                    select_Chuong.val(uriChapter).trigger('change');
                } else {
                    setupFirstShowModal(uriChapter);
                }
            });
        }
        else {
            setupFirstShowModal(data_chuong[0].uriChuong);
        }
        var dataRender = [];
        for (var i = 0; i < data_chuong.length; i += 2) {
            var col1 = data_chuong[i];
            var col2 = data_chuong[i + 1];
            dataRender.push({ l: col1, r: col2 });
        }

        $('#table-chapter').dataTable({
            aaData: dataRender,
            pagingType: 'full_numbers',
            ordering: false,
            info: false,
            lengthChange: false,
            columns: [
                {
                    "targets": 0,
                    "render": function (data, type, row, meta) {
                        return '<a itemprop="url" href="https://truyenfree.net?tenTruyen=' + qTruyen + '&chuong=' + row.l.uriChuong + '"><i class="ri-book-mark-fill"></i> ' + row.l.title + '</a>';
                    },
                },
                {
                    "targets": 1,
                    "render": function (data, type, row, meta) {
                        if (row == undefined || row.r == undefined) {
                            return '';
                        }
                        return '<a itemprop="url" href="https://truyenfree.net?tenTruyen=' + qTruyen + '&chuong=' + row.r.uriChuong + '"><i class="ri-book-mark-fill"></i> ' + row.r.title + '</a>';
                    },
                }
            ],
        });
    });

}


function setupFirstShowModal(uriChuong) {
    var shown = false;
    $('#modal-chuong').on('shown.bs.modal', function (e) {
        if (shown) return;
        shown = true;
        select_Chuong.val(uriChuong).trigger('change');
    });
 
}

$('#modal-chuong').on('shown.bs.modal', function (e) {
    $('#expand-details-truyen').css('display','none');
    $('#container-details-truyen').css('display','none');
});
$('#modal-chuong').on('hidden.bs.modal', function (e) {
    $('#expand-details-truyen').css('display','');
    $('#container-details-truyen').css('display','');
});