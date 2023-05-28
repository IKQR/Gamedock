window.tusUpload = (fileInput, endpoint, metadata, processor) => {
    var file = fileInput.files[0];

    var upload = new tus.Upload(file, {
        endpoint: endpoint,
        retryDelays: [0, 3000, 5000, 10000, 20000],
        metadata: metadata,
        onError: function(error) {
            processor.invokeMethodAsync('OnError', error);
        },
        onProgress: function(bytesUploaded, bytesTotal) {
            processor.invokeMethodAsync('OnProgress', bytesUploaded, bytesTotal);
        },
        onSuccess: function() {
            processor.invokeMethodAsync('OnSuccess');
        },
    });

    upload.findPreviousUploads().then(function (previousUploads) {
        if (previousUploads.length) {
            upload.resumeFromPreviousUpload(previousUploads[0]);
        }

        upload.start();
    });
};
