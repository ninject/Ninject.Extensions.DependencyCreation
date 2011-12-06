//-------------------------------------------------------------------------------
// <copyright file="CreatorInjectionIntoDependencyTests.cs" company="bbv Software Services AG">
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

#if !NODYNAMICPROXY
namespace Ninject.Extensions.DependencyCreation
{
    using System;

    using FluentAssertions;

    using Ninject.Activation.Caching;
    using Ninject.Extensions.ContextPreservation;
    using Ninject.Extensions.DependencyCreation.Fakes;
    using Xunit;
    
    /// <summary>
    /// Tests dependencies that get an instance of the object that requested their creation.
    /// </summary>
    public class CreatorInjectionIntoDependencyTests
    {
        /// <summary>
        /// The kernel used in the tests.
        /// </summary>
        private readonly IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatorInjectionIntoDependencyTests"/> class.
        /// </summary>
        public CreatorInjectionIntoDependencyTests()
        {
            this.kernel = new StandardKernel();
#if SILVERLIGHT
            this.kernel.Load(new ContextPreservationModule());
            this.kernel.Load(new DependencyCreationModule());
#endif
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CreatorInjectionIntoDependencyTests"/> class.
        /// </summary>
        ~CreatorInjectionIntoDependencyTests()
        {
            this.kernel.Dispose();
        }

        /// <summary>
        /// Dependencies are disposed with their parent.
        /// </summary>
        [Fact]
        public void DependencyDisposedWithParent()
        {
            this.kernel.DefineDependency<Parent, DependencyWithParentReference>();

            DependencyWithParentReference dependency = null;
            this.kernel
                .Bind<DependencyWithParentReference>().ToSelf()
                .InDependencyCreatorScope()
                .WithCreatorAsConstructorArgument("parent")
                .OnActivation(instance => dependency = instance);

            var parent = this.kernel.Get<Parent>();

            parent.Should().NotBeNull();
            dependency.IsDisposed.Should().BeFalse();

            parent = null;
            GC.Collect();
            this.kernel.Components.Get<ICache>().Prune();

            dependency.IsDisposed.Should().BeTrue();
        }

        /// <summary>
        /// If WithCreatorAsConstructorArgument is configured the dependency gets a proxy to the creator
        /// This proxy weak references the dependency.
        /// </summary>
        [Fact]
        public void DependencyGetsParentProxy()
        {
            this.kernel.DefineDependency<Parent, DependencyWithParentReference>();

            DependencyWithParentReference dependency = null;
            this.kernel
                .Bind<DependencyWithParentReference>().ToSelf()
                .InDependencyCreatorScope()
                .WithCreatorAsConstructorArgument("parent")
                .OnActivation(instance => dependency = instance);

            var parent = this.kernel.Get<Parent>();
            dependency.Parent.Do();

            dependency.Parent.Should().NotBe(parent);
            parent.DoWasInvoked.Should().BeTrue();
        }
    }
}
#endif