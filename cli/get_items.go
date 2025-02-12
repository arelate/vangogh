package cli

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/nod"
	"net/url"
)

func GetItemsHandler(u *url.URL) error {
	since, err := vangogh_integration.SinceFromUrl(u)
	if err != nil {
		return nil
	}

	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return GetItems(ids, since)
}

func GetItems(
	ids []string,
	since int64) error {

	gia := nod.NewProgress("getting description items...")
	defer gia.EndWithResult("done")

	rdx, err := vangogh_integration.NewReduxReader(
		vangogh_integration.TitleProperty,
		vangogh_integration.DescriptionOverviewProperty,
		vangogh_integration.DescriptionFeaturesProperty)
	if err != nil {
		return err
	}

	dl := dolo.DefaultClient

	all, err := itemizations.All(ids, false, true, since, vangogh_integration.ApiProductsV2)
	if err != nil {
		return err
	}

	gia.TotalInt(len(all))

	for _, id := range all {

		title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id)
		if !ok {
			gia.Log("%s has no title", id)
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
			gia.Increment()
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

		dia.EndWithResult("done")
		gia.Increment()
	}

	return nil
}
