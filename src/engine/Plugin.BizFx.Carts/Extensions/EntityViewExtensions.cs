using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.BizFx.Carts.Extensions
{
    internal static class EntityViewExtensions
    {
        public static EntityView FillWithCarts(this EntityView cartsView, IEnumerable<Cart> carts)
        {
            foreach (Cart cart in carts)
            {
                EntityView entityView = new EntityView();
                entityView.EntityId = string.Empty;
                entityView.ItemId = cart.Id;
                entityView.Name = "Summary";

                EntityView entityView2 = entityView;

                List<ViewProperty> properties1 = entityView2.Properties;

                ViewProperty viewProperty1 = new ViewProperty();
                viewProperty1.Name = "CartId";
                viewProperty1.RawValue = (object)cart.Id;
                viewProperty1.IsReadOnly = true;
                viewProperty1.UiType = "EntityLink";
                properties1.Add(viewProperty1);

                List<ViewProperty> properties2 = entityView2.Properties;
                ViewProperty viewProperty2 = new ViewProperty();
                viewProperty2.Name = "Items";
                viewProperty2.RawValue = (object)cart.ItemCount;
                viewProperty2.IsReadOnly = true;
                properties2.Add(viewProperty2);

                ViewProperty totalViewProperty = new ViewProperty();
                totalViewProperty.Name = "CartTotal";
                totalViewProperty.RawValue = (object)cart.Totals.GrandTotal;
                totalViewProperty.IsReadOnly = true;
                entityView2.Properties.Add(totalViewProperty);

                cartsView.ChildViews.Add(entityView2);
            }

            return cartsView;
        }

        public static EntityView AddProperty(this EntityView entityView, string name, object rawValue, string displayName = null, string uiType = null)
        {
            ViewProperty viewProperty = new ViewProperty();

            viewProperty.Name = name;
            viewProperty.DisplayName = displayName;
            viewProperty.RawValue = rawValue;
            viewProperty.UiType = uiType;

            entityView.Properties.Add(viewProperty);

            return entityView;
        }
    }
}
