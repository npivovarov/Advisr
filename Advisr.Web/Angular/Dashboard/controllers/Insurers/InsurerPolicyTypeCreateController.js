/// <reference path="../../templates/insurers/addField.html" />
'use strict';

angular.module('DashboardApp').controller('InsurerPolicyTypeCreateController', ['$scope', '$rootScope', '$state', '$stateParams', '$cookies', '$timeout', 'ngDialog', 'ConfigService', 'InsurersService', function ($scope, $rootScope, $state, $stateParams, $cookies, $timeout, ngDialog, ConfigService, InsurersService) {

    $scope.data = {
        additionalProperties: []
    };

    $scope.field = {
        policyGroupId: 0
    };

    $scope.groupNames = ConfigService.policyGroupName;
    $scope.groupTypes = ConfigService.policyTypeName;
    $scope.statuses = ConfigService.policyTypeStatus;
    $scope.fieldTypes = ConfigService.policyTypeFieldTypes;

    var insurerId = $stateParams.id;


    InsurersService.getInsurer(insurerId).then(function (res) {
        $scope.data.insurer = res.data;
    })

    function _add() {
        var policyType = angular.copy($scope.data);
        policyType.policyGroupName = $scope.data.policyGroupName.name;
        policyType.policyGroupType = $scope.data.policyGroupType.id;
        policyType.status = $scope.data.status.id;
        policyType.insurerId = $scope.data.insurer.id;

        InsurersService.addPolicyType(policyType).then(function (res) {
            $scope.submitInProgress = false;
            $cookies.put('policyTypeId', res.data.id);
            $rootScope.alerts.push({ type: 'success', msg: 'Group has been saved.' });
            $state.go('editInsurerPolicyType', { 'id': res.data.id });
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
        field.fieldType = $scope.field.fieldType.id;
        var policyTypeId = $cookies.get('policyTypeId');
        field.policyTypeId = policyTypeId;
        ngDialog.close();

        InsurersService.addField(field).then(function (res) {
            $scope.submitInProgress = false;
            $rootScope.alerts.push({ type: 'success', msg: 'Field has been saved.' });
            var policyTypeId = $cookies.get('policyTypeId');
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
        var policyTypeId = $cookies.get('policyTypeId');
        if (!policyTypeId || policyTypeId == null) {
            $rootScope.alerts.push({ type: 'danger', msg: 'You need to save your group first' });
            return;
        }

        ngDialog.open(
            {
                template: 'Angular/Dashboard/templates/insurers/addField.html',
                controller: 'InsurerPolicyTypeCreateController',
                scope: $scope
            });
    }

    function _deleteField(fieldId) {

        InsurersService.deleteField(fieldId).then(function (res) {
            $scope.submitInProgress = false;

            InsurersService.getPolicyType(policyTypeId).then(function (res) {
                $scope.data = res.data;
                $scope.list.policyTypes = res.data.policyTypes;

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
        save: _add,
        openAddPopup: _openAddPopup,
        addField: _addField,
        deleteField: _deleteField,
        loading: false,
    });

}]);