// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownCartViewsPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.BizFx.Carts.Policies
{
    using Sitecore.Commerce.Core;

    /// <inheritdoc />
    /// <summary>
    /// Defines a policy
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Policy" />
    public class KnownCartViewsPolicy : Policy
    {
        public string Master { get; set; } = nameof(Master);
        public string CartsDashboard { get; internal set; } = nameof(CartsDashboard);
        public string CartCountView { get; internal set; } = nameof(CartCountView);
        public string CartAdjustmentsView { get; internal set; } = nameof(CartAdjustmentsView);
        public string CartLinesView { get; internal set; } = nameof(CartLinesView);
        public string CartMessagesView { get; internal set; } = nameof(CartMessagesView);

        public string CartLineChangeQuantityView { get; internal set; } = nameof(CartLineChangeQuantityView);
    }
}
