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
                shortDetails: '/api/policy/ShortDetails',
                getFilledFields: '/api/Policy/GroupFields/PolicyFields',
                // Coverage
                coveragesDescription: '/api/policy/Coverages/Description',
                getCoverages: '/api/policy/GetCoverages'

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
                getPolicyTypes: '/api/Insurer/PolicyTypes/',
                getPolicyType: '/api/Insurer/PolicyType/',
                addPolicyType: '/api/Insurer/AddPolicyType/',
                getCoverages: '/api/Coverage/List/',
                getCoverage: '/api/Coverage/Get/',
                saveCoverage: '/api/Coverage/Edit/',
                addCoverage: '/api/Coverage/Add',
                assignCoverage: '/api/Coverage/Assign',
                deleteField: '/api/Insurer/DeleteField/',
                editPolicyType: '/api/Insurer/EditPolicyType/',
                addField: '/api/Insurer/AddField',
                saveField: '/api/Insurer/SaveField',
                getField: '/api/Insurer/GetField/'
            },
            portfolio: {
                chartDetails: '/api/Portfolio/PieChartDetails'
            },
            notification: {
                list: '/api/Notification/List',
                get: '/api/Notification/Get/',
                getCounter: '/api/Notification/GetCounter',
                mark: '/api/Notification/MarkAsRead/',
                delete: '/api/Notification/Delete/'
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
        policyTypeStatus: [
            { id: 0, name: 'Active' },
            { id: 1, name: 'Hide' }
        ],
        policyTypeName:[
            { id: 0, name: 'Vehicle' },
            { id: 1, name: 'Home' },
            { id: 2, name: 'Life' }
        ],
        policyGroupName: [
            { id: 0, name: 'Vehicle' },
            { id: 1, name: 'Personal' },
            { id: 2, name: 'Commercial' },
            { id: 3, name: 'Property' }
        ],
        policyTypeFieldTypes: [
            { id: 0, name: 'String' },
            { id: 1, name: 'Int' },
            { id: 2, name: 'Bool' },
            { id: 3, name: 'Float' },
            { id: 4, name: 'List' },
            { id: 5, name: 'Date' }
        ],
        policyType: [
            { key: 0, value: 'Health' },
            { key: 1, value: 'Home' },
            { key: 2, value: 'Motor' },
            { key: 3, value: 'Travel' },
            { key: 4, value: 'Commercial' },
            { key: 5, value: 'Other' }
        ],
        coverageTypes: [
            { id: 0, name: 'Standard Cover' },
            { id: 1, name: 'Optional Cover' },
            { id: 2, name: 'Exclusions Cover' }
        ],
        colors: [
            { id: '#ff0000', name: 'Red' },
            { id: '#ff8000', name: 'Orange' },
            { id: '#ffff00', name: 'Yellow' },
            { id: '#40ff00', name: 'Green' },
            { id: '#0000ff', name: 'Blue' },
            { id: '#00ccff', name: 'Light blue' },
            { id: '#bf00ff', name: 'Purple' }
        ],
        countPerPage: 10
    }
    return ConfigService;
});