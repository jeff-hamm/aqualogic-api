import {State}  from './state'
import {Keys} from './keys' 
import {PoolValue} from './poolValue'

export interface PoolStatus {
    displayText: StatusLine[];
    states: String;
    statesValue: number;
    flashingStates: State;
    flashingStatesValue: number;
    statusValues: PoolValue[];
}

export interface StatusLine {
    text: string;
    flash: boolean;
}
