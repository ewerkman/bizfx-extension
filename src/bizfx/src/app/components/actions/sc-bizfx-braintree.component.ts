import { Component, Input, AfterViewInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { ScBizFxView } from '@sitecore/bizfx';

 /**
 * @ignore
 */
declare var braintree: any;

/**
 * BizFx Braintree `Component`
 */
@Component({
    selector: 'sc-bizfx-braintreeview',
    template: `<form id="checkout" method="post" action="/checkout">
                    <div id="payment-form"></div>
                    <button scIconButton="secondary" id="submit" type="submit">
                      <sc-icon icon="sort_up_down2" size="small"></sc-icon>
                    </button>
                </form>`
})

export class ScBizFxBraintreeComponent implements AfterViewInit {
    /**
    * Defines the view
    */
    @Input() view: ScBizFxView;
    /**
    * Defines the action form that maps to the view
    */
    @Input() actionForm: FormGroup;
    /**
     * @ignore
     */
    token: string;

    /**
    * @ignore
    */
    ngAfterViewInit(): void {
        if (this.view === undefined) { return; }
        this.token = this.view.Properties.filter(p => p.Name === 'ClientToken')[0].Value;
        const self = this;

        braintree.setup(this.token, 'dropin', {
            container: 'payment-form',
            paymentMethodNonceReceived: function (event, nonce) {
                if (nonce.length > 0) {
                    self.view.Properties.filter(p => p.Name === 'PaymentMethodNonce')[0].Value = nonce;
                    self.actionForm.controls['PaymentMethodNonce'].setValue(nonce);
                }
            }
        });
    }
}

