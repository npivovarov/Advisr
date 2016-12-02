'use strict';

angular.module('DashboardApp').controller('PolicyViewUploadController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'ConfigService', 'PolicyService', 'Upload', function ($scope, $rootScope, $state, $stateParams, $timeout, ConfigService, PolicyService, Upload) {

    var id = $stateParams.id;
    $scope.data = {
        id: id,
        fileIds: []
    };

    PolicyService.getPolicy(id).then(function(res) {
         $scope.fileNames =  res.data.files;
    });


    function _save() {
        $scope.submitInProgress = true;
        
        PolicyService.addNewFile($scope.data).then(function (res) {
            $scope.submitInProgress = false;
            $rootScope.alerts.push({ type: 'success', msg: 'Policy has been updated.' });

        }, function (res) {
            var data = res.data;

            $scope.submitInProgress = false;

            if ('Message' in data) {
                $rootScope.alerts.push({ type: 'danger', msg: data.Message });
            }

            _.each(data.Details, function (field) {
                $scope.validationErrors[field.field] = field.errors;
            });
        });
    }

    function _uploadFiles(dataUrl) {
        if (dataUrl.length > 0) {
            $scope.submitInProgressFile = true;
            $scope.disabled = true;

            for (var i = 0; i < dataUrl.length; i++) {
                Upload.upload({
                    url: ConfigService.urls.file.upload,
                    data: {
                        file: dataUrl[i]
                    }
                }).then(function (response) {
                    $timeout(function () {
                        $scope.submitInProgressFile = false;
                        $scope.disabled = false;

                        $scope.data.fileIds.push(response.data.id);

                        $scope.fileNames.push({
                            id: response.data.id,
                            fileName: response.data.fileName,
                            url: response.data.url,
                            fileSize: response.data.fileSize,
                            uploadedDate: new Date()
                        });
                    });

                }, function (response) {
                    $scope.submitInProgressFile = false;
                    if (response.status > 0)
                        $scope.errorMsg = response.status + ': ' + response.data;
                });
            } 
        }
    }


    _.extend($scope, {
        uploadFiles: _uploadFiles,
        save: _save,
        validationErrors: {},
        paramExists: $rootScope.paramExists,
        fileNames: [],
        submitInProgress: false,
        submitInProgressFile: false,
        disabled: false
    });

}]);
