'use strict';

angular.module('DashboardApp').controller('NotificationsController', ['$scope', '$rootScope', '$state', '$stateParams', '$sce', 'NotificationsService',  'ConfigService', function ($scope, $rootScope, $state, $stateParams, $sce, NotificationsService, ConfigService) {

    $scope.$on("DataChange", function ($event, collection) {
        $scope.count = {
            countOfRead: collection.countOfRead,
            countOfUnread: collection.countOfUnread
        };

        $scope[$scope.collectionName] = collection.data;
    });

    function _showDetails(index, id) {
        var title = $scope.notifications[index].subjectTitleFirst,
            unread = $scope.notifications[index].isUnread;
        
        if (!$scope.notificationDetails[index]) {    
            NotificationsService.getDetails(id).then(function (res) {
                $scope.notifications[index].details = res.data;
                var body = $scope.notifications[index].details.body,
                    isUnread = $scope.notifications[index].details.isUnread;
                $scope.notifications[index].details.body = $sce.trustAsHtml(body);

                if (isUnread) {
                    NotificationsService.setMarkRead(id).then(function (res) {
                        $scope.count.countOfRead++;
                        $scope.count.countOfUnread--;
                        $rootScope.alerts.push({ type: 'success', msg: title + ' has been read!' });
                        $scope.notifications[index].isUnread = false;
                        $rootScope.countNotification--;
                    });
                }
            });
        }
         $scope.notificationDetails[index] = !$scope.notificationDetails[index];
    }

    _.extend($scope, {
        serviceName: 'NotificationsService',
        collectionName: 'notifications',
        fullData: true,
        notificationDetails: [],
        showDetails: _showDetails
    });

}]);
