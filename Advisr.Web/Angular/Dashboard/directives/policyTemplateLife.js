'use strict';

angular.module('DashboardApp').directive('policyTemplateLife', ['$parse', function ($parse) {
    return {
        restrict: 'E',
        scope: true,
        templateUrl: '/Angular/Dashboard/templates/policy/directives/policyTemplateLife.html',
    }
}]);
