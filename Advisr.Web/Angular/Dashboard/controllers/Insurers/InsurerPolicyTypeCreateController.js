/// <reference path="../../templates/insurers/addField.html" />
'use strict';

angular.module('DashboardApp').controller('InsurerPolicyTypeCreateController', ['$scope', '$rootScope', '$state', '$stateParams', '$cookies', '$timeout', 'ngDialog', 'ConfigService', 'InsurersService', function ($scope, $rootScope, $state, $stateParams, $cookies, $timeout, ngDialog, ConfigService, InsurersService) {

    $scope.data = {
        additionalProperties: []
    };

    $scope.field = {
        policyGroupId: 0
    };

    $scope.list = {};

    var insurerId = $stateParams.id;


    InsurersService.getInsurer(insurerId).then(function (res) {
        $scope.data.insurer = res.data;
        $scope.data.fieldTypes = res.data.fieldTypes;
    })

    function _add() {
        var type = angular.copy($scope.data);
        type.groupType = $scope.data.groupType.name;
        type.status = $scope.data.status.name;
        type.insurerId = $scope.data.insurer.id;

        InsurersService.addGroup(type).then(function (res) {
            $scope.submitInProgress = false;
            $cookies.put('groupId', res.data.id);
            $rootScope.alerts.push({ type: 'success', msg: 'Group has been saved.' });
            $state.go('editInsurer', { 'id': insurerId });
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
        var groupId = $cookies.get('groupId');
        field.policyGroupId = groupId;
        ngDialog.close();

        InsurersService.addField(field).then(function (res) {
            $scope.submitInProgress = false;
            $rootScope.alerts.push({ type: 'success', msg: 'Field has been saved.' });
            var groupId = $cookies.get('groupId');
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
        var groupId = $cookies.get('groupId');
        if (!groupId || groupId == null) {
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
        save: _add,
        openAddPopup: _openAddPopup,
        addField: _addField,
        deleteField: _deleteField,
        loading: false,
    });

}]);