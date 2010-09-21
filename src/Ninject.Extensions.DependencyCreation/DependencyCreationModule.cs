//-------------------------------------------------------------------------------
// <copyright file="DependencyCreationModule.cs" company="bbv Software Services AG">
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
    using Ninject.Activation.Strategies;
    using Ninject.Extensions.ContextPreservation;
    using Ninject.Modules;
    using Ninject.Planning.Strategies;

    /// <summary>
    /// Module for dependency creation.
    /// </summary>
    public class DependencyCreationModule : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            if (!this.Kernel.HasModule(typeof(ContextPreservationModule).Name))
            {
                throw new InvalidOperationException("ContextPreservationModule is required");    
            }

            this.Kernel.Components.Add<IPlanningStrategy, DependencyCreationPlanningStrategy>();
            this.Kernel.Components.Add<IActivationStrategy, DependencyCreationActivationStrategy>();
        }
    }
}