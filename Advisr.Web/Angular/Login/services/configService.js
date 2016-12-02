'use strict';

angular.module('AdvisrLoginApp').factory('ConfigService', function () {
    var ConfigService = {
        urls: {
            login: '/Account/login',
            forgotPassword: '/api/user/forgotPassword',
            setNewPassword: '/api/user/setNewPassword',
            register: '/api/user/register',
            registerExternal: '/api/user/registerExternal',
            confirmEmail: '/account/ConfirmEmail'
        }
    }
    return ConfigService;
});