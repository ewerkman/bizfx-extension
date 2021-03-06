﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeaturesComponent.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Components
{
    using Sitecore.Commerce.Core;
    using System.Collections.Generic;

    /// <inheritdoc />
    /// <summary>
    /// The FeaturesComponent.
    /// </summary>
    public class FeaturesComponent : Component
    {
        public IList<string> FeatureList { get; set; }
    }
}

