'use strict';

angular.module('DashboardApp').controller('EditProfileController', ['$scope', '$rootScope', '$state', '$stateParams', '$timeout', 'ConfigService', 'UserService', 'ProfileService', function ($scope, $rootScope, $state, $stateParams, $timeout, ConfigService, UserService, ProfileService) {

    var userId = $stateParams.userId;

    $scope.stateList = ConfigService.userProfileState;
    $scope.data = {
        roles: [],
    };

    $scope.openDate = function() {
        $scope.openedPopup = true;
    };

    UserService.getCustomer(userId).then(function(res) {
        $scope.data = res.data;
        var state = res.data.address.state;
        $scope.data.address.state = (_.isNull(state)) ? $scope.stateList[0] : _.find($scope.stateList, {'value' : state});
        $scope.data.dateOfBirth =  (_.isNull(res.data.dateOfBirth)) ? '' : new Date(res.data.dateOfBirth);

        _.each(res.data.roles, function (item) {
            if (item === 'ADMIN') {
                $scope.data.roles[0]= item;
            }
            if (item === 'CUSTOMER') {
                $scope.data.roles[1] = item;
            }
        });

    });

    function _save() {
        var profile = angular.copy($scope.data);
        
        profile.address.state = $scope.data.address.state.value;
        profile.roles = $scope.data.roles.filter(Boolean);
        $scope.submitInProgress = true;

        if (profile.contactPhone != null) {
            var phone = profile.contactPhone.search(/[a-zA-Z,./]/i);
            if (phone != -1) {
                $scope.validationErrors['phone'] = {
                    error: 'Phone contains some invalid character.'
                };
                $scope.submitInProgress = false;
                return;
            }
        }

        console.log( profile.roles);
        UserService.saveProfile(profile).then(function(res){
            $scope.submitInProgress = false;
            $rootScope.alerts.push({type: 'success', msg: 'Customer ' + profile.firstName  + ' has been saved.' });

        }, function(res) {
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

    function _lock(id) {
        var data = id;
        $scope.submitInProgressLock = true;
  
        ProfileService.lockProfile(data).then(function(res) {
            $rootScope.alerts.push({type: 'success', msg: 'Profile has been locked.' });
            $scope.data.locked = true;

            $timeout(function() {
                $scope.submitInProgressLock = false;
            }, 1000)

        }, function(res) {
            var data = res.data;

            $scope.submitInProgressLock = false;

            if ('Message' in data) {
                $rootScope.alerts.push({ type: 'danger', msg: data.Message });
            }
        });
    }

    function _unlock(id) {
        var data = id;
        $scope.submitInProgressUnlock = true;

        ProfileService.unlockProfile(data).then(function (res) {
            $rootScope.alerts.push({type: 'success', msg: 'Profile has been unlocked.' });
            $scope.data.locked = false;

            $timeout(function () {
                $scope.submitInProgressUnlock = false;
            }, 1000)

        }, function(res) {
            var data = res.data;

            $scope.submitInProgressUnlock = false;

            if ('Message' in data) {
                $rootScope.alerts.push({ type: 'danger', msg: data.Message });
            }
        });
    }

    _.extend($scope, {
        openedPopup: false,
        submitInProgressLock: false,
        submitInProgressUnlock: false,
        save: _save,
        validationErrors: {},
        paramExists: $rootScope.paramExists,
        lock: _lock,
        unlock: _unlock
    });

}]);
