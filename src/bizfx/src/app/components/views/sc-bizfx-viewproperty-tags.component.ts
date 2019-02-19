import { Component, Input, OnInit } from '@angular/core';
import { ScBizFxProperty } from '@sitecore/bizfx';

/**
 * BizFx View Property Tags `Component`
 *
 */
@Component({
    selector: 'sc-bizfx-viewproperty-tags',
    template: `<div *ngFor="let tag of tags">{{tag}}</div>`
})

export class ScBizFxViewPropertyTagsComponent implements OnInit {
    /**
    * Defines the property to be render
    */
    @Input() property: ScBizFxProperty;
    /**
        * @ignore
        */
    tags: any[];

    /**
    * @ignore
    */
    ngOnInit(): void {
        if (this.property.Value !== null && this.property.Value !== undefined) {
            this.tags = JSON.parse(this.property.Value);
        }
    }
}
