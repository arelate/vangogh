import * as templateController from "./templateController";

interface IBindModelDelegate {
    (template: string, model: any): string;
}

interface ICreateViewDelegate {
    (model: any, getViewModelDelegate: any, templateId: string): string;
}

export interface IViewController {
    bindModel: IBindModelDelegate;
    createView: ICreateViewDelegate;
}

export class ViewController implements IViewController {

    templateController: templateController.ITemplateController;

    public constructor(templateController: templateController.ITemplateController) {
        this.templateController = templateController;
    }

    public bindModel = function(template: string, model: any): string {
        let result = template;
        for (var property in model) {
            var replacedProperty = "{{" + property + "}}";
            while (result.indexOf(replacedProperty) > -1)
                result = result.replace(replacedProperty, model[property]);
        }
        return result;
    }

    public createView = function(model: any, getViewModelDelegate: any, templateId: string): string {
        let view = "";
        let viewModel = getViewModelDelegate(model);
        let template = this.templateController.getTemplate(templateId);
        if (viewModel === undefined) view = "(cannot create viewModel)";
        if (template === "") view = "(cannot find template)";
        view = this.bindModel(template, viewModel);
        return view;
    }
}