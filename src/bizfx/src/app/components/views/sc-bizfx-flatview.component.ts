import { Component, Input } from '@angular/core';

import { ScBizFxProperty, ScBizFxView, ScBizFxContextService } from '@sitecore/bizfx';

/**
 * BizFx Flat View `Component`
 *
 * Handles view with `Flat` UiHint.
 *
 */
@Component({
  selector: 'sc-bizfx-flatview',
  templateUrl: './sc-bizfx-flatview.component.html',
  styleUrls: ['./sc-bizfx-flatview.component.css']
})

export class ScBizFxFlatViewComponent {
  /**
    * Defines the view
    */
  @Input() view: ScBizFxView;

  /**
    * @ignore
    */
  constructor(
    public bizFxContext: ScBizFxContextService) {
  }

  /**
    * Helper method
    *
    * Parse the value of property with `UiType` of `List`.
    */
  getList(property: ScBizFxProperty) {
    console.log(property);
    return property.Value != null ? JSON.parse(property.Value) : [];
  }
}
