// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetFeaturesViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Pipelines.Blocks
{
    using Plugin.Sample.Notes.Components;
    using Plugin.Sample.Notes.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
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
    public class GetFeaturesViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Gets or sets the commander.
        /// </summary>
        /// <value>
        /// The commander.
        /// </value>
        protected ViewCommander Commander { get; set; }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public GetFeaturesViewBlock(ViewCommander commander)
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
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument can not be null");

            var request = this.Commander.CurrentEntityViewArgument(context.CommerceContext);
            var catalogViewsPolicy = context.GetPolicy<KnownCatalogViewsPolicy>();
            var featuresviewsPolicy = context.GetPolicy<KnownFeaturesViewsPolicy>();
            var featuresActionsPolicy = context.GetPolicy<KnownFeaturesActionsPolicy>();
            var isMasterView = request.ViewName.Equals(catalogViewsPolicy.Master, StringComparison.OrdinalIgnoreCase);
            var isFeaturesView = request.ViewName.Equals(featuresviewsPolicy.Features, StringComparison.OrdinalIgnoreCase);
            var isVariationView = request.ViewName.Equals(catalogViewsPolicy.Variant, StringComparison.OrdinalIgnoreCase);
            var isConnectView = entityView.Name.Equals(catalogViewsPolicy.ConnectSellableItem, StringComparison.OrdinalIgnoreCase);

            // Make sure that we target the correct views
            if (!isMasterView && !isConnectView && !isFeaturesView)
            {
                return Task.FromResult(entityView);
            }

            // Only proceed if the current entity is a sellable item
            if (!(request.Entity is SellableItem))
            {
                return Task.FromResult(entityView);
            }

            var sellableItem = (SellableItem)request.Entity;

            // See if we are dealing with the base sellable item or one of its variations.
            var variationId = string.Empty;
            if (isVariationView && !string.IsNullOrEmpty(entityView.ItemId))
            {
                variationId = entityView.ItemId;
            }

            var targetView = entityView;

            // Check if the edit action was requested
            var isEditView = !string.IsNullOrEmpty(entityView.Action) &&
                entityView.Action.Equals(featuresActionsPolicy.EditFeatures, StringComparison.OrdinalIgnoreCase);

            if (!isEditView)
            {
                // Create a new view and add it to the current entity view.
                var view = new EntityView
                {
                    Name = featuresviewsPolicy.Features,
                    DisplayName = featuresviewsPolicy.Features,
                    EntityId = entityView.EntityId,
                    ItemId = variationId
                };

                entityView.ChildViews.Add(view);

                targetView = view;
            }

            var component = sellableItem.GetComponent<FeaturesComponent>(variationId);

            targetView.Properties.Add(new ViewProperty
            {
                Name = nameof(FeaturesComponent.FeatureList),
                RawValue = component.FeatureList,
                IsReadOnly = !isEditView,
                IsRequired = true,
                UiType = "Selectize"
            });

            return Task.FromResult(entityView);
        }
    }
}