'use strict';

angular.module('DashboardApp').factory('PortfolioService', ['$http', 'ConfigService', function ($http, ConfigService) {
    var PortfolioService = {}

    function _getChartDetails(id) {
        return $http.get(ConfigService.urls.portfolio.chartDetails);
    }

    _.extend(PortfolioService, {
        getChartDetails: _getChartDetails
    })

    return PortfolioService;

}]);