import { Component, Input, Output, EventEmitter, forwardRef, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

import { NgbInputDatepicker, NgbDateStruct, NgbTimeStruct, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';

import { ScBizFxProperty, NgbDateScParserFormatter } from '@sitecore/bizfx';

/**
 * BizFx Action Date Time `Component`
 */
@Component({
  selector: 'sc-bizfx-actionproperty-datetime',
  styles: [`
    :host {
      display: flex;
    }

    .date-box {
      background-color: white;
    }

    .date-picker {
      flex: 1 1 100%;
    }

    .date-picker .input-group {
      width: auto;
    }

    .time-picker {
      flex: 1 1 auto;
      margin-left: 15px;
    }

    .date-picker-expand {
      height: 260px;
    }
  `],
  templateUrl: './sc-bizfx-actionproperty-datetime.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ScBizFxActionPropertyDateTimeComponent),
      multi: true
    },
    { provide: NgbDateParserFormatter, useClass: NgbDateScParserFormatter }
  ]
})

export class ScBizFxActionPropertyDateTimeComponent implements ControlValueAccessor, OnInit {
  /**
     * Defines the date
     */
  @Input() dateTime: Date;
  /**
     * Defines the `ScBizFxProperty` to be render
     */
  @Input() property: ScBizFxProperty;
  /**
     * @ignore
     */
  date: NgbDateStruct;
  /**
     * @ignore
     */
  time: NgbTimeStruct;
  /**
     * @ignore
     */
  isExpanded: boolean;

  /**
     * @ignore
     */
  ngOnInit(): void {
    const dateTime = new Date(this.property.Value);
    this.date = { day: dateTime.getDate(), month: dateTime.getMonth() + 1, year: dateTime.getFullYear() };
    this.time = { hour: dateTime.getHours() || 0, minute: dateTime.getMinutes() || 0, second: dateTime.getSeconds() || 0 };
    this.isExpanded = false;
  }

  /**
     * @ignore
     */
  propagateChange: any = () => { };

  /**
     * @ignore
     */
  writeValue() { }

  /**
     * @ignore
     */
  registerOnChange(fn) {
    this.propagateChange = fn;
  }

  /**
     * @ignore
     */
  registerOnTouched() { }

  /**
    * Hides/Shows the component calendar selector
    */
  toggleCalendar(calendar: NgbInputDatepicker) {
    this.isExpanded = !this.isExpanded;
    calendar.toggle();
  }

  /**
     * @ignore
     */
  onChange(expand) {
    const {
      date,
      time
    } = this;

    if (date && time) {
      const {
        day,
        month,
        year
      } = date;

      const {
        hour,
        minute,
        second
      } = time;

      this.dateTime = new Date(year, (month - 1), day, hour, minute, second);
    } else if (date && !time) {
      this.time = { hour: 0, minute: 0, second: 0 };
    } else {
      this.dateTime = null;
    }

    if (expand) {
      this.isExpanded = !this.isExpanded;
    }

    this.propagateChange(this.dateTime);
  }
}
