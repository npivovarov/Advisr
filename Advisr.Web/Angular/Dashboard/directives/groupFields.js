'use strict';

angular.module('DashboardApp').directive('groupFields', ['$parse', function ($parse) {
    return {
        restrict: 'E',
        scope: true,
        templateUrl: '/Angular/Dashboard/templates/groupFields.html',
        link: function ($scope, element, attrs) {
            $scope.$watch("groupFields", function(value) {
                if (value.length > 0) {
                    console.log(value);
                }
            })
        }
    }
}]);
