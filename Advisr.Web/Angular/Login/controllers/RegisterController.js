'use strict';

angular.module('AdvisrLoginApp').controller('RegisterController', ['$scope', '$rootScope',  '$window', '$stateParams', '$state', '$timeout', 'LoginService', function ($scope, $rootScope, $window, $stateParams, $state, $timeout, LoginService) {

    $scope.data = {
        email: _.isUndefined($stateParams.email) ? '' : $stateParams.email
    };

    $scope.external = !_.isUndefined($stateParams.external_access_token);

    function _register($event) {

        $scope.submitInProgress = true;

        LoginService.register($scope.data).then(function (response) {

            $scope.submitInProgress = false;
            $state.go('login');

            $timeout(function () {
                $rootScope.alerts.push({ type: 'success', msg: 'You have been sent a confirmation email.' });
            }, 1000)

        }, function (response) {
            var data = response.data;

            $scope.submitInProgress = false;

            if ('Message' in data) {
                $rootScope.alerts.push({ type: 'danger', msg: data.Message });
            }

            _.each(data.Details, function (field) {
                $scope.validationErrors[field.field] = field.errors;
            });
        })
    }

    function _registerExternal() {

        $scope.submitInProgress = true;
        var names = $stateParams.external_user_name.split(' ');

        var data = {
            email: $scope.data.email,
            userName: $stateParams.external_user_name,
            provider: $stateParams.provider,
            externalAccessToken: $stateParams.external_access_token,
            firstName: names[0] == null ? "" : names[0],
            lastName: names[1] == null ? "" : names[1],
            contactPhone: $scope.data.contactPhone
        };
        
        LoginService.registerExternal(data).then(function (response) {

            $scope.submitInProgress = false;
            $state.go('login');

            $timeout(function () {
                $rootScope.alerts.push({ type: 'success', msg: 'You have been sent a confirmation email.' });
            }, 1000)

        }, function (response) {
            var data = response.data;

            $scope.submitInProgress = false;

            if ('Message' in data) {
                $rootScope.alerts.push({ type: 'danger', msg: data.Message });
            }

            _.each(data.Details, function (field) {
                $scope.validationErrors[field.field] = field.errors;
            });
        }) 
    }

    function _authExternalProvider(provider) {

        var externalProviderUrl = location.protocol + '//' + location.host + "/account/ExternalLogin?provider=" + provider;

        var oauthWindow = $window.location = externalProviderUrl;
    };


    _.extend($scope, {
        validationErrors: {},
        paramExists: $rootScope.paramExists,
        submitInProgress: false,
        register: _register,
        registerExternal: _registerExternal,
        authExternalProvider: _authExternalProvider
    });

}]);
