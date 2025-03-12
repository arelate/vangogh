package hltb_data

import (
	"github.com/arelate/southern_light/hltb_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
)

func GetRootPage() error {

	grpa := nod.Begin("getting %s...", vangogh_integration.HltbRootPage)
	defer grpa.Done()

	rootPageDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.HltbRootPage)
	if err != nil {
		return err
	}

	kvRootPage, err := kevlar.New(rootPageDir, kevlar.HtmlExt)
	if err != nil {
		return err
	}

	rootPageId := vangogh_integration.HltbRootPage.String()
	rootPageUrl := hltb_integration.RootUrl()

	if err = fetch.RequestSetValue(rootPageId, rootPageUrl, reqs.HltbRootPage(), kvRootPage); err != nil {
		return err
	}

	return nil
}
