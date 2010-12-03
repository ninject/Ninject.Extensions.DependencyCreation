//-------------------------------------------------------------------------------
// <copyright file="DependencyCreationExtensionMethods.cs" company="bbv Software Services AG">
//   Copyright (c) 2010 bbv Software Services AG
//   Author: Remo Gloor remo.gloor@bbv.ch
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

#if SILVERLIGHT_20 || WINDOWS_PHONE || NETCF_35
    #define NODYNAMICPROXY
#endif

namespace Ninject.Extensions.DependencyCreation
{
    using System;
    using System.Globalization;
    using System.Linq;
#if !NODYNAMICPROXY
    using Castle.DynamicProxy;
#endif
    using Ninject;
    using Ninject.Activation;
    using Ninject.Planning.Strategies;
    using Ninject.Planning.Targets;
    using Ninject.Syntax;

    /// <summary>
    /// Extension methods for the definition that an object has a dependency.
    /// </summary>
    public static class DependencyCreationExtensionMethods
    {
        /// <summary>
        /// Defines that an object has a dependency to another object. 
        /// This other object is created upon activation.
        /// </summary>
        /// <typeparam name="TParent">The type of the parent.</typeparam>
        /// <typeparam name="TDependency">The type of the dependency.</typeparam>
        /// <param name="kernel">The kernel.</param>
        public static void DefineDependency<TParent, TDependency>(this IKernel kernel)
        {
            kernel.Components.GetAll<IPlanningStrategy>().OfType<DependencyCreationPlanningStrategy>().Single()
                .DefineDependency<TParent, TDependency>();
        }

        /// <summary>
        /// Defines that the binding is in the scope of the instance that requested the creation of the dependency.
        /// </summary>
        /// <typeparam name="T">The type of the binding.</typeparam>
        /// <param name="syntax">The syntax on which the scope is defined.</param>
        /// <returns>The syntax to configure more things on the binding.</returns>
        public static IBindingNamedWithOrOnSyntax<T> InDependencyCreatorScope<T>(this IBindingInSyntax<T> syntax)
        {
            return syntax.InScope(ctx => ctx.Parameters.OfType<DependencyCreatorParameter>().Single().Creator);
        }

#if !NODYNAMICPROXY
        /// <summary>
        /// Defines that the instance shall get an instance of the component that requested the creation of the dependency.
        /// </summary>
        /// <typeparam name="T">The type of the binding.</typeparam>
        /// <param name="syntax">The syntax.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The syntax to configure more things on the binding.</returns>
        public static IBindingWithOrOnSyntax<T> WithCreatorAsConstructorArgument<T>(
            this IBindingWithSyntax<T> syntax, 
            string parameterName)
        {
            return syntax.WithConstructorArgument(
                parameterName,
                (ctx, target) => GetCreator(ctx, target));
        }

        /// <summary>
        /// Gets an instance of the creator.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>A proxy to the creator.</returns>
        private static object GetCreator(IContext context, ITarget target)
        {
            object creator = context.Parameters.OfType<DependencyCreatorParameter>().Single().Creator;
            if (!target.Type.IsAssignableFrom(creator.GetType()))
            {   
                throw new ActivationException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Error activating: {0}\n Can not inject creator of type {1} into parameter {2} of type {3}",
                    context.Request.Service,
                    creator.GetType(),
                    target.Name,
                    target.Type));
            }

            return CreateWeakReferenceProxy(creator, target.Type);
        }

        /// <summary>
        /// Creates a proxy that has a weak reference to the creator.
        /// </summary>
        /// <param name="creator">The creator.</param>
        /// <param name="proxyType">Type of the proxy.</param>
        /// <returns>A proxy that has a weak reference to the creator.</returns>
        private static object CreateWeakReferenceProxy(object creator, Type proxyType)
        {
            if (proxyType != null && proxyType.IsInterface)
            {
                return new ProxyGenerator().CreateInterfaceProxyWithoutTarget(proxyType, new WeakReferenceInterceptor(creator));
            }

            return new ProxyGenerator().CreateClassProxy(proxyType ?? creator.GetType(), new WeakReferenceInterceptor(creator));
        }

        /// <summary>
        /// An interceptor for the waek reference proxy.
        /// </summary>
        private class WeakReferenceInterceptor : IInterceptor
        {
            /// <summary>
            /// Weak reference to the object
            /// </summary>
            private readonly WeakReference weakReference;

            /// <summary>
            /// Initializes a new instance of the <see cref="WeakReferenceInterceptor"/> class.
            /// </summary>
            /// <param name="reference">The reference.</param>
            public WeakReferenceInterceptor(object reference)
            {
                this.weakReference = new WeakReference(reference);
            }

            /// <summary>
            /// Intercepts the specified invocation.
            /// </summary>
            /// <param name="invocation">The invocation.</param>
            public void Intercept(IInvocation invocation)
            {
                invocation.ReturnValue = invocation.Method.Invoke(this.weakReference.Target, invocation.Arguments);
            }
        }
#endif
    }
}