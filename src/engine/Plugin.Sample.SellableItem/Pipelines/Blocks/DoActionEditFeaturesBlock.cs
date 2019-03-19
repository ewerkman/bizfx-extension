// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionEditFeaturesBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Pipelines.Blocks
{
    using Plugin.Sample.Notes.Components;
    using Plugin.Sample.Notes.Policies;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

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
    public class DoActionEditFeaturesBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
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
        public DoActionEditFeaturesBlock(CommerceCommander commander)
            : base(null)
        {

            this.Commander = commander;

        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="entityView">
        /// The pipeline argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="PipelineArgument"/>.
        /// </returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument can not be null");

            var featuresActionsPolicy = context.GetPolicy<KnownFeaturesActionsPolicy>();

            // Only proceed if the right action was invoked
            if (string.IsNullOrEmpty(entityView.Action) || !entityView.Action.Equals(featuresActionsPolicy.EditFeatures, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(entityView);
            }

            // Get the sellable item from the context
            var entity = context.CommerceContext.GetObject<SellableItem>(x => x.Id.Equals(entityView.EntityId));
            if (entity == null)
            {
                return Task.FromResult(entityView);
            }

            // Get the notes component from the sellable item or its variation
            var component = entity.GetComponent<FeaturesComponent>(entityView.ItemId);

            // Map entity view properties to component
            var featuresAsString = entityView.Properties.FirstOrDefault(x => x.Name.Equals(nameof(FeaturesComponent.FeatureList), StringComparison.OrdinalIgnoreCase))?.Value;
            component.FeatureList = JsonConvert.DeserializeObject<string[]>(featuresAsString);

            // Persist changes
            this.Commander.PersistEntity(context.CommerceContext, entity);

            return Task.FromResult(entityView);
        }
    }
}