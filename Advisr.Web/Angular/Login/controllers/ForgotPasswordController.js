'use strict';

angular.module('AdvisrLoginApp').controller('ForgotPasswordController', ['$scope', '$rootScope', '$window', '$state', '$timeout', 'LoginService', function ($scope, $rootScope, $window, $state, $timeout, LoginService) {

    $scope.data = {};

    function _resetPassword($event) {
        
        $scope.submitInProgress = true;

        LoginService.forgotPassword($scope.data).then(function (response) {

            $scope.submitInProgress = false;
            $state.go('login');

            $timeout(function () {
                $rootScope.alerts.push({ type: 'success', msg: 'Сheck the mailbox!' });
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
        resetPassword: _resetPassword,
        validationErrors: {},
        paramExists: $rootScope.paramExists,
        submitInProgress: false
    })

}]);
