package cli

import (
	"fmt"
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

	return GetDescriptionImages(ids, since)
}

func GetDescriptionImages(
	ids []string,
	since int64) error {

	gdia := nod.NewProgress("getting description images...")
	defer gdia.Done()

	rdx, err := vangogh_integration.NewReduxReader(
		vangogh_integration.TitleProperty,
		vangogh_integration.DescriptionOverviewProperty,
		vangogh_integration.DescriptionFeaturesProperty)
	if err != nil {
		return err
	}

	dl := dolo.DefaultClient

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

		title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id)
		if !ok {
			gdia.Log("%s has no title", id)
			gdia.Increment()
			continue
		}

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

		dia := nod.NewProgress(" %s %s", id, title)
		dia.TotalInt(len(items))

		urls := make([]*url.URL, 0, len(items))
		filenames := make([]string, 0, len(items))

		for _, itemUrl := range items {
			if u, err := url.Parse(itemUrl); err == nil {
				urls = append(urls, u)
				aip, err := vangogh_integration.AbsItemPath(u.Path)
				if err != nil {
					return err
				}
				filenames = append(filenames, aip)
			}
		}

		if errs := dl.GetSet(urls, dolo.NewFileIndexSetter(filenames), dia, false); len(errs) > 0 {
			for ui, e := range errs {
				dia.Error(fmt.Errorf("GetSet %s error: %s", urls[ui], e.Error()))
			}
			continue
		}

		gdia.Increment()
		dia.Done()
	}

	return nil
}
