// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeaturesComponent.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.SellableItem.Components
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Plumber.Component.Decorator.Attributes;

    /// <inheritdoc />
    /// <summary>
    /// The FeaturesComponent.
    /// </summary>
    [EntityView("Features")]
    [AddToEntityType(typeof(SellableItem))]
    public class FeaturesComponent : Component
    {
        [Property(displayName: "Features", UIType:"Multiselect")]
        public string FeatureList { get; set; }
    }
}

