declare module "Osiris"{
    export class Blob extends DataClass{
        static getBlob(id: string): Blob;
        constructor(group?: string);
        data: object;
        getId(): string;
        getPath(path: string): object;
        setPath(path: string, data: object, canAdd?: boolean, canChangeType?: boolean): void;
        validateData(schema: string): boolean;
        validatePath(schema: string, path: string): boolean;
        applyEvent(method: string, payload: object): void;
    }
    export class DataClass{
        static getData(id: string): DataClass;
        static inherit<T>(classObj: (...args: any[])=>T, methods: string[], isSealed?: boolean): void;
        private static wrap(id: string): DataClass;
        constructor(group?: string);
        getId(): string;
        applyEvent(method: string, payload: object): void;
    }
    export namespace Dice{
        function evaluate(formula: string): object;
        function setEvaluator(evaluator: (formula: string) => object): void;
        function rollDice(count: number, size: number): number[];
        function requestRoll(formula: string, callback: (result: object)=>void): void;
    }
    export class Group{
        constructor(groupName: string, baseName: string, isSealed?: boolean);
        addMethod(methodName: string, method: (data: DataClass, event: object)=>void): void;
        finish(): void;
    }
    export namespace Logging{
        function log(...args: any[]): void;
        function logError(...args: any[]): void;
    }
    export namespace Map{
        class Entity extends Blob{
            constructor(group?: string); //if current user is not gm sets them as owner automatically
            name: string;
            isToken: boolean;
            angle: number;
            getSize(): [number, number, number];
            setSize(x: number, y: number, z: number): void;
            getPosition(): [number, number, number];
            setPosition(x: number, y: number, z: number): void;
            isOwnedBy(userId?: string): boolean; // if userId is absent use current user id;
            addOwner(userId: string): void; // gm only?
            removeOwner(userId: string): boolean; // gm only?
            listOwners(): string[]; // gm only?
        }
        class Map extends Blob{
            constructor(group?: string);
            name: string;
            isFogOfWarEnabled: boolean;
            getCell(x: number, y: number, z?: number, w?: number): number;
            setCells(coords: number[][], value: number): void;
            getEntity(id: string): Entity | undefined;
            listEntities(): Entity[];
            listTokens(): Entity[];
            addEntity(entity: Entity): void; // if current user is not gm sets them as owner automatically
            removeEntity(id: string): void; // owner and gm only
        }
    }
}
