'use strict';

angular.module('DashboardApp').factory('NotificationsService', ['$http', '$stateParams', 'ConfigService', function ($http, $stateParams, ConfigService) {
    var NotificationsService = {}
  
    function _getNotificationList(offset, count) {
        var params = {
            offset: offset,
            count: count,
        }

        return $http.get(ConfigService.urls.notification.list, {
            params: params
        });
    }

    function _getCounter(data) {
        return $http.get(ConfigService.urls.notification.getCounter);
    }

    function _setMarkRead(id) {
        return $http.post(ConfigService.urls.notification.mark + id);
    }

    function _getDetails(id) {
        return $http.get(ConfigService.urls.notification.get + id);
    }

    _.extend(NotificationsService, {
        getNotificationList: _getNotificationList,
        getCounter: _getCounter,
        setMarkRead: _setMarkRead,
        getDetails: _getDetails
    })

    return NotificationsService;

}]);