'use strict';

angular.module('DashboardApp').controller('PolicyCreateController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'ngDialog', 'ConfigService', 'PolicyService', 'Upload', function ($scope, $rootScope, $state, $stateParams, $timeout, ngDialog, ConfigService, PolicyService, Upload) {

    $scope.data = {
        fileIds: [],
        prePolicyType: ''
    };

    $scope.listPolicyType = {};
    $scope.listPolicyType.selected = _.head(ConfigService.policyType);

    $scope.policyType = ConfigService.policyType;

    function _creationConfirmed() {
        ngDialog.close();
        $state.reload();
    }

    function _creationRejected() {
        ngDialog.close();
        $state.go('policies');
    }

    function _save() {
        $scope.submitInProgress = true;
        $scope.data.prePolicyType = $scope.listPolicyType.selected.value;

        PolicyService.createPolicy($scope.data).then(function (res) {
            $scope.submitInProgress = false;
            $scope.disabled = true;
            $rootScope.alerts.push({ type: 'success', msg: 'Policy has been created.' });
            
            ngDialog.open({ template: '../Angular/Dashboard/templates/policy/PolicyCreateConfirm.html', controller: 'PolicyCreateController', showClose: false, closeByDocument: false });

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

            $scope.disabled = true;
            $scope.submitInProgressFile = true;

            for (var i = 0; i < dataUrl.length; i++) {
            
            Upload.upload({
                    url: ConfigService.urls.file.upload,
                    data: {
                        file: dataUrl[i]
                    }
                }).then(function(response) {
                $timeout(function() {
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

    function _removeFile(index) {
        $scope.data.fileIds.splice(index, 1);
        $scope.fileNames.splice(index, 1);
    }

    function _showConfirmPopup() {
        
    };


    _.extend($scope, {
        uploadFiles: _uploadFiles,
        removeFile: _removeFile,
        save: _save,
        confirmed: _creationConfirmed,
        rejected: _creationRejected,
        validationErrors: {},
        paramExists: $rootScope.paramExists,
        fileNames: [],
        submitInProgress: false,
        submitInProgressFile: false,
        disabled: false
    });

}]);
