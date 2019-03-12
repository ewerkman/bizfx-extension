// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopulateCartLineItemsActionBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.BizFx.Carts.Pipelines.Blocks
{
    using Plugin.BizFx.Carts.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
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
    public class PopulateCartLineItemsActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
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
        public PopulateCartLineItemsActionsBlock(CommerceCommander commander)
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

            var cartViewsPolicy = context.GetPolicy<KnownCartViewsPolicy>();
            var cartActionsPolicy = context.GetPolicy<KnownCartActionsPolicy>();

            if (!entityView.Name.Equals(cartViewsPolicy.CartLinesView, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(entityView);
            }

            var actionPolicy = entityView.GetPolicy<ActionsPolicy>();
            actionPolicy.Actions.Add(
             new EntityActionView
             {
                 Name = cartActionsPolicy.DeleteCartLine,
                 DisplayName = "Delete Cart Line",
                 Description = "Removes the selected cart line",
                 IsEnabled = true,
                 EntityView = string.Empty,
                 Icon = "delete",
                 RequiresConfirmation = true,
                 ConfirmationMessage = "Are you sure you want to delete this cart line?"
             });

            actionPolicy.Actions.Add(new EntityActionView {
                Name = cartActionsPolicy.ChangeLineQuantity,
                DisplayName = "Change Line Quantity",
                IsEnabled = true,
                EntityView = cartViewsPolicy.CartLineChangeQuantityView,
                Icon = "add"
            });

            return Task.FromResult(entityView);
        }
    }
}