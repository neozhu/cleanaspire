﻿using CleanAspire.PushNotifications.Services.Abstractions;

namespace CleanAspire.PushNotifications.Services.Cosmos;

internal class CosmosDbPushSubscriptionStoreAccessorProvider : IPushSubscriptionStoreAccessorProvider
{
    private readonly IPushSubscriptionStore _pushSubscriptionStore;

    public CosmosDbPushSubscriptionStoreAccessorProvider(IPushSubscriptionStore pushSubscriptionStore)
    {
        _pushSubscriptionStore = pushSubscriptionStore;
    }

    public IPushSubscriptionStoreAccessor GetPushSubscriptionStoreAccessor()
    {
        return new CosmosDbPushSubscriptionStoreAccessor(_pushSubscriptionStore);
    }
}