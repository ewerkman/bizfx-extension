namespace Plugin.BizFx.Carts.Pipelines.Blocks
{
    using Plugin.BizFx.Carts.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Catalog;
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
    public class GetCartLinesViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
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
        public GetCartLinesViewBlock(CommerceCommander commander)
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

            if(string.IsNullOrEmpty(request?.ViewName) ||
                !request.ViewName.Equals(knownCartViewsPolicy.Master, StringComparison.OrdinalIgnoreCase) || 
                !(request.Entity is Cart))
            {
                return Task.FromResult(entityView);
            }
            
            Cart cart = (Cart)request.Entity;

            EntityView linesView = new EntityView();
            linesView.EntityId = cart.Id;
            linesView.Name = knownCartViewsPolicy.CartLinesView;
            linesView.UiHint = "Table";

            entityView.ChildViews.Add(linesView);

            List<CartLineComponent> cartLineComponentList = new List<CartLineComponent>();
            var cartLines = cart.Lines;

            foreach (CartLineComponent line in cartLines)
            {
                EntityView lineView = new EntityView();
                lineView.EntityId = linesView.EntityId;
                lineView.ItemId = line.Id;
                lineView.Name = "LineItemDetails";

                this.PopulateLineChildView(lineView, line, context);

                CartProductComponent component = line.GetComponent<CartProductComponent>();                
                EntityView sellableItemView = new EntityView();
                sellableItemView.EntityId = component.Id;
                sellableItemView.DisplayName = component.DisplayName;
                lineView.ChildViews.Add(sellableItemView);

                linesView.ChildViews.Add((Model)lineView);
            }

            return Task.FromResult(entityView);
        }

        private void PopulateLineChildView(EntityView lineEntityView, CartLineComponent line, CommercePipelineExecutionContext context)
        {
            if (line == null || lineEntityView == null || context == null)
                return;

            LineQuantityPolicy policy = context.GetPolicy<LineQuantityPolicy>();

            ViewProperty itemIdProperty = new ViewProperty();
            itemIdProperty.Name = "ItemId";
            itemIdProperty.IsHidden = true;
            itemIdProperty.IsReadOnly = true;
            itemIdProperty.RawValue = (object)line.Id;
            lineEntityView.Properties.Add(itemIdProperty);
            
            ViewProperty listPriceProperty = new ViewProperty();
            listPriceProperty.Name = "ListPrice";
            listPriceProperty.IsReadOnly = true;
            listPriceProperty.RawValue = (object)line.UnitListPrice;
            lineEntityView.Properties.Add(listPriceProperty);
            
            ViewProperty sellPriceProperty = new ViewProperty();
            sellPriceProperty.Name = "SellPrice";
            sellPriceProperty.IsReadOnly = true;
            sellPriceProperty.RawValue = (object)line.GetPolicy<PurchaseOptionMoneyPolicy>().SellPrice;
            lineEntityView.Properties.Add(sellPriceProperty);
            
            ViewProperty quantityProperty = new ViewProperty();
            quantityProperty.Name = "Quantity";
            quantityProperty.IsReadOnly = true;
            quantityProperty.RawValue = (object)(policy.AllowDecimal ? line.Quantity : (Decimal)(int)line.Quantity);
            quantityProperty.OriginalType = policy.AllowDecimal ? typeof(Decimal).FullName : typeof(int).FullName;
            lineEntityView.Properties.Add(quantityProperty);
            
            ViewProperty subtotalProperty = new ViewProperty();
            subtotalProperty.Name = "Subtotal";
            subtotalProperty.IsReadOnly = true;
            subtotalProperty.RawValue = (object)line.Totals.SubTotal;
            lineEntityView.Properties.Add(subtotalProperty);
            
            ViewProperty adjustmentsProperty = new ViewProperty();
            adjustmentsProperty.Name = "Adjustments";
            adjustmentsProperty.IsReadOnly = true;
            adjustmentsProperty.RawValue = (object)line.Totals.AdjustmentsTotal;
            lineEntityView.Properties.Add(adjustmentsProperty);
            
            ViewProperty lineTotalProperty = new ViewProperty();
            lineTotalProperty.Name = "LineTotal";
            lineTotalProperty.IsReadOnly = true;
            lineTotalProperty.RawValue = (object)line.Totals.GrandTotal;
            lineEntityView.Properties.Add(lineTotalProperty);

            CartProductComponent component = line.GetComponent<CartProductComponent>();
            ViewProperty sellableItemNameProperty = new ViewProperty();
            sellableItemNameProperty.Name = "Name";
            sellableItemNameProperty.IsReadOnly = true;
            sellableItemNameProperty.RawValue = (object)component.DisplayName;
            lineEntityView.Properties.Add(sellableItemNameProperty);

            ViewProperty sizeProperty = new ViewProperty();
            sizeProperty.Name = "Size";
            sizeProperty.IsReadOnly = true;
            sizeProperty.RawValue = (object)component.Size;
            lineEntityView.Properties.Add(sizeProperty);

            ViewProperty colourProperty = new ViewProperty();
            colourProperty.Name = "Color";
            colourProperty.IsReadOnly = true;
            colourProperty.RawValue = (object)component.Color;
            lineEntityView.Properties.Add(colourProperty);

            ViewProperty styleProperty = new ViewProperty();
            styleProperty.Name = "Style";
            styleProperty.IsReadOnly = true;
            styleProperty.RawValue = (object)component.Style;
            lineEntityView.Properties.Add(styleProperty);
            
            ViewProperty variationProperty = new ViewProperty();
            variationProperty.Name = "Variation";
            variationProperty.IsReadOnly = true;
            variationProperty.RawValue = line.HasComponent<ItemVariationSelectedComponent>() ? (object)line.GetComponent<ItemVariationSelectedComponent>().VariationId : (object)string.Empty;
            lineEntityView.Properties.Add(variationProperty);
        }

    }
}