'use strict';

angular.module('DashboardApp').controller('PortfolioController', ['$scope', '$state', '$stateParams', '$rootScope', '$interval', '$cookies', '$timeout', 'anchorSmoothScroll', 'ConfigService', 'PortfolioService', 'PolicyService', function ($scope, $state, $stateParams, $rootScope, $interval, $cookies,  $timeout, anchorSmoothScroll, ConfigService, PortfolioService, PolicyService ) {

    $scope.showPanel = true;
    $scope.data = 0;
    $scope.groupName = _.isUndefined($stateParams.groupName) ? '' : $stateParams.groupName;

    PortfolioService.getChartDetails().then(function(res) {
        $scope.loading = true;
        $scope.pending = res.data.countOfPendingPolicies;
        $scope.currency = res.data;
        delete res.data.countOfPendingPolicies;
        _.each(res.data, function (item) {
            $scope.data += item;
        });
    });

    function _showPolicy(group) {
        $scope.showPanel = true;
        $state.go('portfolioListPager', {
            groupName: group,
            status: 1, 
            page: 1 
        });

        $timeout(function () {
             anchorSmoothScroll.scrollTo('policy');
        }, 1000)
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

    function _closePortfolio() {
        $scope.showPanel = false;
    }

    $scope.$on("DataChange", function ($event, collection) {
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
        openDetails: _openDetails,
        closePortfolio: _closePortfolio
    });

}]);
