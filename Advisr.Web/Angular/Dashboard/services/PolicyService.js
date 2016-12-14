'use strict';

angular.module('DashboardApp').factory('PolicyService', ['$http', '$stateParams', 'ConfigService', function ($http, $stateParams, ConfigService) {
    var PolicyService = {}

    function _getPolicies(offset, count) {
        var params = {
            offset: offset,
            count: count,
            sortType: $stateParams.sortType,
            q: $stateParams.q || null
        }

        return $http.get(ConfigService.urls.policy.list, {
            params: params
        });
    }

    function _getAllPolicies(offset, count) {
        var params = {
            offset: offset,
            count: count,
            status: $stateParams.status || -1,
            q: $stateParams.q || null
        }

        return $http.get(ConfigService.urls.policy.viewAll, {
            params: params
        });
    }

    function _getPoliciesPortfolio(offset, count) {
        var params = {
            offset: offset,
            count: count,
            status: 1,
            active: 1,
            groupName: $stateParams.groupName,
            q: $stateParams.q || null
        }

        return $http.get(ConfigService.urls.policy.list, {
            params: params
        });
    }

    function _getInsurerList(offset, count) {
        var params = {
            offset: offset,
            count: count,
            q: $stateParams.q || null
        }

        return $http.get(ConfigService.urls.policy.insurerList, {
            params: params
        });
    }

    function _createPolicy(data) {
        return $http.post(ConfigService.urls.policy.create, data);
    }

    function _getPolicy(id) {
        return $http.get(ConfigService.urls.policy.get + id);
    }

    function _addNewFile(data) {
        return $http.post(ConfigService.urls.file.addNewPolicy, data);
    }

    function _getGroups(id) {
        return $http.get(ConfigService.urls.policy.groups, {
                params: {
                    insurerId: id
                }
            });
    }

    function _getGroupFields(data) {
        return $http.get(ConfigService.urls.policy.groupFields, {
            params: data
        });
    }

    function _getFilledGroupFields(groupId, policyId) {
        return $http.get(ConfigService.urls.policy.getFilledFields, {
            params: {
                policyTypeId: groupId,
                policyId: policyId
            }
        });
    }

    function _updatePolicy(data) {
        return $http.post(ConfigService.urls.policy.update, data);
    }

    function _getShortDetails(id) {
        return $http.get(ConfigService.urls.policy.shortDetails, {
            params: {
                id: id
            }
        });
    }

    function _getCoveragesDetails(data) {
        return $http.get(ConfigService.urls.policy.coveragesDescription, {
            params: {
                policyId: data.policyId,
                policyTypeId: data.policyTypeId
            }
        });
    }

    function _getCoverages(id) {
        return $http.get(ConfigService.urls.policy.getCoverages, {
            params: {
                id: id
            }
        });
    }

    function _setHideStatus(id) {
        return $http.post(ConfigService.urls.policy.hide, {}, { params: { id: id } })
    }

    function _setShowStatus(id) {
        return $http.post(ConfigService.urls.policy.show, {}, { params: { id: id } })
    }

    _.extend(PolicyService, {
        createPolicy: _createPolicy,
        getPolicies: _getPolicies,
        getPolicy: _getPolicy,
        addNewFile: _addNewFile,
        getAllPolicies: _getAllPolicies,
        getGroups: _getGroups,
        getGroupFields: _getGroupFields,
        getFilledGroupFields: _getFilledGroupFields,
        getInsurerList: _getInsurerList,
        updatePolicy: _updatePolicy,
        getShortDetails: _getShortDetails,
        getPoliciesPortfolio: _getPoliciesPortfolio,
        getCoveragesDetails: _getCoveragesDetails,
        getCoverages: _getCoverages,
        setHideStatus: _setHideStatus,
        setShowStatus: _setShowStatus
    })

    return PolicyService;

}]);