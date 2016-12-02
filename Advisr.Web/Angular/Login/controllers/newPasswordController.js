'use strict';

angular.module('AdvisrLoginApp').controller('NewPasswordController', ['$scope', '$rootScope',  '$window', '$stateParams', '$state', '$timeout', 'LoginService', function ($scope, $rootScope, $window, $stateParams, $state, $timeout, LoginService) {

    $scope.data = {
        id: $stateParams.userId,
        code: _.isUndefined($stateParams.code) ? '' : $stateParams.code
    };

    function _savePassword($event) {
        
        $scope.submitInProgress = true;

        LoginService.setNewPassword($scope.data).then(function (response) {

            $scope.submitInProgress = false; 
            $state.go('login');

            $timeout(function () {
                $rootScope.alerts.push({ type: 'success', msg: 'Password has been changed.' });
            }, 1000)

        }, function (response) {
            var data = response.data;

            $scope.submitInProgress = false;

            if ('Message' in data) {
                $rootScope.alerts.push({type: 'danger', msg: data.Message });
            }

            _.each(data.Details, function (field) {
                $scope.validationErrors[field.field] = field.errors;
            });
        })
    }

    _.extend($scope, {
        savePassword: _savePassword,
        validationErrors: {},
        paramExists: $rootScope.paramExists,
        submitInProgress: false
    })

}]);
