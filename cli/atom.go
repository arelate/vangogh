package cli

import (
	"encoding/xml"
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
	"strings"
	"time"
)

const (
	atomXMLNS     = "http://www.w3.org/2005/Atom"
	atomFeedTitle = "vangogh sync updates"
)

type AtomFeed struct { // <feed xmlns="http://www.w3.org/2005/Atom">
	XMLName xml.Name `xml:"feed"`
	XMLNS   string   `xml:"xmlns,attr"`
	Title   string   `xml:"title"` // <title>Example Feed</title>
	//<subtitle>A subtitle.</subtitle>
	//<link href="http://example.org/feed/" rel="self" />
	//<link href="http://example.org/" />
	//<id>urn:uuid:60a76c80-d399-11d9-b91C-0003939e0af6</id>
	Updated string     `xml:"updated"` // <updated>2003-12-13T18:30:02Z</updated>
	Entry   *AtomEntry `xml:"entry"`
} //</feed>

type AtomEntry struct {
	Title string `xml:"title"` // <title>Atom-Powered Robots Run Amok</title>
	// <link href="http://example.org/2003/12/13/atom03" />
	// <link rel="alternate" type="text/html" href="http://example.org/2003/12/13/atom03.html"/>
	// <link rel="edit" href="http://example.org/2003/12/13/atom03/edit"/>
	// <id>urn:uuid:1225c695-cfb8-4ebb-aaaa-80da344efa6a</id>
	Published string `xml:"published"` // <published>2003-11-09T17:23:02Z</published>
	// <updated>2003-12-13T18:30:02Z</updated>
	// <summary>Some text.</summary>
	Author  *AtomEntryAuthor  `xml:"author"`
	Content *AtomEntryContent `xml:"content"`
}

type AtomEntryContent struct {
	XMLName xml.Name `xml:"content"`
	Type    string   `xml:"type,attr"`
	Content string   `xml:",innerxml"`
}

type AtomEntryAuthor struct {
	XMLName xml.Name `xml:"author"`
	Name    string   `xml:"name"`  // <name>John Doe</name>
	Email   string   `xml:"email"` // <email>johndoe@example.com</email>
}

func NewAtomFeed(rxa kvas.ReduxAssets, summary map[string][]string) *AtomFeed {
	updated := time.Now()
	return &AtomFeed{
		XMLNS:   atomXMLNS,
		Title:   atomFeedTitle,
		Updated: updated.Format(time.RFC3339),
		Entry: &AtomEntry{
			Title:     fmt.Sprintf("Sync results %s", updated.Format(time.RFC1123)),
			Published: updated.Format(time.RFC3339),
			Author: &AtomEntryAuthor{
				Name:  "Vincent van Gogh",
				Email: "vg@arelate",
			},
			Content: &AtomEntryContent{
				Type:    "xhtml",
				Content: NewAtomFeedContent(rxa, summary),
			},
		},
	}
}

func NewAtomFeedContent(rxa kvas.ReduxAssets, summary map[string][]string) string {
	sb := strings.Builder{}
	for section := range summary {
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
