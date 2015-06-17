// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Testing;
using Microsoft.Framework.DependencyInjection.Tests.Fakes;
using Xunit;

namespace Microsoft.Framework.DependencyInjection
{
    public class ServiceCollectionExtensionTest
    {
        private static readonly Func<IServiceProvider, IFakeService> _factory = _ => new FakeService();
        private static readonly FakeService _instance = new FakeService();

        public static TheoryData AddImplementationTypeData
        {
            get
            {
                var serviceType = typeof(IFakeService);
                var implementationType = typeof(FakeService);
                return new TheoryData<Action<IServiceCollection>, Type, Type, ServiceLifetime>
                {
                    { collection => collection.AddTransient(serviceType, implementationType), serviceType, implementationType, ServiceLifetime.Transient },
                    { collection => collection.AddTransient<IFakeService, FakeService>(), serviceType, implementationType, ServiceLifetime.Transient },
                    { collection => collection.AddTransient<IFakeService>(), serviceType, serviceType, ServiceLifetime.Transient },
                    { collection => collection.AddTransient(implementationType), implementationType, implementationType, ServiceLifetime.Transient },

                    { collection => collection.AddScoped(serviceType, implementationType), serviceType, implementationType, ServiceLifetime.Scoped },
                    { collection => collection.AddScoped<IFakeService, FakeService>(), serviceType, implementationType, ServiceLifetime.Scoped },
                    { collection => collection.AddScoped<IFakeService>(), serviceType, serviceType, ServiceLifetime.Scoped },
                    { collection => collection.AddScoped(implementationType), implementationType, implementationType, ServiceLifetime.Scoped },

                    { collection => collection.AddSingleton(serviceType, implementationType), serviceType, implementationType, ServiceLifetime.Singleton },
                    { collection => collection.AddSingleton<IFakeService, FakeService>(), serviceType, implementationType, ServiceLifetime.Singleton },
                    { collection => collection.AddSingleton<IFakeService>(), serviceType, serviceType, ServiceLifetime.Singleton },
                    { collection => collection.AddSingleton(implementationType), implementationType, implementationType, ServiceLifetime.Singleton },
                };
            }
        }

        [Theory]
        [MemberData(nameof(AddImplementationTypeData))]
        public void AddWithTypeAddsServiceWithRightLifecyle(Action<IServiceCollection> addTypeAction,
                                                            Type expectedServiceType,
                                                            Type expectedImplementationType,
                                                            ServiceLifetime lifeCycle)
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            addTypeAction(collection);

            // Assert
            var descriptor = Assert.Single(collection);
            Assert.Equal(expectedServiceType, descriptor.ServiceType);
            Assert.Equal(expectedImplementationType, descriptor.ImplementationType);
            Assert.Equal(lifeCycle, descriptor.Lifetime);
        }

        public static TheoryData AddImplementationFactoryData
        {
            get
            {
                var serviceType = typeof(IFakeService);

                return new TheoryData<Action<IServiceCollection>, ServiceLifetime>
                {
                    { collection => collection.AddTransient<IFakeService>(_factory), ServiceLifetime.Transient },
                    { collection => collection.AddTransient(serviceType, _factory), ServiceLifetime.Transient },

                    { collection => collection.AddScoped<IFakeService>(_factory), ServiceLifetime.Scoped },
                    { collection => collection.AddScoped(serviceType, _factory), ServiceLifetime.Scoped },

                    { collection => collection.AddSingleton<IFakeService>(_factory), ServiceLifetime.Singleton },
                    { collection => collection.AddSingleton(serviceType, _factory), ServiceLifetime.Singleton },
                };
            }
        }

        [Theory]
        [MemberData(nameof(AddImplementationFactoryData))]
        public void AddWithFactoryAddsServiceWithRightLifecyle(Action<IServiceCollection> addAction,
                                                               ServiceLifetime lifeCycle)
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            addAction(collection);

