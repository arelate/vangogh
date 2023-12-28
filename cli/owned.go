package cli

import (
	"fmt"
	"github.com/arelate/vangogh/cli/reductions"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/url"
)

const (
	ownedSection    = "owned"
	notOwnedSection = "not owned"
)

func OwnedHandler(u *url.URL) error {
	idSet, err := vangogh_local_data.IdSetFromUrl(u)
	if err != nil {
		return err
	}

	return Owned(idSet)
}

func Owned(idSet map[string]bool) error {

	oa := nod.Begin("checking ownership...")
	defer oa.End()

	propSet := map[string]bool{
		vangogh_local_data.TitleProperty:             true,
		vangogh_local_data.SlugProperty:              true,
		vangogh_local_data.IncludesGamesProperty:     true,
		vangogh_local_data.IsIncludedByGamesProperty: true,
		vangogh_local_data.OwnedProperty:             true,
	}

	rdx, err := vangogh_local_data.NewReduxReader(maps.Keys(propSet)...)
	if err != nil {
		return oa.EndWithError(err)
	}

	ownedSet, err := reductions.CheckOwnership(idSet, rdx)
	if err != nil {
		return oa.EndWithError(err)
	}

	ownSummary := make(map[string][]string)
	ownSummary[ownedSection] = make([]string, 0, len(ownedSet))
	for id := range ownedSet {
		if title, ok := rdx.GetFirstVal(vangogh_local_data.TitleProperty, id); ok {
			ownSummary[ownedSection] = append(ownSummary[ownedSection], fmt.Sprintf("%s %s", id, title))
		}
	}

	notOwned := make(map[string]bool)
	for id := range idSet {
		if !ownedSet[id] {
			notOwned[id] = true
		}
	}

	ownSummary[notOwnedSection] = make([]string, 0, len(notOwned))
	for id := range notOwned {
		if title, ok := rdx.GetFirstVal(vangogh_local_data.TitleProperty, id); ok {
			ownSummary[notOwnedSection] = append(ownSummary[notOwnedSection], fmt.Sprintf("%s %s", id, title))
		}
	}

	oa.EndWithSummary("ownership results:", ownSummary)

	return nil
}
