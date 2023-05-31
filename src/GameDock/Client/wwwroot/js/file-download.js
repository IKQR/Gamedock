window.downloadFile = function (fileUrl) {
    var link = document.createElement("a");
    link.href = fileUrl;
    link.download = fileUrl.substr(fileUrl.lastIndexOf("/") + 1);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};