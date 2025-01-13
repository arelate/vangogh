package cli

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reductions"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/url"
	"slices"
)

const (
	ownedSection    = "owned"
	notOwnedSection = "not owned"
)

func OwnedHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return Owned(ids)
}

func Owned(ids []string) error {

	oa := nod.Begin("checking ownership...")
	defer oa.End()

	propSet := map[string]bool{
		vangogh_integration.TitleProperty:             true,
		vangogh_integration.SlugProperty:              true,
		vangogh_integration.IncludesGamesProperty:     true,
		vangogh_integration.IsIncludedByGamesProperty: true,
		vangogh_integration.OwnedProperty:             true,
	}

	rdx, err := vangogh_integration.NewReduxReader(maps.Keys(propSet)...)
	if err != nil {
		return oa.EndWithError(err)
	}

	owned, err := reductions.CheckOwnership(ids, rdx)
	if err != nil {
		return oa.EndWithError(err)
	}

	ownSummary := make(map[string][]string)
	ownSummary[ownedSection] = make([]string, 0, len(owned))
	for _, id := range owned {
		if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
			ownSummary[ownedSection] = append(ownSummary[ownedSection], fmt.Sprintf("%s %s", id, title))
		}
	}

	notOwned := make(map[string]bool)
	for _, id := range ids {
		if !slices.Contains(owned, id) {
			notOwned[id] = true
		}
	}

	ownSummary[notOwnedSection] = make([]string, 0, len(notOwned))
	for id := range notOwned {
		if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
			ownSummary[notOwnedSection] = append(ownSummary[notOwnedSection], fmt.Sprintf("%s %s", id, title))
		}
	}

	oa.EndWithSummary("ownership results:", ownSummary)

	return nil
}
