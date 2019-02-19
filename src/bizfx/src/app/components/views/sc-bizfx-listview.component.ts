import { Component, Input, OnInit } from '@angular/core';

import { ScBizFxView } from '@sitecore/bizfx';

/**
 * BizFx List View `Component`
 *
 * Handles view with `List` UiHint.
 *
 */
@Component({
  selector: 'sc-bizfx-listview',
  styleUrls: ['./sc-bizfx-listview.component.scss'],
  templateUrl: './sc-bizfx-listview.component.html'
})

export class ScBizFxListViewComponent implements OnInit {
  /**
    * @ignore
    */
  selectedView: ScBizFxView;
  /**
    * Defines the view to be render
    */
  @Input() view: ScBizFxView;

  /**
    * @ignore
    */
  ngOnInit(): void {
    if (this.view === undefined || !this.view.ChildViews || this.view.ChildViews.length === 0) { return; }

    this.selectedView = this.view.ChildViews[0];
    this.view.ItemId = this.view.ChildViews[0].ItemId;
  }

  /**
    * Handles the select of a child view.
    */
  onSelect(view: ScBizFxView): void {
    if (this.view && this.view.Actions && this.view.Actions.length) {
      this.selectedView = view;
      this.view.ItemId = view.ItemId;
    }
  }
}
