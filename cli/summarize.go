package cli

import (
	"fmt"
	"github.com/boggydigital/atomus"
	"github.com/boggydigital/redux"
	"net/url"
	"os"
	"sort"
	"strings"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
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
	defer sa.End()

	updates, err := vangogh_integration.Updates(since)
	if err != nil {
		return sa.EndWithError(err)
	}

	if len(updates) == 0 {
		return nil
	}

	rdx, err := vangogh_integration.NewReduxWriter(
		vangogh_integration.LastSyncUpdatesProperty,
		vangogh_integration.TitleProperty,
		vangogh_integration.GOGReleaseDateProperty)
	if err != nil {
		return sa.EndWithError(err)
	}

	summary := make(map[string][]string)

	//set new values for each section
	for section, ids := range updates {
		sortedIds, err := rdx.Sort(maps.Keys(ids),
			vangogh_integration.DefaultDesc,
			vangogh_integration.DefaultSort)
		if err != nil {
			return sa.EndWithError(err)
		}
		summary[section] = sortedIds
	}

	//clean sections filled earlier that don't exist anymore
	for section := range rdx.Keys(vangogh_integration.LastSyncUpdatesProperty) {
		if _, ok := updates[section]; ok {
			continue
		}
		summary[section] = nil
	}

	if rt, err := releasedToday(rdx); err == nil {
		if len(rt) > 0 {
			summary["released today"] = rt
		}
	}

	if err := rdx.BatchReplaceValues(vangogh_integration.LastSyncUpdatesProperty, summary); err != nil {
		sa.EndWithError(err)
	}

	was := nod.Begin("publishing atom...")
	defer was.End()

	if err := publishAtom(rdx, summary); err != nil {
		return was.EndWithError(err)
	}

	was.EndWithResult("done")
	sa.EndWithResult("done")

	return nil
}

func releasedToday(rdx redux.Readable) ([]string, error) {

	if err := rdx.MustHave(vangogh_integration.GOGReleaseDateProperty); err != nil {
		return nil, err
	}

	ids := make([]string, 0)
	today := time.Now().Format("2006.01.02")

	for id := range rdx.Keys(vangogh_integration.GOGReleaseDateProperty) {
		if rt, ok := rdx.GetLastVal(vangogh_integration.GOGReleaseDateProperty, id); ok {
			if rt == today {
				ids = append(ids, id)
			}
		}
	}

	return rdx.Sort(ids, vangogh_integration.DefaultDesc, vangogh_integration.DefaultSort)
}

func publishAtom(rdx redux.Readable, summary map[string][]string) error {

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

	sections := maps.Keys(summary)
	sort.Strings(sections)

	for _, section := range sections {
		if len(summary[section]) == 0 {
			continue
		}
		sb.WriteString("<h1>" + section + "</h1>")
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
