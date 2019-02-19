import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { ScBizFxSearchService } from '@sitecore/bizfx';
import { ScBizFxProperty, ScBizFxView } from '@sitecore/bizfx';

/**
 * BizFx Media Picker `Component`
 */
@Component({
  selector: 'sc-bizfx-mediapicker',
  templateUrl: './sc-bizfx-mediapicker.component.html'
})

export class ScBizFxMediaPickerComponent implements OnInit {
  /**
    * Defines the view
    */
  @Input() view: ScBizFxView;
  /**
    * Defines the form group that maps to the view
    */
  @Input() actionForm: FormGroup;
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
  resultsView: ScBizFxView[];
  /**
    * @ignore
    */
  private searchView: ScBizFxView;

  /**
    * @ignore
    */
  constructor(
    private searchService: ScBizFxSearchService) {
  }

  /**
    * @ignore
    */
  ngOnInit(): void {
    if (this.view === undefined || !this.view.ChildViews || this.view.ChildViews.length === 0) {
      return;
    }

    this.setData(this.view);
  }

  /**
    * Executes a search.
    */
  search(page: number): void {
    this.top.Value = this.pageSize.toString();

    const pageIndex = page - 1;
    this.skip.Value = (pageIndex * this.pageSize).toString();

    this.searchService.search(this.searchView).then(view => this.setData(view));
  }

  /**
    * @ignore
    */
  onSelect(id: string): void {
    const view = this.resultsView.find(v => v.ItemId === id);

    this.selectedView = view;
    this.view.ItemId = view.ItemId;
  }

  /**
    * Helper method
    *
    * Prepare the component propeties based on the returned view
    */
  protected setData(view: ScBizFxView) {
    this.resultsView = view.ChildViews.filter(v => v.UiHint !== 'Search');

    if (this.resultsView && this.resultsView[0]) {
      this.selectedView = this.resultsView[0];
      this.view.ItemId = this.resultsView[0].ItemId;
    }

    this.searchView = view.ChildViews.filter(v => v.UiHint === 'Search')[0];
    if (this.searchView) {
      this.top = this.searchView.Properties.filter(p => p.Name === 'Top')[0];
      this.skip = this.searchView.Properties.filter(p => p.Name === 'Skip')[0];
      this.pageSize = +this.top.Value;
      this.count = this.searchView.Properties.filter(p => p.Name === 'Count')[0]
        ? +this.searchView.Properties.filter(p => p.Name === 'Count')[0].Value
        : 0;
    }
  }
}
