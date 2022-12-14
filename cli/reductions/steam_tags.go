package reductions

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/match_node"
	"github.com/boggydigital/nod"
	"golang.org/x/net/html"
	"golang.org/x/net/html/atom"
	"strings"
)

// TODO: Convert into a property on a struct under steam_integration
func SteamTags(since int64) error {
	sta := nod.NewProgress(" %s...", vangogh_local_data.SteamTagsProperty)
	defer sta.End()

	rxa, err := vangogh_local_data.ConnectReduxAssets(
		vangogh_local_data.TitleProperty,
		vangogh_local_data.SteamTagsProperty)
	if err != nil {
		return sta.EndWithError(err)
	}

	vrSteamStorePage, err := vangogh_local_data.NewReader(vangogh_local_data.SteamStorePage)
	if err != nil {
		return sta.EndWithError(err)
	}

	vrCatalogProducts, err := vangogh_local_data.NewReader(vangogh_local_data.CatalogProducts)
	if err != nil {
		return sta.EndWithError(err)
	}

	steamTags := make(map[string][]string)
	modified := vrCatalogProducts.ModifiedAfter(since, false)

	sta.TotalInt(len(modified))

	for _, id := range modified {

		steamStorePage, err := vrSteamStorePage.SteamStorePage(id)
		if err != nil {
			sta.Error(err)
			sta.Increment()
			continue
		}

		if steamStorePage == nil {
			sta.Increment()
			continue
		}

		tagsContainer := match_node.Match(steamStorePage, &appTagMatcher{})

		if tagsContainer == nil {
			nod.Log("steam-tags reduction: tags container not found")
			sta.Increment()
			continue
		}

		steamTags[id] = extractTagsFromContainer(tagsContainer)
		sta.Increment()
	}

	if err := rxa.BatchReplaceValues(vangogh_local_data.SteamTagsProperty, steamTags); err != nil {
		return sta.EndWithError(err)
	}

	sta.EndWithResult("done")

	return nil
}

const (
	tagsContainerClassVariant1 = "glance_tags popular_tags"
	tagsContainerClassVariant2 = "popular_tags glance_tags"
)

type appTagMatcher struct{}

func (atm *appTagMatcher) Match(node *html.Node) bool {
	if node.Type != html.ElementNode ||
		node.DataAtom != atom.Div ||
		len(node.Attr) == 0 {
		return false
	}

	for _, a := range node.Attr {
		if a.Key == "class" &&
			(strings.Contains(a.Val, tagsContainerClassVariant1) ||
				strings.Contains(a.Val, tagsContainerClassVariant2)) {
			return true
		}
	}

	return false
}

func extractTagsFromContainer(node *html.Node) []string {
	tags := make([]string, 0)
	if node == nil ||
		node.FirstChild == nil {
		return tags
	}

	for ch := node.FirstChild; ch != nil; ch = ch.NextSibling {
		if ch.DataAtom != atom.A {
			continue
		}

		tags = append(tags, strings.Trim(ch.FirstChild.Data, "\n\t"))
	}

	return tags
}
