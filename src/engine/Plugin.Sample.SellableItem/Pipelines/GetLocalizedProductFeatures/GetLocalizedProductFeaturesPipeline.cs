﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGetLocalizedProductFeaturesPipelinePipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures
{
    using Microsoft.Extensions.Logging;
    using Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures.Arguments;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using System.Collections.Generic;

    /// <inheritdoc />
    /// <summary>
    ///  Defines the IGetLocalizedProductFeaturesPipelinePipeline pipeline.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Commerce.Core.CommercePipeline{Namespace.PipelineArgumentOrEntity,
    ///         Namespace.PipelineArgumentOrEntity}
    ///     </cref>
    /// </seealso>
    /// <seealso cref="T:Plugin.Sample.Notes.Pipelines.safeitemrootname" />
    public class GetLocalizedProductFeaturesPipeline : CommercePipeline<LocalizedProductFeaturesArgument, IEnumerable<LocalizedTerm>>, IGetLocalizedProductFeaturesPipeline
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Plugin.Sample.Notes.Pipelines.IGetLocalizedProductFeaturesPipelinePipeline" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public GetLocalizedProductFeaturesPipeline(IPipelineConfiguration<IGetLocalizedProductFeaturesPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}

