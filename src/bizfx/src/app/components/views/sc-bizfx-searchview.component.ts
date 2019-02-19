import { Component, Input, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/takeWhile';

import { SortDirection } from '@speak/ng-bcl/core/types';
import { SortHeaderState } from '@speak/ng-bcl/table';

import { ScBizFxSearchService } from '@sitecore/bizfx';
import { ScBizFxView, ScBizFxProperty, ScBizFxActionMessage } from '@sitecore/bizfx';

/**
 * BizFx Search View `Component`
 *
 * Handles view with `Searchs` UiHint.
 *
 */
@Component({
  selector: 'sc-bizfx-searchview',
  templateUrl: './sc-bizfx-searchview.component.html'
})

export class ScBizFxSearchViewComponent implements OnInit, OnDestroy {
  /**
    * Defines the view to be render
    */
  @Input() view: ScBizFxView;
  /**
    * @ignore
    */
  selectedResult: ScBizFxView;
  /**
    * @ignore
    */
  results: Observable<ScBizFxView>;
  /**
    * @ignore
    */
  resultsView: ScBizFxView;
  /**
    * @ignore
    */
  searchTerms = new Subject<string>();
  /**
    * @ignore
    */
  searching: boolean;
  /**
    * @ignore
    */
  term: ScBizFxProperty;
  /**
    * @ignore
    */
  orderBy: ScBizFxProperty;
  /**
    * @ignore
    */
  filter: ScBizFxProperty;
  /**
    * @ignore
    */
  skip: ScBizFxProperty;
  /**
    * @ignore
    */
  top: ScBizFxProperty;
  /**
    * @ignore
    */
  count = 0;
  /**
    * @ignore
    */
  pageSize = 10;
  /**
    * @ignore
    */
  sortState = [];
  /**
    * @ignore
    */
  propView: ScBizFxProperty[];
  /**
    * @ignore
    */
  noResultsFound = false;
  /**
    * @ignore
    */
  private alive = true;

  /**
    * @ignore
    */
  constructor(
    private searchService: ScBizFxSearchService,
    private cdr: ChangeDetectorRef) {
  }

  /**
    * @ignore
    */
  ngOnInit(): void {
    this.term = this.view.Properties.filter(p => p.Name === 'Term')[0];
    this.filter = this.view.Properties.filter(p => p.Name === 'Filter')[0];
    this.orderBy = this.view.Properties.filter(p => p.Name === 'OrderBy')[0];
    this.skip = this.view.Properties.filter(p => p.Name === 'Skip')[0];
    this.top = this.view.Properties.filter(p => p.Name === 'Top')[0];

    this.results = this.searchTerms
      .debounceTime(1000)
      .switchMap(term => term
        ? this.searchService.search(this.view)
        : Observable.of<ScBizFxView>())
      .catch(error => {
        this.searching = false;
        this.noResultsFound = false;
        return Observable.of<ScBizFxView>();
      });

    this.results
    .takeWhile(() => this.alive)
    .subscribe(view => {
      this.resultsView = view;

      if (view.ChildViews.length > 0) {
        this.selectedResult = view.ChildViews[0];
        this.resultsView.ItemId = this.selectedResult.ItemId;
        this.propView = view && this.selectedResult ? this.selectedResult.Properties.filter(prop => !prop.IsHidden) : [];
      } else {
        this.noResultsFound = true;
      }

      const count = view.Properties.filter(p => p.Name === 'Count')[0];
      this.count = count !== null && count !== undefined ? +view.Properties.filter(p => p.Name === 'Count')[0].Value : 0;
      this.searching = false;
      this.cdr.detectChanges();
    });
  }

  /**
    * @ignore
    */
  ngOnDestroy(): void {
    this.alive = false;
  }

  /**
    * Executes the search
    */
  search(sort: any, page?: number): void {
    if (this.term.Value === '') {
      return;
    }

    if (sort) {
      this.orderBy.Value = Array.isArray(sort) && sort.length ? `${sort[0].id} ${sort[0].direction}` : '';
    }

    if (page !== undefined && page !== null) {
      const pageIndex = page - 1;
      this.top.Value = this.pageSize.toString();
      this.skip.Value = (pageIndex * this.pageSize).toString();
    }

    this.searching = true;
    this.noResultsFound = false;
    this.searchTerms.next(this.term.Value);
  }

  /**
    * Handles the select of a child view
    */
  onSelect(id: string): void {
    const view = this.resultsView.ChildViews.find(v => v.ItemId === id);

    this.selectedResult = view;
    this.resultsView.ItemId = view ? view.ItemId : null;
  }

  /**
    * Handles the changes of sort column
    */
  onSortChange(sortState: SortHeaderState[]) {
    this.sortState = sortState;
    this.search(sortState);
  }

  /**
    * Handles the changes of sort direction
    */
  getDirection(id: string): SortDirection {
    const state = this.sortState.find((s: SortHeaderState) => s.id === id);
    return state ? state.direction : '';
  }
}
