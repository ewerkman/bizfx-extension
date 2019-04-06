// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductFeaturesControlPanelPolicy.cs" company="Sitecore Corporation">
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
    public class ProductFeaturesControlPanelPolicy : Policy
    {
        public string ProductFeaturesNamesPath { get; set; } = "/sitecore/Commerce/Commerce Control Panel/Commerce Engine Settings/Commerce Terms/BusinessTools/ProductFeatures";
        public string AvailableProductFeaturesPath { get; set; } = "/sitecore/Commerce/Commerce Control Panel/Storefront Settings/Commerce Terms/ProductFeatures";
    }
}