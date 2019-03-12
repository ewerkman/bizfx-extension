// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCartViewBlock.cs" company="Sitecore Corporation">
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
    public class GetCartViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
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
        public GetCartViewBlock(CommerceCommander commander)
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
        public override async Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: The argument can not be null");

            var cartViewsPolicy = context.GetPolicy<KnownCartViewsPolicy>();

            /* Add business logic here */
            if (!arg.Name.Equals(cartViewsPolicy.CartsDashboard, StringComparison.InvariantCultureIgnoreCase))
            {
                return arg;
            }

            // Create a new view and add it to the current entity view.
            var view = new EntityView
            {
                Name = cartViewsPolicy.CartCountView,
                DisplayName = cartViewsPolicy.CartCountView,
                EntityId = arg.EntityId,
                UiHint = "Counter"
            };

            arg.ChildViews.Add(view);

            var targetView = view;

            var cartCount = await GetListCount("Carts", context.CommerceContext);

            targetView.Properties.Add(new ViewProperty
            {
                Name = "CartCount",
                RawValue = cartCount
            });

            return arg;
        }

        private async Task<Decimal> GetListCount(string listName, CommerceContext context)
        {
            var listMetaData = await this.Commander.Pipeline<IPopulateListMetadataPipeline>().Run(new ListMetadata(listName), (IPipelineExecutionContextOptions)context.GetPipelineContextOptions());

            return Convert.ToDecimal(listMetaData.Count);
        }
    }
}