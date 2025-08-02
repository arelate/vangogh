package cli

import (
	"fmt"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/atomus"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"iter"
	"net/url"
	"os"
	"strings"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"maps"
)

const (
	atomFeedTitle   = "vangogh sync updates"
	atomEntryAuthor = "Vincent van Gogh"
	atomEntryTitle  = "Sync results"
)

func SummarizeHandler(u *url.URL) error {

	since, err := vangogh_integration.SinceFromUrl(u)
	if err != nil {
		return err
	}

	return Summarize(since)
}

func Summarize(since int64) error {

	sa := nod.Begin("summarizing updates...")
	defer sa.Done()

	summary := make(map[string][]string)

	for _, section := range vangogh_integration.UpdatesOrder {
		summary[section] = nil
	}

	// new products

	apiProductsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.ApiProducts)
	if err != nil {
		return err
	}

	kvApiProducts, err := kevlar.New(apiProductsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	newApiProducts := kvApiProducts.Since(since, kevlar.Create)

	for id := range newApiProducts {
		summary[vangogh_integration.UpdatesNewProducts] = append(summary[vangogh_integration.UpdatesNewProducts], id)
	}

	// released today

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir,
		vangogh_integration.TitleProperty,
		vangogh_integration.SteamAppIdProperty,
		vangogh_integration.GOGReleaseDateProperty,
		vangogh_integration.GlobalReleaseDateProperty,
		vangogh_integration.LastSyncUpdatesProperty)
	if err != nil {
		return err
	}

	rt, err := releasedToday(rdx)
	if err != nil {
		return err
	}

	for id := range rt {
		summary[vangogh_integration.UpdatesReleasedToday] = append(summary[vangogh_integration.UpdatesReleasedToday], id)
	}

	// updated installers

	updatedDetails, err := shared_data.GetDetailsUpdates(since)
	if err != nil {
		return err
	}

	for id := range updatedDetails {
		summary[vangogh_integration.UpdatesInstallers] = append(summary[vangogh_integration.UpdatesInstallers], id)
	}

	// updated news

	appNewsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamAppNews)
	if err != nil {
		return err
	}

	kvAppNews, err := kevlar.New(appNewsDir, kevlar.JsonExt)
	if err != nil {
		return nil
	}

	updatedAppNews := kvAppNews.Since(since, kevlar.Update)
	for steamAppId := range updatedAppNews {
		gogIds := rdx.Match(map[string][]string{vangogh_integration.SteamAppIdProperty: {steamAppId}}, redux.FullMatch)
		for gogId := range gogIds {
			summary[vangogh_integration.UpdatesSteamNews] = append(summary[vangogh_integration.UpdatesSteamNews], gogId)
		}
	}

	for section, ids := range summary {
		sortedIds, sortErr := rdx.Sort(ids, vangogh_integration.DefaultDesc, vangogh_integration.DefaultSort)
		if sortErr != nil {
			return sortErr
		}
		summary[section] = sortedIds
	}

	if err = rdx.BatchReplaceValues(vangogh_integration.LastSyncUpdatesProperty, summary); err != nil {
		return err
	}

	if err = publishAtom(rdx, summary); err != nil {
		return err
	}

	return nil
}

func releasedToday(rdx redux.Readable) (iter.Seq[string], error) {

	releaseProperties := []string{vangogh_integration.GOGReleaseDateProperty, vangogh_integration.GlobalReleaseDateProperty}

	if err := rdx.MustHave(releaseProperties...); err != nil {
		return nil, err
	}

	ids := make(map[string]any)
	today := time.Now().Format("2006.01.02")

	for _, property := range releaseProperties {
		for id := range rdx.Keys(property) {
			if rt, ok := rdx.GetLastVal(property, id); ok {
				if rt == today {
					ids[id] = nil
				}
			}
		}
	}

	return maps.Keys(ids), nil
}

func publishAtom(rdx redux.Readable, summary map[string][]string) error {

	paa := nod.Begin(" publishing atom...")
	defer paa.Done()

	afp, err := vangogh_integration.AbsAtomFeedPath()
	if err != nil {
		return err
	}

	atomFile, err := os.Create(afp)
	if err != nil {
		return err
	}

	af := atomus.NewFeed(atomFeedTitle, "")
	af.SetEntry(atomEntryTitle, atomEntryAuthor, "", NewAtomFeedContent(rdx, summary))

	return af.Encode(atomFile)
}

func NewAtomFeedContent(rdx redux.Readable, summary map[string][]string) string {
	sb := strings.Builder{}

	for _, section := range vangogh_integration.UpdatesOrder {
		if _, ok := summary[section]; !ok {
			continue
		}
		if len(summary[section]) == 0 {
			continue
		}
		sb.WriteString("<h1>" + vangogh_integration.UpdatesLongerTitles[section] + "</h1>")
		sb.WriteString("<ul>")
		for _, id := range summary[section] {
			if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
				sb.WriteString(fmt.Sprintf("<li>%s (%s)</li>", title, id))
			} else {
				sb.WriteString("<li>" + id + "</li>")
			}
		}
		sb.WriteString("</ul>")
	}
	return sb.String()
}
