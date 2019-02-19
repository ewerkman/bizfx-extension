import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormArray, FormBuilder, FormControl, Validators, AbstractControl } from '@angular/forms';

import { ScBizFxContextService, ScBizFxProperty, ScBizFxView, getPropertyValidators, deepClone } from '@sitecore/bizfx';

/**
 * BizFx Action Grid `Component`
 */
@Component({
    selector: 'sc-bizfx-actiongrid',
    templateUrl: './sc-bizfx-actiongrid.component.html'
})

export class ScBizFxActionGridComponent implements OnInit {
    /**
     * Defines the view to be render
     */
    @Input() view: ScBizFxView;
    /**
     * Defines the form tha maps to the view
     */
    @Input() actionForm: FormGroup;
    /**
     * Defines the grid tha maps to the view
     */
    @Input() grid: FormArray;
    /**
     * @ignore
     */
    allowAdd = true;
    /**
     * @ignore
     */
    headers: ScBizFxProperty[] = [];
    /**
     * @ignore
     */
    children: ScBizFxView[] = [];

    /**
     * @ignore
     */
    constructor(
        public bizFxContext: ScBizFxContextService,
        private fb: FormBuilder) {
    }

    /**
     * @ignore
     */
    ngOnInit(): void {
        if (this.view === undefined || !this.view.ChildViews || this.view.ChildViews.length === 0) { return; }

        this.headers = this.view.ChildViews[0].Properties.filter(p => !p.IsHidden);
        this.children = this.view.ChildViews;

        const propertyAllowAdd = this.view.Properties.filter(p => p.Name.toLowerCase() === 'allowadd')[0];
        if (propertyAllowAdd !== undefined && propertyAllowAdd.Value === 'false') {
            this.allowAdd = false;
        }
    }

    /**
     * Helper method
     *
     * @returns a `AbstractControl` that maps to the `ScBizFxProperty` by name.
     */
    protected propertyControl(groupIndex: number, property: ScBizFxProperty): AbstractControl {
        return this.grid.controls[groupIndex].get(property.Name);
    }

    /**
     * Validates a grid's row
     */
    isValid(groupIndex: number, property: ScBizFxProperty) {
        const control = this.propertyControl(groupIndex, property);

        if (property !== undefined && property.UiType === 'Autocomplete') {
            const inputElement = (<HTMLInputElement>document.getElementsByClassName('ng-autocomplete-input')[0]);
            if (inputElement !== undefined && inputElement.value.length > 0) {
                control.setValue(inputElement.value);
                control.markAsTouched();
            }
        }

        return control.valid || control.pristine;
    }

    /**
     * Adds a new row to the grid
     */
    addRow(): void {
        const clone = deepClone(this.view.ChildViews[0]);
        this.children.push(clone);

        const cloneGroup: any = {};
        clone.Properties.forEach(property => {
            const validators = getPropertyValidators(property, this.bizFxContext.language);
            cloneGroup[property.Name] = new FormControl({ value: property.Value || null, disabled: property.IsReadOnly }, validators);
        });
        this.grid.push(this.fb.group(cloneGroup));
    }

    /**
     * Removes a row from the grid
     */
    removeRow(index: number): void {
        if (index <= -1) { return; }

        this.children.splice(index, 1);
        this.grid.removeAt(index);
    }
}
