package cli

import (
	"encoding/xml"
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
	"golang.org/x/exp/maps"
	"sort"
	"strconv"
	"strings"
	"time"
)

const (
	atomXMLNS       = "http://www.w3.org/2005/Atom"
	atomFeedTitle   = "vangogh sync updates"
	atomEntryName   = "Vincent van Gogh"
	atomContentType = "xhtml"
	atomEntryTitle  = "Sync results "
)

type AtomFeed struct {
	XMLName xml.Name `xml:"feed"` // <feed xmlns="http://www.w3.org/2005/Atom">
	XMLNS   string   `xml:"xmlns,attr"`
	Title   string   `xml:"title"` // <title>Example Feed</title>
	//<subtitle>A subtitle.</subtitle>
	//<link href="http://example.org/feed/" rel="self" />
	//<link href="http://example.org/" />
	Updated string     `xml:"updated"` // <updated>2003-12-13T18:30:02Z</updated>
	Entry   *AtomEntry `xml:"entry"`
}

type AtomEntry struct {
	Title string `xml:"title"` // <title>Atom-Powered Robots Run Amok</title>
	// <link href="http://example.org/2003/12/13/atom03" />
	// <link rel="alternate" type="text/html" href="http://example.org/2003/12/13/atom03.html"/>
	// <link rel="edit" href="http://example.org/2003/12/13/atom03/edit"/>
	Id        string            `xml:"id"`                // <id>urn:uuid:1225c695-cfb8-4ebb-aaaa-80da344efa6a</id>
	Published string            `xml:"published"`         // <published>2003-11-09T17:23:02Z</published>
	Updated   string            `xml:"updated"`           // <updated>2003-12-13T18:30:02Z</updated>
	Summary   string            `xml:"summary,omitempty"` // <summary>Some text.</summary>
	Author    *AtomEntryAuthor  `xml:"author"`
	Content   *AtomEntryContent `xml:"content"`
}

type AtomEntryContent struct {
	XMLName xml.Name `xml:"content"`
	Type    string   `xml:"type,attr"`
	Content string   `xml:",innerxml"`
}

type AtomEntryAuthor struct {
	XMLName xml.Name `xml:"author"`
	Name    string   `xml:"name"`            // <name>John Doe</name>
	Email   string   `xml:"email,omitempty"` // <email>johndoe@example.com</email>
}

func NewAtomFeed(rxa kvas.ReduxAssets, summary map[string][]string) *AtomFeed {
	updated := time.Now()
	return &AtomFeed{
		XMLNS:   atomXMLNS,
		Title:   atomFeedTitle,
		Updated: updated.Format(time.RFC3339),
		Entry: &AtomEntry{
			Id:        strconv.FormatInt(updated.Unix(), 10),
			Title:     atomEntryTitle + updated.Format(time.RFC1123),
			Published: updated.Format(time.RFC3339),
			Updated:   updated.Format(time.RFC3339),
			Author: &AtomEntryAuthor{
				Name: atomEntryName,
			},
			Content: &AtomEntryContent{
				Type:    atomContentType,
				Content: NewAtomFeedContent(rxa, summary),
			},
		},
	}
}

func NewAtomFeedContent(rxa kvas.ReduxAssets, summary map[string][]string) string {
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
			if title, ok := rxa.GetFirstVal(vangogh_local_data.TitleProperty, id); ok {
				sb.WriteString(fmt.Sprintf("<li>%s (%s)</li>", title, id))
			} else {
				sb.WriteString("<li>" + id + "</li>")
			}
		}
		sb.WriteString("</ul>")
	}
	return sb.String()
}
