// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCartsViewBlockBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Pipelines.Blocks
{
    using Plugin.Sample.Notes.Extensions;
    using Plugin.Sample.Notes.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Collections.Generic;
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
    public class GetCartsViewBlock : GetListViewBlock
    {


        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public GetCartsViewBlock(ViewCommander commander)
            : base(commander)
        {
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

            if (!arg.Name.Equals(cartViewsPolicy.CartsDashboard, StringComparison.InvariantCultureIgnoreCase))
            {
                return arg;
            }

            EntityViewArgument entityViewArgument = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault<EntityViewArgument>();

            var cartsView = new EntityView()
            {
                Name = "Carts",
                DisplayName = "Carts",
                UiHint = "Table"
            };
            arg.ChildViews.Add(cartsView);

            string listName = "Carts";
            await this.SetListMetadata(cartsView, listName, "PaginateCartsViewList", context);

            var carts = (await this.GetEntities(arg, "Carts", context)).OfType<Cart>();

            cartsView.FillWithCarts(carts);

            return arg;
        }
    }
}