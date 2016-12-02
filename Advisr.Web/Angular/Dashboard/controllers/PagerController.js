'use strict';

angular.module('DashboardApp').controller('PagerController', ['$scope', '$q', '$state', '$stateParams', 'ConfigService', 'ProfileService', 'PolicyService', 'InsurersService',
function ($scope, $q, $state, $stateParams, ConfigService, ProfileService, PolicyService, InsurersService) {

    $scope.activePage = $scope.$root.activePage = parseInt($stateParams.page) || 1;

    var services = {
        ProfileService: {
            getList: ProfileService.getProfiles
        },
        PolicyService: {
            getList: PolicyService.getPolicies
        },
        PolicyAllService : {
            getList: PolicyService.getAllPolicies
        },
        PolicyPortfolio: {
            getList: PolicyService.getPoliciesPortfolio
        },
        InsurersService: {
            getList: InsurersService.getList
        }
    },

    queryParams = ['q', 'status', 'group'],
    queryParamsList = {},
    service = services[$scope.$parent.serviceName],
    args = [ConfigService.countPerPage * ($scope.activePage - 1), ConfigService.countPerPage];

    if ('id' in service) {
        args.unshift(service.id);
    }

    service.getList.apply(service, args).then(function (response) {
        var data = ('fullData' in $scope.$parent && $scope.$parent.fullData === true) ? response.data : response.data.data;

        $scope.$emit('DataChange', data);
        $scope.count = response.data.count;

        _processPager();
    }, function (response) {
        $scope.$emit('pagerError');
    });

    _.each(queryParams, function (param) {
        if (param in $stateParams && $stateParams[param]) {
            queryParamsList[param] = $stateParams[param];
        }
    });

    function _processPager() {
        var count;

        if ($scope.count > ConfigService.countPerPage) {
            var pages = Math.ceil($scope.count / ConfigService.countPerPage);
            $scope.pages = _.range(1, pages + 1, 1);


            if ($scope.activePage % 10 != 0) {
                if ($scope.activePage < 10) {
                    $scope.renderPages = $scope.pages.slice(0, 10);
                } else {
                    count = Math.floor($scope.activePage / 10);
                    $scope.renderPages = $scope.pages.slice(count * 10 - 1, count * 10 + 10);
                }

            } else if ($scope.activePage % 10 === 0) {
                count = Math.floor($scope.activePage / 10);
                $scope.renderPages = $scope.pages.slice(count * 10 - 1, count * 10 + 10);
            }
        } else {
            $scope.renderPages = [];
        }
    }

    function _buildParams() {
        var list = [];

        _.each(queryParamsList, function (val, key) {
            list.push(key + '=' + val);
        });

        return list.join('&')
    }

    function _loadPage(params, callback) {
        callback = callback || function (response) {
            if (!response.data.data.length && $scope.activePage > 1) {
                $scope.activePage--;
                return $scope.goPage(null, $scope.activePage);
            }
            $scope.$emit('DataChange', response.data.data);
            $scope.count = response.data.count;
            _processPager();
        }
        service.getList.apply(service, params).then(callback);
    }

    function _goPage($event, page) {
        if (_.isObject($event)) $event.preventDefault();


        if ($scope.editSchedule && _.size($scope.editedItems)) {
            return $scope.saveChanges(true).then(function () {
                $state.go($scope.collectionName + 'Pager', _.extend({}, $stateParams, { page: page }));
            }, function () {
                $state.go($scope.collectionName + 'Pager', _.extend({}, $stateParams, { page: page }));
            });
        }
        $state.go($scope.collectionName + 'Pager', _.extend({}, $stateParams, { page: page }));
    }



    function _getPrevPage() {
        var page = $scope.activePage - 1;
        if (page < 1) {
            return 1;
        }
        return page;
    }

    function _getNextPage() {
        var lastPage = _.last($scope.pages),
            page = $scope.activePage + 1;

        if (page > lastPage) {
            return lastPage;
        }

        return page;
    }

    function _getLastPage() {
        return _.last($scope.pages);
    }

    function _notFirst() {
        return $scope.activePage != 1;
    }

    function _notLast() {
        return $scope.activePage != _.last($scope.pages);
    }

    $scope.$on('item:removed', function () {
        console.log('item:removed');
        _loadPage([ConfigService.countPerPage * ($scope.activePage - 1), ConfigService.countPerPage]);
    });


    $scope.$on('add:item', function ($event, data) {
        var params = [ConfigService.countPerPage * ($scope.activePage - 1), ConfigService.countPerPage];

        if (_.isObject(data) && _.isNumber(data.id)) {
            var objectId = data.id;

            _loadPage(params, function (response) {
                /**
                 * If last object in current list is not last added object. Increment page.
                 */
                if (_.first(response.data.data).id != objectId) {
                    return $scope.goPage(null, 1);
                }
                $scope.$emit('DataChange', response.data.data);
                $scope.count = response.data.count;
                _processPager();
            });
        } else if (_.isObject(data) && ('entityId' in data)) {
            params.unshift(data.entityId);
            _loadPage(params);
        } else {
            _loadPage(params);
        }

    });

    _.extend($scope, {
        getLastPage: _getLastPage,
        notFirst: _notFirst,
        notLast: _notLast,
        getPrevPage: _getPrevPage,
        getNextPage: _getNextPage,
        goPage: _goPage,
        buildParams: _buildParams
    })
}]);