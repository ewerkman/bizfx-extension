// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionChangeQuantityBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.BizFx.Carts.Pipelines.Blocks
{
    using Plugin.BizFx.Carts.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
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
    public class DoActionChangeLineQuantityBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
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
        public DoActionChangeLineQuantityBlock(CommerceCommander commander)
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
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument can not be null");

            var knownCartActionsPolicy = context.GetPolicy<KnownCartActionsPolicy>();

            if (string.IsNullOrEmpty(entityView?.Action) ||
                !entityView.Action.Equals(knownCartActionsPolicy.ChangeLineQuantity, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var quantityViewProperty = entityView.Properties.FirstOrDefault(prop => prop.Name.Equals("Quantity", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(quantityViewProperty?.Value))
            {
                string str1 = quantityViewProperty == null ? "Quantity" : quantityViewProperty.DisplayName;
                string str2 = await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().ValidationError, "InvalidOrMissingPropertyValue", new object[1]
                {
                    (object) str1
                }, "Invalid or missing value for property 'Quantity'.");
                return entityView;
            }

            decimal quantity;
            if (!Decimal.TryParse(quantityViewProperty.Value, out quantity))
            {
                string validationError = context.GetPolicy<KnownResultCodes>().ValidationError;
                object[] args = new object[1]
                    {
                        quantityViewProperty.Value
                    };
                string defaultMessage = string.Format("'{0}' is not a valid number.", quantityViewProperty.Value);
                context.Abort(await context.CommerceContext.AddMessage(validationError, "InvalidNumber", args, defaultMessage), (object)context);
                return (EntityView)null;
            }

            var updatedCart = await Commander.Command<UpdateCartLineCommand>().Process(context.CommerceContext, entityView.EntityId, entityView.ItemId, quantity);

            return entityView;
        }
    }
}