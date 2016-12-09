/// <reference path="../../templates/insurers/addField.html" />
'use strict';

angular.module('DashboardApp').controller('InsurerPolicyTypeController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'ngDialog', 'ConfigService', 'InsurersService', function ($scope, $rootScope, $state, $stateParams, $timeout, ngDialog, ConfigService, InsurersService) {
    
    $scope.data = {
        additionalProperties: []
    };

    $scope.listSelected = false;
    $scope.field = {};

    $scope.bool = [
        { 'id': true, 'name': 'Yes' },
        { 'id': false, 'name': 'No' }
    ];

    $scope.groupNames = ConfigService.policyGroupName;
    $scope.groupTypes = ConfigService.policyTypeName;
    $scope.statuses = ConfigService.policyTypeStatus;
    $scope.fieldTypes = ConfigService.policyTypeFieldTypes;
    $scope.coverageTypes = ConfigService.coverageTypes;

    $scope.way = '0';

    var policyTypeId = $stateParams.id;

    InsurersService.getPolicyType(policyTypeId).then(function (res) {
        $scope.data = res.data;
        $scope.data.policyGroupName = _.find($scope.groupNames, { 'name': $scope.data.policyGroupName });
        $scope.data.policyGroupType = _.find($scope.groupTypes, { 'id': $scope.data.policyGroupType });
        $scope.data.status = _.find($scope.statuses, { 'id': $scope.data.status });

        for (var i = 0; i < $scope.data.additionalProperties.length; i++) {
            $scope.data.additionalProperties[i].fieldType = _.find($scope.fieldTypes,
                { 'id': $scope.data.additionalProperties[i].fieldType }).name;
        }

        InsurersService.getInsurer($scope.data.insurerId).then(function (res) {
            $scope.data.insurer = res.data;
        })

        InsurersService.getPolicyTypeCoverages(policyTypeId).then(function (res) {
            $scope.data.coverages = res.data;
            
            for (var i = 0; i < $scope.data.coverages.length; i++) {
                $scope.data.coverages[i].type = _.find($scope.coverageTypes, { 'id': $scope.data.coverages[i].type }).name;
            }
        });
    });

    function _save() {
        var policyType = angular.copy($scope.data);
        policyType.policyGroupName = $scope.data.policyGroupName.name;
        policyType.policyGroupType = $scope.data.policyGroupType.id;
        policyType.status = $scope.data.status.id;

        InsurersService.editPolicyType(policyType).then(function (res) {
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
        if (field.fieldType.name == 'Bool') {
            if (field.defaultValue != null) {
                field.defaultValue = field.defaultValue.id;
            }
        }
        field.fieldType = $scope.field.fieldType.id;
        field.policyTypeId = $scope.data.policyTypeId;

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
                template: '../Angular/Dashboard/templates/insurers/addField.html',
                controller: 'InsurerPolicyTypeController',
                scope: $scope
            });
    }

    function _openEditPopup(fieldId) {
        InsurersService.getField(fieldId).then(function (res) {
            $scope.field = res.data;
            $scope.field.fieldType = _.find($scope.fieldTypes,
                { 'id': $scope.field.fieldType });
            $scope.formData = { field: $scope.field };

            if ($scope.field.fieldType.name == 'List') {
                $scope.formData.listSelected = true;
            } else {
                if ($scope.field.fieldType.name == 'Bool') {
                    if ($scope.field.defaultValue != null) {
                        var value = JSON.parse($scope.field.defaultValue.toLowerCase());

                        $scope.field.defaultValue = _.find($scope.bool, { 'id': value });
                    }
                    
                    $scope.formData.boolSelected = true;
                } else {
                    $scope.formData.boolSelected = false;
                }

                $scope.listSelected = false;
            }

            ngDialog.open(
            {
                template: '../Angular/Dashboard/templates/insurers/editField.html',
                controller: 'InsurerPolicyTypeController',
                scope: $scope
            });
        });       
    }

    function _saveField() {
        var field = angular.copy($scope.formData.field);
        field.fieldType = $scope.formData.field.fieldType.name;
        field.policyTypeId = $scope.data.policyTypeId;
        if (field.fieldType == 'Bool') {
            field.defaultValue = field.defaultValue.id;
        }
        
        InsurersService.saveField(field).then(function (res) {
            $scope.submitInProgress = false;
            $rootScope.alerts.push({ type: 'success', msg: 'Field has been saved.' });

            ngDialog.close();

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

    function _deleteField(fieldId) {

        InsurersService.deleteField(fieldId).then(function (res) {
            $scope.submitInProgress = false;

            InsurersService.getPolicyType(policyTypeId).then(function (res) {
                $scope.data = res.data;
                $scope.data.policyGroupName = _.find($scope.groupNames, { 'name': $scope.data.policyGroupName });
                $scope.data.policyGroupType = _.find($scope.groupTypes, { 'id': $scope.data.policyGroupType });
                $scope.data.status = _.find($scope.statuses, { 'id': $scope.data.status });

                for (var i = 0; i < $scope.data.additionalProperties.length; i++) {
                    $scope.data.additionalProperties[i].fieldType = _.find($scope.fieldTypes,
                        { 'id': $scope.data.additionalProperties[i].fieldType }).name;
                }

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

    function _openAddCoverage(id) {

        InsurersService.getInsurersCoverages(id).then(function (res) {
            $scope.insurersCoverages = res.data;
            var values = [];

            for (var i = 0; i < $scope.insurersCoverages.length; i++) {
                $scope.insurersCoverages[i].type = _.find($scope.coverageTypes, { 'id': $scope.insurersCoverages[i].type }).name;

                values.push({ id: $scope.insurersCoverages[i].id, name: $scope.insurersCoverages[i].title + ' (' + $scope.insurersCoverages[i].type + ')' });
            }

            $scope.existingCoverages = values;
        });


        ngDialog.open({
            template: '../Angular/Dashboard/templates/insurers/addNewCoverage.html',
            controller: 'InsurerPolicyTypeController',
            scope: $scope
        });
    }

    function _saveCoverage() {
        var coverage = angular.copy($scope.coverage);
        coverage.type = coverage.type.id
        ngDialog.close();

        InsurersService.saveCoverage(coverage).then(function (res) {
            $scope.submitInProgress = false;
            $rootScope.alerts.push({ type: 'success', msg: 'Coverage has been saved.' });

            $state.reload();
        },
        function (res) {
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

    function _addCoverage() {
        var coverage = angular.copy($scope.newCoverage);
        coverage.type = coverage.type.id;
        coverage.policyTypeId = policyTypeId;
        coverage.insurerId = $scope.data.insurerId;
        ngDialog.close();

        InsurersService.addCoverage(coverage).then(function (res) {
            $scope.submitInProgress = false;
            $rootScope.alerts.push({ type: 'success', msg: 'Coverage has been created.' });

            $state.reload();
        },
        function (res) {
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

    function _assignCoverage() {
        var coverage = angular.copy($scope.existingCoverage);
        coverage.policyTypeId = policyTypeId;
        coverage.coverageId = coverage.id;

        ngDialog.close();

        InsurersService.assignCoverage(coverage).then(function (res) {
            $scope.submitInProgress = false;
            $rootScope.alerts.push({ type: 'success', msg: 'Coverage has been saved.' });

            ngDialog.close();

            $state.reload();
        },
        function (res) {
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

    function _onSelection(item) {
        InsurersService.getCoverage(item.id).then(function (res) {
            $scope.existingCoverage = res.data;
            $scope.existingCoverage.type = _.find($scope.coverageTypes, { 'id': $scope.existingCoverage.type });
        });
    }

    function _onTypeSelected(item) {
        if (item.name == 'List') {
            $scope.listSelected = true;
            $scope.field.listDescription = "[item1, item2]"
        } else {
            if (item.name == 'Bool') {
                $scope.boolSelected = true;
            } else {
                $scope.boolSelected = false;
            }
            $scope.listSelected = false;
        }
    }

    _.extend($scope, {
        serviceName: 'InsurersService',
        validationErrors: {},
        save: _save,
        openAddPopup: _openAddPopup,
        openEditPopup: _openEditPopup,
        addField: _addField,
        saveField: _saveField,
        deleteField: _deleteField,
        openEditCoverage: _openEditCoverage,
        openAddCoverage: _openAddCoverage,
        onSelection: _onSelection,
        saveCoverage: _saveCoverage,
        addCoverage: _addCoverage,
        assignCoverage: _assignCoverage,
        onTypeSelected: _onTypeSelected,
        loading: false,
    });

}]);

