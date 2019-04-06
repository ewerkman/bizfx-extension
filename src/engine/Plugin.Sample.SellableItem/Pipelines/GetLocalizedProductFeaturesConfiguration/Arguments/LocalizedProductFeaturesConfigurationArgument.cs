// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizedProductFeaturesArgument.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Pipelines.GetLocalizedProductFeatures.Arguments
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;

    /// <inheritdoc />
    /// <summary>
    /// The LocalizedProductFeaturesArgument.
    /// </summary>
    public class LocalizedProductFeaturesConfigurationArgument : PipelineArgument
    {
        public LocalizedProductFeaturesConfigurationArgument(string localizationKey, object[] args = null)
        {
            Condition.Requires<string>(localizationKey, nameof(localizationKey)).IsNotNullOrEmpty();
            this.LocalizationKey = localizationKey;
            this.Args = args;
        }

        public string LocalizationKey { get; set; }

        public object[] Args { get; set; }
    }
}