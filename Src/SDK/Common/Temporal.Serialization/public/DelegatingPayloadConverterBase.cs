﻿using System;
using System.Collections.Generic;
using Temporal.Util;
using Temporal.Api.Common.V1;

namespace Temporal.Serialization
{
    /// <summary>
    /// A <c>DelegatingPayloadConverterBase</c> subclass is an <c>IPayloadConverter</c> that can process complex data types
    /// while delegating the de-/serialization of substructures to other converters.
    /// </summary>
    public abstract class DelegatingPayloadConverterBase : IPayloadConverter
    {
        private IPayloadConverter _delegateConverters = null;

        protected virtual IPayloadConverter DelegateConvertersContainer
        {
            get { return _delegateConverters; }
        }

        public abstract bool TryDeserialize<T>(Payloads serializedData, out T item);

        public abstract bool TrySerialize<T>(T item, Payloads serializedDataAccumulator);

        public virtual void InitDelegates(IEnumerable<IPayloadConverter> delegateConverters)
        {
            Validate.NotNull(delegateConverters);

            _delegateConverters = (delegateConverters is IPayloadConverter compositeConverter)
                                        ? compositeConverter
                                        : new CompositePayloadConverter(delegateConverters);
        }

        public override bool Equals(object obj)
        {
            return (obj != null)
                        && (obj is DelegatingPayloadConverterBase delegatingPayloadConverter)
                        && Equals(delegatingPayloadConverter);
        }

        public virtual bool Equals(DelegatingPayloadConverterBase obj)
        {
            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            return (obj != null)
                        && this.GetType().Equals(obj.GetType())
                        && DelegateConvertersContainer.Equals(DelegateConvertersContainer);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
