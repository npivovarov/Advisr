'use strict';

angular.module('DashboardApp').controller('InsurersListAdminController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'ConfigService', 'InsurersService', function ($scope, $rootScope, $state, $stateParams, $timeout, ConfigService, InsurersService) {
    var ENTER_KEY_CODE = 13;
    $scope.searchData = _.isUndefined($stateParams.q) ? '' : $stateParams.q;

    function _search($event) {
        if (($event.type == 'keyup' && $event.keyCode == ENTER_KEY_CODE)) {
            $state.go('insurersListPager', { q: $scope.searchData, page: 1 });
        }
    }

    $scope.$on("DataChange", function ($event, collection) {
        console.log(collection);
        $scope[$scope.collectionName] = collection;
    });

    function _deleteInsurer(id) {
        InsurersService.delete(id).then(function (res) {
            $state.reload();
            $rootScope.alerts.push({ type: 'success', msg: 'Insurer has been removed.' });
        });
    }

    _.extend($scope, {
        serviceName: 'InsurersService',
        collectionName: 'insurersList',
        delete: _deleteInsurer,
        search: _search,
        loading: false,
    });

}]);

