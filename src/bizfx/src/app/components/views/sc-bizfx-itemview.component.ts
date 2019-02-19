import { Component, Input } from '@angular/core';

import { ScBizFxProperty, ScBizFxView } from '@sitecore/bizfx';

/**
 * BizFx Item View `Component`
 *
 * Handles view with `Item` UiHint.
 *
 */
@Component({
  selector: 'sc-bizfx-itemview',
  templateUrl: './sc-bizfx-itemview.component.html',
  styleUrls: ['./sc-bizfx-itemview.component.css']
})

export class ScBizFxItemViewComponent {
  /**
    * @ignore
    */
  selected: boolean;
  /**
    * Defines the view to be render
   */
  @Input() view: ScBizFxView;
  /**
  * Defines the property
  */
  @Input() property: ScBizFxProperty;

  /**
    * @ignore
    */
  constructor() {
    this.selected = false;
  }
}
