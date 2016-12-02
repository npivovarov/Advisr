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

    function _getGroups(id) {
        return $http.get(ConfigService.urls.insurer.getGroups, {
            params: {
                insurerId: id
            }
        });
    }

    function _getGroup(groupId) {
        return $http.get(ConfigService.urls.insurer.getGroup, {
            params: {
                groupId: groupId
            }
        });
    }

    function _addGroup(data) {
        return $http.post(ConfigService.urls.insurer.addGroup, data);
    }

    function _addField(data) {
        return $http.post(ConfigService.urls.insurer.addField, data);
    }

    function _deleteField(data) {
        return $http.post(ConfigService.urls.insurer.deleteField + data);
    }

    function _editGroup(data) {
        return $http.post(ConfigService.urls.insurer.editGroup, data);
    }

    _.extend(InsurersService, {
        create: _create,
        getInsurer: _getInsurer,
        save: _saveInsurer,
        getList: _getList,
        delete: _delete,
        getGroups: _getGroups,
        getGroup: _getGroup,
        addGroup: _addGroup,
        editGroup: _editGroup,
        addField: _addField,
        deleteField: _deleteField
    })

    return InsurersService;

}]);