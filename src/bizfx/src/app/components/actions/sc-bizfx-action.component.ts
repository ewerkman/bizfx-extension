import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/takeWhile';
import { Component, OnInit, Inject, Output, EventEmitter, OnDestroy } from '@angular/core';
import { FormArray, FormGroup, FormControl, FormBuilder, AbstractControl } from '@angular/forms';

import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

import { ScDialogService } from '@speak/ng-bcl/dialog';

import { ScBizFxContextService, ScBizFxViewsService, getLocaleNumberSymbol, NumberSymbol, parseDecimalNumber } from '@sitecore/bizfx';
import { ScBizFxView, ScBizFxProperty, ScBizFxAction, ScBizFxActionMessage } from '@sitecore/bizfx';
import { getPropertyValidators } from '@sitecore/bizfx';

/**
 * BizFx Action `Component`.
 */
@Component({
  selector: 'sc-bizfx-action',
  templateUrl: './sc-bizfx-action.component.html',
  styleUrls: ['./sc-bizfx-action.component.css']
})

export class ScBizFxActionComponent implements OnInit, OnDestroy {
  /**
     * @ignore
     */
  @Output() submitted: EventEmitter<null> = new EventEmitter();

  /**
     * @ignore
     */
  view: ScBizFxView;
  /**
     * @ignore
     */
  actionForm: FormGroup;
  /**
     * @ignore
     */
  action: ScBizFxAction;
  /**
     * @ignore
     */
  data: ScBizFxView;
  /**
     * @ignore
     */
  messages: ScBizFxActionMessage[];
  /**
     * @ignore
     */
  loadingView = false;
  /**
       * @ignore
       */
  private alive = true;

  /**
    * @ignore
    */
  constructor(
    public bizFxContext: ScBizFxContextService,
    private viewsService: ScBizFxViewsService,
    private dialogService: ScDialogService,
    private fb: FormBuilder) {
    this.actionForm = new FormGroup({});
  }

  /**
    * @ignore
    */
  ngOnInit(): void {
    if (this.data.Actions[0] !== null && this.data.Actions[0] !== undefined) {
      this.action = this.data.Actions[0];
    }

    this.getActionView();

    this.viewsService.actionErrorsAnnounced$
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
     * @returns a 'FormArray' named 'Grid'
     */
  get grid(): FormArray {
    return this.actionForm.get('Grid') as FormArray;
  }

  /**
     * Handles the submit click
     */
  submitAction(): void {
    this.prepareSaveView();
    this.doAction();
  }

  /**
     * Handles the cancel click
     */
  cancelAction(): void {
    this.dialogService.close();
  }

  /**
   * Helper method
   *
   * Calls BizFx Views service `getView` method to get the action's view.
   */
  protected getActionView() {
    this.loadingView = true;
    this.viewsService.getView(this.data.Name, this.data.EntityId, this.data.Action, this.data.ItemId, this.data.EntityVersion)
      .then(view => {
        this.view = view;
        this.buildForm();
        this.loadingView = false;

        if (!this.action) {
          this.action = new ScBizFxAction(view.Action, view.EntityId, view.Action);
        }
      });
  }

  /**
   * Helper method
   *
   * Calls BizFx Views service `doAction` method to execute the action.
   */
  protected doAction() {
    this.viewsService
      .doAction(this.view)
      .then(actionResult => {
        this.view.ActionResult = actionResult;

        if (actionResult.NextView) {
          this.view = actionResult.NextView;
          this.buildForm();

          if (this.view.Properties.filter(p => p.IsHidden).length === this.view.Properties.length
            && this.view.ChildViews.length === 0) {
            this.submitAction();
          }
        } else if (actionResult.ResponseCode === 'Ok') {
          this.submitted.emit();
          this.dialogService.close();
        }
      });
  }

  /**
    * Helper method
    *
    * Builds the `FormGroup` based on the action's view properties.
    */
  protected buildForm() {
    if (this.view.Name === '' ||
      this.view.Name === 'null' ||
      this.view.Name === null ||
      this.view.Name === undefined) {
      return;
    }

    const group = this.buildGroup(this.view.Properties);

    if (this.view.UiHint === 'Grid') {
      const childrenGroups = new FormArray([]);
      this.view.ChildViews.forEach((child, index) => childrenGroups.push(new FormGroup(this.buildGroup(child.Properties))));
      group['Grid'] = childrenGroups;
    }

    this.actionForm = new FormGroup(group);
  }

  /**
   * Helper method
   *
   * Builds the `FormGroup` based on a collection of `ScBizFxProperty`.
   */
  protected buildGroup(properties: ScBizFxProperty[]): any {
    const group: any = {};

    properties.forEach(property => {
      const validators = getPropertyValidators(property, this.bizFxContext.language);
      const control = new FormControl({ value: property.Value || null, disabled: property.IsReadOnly }, validators);
      this.transformProperty(property, control);
      group[property.Name] = control;
    });

    return group;
  }

  /**
   * Helper method
   *
   * Maps the `FormGroup` controls to the action's view properties.
   */
  protected prepareSaveView() {
    this.view.Properties.map(property => {
      const control = this.actionForm.get(property.Name);
      this.mapProperty(property, control ? control : this.actionForm.controls[property.Name]);
    });

    if (this.view.UiHint === 'Grid') {
      this.view.ChildViews.forEach((child, index) => {
        const control = this.grid.at(index);
        if (control) { child.Properties.map(property => this.mapProperty(property, control.get(property.Name))); }
      });
    }
  }

  /**
   * Helper method
   *
   * Maps an `AbstractControl` to a `ScBizFxProperty`.
   */
  protected mapProperty(property: ScBizFxProperty, control: AbstractControl): any {
    if (!control) {
      return;
    }

    switch (property.OriginalType) {
      case 'System.DateTimeOffset':
        if (Date.parse(control.value)) {
          property.Value = (control.value as Date).toISOString();
        }

        break;
      case 'System.String':
        if (control.value != null) {
          property.Value = control.value.trim();
        }

        break;
      case 'System.Decimal':
        const currencyDecimal = getLocaleNumberSymbol(this.bizFxContext.language, NumberSymbol.CurrencyDecimal);
        const currencyGroup = getLocaleNumberSymbol(this.bizFxContext.language, NumberSymbol.CurrencyGroup);
        const options = [currencyGroup, currencyDecimal];
        property.Value = parseDecimalNumber(control.value, options);

        break;
      default:
        if (control.value != null) {
          property.Value = control.value;
        }

        break;
    }
  }

  /**
   * Helper method
   *
   * Transforms a `ScBizFxProperty` to a `FormControl`.
   */
  protected transformProperty(property: ScBizFxProperty, control: FormControl) {
    switch (property.OriginalType) {
      case 'System.DateTimeOffset':
        const date = new Date(property.Value);
        control.setValue(date);

        break;
      case 'System.Boolean':
        let checked = false;
        if (property.Value !== undefined && property.Value !== null) {
          checked = property.Value.toLowerCase() === 'true';
        }
        control.setValue(checked);

        break;
      default:
        break;
    }
  }
}
