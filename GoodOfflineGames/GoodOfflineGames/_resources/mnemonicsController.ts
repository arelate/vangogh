import * as selectionController from "./selectionController";

interface IInitializeForContainerDelegate {
    (container: Element);
}

export interface IMnemonicsController {
    initializeForContainer: IInitializeForContainerDelegate;
}

export class MnemonicsController implements IMnemonicsController {
    
    mnemonicsVisible: Boolean;
    selectionController: selectionController.ISelectionController;
    
    public constructor(selectionController: selectionController.ISelectionController) {
        this.mnemonicsVisible = false;
        this.selectionController = selectionController;
    }
    
    public initializeForContainer = function(container: Element) 
    {
        let accessKeyElements = this.selectionController.getAllFromContainer(container, "*[accessKey]");
        for (var ii = 0; ii < accessKeyElements.length; ii++) {
            var mnemonic = document.createElement("div");
            mnemonic.className = "mnemonic hidden";
            mnemonic.textContent = accessKeyElements[ii].accessKey;
            accessKeyElements[ii].appendChild(mnemonic);
        }

        var that = this;

        window.addEventListener("keydown", function(e) {
            if (!e.altKey || that.mnemonicsVisible) return;

            that.mnemonicsVisible = true;

            var mnemonics = that.selectionController.getAll(".mnemonic");
            for (var ii = 0; ii < mnemonics.length; ii++) {
                mnemonics[ii].classList.remove("hidden");
            }

        });

        window.addEventListener("keyup", function(e) {
            if (e.keyCode !== 18) return;

            that.mnemonicsVisible = false;

            var mnemonics = that.selectionController.getAll(".mnemonic");
            for (var ii = 0; ii < mnemonics.length; ii++) {
                mnemonics[ii].classList.add("hidden");
            }
        }); 
    }
}
