// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIGetLocalizedProductFeaturesPipelinePipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures
{
    using Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures.Arguments;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the IIGetLocalizedProductFeaturesPipelinePipeline interface
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.IPipeline{PipelineArgumentOrEntity,
    ///         PipelineArgumentOrEntity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("[Insert Project Name].Pipeline.IIGetLocalizedProductFeaturesPipelinePipeline")]
    public interface IGetLocalizedProductFeaturesPipeline : IPipeline<LocalizedProductFeaturesArgument, IEnumerable<LocalizedTerm>, CommercePipelineExecutionContext>
    {
    }
}
