import * as selectionController from "./selectionController";

interface IGetTemplateDelegate {
    (string): string;
}

interface IGetKnownTemplatesDelegate {
    (): Array<string>;
}

interface IResolveReferencesDelegate {
    (string): string;
}

export interface ITemplateController {
    getTemplate: IGetTemplateDelegate;
    getKnownTemplates: IGetKnownTemplatesDelegate;
    resolveReferences: IResolveReferencesDelegate;
}

export class TemplateController implements ITemplateController {

    templatesContainer: Element;
    selectionController: selectionController.ISelectionController;

    public constructor(selectionController: selectionController.ISelectionController) {
        this.selectionController = selectionController;
        this.templatesContainer =
            this.selectionController &&
            this.selectionController.getById("templates");
    }

    public getKnownTemplates = function(): Array<string> {
        var knownTemplates = new Array<string>();
        var templates = this.selectionController.getAll("template");
        for (var ii = 0; ii < templates.length; ii++) {
            var t = templates[ii];
            if (t && t.id) knownTemplates.push(t.id);
        }
        return knownTemplates;
    }

    public resolveReferences = function(template: string): string {
        let knownTemplates = this.getKnownTemplates();
        knownTemplates.forEach(knownTemplate => {
            let replacedTemplate = "[[" + knownTemplate + "]]";
            while (template.indexOf(replacedTemplate) > -1)
                template = template.replace(replacedTemplate, knownTemplate);
        });
        return template;
    }

    public getTemplate = function(id: string): string {
        var template = this.selectionController.getFromContainer(
            this.templatesContainer,
            "#" + id);
        var templateContent = template ? template.innerHTML : "";
        return this.resolveReferences(templateContent);
    }
}