// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.BizFx.Carts
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Plugin.BizFx.Carts.Pipelines.Blocks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config.ConfigurePipeline<IGetEntityViewPipeline>(c =>
                {
                    c.Add<GetCartViewBlock>().After<PopulateEntityVersionBlock>()
                    .Add<GetCartsViewBlock>().After<GetCartViewBlock>()
                    .Add<GetCartTotalsBlock>().After<GetCartsViewBlock>()
                    .Add<GetCartLinesViewBlock>().After<GetCartTotalsBlock>()
                    .Add<GetCartAdjustmentsViewBlock>().After<GetCartLinesViewBlock>()
                    .Add<GetCartMessagesViewBlock>().After<GetCartLinesViewBlock>()
                    .Add<GetCartLineChangeQuantityViewBlock>().After<GetCartMessagesViewBlock>();
                })
                .ConfigurePipeline<IPopulateEntityViewActionsPipeline>(c =>
                {
                    c.Add<PopulateCartLineItemsActionsBlock>().After<InitializeEntityViewActionsBlock>();
                })
                .ConfigurePipeline<IDoActionPipeline>(c =>
                {
                    c.Add<DoActionPaginateCartsListBlock>().After<ValidateEntityVersionBlock>()
                    .Add<DoActionDeleteCartLineBlock>().After<DoActionPaginateCartsListBlock>()
                    .Add<DoActionChangeLineQuantityBlock>().After<DoActionDeleteCartLineBlock>();
                })
                .ConfigurePipeline<IBizFxNavigationPipeline>(c =>
                {
                    c.Add<GetCartNavigationViewBlock>().After<GetNavigationViewBlock>();
                })
            );

            services.RegisterAllCommands(assembly);
        }
    }
}