//-------------------------------------------------------------------------------
// <copyright file="DependencyCreationPlanningStrategy.cs" company="bbv Software Services AG">
//   Copyright (c) 2008 bbv Software Services AG
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
    using Ninject.Components;
    using Ninject.Planning;
    using Ninject.Planning.Strategies;

    /// <summary>
    /// Adds a <see cref="DependencyCreationDirective"/> to each plan of objects for which a 
    /// dependency is defined.
    /// </summary>
    public class DependencyCreationPlanningStrategy : NinjectComponent, IPlanningStrategy
    {
        /// <summary>
        /// Contains all dependency definitions.
        /// </summary>
        private readonly IDictionary<Type, IList<DependencyCreationDirective>> dependencyDefinitions =
            new Dictionary<Type, IList<DependencyCreationDirective>>();

        /// <summary>
        /// Defines that <typeparamref name="TParent"/> requires an instance of <typeparamref name="TDependency"/>.
        /// Upon activation of <typeparamref name="TParent"/> an instance of <typeparamref name="TDependency"/> 
        /// is created with <typeparamref name="TParent"/> as its scope.
        /// </summary>
        /// <typeparam name="TParent">The type of the parent.</typeparam>
        /// <typeparam name="TDependency">The type of the dependency.</typeparam>
        public void DefineDependency<TParent, TDependency>()
        {
            var parentType = typeof(TParent);
            var dependencyType = typeof(TDependency);

            IList<DependencyCreationDirective> dependencies;
            if (!this.dependencyDefinitions.TryGetValue(parentType, out dependencies))
            {
                dependencies = new List<DependencyCreationDirective>();
                this.dependencyDefinitions[parentType] = dependencies;
            }

            dependencies.Add(new DependencyCreationDirective(dependencyType));
        }

        /// <summary>
        /// Contributes to the specified plan.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        public void Execute(IPlan plan)
        {
            foreach (var activationDependency in
                this.dependencyDefinitions.Where(pair => pair.Key.IsAssignableFrom(plan.Type)).SelectMany(pair => pair.Value))
            {
                plan.Add(activationDependency);
            }
        }
    }
}