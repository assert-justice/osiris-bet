// import { Logging, Blob } from "Osiris";
import { Actor } from "actor";
import { Dice, Logging } from "Osiris";
// import { Blob, Logging } from "Osiris";

// class Actor extends Blob{}
// class Hero extends Actor{}

export function init(){
    // Blob.registerClass(Actor, []);
    // Blob.registerClass(Hero, []);
    // Blob.registerClass(Actor, "Blob", []);
    // let hero = new Hero();
    // hero.data = {fuck:"you"};
    // Logging.log(hero instanceof Blob)
    // let b = new Blob();
    // b.data = {fuck: "this"}
    // let azar = new Actor();
    // azar.displayName = "Azar";
    // Logging.log(Actor.prototype)
    // for (const key in Actor.prototype.displayName) {
    //     Logging.log(key);
    // }
    // if(azar.validate("actor")) Logging.log("actor validated!");
    // if(azar.validatePath("abilities", "abilities")) Logging.log("actor path validated!");
    // azar.setPath("resistances/fire", 5, true);
    // azar.applyEvent("damage", [
    //     {type: "slashing", value: 12},
    //     {type: "fire", value: 10},
    // ]);
    // throw "butts";
    // let bf = new Bitfield(8);
    // bf.setBit(0, true)
    // Logging.log(bf.getRange(2, 4));
    // let bf = new Bitfield();
    // bf.setRange(2, 4, 10);
    // bf.setRange(2, 4, 12);
    // bf.setRange(2, 4, 0);
    // let cell = new Map.Cell(0, 0, 0);
    // Logging.log(cell.top.toString());
    // Logging.log(Dice.rollDice(5, 6).join(", "));
    // Dice.setEvaluator((_)=>Dice.rollDice(5, 6));
    Dice.setEvaluator((result)=>Dice.rollDice(5, 6));
    Dice.requestRoll("/r 5d6", (res)=>{Logging.log(res.join(", "));});
    Dice.requestRoll("/r 5d6", (res)=>{Logging.log(res.join(", "));});
    // Dice.setEvaluator((result)=>{});
    // var res = Dice.evaluate("/r 5d6");
    // Logging.log(res.join(", "));
}
