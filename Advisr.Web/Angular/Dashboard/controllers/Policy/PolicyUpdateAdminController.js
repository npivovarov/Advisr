﻿'use strict';

angular.module('DashboardApp').controller('PolicyUpdateAdminController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'ConfigService', 'PolicyService', function ($scope, $rootScope, $state, $stateParams, $timeout, ConfigService, PolicyService) {

    var id = parseInt($stateParams.id);
    $scope.checkboxModel = [];
    $scope.policy = {};

    $scope.list = {};

    $scope.list.policyPaymentFrequency = ConfigService.policyPaymentFrequency;

    $scope.data = {
        id: id,
        AdditionalProperties: {},
        coverages: []
    };

    $scope.bool = [
        { 'id': true, 'name': 'Yes' },
        { 'id': false, 'name': 'No' }
    ];

    PolicyService.getPolicy(id).then(function (res) {
        var data = res.data;
        $scope.fileNames = res.data.files;
        $scope.policy = data;

        _.extend($scope.data, {
            InsurerId: data.insurerId,
            policyNumber: data.policyNumber,
            policyPremium: data.policyPremium,
            policyPaymentAmount: data.policyPaymentAmount,
            policyExcess: data.policyExcess,
            startDate:  (_.isNull(data.startDate)) ? '' : new Date(data.startDate),
            endDate:  (_.isNull(data.endDate)) ? '' : new Date(data.endDate),
            policyPaymentFrequency: (data.policyPaymentFrequency == 0) ? _.head($scope.list.policyPaymentFrequency) : _.find($scope.list.policyPaymentFrequency, { id: data.policyPaymentFrequency }),
            description: data.description,
        });
        
        if(data.vehiclePolicyModel != null){
            $scope.data.VehiclePolicyModel = data.vehiclePolicyModel;
        }
        
        if(data.lifePolicyModel != null){
            $scope.data.LifePolicyModel = data.lifePolicyModel;
        }

        if(data.homePolicyModel != null){
            $scope.data.HomePolicyModel = data.homePolicyModel;
        }
    }).then(function (data) {
        PolicyService.getInsurerList(0, 100).then(function (res) {
            $scope.insurerList = res.data.data;
            $scope.list.insurerList = (_.isNull($scope.data.InsurerId)) ? 
            _.head($scope.insurerList) : _.find($scope.insurerList, { insurerId: $scope.data.InsurerId });


            PolicyService.getGroups($scope.data.InsurerId == null ? $scope.insurerList[0].insurerId : $scope.data.InsurerId).then(function (res) {
                $scope.groups = res.data;
                if ($scope.policy.policyGroup == null) {
                    $scope.list.groups = _.head($scope.groups);
                    $scope.categories = _.find($scope.groups, { 'groupName': $scope.list.groups.groupName }).categories;
                    $scope.list.categories = _.head($scope.categories);

                    var catId = $scope.list.categories.id;
                    PolicyService.getGroupFields(catId).then(function (res) {
                        $scope.groupFields = res.data;
                    });
                } else {
                    $scope.list.groups = _.find($scope.groups, { 'groupName': $scope.policy.policyGroup.groupName });
                    $scope.categories = _.find($scope.groups, { 'groupName': $scope.policy.policyGroup.groupName }).categories;
                    $scope.list.categories = _.head($scope.categories);

                    var catId = $scope.list.categories.id;
                    PolicyService.getFilledGroupFields(catId, $scope.policy.id).then(function (res) {
                        $scope.groupFields = res.data;
                        if ($scope.groupFields.length > 0) {
                            for (var i = 0; i < $scope.groupFields.length; i++) {
                                if ($scope.groupFields[i].fieldType == 5) {
                                    $scope.groupFields[i].value = (_.isNull($scope.groupFields[i].value)) ? '' : new Date($scope.groupFields[i].value)
                                }
                            }
                            $scope.data.AdditionalProperties = $scope.groupFields;
                        } else {
                            PolicyService.getGroupFields(catId).then(function (res) {
                                $scope.groupFields = res.data;
                            });
                        }
                    });
                }
                
                
            });
        });
    });

    function _onSelection(selectedItem) {

        PolicyService.getGroups(selectedItem.insurerId).then(function (res) {
            $scope.groups = res.data;
            $scope.list.groups = _.head($scope.groups);
            $scope.categories = _.find($scope.groups, { 'groupName': $scope.list.groups.groupName }).categories;
            $scope.list.categories = _.head($scope.categories);

            var catId = $scope.list.categories.id;
            PolicyService.getGroupFields(catId).then(function (res) {
                $scope.groupFields = res.data;
            });
        });
    }

    function _changeGroups(item) {
        $scope.categories = item.categories;
        $scope.list.categories = _.head($scope.categories);
        var catId = $scope.list.categories.id;

        PolicyService.getGroupFields(catId).then(function (res) {
            $scope.groupFields = res.data;
            $scope.data.AdditionalProperties = {};
        });
    }

    function _changeCategories(item) {
        PolicyService.getGroupFields(item.id).then(function (res) {
            $scope.groupFields = res.data;
            $scope.data.AdditionalProperties = {};
        });
    }

    function _save(isConfirmed) {
        // console.log( $scope.groupFields, 'groupFields');
        // console.log($scope.data.AdditionalProperties, 'AdditionalProperties');

        $scope.data.IsConfirmed = isConfirmed;
        $scope.submitInProgressSave = !isConfirmed;
        $scope.submitInProgressConfirmed = isConfirmed;

        var AdditionalProperties = [];
        var data = angular.copy($scope.data);
        
        _.extend(data, {
            InsurerId: $scope.list.insurerList.insurerId,
            PolicyGroupId: $scope.list.categories.id,
            policyPaymentFrequency: $scope.data.policyPaymentFrequency.id
        });

        _.each($scope.data.AdditionalProperties, function (item, key) {
            var id = $scope.groupFields[key].id;
            var value = item.value;
            if (value != null) {
                AdditionalProperties.push({
                    GroupFieldId: id,
                    value: value
                })
            }
        });

        if ('VehiclePolicyModel' in data && 'IsCommercial' in data.VehiclePolicyModel) {
            data.VehiclePolicyModel.IsCommercial = data.VehiclePolicyModel.IsCommercial.id;
        }
       
        if ($scope.list.groups.groupName === 'Personal' && !data.LifePolicyModel) {
            _.extend(data, {
                LifePolicyModel: {}
            });
        }

        if ($scope.list.groups.groupName === 'Vehicle' && !data.VehiclePolicyModel) {
            _.extend(data, {
                VehiclePolicyModel: {}
            });
        }

        if ($scope.list.groups.groupName === 'Personal' && !data.HomePolicyModel) {
            _.extend(data, {
                HomePolicyModel: {}
            });
        }

        data.AdditionalProperties = AdditionalProperties;

        _.each($scope.coveragesDetails, function (item) {
            if (item.isSelected) {
                var coverageIsSelected = {
                    coverageId: item.coverageId,
                    isActive: item.isActive,
                    excess: item.excess,
                    limit: item.limit
                }
                data.coverages.push(coverageIsSelected);
            }
        });

        PolicyService.updatePolicy(data).then(function (res) {
            $scope.submitInProgressSave = false;
            $scope.submitInProgressConfirmed = false;
            $rootScope.alerts.push({type: 'success', msg: 'Policy has been updated.' });

        }, function(res) {
            var data = res.data;
            $scope.submitInProgressSave = false;
            $scope.submitInProgressConfirmed = false;

            if ('Message' in data) {
                $rootScope.alerts.push({ type: 'danger', msg: data.Message });
            }

            _.each(data.Details, function (field) {
                $scope.validationErrors[field.field] = field.errors;
            });
        });
    }

    function _openStartDate() {
        $scope.openedStartDate = true;
    };

    function _openEndDate() {
        $scope.openedEndDate = true;
    };

    function _openBuildDate() {
        $scope.openedBuildDate = true;
    };

    function _getCoverageType(id) {
        return _.find(ConfigService.coverageTypes, { 'id': id }).name;
    }

    function _onChangeCoverage(isSelected, index) {
        if (!isSelected && index > -1) {
            $scope.data.coverages.splice(index, 1);
        }
    }

    $scope.$watch('list.categories', function(item) {
        if (!_.isUndefined(item)) {
            $scope.policyTemplate = item.policyTemplate;

            var dataCoverages = {
                policyId: parseInt($stateParams.id),
                policyTypeId: item.id
            }
            PolicyService.getCoveragesDetails(dataCoverages).then(function(res) {
                console.log(res);
                $scope.coveragesDetails = res.data;
            });
        }
    });

    _.extend($scope, {
        altInputFormats: ['M!/d!/yyyy'],
        openedStartDate: false,
        openedEndDate: false,
        submitInProgressSave: false,
        submitInProgressConfirmed: false,
        changeGroups: _changeGroups,
        changeCategories: _changeCategories,
        onSelection: _onSelection,
        openStartDate: _openStartDate,
        openEndtDate: _openEndDate,
        openBuildDate: _openBuildDate,
        getCoverageType: _getCoverageType,
        onChangeCoverage: _onChangeCoverage,
        groupFields: [],
        validationErrors: {},
        save: _save,
        paramExists: $rootScope.paramExists
    });

}]);
