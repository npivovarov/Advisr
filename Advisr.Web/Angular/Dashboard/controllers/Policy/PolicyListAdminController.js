'use strict';

angular.module('DashboardApp').controller('PolicyListAdminController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'ConfigService', 'PolicyService', function ($scope, $rootScope, $state, $stateParams, $timeout, ConfigService, PolicyService) {

    var ENTER_KEY_CODE = 13;
    var statusId = parseInt($stateParams.status);

    $scope.status = ConfigService.policyStatus;
    $scope.data = {
        status: _.isUndefined($stateParams.status) ? _.head($scope.status) :  _.find(ConfigService.policyStatus, { 'key' : statusId }),
        search: _.isUndefined($stateParams.q) ? '' :  $stateParams.q
    }

    function _search($event) {
        if (($event.type == 'keyup' && $event.keyCode == ENTER_KEY_CODE)) {
            $state.go('policiesAdminPager', { q: $scope.data.search, page: 1 });
        }
    }

    function _getStatus(id) {
        return _.find(ConfigService.policyStatus, { 'key' : id }).value;
    }

    function _changeStatus(item) {
        $state.go('policiesAdminPager', { q: $scope.data.search, status: item.key, page: 1 });
    }

    $scope.$on("DataChange", function ($event, collection) {
        $scope[$scope.collectionName] = collection;
    });

    _.extend($scope, {
        serviceName: 'PolicyAllService',
        collectionName: 'policiesAdmin',
        loading: false,
        getStatus: _getStatus,
        search: _search,
        changeStatus: _changeStatus
    });

}]);
