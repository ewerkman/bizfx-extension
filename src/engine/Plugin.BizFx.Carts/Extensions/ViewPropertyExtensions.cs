using Sitecore.Commerce.EntityViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.BizFx.Carts.Extensions
{
    public static class ViewPropertyExtensions
    {
        public static List<ViewProperty> AddProperty(this List<ViewProperty> properties, string name, object rawValue, string displayName = null, string uiType = null)
        {
            ViewProperty viewProperty = new ViewProperty();

            viewProperty.Name = name;
            viewProperty.DisplayName = displayName;
            viewProperty.RawValue = rawValue;
            viewProperty.UiType = uiType;

            properties.Add(viewProperty);

            return properties;
        }
    }
}
