// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCartMessagesViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.BizFx.Carts.Pipelines.Blocks
{
    using Plugin.BizFx.Carts.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
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
    public class GetCartMessagesViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Gets or sets the commander.
        /// </summary>
        /// <value>
        /// The commander.
        /// </value>
        protected CommerceCommander Commander { get; set; }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public GetCartMessagesViewBlock(CommerceCommander commander)
            : base(null)
        {

            this.Commander = commander;

        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
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

            var knownCartViewsPolicy = context.GetPolicy<KnownCartViewsPolicy>();
            EntityViewArgument request = context.CommerceContext.GetObject<EntityViewArgument>();

            if (string.IsNullOrEmpty(request?.ViewName) ||
                !request.ViewName.Equals(knownCartViewsPolicy.Master, StringComparison.OrdinalIgnoreCase) ||
                !(request.Entity is Cart))
            {
                return Task.FromResult(entityView);
            }

            Cart cart = (Cart)request.Entity;

            EntityView messagesView = new EntityView();
            messagesView.EntityId = cart.Id;
            messagesView.Name = "Messages";
            messagesView.UiHint = "Table";

            entityView.ChildViews.Add(messagesView);

            var messagesComponent = cart.GetComponent<MessagesComponent>();
            foreach(var message in messagesComponent.Messages)
            {
                var messageView = new EntityView();
                messageView.EntityId = cart.Id;
                messageView.ItemId = message.Id;
                messageView.Name = "MessageDetails";

                ViewProperty codeProperty = new ViewProperty();
                codeProperty.Name = "Code";
                codeProperty.IsHidden = false;
                codeProperty.IsReadOnly = true;
                codeProperty.RawValue = message.Code;
                messageView.Properties.Add(codeProperty);

                ViewProperty textProperty = new ViewProperty();
                textProperty.Name = "Text";
                textProperty.IsHidden = false;
                textProperty.IsReadOnly = true;
                textProperty.RawValue = message.Text;
                messageView.Properties.Add(textProperty);

                messagesView.ChildViews.Add(messageView);
            }


            return Task.FromResult(entityView);
        }
    }
}