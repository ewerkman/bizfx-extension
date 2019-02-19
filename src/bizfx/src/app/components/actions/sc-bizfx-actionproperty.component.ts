import { Component, Input, ElementRef, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

import { ScBizFxProperty } from '@sitecore/bizfx';

/**
 * BizFx Action Property `Component`
 */
@Component({
  selector: 'sc-bizfx-actionproperty',
  styleUrls: ['./sc-bizfx-actionproperty.component.css'],
  templateUrl: './sc-bizfx-actionproperty.component.html',
})

export class ScBizFxActionPropertyComponent implements AfterViewInit {
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
    this.resetSelectedIndex();
  }

  /**
     * Helper method
     *
     * @returns a `AbstractControl` that maps to the `ScBizFxProperty` by name.
     */
  get propertyControl() {
    const control = this.actionForm.get(this.property.Name);
    return control ? control : this.actionForm.controls[this.property.Name];
  }

  /**
     * Validates the control that maps to the property
     */
  get isValid() {
    if (this.property !== undefined && this.property.UiType === 'Autocomplete') {
      const inputElement = (<HTMLInputElement>document.getElementsByClassName('ng-autocomplete-input')[0]);
      if (inputElement !== undefined && inputElement.value.length > 0) {
        const input = this.propertyControl;
        input.setValue(inputElement.value);
        input.markAsTouched();
      }
    }

    const control = this.propertyControl;
    return control.valid || control.pristine;
  }

  /**
     * Calls `Angular2Csv` to generate and donwload a csv file
     */
  downloadCsv() {
    return new Angular2Csv(JSON.parse(this.property.Value), `${this.property.Name}`);
  }

  /**
     * @ignore
     */
  /*
   * Solves IE/Edge preselecting the first option, forcing the user to
   * change to something else and back, if they wanted the first
   * option.
   */
  resetSelectedIndex() {
    if (this.property.UiType === 'SelectList') {
      const ne = this.el.nativeElement as HTMLElement;
      const selectEl = ne.querySelector('select') as HTMLSelectElement;

      if (selectEl && this.property.Value === '') {
        selectEl.selectedIndex = -1;
      }
    }
  }
}
