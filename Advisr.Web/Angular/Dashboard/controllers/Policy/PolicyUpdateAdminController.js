'use strict';

angular.module('DashboardApp').controller('PolicyUpdateAdminController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'ConfigService', 'PolicyService', function ($scope, $rootScope, $state, $stateParams, $timeout, ConfigService, PolicyService) {

    var id = parseInt($stateParams.id);

    $scope.checkboxModel = [];
    $scope.policy = {};
    $scope.list = {};
    $scope.list.policyPaymentFrequency = ConfigService.policyPaymentFrequency;

    $scope.data = {
        id: id,
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
        for (var i = 0; i < res.data.properties.length; i++) {
            if (res.data.properties[i].fieldType == 2) {
                res.data.properties[i].value = _.find($scope.bool, { name: res.data.properties[i].value });
            }
        }
        $scope.properties = res.data.properties;

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
    
    }).then(function (data) {
        PolicyService.getInsurerList(0, 100).then(function (res) {

            $scope.insurerList = res.data.data;
            $scope.list.insurerList = (_.isNull($scope.data.InsurerId)) ? 
            _.head($scope.insurerList) : _.find($scope.insurerList, { insurerId: $scope.data.InsurerId });


            PolicyService.getGroups($scope.data.InsurerId == null ? $scope.insurerList[0].insurerId : $scope.data.InsurerId).then(function (res) {
                $scope.groups = res.data;
                if ($scope.policy.policyGroup == null) {
                    $scope.list.groups = _.head($scope.groups);
                    $scope.categories = _.find($scope.groups, { 'groupName': $scope.list.groups.groupName }).policyTypes;
                    $scope.list.categories = _.head($scope.categories);
                } else {
                    var policyTypeId = $scope.policy.policyTypeId;
                    $scope.list.groups = _.find($scope.groups, { 'groupName': $scope.policy.policyGroup.groupName });
                    $scope.categories = _.find($scope.groups, { 'groupName': $scope.policy.policyGroup.groupName }).policyTypes;
                    $scope.list.categories = _.find($scope.categories, { id: policyTypeId});
                }
            });
        });
    });

    function _onSelection(selectedItem) {
        PolicyService.getGroups(selectedItem.insurerId).then(function (res) {
            $scope.groups = res.data;
            $scope.list.groups = _.head($scope.groups);
            $scope.categories = _.find($scope.groups, { 'groupName': $scope.list.groups.groupName }).policyTypes;
            $scope.list.categories = _.head($scope.categories);
        });
    }

    function _changeGroups(item) {
        $scope.categories = _.find($scope.groups, { 'groupName': $scope.list.groups.groupName }).policyTypes;
        $scope.list.categories = _.head($scope.categories);
    }

    function _getFields(policyTypeId, propertyType) {
        var data = {
            policyTypeId: policyTypeId,
            policyId: id,
            propertyType: propertyType
        };
        
        PolicyService.getGroupFields(data).then(function (res) {
            var dateField = _.filter(res.data, { fieldType: 5 });
            if (!_.isUndefined(dateField)) {
                _.each(dateField, function(item) {
                    var date = new Date(item.value);
                    item.value = date;
                });
            }

            if (data.propertyType === 0) {
                $scope.policyProperties = res.data;
                $scope.data.policyProperties = $scope.policyProperties;
            }
            if (data.propertyType === 1) {
                $scope.groupFields = res.data;
                $scope.data.AdditionalProperties = $scope.groupFields;
            }
        });
    }

    function _save(isConfirmed) {
        $scope.data.IsConfirmed = isConfirmed;
        $scope.submitInProgressSave = !isConfirmed;
        $scope.submitInProgressConfirmed = isConfirmed;

        var AdditionalProperties = [],
            PolicyProperties = [],
            data = angular.copy($scope.data);
        
        _.extend(data, {
            InsurerId: $scope.list.insurerList.insurerId,
            policyTypeId: $scope.list.categories.id,
            policyPaymentFrequency: $scope.data.policyPaymentFrequency.id
        });

        // Coverage Additional Properties model
        _.each($scope.data.AdditionalProperties, function (item, key) {
            var id = $scope.groupFields[key].id,
                value = item.value;
            if (value != null || item.isRequired) {
                AdditionalProperties.push({
                    propertyId: id,
                    value: value
                });
            }
        });

        // Coverage Policy Properties model
        _.each($scope.data.policyProperties, function (item, key) {
            var id = $scope.policyProperties[key].id,
                value = item.value;
            if (value == "") {
                value = null;
            };
            if (value != null || item.isRequired) {
                PolicyProperties.push({
                    propertyId: id,
                    value: value
                });
            }
        });

        // Coverage model
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

        data.AdditionalProperties = AdditionalProperties;
        data.policyProperties = PolicyProperties;

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
                if (!_.isUndefined(field.index)) {
                    $scope.validationErrors[field.field + '_' + field.index] = field.errors;
                } else {
                    $scope.validationErrors[field.field] = field.errors;
                }
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

    function _openDateProperty(index) {
        $scope.openedProperty[index] = true;
    };

    function _openDateAdditional(index) {
        $scope.openedPropertyAdditional[index] = true;
    };

    function _getCoverageType(id) {
        return _.find(ConfigService.coverageTypes, { 'id': id }).name;
    }

    function _onChangeCoverage(isSelected, index) {
        if (!isSelected && index > -1) {
            $scope.data.coverages.splice(index, 1);
        }
    }

    function _hideStatus() {
        $scope.submitInProgressStatus = true;
        PolicyService.setHideStatus(id).then(function (res) {
            $scope.submitInProgressStatus = false;
            $rootScope.alerts.push({type: 'success', msg: 'Policy status has been updated.' });
        }, function (res) {
            $scope.submitInProgressStatus = false;
            $rootScope.alerts.push({type: 'danger', msg: res.data.Message });
        });
    }

    function _showStatus() {
        $scope.submitInProgressStatus = true;
        PolicyService.setShowStatus(id).then(function (res) {
            $scope.submitInProgressStatus = false;
            $rootScope.alerts.push({type: 'success', msg: 'Policy status has been updated.' });
        }, function (res) {
            $scope.submitInProgressStatus = false;
            $rootScope.alerts.push({type: 'danger', msg: res.data.Message });
        });
    }

    $scope.$watch('list.categories', function(item) {
        if (!_.isUndefined(item)) {
            _getFields(item.id, 0);
            _getFields(item.id, 1);

            var dataCoverages = {
                policyId: parseInt($stateParams.id),
                policyTypeId: item.id
            }
            PolicyService.getCoveragesDetails(dataCoverages).then(function(res) {
                $scope.coveragesDetails = res.data;
            });
        }
    });

    _.extend($scope, {
        openedStartDate: false,
        openedEndDate: false,
        openedProperty: [],
        openedPropertyAdditional: [],
        submitInProgressSave: false,
        submitInProgressConfirmed: false,
        submitInProgressStatus: false,
        changeGroups: _changeGroups,
        onSelection: _onSelection,
        openStartDate: _openStartDate,
        openEndtDate: _openEndDate,
        openBuildDate: _openBuildDate,
        openDateProperty: _openDateProperty,
        openDateAdditional: _openDateAdditional,
        getCoverageType: _getCoverageType,
        onChangeCoverage: _onChangeCoverage,
        hideStatus: _hideStatus,
        showStatus: _showStatus,
        groupFields: [],
        validationErrors: {},
        save: _save,
        paramExists: $rootScope.paramExists
    });

}]);
