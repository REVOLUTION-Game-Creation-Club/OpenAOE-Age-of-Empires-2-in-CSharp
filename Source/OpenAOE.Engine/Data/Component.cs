﻿using System;

namespace OpenAOE.Engine.Data
{
    /// <summary>
    /// Interface to support type-erasure for components.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// The interface type that this component implements.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Clone this component and return the copy.
        /// </summary>
        /// <returns>A copy of this component.</returns>
        IComponent Clone();

        /// <summary>
        /// Copy the values in this component to another component of the same type.
        /// </summary>
        /// <exception cref="ArgumentException">If <paramref name="component" /> is not of the same type.</exception>
        /// <param name="component">The component to copy to.</param>
        void CopyTo(IComponent component);
    }

    /// <summary>
    /// Interface for a writeable component (for type-erasure).
    /// </summary>
    public interface IWriteableComponent
    {
    }

    public interface IWriteableComponent<T> : IWriteableComponent where T : IComponent
    {
    }

    /// <summary>
    /// Indicates that a component supports async writes (ie can have <code>IEntity.Modify</code> called more than once on
    /// it).
    /// </summary>
    public interface IAsyncComponent
    {
    }

    /// <summary>
    /// Base class for the concrete implementation of a component. Declares a strong-typed CopyTo method that needs to be
    /// implemented
    /// by components.
    /// </summary>
    /// <typeparam name="TThis">The type of the class that is implementing this abstract component.</typeparam>
    /// <typeparam name="TRead">Read-only interface of the component being implemented.</typeparam>
    /// <typeparam name="TWrite">Write-only interface of the component being implemented.</typeparam>
    public abstract class Component<TThis, TRead, TWrite> : IComponent
        where TThis : Component<TThis, TRead, TWrite>, TRead, TWrite, new()
        where TRead : IComponent
        where TWrite : IWriteableComponent
    {
        private static readonly Type ReadComponentType = typeof(TRead);

        public Type Type => ReadComponentType;

        static Component()
        {
            VerifyGenericParameters(typeof(TRead), typeof(TWrite));
        }

        void IComponent.CopyTo(IComponent component)
        {
            if (!(component is TThis))
                throw new ArgumentException("component must be of same type to copy", nameof(component));

            var c = (TThis) component;
            CopyTo(c);
        }

        IComponent IComponent.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Ensures that the TRead and TWrite arguments are not just the interfaces they should be inheriting from.
        /// </summary>
        internal static void VerifyGenericParameters(Type read, Type write)
        {
            if (read == typeof(IComponent))
                throw new NotSupportedException(
                    "TRead must be an interface that inherits from IComponent, but not be IComponent");

            if (write == typeof(IWriteableComponent))
                throw new NotSupportedException(
                    "TWrite must be an interface that inherits from IWriteableComponent, but not be IWriteableComponent");
        }

        public abstract void CopyTo(TThis other);

        public TThis Clone()
        {
            var t = new TThis();
            CopyTo(t);
            return t;
        }
    }
}
