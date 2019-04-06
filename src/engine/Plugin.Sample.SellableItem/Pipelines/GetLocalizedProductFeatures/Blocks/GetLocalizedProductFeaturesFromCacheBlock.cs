// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetLocalizedProductFeaturesFromCacheBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures.Blocks
{
    using Microsoft.Extensions.Logging;
    using Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures.Arguments;
    using Plugin.Sample.Notes.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
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
    public class GetLocalizedProductFeaturesFromCacheBlock : PipelineBlock<LocalizedProductFeaturesArgument, IEnumerable<LocalizedTerm>, CommercePipelineExecutionContext>
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
        public GetLocalizedProductFeaturesFromCacheBlock(CommerceCommander commander)
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
        public async override Task<IEnumerable<LocalizedTerm>> Run(LocalizedProductFeaturesArgument arg, CommercePipelineExecutionContext context)
        {
            var cachePolicy = context.GetPolicy<ProductFeaturesCachePolicy>();
            if (!cachePolicy.AllowCaching)
            {
                context.CommerceContext.AddUniqueObjectByType((object)new KeyValuePair<string, bool>("IsFromCache", false));
                return null;
            }

            var policy = context.GetPolicy<ProductFeaturesControlPanelPolicy>();

            string cacheKey = policy.AvailableProductFeaturesPath + "|" + context.CommerceContext.CurrentLanguage();
            var cachePipeline = this.Commander.Pipeline<IGetEnvironmentCachePipeline>();

            EnvironmentCacheArgument environmentCacheArgument = new EnvironmentCacheArgument();
            environmentCacheArgument.CacheName = cachePolicy.CacheName;

            var result = (await cachePipeline.Run(environmentCacheArgument, context))?.Get(cacheKey)?.Result as IEnumerable<LocalizedTerm>;
            if (result == null)
            {
                context.CommerceContext.AddUniqueObjectByType((object)new KeyValuePair<string, bool>("IsFromCache", false));
                return null;
            }
            context.CommerceContext.AddUniqueObjectByType((object)new KeyValuePair<string, bool>("IsFromCache", true));
            context.Logger.LogDebug("Mgmt.GetLocMsg." + cacheKey);

            return result;
        }
    }
}