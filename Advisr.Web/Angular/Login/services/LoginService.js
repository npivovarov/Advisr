'use strict';

angular.module('AdvisrLoginApp').factory('LoginService', ['$http', 'ConfigService', function ($http, ConfigService) {
    var LoginService = {}

    function _login(data, returnUrl) {
        return $http.post(ConfigService.urls.login, data, {
            params: {
                returnUrl: returnUrl
            }
        });
    }

    function _forgotPassword(data) {
        return $http.post(ConfigService.urls.forgotPassword, data);
    }

    function _setNewPassword(data) {
        return $http.post(ConfigService.urls.setNewPassword, data);
    }

    function _register(data) {
        return $http.post(ConfigService.urls.register, data);
    }

    function _registerExternal(data) {
        return $http.post(ConfigService.urls.registerExternal, data);
    }

    function _confirmEmail(data) {
        return $http.get(ConfigService.urls.confirmEmail, {
            params: {
                userId: data.userId,
                code: data.code
            }
        });
    }

    _.extend(LoginService, {
        login: _login,
        forgotPassword: _forgotPassword,
        setNewPassword: _setNewPassword,
        register: _register,
        registerExternal: _registerExternal,
        confirmEmail: _confirmEmail
    })

    return LoginService;
}]);