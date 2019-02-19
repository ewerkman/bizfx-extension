import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/takeWhile';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { ScBizFxViewsService, ScBizFxContextService, ScBizFxView, ScBizFxAction, ScBizFxActionMessage } from '@sitecore/bizfx';

/**
 * BizFx View `Component`
 *
 */
@Component({
  selector: 'sc-bizfx-view',
  templateUrl: './sc-bizfx-view.component.html'
})

export class ScBizFxViewComponent implements OnInit, OnDestroy {
  /**
    * Defines the iew
    */
  view: ScBizFxView;
  /**
    * @ignore
    */
  action: ScBizFxAction;
  /**
    * @ignore
    */
  loading = true;
  /**
    * @ignore
    */
  messages: ScBizFxActionMessage[];
  /**
    * @ignore
    */
  private alive = true;

  /**
    * @ignore
    */
  constructor(
    private viewsService: ScBizFxViewsService,
    private bizFxContext: ScBizFxContextService,
    private route: ActivatedRoute) {
  }

  /**
    * @ignore
    */
  ngOnInit(): void {
    this.viewsService.announcePageType(this.route.snapshot.data.pageType);

    this.route.params
      .takeWhile(() => this.alive)
      .switchMap((params: Params) =>
        this.viewsService.getView(
          params['viewName'],
          params['entityId'],
          '',
          params['itemId'],
          params['version']))
      .subscribe(view => {
        this.loading = false;
        this.view = view;
      });

    this.bizFxContext.environment$
      .takeWhile(() => this.alive)
      .subscribe(environment => this.getView());

    this.bizFxContext.language$
      .takeWhile(() => this.alive)
      .subscribe(language => this.getView());

    this.viewsService.actionAnnounced$
      .takeWhile(() => this.alive)
      .subscribe(
        action => {
          this.action = action;
          setTimeout(() => this.action = null, 2000);
          this.getView();
        });

    this.viewsService.viewInfosAnnounced$
      .takeWhile(() => this.alive)
      .subscribe(errors => this.messages = errors);
  }

  /**
    * @ignore
    */
  ngOnDestroy(): void {
    this.alive = false;
  }

  /**
    * Helper method
    *
    * Calls views service `getView` method
    */
  getView() {
    this.loading = true;

    this.viewsService.getView(
      this.view ? this.view.Name : '',
      this.view && this.view.EntityId ? this.view.EntityId : '',
      this.view && this.view.Action ? this.view.Action : '',
      this.view && this.view.ItemId ? this.view.ItemId : '',
      this.view && this.view.EntityVersion ? this.view.EntityVersion : -1)
      .then(view => {
        this.view = view;
        this.loading = false;
      });
  }
}
