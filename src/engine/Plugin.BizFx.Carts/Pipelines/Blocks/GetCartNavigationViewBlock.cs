namespace Plugin.BizFx.Carts.Pipelines.Blocks
{
    using Plugin.BizFx.Carts.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System.Threading.Tasks;

    [PipelineDisplayName(nameof(GetCartNavigationViewBlock))]
    public class GetCartNavigationViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        protected ViewCommander Commander { get; set; }

        public GetCartNavigationViewBlock(ViewCommander commander)
            : base(null)
        {
            this.Commander = commander;
        }

        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument can not be null");

            EntityView cartsView = new EntityView();

            cartsView.Name = context.GetPolicy<KnownCartViewsPolicy>().CartsDashboard;
            cartsView.ItemId = context.GetPolicy<KnownCartViewsPolicy>().CartsDashboard;
            cartsView.Icon = "luggagecart";
            cartsView.DisplayRank = 2;

            entityView.ChildViews.Add((Model)cartsView);

            return Task.FromResult<EntityView>(entityView);
        }
    }
}