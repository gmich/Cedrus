using System;
using System.Collections.Generic;

namespace Gmich.Cedrus
{

    public class SubscriptionEventArgs<TSubscription> : EventArgs
    {
        public SubscriptionEventArgs(Identity identity, TSubscription subscription)
        {
            Subscription = subscription;
            Identity = identity;
        }

        public Identity Identity { get; }
        public TSubscription Subscription { get; }
    }

    public class SubscriberBase<TSubscription>
    {
        protected readonly Dictionary<Identity, TSubscription> subscribed = new Dictionary<Identity, TSubscription>();

        public EventHandler<SubscriptionEventArgs<TSubscription>> SubscriptionAdded { get; set; }
        public EventHandler<SubscriptionEventArgs<TSubscription>> SubscriptionRemoved { get; set; }

        public Result<IDisposable> Subscribe(Identity identity, TSubscription subscription)
        {
            if (subscribed.ContainsKey(identity))
            {
                return Result.FailWith<IDisposable>(State.Forbidden, $"Subscriber already contains subscription {identity.Id}.");
            }
            subscribed.Add(identity, subscription);
            SubscriptionAdded?.Invoke(this, new SubscriptionEventArgs<TSubscription>(identity, subscription));

            return Result.Ok(Disposable.For(() =>
            {
                if (subscribed.ContainsKey(identity))
                {
                    subscribed.Remove(identity);
                    SubscriptionRemoved?.Invoke(this, new SubscriptionEventArgs<TSubscription>(identity, subscription));
                }
            }));
        }
    }
}
