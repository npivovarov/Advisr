'use strict';

angular.module('AdvisrLoginApp').controller('LoginController', ['$scope', '$rootScope',  '$window', '$stateParams', 'LoginService', function ($scope, $rootScope, $window, $stateParams, LoginService) {

    $scope.data = {};

    if(!_.isUndefined($stateParams.error)) $rootScope.alerts.push({type: 'danger', msg: $stateParams.error });

    function _login($event) {

        $scope.submitInProgress = true;

        LoginService.login($scope.data).then(function (response) {

            $window.location.href = response.data.redirectUrl;
            $scope.submitInProgress = false;
            
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

    $scope.authExternalProvider = function (provider) {

        var externalProviderUrl = location.protocol + '//' + location.host + "/account/ExternalLogin?provider=" + provider;

        var oauthWindow = $window.location = externalProviderUrl;
    };

    _.extend($scope, {
        login: _login,
        validationErrors: {},
        paramExists: $rootScope.paramExists,
        submitInProgress: false
    })

}]);
