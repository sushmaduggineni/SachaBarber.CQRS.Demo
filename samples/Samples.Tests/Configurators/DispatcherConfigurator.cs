using System.Collections.Generic;
using System.Reflection;
using CQRSalad.Dispatching;
using CQRSalad.Dispatching.ActionsScanning;
using CQRSalad.Dispatching.Core;
using CQRSalad.Dispatching.Descriptors;
using CQRSalad.Dispatching.HandlersScanning;
using CQRSalad.Dispatching.Priority;
using CQRSalad.Dispatching.ServiceProvider;
using CQRSalad.Dispatching.Subscriptions;
using CQRSalad.Dispatching.TypesScanning;
using CQRSalad.EventSourcing;
using Samples.Domain.Events.User;
using Samples.Tests.Structuremap;
using StructureMap;

namespace Samples.Tests.Configurators
{
    public static class DispatcherConfigurator
    {
        public static IContainer UseAssemblyRuleScanning(this IContainer container)
        {
            Assembly applicationServices = ApplicationServiceGenerator.Generate(typeof(UserAggregate).Assembly);

            var rules = new List<AssemblyScanningRule>
            {
                new AssemblyScanningRule(applicationServices),                                               //for command handling
                new AssemblyScanningRule(typeof(Samples.Domain.Interface._namespace).Assembly),              //scan for commands and queries
                new AssemblyScanningRule(typeof(Samples.Domain.Events._namespace).Assembly)                  //for events
            };
            
            var typeProvider = new AssemblyTypesProvider(rules);
            container.Configure(expression => expression.For<IDispatcherTypesProvider>().Use(typeProvider).Singleton());
            
            var handlersProvider = new DefaultDispatcherHandlersProvider(typeProvider);
            container.Configure(expression => expression.For<IDispatcherHandlersProvider>().Use(handlersProvider).Singleton());

            var handlerActionsProvider = new DefaultDispatcherHandlerActionsProvider();
            container.Configure(expression => expression.For<IDispatcherHandlerActionsProvider>().Use(handlerActionsProvider).Singleton());

            var priorityProvider = new DefaultDispatcherPriorityProvider();

            var subscriptionManager = new DispatcherSubscriptionsManager(
                handlersProvider,
                new DefaultDispatcherHandlerDescriptorsBuilder(priorityProvider),
                new DefaultDispatcherHandlerActionDescriptorsBuilder(handlerActionsProvider, priorityProvider)
            );
            container.Configure(expression => expression.For<DispatcherSubscriptionsManager>().Use(subscriptionManager).Singleton());

            return container;
        }

        public static IContainer UseAsyncDispatcherSingleton(this IContainer container)
        {
            IMessageDispatcher dispatcher = Dispatcher.Create(configuration =>
            {
                configuration.SetServiceProvider(new DefaultDispatcherServiceProvider(new StructureMapServiceProvider(container)));

                var subscriptionManager = container.GetInstance<DispatcherSubscriptionsManager>();
                configuration.SetSubscriptionStore(subscriptionManager.Subscribe());
                
            });

            container.Configure(expression => expression.For<IMessageDispatcher>().Use(dispatcher).Singleton());
            return container;
        }
    }
}