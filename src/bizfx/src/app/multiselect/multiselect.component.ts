import { Component, OnInit, Input, ElementRef, AfterViewInit, ChangeDetectorRef } from '@angular/core';

import { ScBizFxProperty } from '@sitecore/bizfx';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-multiselect',
  templateUrl: './multiselect.component.html',
  styleUrls: ['./multiselect.component.css']
})

export class MultiselectComponent implements AfterViewInit {

  /**
     * Defines the property to be render
     */
  @Input() property: ScBizFxProperty;
  /**
     * Defines the form group that maps to the action's view
     */
  @Input() actionForm: FormGroup;

  /**
   * @ignore
   */
  constructor(private el: ElementRef, private cd: ChangeDetectorRef) { }

  /**
    * @ignore
    */
  ngAfterViewInit(): void {

  }

}
