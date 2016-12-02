'use strict';

angular.module('DashboardApp').factory('ProfileService', ['$http', '$stateParams', 'ConfigService', function ($http, $stateParams, ConfigService) {
    var ProfileService = {}
  
    function _getProfiles(offset, count) {
        var params = {
            offset: offset,
            count: count,
            q: $stateParams.q || null
        }

        return $http.get(ConfigService.urls.profile.list, {
            params: params
        });
    }

    function _lockProfile(data) {
        return $http.post(ConfigService.urls.profile.customerLock + data);
    }

    function _unlockProfile(data) {
        return $http.post(ConfigService.urls.profile.customerUnlock + data);
    }
    
    _.extend(ProfileService, {
        getProfiles: _getProfiles,
        lockProfile: _lockProfile,
        unlockProfile: _unlockProfile
    })

    return ProfileService;

}]);