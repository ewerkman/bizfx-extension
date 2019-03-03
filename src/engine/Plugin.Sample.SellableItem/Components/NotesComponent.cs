// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotesComponent.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Components
{
    using Sitecore.Commerce.Core;

    /// <inheritdoc />
    /// <summary>
    /// The NotesComponent.
    /// </summary>
    public class NotesComponent : Component
    {
        public string InternalNotes { get; set; } = string.Empty;
        public string WarrantyInformation { get; set; } = string.Empty;
    }
}

