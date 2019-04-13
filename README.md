# Extending the Sitecore Commerce Business Tools

This is the code from the the talk I held at SUGCON 2019 in London. You can find the presentation [here](https://commerceservertips.com/sugcon-2019-slides-extending-the-sitecore-commerce-business-tools/).

[![](https://commerceservertips.com/content/images/2019/04/Extending-the-Sitecore-Commerce-Business-Tools.PNG)](https://commerceservertips.com/sugcon-2019-slides-extending-the-sitecore-commerce-business-tools/)

## How is the code structured?

In this repository you will find two folders:
* `bizfx` contains the code for the Business Tools.
* `engine` contains sample engine code.

### The Business Tools project

This is basically the contents of the BizFX SDK with:
* a new component which you will find in `src\bizfx\src\app\selectize`;
* code to render the selectize component in `src\bizfx\src\app\components\actions\sc-bizfx-actionproperty.component.html`;

### The engine project

The engine project contains two notable projects:
* `Plugin.Sample.SellableItem` contains code that adds two components to a Sellable Item: `NotesComponent` and `FeaturesComponent`. The `FeaturesComponent` is used to demonstrate the Selectize component in the Business Tools.
* `Plugin.BizFx.Carts` contains sample code that extends the Business Tools with a dashboard to view carts. It contains samples of creating a new dashboard, new entity views, new actions etc. 