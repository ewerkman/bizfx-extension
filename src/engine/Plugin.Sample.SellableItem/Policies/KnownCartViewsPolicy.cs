// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownCartViewsPolicy.cs" company="Sitecore Corporation">
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
    public class KnownCartViewsPolicy : Policy
    {
        public string CartsDashboard { get; internal set; } = nameof(CartsDashboard);
        public string CartCountView { get; internal set; } = nameof(CartCountView);
    }
}
