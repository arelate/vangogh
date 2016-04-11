import * as selectionController from "./selectionController";

interface IInitializeDelegate {
    ();
}

export interface IInformationController {
    initialize: IInitializeDelegate;
}

export class InformationController implements IInformationController {

    selectionController: selectionController.ISelectionController;
    informationContainer: Element;

    public constructor(selectionController: selectionController.ISelectionController) {
        this.selectionController = selectionController;
        this.informationContainer =
            selectionController &&
            selectionController.getById("infoContainer");
    }

    public initialize: IInitializeDelegate = function() {

        let classes = [
            "product",
            "product owned",
            "product owned validated",
            "product owned validationError",
            "product owned updated",
            "product wishlisted",
            "product bundle",
            "product bundle partOwned",
            "product bundle partOwned updated",
            "product bundle owned",
            "product bundle owned updated",
            "product bundle wishlisted",
            "product dlc",
            "product dlc owned",
            "product dlc owned validated",
            "product dlc owned validationError",
            "product dlc wishlisted"
        ];
        var titles = [
            "Avail. products",
            "Owned products",
            "Validated products",
            "Validation error",
            "Upd. products",
            "Wishlisted products",
            "Bundles",
            "Part. owned bundles",
            "Upd. part. owned bundles",
            "Owned bundles",
            "Upd. bundles",
            "Wishlisted bundles",
            "DLCs",
            "Owned DLCs",
            "Validated DLCs",
            "Validation error DLCs",
            "Wishlisted DLCs"
        ];

        if (classes.length !== titles.length) {
            alert("Inconsistent information data!");
            return;
        }

        for (var i = 0; i < classes.length; i++) {
            var count = document.getElementsByClassName(classes[i]).length;
            var infoEntity = document.createElement("div");
            infoEntity.className = "info";

            var entityClass = document.createElement("span");
            entityClass.className = classes[i];
            entityClass.textContent = titles[i];
            infoEntity.appendChild(entityClass);

            var entityCount = document.createElement("span");
            entityCount.className = "product count";
            entityCount.textContent = count.toString();
            infoEntity.appendChild(entityCount);

            this.informationContainer.appendChild(infoEntity);
        }
    }
}