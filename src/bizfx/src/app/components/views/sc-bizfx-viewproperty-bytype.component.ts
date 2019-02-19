import { Component, Input, OnInit } from '@angular/core';

import { ScBizFxProperty, ScBizFxView, ScBizFxContextService } from '@sitecore/bizfx';

/**
 * BizFx View Property By Type `Component`
 *
 */
@Component({
  selector: 'sc-bizfx-viewproperty-bytype',
  styleUrls: ['./sc-bizfx-viewproperty-bytype.component.scss'],
  templateUrl: './sc-bizfx-viewproperty-bytype.component.html'
})

export class ScBizFxViewPropertyByTypeComponent implements OnInit {
  /**
    * Defines the property to be render
    */
  @Input() property: ScBizFxProperty;
  /**
    * Defines the view
    */
  @Input() view: ScBizFxView;
  /**
    * Defines if the property's header should be display or not.
    */
  @Input() hideHeader: boolean;
  /**
      * @ignore
      */
  list: any[];

  /**
    * @ignore
    */
  constructor(
    private bizFxContext: ScBizFxContextService) {
  }

  /**
    * @ignore
    */
  ngOnInit(): void {
    if (this.property.OriginalType === 'List'
      && this.property.Value !== null
      && this.property.Value !== undefined) {
      this.list = JSON.parse(this.property.Value);
    }
  }
}
