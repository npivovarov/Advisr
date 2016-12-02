'use strict';

angular.module('DashboardApp').directive('policyTemplateVehicle', ['$parse', function ($parse) {
    return {
        restrict: 'E',
        scope: true,
        templateUrl: '/Angular/Dashboard/templates/policy/directives/policyTemplateVehicle.html',
    }
}]);
