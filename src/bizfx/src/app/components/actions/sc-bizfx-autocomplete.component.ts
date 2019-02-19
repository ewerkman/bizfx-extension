import { Component, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Response } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Params } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { FormControl } from '@angular/forms';
import 'rxjs/add/operator/takeWhile';

import { CreateNewAutocompleteGroup, SelectedAutocompleteItem, NgAutocompleteComponent, AutocompleteGroup } from 'ng-auto-complete';

import { ScBizFxView, ScBizFxProperty } from '@sitecore/bizfx';
import {
    ScBizFxContextService,
    ScBizFxBaseService,
    ScBizFxAuthService
} from '@sitecore/bizfx';

/**
 * BizFx Autocomplete `Component`
 */
@Component({
    selector: 'sc-bizfx-autocomplete',
    template: `
    <ng-autocomplete [classes]="['']" [group]="group" (selected)="selected($event)"
            (keyup)="onKey($event)"></ng-autocomplete>
    <div class="ng-dropdown" *ngIf="noResultsFound">
        <span class='dropdown-item'>{{ 'NoSearchResults' | translate }}</span>
    </div>`,
    styles: [`
        ::ng-deep .ng-dropdown {
            border: 1px solid;
            color: #ccc;
        }

        ::ng-deep .dropdown-item.active, .dropdown-item:active {
            background-color: #fff;
        }

        ::ng-deep .ng-autocomplete-placeholder {
            display:none
        }

        ::ng-deep .ng-autocomplete-input {
            display: block;
            width: 100%;
            padding: .75rem 1.25rem;
            font-size: 1rem;
            line-height: 1.4;
            color: #2b2b2b;
            background-color: #fff;
            background-image: none;
            background-clip: padding-box;
            border: 1px solid #ccc;
            border-radius: .17rem;
            box-shadow: inset 0 1px 1px rgba(19,19,19,0.075);
            transition: border-color ease-in-out 0.15s,box-shadow ease-in-out 0.15s;
        }
    `]
})

export class ScBizFxAutocompleteComponent extends ScBizFxBaseService implements OnInit, OnDestroy {
    /**
    * Defines the property to render
    */
    @Input() property: ScBizFxProperty;
    /**
    * @ignore
    */
    @ViewChild(NgAutocompleteComponent) public completer: NgAutocompleteComponent;
    /**
    * @ignore
    */
    public group = [CreateNewAutocompleteGroup('', 'completer', [], { titleKey: 'title', childrenKey: null })];
    /**
    * @ignore
    */
    noResultsFound = false;
    /**
    * @ignore
    */
    private data: any;
    /**
    * @ignore
    */
    private searchTerm: string;
    /**
    * @ignore
    */
    private policyScope: any;
    /**
   * @ignore
   */
    private searchPolicy: any;
    /**
    * @ignore
    */
    private variantSearch = false;
    /**
     * @ignore
     */
    private alive = true;

    /**
    * @ignore
    */
    constructor(
        protected http: HttpClient,
        protected bizFxContext: ScBizFxContextService,
        protected authService: ScBizFxAuthService) {
        super(http, bizFxContext, authService);
    }

    /**
    * @ignore
    */
    ngOnInit(): void {
        if (this.property.Value !== undefined) {
            this.group = [CreateNewAutocompleteGroup(this.property.Value, 'completer', [],
                { titleKey: 'title', childrenKey: null })];
        }

        const propertyPolicy = this.property.Policies.find(p => p.PolicyId === 'EntityType');
        this.policyScope = propertyPolicy && propertyPolicy.Models[0] ? propertyPolicy.Models[0].Name : '';
        const variantModel = propertyPolicy && propertyPolicy.Models[1] ? propertyPolicy.Models[1] : null;
        this.variantSearch = variantModel && variantModel.Name === 'SearchVariants';
        const searchScopePolicy = this.property.Policies
            .find(p => p['@odata.type'] === '#Sitecore.Commerce.Plugin.Search.SearchScopePolicy');
        this.searchPolicy = searchScopePolicy ? searchScopePolicy.Name : '';
    }

    /**
     * @ignore
     */
    ngOnDestroy(): void {
        this.alive = false;
    }

    /**
    * Hanldes the keystrokes and executes the search
    */
    onKey(event: any) {
        this.noResultsFound = false;
        this.searchTerm = event.target.value;
        if (this.searchTerm.length > 3 && this.searchTerm.trim() !== '') {
            this.runSearch();
        }
    }

    /**
     * Executes a search.
     */
    runSearch(): void {
        const termFormat = this.searchTerm + '*';
        const body = {
            'scope': this.searchPolicy,
            'term': termFormat,
            'filter': '',
            'orderBy': '',
            'skip': 0,
            'top': 100
        };
        const headers = this.authService.getHeadersWithAuth();

        this.http
            .put(this.bizFxContext.doSearch(), body, { headers: headers, withCredentials: true })
            .map(res => res)
            .takeWhile(() => this.alive)
            .subscribe(ret => {
                this.data = ret;
                this.updateItems();
            });
    }

    /**
    * Handles the search results.
    */
    updateItems() {
        let searchResults;
        if (this.data !== undefined && this.data['Models'] !== undefined) {
            const children = this.data['Models'][0]['ChildViews'];
            if (this.policyScope === '') {
                searchResults = children;
            } else {
                searchResults = children.filter(c => c.EntityId.startsWith('Entity-' + this.policyScope + '-'));
            }

            const searchValues = [];

            for (const result of searchResults) {
                const properties = result['Properties'];
                const entityId = result['EntityId'];
                const displayName = properties.filter(p => p.Name === 'displayname')[0].Value;
                searchValues.push({ 'title': displayName, 'id': entityId });

                if (this.variantSearch) {
                    const variantIdsProperty = properties.find(p => p.Name === 'variantid' && p.Value !== '');
                    const variantIds = variantIdsProperty && variantIdsProperty.Value ? variantIdsProperty.Value.split('|') : [];
                    const variantNamesProperty = properties.find(p => p.Name === 'variantdisplayname' && p.Value !== '');
                    const variantDisplayNames = variantNamesProperty && variantNamesProperty.Value
                        ? variantNamesProperty.Value.split('|') : [];

                    for (let i = 0; i < variantDisplayNames.length; i++) {
                        searchValues
                            .push({ 'title': '  -> ' + variantDisplayNames[i], 'id': result['EntityId'] + '|' + variantIds[i] });
                    }
                }
            }

            this.noResultsFound = searchValues.length === 0;
            this.group[0].SetValues(searchValues);
            this.completer.TriggerChange();
        } else {
            this.noResultsFound = true;
        }
    }

    /**
    * Handles when a search result is selected.
    */
    selected(item: SelectedAutocompleteItem) {
        setTimeout(function () {
            const inputElement = (<HTMLInputElement>document.getElementsByClassName('ng-autocomplete-input')[0]);
            if (item !== undefined && item !== null
                && item.item !== undefined && item.item !== null
                && item.item.id !== undefined && item.item.id !== null) {
                inputElement.value = <string>item.item.id;
            }
        }, 5);
    }
}
