function copyLink(url) {
    if (url ==null) url = window.location.href;
    navigator.clipboard.writeText(url);
    var tooltip = document.getElementById('myTooltip-link-truyen');
    tooltip.innerHTML = 'Đã sao chép';
}

function shareLinkFaceBook(url) {
    if (url == null) url = window.location.href;
    var width = window.screen.width;
    var height = window.screen.height;
    if (!detectMob()) {
        width = 900;
        height = 745;
    }
    var t = '@title';
    var share_url = 'https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(url) + '&t=' + encodeURIComponent(t);
    popupCenter({ url: share_url, title: t, w: width, h: height });
    return false;
} 


function outFunc() {
    var tooltip = document.getElementById('myTooltip-link-truyen');
    tooltip.innerHTML = "Sao chép link";
}

function detectMob() {
    const toMatch = [
        /Android/i,
        /webOS/i,
        /iPhone/i,
        /iPad/i,
        /iPod/i,
        /BlackBerry/i,
        /Windows Phone/i
    ];

    return toMatch.some((toMatchItem) => {
        return navigator.userAgent.match(toMatchItem);
    });
}

function on_facebook_sharer(url) {
    url = 'https://truyenfree.net/';
    var width = window.screen.width;
    var height = window.screen.height;
    if (!detectMob()) {
        width = 900;
        height = 745;
    }
    var t = 'demo them moi va share link';
    var share_url = 'https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(url) + '&t=' + encodeURIComponent(t);
    popupCenter({ url: share_url, title: t, w: width, h: height });
    return false;
}

const popupCenter = ({ url, title, w, h }) => {
    // Fixes dual-screen position                             Most browsers      Firefox
    const dualScreenLeft = window.screenLeft !== undefined ? window.screenLeft : window.screenX;
    const dualScreenTop = window.screenTop !== undefined ? window.screenTop : window.screenY;

    const width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    const height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    const systemZoom = width / window.screen.availWidth;
    const left = (width - w) / 2 / systemZoom + dualScreenLeft
    const top = (height - h) / 2 / systemZoom + dualScreenTop
    const newWindow = window.open(url, title,
        `
      scrollbars=yes,
      width=${w / systemZoom}, 
      height=${h / systemZoom}, 
      top=${top}, 
      left=${left}
      `
    )

    if (window.focus) newWindow.focus();
}