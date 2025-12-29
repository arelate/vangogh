package hltb_data

import (
	"time"

	"github.com/arelate/southern_light/hltb_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetRootPage() error {

	productType := vangogh_integration.HltbRootPage

	grpa := nod.Begin("getting %s...", productType)
	defer grpa.Done()

	rootPageDir, err := vangogh_integration.AbsProductTypeDir(productType)
	if err != nil {
		return err
	}

	kvRootPage, err := kevlar.New(rootPageDir, kevlar.HtmlExt)
	if err != nil {
		return err
	}

	rootPageId := productType.String()
	rootPageUrl := hltb_integration.RootUrl()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GetDataProperties()...)
	if err != nil {
		return err
	}

	ptId, err := vangogh_integration.ProductTypeId(productType, rootPageId)
	if err != nil {
		return err
	}

	if err = fetch.RequestSetValue(rootPageId, rootPageUrl, reqs.HltbRootPage(), kvRootPage); err != nil {

		if err = rdx.ReplaceValues(vangogh_integration.GetDataErrorMessageProperty, ptId, err.Error()); err != nil {
			return err
		}

		formattedNow := time.Now().UTC().Format(time.RFC3339)
		if err = rdx.ReplaceValues(vangogh_integration.GetDataErrorDateProperty, ptId, formattedNow); err != nil {
			return err
		}

		return nil
	}

	return nil
}
