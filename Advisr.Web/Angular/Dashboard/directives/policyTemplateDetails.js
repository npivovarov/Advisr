'use strict';

angular.module('DashboardApp').directive('policyTemplateDetails', ['$parse', function ($parse) {
    return {
        restrict: 'E',
        scope: true,
        templateUrl: '/Angular/Dashboard/templates/policy/directives/policyTemplateDetails.html',
    }
}]);
