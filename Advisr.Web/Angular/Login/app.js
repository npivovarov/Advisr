'use strict';

var AdvisrLoginApp = angular.module('AdvisrLoginApp', [
    'ui.router',
    'ngAnimate',
    'angular-button-spinner',
    'ui.checkbox',
    'anim-in-out',
    'ui.bootstrap',
    'ngMask'
]);

AdvisrLoginApp.config(['$stateProvider', '$httpProvider', '$urlRouterProvider',
    function ($stateProvider, $httpProvider, $urlRouterProvider) {
        $stateProvider
            .state('login', {
                url: '/login?error',
                templateUrl: '/Angular/Login/templates/login/login.html',
                controller: 'LoginController'
            })
             .state('forgot-password', {
                url: '/forgot-password',
                templateUrl: '/Angular/Login/templates/forgotPassword/forgotPassword.html',
                controller: 'ForgotPasswordController'
            })
             .state('new-password', {
                url: '/set-new-password?userId&code',
                templateUrl: '/Angular/Login/templates/newPassword/newPassword.html',
                controller: 'NewPasswordController'
            })
            .state('register', {
                url: '/register?external_access_token&external_user_name&email&provider',
                templateUrl: '/Angular/Login/templates/register/register.html',
                controller: 'RegisterController'
            })
            .state('confirm', {
                url: '/confirm-email?userId&code',
                templateUrl: '/Angular/Login/templates/confirm/confirm.html',
                controller: 'ConfirmController'
            })
        ;

        $urlRouterProvider.otherwise('/login');  
}]);

AdvisrLoginApp.run(['$rootScope', function ($rootScope) {

        $rootScope.clearError = function (param, object) {
            if (!_.isObject(object)) return;
            if (param in object) {
                delete object[param];
            }
        }

        $rootScope.paramExists = function (param, object) {
            return (param in object);
        }

        //warning, danger, success, info
        function _closeAlert(index) {
            $rootScope.alerts.splice(index, 1);
        };

        _.extend($rootScope, {
            closeAlert: _closeAlert,
            alerts: []
        });
}]);


