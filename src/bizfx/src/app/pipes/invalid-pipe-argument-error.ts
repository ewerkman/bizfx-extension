import {Type, ɵstringify as stringify} from '@angular/core';

/**
 * @ignore
 */
export function InvalidPipeArgumentError(type: Type<any>, value: Object) {
  return Error(`InvalidPipeArgument: '${value}' for pipe '${stringify(type)}'`);
}
