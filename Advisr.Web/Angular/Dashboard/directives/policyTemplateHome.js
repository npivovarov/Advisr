'use strict';

angular.module('DashboardApp').directive('policyTemplateHome', ['$parse', function ($parse) {
    return {
        restrict: 'E',
        scope: true,
        templateUrl: '/Angular/Dashboard/templates/policy/directives/policyTemplateHome.html',
    }
}]);
