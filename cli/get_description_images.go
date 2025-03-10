package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/url"
)

func GetDescriptionImagesHandler(u *url.URL) error {
	since, err := vangogh_integration.SinceFromUrl(u)
	if err != nil {
		return nil
	}

	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	force := u.Query().Has("force")

	return GetDescriptionImages(ids, since, force)
}

func GetDescriptionImages(ids []string, since int64, force bool) error {

	gdia := nod.NewProgress("getting description images...")
	defer gdia.Done()

	rdx, err := vangogh_integration.NewReduxReader(
		vangogh_integration.TitleProperty,
		vangogh_integration.DescriptionOverviewProperty,
		vangogh_integration.DescriptionFeaturesProperty)
	if err != nil {
		return err
	}

	dc := dolo.DefaultClient

	apiProductsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.ApiProducts)
	if err != nil {
		return err
	}

	kvApiProducts, err := kevlar.New(apiProductsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if len(ids) == 0 {
		newApiProducts := kvApiProducts.Since(since, kevlar.Create, kevlar.Update)
		for id := range newApiProducts {
			ids = append(ids, id)
		}
	}

	gdia.TotalInt(len(ids))

	for _, id := range ids {

		var items []string

		descOverview, ok := rdx.GetLastVal(vangogh_integration.DescriptionOverviewProperty, id)
		if ok {
			items = vangogh_integration.ExtractDescItems(descOverview)
		}

		descFeatures, ok := rdx.GetLastVal(vangogh_integration.DescriptionFeaturesProperty, id)
		if ok {
			items = append(items, vangogh_integration.ExtractDescItems(descFeatures)...)
		}

		if len(items) < 1 {
			gdia.Increment()
			continue
		}

		for _, itemUrl := range items {
			if u, err := url.Parse(itemUrl); err == nil {
				dia := nod.NewProgress(" %s", u.Path)

				aip, err := vangogh_integration.AbsItemPath(u.Path)
				if err != nil {
					return err
				}

				if err = dc.Download(u, force, dia, aip); err != nil {
					return err
				}

				dia.Done()
			}
		}

		gdia.Increment()
	}

	return nil
}
