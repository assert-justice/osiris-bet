import { Blob, Logging } from "Osiris";
import { Class } from "utils";

export class Actor extends Blob{
    constructor(){
        super("Actor");
        this.data = {
            display_name: "mysterious figure",
            abilities: {
                str: 10,
                dex: 10,
                con: 10,
                int: 10,
                wis: 10,
                cha: 10,
            }
        }
    }
    get displayName(){return this.getPath("display_name");}
    set displayName(newName){this.setPath("display_name", newName, true);}
    get portrait(){return this.getPath("portrait_filename");}
    set portrait(filename){this.setPath("portrait_filename", filename, true);}
    get token(){return this.getPath("token_filename");}
    set token(filename){this.setPath("token_filename", filename, true);}
    damage(payload){
        // Logging.log(payload);
        Logging.log(`${this.displayName} takes damage!`);
        for (const element of payload) {
            let resistance = this.getPath(`resistances/${element.type}`) ?? 0;
            let damage = element.value - resistance;
            if(resistance > 0){
                Logging.log(`oochie ma goochie for ${damage} ${element.type} damage, ${element.value - damage} resisted.`);
            }
            else Logging.log(`oochie ma goochie for ${element.value} ${element.type} damage.`);
        }
    }
}

// Class.registerClass(Actor, "Blob", ["damage"]);