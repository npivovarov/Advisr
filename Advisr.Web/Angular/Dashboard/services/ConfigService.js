'use strict';
//Dashboard
angular.module('DashboardApp').factory('ConfigService', function () {
    var ConfigService = {
        urls: {
            user: {
               get: '/api/user/get',
               save: '/api/customer/edit',
               getCustomer: '/api/customer/get',
               changePassword: '/api/user/ChangePassword'
            },
            profile: {
                list: '/api/Customer/List',
                customerLock: '/api/customer/lock/',
                customerUnlock: '/api/customer/unlock/'
            },
            policy: {
                create: '/api/Policy/Create',
                get: '/api/Policy/Get/',
                list: '/api/Policy/List',
                viewAll: '/api/Policy/ViewAll',
                insurerList: '/api/Insurer/list',
                groups: '/api/policy/groups/',
                groupFields: '/api/Policy/GroupFields/Description',
                update: '/api/Policy/Update',
                shortDetails: '/api/policy/ShortDetails'
            },
            file: {
                upload: '/api/file/upload',
                addNewPolicy: '/api/Policy/AddNewFiles'
            },
            insurer: {
                create: '/api/Insurer/Add',
                get: '/api/Insurer/Get/',
                list: '/api/Insurer/List',
                edit: '/api/Insurer/Edit/',
                delete: '/api/Insurer/Delete/',
                getGroups: '/api/Insurer/Groups/',
                getGroup: '/api/Insurer/GetGroup/',
                addGroup: '/api/Insurer/AddGroup/',
                deleteField: '/api/Insurer/DeleteField/',
                editGroup: '/api/Insurer/EditGroup/',
                addField: '/api/Insurer/AddField' 
            },
           portfolio: {
               chartDetails: '/api/Portfolio/PieChartDetails'
            }
        },
        userProfileState: [
            { key: 0, value: 'NSW' },
            { key: 1, value: 'NT' },
            { key: 2, value: 'QLD' },
            { key: 3, value: 'SA' },
            { key: 5, value: 'TAS' },
            { key: 6, value: 'VIC' },
            { key: 7, value: 'WA' },
        ],
        policyPaymentFrequency : [
            { id: 1, value: 'Weekly' },
            { id: 2, value: 'Fortnightly' },
            { id: 3, value: 'Monthly' },
            { id: 4, value: 'Annual' }
        ],
        policyStatus : [
            { key: -1, value: 'All' },
            { key: 0, value: 'Unconfirmed' },
            { key: 1, value: 'Confirmed' },
            { key: 2, value: 'Rejected' },
        ],
        sortType: [
            { key: 0, value: 'All' },
            { key: 1, value: 'Active' },
            { key: 2, value: 'Expired' },
        ],
        countPerPage: 10
    }
    return ConfigService;
});