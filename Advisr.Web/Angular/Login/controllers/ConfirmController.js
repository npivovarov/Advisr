'use strict';

angular.module('AdvisrLoginApp').controller('ConfirmController', ['$scope', '$rootScope', '$timeout', '$interval', '$location', '$stateParams', 'LoginService', function ($scope, $rootScope, $timeout, $interval, $location, $stateParams, LoginService) {

    $scope.timer = 3;

    LoginService.confirmEmail($stateParams).then(function (res) {
        $scope.confirm = res.data.succeeded;

        $interval(function () {
            $scope.timer--;
        }, 1000, 0);

        $timeout(function() {
            $location.path('account/login');
        }, 3000);
        
    }, function (res) {
        $scope.confirm = false;
    });

}]);
