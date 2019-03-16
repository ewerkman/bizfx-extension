// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Plugin.Sample.Notes.Pipelines.Blocks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Commerce.Plugin.Catalog;
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
                         c.Add<GetNotesViewBlock>().After<GetSellableItemDetailsViewBlock>()
                         .Add<GetFeaturesViewBlock>().After<GetNotesViewBlock>();
                     })
                     .ConfigurePipeline<IPopulateEntityViewActionsPipeline>(c =>
                     {
                         c.Add<PopulateNotesActionsBlock>().After<InitializeEntityViewActionsBlock>()
                         .Add<PopulateFeaturesActionsBlock>().After<PopulateNotesActionsBlock>();
                     })
                     .ConfigurePipeline<IDoActionPipeline>(c =>
                     {
                         c.Add<DoActionEditNotesBlock>().After<ValidateEntityVersionBlock>()
                         .Add<DoActionEditFeaturesBlock>().After<DoActionEditNotesBlock>();
                     })
                 );

            services.RegisterAllCommands(assembly);
        }
    }
}