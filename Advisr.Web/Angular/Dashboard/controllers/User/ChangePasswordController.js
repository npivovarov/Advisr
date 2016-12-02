'use strict';

angular.module('DashboardApp').controller('ChangePasswordController', ['$scope', '$rootScope', '$interval', '$cookies', 'ConfigService', 'UserService', function ($scope, $rootScope, $interval, $cookies, ConfigService, UserService ) {

    var userId = $cookies.get('userId');
    $scope.data = {};
            console.log($rootScope.currentUser);
    function _save() {
        var profile = angular.copy($scope.data);

        if (!$rootScope.currentUser.hasPassword){
            profile.oldPassword = 'external';
        }

        $scope.submitInProgress = true;

        UserService.changePassword(profile).then(function(res){
            $scope.submitInProgress = false;
            $rootScope.alerts.push({type: 'success', msg: 'Password has been saved.' });

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
        submitInProgress: false,
        save: _save,
        validationErrors: {},
        paramExists: $rootScope.paramExists
    });

}]);
