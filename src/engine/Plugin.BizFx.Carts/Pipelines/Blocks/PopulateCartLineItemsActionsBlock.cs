namespace Plugin.BizFx.Carts.Pipelines.Blocks
{
    using Plugin.BizFx.Carts.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Threading.Tasks;

    [PipelineDisplayName(nameof(PopulateCartLineItemsActionsBlock))]
    public class PopulateCartLineItemsActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        protected CommerceCommander Commander { get; set; }

        public PopulateCartLineItemsActionsBlock(CommerceCommander commander)
            : base(null)
        {
            this.Commander = commander;
        }

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