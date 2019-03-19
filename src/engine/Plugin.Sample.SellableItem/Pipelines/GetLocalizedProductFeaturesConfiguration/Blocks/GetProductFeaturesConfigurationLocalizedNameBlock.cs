// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetProductFeaturesLocalizedNameBlockBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures.Blocks
{
    using Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures.Arguments;
    using Plugin.Sample.Notes.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Management;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Linq;
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
    public class GetProductFeaturesConfigurationLocalizedBlock : PipelineBlock<LocalizedTerm, LocalizedTerm, CommercePipelineExecutionContext>
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
        public GetProductFeaturesConfigurationLocalizedBlock(CommerceCommander commander)
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
            if(arg != null)
            {
                return arg; // Return the argument we retrieved from the cache
            }

            LocalizedProductFeaturesConfigurationArgument argument = context.CommerceContext.GetObjects<LocalizedProductFeaturesConfigurationArgument>().FirstOrDefault<LocalizedProductFeaturesConfigurationArgument>();

            if (string.IsNullOrEmpty(argument?.LocalizationKey))
                return arg;

            var policy = context.GetPolicy<ProductFeaturesControlPanelPolicy>();

            string currentLanguage = context.CommerceContext.CurrentLanguage();

            ILocalizableTermsPipeline localizableTermsPipeline = this.Commander.Pipeline<ILocalizableTermsPipeline>();
            LocalizableTermArgument localizableTermArgument = new LocalizableTermArgument(string.Empty, policy.ProductFeaturesNamesPath);
            localizableTermArgument.Language = currentLanguage;
            
            return (await localizableTermsPipeline.Run(localizableTermArgument, context)).FirstOrDefault<LocalizedTerm>(t => t.Key.Equals(argument.LocalizationKey, StringComparison.OrdinalIgnoreCase));  
        }
    }
}