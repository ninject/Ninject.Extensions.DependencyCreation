//-------------------------------------------------------------------------------
// <copyright file="DependencyCreationTest.cs" company="bbv Software Services AG">
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

namespace Ninject.Extensions.DependencyCreation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Ninject.Activation.Caching;
    using Ninject.Extensions.ContextPreservation;
    using Ninject.Extensions.DependencyCreation.Fakes;
    using Xunit;
    
    /// <summary>
    /// Integration Test for Dependency Creation Module.
    /// </summary>
    public class DependencyCreationTest
    {
        /// <summary>
        /// The kernel used in the tests.
        /// </summary>
        private IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyCreationTest"/> class.
        /// </summary>
        public DependencyCreationTest()
        {
            this.kernel = new StandardKernel();
#if SILVERLIGHT
            this.kernel.Load(new ContextPreservationModule());
            this.kernel.Load(new DependencyCreationModule());
#endif
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DependencyCreationTest"/> class.
        /// </summary>
        ~DependencyCreationTest()
        {
            this.kernel.Dispose();
        }

        /// <summary>
        /// Dependencies are disposed with their parent.
        /// </summary>
        [Fact]
        public void DependencyDisposedWithParent()
        {
            this.kernel.DefineDependency<Parent, Dependency>();

            IList<Dependency> dependencies = new List<Dependency>();
            this.kernel
                .Bind<Dependency>()
                .ToMethod(ctx => { dependencies.Add(new Dependency()); return dependencies.Last(); })
                .InDependencyCreatorScope();

            var parent1 = this.kernel.Get<Parent>();
            var parent2 = this.kernel.Get<Parent>();
            dependencies[0].IsDisposed.Should().BeFalse();
            dependencies[1].IsDisposed.Should().BeFalse();

            parent2.Dispose();
            dependencies[0].IsDisposed.Should().BeFalse();
            dependencies[1].IsDisposed.Should().BeTrue();

            parent1.Dispose();
            dependencies[0].IsDisposed.Should().BeTrue();
        }
    
        /// <summary>
        /// Dependencies are disposed with their parent.
        /// </summary>
        [Fact]
        public void DependencyDisposedWhenParentIsCollected()
        {
            this.kernel.DefineDependency<Parent, Dependency>();

            IList<Dependency> dependencies = new List<Dependency>();
            this.kernel
                .Bind<Dependency>().ToSelf()
                .InDependencyCreatorScope()
                .OnActivation(dependencies.Add);

            this.kernel.Get<Parent>();
            GC.Collect();
            this.kernel.Components.Get<ICache>().Prune();

            dependencies[0].IsDisposed.Should().BeTrue();
        }

        /// <summary>
        /// The dependencies are created even if they are defined by a interface
        /// of the actually created object. 
        /// </summary>
        [Fact]
        public void ParentResolvableByInterface()
        {
            this.kernel.DefineDependency<IParent, Dependency>();

            bool dependencyCreated = false;
            this.kernel
                .Bind<Dependency>().ToSelf()
                .InDependencyCreatorScope()
                .OnActivation(instance => dependencyCreated = true);

            this.kernel.Get<Parent>();

            dependencyCreated.Should().BeTrue();
        }

        /// <summary>
        /// Multiple dependencies can be created for an object.
        /// </summary>
        [Fact]
        public void MultipleDependenciesCanBeCreated()
        {
            this.kernel.DefineDependency<IParent, Dependency>();
            this.kernel.DefineDependency<IParent, Dependency2>();

            Dependency dependency1 = null;
            Dependency2 dependency2 = null;
            this.kernel
                .Bind<Dependency>().ToSelf()
                .InDependencyCreatorScope()
                .OnActivation(instance => dependency1 = instance);
            this.kernel
                .Bind<Dependency2>().ToSelf()
                .InDependencyCreatorScope()
                .OnActivation(instance => dependency2 = instance);

            var parent = this.kernel.Get<Parent>();
            parent.Dispose();

            dependency1.Should().NotBeNull();
            dependency2.Should().NotBeNull();
            dependency1.IsDisposed.Should().BeTrue();
            dependency2.IsDisposed.Should().BeTrue();
        }

        /// <summary>
        /// A dependency can have its own dependency.
        /// </summary>
        [Fact]
        public void NestedDependenciesCanBeCreated()
        {
            this.kernel.DefineDependency<IParent, Dependency>();
            this.kernel.DefineDependency<Dependency, Dependency2>();

            Dependency dependency1 = null;
            Dependency2 dependency2 = null;
            this.kernel
                .Bind<Dependency>().ToSelf()
                .InDependencyCreatorScope()
                .OnActivation(instance => dependency1 = instance);
            this.kernel
                .Bind<Dependency2>().ToSelf()
                .InDependencyCreatorScope()
                .OnActivation(instance => dependency2 = instance);

            var parent = this.kernel.Get<Parent>();
            parent.Dispose();

            dependency1.Should().NotBeNull();
            dependency2.Should().NotBeNull();
            dependency1.IsDisposed.Should().BeTrue();
            dependency2.IsDisposed.Should().BeTrue();
        }
    }
}