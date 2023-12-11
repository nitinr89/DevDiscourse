﻿/*!
 * ASP.NET SignalR JavaScript Library v2.2.2
 * http://signalr.net/
 *
 * Copyright (c) .NET Foundation. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 *
 */

debugger;
(function ($, window, undefined) {
    /// <param name="$" type="jQuery" />
    "use strict";
    debugger;
    if (typeof ($.signalR) !== "function") {
        throw new Error("SignalR: SignalR is not loaded. Please ensure jquery.signalR-x.js is referenced before ~/signalr/js.");
    }

    var signalR = $.signalR;

    function makeProxyCallback(hub, callback) {
        return function () {
            // Call the client hub method
            callback.apply(hub, $.makeArray(arguments));
        };
    }

    function registerHubProxies(instance, shouldSubscribe) {
        var key, hub, memberKey, memberValue, subscriptionMethod;
        debugger;
        for (key in instance) {
            if (instance.hasOwnProperty(key)) {
                hub = instance[key];

                if (!(hub.hubName)) {
                    // Not a client hub
                    continue;
                }

                if (shouldSubscribe) {
                    // We want to subscribe to the hub events
                    subscriptionMethod = hub.on;
                } else {
                    // We want to unsubscribe from the hub events
                    subscriptionMethod = hub.off;
                }

                // Loop through all members on the hub and find client hub functions to subscribe/unsubscribe
                for (memberKey in hub.client) {
                    if (hub.client.hasOwnProperty(memberKey)) {
                        memberValue = hub.client[memberKey];

                        if (!$.isFunction(memberValue)) {
                            // Not a client hub function
                            continue;
                        }

                        subscriptionMethod.call(hub, memberKey, makeProxyCallback(hub, memberValue));
                    }
                }
            }
        }
    }

    debugger;
    $.hubConnection.prototype.createHubProxies = function () {
        var proxies = {};
        debugger;
        this.starting(function () {
            // Register the hub proxies as subscribed
            // (instance, shouldSubscribe)
            registerHubProxies(proxies, true);

            this._registerSubscribedHubs();
        }).disconnected(function () {
            // Unsubscribe all hub proxies when we "disconnect".  This is to ensure that we do not re-add functional call backs.
            // (instance, shouldSubscribe)
            registerHubProxies(proxies, false);
        });

        proxies['chatHub'] = this.createHubProxy('chatHub');
        proxies['chatHub'].client = {};
        proxies['chatHub'].server = {
            send: function (name, message) {
                return proxies['chatHub'].invoke.apply(proxies['chatHub'], $.merge(["Send"], $.makeArray(arguments)));
            },

            sendNotification: function (title, description, url) {
                return proxies['chatHub'].invoke.apply(proxies['chatHub'], $.merge(["SendNotification"], $.makeArray(arguments)));
            },

            updateMap: function (url) {
                debugger;
                return proxies['chatHub'].invoke.apply(proxies['chatHub'], $.merge(["UpdateMap"], $.makeArray(arguments)));
            }
        };

        proxies['fileHub'] = this.createHubProxy('fileHub');
        proxies['fileHub'].client = {};
        proxies['fileHub'].server = {
            send: function (name, message) {
                return proxies['fileHub'].invoke.apply(proxies['fileHub'], $.merge(["Send"], $.makeArray(arguments)));
            },

            sendNotification: function (title, description, url) {
                return proxies['fileHub'].invoke.apply(proxies['fileHub'], $.merge(["SendNotification"], $.makeArray(arguments)));
            },

            sendProgresss: function (signalConnectionId, progress) {
                return proxies['fileHub'].invoke.apply(proxies['fileHub'], $.merge(["SendProgresss"], $.makeArray(arguments)));
            },

            updateMap: function (url) {
                return proxies['fileHub'].invoke.apply(proxies['fileHub'], $.merge(["UpdateMap"], $.makeArray(arguments)));
            }
        };

        return proxies;
    };
    debugger;
    signalR.hub = $.hubConnection("/signalr", { useDefaultPath: false });
    $.extend(signalR, signalR.hub.createHubProxies());

}(window.jQuery, window));