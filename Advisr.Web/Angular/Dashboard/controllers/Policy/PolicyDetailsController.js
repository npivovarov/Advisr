'use strict';

angular.module('DashboardApp').controller('PolicyDetailsController', ['$scope', '$rootScope', '$state', '$stateParams', '$window', 'ConfigService', 'PolicyService', function ($scope, $rootScope, $state, $stateParams, $window, ConfigService, PolicyService) {

    var id = $stateParams.id;
    var tab = $stateParams.tabname;

    $scope.tabs = [
        { title: 'Information', url: '/Angular/Dashboard/templates/policy/details/policyInformation.html' },
        { title: 'Description', url: '/Angular/Dashboard/templates/policy/details/policyDescription.html' },
        { title: 'Cover', url: '/Angular/Dashboard/templates/policy/details/policyCover.html' },
        { title: 'Documents', url: '/Angular/Dashboard/templates/policy/details/policyDocuments.html' },
        // { title: 'F.A.Q', url: '/Angular/Dashboard/templates/policy/details/policyFaq.html' }
    ];

      $scope.groups = [
        {
            title: "How can I upgrade or make changes to my policy?",
            content: "Comprehensive Car Insurance in NSW starts from as little as $1.60 a day. For that we cover your car for accidental damage, collision or crash, severe weather, theft, vandalism or malicious acts. Plus we cover the extras on your car like alloy wheels, sunroofs and custom sound systems. It also includes hire car cover for up to 21 days if your car's stolen. ",
            open: false
        },  
        {
            title: "How can I upgrade or make changes to my policy?",
            content: "Comprehensive Car Insurance in NSW starts from as little as $1.60 a day. For that we cover your car for accidental damage, collision or crash, severe weather, theft, vandalism or malicious acts. Plus we cover the extras on your car like alloy wheels, sunroofs and custom sound systems. It also includes hire car cover for up to 21 days if your car's stolen. ",
            open: false
        }
  ];

        $scope.groupsCover = {
            standart: [],
            optional: [],
            exclusions: [],

        };
  

    $scope.currentTab = '/Angular/Dashboard/templates/policy/details/policyInformation.html';

    if (tab != null) {
        $scope.currentTab = _.find($scope.tabs, { title: tab }).url;
    }

    $scope.onClickTab = function (tab) {
        $scope.currentTab = tab.url;
    }

    $scope.isActiveTab = function (tabUrl) {
        return tabUrl == $scope.currentTab;
    }

    PolicyService.getPolicy(id).then(function (res) {
        $scope.policy = res.data;

        if ($scope.policy.properties != null) {
            for (var i = 0; i < $scope.policy.properties.length; i++) {
                if ($scope.policy.properties[i].fieldName == "Life Insurance Premium Type") {
                    $scope.policy.properties.splice(i, 1);
                }
                if ($scope.policy.properties[i].fieldType == 5) {
                    $scope.policy.properties[i].value = new Date($scope.policy.properties[i].value);
                }
            }
        }
    });

    PolicyService.getCoverages(id).then(function (res) {
        _.each(res.data, function(item) {
            item.open = false;
            switch (item.type) {
                case 0:
                    $scope.groupsCover.standart.push(item);
                    break;
                case 1:
                    $scope.groupsCover.optional.push(item);
                    break;
                case 2:
                    $scope.groupsCover.exclusions.push(item);
                    break;
            }
        });
    })

    function _getPayment(id) {
        if (_.isUndefined(id)) return;
        return _.find(ConfigService.policyPaymentFrequency, { 'id': id }).value;
    }

    function _back() {
        $window.history.back();
    }

    _.extend($scope, {
        getPayment: _getPayment,
        back: _back
    });

}]);
