// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCartTotalsBlock.cs" company="Sitecore Corporation">
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
    public class GetCartTotalsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
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
        public GetCartTotalsBlock(CommerceCommander commander)
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

            EntityView totalsView = new EntityView();
            totalsView.EntityId = cart.Id;
            totalsView.Name = "Cart Totals";
            totalsView.UiHint = "Table";

            entityView.ChildViews.Add(totalsView);

            var totalView = new EntityView();
            totalView.EntityId = cart.Id;
            totalView.Name = "TotalDetails";

            totalsView.ChildViews.Add(totalView);

            totalView.Properties.AddProperty("SubTotal", cart.Totals.SubTotal);
            totalView.Properties.AddProperty("AdjustmentsTotal", cart.Totals.AdjustmentsTotal);
            totalView.Properties.AddProperty("PaymentsTotal", cart.Totals.PaymentsTotal);
            totalView.Properties.AddProperty("GrandTotal", cart.Totals.GrandTotal);

            return Task.FromResult(entityView);
        }
    }
}