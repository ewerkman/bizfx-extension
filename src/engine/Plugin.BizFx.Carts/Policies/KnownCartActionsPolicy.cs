// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownCartActionsPolicy.cs" company="Sitecore Corporation">
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
    public class KnownCartActionsPolicy : Policy
    {
        public string DeleteCartLine { get; internal set; } = nameof(DeleteCartLine);
        public string ChangeLineQuantity { get; internal set; } = nameof(ChangeLineQuantity);
    }
}
