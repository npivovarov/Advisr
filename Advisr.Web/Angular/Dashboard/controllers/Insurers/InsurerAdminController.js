'use strict';

angular.module('DashboardApp').controller('InsurerAdminController', ['$scope', '$rootScope', '$interval', '$cookies', '$document', '$stateParams', '$state', '$timeout', 'ngDialog', 'ConfigService', 'InsurersService', 'Upload', function ($scope, $rootScope, $interval, $cookies, $document, $stateParams, $state, $timeout, ngDialog, ConfigService, InsurersService, Upload) {

    $scope.data = {
        policyTypes: []
    };

    $scope.groupNames = ConfigService.policyGroupName;
    $scope.groupTypes = ConfigService.policyTypeName;
    $scope.statuses = ConfigService.policyTypeStatus;
    $scope.fieldTypes = ConfigService.policyTypeFieldTypes;
    $scope.coverageTypes = ConfigService.coverageTypes;
    $scope.colors = ConfigService.colors;

    var id = $stateParams.id;

    InsurersService.getInsurer(id).then(function (res) {
        $scope.data = res.data;

        $scope.data.logo = {
            id: res.data.logo.id,
            fileName: res.data.logo.fileName,
            url: res.data.logo.url
        };

        $scope.data.logoId = $scope.data.logo.id;

        $document.find('#logo').src = $scope.data.logo.url;

        InsurersService.getPolicyTypes(res.data.id).then(function (res) {
            $scope.data.policyTypes = res.data;

            for (var i = 0; i < $scope.data.policyTypes.length; i++) {
                $scope.data.policyTypes[i].policyGroupName = _.find($scope.groupNames,
                    { 'name': $scope.data.policyTypes[i].policyGroupName }).name;
                $scope.data.policyTypes[i].policyGroupType = _.find($scope.groupTypes,
                    { 'id': $scope.data.policyTypes[i].policyGroupType }).name;
                $scope.data.policyTypes[i].status = _.find($scope.statuses,
                    { 'id': $scope.data.policyTypes[i].status }).name;
            }
        });

        InsurersService.getInsurersCoverages(id).then(function (res) {
            $scope.data.coverages = res.data;

            for (var i = 0; i < $scope.data.coverages.length; i++) {
                $scope.data.coverages[i].type = _.find($scope.coverageTypes, { 'id': $scope.data.coverages[i].type }).name;
            }
        });
    });

    function _save() {
        var insurer = angular.copy($scope.data);
        
        $scope.submitInProgress = true;

        var phone = -1;
        var overseas = -1;

        if (insurer.phone != null) {
            phone = insurer.phone.search(/[a-zA-Z,./]/i);
            if (phone != -1) {
                $scope.validationErrors['phone'] = {
                    error: 'Phone contains some invalid character.'
                };
            }

            if (phone != -1 || overseas != -1) {
                $scope.submitInProgress = false;
                return;
            }
        }

        if (insurer.phoneOverseas != null) {
            overseas = insurer.phoneOverseas.search(/[a-zA-Z,./]/i);

            if (overseas != -1) {
                $scope.validationErrors['phoneOverseas'] = {
                    error: 'Phone contains some invalid character.'
                };
            }

            if (phone != -1 || overseas != -1) {
                $scope.submitInProgress = false;
                return;
            }
        }

        InsurersService.save(insurer).then(function (res) {
            $scope.submitInProgress = false;
            $state.go('insurers');
            $rootScope.alerts.push({ type: 'success', msg: 'Insurer has been saved.' });

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

    function _openEditCoverage(id) {
        InsurersService.getCoverage(id).then(function (res) {
            $scope.coverage = res.data;
            $scope.coverage.type = _.find($scope.coverageTypes, { 'id': $scope.coverage.type });
        });

        ngDialog.open({
            template: '../Angular/Dashboard/templates/insurers/editCoverage.html',
            controller: 'InsurerPolicyTypeController',
            scope: $scope
        });
    }

    

    _.extend($scope, {
        openedPopup: false,
        submitInProgress: false,
        save: _save,
        uploadLogo: _uploadFiles,
        openEditCoverage: _openEditCoverage,
        logoFile: [],
        validationErrors: {},
        paramExists: $rootScope.paramExists
    });

}]);