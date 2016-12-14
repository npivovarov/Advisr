angular.module('DashboardApp').factory('InsurersService', ['$http', '$stateParams', 'ConfigService', function ($http, $stateParams, ConfigService) {
    var InsurersService = {}

    function _getInsurer(id) {
        return $http.get(ConfigService.urls.insurer.get + id);
    }

    function _saveInsurer(data) {
        return $http.post(ConfigService.urls.insurer.edit, data);
    }

    function _create(data) {
        return $http.post(ConfigService.urls.insurer.create, data);
    }

    function _getList(offset, count) {
        var params = {
            offset: offset,
            count: count,
            q: $stateParams.q || null
        }

        return $http.get(ConfigService.urls.insurer.list, {
            params: params
        });
    }

    function _delete(data) {
        return $http.post(ConfigService.urls.insurer.delete + data);
    }

    function _getPolicyTypes(id) {
        return $http.get(ConfigService.urls.insurer.getPolicyTypes, {
            params: {
                insurerId: id
            }
        });
    }

    function _getPolicyType(policyTypeId) {
        return $http.get(ConfigService.urls.insurer.getPolicyType, {
            params: {
                policyTypeId: policyTypeId
            }
        });
    }

    function _addPolicyType(data) {
        return $http.post(ConfigService.urls.insurer.addPolicyType, data);
    }

    function _addField(data) {
        return $http.post(ConfigService.urls.insurer.addField, data);
    }

    function _deleteField(data) {
        return $http.post(ConfigService.urls.insurer.deleteField + data);
    }

    function _editPolicyType(data) {
        return $http.post(ConfigService.urls.insurer.editPolicyType, data);
    }

    function _saveField(data) {
        return $http.post(ConfigService.urls.insurer.saveField, data);
    }

    function _getField(fieldId) {
        return $http.get(ConfigService.urls.insurer.getField, {
            params: {
                fieldId: fieldId
            }
        });
    }

    function _getInsurersCoverages(insurerId) {
        return $http.get(ConfigService.urls.insurer.getCoverages, {
            params: {
                insurerId: insurerId
            }
        });
    }

    function _getPolicyTypeCoverages(policyTypeId) {
        return $http.get(ConfigService.urls.insurer.getCoverages, {
            params: {
                policyTypeId: policyTypeId
            }
        });
    }

    function _getCoverage(id) {
        return $http.get(ConfigService.urls.insurer.getCoverage + id);
    }

    function _saveCoverage(data) {
        return $http.post(ConfigService.urls.insurer.saveCoverage, data);
    }

    function _addCoverage(data) {
        return $http.post(ConfigService.urls.insurer.addCoverage, data);
    }

    function _assignCoverage(data) {
        return $http.post(ConfigService.urls.insurer.assignCoverage, data);
    }

    function _getGroups() {
        return $http.get(ConfigService.urls.insurer.getGroups);
    }

    function _getGroupTemplates(groupId) {
        return $http.get(ConfigService.urls.insurer.getGroupTemplates, {
            params: {
                groupId: groupId
            }
        });
    }

    _.extend(InsurersService, {
        create: _create,
        getInsurer: _getInsurer,
        save: _saveInsurer,
        getList: _getList,
        delete: _delete,
        getPolicyTypes: _getPolicyTypes,
        getPolicyType: _getPolicyType,
        addPolicyType: _addPolicyType,
        editPolicyType: _editPolicyType,
        addField: _addField,
        deleteField: _deleteField,
        saveField: _saveField,
        getField: _getField,
        getInsurersCoverages: _getInsurersCoverages,
        getPolicyTypeCoverages: _getPolicyTypeCoverages,
        getCoverage: _getCoverage,
        saveCoverage: _saveCoverage,
        addCoverage: _addCoverage,
        assignCoverage: _assignCoverage,
        getGroups: _getGroups,
        getGroupTemplates: _getGroupTemplates
    })

    return InsurersService;

}]);