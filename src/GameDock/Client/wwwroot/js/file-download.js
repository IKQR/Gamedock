window.downloadFile = function (url, authToken) {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', url, true);
    xhr.setRequestHeader('Authorization', `Bearer ${authToken}`);
    xhr.responseType = 'blob';

    xhr.onload = function () {
        if (xhr.status === 200) {
            const contentDisposition = xhr.getResponseHeader('Content-Disposition');
            const fileName = contentDisposition
                .split(';')
                .map(item => item.trim())
                .find(item => item.startsWith('filename='))
                .split('=')[1]
                .replace(/"/g, '');

            const blob = new Blob([xhr.response], {type: 'application/octet-stream'});
            const url = window.URL.createObjectURL(blob);

            const downloadLink = document.createElement('a');
            downloadLink.href = url;
            downloadLink.download = fileName;
            downloadLink.click();

            window.URL.revokeObjectURL(url);
        }
    };

    xhr.send();
}