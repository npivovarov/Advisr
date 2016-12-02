'use strict';

angular.module('DashboardApp').factory('UserService', ['$http', 'ConfigService', function ($http, ConfigService) {
    var UserService = {}

    function _getProfile(data) {
        return $http.get(ConfigService.urls.user.get);
    }

    function _saveProfile(data) {
        return $http.post(ConfigService.urls.user.save, data);
    }

    function _getCustomer(userId) {
        return $http.get(ConfigService.urls.user.getCustomer, {
            params: {
                userId: userId
            }
        });
    }

    function _changePassword(data) {
        return $http.post(ConfigService.urls.user.changePassword, data);
    }

    _.extend(UserService, {
        getProfile: _getProfile,
        saveProfile: _saveProfile,
        getCustomer: _getCustomer,
        changePassword: _changePassword
    })

    return UserService;

}]);