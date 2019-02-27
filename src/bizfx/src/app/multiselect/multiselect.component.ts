import { Component, OnInit, Input, ElementRef, AfterViewInit, ChangeDetectorRef } from '@angular/core';

import { ScBizFxProperty } from '@sitecore/bizfx';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-multiselect',
  templateUrl: './multiselect.component.html',
  styleUrls: ['./multiselect.component.css']
})

export class MultiselectComponent implements AfterViewInit, OnInit {

  ngOnInit(): void {
    this.dropdownList = [
      { item_id: 1, item_text: 'Mumbai' },
      { item_id: 2, item_text: 'Bangaluru' },
      { item_id: 3, item_text: 'Pune' },
      { item_id: 4, item_text: 'Navsari' },
      { item_id: 5, item_text: 'New Delhi' }
    ];
    console.log(JSON.parse(this.property.Value));
    this.selectedItems = JSON.parse(this.property.Value);
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'item_id',
      textField: 'item_text',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 3,
      allowSearchFilter: true
    };
  }

  /**
     * Defines the property to be render
     */
  @Input() property: ScBizFxProperty;
  /**
     * Defines the form group that maps to the action's view
     */
  @Input() actionForm: FormGroup;

  dropdownList = [];
  selectedItems = [];
  dropdownSettings = {};

  /**
   * @ignore
   */
  constructor(private el: ElementRef, private cd: ChangeDetectorRef) { }

  /**
    * @ignore
    */
  ngAfterViewInit(): void {
    
  }

  onItemSelect(item: any) {
    console.log(item);
    //this.property.Value = JSON.stringify(this.selectedItems);
    this.actionForm.controls[this.property.Name].setValue(JSON.stringify(this.selectedItems));
  }
  onSelectAll(items: any) {
    console.log(items);
    this.property.Value = JSON.stringify(this.selectedItems);
  }
}
