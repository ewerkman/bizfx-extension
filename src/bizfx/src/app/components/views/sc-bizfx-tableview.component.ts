import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';

import { ScBizFxView, ScBizFxProperty } from '@sitecore/bizfx';
import { ScBizFxViewsService } from '@sitecore/bizfx';

/**
 * BizFx Table View `Component`
 *
 * Handles view with `Table` UiHint.
 *
 */
@Component({
  selector: 'sc-bizfx-tableview',
  templateUrl: './sc-bizfx-tableview.component.html'
})

export class ScBizFxTableViewComponent implements OnInit, OnChanges {
  /**
    * Defiens the view to be render
    */
  @Input() view: ScBizFxView;
/**
    * @ignore
    */
  selectedView: ScBizFxView;
  /**
    * @ignore
    */
  top: ScBizFxProperty;
  /**
    * @ignore
    */
  private skip: ScBizFxProperty;
  /**
    * @ignore
    */
  count = 0;
  /**
    * @ignore
    */
  pageSize: number;
  /**
    * @ignore
    */
  paginating = false;
  /**
    * @ignore
    */
  showPagination: boolean;
  /**
    * @ignore
    */
  page = 1;

  /**
    * @ignore
    */
  constructor(
    private viewsService: ScBizFxViewsService) {
  }

  /**
    * @ignore
    */
  ngOnInit(): void {
    if (this.view === undefined || !this.view.ChildViews || this.view.ChildViews.length === 0) {
      return;
    }

    this.setData();
  }

  /**
    * @ignore
    */
  ngOnChanges(changes: SimpleChanges): void {
    this.page = 1;
    this.skip = this.view.Properties.filter(p => p.Name === 'Skip')[0];
  }

  /**
    * Handles the pagination for the table
    */
  paginate(page: number): void {
    if (!this.view) {
      return;
    }

    this.page = page;
    this.top.Value = this.pageSize.toString();
    this.skip.Value = ((page - 1) * this.pageSize).toString();
    this.paginating = true;
    this.view.ChildViews = [];

    this.viewsService
      .doAction(this.view)
      .then(actionResult => {
        this.view.ActionResult = actionResult;
        this.view = actionResult.NextView;

        this.setData();
      });
  }

  /**
    * Hanldes the select of a child view
    */
  onSelect(id: string): void {
    const view = this.view.ChildViews.find(c => c.VersionedItemId === id);

    if (view) {
      this.selectedView = view;
      this.view.ItemId = view.ItemId;
      this.view.VersionedItemId = view.VersionedItemId;
    }
  }

  /**
    * Helper method
    *
    * Prepares data to be render.
    */
  protected setData() {
    this.paginating = false;

    if (!this.view) {
      return;
    }
    if (this.view.SelectedChildView && this.view.SelectedChildView.ItemId) {
      this.selectedView = this.view.SelectedChildView;
    } else {
      this.selectedView = this.view.ChildViews[0];
    }
    this.view.ItemId = this.selectedView.ItemId;
    this.view.VersionedItemId = this.selectedView.VersionedItemId;

    this.top = this.view.Properties.find(p => p.Name === 'Top');
    this.skip = this.view.Properties.find(p => p.Name === 'Skip');
    const count = this.view.Properties.find(p => p.Name === 'Count');
    this.showPagination = this.skip && this.top && count && this.view && this.view.ChildViews.length > 0;
    this.pageSize = this.showPagination ? +this.top.Value : 0;
    this.count = this.showPagination && count ? +count.Value : 0;
  }
}
