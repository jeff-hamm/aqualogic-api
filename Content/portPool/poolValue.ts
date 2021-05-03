export interface PoolValue {
    id: number;
    name: PoolValueName;
    text: string;
    value: any;
    unit: ValueUnit | null;
    modified: boolean;
}
export enum PoolValueName {
    Unknown,
    PoolTemp,
    SpaTemp,
    AirTemp,
    PoolChlorinator,
    SpaChlorinator,
    SaltLevel,
    CheckSystem,
    HeatPump,
    HeaterAuto
}

export enum ValueUnit {
    DegreesF,
    DegreesC,
    Percentage,
    GramsLiter,
    PartsPerMillion,
    String,
    AutoMode
}