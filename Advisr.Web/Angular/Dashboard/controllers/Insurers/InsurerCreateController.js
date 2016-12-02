'use strict';

angular.module('DashboardApp').controller('InsurerCreateController', ['$scope', '$rootScope', '$interval', '$cookies', '$document', '$stateParams', '$state', '$timeout', 'ConfigService', 'InsurersService', 'Upload', function ($scope, $rootScope, $interval, $cookies, $document, $stateParams, $state, $timeout, ConfigService, InsurersService, Upload) {

    $scope.data = {
        name:""
    };

    function _create() {
        var insurer = angular.copy($scope.data);
        $scope.submitInProgress = true;

        InsurersService.create(insurer).then(function (res) {
            $scope.submitInProgress = false;
            $state.go('insurers');
            $rootScope.alerts.push({ type: 'success', msg: 'Insurer has been created.' });

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
        if (dataUrl) {
            $scope.submitInProgressFile = true;
            Upload.upload({
                url: ConfigService.urls.file.upload,
                data: {
                    file: dataUrl
                }
            }).then(function (response) {
                $timeout(function () {
                    $scope.submitInProgressFile = false;
                    $scope.data.logoId = response.data.id;

                    $scope.data.logo = {
                        id: response.data.id,
                        fileName: response.data.fileName,
                        url: response.data.url
                    };

                    $document.find('#logo').src = $scope.data.logo.url;
                });

            }, function (response) {
                $scope.submitInProgressFile = false;
                if (response.status > 0)
                    $scope.errorMsg = response.status + ': ' + response.data;
            });
        }
    }

    _.extend($scope, {
        openedPopup: false,
        submitInProgress: false,
        create: _create,
        uploadLogo: _uploadFiles,
        validationErrors: {},
        paramExists: $rootScope.paramExists
    });

}]);