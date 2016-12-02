'use strict';

angular.module('DashboardApp').controller('PolicyListController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'ConfigService', 'PolicyService', function ($scope, $rootScope, $state, $stateParams, $timeout, ConfigService, PolicyService) {

    var sortId = $stateParams.sortType;
    $scope.sortBy = _.isUndefined(sortId) ? _.head(ConfigService.sortType).value : _.find(ConfigService.sortType, { 'key' : parseInt(sortId) }).value;

    function _openDetails(index, id) {
        $scope.policyDetails[index] = !$scope.policyDetails[index];
        if ($scope.policyDetails[index]) {
            PolicyService.getShortDetails(id).then(function (res) {
                console.log(res);
                $scope.policyInfo[index] = res.data;
            });
        }
    }

    function _getPayment(id) {
        return _.find(ConfigService.policyPaymentFrequency, { 'id' : id }).value;
    }

    function _sortPolicy(sortType) {
        $state.go('policiesPager', {
            sortType: sortType,
            page: 1 
        });
    }

    $scope.$on("DataChange", function ($event, collection) {
        $scope.proccessing = {
            count: collection.count,
            countOfProcessing: collection.countOfProcessing
        }
        $scope[$scope.collectionName] = collection.data;
    });

    _.extend($scope, {
        serviceName: 'PolicyService',
        collectionName: 'policies',
        fullData: true,
        loading: false,
        openDetails: _openDetails,
        getPayment: _getPayment,
        sortPolicy: _sortPolicy,
        policyInfo: [],
        policyDetails: []
    });

}]);
