declare module "Osiris"{
    export namespace Logging{
        function log(...args: any[]): void;
        function logError(...args: any[]): void;
    }
    export class Blob{
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
    export namespace Dice{
        function roll(formula: string, callback: () => DiceGroup[]): void;
        interface DiceGroup{
            size: number;
            rolls: number[];
        }
    }
    export class Group{
        constructor(groupName: string, baseName: string, isSealed?: boolean);
        addMethod(methodName: string, method: (blob: Blob, event: object)=>void): void;
        finish(): void;
    }
    export namespace Map{
        class Entity extends Blob{
            constructor(map: Map); //if current user is not gm sets them as owner automatically
            name: string;
            position: [number, number, number];
            isOwnedBy(userId?: string): boolean; // if userId is absent use current user id;
            addOwner(userId: string): void; // gm only?
            removeOwner(userId: string): boolean; // gm only?
            listOwners(): string[]; // gm only?
            free(): void; // remove from map and delete, owner and gm only
            isToken: boolean;
        }
        class Map extends Blob{
            constructor();
            name: string;
            size: [number, number, number];
            isFogOfWarEnabled: boolean;
            setCells(coords: [number, number, number, number][], value: number): void;
            getCell(coord: [number, number, number, number]): number;
            listEntities(pred?: (ent: Entity)=>boolean): Entity[];
            getEntity(id: string): Entity;
        }
    }
}
