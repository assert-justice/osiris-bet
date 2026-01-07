import { Group, Logging } from "Osiris";

export class Class{
    static registerClass(classObj, baseName, methodNames, isSealed = false){
        const name = classObj.name;
        // Logging.log("name: ", name);
        const prototype = classObj.prototype;
        const group = new Group(name, baseName, isSealed);
        for (const methodName of methodNames) {
            group.addMethod(methodName, (obj, e)=>Object.setPrototypeOf(obj, prototype)[methodName](e));
        }
        group.finish();
    }
}
