namespace Plugin.Sample.Notes.Pipelines.Blocks
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Threading.Tasks;

    [PipelineDisplayName(nameof(ExtendSellableItemDetailsViewBlock))]
    public class ExtendSellableItemDetailsViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        protected CommerceCommander Commander { get; set; }

        public ExtendSellableItemDetailsViewBlock(CommerceCommander commander)
            : base(null)
        {
            this.Commander = commander;
        }

        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument can not be null");

            var policy = context.GetPolicy<KnownCatalogViewsPolicy>();
            EntityViewArgument request = context.CommerceContext.GetObject<EntityViewArgument>();
            if (string.IsNullOrEmpty(request?.ViewName) || !request.ViewName.Equals(policy.Master, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(entityView);
            }

            if (!(request.Entity is SellableItem) || !string.IsNullOrEmpty(request.ForAction))
            {
                return Task.FromResult(entityView);
            }

            var sellableItem = request.Entity as SellableItem;

            var dateCreatedProperty = new ViewProperty() {
                DisplayName = "Date Created",
                Name = nameof(sellableItem.DateCreated),
                IsReadOnly = true,
                RawValue = sellableItem.DateCreated
            };
            entityView.Properties.Add(dateCreatedProperty);

            var dateUpdatedProperty = new ViewProperty()
            {
                DisplayName = "Date Updated",
                Name = nameof(sellableItem.DateUpdated),
                IsReadOnly = true,
                RawValue = sellableItem.DateUpdated
            };
            entityView.Properties.Add(dateUpdatedProperty);

            return Task.FromResult(entityView);
        }
    }
}