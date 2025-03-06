package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"slices"
)

func Debug(id string) (compton.PageElement, error) {

	pageTitle := "Product data for " + id

	//TODO: rewrite this with the new data model
	localProducts := make([]vangogh_integration.ProductType, 0)
	slices.Sort(localProducts)

	kvs := make(map[vangogh_integration.ProductType]kevlar.KeyValues, len(localProducts))

	for _, pt := range localProducts {
		if absPtDir, err := vangogh_integration.AbsProductTypeDir(pt); err != nil {
			return nil, err
		} else {
			if kvs[pt], err = kevlar.New(absPtDir, kevlar.JsonExt); err != nil {
				return nil, err
			}
		}
	}

	p := compton.Page(pageTitle)
	p.RegisterStyles(compton_styles.Styles, "debug.css")

	pageStack := compton.FlexItems(p, direction.Column)
	p.Append(pageStack)

	pageStack.Append(compton.FICenter(p, compton.H1Text(pageTitle)))

	for _, pt := range localProducts {

		if !kvs[pt].Has(id) {
			continue
		}

		//summaryHeading := compton.DSTitle(p, compton_data.TypesTitles[pt.String()])

		ds := compton.DSLarge(p, compton_data.TypesTitles[pt.String()], false).BackgroundColor(color.Highlight)
		pageStack.Append(ds)

		//subtitleFspan := compton.Fspan(p, pt.String()).
		//	FontSize(size.Small).
		//	BackgroundColor(color.Background)
		ds.SetLabelText(pt.String())

		iframe := compton.IframeExpandHost(p, pt.String(), "/debug-data?id="+id+"&product-type="+pt.String())
		ds.Append(iframe)

	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	reduxProperties := vangogh_integration.ReduxProperties()
	slices.Sort(reduxProperties)

	rdx, err := redux.NewReader(reduxDir, reduxProperties...)
	if err != nil {
		return nil, err
	}

	//reduxHeading := compton.DSTitle(p, "Redux")

	reduxDs := compton.DSLarge(p, "Redux", false).
		BackgroundColor(color.Foreground).
		ForegroundColor(color.Background)
	pageStack.Append(reduxDs)

	reduxStack := compton.FlexItems(p, direction.Column)
	reduxStack.AddClass("redux-data")
	reduxDs.Append(compton.FICenter(p, reduxStack))

	for _, property := range reduxProperties {
		if values, ok := rdx.GetAllValues(property, id); ok {

			//propertyHeading := compton.Fspan(p, property).FontSize(size.Normal)

			open := !slices.Contains(vangogh_integration.LongTextProperties(), property) &&
				!slices.Contains(vangogh_integration.DehydratedImagesProperties(), property)

			ds := compton.DSSmall(p, property, open)

			ul := compton.Ul()
			for _, value := range values {
				ul.Append(compton.ListItemText(value))
			}

			ds.Append(ul)

			reduxStack.Append(ds)
		}
	}

	return p, nil
}
