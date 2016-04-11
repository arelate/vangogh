interface IGetByIdDelegate {
    (id: string): Element;
}

interface IGetFromContainerDelegate {
    (container: Element, selector: string): Element;
}

interface IGetAllDelegate {
    (selector: string): NodeList;
}

interface IGetAllFromContainer {
    (container: Element, selector: string): NodeList;
}

export interface ISelectionController {
    getById: IGetByIdDelegate;
    getFromContainer: IGetFromContainerDelegate;
    getAll: IGetAllDelegate;
    getAllFromContainer: IGetAllFromContainer;
}

export class SelectionController implements ISelectionController {

    public getById: IGetByIdDelegate =
    function(id: string): Element {
        return document.getElementById(id);
    }

    public getFromContainer: IGetFromContainerDelegate =
    function(container: Element, selector: string): Element {
        return container && container.querySelector(selector);
    }

    public getAll: IGetAllDelegate =
    function(selector: string): NodeList { 
        return document.querySelectorAll(selector); 
    }

    public getAllFromContainer: IGetAllFromContainer =
    function(container: Element, selector: string): NodeList { 
        return container && container.querySelectorAll(selector); 
    }
}