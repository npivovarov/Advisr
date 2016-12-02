'use strict';

angular.module('DashboardApp').controller('PortfolioController', ['$scope', '$state', '$stateParams', '$rootScope', '$interval', '$cookies', 'ConfigService', 'PortfolioService', 'PolicyService', function ($scope, $state, $stateParams, $rootScope, $interval, $cookies, ConfigService, PortfolioService, PolicyService ) {

    $scope.data = 0;
    $scope.groupName = _.isUndefined($stateParams.groupName) ? '' : $stateParams.groupName;

    PortfolioService.getChartDetails().then(function(res) {
        $scope.loading = true;
        $scope.currency = res.data;
        _.each(res.data, function (item) {
            $scope.data += item;
        });
    });

    function _showPolicy(group) {
        $state.go('portfolioListPager', {
            groupName: group,
            status: 1, 
            page: 1 
        });
    }

    function _openDetails(index, id) {
        $scope.policyDetails[index] = !$scope.policyDetails[index];
        if ($scope.policyDetails) {
            PolicyService.getShortDetails(id).then(function (res) {
                $scope.policyInfo[index] = res.data;
            });
        }
    }

    function _getPayment(id) {
        return _.find(ConfigService.policyPaymentFrequency, { 'id' : id }).value;
    }

    $scope.$on("DataChange", function ($event, collection) {
        console.log(collection);
        $scope[$scope.collectionName] = collection;
    });

    
    _.extend($scope, {
        serviceName: 'PolicyPortfolio',
        collectionName: 'portfolioList',
        policyInfo: [],
        policyDetails: [],  
        loading: false,
        showPolicy: _showPolicy,
        getPayment: _getPayment,
        openDetails: _openDetails
    });

}]);
