package opencritic_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"maps"
)

func GetApiGame(openCriticGogIds map[string]string, force bool) error {

	gaga := nod.NewProgress("getting %s...", vangogh_integration.OpenCriticApiGame)
	defer gaga.Done()

	apiGameDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.OpenCriticApiGame)
	if err != nil {
		return err
	}

	kvApiGame, err := kevlar.New(apiGameDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gaga.TotalInt(len(openCriticGogIds))

	if err = fetch.Items(maps.Keys(openCriticGogIds), reqs.OpenCriticApiGame(), kvApiGame, gaga, force); err != nil {
		return err
	}

	return nil
}
