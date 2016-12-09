'use strict';

var DashboardApp = angular.module('DashboardApp', [
    'ui.router',
    'ngAnimate',
    'angular-button-spinner',
    'ui.checkbox',
    'anim-in-out',
    'ui.bootstrap',
    'ui.select',
    'ngMask',
    'ngCookies',
    'ngFileUpload',
    'ngDialog'
]);

DashboardApp.config(['$stateProvider', '$httpProvider', '$urlRouterProvider', 'uibDatepickerConfig', function ($stateProvider, $httpProvider, $urlRouterProvider, uibDatepickerConfig) {

    $stateProvider
        .state('portfolioList', {
            url: '/portfolio',
            templateUrl: '/Angular/Dashboard/templates/portfolio/portfolio.html',
            controller: 'PortfolioController'
        })
        .state('portfolioListPager', {
            url: '/portfolio/page/:page?q&groupName&status',
            templateUrl: '/Angular/Dashboard/templates/portfolio/portfolio.html',
            controller: 'PortfolioController'
        })
        .state('profile', {
            url: '/user/profile',
            templateUrl: '/Angular/Dashboard/templates/user/userProfile.html',
            controller: 'UserProfileController'
        })
        .state('changePassword', {
            url: '/user/change-password',
            templateUrl: '/Angular/Dashboard/templates/user/changePassword.html',
            controller: 'ChangePasswordController'
        })
        .state('profilesList', {
            url: '/customer-list?q',
            templateUrl: '/Angular/Dashboard/templates/profiles/profilesList.html',
            controller: 'ProfileListController'
        })
        .state('profilesListPager', {
            url: '/customer-list/page/:page?q',
            templateUrl: '/Angular/Dashboard/templates/profiles/profilesList.html',
            controller: 'ProfileListController'
        })
        .state('editProfile', {
            url: '/customer-profile/:userId',
            templateUrl: '/Angular/Dashboard/templates/profiles/editProfile.html',
            controller: 'EditProfileController'
        })
        .state('policyCreate', {
            url: '/policy/create',
            templateUrl: '/Angular/Dashboard/templates/policy/policyCreate.html',
            controller: 'PolicyCreateController'
        })
        .state('policyDetails', {
            url: '/policy/:id',
            templateUrl: '/Angular/Dashboard/templates/policy/policyDetails.html',
            controller: 'PolicyDetailsController'
        })
        .state('policies', {
            url: '/policies',
            templateUrl: '/Angular/Dashboard/templates/policy/policyList.html',
            controller: 'PolicyListController'
        })
        .state('policiesPager', {
            url: '/policies/page/:page?q&sortType',
            templateUrl: '/Angular/Dashboard/templates/policy/policyList.html',
            controller: 'PolicyListController'
        })
        .state('policyViewUpload', {
            url: '/policy/view-upload/:id',
            templateUrl: '/Angular/Dashboard/templates/policy/policyViewUpload.html',
            controller: 'PolicyViewUploadController'
        })
        .state('policiesAdmin', {
            url: '/admin/policies',
            templateUrl: '/Angular/Dashboard/templates/policy/policyListAdmin.html',
            controller: 'PolicyListAdminController'
        })
        .state('policiesAdminPager', {
            url: '/admin/policies/page/:page?q&status',
            templateUrl: '/Angular/Dashboard/templates/policy/policyListAdmin.html',
            controller: 'PolicyListAdminController'
        })
        .state('policyUpdateAdmin', {
            url: '/policy/update/:id',
            templateUrl: '/Angular/Dashboard/templates/policy/policyUpdateAdmin.html',
            controller: 'PolicyUpdateAdminController'
        })
        .state('insurers', {
            url: '/admin/insurers',
            templateUrl: '/Angular/Dashboard/templates/insurers/insurersList.html',
            controller: 'InsurersListAdminController'
        })
        .state('insurersListPager', {
            url: '/admin/insurers/page/:page?q',
            templateUrl: '/Angular/Dashboard/templates/insurers/insurersList.html',
            controller: 'InsurersListAdminController'
        })
        .state('editInsurer', {
            url: '/admin/editinsurer/:id',
            templateUrl: '/Angular/Dashboard/templates/insurers/editInsurer.html',
            controller: 'InsurerAdminController'
        })
        .state('addInsurer', {
            url: '/admin/addinsurer',
            templateUrl: '/Angular/Dashboard/templates/insurers/addInsurer.html',
            controller: 'InsurerCreateController'
        })
        .state('editInsurerPolicyType', {
            url: '/admin/insurers/editInsurerPolicyType/:id',
            templateUrl: '/Angular/Dashboard/templates/insurers/editPolicyGroup.html',
            controller: 'InsurerPolicyTypeController'
        })
        .state('addInsurerPolicyType', {
            url: '/admin/insurers/addInsurerPolicyType/:id',
            templateUrl: '/Angular/Dashboard/templates/insurers/addPolicyGroup.html',
            controller: 'InsurerPolicyTypeCreateController'
        })
        .state('notifications', {
            url: '/notifications',
            templateUrl: '/Angular/Dashboard/templates/notifications/notifications.html',
            controller: 'NotificationsController'
        })
        .state('notificationsPager', {
            url: '/notifications/page/:page',
            templateUrl: '/Angular/Dashboard/templates/notifications/notifications.html',
            controller: 'NotificationsController'
        })
        .state('about', {
            url: '/about',
            templateUrl: '/Angular/Dashboard/templates/about/about.html',
        })
    ;

    $urlRouterProvider.when('', '/portfolio');
    
    uibDatepickerConfig.showWeeks = false;
    $httpProvider.interceptors.push('redirectInterceptor');

}]);

