using System;

namespace HelpMyStreet.Cache.Models
{
    public class CachedItemWrapper<T>
    {
        public CachedItemWrapper(T content, DateTimeOffset isFreshUntil)
        {
            Content = content;
            IsFreshUntil = isFreshUntil;
        }

        public T Content { get; }

        public DateTimeOffset IsFreshUntil { get; }

    }
}
