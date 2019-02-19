import { Component, Input } from '@angular/core';

import { ScBizFxView } from '@sitecore/bizfx';

/**
 * BizFx List Policies `Component`
 *
 */
@Component({
  selector: 'sc-bizfx-listpolicies',
  templateUrl: './sc-bizfx-listPolicies.component.html'
})

export class ScBizFxListPoliciesComponent {
  /**
    * Defines the view
    */
  @Input() view: ScBizFxView;
  /**
    * Defines the view's policies to be render
    */
  @Input() policies: ScBizFxView[];
}
