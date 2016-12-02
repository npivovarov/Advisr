'use strict';

angular.module('DashboardApp').controller('UserProfileController', ['$scope', '$rootScope', '$interval', '$cookies', '$state', 'ConfigService', 'UserService', function ($scope, $rootScope, $interval, $cookies, $state, ConfigService, UserService ) {

    var userId = $cookies.get('userId');

    $scope.stateList = ConfigService.userProfileState;
    $scope.data = {
        address : {
            state: {}
        }
    };
    
    $scope.openDate = function() {
        $scope.openedPopup = true;
    };

    UserService.getCustomer(userId).then(function(res) {
        $scope.data = res.data;
        var state = res.data.address.state;
        $scope.data.address.state = (_.isNull(state)) ? $scope.stateList[0] : _.find($scope.stateList, {'value' : state});
        $scope.data.dateOfBirth = (_.isNull(res.data.dateOfBirth)) ? '' : new Date(res.data.dateOfBirth);
    });

    function _save() {
        var profile = angular.copy($scope.data);
        
        profile.address.state = $scope.data.address.state.value;
        profile.modifiedReason = 'user modification';

        $scope.submitInProgress = true;
        $rootScope.currentUser.isProfileCompleted = true;

        UserService.saveProfile(profile).then(function(res){
            $scope.submitInProgress = false;
            $state.go('portfolioList');
            $rootScope.alerts.push({type: 'success', msg: 'Profile has been saved.' });

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
    
    _.extend($scope, {
        openedPopup: false,
        submitInProgress: false,
        save: _save,
        validationErrors: {},
        paramExists: $rootScope.paramExists
    });

}]);
