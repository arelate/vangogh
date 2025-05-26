package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
)

func DebugGetDataRedux(id string) (compton.PageElement, error) {

	p := compton.IframeExpandContent("get-data-redux", "Get-Data Redux")

	hasData := false

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.GetDataProperties()...)
	if err != nil {
		return nil, err
	}

	for pt := range vangogh_integration.AllProductTypes() {
		if pt == vangogh_integration.UnknownProductType {
			continue
		}

		ptId, err := vangogh_integration.ProductTypeId(pt, id)
		if err != nil {
			return nil, err
		}

		frow := compton.Frow(p)

		if msg, ok := rdx.GetLastVal(vangogh_integration.GetDataErrorMessageProperty, ptId); ok && msg != "" {
			frow.PropVal("Error", msg)
			if dt, sure := rdx.GetLastVal(vangogh_integration.GetDataErrorDateProperty, ptId); sure && dt != "" {
				frow.PropVal("Date", dt)
			}
			hasData = true
		}

		if lastUpdated, ok := rdx.GetLastVal(vangogh_integration.GetDataLastUpdatedProperty, ptId); ok && lastUpdated != "" {
			frow.PropVal("Last Updated", lastUpdated)
			hasData = true
		}

		if hasData {
			p.Append(frow)
		}
	}

	if !hasData {
		p.Append(compton.FICenter(p, compton.Fspan(p, "No get data values found").ForegroundColor(color.Gray)))
	}

	return p, nil
}
