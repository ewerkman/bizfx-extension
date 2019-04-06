// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetFeaturesViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Pipelines.Blocks
{
    using Plugin.Sample.Notes.Components;
    using Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures;
    using Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures.Arguments;
    using Plugin.Sample.Notes.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Collections.Generic;
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
        public async override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
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
                return entityView;
            }

            // Only proceed if the current entity is a sellable item
            if (!(request.Entity is SellableItem))
            {
                return entityView;
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
                    ItemId = variationId,
                    EntityVersion = entityView.EntityVersion
                };

                entityView.ChildViews.Add(view);

                targetView = view;
            }

            var component = sellableItem.GetComponent<FeaturesComponent>(variationId);

            var selectizeConfig = await GetSelectizeConfiguration(context);

            targetView.Properties.Add(new ViewProperty
            {
                Name = nameof(FeaturesComponent.FeatureList),
                RawValue = component.FeatureList?.ToArray<string>(),
                IsReadOnly = !isEditView,
                IsRequired = true,
                UiType = isEditView ? "Selectize" : "Tags",
                Policies = new List<Policy> { selectizeConfig },
                OriginalType = "List"
            });

            return entityView;
        }

        private async Task<SelectizeConfigPolicy> GetSelectizeConfiguration(CommercePipelineExecutionContext context)
        {
            var availableSelectionsPolicy = new SelectizeConfigPolicy();

            availableSelectionsPolicy.Options = await TryGetAvailableFeatures(context);
            availableSelectionsPolicy.Placeholder = await TryLocalize("Placeholder", context);

            return availableSelectionsPolicy;
        }

        private async Task<List<Selection>> TryGetAvailableFeatures(CommercePipelineExecutionContext context)
        {
            var localizedOptions = await this.Commander.Pipeline<IGetLocalizedProductFeaturesPipeline>().Run(new LocalizedProductFeaturesArgument(), context);
            return localizedOptions?.Select(term => new Selection() { DisplayName = term.Value, Name = term.Key }).ToList() ?? new List<Selection>();
        }

        private async Task<string> TryLocalize(string name, CommercePipelineExecutionContext context)
        {
            var result = await Commander.Pipeline<IGetLocalizedProductFeaturesConfigurationPipeline>().Run(new LocalizedProductFeaturesConfigurationArgument(name, (object[])null), context);
            return result?.Value ?? name;
        }
    }
}