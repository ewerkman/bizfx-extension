namespace Plugin.BizFx.Carts.Pipelines.Blocks
{
    using Plugin.BizFx.Carts.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Threading.Tasks;

    [PipelineDisplayName(nameof(DoActionDeleteCartLineBlock))]
    public class DoActionDeleteCartLineBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        protected CommerceCommander Commander { get; set; }

        public DoActionDeleteCartLineBlock(CommerceCommander commander)
            : base(null)
        {

            this.Commander = commander;

        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument can not be null");

            var knownCartActionsPolicy = context.GetPolicy<KnownCartActionsPolicy>();

            if (string.IsNullOrEmpty(entityView?.Action) ||
                !entityView.Action.Equals(knownCartActionsPolicy.DeleteCartLine, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var updatedCart = await Commander.Command<RemoveCartLineCommand>().Process(context.CommerceContext, entityView.EntityId, entityView.ItemId);

            return entityView;
        }
    }
}