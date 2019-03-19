// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvailalableSelectionsPolicyPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;
using System.Collections.Generic;

namespace Plugin.Sample.Notes.Policies
{


    /// <inheritdoc />
    /// <summary>
    /// Defines a policy
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Policy" />
    public class SelectizeConfigPolicy : Policy
    {
        public SelectizeConfigPolicy()
        {
            this.Options = new System.Collections.Generic.List<Selection>();
        }

        // Available options to select from
        public List<Selection> Options { get; set; }

        // Allows the user to create new items that aren't in the initial list of options. 
        public bool Create { get; set; } = false;

        // Placeholder text to be displayed. Is overridden if hasOptionsPlaceholder/noOptionsPlaceholder are non-null
        public string Placeholder { get; set; }

        // Placeholder text to be displayed when no options are available
        public string NoOptionsPlaceholder { get; set; }

        // Placeholder text to be displayed when options are available
        public string HasOptionsPlaceholder { get; set; }

        // Enables the input field when true, disabled otherwise
        public bool Enabled { get; set; }
    }
}
