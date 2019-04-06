// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownFeaturesViewsPolicyPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Policies
{
    using Sitecore.Commerce.Core;

    /// <inheritdoc />
    /// <summary>
    /// Defines a policy
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Policy" />
    public class KnownFeaturesViewsPolicy : Policy
    {
        public string Features { get; internal set; } = nameof(Features);
    }
}
