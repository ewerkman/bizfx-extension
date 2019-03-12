// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCartAdjustmentsViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.BizFx.Carts.Pipelines.Blocks
{
    using Plugin.BizFx.Carts.Extensions;
    using Plugin.BizFx.Carts.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Pricing;
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
    public class GetCartAdjustmentsViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
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
        public GetCartAdjustmentsViewBlock(CommerceCommander commander)
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

            var knownCartViewsPolicy = context.GetPolicy<KnownCartViewsPolicy>();
            EntityViewArgument request = context.CommerceContext.GetObject<EntityViewArgument>();

            if (string.IsNullOrEmpty(request?.ViewName) ||
                !request.ViewName.Equals(knownCartViewsPolicy.Master, StringComparison.OrdinalIgnoreCase) ||
                !(request.Entity is Cart))
            {
                return Task.FromResult(entityView);
            }

            Cart cart = request.Entity as Cart;
            
            EntityView adjustmentsView = new EntityView();
            adjustmentsView.EntityId = cart.Id;
            adjustmentsView.Name = "Adjustments";
            adjustmentsView.UiHint = "Table";
            entityView.ChildViews.Add(adjustmentsView);
        
            foreach (AwardedAdjustment adjustment in (IEnumerable<AwardedAdjustment>)cart.Adjustments)
            {
                this.PopulateAdjustmentChildView(adjustmentsView, adjustment, context);
            }
            
            return Task.FromResult( entityView);
        }

        private void PopulateAdjustmentChildView(EntityView entityView, AwardedAdjustment adjustment, CommercePipelineExecutionContext context)
        {
            EntityView entityView1 = new EntityView();
            entityView1.EntityId = entityView.EntityId;
            entityView1.ItemId = adjustment.Name;
            entityView1.Name = "Adjustment"; // context.GetPolicy<KnownOrderViewsPolicy>().Adjustment;
            EntityView entityView2 = entityView1;
            List<ViewProperty> properties1 = entityView2.Properties;
            ViewProperty viewProperty1 = new ViewProperty();
            viewProperty1.Name = "ItemId";
            viewProperty1.IsHidden = true;
            viewProperty1.IsReadOnly = true;
            viewProperty1.RawValue = (object)adjustment.Name;
            properties1.Add(viewProperty1);

            entityView1.AddProperty("Name", adjustment.DisplayName);

            List<ViewProperty> properties2 = entityView2.Properties;
            ViewProperty viewProperty2 = new ViewProperty();
            viewProperty2.Name = "Type";
            viewProperty2.IsReadOnly = true;
            viewProperty2.RawValue = (object)adjustment.AdjustmentType;
            properties2.Add(viewProperty2);
            List<ViewProperty> properties3 = entityView2.Properties;
            ViewProperty viewProperty3 = new ViewProperty();
            viewProperty3.Name = "Adjustment";
            viewProperty3.IsReadOnly = true;
            viewProperty3.RawValue = (object)adjustment.Adjustment;
            properties3.Add(viewProperty3);
            List<ViewProperty> properties4 = entityView2.Properties;
            ViewProperty viewProperty4 = new ViewProperty();
            viewProperty4.Name = "Taxable";
            viewProperty4.IsReadOnly = true;
            viewProperty4.RawValue = (object)adjustment.IsTaxable;
            properties4.Add(viewProperty4);
            List<ViewProperty> properties5 = entityView2.Properties;
            ViewProperty viewProperty5 = new ViewProperty();
            viewProperty5.Name = "IncludeInGrandTotal";
            viewProperty5.IsReadOnly = true;
            viewProperty5.RawValue = (object)adjustment.IncludeInGrandTotal;
            properties5.Add(viewProperty5);
            List<ViewProperty> properties6 = entityView2.Properties;
            ViewProperty viewProperty6 = new ViewProperty();
            viewProperty6.Name = "AwardingBlock";
            viewProperty6.IsReadOnly = true;
            viewProperty6.RawValue = (object)adjustment.AwardingBlock;
            properties6.Add(viewProperty6);
            entityView.ChildViews.Add((Model)entityView2);
        }
    }
}