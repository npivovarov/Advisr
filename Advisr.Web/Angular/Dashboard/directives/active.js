'use strict';

angular.module('DashboardApp').directive('active', function ($location) {
    return {
        link: function(scope, el, attrs) {
            scope.$watch(function () {
                var path = $location.path().split("/");
                return '#/' + path[1] == attrs.href;
            },function(newValue){
                el.toggleClass('w--current', newValue);
            });
        }
    }
});