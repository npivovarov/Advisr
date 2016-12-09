'use strict';

angular.module('DashboardApp').directive('groupFields', ['$parse', function ($parse) {
    return {
        restrict: 'E',
        scope: true,
        templateUrl: '/Angular/Dashboard/templates/groupFields.html',
        link: function ($scope, element, attrs) {  
        },
        controller: function ($scope) {
            $scope.openDate = function () {
                $scope.datepicker = { 'opened': true };
            };
        }
    }
}]);
