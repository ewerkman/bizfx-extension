// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCartLinesViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Pipelines.Blocks
{
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

            EntityViewArgument request = context.CommerceContext.GetObject<EntityViewArgument>();
            //if (string.IsNullOrEmpty(request?.ViewName) || !request.ViewName.Equals(context.GetPolicy<KnownOrderViewsPolicy>().Lines, StringComparison.OrdinalIgnoreCase) && !request.ViewName.Equals(context.GetPolicy<KnownOrderViewsPolicy>().LineItemDetails, StringComparison.OrdinalIgnoreCase) && !request.ViewName.Equals(context.GetPolicy<KnownOrderViewsPolicy>().Master, StringComparison.OrdinalIgnoreCase) || (request.ViewName.Equals(context.GetPolicy<KnownOrderViewsPolicy>().LineItemDetails, StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(request.ItemId) || !(request.Entity is Order)))
            //    return entityView;
            if (!(request.Entity is Cart))
            {
                return Task.FromResult(entityView);
            }

            Cart cart = (Cart)request.Entity;

            EntityView linesView = new EntityView();
            linesView.EntityId = cart.Id;
            linesView.Name = "Lines";
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

                linesView.ChildViews.Add((Model)lineView);
            }

            return Task.FromResult(entityView);
        }

        private void PopulateLineChildView(EntityView lineEntityView, CartLineComponent line, CommercePipelineExecutionContext context)
        {
            if (line == null || lineEntityView == null || context == null)
                return;

            LineQuantityPolicy policy = context.GetPolicy<LineQuantityPolicy>();
            List<ViewProperty> properties1 = lineEntityView.Properties;
            ViewProperty viewProperty1 = new ViewProperty();
            viewProperty1.Name = "ItemId";
            viewProperty1.IsHidden = true;
            viewProperty1.IsReadOnly = true;
            viewProperty1.RawValue = (object)line.Id;
            properties1.Add(viewProperty1);
            List<ViewProperty> properties2 = lineEntityView.Properties;
            ViewProperty viewProperty2 = new ViewProperty();
            viewProperty2.Name = "ListPrice";
            viewProperty2.IsReadOnly = true;
            viewProperty2.RawValue = (object)line.UnitListPrice;
            properties2.Add(viewProperty2);
            List<ViewProperty> properties3 = lineEntityView.Properties;
            ViewProperty viewProperty3 = new ViewProperty();
            viewProperty3.Name = "SellPrice";
            viewProperty3.IsReadOnly = true;
            viewProperty3.RawValue = (object)line.GetPolicy<PurchaseOptionMoneyPolicy>().SellPrice;
            properties3.Add(viewProperty3);
            List<ViewProperty> properties4 = lineEntityView.Properties;
            ViewProperty viewProperty4 = new ViewProperty();
            viewProperty4.Name = "Quantity";
            viewProperty4.IsReadOnly = true;
            viewProperty4.RawValue = (object)(policy.AllowDecimal ? line.Quantity : (Decimal)(int)line.Quantity);
            viewProperty4.OriginalType = policy.AllowDecimal ? typeof(Decimal).FullName : typeof(int).FullName;
            properties4.Add(viewProperty4);
            List<ViewProperty> properties5 = lineEntityView.Properties;
            ViewProperty viewProperty5 = new ViewProperty();
            viewProperty5.Name = "Subtotal";
            viewProperty5.IsReadOnly = true;
            viewProperty5.RawValue = (object)line.Totals.SubTotal;
            properties5.Add(viewProperty5);
            List<ViewProperty> properties6 = lineEntityView.Properties;
            ViewProperty viewProperty6 = new ViewProperty();
            viewProperty6.Name = "Adjustments";
            viewProperty6.IsReadOnly = true;
            viewProperty6.RawValue = (object)line.Totals.AdjustmentsTotal;
            properties6.Add(viewProperty6);
            List<ViewProperty> properties7 = lineEntityView.Properties;
            ViewProperty viewProperty7 = new ViewProperty();
            viewProperty7.Name = "LineTotal";
            viewProperty7.IsReadOnly = true;
            viewProperty7.RawValue = (object)line.Totals.GrandTotal;
            properties7.Add(viewProperty7);
            CartProductComponent component = line.GetComponent<CartProductComponent>();
            List<ViewProperty> properties8 = lineEntityView.Properties;
            ViewProperty viewProperty8 = new ViewProperty();
            viewProperty8.Name = "Name";
            viewProperty8.IsReadOnly = true;
            viewProperty8.RawValue = (object)component.DisplayName;
            viewProperty8.UiType = "ItemLink";
            properties8.Add(viewProperty8);
            List<ViewProperty> properties9 = lineEntityView.Properties;
            ViewProperty viewProperty9 = new ViewProperty();
            viewProperty9.Name = "Size";
            viewProperty9.IsReadOnly = true;
            viewProperty9.RawValue = (object)component.Size;
            properties9.Add(viewProperty9);
            List<ViewProperty> properties10 = lineEntityView.Properties;
            ViewProperty viewProperty10 = new ViewProperty();
            viewProperty10.Name = "Color";
            viewProperty10.IsReadOnly = true;
            viewProperty10.RawValue = (object)component.Color;
            properties10.Add(viewProperty10);
            List<ViewProperty> properties11 = lineEntityView.Properties;
            ViewProperty viewProperty11 = new ViewProperty();
            viewProperty11.Name = "Style";
            viewProperty11.IsReadOnly = true;
            viewProperty11.RawValue = (object)component.Style;
            properties11.Add(viewProperty11);
            List<ViewProperty> properties12 = lineEntityView.Properties;
            ViewProperty viewProperty12 = new ViewProperty();
            viewProperty12.Name = "Variation";
            viewProperty12.IsReadOnly = true;
            viewProperty12.RawValue = line.HasComponent<ItemVariationSelectedComponent>() ? (object)line.GetComponent<ItemVariationSelectedComponent>().VariationId : (object)string.Empty;
            properties12.Add(viewProperty12);
        }

    }
}