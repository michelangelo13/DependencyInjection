// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.DependencyInjection.Abstractions;
using Microsoft.Framework.Internal;

namespace Microsoft.Framework.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a sequence of <see cref="ServiceDescriptor"/> to the <paramref name="collection"/>.
        /// </summary>
        /// <param name="descriptor">The <see cref="IEnumerable{T}"/> of <see cref="ServiceDescriptor"/>s to add.</param>
        /// <returns>A reference to the current instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection Add([NotNull] this IServiceCollection collection,
                                             [NotNull] IEnumerable<ServiceDescriptor> descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                collection.Add(descriptor);
            }

            return collection;
        }

        /// <summary>
        /// Adds the specified <paramref name="descriptor"/> to the <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/>.</param>
        /// <returns>A reference to the current instance of <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection Add([NotNull] this IServiceCollection collection,
                                             [NotNull] ServiceDescriptor descriptor)
        {
            collection.Add(descriptor);
            return collection;
        }

        /// <summary>
        /// Adds the specified <paramref name="descriptor"/> to the <paramref name="collection"/> if the
        /// service type hasn't been already registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/>.</param>
        /// <returns><c>true</c> if the <paramref name="descriptor"/> was added; otherwise <c>false</c>.</returns>
        public static bool TryAdd([NotNull] this IServiceCollection collection,
                                  [NotNull] ServiceDescriptor descriptor)
        {
            if (!collection.Any(d => d.ServiceType == descriptor.ServiceType))
            {
                collection.Add(descriptor);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the specified <paramref name="descriptor"/>s to the <paramref name="collection"/> if the
        /// service type hasn't been already registered.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/>s.</param>
        /// <returns><c>true</c> if any of the <paramref name="descriptor"/>s was added; otherwise <c>false</c>.</returns>
        public static bool TryAdd([NotNull] this IServiceCollection collection,
                                  [NotNull] IEnumerable<ServiceDescriptor> descriptors)
        {
            var anyAdded = false;
            foreach (var d in descriptors)
            {
                anyAdded = collection.TryAdd(d) || anyAdded;
            }

            return anyAdded;
        }

        public static bool TryAddTransient(
            [NotNull] this IServiceCollection collection,
            [NotNull] Type service)
        {
            var descriptor = ServiceDescriptor.Transient(service, service);
            return TryAdd(collection, descriptor);
        }

        public static bool TryAddTransient(
            [NotNull] this IServiceCollection collection,
            [NotNull] Type service,
            [NotNull] Type implementationType)
        {
            var descriptor = ServiceDescriptor.Transient(service, implementationType);
            return TryAdd(collection, descriptor);
        }

        public static bool TryAddTransient(
            [NotNull] this IServiceCollection collection,
            [NotNull] Type service,
            [NotNull] Func<IServiceProvider, object> implementationFactory)
        {
            var descriptor = ServiceDescriptor.Transient(service, implementationFactory);
            return TryAdd(collection, descriptor);
        }

        public static bool TryAddTransient<TService>([NotNull] this IServiceCollection collection)
        {
            return TryAddTransient(collection, typeof(TService), typeof(TService));
        }

        public static bool TryAddTransient<TService, TImplementation>([NotNull] this IServiceCollection collection)
        {
            return TryAddTransient(collection, typeof(TService), typeof(TImplementation));
        }

        public static bool TryAddScoped(
            [NotNull] this IServiceCollection collection,
            [NotNull] Type service)
        {
            var descriptor = ServiceDescriptor.Scoped(service, service);
            return TryAdd(collection, descriptor);
        }

        public static bool TryAddScoped(
            [NotNull] this IServiceCollection collection,
            [NotNull] Type service,
            [NotNull] Type implementationType)
        {
            var descriptor = ServiceDescriptor.Scoped(service, implementationType);
            return TryAdd(collection, descriptor);
        }

        public static bool TryAddScoped(
            [NotNull] this IServiceCollection collection,
            [NotNull] Type service,
            [NotNull] Func<IServiceProvider, object> implementationFactory)
        {
            var descriptor = ServiceDescriptor.Scoped(service, implementationFactory);
            return TryAdd(collection, descriptor);
        }

        public static bool TryAddScoped<TService>([NotNull] this IServiceCollection collection)
        {
            return TryAddScoped(collection, typeof(TService), typeof(TService));
        }

        public static bool TryAddScoped<TService, TImplementation>([NotNull] this IServiceCollection collection)
        {
            return TryAddScoped(collection, typeof(TService), typeof(TImplementation));
        }

        public static bool TryAddSingleton(
            [NotNull] this IServiceCollection collection,
            [NotNull] Type service)
        {
            var descriptor = ServiceDescriptor.Singleton(service, service);
            return TryAdd(collection, descriptor);
        }

        public static bool TryAddSingleton(
            [NotNull] this IServiceCollection collection,
            [NotNull] Type service,
            [NotNull] Type implementationType)
        {
            var descriptor = ServiceDescriptor.Singleton(service, implementationType);
            return TryAdd(collection, descriptor);
        }

        public static bool TryAddSingleton(
            [NotNull] this IServiceCollection collection,
            [NotNull] Type service,
            [NotNull] Func<IServiceProvider, object> implementationFactory)
        {
            var descriptor = ServiceDescriptor.Singleton(service, implementationFactory);
            return TryAdd(collection, descriptor);
        }

        public static bool TryAddSingleton<TService>([NotNull] this IServiceCollection collection)
        {
            return TryAddSingleton(collection, typeof(TService), typeof(TService));
        }

        public static bool TryAddSingleton<TService, TImplementation>([NotNull] this IServiceCollection collection)
        {
            return TryAddSingleton(collection, typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Adds a <see cref="ServiceDescriptor"/> if an existing descriptor with the same
        /// <see cref="ServiceDescriptor.ServiceType"/> and <see cref="ServiceDescriptor.ImplementationType"/> does
        /// not already exist in <paramref name="services."/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptor">
        /// A <see cref="ServiceDescriptor"/> with <see cref="ServiceDescriptor.ImplementationType"/> set to a
        /// non-<c>null</c> value.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="descriptor"/> is added to the collection, otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Use <see cref="TryAddMultiRegistration"/> when registing a service implementation of a service type that
        /// supports multiple registrations of the same service type. Using
        /// <see cref="Add(IServiceCollection, ServiceDescriptor)"/> is not idempotent and can add duplicate
        /// <see cref="ServiceDescriptor"/> instances if called twice. Using
        /// <see cref="TryAddMultiRegistration(IServiceCollection, ServiceDescriptor)"/> will prevent registration
        /// of multiple implementation types.
        /// </remarks>
        public static bool TryAddMultiRegistration(
            [NotNull] this IServiceCollection services,
            [NotNull] ServiceDescriptor descriptor)
        {
            // This can't work when registering a factory or instance, you have to register a type.
            // Additionally, if any existing registrations use a factory or instance, we can't check those, but we don't
            // throw for those, because it might be added by user code and is totally valid.
            if (descriptor.ImplementationType == null)
            {
                var message = Resources.FormatTryAddMultiRegistration_ImplementationTypeMustBeSet(
                    nameof(ServiceDescriptor),
                    nameof(ServiceDescriptor.ImplementationType));
                throw new ArgumentException(message, nameof(descriptor));
            }

            if (services.Any(d =>
                d.ServiceType == descriptor.ServiceType &&
                d.ImplementationType == descriptor.ImplementationType))
            {
                return false;
            }

            services.Add(descriptor);
            return true;
        }

        public static IServiceCollection AddTransient([NotNull] this IServiceCollection collection,
                                                      [NotNull] Type service,
                                                      [NotNull] Type implementationType)
        {
            return Add(collection, service, implementationType, ServiceLifetime.Transient);
        }

        public static IServiceCollection AddTransient([NotNull] this IServiceCollection collection,
                                                      [NotNull] Type service,
                                                      [NotNull] Func<IServiceProvider, object> implementationFactory)
        {
            return Add(collection, service, implementationFactory, ServiceLifetime.Transient);
        }

        public static IServiceCollection AddScoped([NotNull] this IServiceCollection collection,
                                                   [NotNull] Type service,
                                                   [NotNull] Type implementationType)
        {
            return Add(collection, service, implementationType, ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddScoped([NotNull] this IServiceCollection collection,
                                                   [NotNull] Type service,
                                                   [NotNull] Func<IServiceProvider, object> implementationFactory)
        {
            return Add(collection, service, implementationFactory, ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddSingleton([NotNull] this IServiceCollection collection,
                                                      [NotNull] Type service,
                                                      [NotNull] Type implementationType)
        {
            return Add(collection, service, implementationType, ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddSingleton([NotNull] this IServiceCollection collection,
                                                      [NotNull] Type service,
                                                      [NotNull] Func<IServiceProvider, object> implementationFactory)
        {
            return Add(collection, service, implementationFactory, ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddInstance([NotNull] this IServiceCollection collection,
                                                     [NotNull] Type service,
                                                     [NotNull] object implementationInstance)
        {
            var serviceDescriptor = new ServiceDescriptor(service, implementationInstance);
            collection.Add(serviceDescriptor);
            return collection;
        }

        public static IServiceCollection AddTransient<TService, TImplementation>([NotNull] this IServiceCollection services)
            where TImplementation : TService
        {
            return services.AddTransient(typeof(TService), typeof(TImplementation));
        }

        public static IServiceCollection AddTransient([NotNull] this IServiceCollection services,
                                                      [NotNull] Type serviceType)
        {
            return services.AddTransient(serviceType, serviceType);
        }

        public static IServiceCollection AddTransient<TService>([NotNull] this IServiceCollection services)
        {
            return services.AddTransient(typeof(TService));
        }

        public static IServiceCollection AddTransient<TService>([NotNull] this IServiceCollection services,
                                                                [NotNull] Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            return services.AddTransient(typeof(TService), implementationFactory);
        }

        public static IServiceCollection AddScoped<TService, TImplementation>([NotNull] this IServiceCollection services)
            where TImplementation : TService
        {
            return services.AddScoped(typeof(TService), typeof(TImplementation));
        }

        public static IServiceCollection AddScoped([NotNull] this IServiceCollection services,
                                                   [NotNull] Type serviceType)
        {
            return services.AddScoped(serviceType, serviceType);
        }

        public static IServiceCollection AddScoped<TService>([NotNull] this IServiceCollection services,
                                                             [NotNull] Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            return services.AddScoped(typeof(TService), implementationFactory);
        }

        public static IServiceCollection AddScoped<TService>([NotNull] this IServiceCollection services)
        {
            return services.AddScoped(typeof(TService));
        }

        public static IServiceCollection AddSingleton<TService, TImplementation>([NotNull] this IServiceCollection services)
        {
            return services.AddSingleton(typeof(TService), typeof(TImplementation));
        }

        public static IServiceCollection AddSingleton([NotNull] this IServiceCollection services,
                                                      [NotNull] Type serviceType)
        {
            return services.AddSingleton(serviceType, serviceType);
        }

        public static IServiceCollection AddSingleton<TService>([NotNull] this IServiceCollection services)
        {
            return services.AddSingleton(typeof(TService));
        }

        public static IServiceCollection AddSingleton<TService>([NotNull] this IServiceCollection services,
                                                                [NotNull] Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            return services.AddSingleton(typeof(TService), implementationFactory);
        }

        public static IServiceCollection AddInstance<TService>([NotNull] this IServiceCollection services,
                                                               [NotNull] TService implementationInstance)
            where TService : class
        {
            return services.AddInstance(typeof(TService), implementationInstance);
        }

        /// <summary>
        /// Removes the first service in <see cref="IServiceCollection"/> with the same service type
        /// as <paramref name="descriptor"/> and adds <paramef name="descriptor"/> to the collection.
        /// </summary>
        /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="descriptor">The <see cref="ServiceDescriptor"/> to replace with.</param>
        /// <returns></returns>
        public static IServiceCollection Replace([NotNull] this IServiceCollection collection,
                                                 [NotNull] ServiceDescriptor descriptor)
        {
            var registeredServiceDescriptor = collection.FirstOrDefault(s => s.ServiceType == descriptor.ServiceType);
            if (registeredServiceDescriptor != null)
            {
                collection.Remove(registeredServiceDescriptor);
            }

            collection.Add(descriptor);
            return collection;
        }

        private static IServiceCollection Add(IServiceCollection collection,
                                              Type service,
                                              Type implementationType,
                                              ServiceLifetime lifeCycle)
        {
            var descriptor = new ServiceDescriptor(service, implementationType, lifeCycle);
            collection.Add(descriptor);
            return collection;
        }

        private static IServiceCollection Add(IServiceCollection collection,
                                              Type service,
                                              Func<IServiceProvider, object> implementationFactory,
                                              ServiceLifetime lifeCycle)
        {
            var descriptor = new ServiceDescriptor(service, implementationFactory, lifeCycle);
            collection.Add(descriptor);
            return collection;
        }
    }
}
