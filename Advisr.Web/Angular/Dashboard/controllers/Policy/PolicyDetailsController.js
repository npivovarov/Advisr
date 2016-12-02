'use strict';

angular.module('DashboardApp').controller('PolicyDetailsController', ['$scope', '$rootScope', '$state', '$stateParams', 'ConfigService', 'PolicyService', function ($scope, $rootScope, $state, $stateParams, ConfigService, PolicyService) {

    $scope.tabs = [
        { title: 'Information', url: '' },
        { title: 'Description', url: '' },
        { title: 'Cover', url: '' },
        { title: 'Documents', url: '' },
        { title: 'F.A.Q', url: '' }
    ]

    _.extend($scope, {
    });

}]);