DashboardApp.run(['$rootScope', '$cookies', '$location', '$state', '$anchorScroll', '$q', 'UserService', 'NotificationsService', 'ConfigService', function ($rootScope, $cookies, $location, $state, $anchorScroll, $q, UserService, NotificationsService, ConfigService) {
    
    UserService.getProfile().then(function (res) {
        $rootScope.currentUser = res.data;
        $cookies.put('userId', res.data.id);
    });

    NotificationsService.getCounter().then(function (res) {
        $rootScope.countNotification = res.data;
    });

    $rootScope.clearError = function (param, object) {
        if (!_.isObject(object)) return;
        if (param in object) {
            delete object[param];
        }
    }

   $rootScope.$on("$locationChangeSuccess", function(){
        $anchorScroll();
    });

    $rootScope.$on('$locationChangeStart', function (event, toState, toParams, fromState, fromParams, options) {
        var isCompleted;
        UserService.getProfile().then(function (res) {
            $rootScope.currentUser = res.data;
            isCompleted = res.data.isProfileCompleted;

            if (!isCompleted) {
                if ($rootScope.alerts.length > 0) {
                    for (var i = 0; i < $rootScope.alerts.length; i++) {
                        if ($rootScope.alerts[i].msg == 'Please, fill your profile before you start.') {
                            continue;
                        } else {
                            $rootScope.alerts.push({ type: 'danger', msg: 'Please, fill your profile before you start.' });
                        }
                    }
                } else {
                    $rootScope.alerts.push({ type: 'danger', msg: 'Please, fill your profile before you start.' });
                }
                
                $state.go('profile');
                $location.path("/user/profile");
            }
        });        
    });

    $rootScope.paramExists = function (param, object) {
        return (param in object);
    }

    //warning, danger, success, info
    function _closeAlert(index) {
        $rootScope.alerts.splice(index, 1);
    };

    function _logout() {
        $cookies.remove('userId');
        $cookies.remove("isProfileCompleted");
    }

    function _getClass(viewLocation) {
        var active = (viewLocation === $location.path());
        return active;
    }

    function _getPrevNumber() {
        return ($rootScope.activePage - 1) * ConfigService.countPerPage;
    }

    function _checkRole(roles) {
        if (!_.isObject($rootScope.currentUser)) return false;
        _.mixin({
            'findByValues': function(collection, property, values) {
                return _.filter(collection, function(item) {
                return _.contains(values, item[property]);
                });
            }
        });
        var contains = _.findByValues($rootScope.currentUser.roles, "id", roles);
        return contains.length > 0;
    }

    function _getTimeAgo(date) {
        if (Math.abs(moment().diff(date)) < 15000) {
            return 'Just now';
        }
        return moment(date).fromNow();
    }

    _.extend($rootScope, {
        closeAlert: _closeAlert,
        logout: _logout,
        getClass: _getClass,
        checkRole: _checkRole,
        getPrevNumber: _getPrevNumber,
        getTimeAgo: _getTimeAgo,
        alerts: []
    });

}]);

DashboardApp.factory('redirectInterceptor', ['$rootScope', '$location','$q', function ($rootScope, $location, $q) {    
    return {
        response: function (response) {
            var deferred = $q.defer();
            
            if (response.status == 401) {
                location.pathname = '/Account';
            } else if (response.status == 403) {
                console.log(response)
            }

            if (response.status == 200) {
                deferred.resolve(response);
            } else {
                deferred.reject(response)
            }

            return deferred.promise;
        },
        responseError: function (response) {
            if (response.status == 401) {
                location.href = '/Account#/login';
            } else if (response.status == 403) {
                $rootScope.alerts.push({type: 'danger', msg: response.data.Message });
            }            

            return $q.reject(response);
        }
    }
}]);

