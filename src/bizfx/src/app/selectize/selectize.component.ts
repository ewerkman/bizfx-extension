import { Component, Input, OnInit} from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ScBizFxProperty } from '@sitecore/bizfx';

@Component({
  selector: 'app-selectize',
  templateUrl: './selectize.component.html',
  styleUrls: ['./selectize.component.css']
})

export class SelectizeComponent implements OnInit {

  /**
 * Defines the form group that maps the action's view
 */
  @Input() actionForm: FormGroup;
  /**
   * Defines the view property to be render
   */
  @Input() property: ScBizFxProperty;

  config: any = {
    plugins: ['dropdown_direction', 'remove_button'],
    dropdownDirection: 'down',
    labelField: 'DisplayName',
    valueField: 'Name',
    searchField: ['Name'],
    maxItems: 10
  };

  items = [];

  placeholder = 'Placeholder...';

  options = [];


  constructor() { }

  ngOnInit() {

    this.items = JSON.parse(this.property.Value);

    var availableSelectionsPolicy : any;

    availableSelectionsPolicy = this.property.Policies
            .find(p => p['@odata.type'] === '#Plugin.Sample.Notes.Policies.SelectizeConfigPolicy');
    if(availableSelectionsPolicy)
    {
      this.options = availableSelectionsPolicy.Options;
      this.placeholder = availableSelectionsPolicy.Placeholder;
    }
  }

  onInputBlurred(): void {
    this.actionForm.controls[this.property.Name].setValue(JSON.stringify(this.items));
  }
}
