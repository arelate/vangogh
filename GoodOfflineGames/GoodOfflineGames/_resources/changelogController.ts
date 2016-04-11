import * as selectionController from "./selectionController";

interface IShowDelegate 
{
    (element: Element);
}

export interface IChangelogController {
    show: IShowDelegate;
}

export class ChangelogController implements IChangelogController {
    
    selectionController: selectionController.ISelectionController;
    
    public constructor(selectionController: selectionController.ISelectionController) {
        this.selectionController = selectionController;
    }
    
    show: IShowDelegate = function(element: Element) {
        if (element &&
            element.parentNode) {
            var changelogContent = this.selectionController.getFromContainer(element.parentNode, ".changelogContent");
            changelogContent.classList.toggle("hidden");
        }
    }
}