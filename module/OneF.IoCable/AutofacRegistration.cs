#pragma warning disable IDE0073
// This software is part of the Autofac IoC container Copyright © 2015 Autofac Contributors https://autofac.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#pragma warning restore IDE0073

using System;
using System.Reflection;
using Autofac.Builder;
using Autofac.Core.Activators.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OneF;
using OneF.IoCable;

namespace Autofac.Extensions.DependencyInjection;

public static class AutofacRegistration
{
    private static DefaultConstructorFinder DefaultConstructorFinder
    {
        get
        {
            return new();
        }
    }

    public static void Populate(
    this ContainerBuilder builder,
    IServiceCollection services)
    {
        Populate(builder, services, null);
    }

    public static void Populate(
    this ContainerBuilder builder,
     IServiceCollection services,
    object? lifetimeScopeTagForSingletons = null)
    {
        Check.NotNull(services);

        builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().ExternallyOwned();

        var autofacServiceScopeFactory = typeof(AutofacServiceProvider).Assembly
                                                                       .GetType("Autofac.Extensions.DependencyInjection.AutofacServiceScopeFactory");

        if(autofacServiceScopeFactory == null)
        {
            throw OneFExceptionHelper.Wrapper(new Exception("Unable get type of Autofac.Extensions.DependencyInjection.AutofacServiceScopeFactory!"));
        }

        builder.RegisterType(autofacServiceScopeFactory!).As<IServiceScopeFactory>();

        Register(builder, services, lifetimeScopeTagForSingletons);
    }

    private static void Register(
    ContainerBuilder builder,
    IServiceCollection services,
    object? lifetimeScopeTagForSingletons = null)
    {
        foreach(var descriptor in services)
        {
            if(descriptor.ImplementationType != null)
            {
                var serviceTypeInfo = descriptor.ServiceType.GetTypeInfo();
                if(serviceTypeInfo.IsGenericTypeDefinition)
                {
                    builder
                        .RegisterGeneric(descriptor.ImplementationType)
                        .As(descriptor.ServiceType)
                        .ConfigureLifecycle(descriptor.Lifetime, lifetimeScopeTagForSingletons)
                        .FindConstructorsWith(DefaultConstructorFinder)
                        .ConfigureConventions();
                }
                else
                {
                    builder
                        .RegisterType(descriptor.ImplementationType)
                        .As(descriptor.ServiceType)
                        .ConfigureLifecycle(descriptor.Lifetime, lifetimeScopeTagForSingletons)
                        .FindConstructorsWith(DefaultConstructorFinder)
                        .ConfigureConventions();
                }
            }
            else if(descriptor.ImplementationFactory != null)
            {
                var registration = RegistrationBuilder.ForDelegate(
                                                          descriptor.ServiceType, (context, parameters) =>
                                                          {
                                                              var serviceProvider = context.Resolve<IServiceProvider>();
                                                              return descriptor.ImplementationFactory(serviceProvider);
                                                          })
                                                      .ConfigureLifecycle(descriptor.Lifetime, lifetimeScopeTagForSingletons)
                                                      .CreateRegistration();

                builder.RegisterComponent(registration);
            }
            else if(descriptor.ImplementationInstance != null)
            {
                builder
                    .RegisterInstance(descriptor.ImplementationInstance)
                    .As(descriptor.ServiceType)
                    .ConfigureLifecycle(descriptor.Lifetime, null);
            }
        }
    }

    private static IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> ConfigureLifecycle<TActivatorData, TRegistrationStyle>(
    this IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registrationBuilder,
    ServiceLifetime lifecycleKind,
    object? lifetimeScopeTagForSingleton)
    {
        switch(lifecycleKind)
        {
            case ServiceLifetime.Singleton:
                if(lifetimeScopeTagForSingleton == null)
                {
                    registrationBuilder.SingleInstance();
                }
                else
                {
                    registrationBuilder.InstancePerMatchingLifetimeScope(lifetimeScopeTagForSingleton);
                }

                break;

            case ServiceLifetime.Scoped:
                registrationBuilder.InstancePerLifetimeScope();
                break;

            case ServiceLifetime.Transient:
                registrationBuilder.InstancePerDependency();
                break;
        }

        return registrationBuilder;
    }
}
