package pcgw_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"maps"
)

const (
	txtExt = ".txt"
)

func GetRaw(pcgwGogIds map[string][]string, force bool) error {

	gra := nod.NewProgress("getting %s...", vangogh_integration.PcgwRaw)
	defer gra.Done()

	rawDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwRaw)
	if err != nil {
		return err
	}

	kvRaw, err := kevlar.New(rawDir, txtExt)
	if err != nil {
		return err
	}

	gra.TotalInt(len(pcgwGogIds))

	if err = fetch.Items(maps.Keys(pcgwGogIds), reqs.PcgwRaw(), kvRaw, gra, force); err != nil {
		return err
	}

	return nil
}
