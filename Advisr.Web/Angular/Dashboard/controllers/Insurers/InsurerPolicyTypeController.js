/// <reference path="../../templates/insurers/addField.html" />
'use strict';

angular.module('DashboardApp').controller('InsurerPolicyTypeController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'ngDialog', 'ConfigService', 'InsurersService', function ($scope, $rootScope, $state, $stateParams, $timeout, ngDialog, ConfigService, InsurersService) {
    
    $scope.data = {
        additionalProperties: []
    };

    $scope.field = {};

    $scope.list = {};

    var groupId = $stateParams.id;



    InsurersService.getGroup(groupId).then(function (res) {
        $scope.data = res.data;
        $scope.list.groupTypes = res.data.groupTypes;

        InsurersService.getInsurer($scope.data.insurerId).then(function (res) {
            $scope.data.insurer = res.data;
        })
    });

    function _save() {
        var type = angular.copy($scope.data);
        type.groupType = $scope.data.groupType.name;
        type.status = $scope.data.status.name;

        InsurersService.editGroup(type).then(function (res) {
            $scope.submitInProgress = false;
            $rootScope.alerts.push({ type: 'success', msg: 'Group has been saved.' });

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

    function _addField() {

        var field = angular.copy($scope.field);
        field.fieldType = $scope.field.fieldType.name;
        field.policyGroupId = $scope.data.groupId;

        ngDialog.close();

        InsurersService.addField(field).then(function (res) {
            $scope.submitInProgress = false;
            $rootScope.alerts.push({ type: 'success', msg: 'Field has been saved.' });

            $state.reload();

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

    function _openAddPopup() {
        ngDialog.open(
            {
                template: 'Angular/Dashboard/templates/insurers/addField.html',
                controller: 'InsurerPolicyTypeController',
                scope: $scope
            });
    }

    function _deleteField(fieldId) {

        InsurersService.deleteField(fieldId).then(function (res) {
            $scope.submitInProgress = false;

            InsurersService.getGroup(groupId).then(function (res) {
                $scope.data = res.data;
                $scope.list.groupTypes = res.data.groupTypes;

                InsurersService.getInsurer($scope.data.insurerId).then(function (res) {
                    $scope.data.insurer = res.data;
                })
            });

            $rootScope.alerts.push({ type: 'success', msg: 'Field has been deleted.' });


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

    _.extend($scope, {
        serviceName: 'InsurersService',
        validationErrors: {},
        save: _save,
        openAddPopup: _openAddPopup,
        addField: _addField,
        deleteField: _deleteField,
        loading: false,
    });

}]);