            // Assert
            var descriptor = Assert.Single(collection);
            Assert.Equal(typeof(IFakeService), descriptor.ServiceType);
            Assert.Same(_factory, descriptor.ImplementationFactory);
            Assert.Equal(lifeCycle, descriptor.Lifetime);
        }

        public static TheoryData AddInstanceData
        {
            get
            {
                return new TheoryData<Action<IServiceCollection>>
                {
                    { collection => collection.AddInstance<IFakeService>(_instance) },
                    { collection => collection.AddInstance(typeof(IFakeService), _instance) },
                };
            }
        }

        [Theory]
        [MemberData(nameof(AddInstanceData))]
        public void AddInstance_AddsWithSingletonLifecycle(Action<IServiceCollection> addAction)
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            addAction(collection);

            // Assert
            var descriptor = Assert.Single(collection);
            Assert.Equal(typeof(IFakeService), descriptor.ServiceType);
            Assert.Same(_instance, descriptor.ImplementationInstance);
            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        }

        [Theory]
        [MemberData(nameof(AddInstanceData))]
        public void TryAddNoOpFailsIfExists(Action<IServiceCollection> addAction)
        {
            // Arrange
            var collection = new ServiceCollection();
            addAction(collection);

            // Act
            var d = ServiceDescriptor.Transient<IFakeService, FakeService>();

            // Assert
            Assert.False(collection.TryAdd(d));
            var descriptor = Assert.Single(collection);
            Assert.Equal(typeof(IFakeService), descriptor.ServiceType);
            Assert.Same(_instance, descriptor.ImplementationInstance);
            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        }

        public static TheoryData TryAddImplementationTypeData
        {
            get
            {
                var serviceType = typeof(IFakeService);
                var implementationType = typeof(FakeService);
                return new TheoryData<Func<IServiceCollection, bool>, Type, Type, ServiceLifetime>
                {
                    { collection => collection.TryAddTransient(serviceType, implementationType), serviceType, implementationType, ServiceLifetime.Transient },
                    { collection => collection.TryAddTransient<IFakeService, FakeService>(), serviceType, implementationType, ServiceLifetime.Transient },
                    { collection => collection.TryAddTransient<IFakeService>(), serviceType, serviceType, ServiceLifetime.Transient },
                    { collection => collection.TryAddTransient(implementationType), implementationType, implementationType, ServiceLifetime.Transient },

                    { collection => collection.TryAddScoped(serviceType, implementationType), serviceType, implementationType, ServiceLifetime.Scoped },
                    { collection => collection.TryAddScoped<IFakeService, FakeService>(), serviceType, implementationType, ServiceLifetime.Scoped },
                    { collection => collection.TryAddScoped<IFakeService>(), serviceType, serviceType, ServiceLifetime.Scoped },
                    { collection => collection.TryAddScoped(implementationType), implementationType, implementationType, ServiceLifetime.Scoped },

                    { collection => collection.TryAddSingleton(serviceType, implementationType), serviceType, implementationType, ServiceLifetime.Singleton },
                    { collection => collection.TryAddSingleton<IFakeService, FakeService>(), serviceType, implementationType, ServiceLifetime.Singleton },
                    { collection => collection.TryAddSingleton<IFakeService>(), serviceType, serviceType, ServiceLifetime.Singleton },
                    { collection => collection.TryAddSingleton(implementationType), implementationType, implementationType, ServiceLifetime.Singleton },
                };
            }
        }

        [Theory]
        [MemberData(nameof(TryAddImplementationTypeData))]
        public void TryAdd_WithType_AddsService(
            Func<IServiceCollection, bool> addAction,
            Type expectedServiceType,
            Type expectedImplementationType,
            ServiceLifetime expectedLifetime)
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            var result = addAction(collection);

            // Assert
            Assert.True(result);

            var descriptor = Assert.Single(collection);
            Assert.Equal(expectedServiceType, descriptor.ServiceType);
            Assert.Same(expectedImplementationType, descriptor.ImplementationType);
            Assert.Equal(expectedLifetime, descriptor.Lifetime);
        }

        [Theory]
        [MemberData(nameof(TryAddImplementationTypeData))]
        public void TryAdd_WithType_DoesNotAddDuplicate(
            Func<IServiceCollection, bool> addAction,
            Type expectedServiceType,
            Type expectedImplementationType,
            ServiceLifetime expectedLifetime)
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.Add(ServiceDescriptor.Transient(expectedServiceType, expectedServiceType));

            // Act
            var result = addAction(collection);

            // Assert
            Assert.False(result);

            var descriptor = Assert.Single(collection);
            Assert.Equal(expectedServiceType, descriptor.ServiceType);
            Assert.Same(expectedServiceType, descriptor.ImplementationType);
            Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        }

        [Fact]
        public void TryAddIfMissingActuallyAdds()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            var d = ServiceDescriptor.Transient<IFakeService, FakeService>();

            // Assert
            Assert.True(collection.TryAdd(d));
            var descriptor = Assert.Single(collection);
            Assert.Equal(typeof(IFakeService), descriptor.ServiceType);
            Assert.Null(descriptor.ImplementationInstance);
            Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        }

        [Fact]
        public void TryAddWithEnumerableReturnsTrueIfAnyAdded()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            var ds = new ServiceDescriptor[] {
                ServiceDescriptor.Transient<IFakeService, FakeService>(),
                ServiceDescriptor.Transient<IFakeService, FakeService>()
            };

            // Assert
            Assert.True(collection.TryAdd(ds));
            var descriptor = Assert.Single(collection);
            Assert.Equal(typeof(IFakeService), descriptor.ServiceType);
            Assert.Null(descriptor.ImplementationInstance);
            Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        }

        [Fact]
        public void TryAddWithEnumerableReturnsFalseIfNoneAdded()
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.AddSingleton<IFakeService, FakeService>();

            // Act
            var ds = new ServiceDescriptor[] {
                ServiceDescriptor.Transient<IFakeService, FakeService>(),
                ServiceDescriptor.Transient<IFakeService, FakeService>()
            };

            // Assert
            Assert.False(collection.TryAdd(ds));
            var descriptor = Assert.Single(collection);
            Assert.Equal(typeof(IFakeService), descriptor.ServiceType);
            Assert.Null(descriptor.ImplementationInstance);
            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        }

        [Fact]
        public void TryAddMultiRegistration_DoesNotAddDuplicate()
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.TryAddMultiRegistration(ServiceDescriptor.Transient<IFakeService, FakeService>());

            // Act
            var result = collection.TryAddMultiRegistration(ServiceDescriptor.Transient<IFakeService, FakeService>());

            // Assert
            Assert.False(result);

            var descriptor = Assert.Single(collection);
            Assert.Equal(typeof(IFakeService), descriptor.ServiceType);
            Assert.Equal(typeof(FakeService), descriptor.ImplementationType);
            Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        }

        [Fact]
        public void TryAddMultiRegistration_AllowsMultiRegistration()
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.TryAddMultiRegistration(ServiceDescriptor.Transient<IFakeService, FakeService>());

            // Act
            var result = collection.TryAddMultiRegistration(ServiceDescriptor.Transient<IFakeService, IFakeService>());

            // Assert
            Assert.True(result);

            Assert.Single(collection, sd => sd.ImplementationType == typeof(FakeService));
            Assert.Single(collection, sd => sd.ImplementationType == typeof(IFakeService));
        }

        [Fact]
        public void TryAddMultiRegistration_AllowsMultiRegistration_WithFactory()
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.Add(ServiceDescriptor.Transient<IFakeService>((_) => new FakeService()));

            // Act
            var result = collection.TryAddMultiRegistration(ServiceDescriptor.Transient<IFakeService, FakeService>());

            // Assert
            Assert.True(result);

            Assert.Single(collection, sd => sd.ImplementationFactory != null);
            Assert.Single(collection, sd => sd.ImplementationType == typeof(FakeService));
        }

        [Fact]
        public void TryAddMultiRegistration_AllowsMultiRegistration_WithInstance()
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.Add(ServiceDescriptor.Instance<IFakeService>(new FakeService()));

            // Act
            var result = collection.TryAddMultiRegistration(ServiceDescriptor.Transient<IFakeService, FakeService>());

            // Assert
            Assert.True(result);

            Assert.Single(collection, sd => sd.ImplementationInstance != null);
            Assert.Single(collection, sd => sd.ImplementationType == typeof(FakeService));
        }

        [Fact]
        public void TryAddMultiRegistration_ThrowsForNonType()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act & Assert
            ExceptionAssert.ThrowsArgument(
                () => collection.TryAddMultiRegistration(ServiceDescriptor.Instance<IFakeService>(new FakeService())),
                "descriptor",
                "The ServiceDescriptor must have the ImplementationType property set to a non-null value.");
        }

        [Fact]
        public void AddSequence_AddsServicesToCollection()
        {
            // Arrange
            var collection = new ServiceCollection();
            var descriptor1 = new ServiceDescriptor(typeof(IFakeService), typeof(FakeService), ServiceLifetime.Transient);
            var descriptor2 = new ServiceDescriptor(typeof(IFakeOuterService), typeof(FakeOuterService), ServiceLifetime.Transient);
            var descriptors = new[] { descriptor1, descriptor2 };

            // Act
            var result = collection.Add(descriptors);

            // Assert
            Assert.Equal(descriptors, collection);
        }

        [Fact]
        public void Replace_AddsServiceIfServiceTypeIsNotRegistered()
        {
            // Arrange
            var collection = new ServiceCollection();
            var descriptor1 = new ServiceDescriptor(typeof(IFakeService), typeof(FakeService), ServiceLifetime.Transient);
            var descriptor2 = new ServiceDescriptor(typeof(IFakeOuterService), typeof(FakeOuterService), ServiceLifetime.Transient);
            collection.Add(descriptor1);

            // Act
            collection.Replace(descriptor2);

            // Assert
            Assert.Equal(new[] { descriptor1, descriptor2 }, collection);
        }

        [Fact]
        public void Replace_ReplacesFirstServiceWithMatchingServiceType()
        {
            // Arrange
            var collection = new ServiceCollection();
            var descriptor1 = new ServiceDescriptor(typeof(IFakeService), typeof(FakeService), ServiceLifetime.Transient);
            var descriptor2 = new ServiceDescriptor(typeof(IFakeService), typeof(FakeService), ServiceLifetime.Transient);
            collection.Add(descriptor1);
            collection.Add(descriptor2);
            var descriptor3 = new ServiceDescriptor(typeof(IFakeService), typeof(FakeService), ServiceLifetime.Singleton);

            // Act
            collection.Replace(descriptor3);

            // Assert
            Assert.Equal(new[] { descriptor2, descriptor3 }, collection);
        }
    }
}