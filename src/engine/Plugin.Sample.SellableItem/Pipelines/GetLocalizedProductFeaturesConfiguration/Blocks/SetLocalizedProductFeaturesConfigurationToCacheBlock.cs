// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetLocalizedProductFeaturesToCacheBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures.Blocks
{
    using Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures.Arguments;
    using Plugin.Sample.Notes.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Framework.Caching;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a pipeline block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Core.PipelineArgument,
    ///         Sitecore.Commerce.Core.PipelineArgument, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("Change to <Project>Constants.Pipelines.Blocks.<Block Name>")]
    public class SetLocalizedProductFeaturesConfigurationToCacheBlock : PipelineBlock<LocalizedTerm, LocalizedTerm, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Gets or sets the commander.
        /// </summary>
        /// <value>
        /// The commander.
        /// </value>
        protected CommerceCommander Commander { get; set; }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public SetLocalizedProductFeaturesConfigurationToCacheBlock(CommerceCommander commander)
            : base(null)
        {
            this.Commander = commander;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
        /// The pipeline argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="PipelineArgument"/>.
        /// </returns>
        public async override Task<LocalizedTerm> Run(LocalizedTerm arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return null;
            }

            var argument = context.CommerceContext.GetObject<LocalizedProductFeaturesConfigurationArgument>();
            if (string.IsNullOrEmpty(argument?.LocalizationKey))
                return arg;

            KeyValuePair<string, bool> keyValuePair = context.CommerceContext.GetObject<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>)(k => k.Key.Equals("IsFromCache", StringComparison.OrdinalIgnoreCase)));

            string currentLanguage = context.CommerceContext.CurrentLanguage();
            var cachePolicy = context.GetPolicy<ProductFeaturesCachePolicy>();

            if (cachePolicy.AllowCaching && !keyValuePair.Value)
            {
                string cacheKey = argument.LocalizationKey + "|" + currentLanguage;
                IGetEnvironmentCachePipeline cachePipeline = this.Commander.Pipeline<IGetEnvironmentCachePipeline>();
                EnvironmentCacheArgument environmentCacheArgument = new EnvironmentCacheArgument();
                environmentCacheArgument.CacheName = cachePolicy.CacheName;
                await(await cachePipeline.Run(environmentCacheArgument, context)).Set(cacheKey, (ICachable)new Cachable<LocalizedTerm>(arg, 1L), cachePolicy.GetCacheEntryOptions());
                cacheKey = (string)null;
            }

            return arg;
        }
    }
}