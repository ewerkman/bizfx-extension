namespace Plugin.Sample.Notes.Components
{
    using Sitecore.Commerce.Core;

    public class NotesComponent : Component
    {
        public string InternalNotes { get; set; } = string.Empty;
        public string WarrantyInformation { get; set; } = string.Empty;
    }
}

