﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionEditNotesBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Notes.Pipelines.Blocks
{
    using Plugin.Sample.Notes.Components;
    using Plugin.Sample.Notes.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a pipeline block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Core.PipelineArgument,
    ///         Sitecore.Commerce.Core.PipelineArgument, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("Change to <Project>Constants.Pipelines.Blocks.<Block Name>")]
    public class DoActionEditNotesBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Gets or sets the commander.
        /// </summary>
        /// <value>
        /// The commander.
        /// </value>
        protected ViewCommander Commander { get; set; }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public DoActionEditNotesBlock(ViewCommander commander)
            : base(null)
        {

            this.Commander = commander;

        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="entityView">
        /// The pipeline argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="PipelineArgument"/>.
        /// </returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument can not be null");

            var notesActionsPolicy = context.GetPolicy<KnownNotesActionsPolicy>();

            // Only proceed if the right action was invoked
            if (string.IsNullOrEmpty(entityView.Action) || !entityView.Action.Equals(notesActionsPolicy.EditNotes, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(entityView);
            }

            // Get the sellable item from the context
            var entity = context.CommerceContext.GetObject<SellableItem>(x => x.Id.Equals(entityView.EntityId));
            if (entity == null)
            {
                return Task.FromResult(entityView);
            }

            // Get the notes component from the sellable item or its variation
            var component = entity.GetComponent<NotesComponent>(entityView.ItemId);

            // Map entity view properties to component
            component.WarrantyInformation = entityView.Properties.FirstOrDefault(x => x.Name.Equals(nameof(NotesComponent.WarrantyInformation), StringComparison.OrdinalIgnoreCase))?.Value;
            component.InternalNotes = entityView.Properties.FirstOrDefault(x => x.Name.Equals(nameof(NotesComponent.InternalNotes), StringComparison.OrdinalIgnoreCase))?.Value;

            // Persist changes
            this.Commander.PersistEntity(context.CommerceContext, entity);
            
            return Task.FromResult(entityView);
        }
    }
}