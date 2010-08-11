//-------------------------------------------------------------------------------
// <copyright file="DependencyCreationActivationStrategy.cs" company="bbv Software Services AG">
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
    using System.Linq;
    using ContextPreservation;
    using Ninject.Activation;
    using Ninject.Activation.Strategies;

    /// <summary>
    /// Creates dependencies for every object that has a <see cref="DependencyCreationDirective"/> assigned to its
    /// plan.
    /// </summary>
    public class DependencyCreationActivationStrategy : ActivationStrategy
    {
        /// <summary>
        /// Contributes to the activation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public override void Activate(IContext context, InstanceReference reference)
        {
            var dependencyCreationDirectives = context.Plan.GetAll<DependencyCreationDirective>();
            if (!dependencyCreationDirectives.Any())
            {
                return;
            }

            var resolutionRoot = new ContextPreservingResolutionRoot(context);
            foreach (var dependencyCreationDirective in dependencyCreationDirectives)
            {
                resolutionRoot.Get(dependencyCreationDirective.DependencyType);
            }
        }
    }
}