package cli

import (
	"fmt"
	"github.com/boggydigital/atomus"
	"github.com/boggydigital/kevlar"
	"net/url"
	"os"
	"sort"
	"strings"
	"time"

	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
)

const (
	atomFeedTitle   = "vangogh sync updates"
	atomEntryAuthor = "Vincent van Gogh"
	atomEntryTitle  = "Sync results"
)

func SummarizeHandler(u *url.URL) error {

	since, err := vangogh_local_data.SinceFromUrl(u)
	if err != nil {
		return err
	}

	gauginUrl := vangogh_local_data.ValueFromUrl(u, "gaugin-url")

	return Summarize(
		since,
		gauginUrl)
}

func Summarize(since int64, gauginUrl string) error {

	sa := nod.Begin("summarizing updates...")
	defer sa.End()

	updates, err := vangogh_local_data.Updates(since)
	if err != nil {
		return sa.EndWithError(err)
	}

	if len(updates) == 0 {
		return nil
	}

	rdx, err := vangogh_local_data.NewReduxWriter(
		vangogh_local_data.LastSyncUpdatesProperty,
		vangogh_local_data.TitleProperty,
		vangogh_local_data.GOGReleaseDateProperty)
	if err != nil {
		return sa.EndWithError(err)
	}

	summary := make(map[string][]string)

	//set new values for each section
	for section, ids := range updates {
		sortedIds, err := rdx.Sort(maps.Keys(ids),
			vangogh_local_data.DefaultDesc,
			vangogh_local_data.DefaultSort)
		if err != nil {
			return sa.EndWithError(err)
		}
		summary[section] = sortedIds
	}

	//clean sections filled earlier that don't exist anymore
	for _, section := range rdx.Keys(vangogh_local_data.LastSyncUpdatesProperty) {
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

	if err := rdx.BatchReplaceValues(vangogh_local_data.LastSyncUpdatesProperty, summary); err != nil {
		sa.EndWithError(err)
	}

	was := nod.Begin("publishing atom...")
	defer was.End()

	if err := publishAtom(gauginUrl, rdx, summary); err != nil {
		return was.EndWithError(err)
	}

	was.EndWithResult("done")
	sa.EndWithResult("done")

	return nil
}

func releasedToday(rdx kevlar.ReadableRedux) ([]string, error) {

	if err := rdx.MustHave(vangogh_local_data.GOGReleaseDateProperty); err != nil {
		return nil, err
	}

	ids := make([]string, 0)
	today := time.Now().Format("2006.01.02")

	for _, id := range rdx.Keys(vangogh_local_data.GOGReleaseDateProperty) {
		if rt, ok := rdx.GetLastVal(vangogh_local_data.GOGReleaseDateProperty, id); ok {
			if rt == today {
				ids = append(ids, id)
			}
		}
	}

	return rdx.Sort(ids, vangogh_local_data.DefaultDesc, vangogh_local_data.DefaultSort)
}

func publishAtom(gauginUrl string, rdx kevlar.ReadableRedux, summary map[string][]string) error {

	afp, err := vangogh_local_data.AbsAtomFeedPath()
	if err != nil {
		return err
	}

	atomFile, err := os.Create(afp)
	if err != nil {
		return err
	}

	af := atomus.NewFeed(atomFeedTitle, gauginUrl)
	af.SetEntry(atomEntryTitle, atomEntryAuthor, gauginUrl, NewAtomFeedContent(rdx, summary))

	return af.Encode(atomFile)
}

func NewAtomFeedContent(rdx kevlar.ReadableRedux, summary map[string][]string) string {
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
			if title, ok := rdx.GetLastVal(vangogh_local_data.TitleProperty, id); ok {
				sb.WriteString(fmt.Sprintf("<li>%s (%s)</li>", title, id))
			} else {
				sb.WriteString("<li>" + id + "</li>")
			}
		}
		sb.WriteString("</ul>")
	}
	return sb.String()
}
