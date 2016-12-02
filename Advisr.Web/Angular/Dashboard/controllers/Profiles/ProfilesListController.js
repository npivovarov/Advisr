'use strict';

angular.module('DashboardApp').controller('ProfileListController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'ConfigService', 'ProfileService', function ($scope, $rootScope, $state, $stateParams, $timeout, ConfigService, ProfileService ) {

    var ENTER_KEY_CODE = 13;
    $scope.searchData = _.isUndefined($stateParams.q) ? '' : $stateParams.q;
    $scope.locked = null;

    function _search($event) {
        if (($event.type == 'keyup' && $event.keyCode == ENTER_KEY_CODE)) {
            $state.go('profilesListPager', { q: $scope.searchData, page: 1 });
        }
    }

    function _lock(id, $index) {
        var data = id;
        $scope.submitInProgressLock[$index] = true;
  
        ProfileService.lockProfile(data).then(function(res) {
            $rootScope.alerts.push({type: 'success', msg: 'Profile has been locked.' });
            $scope.profilesList[$index].locked = true;

            $timeout(function() {
                $scope.submitInProgressLock[$index] = false;
            }, 1000)

        }, function(res) {
            var data = res.data;

            $scope.submitInProgressLock[$index] = false;

            if ('Message' in data) {
                $rootScope.alerts.push({ type: 'danger', msg: data.Message });
            }
        });
    }

    function _unlock(id, $index) {
        var data = id;
        $scope.submitInProgressUnlock[$index] = true;

        ProfileService.unlockProfile(data).then(function (res) {
            $rootScope.alerts.push({type: 'success', msg: 'Profile has been unlocked.' });
            $scope.profilesList[$index].locked = false;

            $timeout(function () {
                $scope.submitInProgressUnlock[$index] = false;
            }, 1000)

        }, function(res) {
            var data = res.data;

            $scope.submitInProgressUnlock[$index] = false;

            if ('Message' in data) {
                $rootScope.alerts.push({ type: 'danger', msg: data.Message });
            }
        });
    }

    $scope.$on("DataChange", function ($event, collection) {
        $scope[$scope.collectionName] = collection;
    });

    _.extend($scope, {
        serviceName: 'ProfileService',
        collectionName: 'profilesList',
        loading: false,
        submitInProgressLock: [],
        submitInProgressUnlock: [],
        search: _search,
        lock: _lock,
        unlock: _unlock
    });

}]);
