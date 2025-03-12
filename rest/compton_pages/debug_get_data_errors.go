package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"path/filepath"
)

func DebugGetDataErrors(id string) (compton.PageElement, error) {

	p := compton.IframeExpandContent("get-data-errors", "Get-Data Errors")

	hasErrors := false

	typeErrorsDir, err := pathways.GetAbsRelDir(vangogh_integration.TypeErrors)
	if err != nil {
		return nil, err
	}

	for pt := range vangogh_integration.AllProductTypes() {
		if pt == vangogh_integration.UnknownProductType {
			continue
		}

		rdx, err := redux.NewReader(filepath.Join(typeErrorsDir, pt.String()),
			vangogh_integration.TypeErrorMessageProperty,
			vangogh_integration.TypeErrorDateProperty)
		if err != nil {
			return nil, err
		}

		if msg, ok := rdx.GetLastVal(vangogh_integration.TypeErrorMessageProperty, id); ok && msg != "" {
			frow := compton.Frow(p)
			frow.PropVal(pt.String(), msg)
			if dt, sure := rdx.GetLastVal(vangogh_integration.TypeErrorDateProperty, id); sure && dt != "" {
				frow.PropVal("Date", dt)
			}
			p.Append(frow)
			hasErrors = true
		}

	}

	if !hasErrors {
		p.Append(compton.FICenter(p, compton.Fspan(p, "No errors found").ForegroundColor(color.Gray)))
	}

	return p, nil
}
