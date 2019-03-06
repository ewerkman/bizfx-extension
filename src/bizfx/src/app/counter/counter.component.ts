import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { ScBizFxViewsService, ScBizFxContextService, ScBizFxView, ScBizFxAction, ScBizFxActionMessage } from '@sitecore/bizfx';


@Component({
  selector: 'app-counter',
  templateUrl: './counter.component.html',
  styleUrls: ['./counter.component.css']
})
export class CounterComponent implements OnInit {

    /**
    * Defines the iew
    */
   @Input() view: ScBizFxView;
   /**
     * @ignore
     */
   action: ScBizFxAction;
 

   constructor(
    private viewsService: ScBizFxViewsService,
    private bizFxContext: ScBizFxContextService,
    private route: ActivatedRoute) {
  }

  ngOnInit() {
  }

}
